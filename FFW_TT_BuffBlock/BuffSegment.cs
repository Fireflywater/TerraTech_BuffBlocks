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
            List<float> allMults = (List<float>)effectBuffBlocks.Select(x => x.Key.Strength(x.Key, x.Value)).ToList();
            if (allMults.Count > 0)
            {
                m = allMults.Average();
            }

            return (effectBuffBlocks.Count > 0) ? m : 1.0f;
        }

        public float GetBuffAddAverage()
        {
            float a = 0.0f;
            List<float> allAdds = (List<float>)effectBuffBlocks.Select(x => x.Key.AddAfter(x.Key, x.Value)).ToList();
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }

            return (effectBuffBlocks.Count > 0) ? a : 0.0f;
        }

        public object[] GetObjAndField(object x, string path) // [0] = OBJECT, [1] = FIELDINFO
        {
            Console.WriteLine("FFW! Launch!");
            List<string> splitPath = path.Split('.').ToList();
            object tgtObjPre = null;
            object tgtObj = x;
            FieldInfo tgtStat = null;
            foreach(string e in splitPath)
            {
                tgtStat = tgtObj.GetType()
                    .GetField(e, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                tgtObjPre = tgtObj;
                tgtObj = tgtStat.GetValue(tgtObj);
                Console.WriteLine("FFW! 1, " + tgtObjPre.GetType().Name + "." + e + " = " + tgtObj.GetType().Name);
            }
            Console.WriteLine("FFW! FFW!");
            if ((tgtStat != null) && (tgtStat.GetValue(tgtObjPre) != null))
            {
                Console.WriteLine("FFW! " + path + " = " + tgtStat.GetValue(tgtObjPre));
                return new object[] { tgtObjPre, tgtStat };
            }
            return new object[] { null, null };
        }

        public void AddBuff(ModuleBuff buff)
        {
            effectBuffBlocks.Add(buff, buff.GetEffect(effectType));
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            effectBuffBlocks.Remove(buff);
        }

        public void SaveObject(object obj)
        {
            this.effectMemory.Add(obj, new List<float>());
            foreach (string path in this.effectPaths)
            {
                object[] y = this.GetObjAndField(obj, path);
                if ((y[0] != null) && (y[1] != null))
                {
                    FieldInfo z = (FieldInfo)y[1];
                    Console.WriteLine("FFW! GET! " + z.GetValue(y[0]));
                    this.effectMemory[obj].Add((float)z.GetValue(y[0]));
                    Console.WriteLine("FFW! SAVED!");
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
                    object[] y = this.GetObjAndField(element, path);
                    if ((y[0] != null) && (y[1] != null))
                    {
                        FieldInfo z = (FieldInfo)y[1];
                        Console.WriteLine("FFW! GET! " + z.GetValue(y[0]));
                        z.SetValue(y[0], this.effectMemory[element][i] * this.GetBuffAverage() + this.GetBuffAddAverage() );
                        Console.WriteLine("FFW! SET! " + z.GetValue(y[0]));
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
                object[] y = this.GetObjAndField(obj, path);
                if ((y[0] != null) && (y[1] != null))
                {
                    FieldInfo z = (FieldInfo)y[1];
                    Console.WriteLine("FFW! GET! " + z.GetValue(y[0]));
                    z.SetValue(y[0], this.effectMemory[obj][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
                    Console.WriteLine("FFW! SET! " + z.GetValue(y[0]));
                }
                i++;
            }
            this.effectMemory.Remove(obj);
        }

    }
}
