using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class BuffControllerMk2
    {
        public static Dictionary<Tank, BuffControllerMk2> allControllers = new Dictionary<Tank, BuffControllerMk2>();
        public Tank tank;
        

        public List<TankBlock> allBlocks = new List<TankBlock>(); // Memory of all blocks on tech
        public Dictionary<Type, List<TankBlock>> typeToBlock = new Dictionary<Type, List<TankBlock>>(); // Shorthand for all blocks with specific Type
        public Dictionary<string, BuffSegmentMk2> pathToSegment = new Dictionary<string, BuffSegmentMk2>(); // Reference to all Segments, via path


        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static BuffControllerMk2 MakeNewIfNone(Tank objTank)
        {
            if (BuffControllerMk2.allControllers.ContainsKey(objTank))
            {
                return BuffControllerMk2.allControllers[objTank];
            }
            BuffControllerMk2 newController = new BuffControllerMk2
            {
                tank = objTank
            };
            BuffControllerMk2.AddController(objTank, newController);
            Console.WriteLine("FFW: Active BuffControlsMk2: " + BuffControllerMk2.allControllers.Count);
            return newController;
        }

        public static void AddController(Tank tank, BuffControllerMk2 obj)
        {
            BuffControllerMk2.allControllers.Add(tank, obj);
        }

        public static void RemoveController(Tank tank)
        {
            BuffControllerMk2.allControllers.Remove(tank);
        }

        public void AddBuff(ModuleBuffMk2 buff)
        {
            for (int i = 0; i < buff.m_BuffPath.Length; i++)
            {
                string path = buff.m_BuffPath[i];
                List<string> splitPath = path.Split('.').ToList();
                List<string> restOfPath = splitPath.Skip(1).ToList();
                Type component = typeof(TankBlock).Assembly.GetType(splitPath[0]);
                if (component != null)
                {
                    if (!typeToBlock.ContainsKey(component))
                    {
                        typeToBlock.Add(component, new List<TankBlock>());
                        Console.WriteLine("FFW! Added typeToBlock => " + component.Name);
                        foreach (TankBlock block in this.allBlocks)
                        {
                            object blockComponent = block.GetComponent(component);
                            if (blockComponent != null)
                            {
                                this.typeToBlock[component].Add(block);
                                Console.WriteLine("FFW! +Reg => " + block.name + " => " + path);
                            }
                        }
                    }
                    if (!pathToSegment.ContainsKey(path))
                    {
                        BuffSegmentMk2 segment = new BuffSegmentMk2()
                        {
                            tank = this.tank,
                            controller = this,
                            effectComponent = component,
                            effectPath = restOfPath
                        };
                        this.pathToSegment.Add(path, segment);
                        Console.WriteLine("FFW! Added pathToSegment => " + path);
                    }
                    pathToSegment[path].AddBuff(buff, i);
                    Console.WriteLine("FFW! Added buff to Segment => " + path);
                }
                else
                {
                    Console.WriteLine("FFW! AddBuff! Type " + splitPath[0] + " doesn't exist.");
                }
            }
        }

        public void RemoveBuff(ModuleBuffMk2 buff)
        {
            for (int i = 0; i < buff.m_BuffPath.Length; i++)
            {
                string path = buff.m_BuffPath[i];
                List<string> splitPath = path.Split('.').ToList();
                Type component = typeof(TankBlock).Assembly.GetType(splitPath[0]);
                if (component != null)
                {
                    pathToSegment[path].RemoveBuff(buff);
                    Console.WriteLine("FFW! Removed buff from Segment => " + path);
                }
                else
                {
                    Console.WriteLine("FFW! RemoveBuff! Type " + splitPath[0] + " doesn't exist.");
                }
            }
        }

        public void AddBlock(TankBlock block)
        {
            this.allBlocks.Add(block);
            foreach (KeyValuePair<string, BuffSegmentMk2> segPair in this.pathToSegment)
            {
                Type component = segPair.Value.effectComponent;
                object blockComponent = block.GetComponent(component);
                if (blockComponent != null)
                {
                    this.typeToBlock[component].Add(block);
                    Console.WriteLine("FFW! +Reg => " + block.name + " => " + segPair.Key);
                    segPair.Value.ManipulateObj(new List<TankBlock> { block }, "SAVE");
                }
            }
        }

        public void RemoveBlock(TankBlock block)
        {
            foreach (KeyValuePair<string, BuffSegmentMk2> segPair in this.pathToSegment)
            {
                Type component = segPair.Value.effectComponent;
                object blockComponent = block.GetComponent(component);
                if (blockComponent != null)
                {
                    this.typeToBlock[component].Remove(block);
                    Console.WriteLine("FFW! -Reg => " + block.name + " => " + segPair.Key);
                    segPair.Value.ManipulateObj(new List<TankBlock> { block }, "CLEAN");
                }
            }
            this.allBlocks.Remove(block);
        }
        
    }
}