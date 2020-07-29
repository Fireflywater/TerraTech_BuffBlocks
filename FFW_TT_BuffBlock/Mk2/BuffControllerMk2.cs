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

        /* TODO: Refactor 3
        Instatiate all available buffs on load into static field
        Copy said information into local field on controller instantiation

            When BUFF gets pooled:
                go through BUFF.BUFFPATHS as PATH
                    add new SEGMENT to static SEGMENTS

            When CONTROLLER gets created:
                go through static SEGMENTS as SEGMENT
                    add a copy of SEGMENT to local SEGMENTS
                    // use shallow copy?

            When BLOCK gets added:
                Add to ALL
                go through SEGMENTS
                    if SEGMENT.COMPONENT exists on BLOCK
                        add BLOCK into SEGMENT.BLOCKS
                        save this BLOCK

            When BLOCK gets removed:
                go through SEGMENTS
                    if SEGMENT.BLOCKS contains BLOCK
                        clean this BLOCK
                        remove BLOCK from SEGMENT.BLOCKS
                Remove from ALL

            When BUFF gets added:
                go through BUFF.BUFFPATHS as PATH
                    reference SEGMENT via PATH
                        add BUFF to SEGMENT.BUFFS
                        update all in SEGMENT.BLOCKS

            When BUFF gets removed
                go through BUFF.BUFFPATHS as PATH
                    reference SEGMENT via PATH
                        remove BUFF from SEGMENT.BUFFS
                        update all in SEGMENT.BLOCKS
        */

        public List<TankBlock> allBlocks = new List<TankBlock>(); // Memory of all blocks on tech
        public Dictionary<Type, List<TankBlock>> typeToBlock = new Dictionary<Type, List<TankBlock>>(); // Shorthand for all blocks with specific Type
        public Dictionary<string, BuffSegmentMk2> pathToSegment = new Dictionary<string, BuffSegmentMk2>(); // Reference to all Segments, via path
        
        public Dictionary<CannonBarrel, float> weaponSpeedMemory = new Dictionary<CannonBarrel, float>();

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
            List<string> affectedModules = new List<string>();
            for (int i = 0; i < buff.m_BuffPath.Length; i++)
            {
                string path = buff.m_BuffPath[i];
                List<string> splitPath = path.Split('.').ToList();
                List<string> restOfPath = splitPath.Skip(1).ToList();
                Type component = typeof(TankBlock).Assembly.GetType(splitPath[0]);
                if (component != null)
                {
                    affectedModules.Add(splitPath[0]);
                    if (!typeToBlock.ContainsKey(component))
                    {
                        typeToBlock.Add(component, new List<TankBlock>());
                        Console.WriteLine("FFW! Added typeToBlock => " + component.Name);
                        foreach (TankBlock block in this.allBlocks)
                        {
                            object blockComponent = block.GetComponent(component);
                            if (blockComponent != null)
                            {
                                if (!this.typeToBlock[component].Contains(block))
                                {
                                    this.typeToBlock[component].Add(block);
                                    Console.WriteLine("FFW! +Reg => " + block.name + " => " + component.Name);
                                }
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
                        pathToSegment[path].ManipulateObj(this.typeToBlock[component], "SAVE");
                        if (path == "ModuleWeaponGun.m_ShotCooldown")
                        {
                            BuffSpecificFix.ManipulateBarrels(this.typeToBlock[component], "SAVE", this.weaponSpeedMemory, 1.0f);
                        }
                    }
                    pathToSegment[path].AddBuff(buff, i);
                    Console.WriteLine("FFW! Added buff to Segment => " + path);
                    pathToSegment[path].ManipulateObj(this.typeToBlock[component], "UPDATE");
                }
                else
                {
                    Console.WriteLine("FFW! AddBuff! Type " + splitPath[0] + " doesn't exist.");
                }
            }
            if (affectedModules.Contains("ModuleWeaponGun") && pathToSegment.ContainsKey("ModuleWeaponGun.m_ShotCooldown"))
            {
                float avg = pathToSegment["ModuleWeaponGun.m_ShotCooldown"].GetAverages();
                BuffSpecificFix.ManipulateBarrels(this.typeToBlock[typeof(ModuleWeaponGun)], "UPDATE", this.weaponSpeedMemory, avg);
            }
            if (affectedModules.Contains("ModuleWheels"))
            {
                BuffSpecificFix.RefreshWheels(this.typeToBlock[typeof(ModuleWheels)]);
            }
        }

        public void RemoveBuff(ModuleBuffMk2 buff)
        {
            List<string> affectedModules = new List<string>();
            for (int i = 0; i < buff.m_BuffPath.Length; i++)
            {
                string path = buff.m_BuffPath[i];
                List<string> splitPath = path.Split('.').ToList();
                Type component = typeof(TankBlock).Assembly.GetType(splitPath[0]);
                if (component != null)
                {
                    affectedModules.Add(splitPath[0]);
                    pathToSegment[path].RemoveBuff(buff);
                    Console.WriteLine("FFW! Removed buff from Segment => " + path);
                    pathToSegment[path].ManipulateObj(this.typeToBlock[component], "UPDATE");
                }
                else
                {
                    Console.WriteLine("FFW! RemoveBuff! Type " + splitPath[0] + " doesn't exist.");
                }
            }
            if (affectedModules.Contains("ModuleWeaponGun") && pathToSegment.ContainsKey("ModuleWeaponGun.m_ShotCooldown"))
            {
                float avg = pathToSegment["ModuleWeaponGun.m_ShotCooldown"].GetAverages();
                BuffSpecificFix.ManipulateBarrels(this.typeToBlock[typeof(ModuleWeaponGun)], "UPDATE", this.weaponSpeedMemory, avg);
            }
            if (affectedModules.Contains("ModuleWheels"))
            {
                BuffSpecificFix.RefreshWheels(this.typeToBlock[typeof(ModuleWheels)]);
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
                    if (!this.typeToBlock[component].Contains(block))
                    {
                        this.typeToBlock[component].Add(block);
                        Console.WriteLine("FFW! +Reg => " + block.name + " => " + component.Name);
                    }
                    segPair.Value.ManipulateObj(new List<TankBlock> { block }, "SAVE");
                    if (segPair.Key == "ModuleWeaponGun.m_ShotCooldown")
                    {
                        float avg = segPair.Value.GetAverages();
                        BuffSpecificFix.ManipulateBarrels(new List<TankBlock> { block }, "SAVE", this.weaponSpeedMemory, 1.0f);
                        BuffSpecificFix.ManipulateBarrels(new List<TankBlock> { block }, "UPDATE", this.weaponSpeedMemory, avg);
                    }
                    if (component == typeof(ModuleWheels))
                    {
                        BuffSpecificFix.RefreshWheels(this.typeToBlock[typeof(ModuleWheels)]);
                    }
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
                    if (segPair.Key == "ModuleWeaponGun.m_ShotCooldown")
                    {
                        float avg = segPair.Value.GetAverages();
                        BuffSpecificFix.ManipulateBarrels(new List<TankBlock> { block }, "CLEAN", this.weaponSpeedMemory, 1.0f);
                    }
                    if (component == typeof(ModuleWheels))
                    {
                        BuffSpecificFix.RefreshWheels(this.typeToBlock[typeof(ModuleWheels)]);
                    }
                }
            }
            this.allBlocks.Remove(block);
        }
        
    }
}