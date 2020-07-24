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
