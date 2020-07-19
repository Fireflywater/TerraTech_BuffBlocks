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
        {
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
            }
        }

        public void UpdateObject(List<object> tgtPool)
        {
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
            }
        }

        public void CleanObject(object obj)
        {
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
            this.effectMemory.Remove(obj);
        }

    }
}
