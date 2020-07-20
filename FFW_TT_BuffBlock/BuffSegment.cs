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

        public List<List<object>> mapObjs = new List<List<object>>();
        public List<List<string>> mapObjsSpecials = new List<List<string>>();
        public List<List<FieldInfo>> mapFields = new List<List<FieldInfo>>();


        public void MapParams(object sample)
        {
            foreach (string path in effectPaths)
            {
                List<string> splitPath = path.Split('.').ToList();

                List<object> thisIterMapObjs = new List<object>();
                List<string> thisIterMapObjsSpecials = new List<string>();
                List<FieldInfo> thisIterMapFields = new List<FieldInfo>();

                object lastIterObj = null;
                object thisIterObj = sample;

                FieldInfo field_lastIter = null;
                FieldInfo field_thisIter = null;

                thisIterMapObjs.Add(sample);
                thisIterMapObjsSpecials.Add("normal"); // Assuming no module is a struct

                foreach (string e in splitPath)
                {
                    field_lastIter = field_thisIter;
                    lastIterObj = thisIterObj;
                    thisIterObj = null;

                    if (lastIterObj != null)
                    {
                        field_thisIter = lastIterObj.GetType().GetField(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                        if (field_thisIter != null)
                        {
                            thisIterMapFields.Add(field_thisIter);
                            object value_thisIter = field_thisIter.GetValue(lastIterObj);
                            var arrayTest = value_thisIter as Array;
                            var listTest = value_thisIter as System.Collections.IList;
                            Boolean isStruct = value_thisIter.GetType().IsValueType && !value_thisIter.GetType().IsPrimitive;
                            if (isStruct)
                            {
                                thisIterObj = value_thisIter;
                                thisIterMapObjs.Add(value_thisIter);
                                thisIterMapObjsSpecials.Add("struct");
                            }
                            else if (arrayTest != null)
                            {
                                Array value_thisIterCasted = (Array)value_thisIter;
                                foreach (object element in value_thisIterCasted)
                                {
                                    thisIterObj = element;
                                    thisIterMapObjs.Add(element);
                                    thisIterMapObjsSpecials.Add("array");
                                    break;
                                }
                            }
                            else if (listTest != null)
                            {
                                System.Collections.IList value_thisIterCasted = (System.Collections.IList)value_thisIter;
                                foreach (object element in value_thisIterCasted)
                                {
                                    thisIterObj = element;
                                    thisIterMapObjs.Add(element);
                                    thisIterMapObjsSpecials.Add("list");
                                    break;
                                }
                            }
                            else
                            {
                                thisIterObj = value_thisIter;
                                thisIterMapObjs.Add(value_thisIter);
                                thisIterMapObjsSpecials.Add("normal");
                            }
                        }
                    }
                }
                string thisIterMapObjsString = "";
                string thisIterMapObjsStructsString = "";
                string thisIterMapFieldsString = "";
                for (int i = 0; i < thisIterMapObjs.Count; i++)
                {
                    thisIterMapObjsString += " . " + thisIterMapObjs[i].GetType().Name;
                    thisIterMapObjsStructsString += " . " + thisIterMapObjsSpecials[i];
                }

                for (int i = 0; i < thisIterMapFields.Count; i++)
                {
                    thisIterMapFieldsString += " . " + thisIterMapFields[i].Name;
                }
                Console.WriteLine("FFW! path ... " + sample.GetType().Name + " . " + path);

                Console.WriteLine("FFW! mapObjs ... " + thisIterMapObjsString);
                Console.WriteLine("FFW! Structs ... " + thisIterMapObjsStructsString);
                Console.WriteLine("FFW! Fields  ... " + thisIterMapFieldsString);

                mapObjs.Add(thisIterMapObjs);
                mapObjsSpecials.Add(thisIterMapObjsSpecials);
                mapFields.Add(thisIterMapFields);
            }
        }

        public void ManipulateObj(List<object> tgtPool, string request)
        {
            Console.WriteLine("FFW! ! 1");
            foreach (object tgt in tgtPool)
            {
                if (this.mapObjs.Count == 0)
                {
                    this.MapParams(tgt);
                }
                
                if (request == "SAVE")
                {
                    Console.WriteLine("FFW! ! 3");
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
                                Console.WriteLine("FFW! ! 3 Save");
                                this.effectMemory[tgt].Add((float)field_thisIter.GetValue(ara));
                            }
                            else if (request == "UPDATE")
                            {
                                Console.WriteLine("FFW! ! 3 Update");
                                field_thisIter.SetValue(ara, this.effectMemory[tgt][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
                            }
                            else if (request == "CLEAN")
                            {
                                Console.WriteLine("FFW! ! 3 Clean");
                                field_thisIter.SetValue(ara, this.effectMemory[tgt][i]);
                            }
                        }
                    }
                    if (structWarningObj != null)
                    {
                        structWarningField.SetValue(structWarningParent, structWarningObj);
                    }

                    /*Console.WriteLine("FFW! ! 5");
                    List<object> lastIterObjs = null;
                    Console.WriteLine("FFW! ! 6");
                    List<object> thisIterObjs = new List<object> { tgt };
                    Console.WriteLine("FFW! ! 7");
                    for (int j = 0; j < this.mapObjs[i].Count; j++)
                    {
                        Console.WriteLine("FFW! ! 8");
                        lastIterObjs = new List<object>(thisIterObjs);
                        Console.WriteLine("FFW! ! 9");
                        thisIterObjs = new List<object>();
                        Console.WriteLine("FFW! ! 10");
                        foreach (object obj in lastIterObjs)
                        {
                            Console.WriteLine("FFW! ! 11");
                            if (j < this.mapFields[i].Count)
                            {
                                Console.WriteLine("FFW! Check! " + this.mapFields[i][j].GetValue(obj));
                                Console.WriteLine("FFW! ! 12");
                                object value_thisIter = this.mapFields[i][j].GetValue(obj);
                                Console.WriteLine("FFW! ! 13");
                                if (this.mapObjsSpecials[i][j] == "array")
                                {
                                    Console.WriteLine("FFW! ! 14 array");
                                    Array value_thisIterCasted = (Array)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        Console.WriteLine("FFW! ! 15 array");
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else if (this.mapObjsSpecials[i][j] == "list")
                                {
                                    Console.WriteLine("FFW! ! 14 list");
                                    System.Collections.IList value_thisIterCasted = (System.Collections.IList)value_thisIter;
                                    foreach (object element in value_thisIterCasted)
                                    {
                                        Console.WriteLine("FFW! ! 15 list");
                                        thisIterObjs.Add(element);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("FFW! ! 14 normal");
                                    thisIterObjs.Add(value_thisIter);
                                }
                            }
                        }
                    }
                    foreach (object obj2 in lastIterObjs)
                    {
                        Console.WriteLine("FFW! ! 21 . " + obj2.GetType().Name);
                        float test = (float)this.mapFields[i].LastOrDefault().GetValue(obj2);
                        Console.WriteLine("FFW! ! 22");
                        Console.WriteLine("FFW! " + this.effectPaths[i] + " = " + test);
                    }*/
                }
                if (request == "CLEAN")
                {
                    Console.WriteLine("FFW! ! 5 CLEAN");
                    this.effectMemory.Remove(tgt);
                }

                /*int i = 0;
                foreach (string path in this.effectPaths)
                {
                    KeyValuePair<FieldInfo, List<object>>? yNull = this.GetObjAndField(obj, path);
                    if (yNull != null)
                    {
                        KeyValuePair<FieldInfo, List<object>> y = (KeyValuePair<FieldInfo, List<object>>)yNull;
                        FieldInfo z = (FieldInfo)y.Key;
                        foreach (object ara in y.Value)
                        {
                            z.SetValue(ara, this.effectMemory[obj][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
                        }
                    }
                    i++;
                }*/
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

        public KeyValuePair<FieldInfo, List<object>>? GetObjAndField(object x, string path) // KEY = FIELDINFO, VALUE = LIST of OBJECTS
        {
            if (this.mapObjs.Count == 0)
            {
                this.MapParams(x);
            }

            List<string> splitPath = path.Split('.').ToList();

            List<object> lastIterObjs = null;
            List<object> thisIterObjs = new List<object> { x };

            FieldInfo field_lastIter = null;
            FieldInfo field_thisIter = null;
            
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
                        if (arrayTest != null)
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
            
            if (field_thisIter != null)
            {
                return new KeyValuePair<FieldInfo, List<object>>(field_thisIter, lastIterObjs );
            }
            return null;
        }

        public void AddBuff(ModuleBuff buff)
        {
            this.effectBuffBlocks.Add(buff, buff.GetEffect(this.effectType));
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            this.effectBuffBlocks.Remove(buff);
        }

        public void SaveObject(object obj)
        {/*
            this.effectMemory.Add(obj, new List<float>());
            foreach (string path in this.effectPaths)
            {
                KeyValuePair<FieldInfo, List<object>>? yNull = this.GetObjAndField(obj, path);
                if (yNull != null)
                {
                    KeyValuePair<FieldInfo, List<object>> y = (KeyValuePair<FieldInfo, List<object>>)yNull;
                    FieldInfo z = (FieldInfo)y.Key;
                    foreach (object ara in y.Value)
                    {
                        this.effectMemory[obj].Add((float)z.GetValue(ara));
                    }
                }
            }*/
        }

        public void UpdateObject(List<object> tgtPool)
        { /*
            foreach (object element in tgtPool)
            {
                int i = 0;
                foreach (string path in this.effectPaths)
                {
                    KeyValuePair<FieldInfo, List<object>>? yNull = this.GetObjAndField(element, path);
                    if (yNull != null)
                    {
                        KeyValuePair<FieldInfo, List<object>> y = (KeyValuePair<FieldInfo, List<object>>)yNull;
                        FieldInfo z = (FieldInfo)y.Key;
                        foreach (object ara in y.Value)
                        {
                            z.SetValue(ara, this.effectMemory[element][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
                        }
                    }
                    i++;
                }
            }*/
        }

        public void CleanObject(object obj)
        { /*
            int i = 0;
            foreach (string path in this.effectPaths)
            {
                KeyValuePair<FieldInfo, List<object>>? yNull = this.GetObjAndField(obj, path);
                if (yNull != null)
                {
                    KeyValuePair<FieldInfo, List<object>> y = (KeyValuePair<FieldInfo, List<object>>)yNull;
                    FieldInfo z = (FieldInfo)y.Key;
                    foreach (object ara in y.Value)
                    {
                        z.SetValue(ara, this.effectMemory[obj][i]);
                    }
                }
                i++;
            }
            this.effectMemory.Remove(obj);*/
        }

    }
}
