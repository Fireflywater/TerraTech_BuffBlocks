using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class BuffSegment
    {
        public Tank tank;
        public BuffController controller;

        public string effectType;
        public string[] effectPaths;

        public Dictionary<ModuleBuff, int> effectBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<object, List<float>> effectMemory = new Dictionary<object, List<float>>();
        

        public void ManipulateObj(List<object> tgtPool, string request)
        {
            foreach (object tgt in tgtPool)
            {
                if (request == "SAVE")
                {
                    this.effectMemory.Add(tgt, new List<float>());
                }
                
                for (int i = 0; i < this.effectPaths.Length; i++)
                {
                    List<string> splitPath = this.effectPaths[i].Split('.').ToList();

                    List<object> lastIterObjs = null;
                    List<object> thisIterObjs = new List<object> { tgt };

                    FieldInfo field_lastIter = null;
                    FieldInfo field_thisIter = null;

                    object structWarningObj = null;
                    object structWarningParent = null;
                    FieldInfo structWarningField = null;

                    foreach (string e in splitPath)
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
                            if (request == "SAVE")
                            {
                                this.effectMemory[tgt].Add((float)field_thisIter.GetValue(ara));
                            }
                            else if (request == "UPDATE")
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[tgt][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
                            }
                            else if (request == "CLEAN")
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[tgt][i]);
                            }
                        }
                    }
                    if (structWarningObj != null)
                    {
                        structWarningField.SetValue(structWarningParent, structWarningObj);
                    }
                }
                if (request == "CLEAN")
                {
                    this.effectMemory.Remove(tgt);
                }
            }
        }

        public float GetBuffAverage()
        {
            float m = 1.0f;
            List<float> allMults = (List<float>)this.effectBuffBlocks.Select(x => x.Key.Strength(x.Key, x.Value)).ToList();
            if (allMults.Count > 0)
            {
                m = allMults.Average();
            }
            return m;
        }

        public float GetBuffAddAverage()
        {
            float a = 0.0f;
            List<float> allAdds = (List<float>)this.effectBuffBlocks.Select(x => x.Key.AddAfter(x.Key, x.Value)).ToList();
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }
            return a;
        }

        public float GetAveragesByKey(object x, int i)
        {
            return this.effectMemory[x][i] * GetBuffAverage() + GetBuffAddAverage();
        }

        public void AddBuff(ModuleBuff buff)
        {
            this.effectBuffBlocks.Add(buff, buff.GetEffect(this.effectType));
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            this.effectBuffBlocks.Remove(buff);
        }
    }
}
