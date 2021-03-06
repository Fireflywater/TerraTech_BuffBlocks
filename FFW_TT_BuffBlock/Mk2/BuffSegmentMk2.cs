﻿using System;
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
                Console.WriteLine("FFW! Manipulate Obj " + request + "... 1 ");
                /*if (request == "SAVE" && this.effectMemory.ContainsKey(block))
                {
                    Console.WriteLine("Aborting " + request + "! effectMemory already contains " + block.name);
                    return;
                }
                if ((request == "UPDATE" || request == "CLEAN") && !this.effectMemory.ContainsKey(block))
                {
                    Console.WriteLine("Aborting " + request + "! effectMemory doesn't contain " + block.name);
                    return;
                }*/
                object tgt = block.GetComponent(effectComponent);
                if (tgt == null)
                {
                    break;
                }
                if (request == "SAVE")
                {
                    //Console.Write("2 ");
                    this.effectMemory.Add(block, 1.0f);
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
                    //Console.Write("3 ");
                    field_lastIter = field_thisIter;
                    lastIterObjs = new List<object>(thisIterObjs);
                    thisIterObjs = new List<object>();
                    foreach (object obj in lastIterObjs)
                    {
                        //Console.Write("4 ");
                        field_thisIter = obj.GetType().GetField(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field_thisIter != null)
                        {
                            //Console.Write("5 ");
                            object value_thisIter = field_thisIter.GetValue(obj);
                            if (value_thisIter != null)
                            {
                                var arrayTest = value_thisIter as Array;
                                var listTest = value_thisIter as System.Collections.IList;
                                Boolean isStruct = value_thisIter.GetType().IsValueType && !value_thisIter.GetType().IsPrimitive;
                                if (isStruct)
                                {
                                    //Console.Write("6 ");
                                    structWarningObj = value_thisIter;
                                    structWarningParent = obj;
                                    structWarningField = field_thisIter;
                                    thisIterObjs.Add(structWarningObj);
                                }
                                else if (arrayTest != null)
                                {
                                    //Console.Write("7 ");
                                    Array value_thisIterCasted = (Array)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else if (listTest != null)
                                {
                                    //Console.Write("8 ");
                                    System.Collections.IList value_thisIterCasted = (System.Collections.IList)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else
                                {
                                    //Console.Write("9 ");
                                    thisIterObjs.Add(value_thisIter);
                                }
                            }
                        }
                    }
                }

                //Console.Write("10 ");
                foreach (object ara in lastIterObjs)
                {
                    //Console.Write("11 ");
                    if (field_thisIter != null)
                    {
                        //Console.Write("12 ");
                        object value_thisIter = field_thisIter.GetValue(ara);
                        if (request == "SAVE")
                        {
                            //Console.Write("13 ");
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                //this.effectMemory.Add(block, (float)value_thisIter);
                                this.effectMemory[block] = (float)value_thisIter;
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                //this.effectMemory.Add(block, Convert.ToSingle((int)value_thisIter));
                                this.effectMemory[block] = Convert.ToSingle((int)value_thisIter);
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                //this.effectMemory.Add(block, Convert.ToSingle((bool)value_thisIter));
                                this.effectMemory[block] = Convert.ToSingle((bool)value_thisIter);
                            }
                        }
                        else if (request == "UPDATE")
                        {
                            //Console.Write("14 ");
                            //Console.WriteLine("FFW! Update From " + field_thisIter.GetValue(ara));
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name));
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                field_thisIter.SetValue(ara, Convert.ToInt32(Math.Ceiling(this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name))));
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                field_thisIter.SetValue(ara, Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block] * this.GetBuffAverage(block.name) + this.GetBuffAddAverage(block.name), 0.0f, 1.0f))));
                            }
                            //Console.WriteLine("FFW! Update To " + field_thisIter.GetValue(ara));
                        }
                        else if (request == "CLEAN")
                        {
                            //Console.Write("15 ");
                            //Console.WriteLine("FFW! Clean From " + field_thisIter.GetValue(ara));
                            if (value_thisIter.GetType() == typeof(float))
                            {
                                field_thisIter.SetValue(ara, this.effectMemory[block]);
                            }
                            else if (value_thisIter.GetType() == typeof(int))
                            {
                                field_thisIter.SetValue(ara, Convert.ToInt32(Math.Ceiling(this.effectMemory[block])));
                            }
                            else if (value_thisIter.GetType() == typeof(bool))
                            {
                                field_thisIter.SetValue(ara, Convert.ToBoolean(Math.Round(BuffControllerMk2.Clamp(this.effectMemory[block], 0.0f, 1.0f))));
                            }
                            //Console.WriteLine("FFW! Clean To " + field_thisIter.GetValue(ara));
                        }
                    }
                }
                //Console.Write("16 ");
                if (structWarningObj != null)
                {
                    //Console.Write("17 ");
                    structWarningField.SetValue(structWarningParent, structWarningObj);
                }
                //Console.Write("18 ");
                if (request == "CLEAN")
                {
                    //Console.Write("19 ");
                    this.effectMemory.Remove(block);
                }
            }
            if (request == "SAVE")
            {
                //Console.Write("20 ");
                this.ManipulateObj(blockPool, "UPDATE");
            }
        }

        public float GetBuffAverage(string name)
        {
            float m = 1.0f;
            List<float> allMults = (List<float>)this.effectBuffBlocks.Select(x => x.Key.Strength(x.Value)).ToList();
            /*List<float> allMults = new List<float>();
            foreach (KeyValuePair<ModuleBuffMk2, int> buffKvp in this.effectBuffBlocks)
            {
                if (
                    (buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "white"))
                    ||
                    (!buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "black"))
                )
                {
                    allMults.Add(buffKvp.Key.Strength(buffKvp.Value));
                }
            }*/
            if (allMults.Count > 0)
            {
                m = allMults.Average();
            }
            return m;
        }

        public float GetBuffAddAverage(string name)
        {
            float a = 0.0f;
            List<float> allAdds = (List<float>)this.effectBuffBlocks.Select(x => x.Key.AddAfter(x.Value)).ToList();
            /*List<float> allAdds = new List<float>();
            foreach (KeyValuePair<ModuleBuffMk2, int> buffKvp in this.effectBuffBlocks)
            {
                //Console.WriteLine("FFW! Find me! ");
                foreach (string x in buffKvp.Key.m_AffectedBlockList)
                {
                    /*foreach (string y in x)
                    {
                        Console.Write(y);
                    }//
                    Console.Write(x);

                }
                if (
                    (buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "white"))
                    ||
                    (!buffKvp.Key.m_AffectedBlockList[buffKvp.Value].Contains(name) && (buffKvp.Key.m_AffectedBlockListType[buffKvp.Value] == "black"))
                )
                {
                    allAdds.Add(buffKvp.Key.Strength(buffKvp.Value));
                }
            }*/
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }
            return a;
        }
        
        public float GetAverages(string name)
        {
            return GetBuffAverage(name) + GetBuffAddAverage(name);
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
