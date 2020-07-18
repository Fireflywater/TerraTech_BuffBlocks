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
            Console.WriteLine("FFW: MULTs: " + allMults.Count + " " + m);
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
            Console.WriteLine("FFW: ADDs: " + allAdds.Count + " " + a);
            return a;
        }

        public KeyValuePair<FieldInfo, List<object>>? GetObjAndField(object x, string path) // [0] = OBJECT, [1] = FIELDINFO
        {
            List<string> splitPath = path.Split('.').ToList();

            List<object> lastIterObjs = null;
            List<object> thisIterObjs = new List<object> { x };

            FieldInfo field_lastIter = null;
            FieldInfo field_thisIter = null;

            Console.WriteLine("FFW! 01");
            foreach (string e in splitPath)
            {
                Console.WriteLine("FFW! 02");
                field_lastIter = field_thisIter;
                Console.WriteLine("FFW! 03");
                lastIterObjs = new List<object>(thisIterObjs);
                Console.WriteLine("FFW! 04");
                thisIterObjs = new List<object>();
                Console.WriteLine("FFW! 05");
                foreach (object obj in lastIterObjs)
                {
                    Console.WriteLine("FFW! 06");
                    field_thisIter = obj.GetType().GetField(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Console.WriteLine("FFW! 07");
                    object value_thisIter = field_thisIter.GetValue(obj);
                    Console.WriteLine("FFW! 08");
                    if (value_thisIter.GetType() == typeof(Array))
                    {
                        Console.WriteLine("FFW! 09 array");
                        Array value_thisIterCasted = (Array)value_thisIter;
                        Console.WriteLine("FFW! 10 array");
                        foreach (object element in value_thisIterCasted)
                        {
                            Console.WriteLine("FFW! 11 array");
                            thisIterObjs.Add(element);
                        }
                    }
                    else if (value_thisIter.GetType() == typeof(List<HoverJet>))
                    {
                        Console.WriteLine("FFW! 09 list");
                        List<HoverJet> value_thisIterCasted = (List<HoverJet>)value_thisIter;
                        Console.WriteLine("FFW! 10 list");
                        foreach (HoverJet element in value_thisIterCasted)
                        {
                            Console.WriteLine("FFW! 11 list");
                            thisIterObjs.Add(element);
                        }
                    }
                    else
                    {
                        Console.WriteLine("FFW! 09 else");
                        thisIterObjs.Add(value_thisIter);
                    }
                }
            }

            Console.WriteLine("FFW! 21");
            if (field_thisIter != null)
            {
                Console.WriteLine("FFW! 22");
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
