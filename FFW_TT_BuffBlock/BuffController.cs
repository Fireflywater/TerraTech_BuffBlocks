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
        public List<ModuleDrill> drillList = new List<ModuleDrill>();
        public List<ModuleEnergy> energyList = new List<ModuleEnergy>();
        public List<ModuleEnergyStore> energyStoreList = new List<ModuleEnergyStore>();
        public List<ModuleItemConsume> itemConList = new List<ModuleItemConsume>();


        //public Dictionary<ModuleBuff, bool> buffBlocksNeedsAnchor = new Dictionary<ModuleBuff, bool>();

        /* WEAPON : FIRE RATE */
        public Dictionary<ModuleBuff, int> weaponCooldownBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWeaponGun, List<float>> weaponCooldownOld = new Dictionary<ModuleWeaponGun, List<float>>(); // [0] = ShotCooldown, [1] = BurstCooldown
        public static FieldInfo field_ShotCooldown = typeof(ModuleWeaponGun)
            .GetField("m_ShotCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_BurstCooldown = typeof(ModuleWeaponGun)
            .GetField("m_BurstCooldown", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_ModuleWeapon = typeof(ModuleWeaponGun)
            .GetField("m_WeaponModule", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_MW_ShotCooldown = typeof(ModuleWeapon)
            .GetField("m_ShotCooldown", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WEAPON : ROTATION SPEED */
        public Dictionary<ModuleBuff, int> weaponRotationBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWeaponGun, float> weaponRotationOld = new Dictionary<ModuleWeaponGun, float>();
        public static FieldInfo field_Rotation = typeof(ModuleWeapon)
            .GetField("m_RotateSpeed", BindingFlags.NonPublic | BindingFlags.Instance);

        /* WEAPON : SPREAD */
        public Dictionary<ModuleBuff, int> weaponSpreadBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWeaponGun, float> weaponSpreadOld = new Dictionary<ModuleWeaponGun, float>();
        public static FieldInfo field_FiringData = typeof(ModuleWeaponGun)
            .GetField("m_FiringData", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Spread = typeof(FireData)
            .GetField("m_BulletSprayVariance", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* WEAPON : VELOCITY */
        public Dictionary<ModuleBuff, int> weaponVelocityBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWeaponGun, float> weaponVelocityOld = new Dictionary<ModuleWeaponGun, float>();
        public static FieldInfo field_Velocity = typeof(FireData)
            .GetField("m_MuzzleVelocity", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* WHEELS : MAX RPM */
        public Dictionary<ModuleBuff, int> wheelsRpmBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWheels, float> wheelsRpmOld = new Dictionary<ModuleWheels, float>();
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
        public Dictionary<ModuleBuff, int> wheelsBrakeBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWheels, List<float>> wheelsBrakeOld = new Dictionary<ModuleWheels, List<float>>(); // [0] = passiveBrakeMaxTorque, [1] = basicFrictionTorque

        /* WHEELS : MAX TORQUE */
        public Dictionary<ModuleBuff, int> wheelsTorqueBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWheels, float> wheelsTorqueOld = new Dictionary<ModuleWheels, float>(); // [0] = passiveBrakeMaxTorque, [1] = basicFrictionTorque

        /* BOOSTER : BURN RATE */
        public Dictionary<ModuleBuff, int> boosterBurnRateBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleBooster, float> boosterBurnRateOld = new Dictionary<ModuleBooster, float>();
        public static FieldInfo field_Jets = typeof(ModuleBooster)
            .GetField("jets", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_BurnRate = typeof(BoosterJet)
            .GetField("m_BurnRate", BindingFlags.NonPublic | BindingFlags.Instance);

        /* SHIELD : RADIUS */
        /*public Dictionary<ModuleBuff, int> shieldRadiusBuffBlocks = new List<ModuleBuff>();
        public Dictionary<ModuleShieldGenerator, float> shieldRadiusOld = new Dictionary<ModuleShieldGenerator, float>();
        public static FieldInfo field_Radius = typeof(ModuleShieldGenerator)
            .GetField("m_Radius", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_State = typeof(ModuleShieldGenerator)
            .GetField("m_State", BindingFlags.NonPublic | BindingFlags.Instance);*/

        /* DRILL : DAMAGE PER SECOND */
        public Dictionary<ModuleBuff, int> drillDpsBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleDrill, float> drillDpsOld = new Dictionary<ModuleDrill, float>();
        public static FieldInfo field_Dps = typeof(ModuleDrill)
            .GetField("damagePerSecond", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* ENERGY : OUTPUT PER SECOND */
        public Dictionary<ModuleBuff, int> energyOpsBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleEnergy, float> energyOpsOld = new Dictionary<ModuleEnergy, float>();
        public static FieldInfo field_Ops = typeof(ModuleEnergy)
            .GetField("m_OutputPerSecond", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* ENERGY STORE : CAPACITY */
        public Dictionary<ModuleBuff, int> energyStoreCapBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleEnergyStore, float> energyStoreCapOld = new Dictionary<ModuleEnergyStore, float>();
        public static FieldInfo field_Capacity = typeof(ModuleEnergyStore)
            .GetField("m_Capacity", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static PropertyInfo prop_Current = typeof(ModuleEnergyStore)
            .GetProperty("CurrentAmount", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* ITEM CONSUME : NEEDS ANCHORED */
        public Dictionary<ModuleBuff, bool> itemConAnchorBuffBlocks = new Dictionary<ModuleBuff, bool>();
        public Dictionary<ModuleItemConsume, bool> itemConAnchorOld = new Dictionary<ModuleItemConsume, bool>();
        public bool ItemConAnchorFixed
        {
            get
            {
                return itemConAnchorBuffBlocks.ContainsValue(true);
            }
        } // Priority on True
        public static FieldInfo field_NeedsAnchor = typeof(ModuleItemConsume)
            .GetField("m_NeedsToBeAnchored", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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

        public static void RemoveObject(BuffController obj) // Todo: Make "cleaning" function
        {
            BuffController.allControllers.Remove(obj);
        }

        public float GetBuffAverage(string prop)
        {
            FieldInfo field_LocalProp = this.GetType().GetField(prop);
            Dictionary<ModuleBuff, int> value_LocalProp = (Dictionary<ModuleBuff, int>)field_LocalProp.GetValue(this);

            float d = 1.0f;
            //List<float> allMults = (List<float>)value_LocalProp.Select(x => x.Value.Strength(x)).ToList();

            List<float> allMults = (List<float>)value_LocalProp.Select(x => x.Key.Strength(x.Key, x.Value)).ToList();

            if (allMults.Count > 0)
            { 
                d = allMults.Average();
            }
            return (value_LocalProp.Count > 0) ? d : 1.0f;

            //return (value_LocalProp.Count > 0) ? value_LocalProp.Values.Average() : 1.0f;
        }

        public void Update(string[] type)
        {
            // Use "All" to update all, use m_BuffType to update specifics
            if (type.Contains("WeaponCooldown") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * this.GetBuffAverage("weaponCooldownBuffBlocks"));
                    field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * this.GetBuffAverage("weaponCooldownBuffBlocks"));
                    ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                    field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0] * this.GetBuffAverage("weaponCooldownBuffBlocks"));
                }
            }
            if (type.Contains("WeaponRotation") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                    field_Rotation.SetValue(value_ModuleWeapon, this.weaponRotationOld[weapon] * this.GetBuffAverage("weaponRotationBuffBlocks"));
                }
            }
            if (type.Contains("WeaponSpread") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                    field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon] * this.GetBuffAverage("weaponSpreadBuffBlocks"));
                }
            }
            if (type.Contains("WeaponVelocity") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                    field_Velocity.SetValue(value_FiringData, this.weaponVelocityOld[weapon] * this.GetBuffAverage("weaponVelocityBuffBlocks"));
                }
            }
            if (type.Contains("WheelsRpm") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * this.GetBuffAverage("wheelsRpmBuffBlocks");
                    this.RefreshWheels(wheels, torque);
                }
            }
            if (type.Contains("WheelsBrake") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0] * this.GetBuffAverage("wheelsBrakeBuffBlocks");
                    torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1] * this.GetBuffAverage("wheelsBrakeBuffBlocks");
                    this.RefreshWheels(wheels, torque);
                }
            }
            if (type.Contains("WheelsTorque") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels] * this.GetBuffAverage("wheelsTorqueBuffBlocks");
                    this.RefreshWheels(wheels, torque);
                }
            }
            if (type.Contains("BoosterBurnRate") || type.Contains("All"))
            {
                foreach (ModuleBooster booster in this.boosterList)
                {
                    List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
                    foreach (BoosterJet jet in value_Jets)
                    {
                        field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster] * this.GetBuffAverage("boosterBurnRateBuffBlocks"));
                    }
                }
            }
            /*if (type.Contains("ShieldRadius") || type.Contains("All"))
            {
                foreach (ModuleShieldGenerator shield in this.shieldList)
                {
                    Type stateEnum = field_State.GetValue(shield).GetType();
                    field_Radius.SetValue(shield, this.shieldRadiusOld[shield] * this.GetBuffAverage("shieldRadiusBuffBlocks"));
                    field_State.SetValue(shield, Enum.ToObject(stateEnum, 0));
                }
            }*/
            if (type.Contains("DrillDps") || type.Contains("All"))
            {
                foreach (ModuleDrill drill in this.drillList)
                {
                    field_Dps.SetValue(drill, this.drillDpsOld[drill] * this.GetBuffAverage("drillDpsBuffBlocks"));
                }
            }
            if (type.Contains("EnergyOps") || type.Contains("All"))
            {
                foreach (ModuleEnergy energy in this.energyList)
                {
                    field_Ops.SetValue(energy, this.energyOpsOld[energy] * this.GetBuffAverage("energyOpsBuffBlocks"));
                }
            }
            if (type.Contains("EnergyStoreCap") || type.Contains("All"))
            {
                foreach (ModuleEnergyStore store in this.energyStoreList)
                {
                    if (this.energyStoreCapBuffBlocks.Count > 0 && !type.Contains("Anchor"))
                    {
                        prop_Current.SetValue(store, 0.0f);
                    }
                    field_Capacity.SetValue(store, this.energyStoreCapOld[store] * this.GetBuffAverage("energyStoreCapBuffBlocks"));
                }
            }
            if (type.Contains("ItemConAnchored") || type.Contains("All"))
            {
                foreach (ModuleItemConsume item in this.itemConList)
                {
                    if (itemConAnchorBuffBlocks.Count > 0)
                    {
                        field_NeedsAnchor.SetValue(item, ItemConAnchorFixed);
                    }
                    else
                    {
                        field_NeedsAnchor.SetValue(item, this.itemConAnchorOld[item]);
                    }
                }
            }
        }

        public void AddBuff(ModuleBuff buff)
        {
            List<string> effects = buff.AllEffects;
            if (effects.Contains("WeaponCooldown"))
            {
                this.weaponCooldownBuffBlocks.Add(buff, buff.GetEffect("WeaponCooldown"));
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.weaponRotationBuffBlocks.Add(buff, buff.GetEffect("WeaponRotation"));
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.weaponSpreadBuffBlocks.Add(buff, buff.GetEffect("WeaponSpread"));
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.weaponVelocityBuffBlocks.Add(buff, buff.GetEffect("WeaponVelocity"));
            }
            if (effects.Contains("WheelsRpm"))
            {
                this.wheelsRpmBuffBlocks.Add(buff, buff.GetEffect("WheelsRpm"));
            }
            if (effects.Contains("WheelsBrake"))
            {
                this.wheelsBrakeBuffBlocks.Add(buff, buff.GetEffect("WheelsBrake"));
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.wheelsTorqueBuffBlocks.Add(buff, buff.GetEffect("WheelsTorque"));
            }
            if (effects.Contains("BoosterBurnRate"))
            {
                this.boosterBurnRateBuffBlocks.Add(buff, buff.GetEffect("BoosterBurnRate"));
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
                            }
                        }
                    }
                }
            }
            /*if (effects.Contains("ShieldRadius")) 
            {
                this.shieldRadiusBuffBlocks.Add(buff, buff.GetEffect("WeaponCooldown"));
            }*/
            if (effects.Contains("DrillDps"))
            {
                this.drillDpsBuffBlocks.Add(buff, buff.GetEffect("DrillDps"));
            }
            if (effects.Contains("EnergyOps"))
            {
                this.energyOpsBuffBlocks.Add(buff, buff.GetEffect("EnergyOps"));
            }
            if (effects.Contains("EnergyStoreCap"))
            {
                this.energyStoreCapBuffBlocks.Add(buff, buff.GetEffect("EnergyStoreCap"));
            }
            if (effects.Contains("ItemConAnchored"))
            {
                //this.itemConAnchorBuffBlocks.Add(buff, buff.m_Strength == 1.0f); // true if 1, false if not
                this.itemConAnchorBuffBlocks.Add(buff, false); // true if 1, false if not
            }
            //this.buffBlocksNeedsAnchor.Add(buff, buff.m_NeedsToBeAnchored);
            this.Update(buff.m_BuffType);
            //this.Update(new string[] { buff.m_BuffType });
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            List<string> effects = buff.AllEffects;
            if (effects.Contains("WeaponCooldown"))
            {
                this.weaponCooldownBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.weaponRotationBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.weaponSpreadBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.weaponVelocityBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsRpm"))
            {
                this.wheelsRpmBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsBrake"))
            {
                this.wheelsBrakeBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.wheelsTorqueBuffBlocks.Remove(buff);
            }
            if (effects.Contains("BoosterBurnRate"))
            {
                this.boosterBurnRateBuffBlocks.Remove(buff);
            }
            /*if (effects.Contains("ShieldRadius")) {
                this.shieldRadiusBuffBlocks.Remove(buff);
            }*/
            if (effects.Contains("DrillDps"))
            {
                this.drillDpsBuffBlocks.Remove(buff);
            }
            if (effects.Contains("EnergyOps"))
            {
                this.energyOpsBuffBlocks.Remove(buff);
            }
            if (effects.Contains("EnergyStoreCap"))
            {
                this.energyStoreCapBuffBlocks.Remove(buff);
            }
            if (effects.Contains("ItemConAnchored"))
            {
                this.itemConAnchorBuffBlocks.Remove(buff);
            }
            this.Update(buff.m_BuffType);
            //this.Update(new string[] { buff.m_BuffType });
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
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
            this.weaponVelocityOld.Add(weapon, (float)field_Velocity.GetValue(value_FiringData));

            this.Update(new string[] { "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity" });
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
            field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
            ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
            field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0]);
            FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
            field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon]);
            field_Velocity.SetValue(value_FiringData, this.weaponVelocityOld[weapon]);

            this.weaponList.Remove(weapon);
            this.weaponCooldownOld.Remove(weapon);
            this.weaponRotationOld.Remove(weapon);
            this.weaponSpreadOld.Remove(weapon);
            this.weaponVelocityOld.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
            this.wheelsList.Add(wheels);
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            this.wheelsRpmOld.Add(wheels, torque.torqueCurveMaxRpm);
            this.wheelsBrakeOld.Add(wheels, new List<float>()
            {
                torque.passiveBrakeMaxTorque,
                torque.basicFrictionTorque
            });
            this.wheelsTorqueOld.Add(wheels, torque.torqueCurveMaxTorque);
            
            this.Update(new string[] { "WheelsRpm", "WheelsBrake", "WheelsTorque"});
            //this.RefreshWheels(wheels, torque);
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
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
            this.boosterList.Add(booster);
            List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
            foreach (BoosterJet jet in value_Jets)
            {
                if (!boosterBurnRateOld.ContainsKey(booster))
                { 
                    float value_BurnRate = (float)field_BurnRate.GetValue(jet);
                    this.boosterBurnRateOld.Add(booster, value_BurnRate);
                }
            }

            this.Update(new string[] { "BoosterBurnRate" });
        }

        public void RemoveBooster(ModuleBooster booster)
        {
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

        /*public void AddShield(ModuleShieldGenerator shield)
        {
            this.shieldList.Add(shield);
            float value_Radius = (float)field_Radius.GetValue(shield);
            this.shieldRadiusOld.Add(shield, value_Radius);
            
            this.Update(new string[] { "ShieldRadius" });
        }


        public void RemoveShield(ModuleShieldGenerator shield)
        {
            field_Radius.SetValue(shield, this.shieldRadiusOld[shield]);
            this.shieldList.Remove(shield);
            this.shieldRadiusOld.Remove(shield);
        }*/

        public void AddDrill(ModuleDrill drill)
        {
            this.drillList.Add(drill);
            this.drillDpsOld.Add(drill, (float)field_Dps.GetValue(drill));

            this.Update(new string[] { "DrillDps" });
        }

        public void RemoveDrill(ModuleDrill drill)
        {
            field_Dps.SetValue(drill, this.drillDpsOld[drill]);
            this.drillList.Remove(drill);
            this.drillDpsOld.Remove(drill);
        }

        public void AddEnergy(ModuleEnergy energy)
        {
            this.energyList.Add(energy);
            this.energyOpsOld.Add(energy, (float)field_Ops.GetValue(energy));

            this.Update(new string[] { "EnergyOps" });
        }

        public void RemoveEnergy(ModuleEnergy energy)
        {
            field_Ops.SetValue(energy, this.energyOpsOld[energy]);
            this.energyList.Remove(energy);
            this.energyOpsOld.Remove(energy);
        }

        public void AddEnergyStore(ModuleEnergyStore store)
        {
            this.energyStoreList.Add(store);
            this.energyStoreCapOld.Add(store, (float)field_Capacity.GetValue(store));
            
            this.Update(new string[] { "EnergyStoreCap" });
        }

        public void RemoveEnergyStore(ModuleEnergyStore store)
        {
            if (this.drillDpsBuffBlocks.Count > 0)
            {
                prop_Current.SetValue(store, 0.0f);
                field_Capacity.SetValue(store, this.energyStoreCapOld[store]);
            }
            this.energyStoreList.Remove(store);
            this.energyStoreCapOld.Remove(store);
        }

        public void AddItemCon(ModuleItemConsume item)
        {
            this.itemConList.Add(item);
            this.itemConAnchorOld.Add(item, (bool)field_NeedsAnchor.GetValue(item));

            this.Update(new string[] { "ItemConAnchored" });
        }

        public void RemoveItemCon(ModuleItemConsume item)
        {
            field_NeedsAnchor.SetValue(item, this.itemConAnchorOld[item]);
            this.itemConList.Remove(item);
            this.itemConAnchorOld.Remove(item);
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

                    //FieldInfo field_p_WheelParams = value_AttachedWheelState.GetType()
                    //    .GetField("wheelParams", BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    //field_p_WheelParams.SetValue(value_AttachedWheelState, wheelparams);

                    wheel.UpdateAttachData(moduleData); // Update it! Live! Do it!
                                                        // Also logs "only for use in Editor" error, annoying...
                                                        
                }
            }
        }
    }
}
