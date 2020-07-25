using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class BuffSegmentMk2
    {
        public Tank tank;
        public BuffControllerMk2 controller;

        public Type effectComponent;
        public List<string> effectPath;

        public Dictionary<ModuleBuffMk2, int> effectBuffBlocks = new Dictionary<ModuleBuffMk2, int>();
        public Dictionary<object, float> effectMemory = new Dictionary<object, float>();

        public void ManipulateObj(List<TankBlock> blockPool, string request)
        {
            foreach (TankBlock block in blockPool)
            {
                object tgt = block.GetComponent(effectComponent);
                if (tgt == null)
                {
                    break;
                }

                List<object> lastIterObjs = null;
                List<object> thisIterObjs = new List<object> { tgt };

                FieldInfo field_lastIter = null;
                FieldInfo field_thisIter = null;

                object structWarningObj = null;
                object structWarningParent = null;
                FieldInfo structWarningField = null;

                foreach (string e in this.effectPath)
                {
                    field_lastIter = field_thisIter;
                    lastIterObjs = new List<object>(thisIterObjs);
                    thisIterObjs = new List<object>();
                    foreach (object obj in lastIterObjs)
                    {
                        field_thisIter = obj.GetType().GetField(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field_thisIter != null)
                        {
                            object value_thisIter = field_thisIter.GetValue(obj);
                            var arrayTest = value_thisIter as Array;
                            var listTest = value_thisIter as System.Collections.IList;
                            Boolean isStruct = value_thisIter.GetType().IsValueType && !value_thisIter.GetType().IsPrimitive;
                            if (isStruct)
                            {
                                structWarningObj = value_thisIter;
                                structWarningParent = obj;
                                structWarningField = field_thisIter;
                                thisIterObjs.Add(structWarningObj);
                            }
                            else if (arrayTest != null)
                            {
                                Array value_thisIterCasted = (Array)value_thisIter;
                                foreach (object element in value_thisIterCasted)
                                {
                                    thisIterObjs.Add(element);
                                }
                            }
                            else if (listTest != null)
                            {
                                System.Collections.IList value_thisIterCasted = (System.Collections.IList)value_thisIter;
                                foreach (object element in value_thisIterCasted)
                                {
                                    thisIterObjs.Add(element);
                                }
                            }
                            else
                            {
                                thisIterObjs.Add(value_thisIter);
                            }
                        }
                    }
                }

                foreach (object ara in lastIterObjs)
                {
                    if (field_thisIter != null)
                    {
                        object value_thisIter = field_thisIter.GetValue(ara);
                        if (request == "SAVE")
                        {
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                this.effectMemory.Add(block, (float)value_thisIter);
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                this.effectMemory.Add(block, Convert.ToSingle((int)value_thisIter));
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                this.effectMemory.Add(block, Convert.ToSingle((bool)value_thisIter));
                            }
                        }
                        else if (request == "UPDATE")
                        {
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[block] * this.GetBuffAverage() + this.GetBuffAddAverage());
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                field_thisIter.SetValue(ara, Convert.ToInt32(Math.Ceiling(this.effectMemory[block] * this.GetBuffAverage() + this.GetBuffAddAverage())));
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                field_thisIter.SetValue(ara, Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block] * this.GetBuffAverage() + this.GetBuffAddAverage(), 0.0f, 1.0f))));
                            }
                        }
                        else if (request == "CLEAN")
                        {
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[block]);
                                this.effectMemory.Remove(block);
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                field_thisIter.SetValue(ara, Convert.ToInt32(Math.Ceiling(this.effectMemory[block])));
                                this.effectMemory.Remove(block);
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                field_thisIter.SetValue(ara, Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block], 0.0f, 1.0f))));
                                this.effectMemory.Remove(block);
                            }
                        }
                    }
                }
                if (structWarningObj != null)
                {
                    structWarningField.SetValue(structWarningParent, structWarningObj);
                }
            }
            if (request == "SAVE")
            {
                this.ManipulateObj(blockPool, "UPDATE");
            }
        }

        public float GetBuffAverage()
        {
            float m = 1.0f;
            List<float> allMults = (List<float>)this.effectBuffBlocks.Select(x => x.Key.Strength(x.Value)).ToList();
            if (allMults.Count > 0)
            {
                m = allMults.Average();
            }
            return m;
        }

        public float GetBuffAddAverage()
        {
            float a = 0.0f;
            List<float> allAdds = (List<float>)this.effectBuffBlocks.Select(x => x.Key.AddAfter(x.Value)).ToList();
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }
            return a;
        }
        
        public float GetAverages()
        {
            return GetBuffAverage() + GetBuffAddAverage();
        }

        public void AddBuff(ModuleBuffMk2 buff, int i)
        {
            this.effectBuffBlocks.Add(buff, i);
        }

        public void RemoveBuff(ModuleBuffMk2 buff)
        {
            this.effectBuffBlocks.Remove(buff);
        }

    }
}
