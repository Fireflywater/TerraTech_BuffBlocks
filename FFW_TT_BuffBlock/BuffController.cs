using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;

namespace FFW_TT_BuffBlock
{
    class BuffController
    {
        /* PRIME PROPERTIES */
        public static List<BuffController> allControllers = new List<BuffController>();
        public Tank tank;
        public List<ModuleWeaponGun> weaponList = new List<ModuleWeaponGun>();
        public List<ModuleWheels> wheelsList = new List<ModuleWheels>();

        /* WEAPON : FIRE RATE */
        public Dictionary<ModuleBuff, float> weaponCooldownBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWeaponGun, List<float>> weaponCooldownOld = new Dictionary<ModuleWeaponGun, List<float>>(); // [0] = ShotCooldown, [1] = BurstCooldown
        public float WeaponCooldownMult { get { return this.weaponCooldownBuffBlocks.Values.Average(); } }
        public static FieldInfo field_ShotCooldown = typeof(ModuleWeaponGun)
            .GetField("m_ShotCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_BurstCooldown = typeof(ModuleWeaponGun)
            .GetField("m_BurstCooldown", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WHEELS : MAX RPM */
        public Dictionary<ModuleBuff, float> wheelsRpmBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWheels, float> wheelsRpmOld = new Dictionary<ModuleWheels, float>();
        public float WheelsRpmMult { get { return this.wheelsRpmBuffBlocks.Values.Average(); } }
        // ModuleWheels.m_TorqueParams.torqueCurveMaxRpm
        public static FieldInfo field_TorqueParams = typeof(ModuleWheels)
            .GetField("m_TorqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Wheels = typeof(ModuleWheels)
            .GetField("m_Wheels", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_WheelParams = typeof(ModuleWheels)
            .GetField("m_WheelParams", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Animated = typeof(ModuleWheels)
            .GetField("m_Animated", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo method_OnAttach = typeof(ModuleWheels)
            .GetMethod("OnAttach", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo method_OnDetach = typeof(ModuleWheels)
            .GetMethod("OnDetach", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_AttachedId = typeof(ManWheels.Wheel)
            .GetField("attachedID", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_WheelState = typeof(ManWheels)
            .GetField("m_WheelState", BindingFlags.NonPublic | BindingFlags.Instance);
        /*public static MethodInfo field_AttachedWheelState = typeof(ManWheels)
            .GetField("AttachedWheelState", BindingFlags.NonPublic | BindingFlags.Instance);*/

        public static BuffController MakeNewIfNone(Tank objTank)
        {
            foreach (BuffController element in BuffController.allControllers)
            {
                if (element.tank == objTank)
                {
                    return element;
                }
            }
            BuffController newObject = new BuffController
            {
                tank = objTank
            };
            BuffController.AddObject(newObject);
            return newObject;
        }

        public static void AddObject(BuffController obj)
        {
            BuffController.allControllers.Add(obj);
        }


        public static void RemoveObject(BuffController obj)
        {
            BuffController.allControllers.Remove(obj);
        }

        public void AddBuff(ModuleBuff buff)
        {
            if (buff.m_BuffType == "WeaponCooldown")
            {
                this.weaponCooldownBuffBlocks.Add(buff, buff.m_Strength);
                if (this.weaponCooldownBuffBlocks.Count == 1)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
                        field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * WeaponCooldownMult);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsRPM")
            {
                Console.WriteLine("FFW - Wheels Buff Added");
                this.wheelsRpmBuffBlocks.Add(buff, buff.m_Strength);
                if (this.wheelsRpmBuffBlocks.Count == 1)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        /*ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * WheelsRpmMult;
                        field_TorqueParams.SetValue(wheels, torque);*/

                        Console.WriteLine(this.wheelsRpmOld[wheels] * WheelsRpmMult);
                        this.RefreshWheels(wheels, this.wheelsRpmOld[wheels] * WheelsRpmMult);
                    }
                }
            }
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            if (buff.m_BuffType == "WeaponCooldown")
            {
                this.weaponCooldownBuffBlocks.Remove(buff);
                if (this.weaponCooldownBuffBlocks.Count == 0)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
                        field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsRPM")
            {
                Console.WriteLine("FFW - Wheels Buff Removed");
                this.wheelsRpmBuffBlocks.Remove(buff);
                if (this.wheelsRpmBuffBlocks.Count == 0)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        /*ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels];
                        field_TorqueParams.SetValue(wheels, torque);*/
                        Console.WriteLine(this.wheelsRpmOld[wheels]);
                        this.RefreshWheels(wheels, this.wheelsRpmOld[wheels]);
                    }
                }
            }
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            this.weaponList.Add(weapon);
            this.weaponCooldownOld.Add(weapon, new List<float>()
            {
                (float)field_ShotCooldown.GetValue(weapon),
                (float)field_BurstCooldown.GetValue(weapon)
            });
            if (this.weaponCooldownBuffBlocks.Count > 0)
            {
                field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
                field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * WeaponCooldownMult);
            }
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
            field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
            this.weaponList.Remove(weapon);
            this.weaponCooldownOld.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
            //Console.WriteLine("FFW - Wheels Added");
            this.wheelsList.Add(wheels);
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            this.wheelsRpmOld.Add(wheels, torque.torqueCurveMaxRpm);
            if (this.wheelsRpmBuffBlocks.Count > 0)
            {
                this.RefreshWheels(wheels, this.wheelsRpmOld[wheels] * WheelsRpmMult);
                //torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * 4.0f; //WheelsRpmMult;
                //field_TorqueParams.SetValue(wheels, torque);
            }
            //field_TorqueCurveMaxRpm.SetValue((ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels), 0);
            //ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            //Console.WriteLine("FFW");
            //torque.torqueCurveMaxRpm = 5.0f;
            //torque.torqueCurveMaxTorque = 5.0f;
            //field_TorqueParams.SetValue(wheels, torque);
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            //Console.WriteLine("FFW - Wheels Removed");
            //ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            //torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels];
            //field_TorqueParams.SetValue(wheels, torque);
            //this.RefreshWheels(wheels, this.wheelsRpmOld[wheels]);
            this.RefreshWheels(wheels, this.wheelsRpmOld[wheels]);
            this.wheelsList.Remove(wheels);
            this.wheelsRpmOld.Remove(wheels);
        }

        public void RefreshWheels(ModuleWheels wheels, float rpm)
        {
            Console.WriteLine("FFW!");
            Console.WriteLine(1);
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            Console.WriteLine(2);
            torque.torqueCurveMaxRpm = rpm;
            Console.WriteLine("Wheels set to...");
            Console.WriteLine(rpm);
            Console.WriteLine(3);
            field_TorqueParams.SetValue(wheels, torque);

            List<ManWheels.Wheel> value_Wheels = (List<ManWheels.Wheel>)field_Wheels.GetValue(wheels);
            Console.WriteLine(4);
            
            foreach (ManWheels.Wheel wheel in value_Wheels)
            {
                Console.WriteLine(5);
                /* FieldInfo field_torqueParams2 = typeof(ManWheels).GetNestedTypes(BindingFlags.NonPublic)
                     .FirstOrDefault(type =>
                         type.Name.Contains("AttachedWheelState")
                     )
                     .GetField("torqueParams", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);*/
                /*Type field_torqueParams2 = typeof(ManWheels).GetNestedTypes(BindingFlags.NonPublic)
                    .FirstOrDefault(type =>
                        type.Name.Contains("AttachedWheelState")
                    );
                int value_AttachedId = (int)field_AttachedId.GetValue(wheel);
                object[] array = (object[])field_torqueParams2.GetProperty("AttachedWheelState");*/
                var iunno = Singleton.Manager<ManWheels>.inst;
                Console.WriteLine(6);
                int iunno2 = (int)field_AttachedId.GetValue(wheel); // Keep this
                Console.WriteLine("after this");
                Console.WriteLine(iunno2);
                Console.WriteLine("before this");
                /*FieldInfo iunno10 = typeof(Singleton.Manager<ManWheels>)
                    .GetField("inst", BindingFlags.Public | BindingFlags.Instance).GetValue(iunno).GetType()
                    .GetField("m_WheelState", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(iunno2).GetType()
                    .GetField("torqueParams");*/

                //Type privateTypeThing = Type.GetType("TerraTech.ManWheels.AttachedWheelState");
                //Console.WriteLine(privateTypeThing);
                //Console.WriteLine("7.5");
                Array iunno3 = (Array)field_WheelState.GetValue(iunno);
                Console.WriteLine(7);
                if (iunno2 > 0)
                {
                    object iunno4 = iunno3.GetValue(iunno2);
                    Console.WriteLine(iunno4);
                    Console.WriteLine(8);
                    FieldInfo field_TorqueParams2 = iunno4.GetType()
                        .GetField("torqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
                    Console.WriteLine(9);
                    Console.WriteLine(field_TorqueParams2.GetValue(iunno4));
                    var iunno5 = field_TorqueParams2.GetValue(iunno4);
                    //iunno10.SetValue(iunno, torque);
                    //Console.WriteLine(iunno3);
                    Console.WriteLine(10);
                    FieldInfo field_TorqueCurve = iunno5.GetType()
                        .GetField("torqueCurveMaxRpm", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Console.WriteLine(11);
                    Console.WriteLine(field_TorqueCurve.GetValue(iunno5));
                    //FieldInfo iunno5 = structType.GetField("torqueParams");
                    Console.WriteLine(12);
                    field_TorqueParams2.SetValue(iunno4, torque);
                    //field_TorqueCurve.SetValue(iunno5, rpm);
                    //FieldInfo iunno4 = iunno3.GetValue(iunno2).GetType().GetField("torqueParams");
                    //Console.WriteLine(iunno4.GetValue(???));
                    Console.WriteLine(13);
                    Console.WriteLine(field_TorqueCurve.GetValue(iunno5));
                    //FieldInfo iunno5 = iunno4.GetType().GetField("torqueCurveMaxRpm");
                    Console.WriteLine(14);
                    MethodInfo method_Init = iunno5.GetType()
                        .GetMethod("Init");
                    MethodInfo method_Reset = iunno5.GetType()
                        .GetMethod("UpdateAttachData");
                    Console.WriteLine(15);
                    ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    Console.WriteLine(16);
                    bool value_Animated = (bool)field_Animated.GetValue(wheels);
                    Console.WriteLine(17);
                    float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
                    Console.WriteLine(18);
                    ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, i, value_Wheels.Count);
                    Console.WriteLine(19);
                    object[] parametersArray = new object[] { wheel, moduleData };
                    Console.WriteLine(20);
                    //method_Init.Invoke(iunno5, parametersArray);
                    //method_Reset.Invoke(iunno5, new object[] { moduleData });
                    wheel.UpdateAttachData(moduleData);
                    Console.WriteLine(21);
                    //Console.WriteLine(iunno5.GetValue(iunno4));
                    //iunno5.SetValue(iunno3.GetValue(iunno2), 0.0f); // Issue here
                    //Singleton.Manager<ManWheels>.inst.m_WheelState[this.attachedID]
                    //field_torqueParams2.SetValue(wheel, torque);

                    /*Console.WriteLine(5);
                    var amountField = GetType().GetField("m_WheelState");
                    Console.WriteLine(6);
                    var money = amountField.GetValue(element);
                    Console.WriteLine(7);
                    var codeField = money.GetType().GetField("torqueParams");
                    Console.WriteLine(8);
                    //element.AttachedWheelState.torqueParams = torque;
                    codeField.SetValue(money, torque);
                    Console.WriteLine(9);*/
                }
            }
            /*ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
            Console.WriteLine(5);
            bool value_Animated = (bool)field_Animated.GetValue(wheels);
            Console.WriteLine(6);

            float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
            Console.WriteLine(7);
            ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, i, value_Wheels.Count);
            Console.WriteLine(8);*/
            //method_OnDetach.Invoke(wheels, new object[] { });
            //method_OnAttach.Invoke(wheels, new object[] { });
            /*foreach (ManWheels.Wheel element in value_Wheels)
            {
                Console.WriteLine(9);
                int value_AttachedId = (int)field_AttachedId.GetValue(element);
                ManWheels.AttachedWheelState[] value_WheelState = (ManWheels.AttachedWheelState[])field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);
                value_WheelState[value_AttachedId].Init(element, moduleData);
                //element.Detach();
            }*/
            /*for (int j = 0; j < value_Wheels.Count; j++)
            {
                Console.WriteLine(9);
            }
            for (int j = 0; j < value_Wheels.Count; j++)
            {
                if (!value_Wheels[j].IsNullOrEmpty())
                {
                    Console.WriteLine(10);
                    value_Wheels[j].Attach(moduleData);
                    Console.WriteLine(11);
                    value_Wheels[j].SetAnimated(value_Animated);
                    Console.WriteLine(12);
                }
            }*/
            /*foreach(ManWheels.Wheel element in value_Wheels)
            {
                Console.WriteLine(9);
                element.Detach();
            }
            foreach (ManWheels.Wheel element in value_Wheels)
            {
                Console.WriteLine(10);
                element.Attach(moduleData);
                Console.WriteLine(11);
                element.SetAnimated(value_Animated);
                Console.WriteLine(12);
            }
            Console.WriteLine(13);*/
        }
    }
}
