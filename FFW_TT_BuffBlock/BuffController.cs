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
        public List<ModuleBooster> boosterList = new List<ModuleBooster>();
        public List<ModuleShieldGenerator> shieldList = new List<ModuleShieldGenerator>();

        /* WEAPON : FIRE RATE */
        public Dictionary<ModuleBuff, float> weaponCooldownBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWeaponGun, List<float>> weaponCooldownOld = new Dictionary<ModuleWeaponGun, List<float>>(); // [0] = ShotCooldown, [1] = BurstCooldown
        public float WeaponCooldownMult { get { return this.weaponCooldownBuffBlocks.Values.Average(); } }
        public static FieldInfo field_ShotCooldown = typeof(ModuleWeaponGun)
            .GetField("m_ShotCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_BurstCooldown = typeof(ModuleWeaponGun)
            .GetField("m_BurstCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_ModuleWeapon = typeof(ModuleWeaponGun)
            .GetField("m_WeaponModule", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_MW_ShotCooldown = typeof(ModuleWeapon)
            .GetField("m_ShotCooldown", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WEAPON : ROTATION SPEED */
        public Dictionary<ModuleBuff, float> weaponRotationBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWeaponGun, float> weaponRotationOld = new Dictionary<ModuleWeaponGun, float>();
        public float WeaponRotationMult { get { return this.weaponRotationBuffBlocks.Values.Average(); } }
        public static FieldInfo field_Rotation = typeof(ModuleWeapon)
            .GetField("m_RotateSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WEAPON : SPREAD */
        public Dictionary<ModuleBuff, float> weaponSpreadBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWeaponGun, float> weaponSpreadOld = new Dictionary<ModuleWeaponGun, float>();
        public float WeaponSpreadMult { get { return this.weaponSpreadBuffBlocks.Values.Average(); } }
        public static FieldInfo field_FiringData = typeof(ModuleWeaponGun)
            .GetField("m_FiringData", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Spread = typeof(FireData)
            .GetField("m_BulletSprayVariance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* WHEELS : MAX RPM */
        public Dictionary<ModuleBuff, float> wheelsRpmBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWheels, float> wheelsRpmOld = new Dictionary<ModuleWheels, float>();
        public float WheelsRpmMult { get { return this.wheelsRpmBuffBlocks.Values.Average(); } }
        public static FieldInfo field_TorqueParams = typeof(ModuleWheels)
            .GetField("m_TorqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Wheels = typeof(ModuleWheels)
            .GetField("m_Wheels", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_WheelParams = typeof(ModuleWheels)
            .GetField("m_WheelParams", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_AttachedId = typeof(ManWheels.Wheel)
            .GetField("attachedID", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_WheelState = typeof(ManWheels)
            .GetField("m_WheelState", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WHEELS : BRAKE TORQUE */
        public Dictionary<ModuleBuff, float> wheelsBrakeBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWheels, List<float>> wheelsBrakeOld = new Dictionary<ModuleWheels, List<float>>(); // [0] = passiveBrakeMaxTorque, [1] = basicFrictionTorque
        public float WheelsBrakeMult { get { return this.wheelsBrakeBuffBlocks.Values.Average(); } }

        /* WHEELS : MAX TORQUE */
        public Dictionary<ModuleBuff, float> wheelsTorqueBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleWheels, float> wheelsTorqueOld = new Dictionary<ModuleWheels, float>(); // [0] = passiveBrakeMaxTorque, [1] = basicFrictionTorque
        public float WheelsTorqueMult { get { return this.wheelsTorqueBuffBlocks.Values.Average(); } }

        /* BOOSTER : BURN RATE */
        public Dictionary<ModuleBuff, float> boosterBurnRateBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleBooster, float> boosterBurnRateOld = new Dictionary<ModuleBooster, float>();
        public float BoosterBurnRateMult { get { return this.boosterBurnRateBuffBlocks.Values.Average(); } }
        public static FieldInfo field_Jets = typeof(ModuleBooster)
            .GetField("jets", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_BurnRate = typeof(BoosterJet)
            .GetField("m_BurnRate", BindingFlags.NonPublic | BindingFlags.Instance);

        /* SHIELD : RADIUS */
        public Dictionary<ModuleBuff, float> shieldRadiusBuffBlocks = new Dictionary<ModuleBuff, float>();
        public Dictionary<ModuleShieldGenerator, float> shieldRadiusOld = new Dictionary<ModuleShieldGenerator, float>();
        public float ShieldRadiusMult { get { return this.shieldRadiusBuffBlocks.Values.Average(); } }
        public static FieldInfo field_Radius = typeof(ModuleShieldGenerator)
            .GetField("m_Radius", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_State = typeof(ModuleShieldGenerator)
            .GetField("m_State", BindingFlags.NonPublic | BindingFlags.Instance);

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
            //Console.WriteLine("FFW - Controller Added");
            BuffController.allControllers.Add(obj);
        }


        public static void RemoveObject(BuffController obj) // Todo: Make "cleaning" function
        {
            //Console.WriteLine("FFW - Controller Removed");
            BuffController.allControllers.Remove(obj);
        }

        public void AddBuff(ModuleBuff buff)
        {
            //Console.WriteLine("FFW - Buff Added");
            if (buff.m_BuffType == "WeaponCooldown")
            {
                //Console.WriteLine("FFW - Weapon Cooldown Buff Added");
                this.weaponCooldownBuffBlocks.Add(buff, buff.m_Strength);
                if (this.weaponCooldownBuffBlocks.Count == 1)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
                        field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * WeaponCooldownMult);
                        ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                        field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
                    }
                }
            }
            if (buff.m_BuffType == "WeaponRotation")
            {
                //Console.WriteLine("FFW - Weapon Rotation Buff Added");
                this.weaponRotationBuffBlocks.Add(buff, buff.m_Strength);
                if (this.weaponRotationBuffBlocks.Count == 1)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                        field_Rotation.SetValue(value_ModuleWeapon, this.weaponRotationOld[weapon] * WeaponRotationMult);
                    }
                }
            }
            if (buff.m_BuffType == "WeaponSpread")
            {
                //Console.WriteLine("FFW - Weapon Spread Buff Added");
                this.weaponSpreadBuffBlocks.Add(buff, buff.m_Strength);
                if (this.weaponSpreadBuffBlocks.Count == 1)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                        field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon] * WeaponSpreadMult);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsRPM")
            {
                //Console.WriteLine("FFW - Wheels RPM Buff Added");
                this.wheelsRpmBuffBlocks.Add(buff, buff.m_Strength);
                if (this.wheelsRpmBuffBlocks.Count == 1)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * WheelsRpmMult;
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsBrake")
            {
                //Console.WriteLine("FFW - Wheels Brake Buff Added");
                this.wheelsBrakeBuffBlocks.Add(buff, buff.m_Strength);
                if (this.wheelsBrakeBuffBlocks.Count == 1)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0] * WheelsBrakeMult;
                        torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1] * WheelsBrakeMult;
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsTorque")
            {
                //Console.WriteLine("FFW - Wheels Torque Buff Added");
                this.wheelsTorqueBuffBlocks.Add(buff, buff.m_Strength);
                if (this.wheelsTorqueBuffBlocks.Count == 1)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels] * WheelsTorqueMult;
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "BoosterBurnRate")
            {
                //Console.WriteLine("FFW - Booster Burn Rate Buff Added");
                this.boosterBurnRateBuffBlocks.Add(buff, buff.m_Strength);
                if (this.boosterBurnRateBuffBlocks.Count == 1)
                {
                    foreach (ModuleBooster booster in this.boosterList)
                    {
                        List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
                        foreach (BoosterJet jet in value_Jets)
                        {
                            if (!boosterBurnRateOld.ContainsKey(booster))
                            {
                                float value_BurnRate = (float)field_BurnRate.GetValue(jet);
                                this.boosterBurnRateOld.Add(booster, value_BurnRate);
                                Console.WriteLine("Registered burn rate...");
                                Console.WriteLine(value_BurnRate);
                            }
                            field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster] * BoosterBurnRateMult);
                            Console.WriteLine("Burn rate set");
                        }
                    }
                }
            }
            if (buff.m_BuffType == "ShieldRadius")
            {
                //Console.WriteLine("FFW - Shield Radius Buff Added");
                this.shieldRadiusBuffBlocks.Add(buff, buff.m_Strength);
                if (this.shieldRadiusBuffBlocks.Count == 1)
                {
                    foreach (ModuleShieldGenerator shield in this.shieldList)
                    {
                        Type stateEnum = field_State.GetValue(shield).GetType();
                        field_Radius.SetValue(shield, this.shieldRadiusOld[shield] * ShieldRadiusMult);
                        field_State.SetValue(shield, Enum.ToObject(stateEnum, 0));
                    }
                }
            }
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            //Console.WriteLine("FFW - Buff Removed");
            if (buff.m_BuffType == "WeaponCooldown")
            {
                //Console.WriteLine("FFW - Weapon Cooldown Buff Removed");
                this.weaponCooldownBuffBlocks.Remove(buff);
                if (this.weaponCooldownBuffBlocks.Count == 0)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
                        field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
                        ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                        field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0]);
                    }
                }
            }
            if (buff.m_BuffType == "WeaponRotation")
            {
                //Console.WriteLine("FFW - Weapon Rotation Buff Removed");
                this.weaponRotationBuffBlocks.Remove(buff);
                if (this.weaponRotationBuffBlocks.Count == 0)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                        field_Rotation.SetValue(value_ModuleWeapon, this.weaponRotationOld[weapon]);
                    }
                }
            }
            if (buff.m_BuffType == "WeaponSpread")
            {
                //Console.WriteLine("FFW - Weapon Spread Buff Removed");
                this.weaponSpreadBuffBlocks.Remove(buff);
                if (this.weaponSpreadBuffBlocks.Count == 0)
                {
                    foreach (ModuleWeaponGun weapon in this.weaponList)
                    {
                        FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                        field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon]);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsRPM")
            {
                //Console.WriteLine("FFW - Wheels RPM Buff Removed");
                this.wheelsRpmBuffBlocks.Remove(buff);
                if (this.wheelsRpmBuffBlocks.Count == 0)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels];
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsBrake")
            {
                //Console.WriteLine("FFW - Wheels Brake Buff Removed");
                this.wheelsBrakeBuffBlocks.Remove(buff);
                if (this.wheelsBrakeBuffBlocks.Count == 0)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0];
                        torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1];
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "WheelsTorque")
            {
                //Console.WriteLine("FFW - Wheels Torque Buff Removed");
                this.wheelsTorqueBuffBlocks.Remove(buff);
                if (this.wheelsTorqueBuffBlocks.Count == 0)
                {
                    foreach (ModuleWheels wheels in this.wheelsList)
                    {
                        ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                        torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels];
                        this.RefreshWheels(wheels, torque);
                    }
                }
            }
            if (buff.m_BuffType == "BoosterBurnRate")
            {
                //Console.WriteLine("FFW - Booster Burn Rate Buff Removed");
                foreach (ModuleBooster booster in this.boosterList)
                {
                    List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
                    foreach (BoosterJet jet in value_Jets)
                    {
                        if (this.boosterBurnRateOld.ContainsKey(booster))
                        {
                            field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster]);
                        }
                    }
                }
                this.boosterBurnRateBuffBlocks.Remove(buff);
            }
            if (buff.m_BuffType == "ShieldRadius")
            {
                //Console.WriteLine("FFW - Shield Radius Buff Removed");
                this.shieldRadiusBuffBlocks.Remove(buff);
                if (this.shieldRadiusBuffBlocks.Count == 0)
                {
                    foreach (ModuleShieldGenerator shield in this.shieldList)
                    {
                        Type stateEnum = field_State.GetValue(shield).GetType();
                        field_Radius.SetValue(shield, this.shieldRadiusOld[shield]);
                        field_State.SetValue(shield, Enum.ToObject(stateEnum, 0));
                    }
                }
            }
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            //Console.WriteLine("FFW - Add Removed");
            this.weaponList.Add(weapon);
            this.weaponCooldownOld.Add(weapon, new List<float>()
            {
                (float)field_ShotCooldown.GetValue(weapon),
                (float)field_BurstCooldown.GetValue(weapon)
            });
            ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
            this.weaponRotationOld.Add(weapon, (float)field_Rotation.GetValue(value_ModuleWeapon));
            FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
            this.weaponSpreadOld.Add(weapon, (float)field_Spread.GetValue(value_FiringData));

            if (this.weaponCooldownBuffBlocks.Count > 0)
            {
                field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
                field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * WeaponCooldownMult);
                field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0] * WeaponCooldownMult);
            }
            if (this.weaponRotationBuffBlocks.Count > 0)
            {
                field_Rotation.SetValue(value_ModuleWeapon, this.weaponRotationOld[weapon] * WeaponRotationMult);
            }
            if (this.weaponSpreadBuffBlocks.Count > 0)
            {
                field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon] * WeaponSpreadMult);
            }
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            //Console.WriteLine("FFW - Weapon Removed");
            field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
            field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
            ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
            field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0]);
            FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
            field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon]);

            this.weaponList.Remove(weapon);
            this.weaponCooldownOld.Remove(weapon);
            this.weaponRotationOld.Remove(weapon);
            this.weaponSpreadOld.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
            //Console.WriteLine("FFW - Wheels Added");
            this.wheelsList.Add(wheels);
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            this.wheelsRpmOld.Add(wheels, torque.torqueCurveMaxRpm);
            this.wheelsBrakeOld.Add(wheels, new List<float>()
            {
                torque.passiveBrakeMaxTorque,
                torque.basicFrictionTorque
            });
            this.wheelsTorqueOld.Add(wheels, torque.torqueCurveMaxTorque);
            if (this.wheelsRpmBuffBlocks.Count > 0)
            {
                torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * WheelsRpmMult;
                this.RefreshWheels(wheels, torque); // Move this out of ifs?
            }
            if (this.wheelsBrakeBuffBlocks.Count > 0)
            {
                torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0] * WheelsBrakeMult;
                torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1] * WheelsBrakeMult;
                this.RefreshWheels(wheels, torque); // Identical lines...
            }
            if (this.wheelsTorqueBuffBlocks.Count > 0)
            {
                torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels] * WheelsTorqueMult;
                this.RefreshWheels(wheels, torque);
            }
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            //Console.WriteLine("FFW - Wheels Removed");
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels];
            torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0];
            torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1];
            torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels];
            this.RefreshWheels(wheels, torque);
            this.wheelsList.Remove(wheels);
            this.wheelsRpmOld.Remove(wheels);
            this.wheelsBrakeOld.Remove(wheels);
            this.wheelsTorqueOld.Remove(wheels);
        }

        public void AddBooster(ModuleBooster booster)
        {
            //Console.WriteLine("FFW - Booster Added");
            this.boosterList.Add(booster);
            List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
            
            if (this.boosterBurnRateBuffBlocks.Count > 0)
            {
                foreach (BoosterJet jet in value_Jets)
                {
                    if (!boosterBurnRateOld.ContainsKey(booster))
                    { 
                        float value_BurnRate = (float)field_BurnRate.GetValue(jet);
                        this.boosterBurnRateOld.Add(booster, value_BurnRate);
                        Console.WriteLine("Registered burn rate...");
                        Console.WriteLine(value_BurnRate);
                    }
                    field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster] * BoosterBurnRateMult);
                    Console.WriteLine("Burn rate set");
                }
            }
        }

        public void RemoveBooster(ModuleBooster booster)
        {
            //Console.WriteLine("FFW - Booster Removed");
            List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
            foreach (BoosterJet jet in value_Jets)
            {
                if (this.boosterBurnRateOld.ContainsKey(booster))
                {
                    field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster]);
                }
            }
            this.boosterList.Remove(booster);
            this.boosterBurnRateOld.Remove(booster);
        }

        public void AddShield(ModuleShieldGenerator shield)
        {
            Console.WriteLine("FFW - Shield Added");
            this.shieldList.Add(shield);
            float value_Radius = (float)field_Radius.GetValue(shield);
            this.shieldRadiusOld.Add(shield, value_Radius);
            if (this.shieldRadiusBuffBlocks.Count > 0)
            {
                field_Radius.SetValue(shield, this.shieldRadiusOld[shield] * ShieldRadiusMult);
            }
        }


        public void RemoveShield(ModuleShieldGenerator shield)
        {
            Console.WriteLine("FFW - Shield Removed");
            field_Radius.SetValue(shield, this.shieldRadiusOld[shield]);
            this.shieldList.Remove(shield);
            this.shieldRadiusOld.Remove(shield);
        }

        public void RefreshWheels(ModuleWheels wheels, ManWheels.TorqueParams torque)
        {
            field_TorqueParams.SetValue(wheels, torque); // Apply new Torque to ModuleWheels

            //ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
            //wheelparams.strafeSteeringSpeed = 20.0f;

            //field_WheelParams.SetValue(wheels, wheelparams);

            List<ManWheels.Wheel> value_Wheels = (List<ManWheels.Wheel>)field_Wheels.GetValue(wheels);
            foreach (ManWheels.Wheel wheel in value_Wheels)
            {
                Array value_WheelState = (Array)field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);

                int value_WheelAttachedId = (int)field_AttachedId.GetValue(wheel); // Important, determines what AttachedWheelState is associated
                if (value_WheelAttachedId > 0) // value is -1 if it's unloaded, I think...
                {
                    object value_AttachedWheelState = value_WheelState.GetValue(value_WheelAttachedId); // AttachedWheelState is a PRIVATE STRUCT, `object` neccessary
                    FieldInfo field_p_TorqueParams = value_AttachedWheelState.GetType() // Get types of private struct...
                        .GetField("torqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo field_p_Inertia = value_AttachedWheelState.GetType()
                        .GetField("inertia", BindingFlags.NonPublic | BindingFlags.Instance);

                    field_p_TorqueParams.SetValue(value_AttachedWheelState, torque); // Apply new Torque to ManWheels.Wheel
                    //ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Note: Keep these incase inertia read causes issues
                    //float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
                    ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, (float)field_p_Inertia.GetValue(value_AttachedWheelState), value_Wheels.Count);

                    /*FieldInfo field_p_WheelParams = value_AttachedWheelState.GetType()
                        .GetField("wheelParams", BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    field_p_WheelParams.SetValue(value_AttachedWheelState, wheelparams);*/

                    wheel.UpdateAttachData(moduleData); // Update it! Live! Do it!
                                                        // Also logs "only for use in Editor" error, annoying...
                                                        
                }
            }
        }
    }
}
