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

        public object[] GetObjAndField(object x, string path) // [0] = OBJECT, [1] = FIELDINFO
        {
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
            }
            if ((tgtStat != null) && (tgtStat.GetValue(tgtObjPre) != null))
            {
                return new object[] { tgtObjPre, tgtStat };
            }
            return new object[] { null, null };
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
                object[] y = this.GetObjAndField(obj, path);
                if ((y[0] != null) && (y[1] != null))
                {
                    FieldInfo z = (FieldInfo)y[1];
                    this.effectMemory[obj].Add((float)z.GetValue(y[0]));
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
                        z.SetValue(y[0], this.effectMemory[element][i] * this.GetBuffAverage() + this.GetBuffAddAverage());
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
                    z.SetValue(y[0], this.effectMemory[obj][i]);
                }
                i++;
            }
            this.effectMemory.Remove(obj);
        }

    }
}
