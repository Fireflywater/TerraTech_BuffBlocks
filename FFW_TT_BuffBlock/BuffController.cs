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
        /* TEST PROPERTIES */
        public static Dictionary<string, string[]> allEffects = new Dictionary<string, string[]> {
            { "WeaponCooldown" , new string[] { "m_WeaponModule.m_ShotCooldown", "m_ShotCooldown", "m_BurstCooldown" } },
            { "WeaponRotation" , new string[] { "m_WeaponModule.m_RotateSpeed" } },
            { "WeaponSpread" , new string[] { "m_FiringData.m_BulletSprayVariance" } },
            { "WeaponVelocity" , new string[] { "m_FiringData.m_MuzzleVelocity" } },

            { "WheelsRpm" , new string[] { "m_TorqueParams.torqueCurveMaxRpm" } },

            { "HoverForce" , new string[] { "jets.forceMax" } },
            { "HoverRange" , new string[] { "jets.forceRangeMax" } },
            { "HoverDamping" , new string[] { "jets.m_DampingScale" } },
            { "BoosterBurnRate" , new string[] { "jets.m_BurnRate" } },
            { "ItemProSpeed" , new string[] { "m_SecPerItemProduced", "m_MinDispenseInterval" } }
        };
        public Dictionary<string, BuffSegment> allSegments = new Dictionary<string, BuffSegment>();
        public List<object> weaponListGeneric = new List<object>();
        public List<object> wheelsListGeneric = new List<object>();
        public List<object> hoverListGeneric = new List<object>();
        public List<object> boosterListGeneric = new List<object>();
        public List<object> itemProListGeneric = new List<object>();

        /* PRIME PROPERTIES */
        public static List<BuffController> allControllers = new List<BuffController>();
        public Tank tank;
        public List<ModuleWheels> wheelsList = new List<ModuleWheels>();
        public List<ModuleShieldGenerator> shieldList = new List<ModuleShieldGenerator>();
        public List<ModuleDrill> drillList = new List<ModuleDrill>();
        public List<ModuleEnergy> energyList = new List<ModuleEnergy>();
        public List<ModuleEnergyStore> energyStoreList = new List<ModuleEnergyStore>();
        public List<ModuleItemConsume> itemConList = new List<ModuleItemConsume>();
        public List<ModuleHeart> heartList = new List<ModuleHeart>();
        public List<ModuleItemPickup> itemPickupList = new List<ModuleItemPickup>();
        public List<ModuleItemProducer> itemProList = new List<ModuleItemProducer>();
        

        /* WEAPON : DAMAGE */
        /*public Dictionary<ModuleBuff, int> weaponDamageBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWeaponGun, List<float>> weaponDamageOld = new Dictionary<ModuleWeaponGun, List<float>>(); // [0] = Projectile.m_Damage, Projectile.m_Explosion.m_MaxDamageStrength
        public static FieldInfo field_Damage = typeof(Projectile)
            .GetField("m_Damage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_Explosion = typeof(Projectile)
            .GetField("m_Explosion", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_ExplDamage = typeof(Explosion)
            .GetField("m_MaxDamageStrength", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_CannonBarrels = typeof(ModuleWeaponGun)
            .GetField("m_CannonBarrels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_CannonBarrelFiringData = typeof(CannonBarrel)
            .GetField("m_FiringData", BindingFlags.NonPublic | BindingFlags.Instance);*/

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
        public Dictionary<ModuleWheels, float> wheelsTorqueOld = new Dictionary<ModuleWheels, float>();

        /* WHEELS : GRIP */
        public Dictionary<ModuleBuff, int> wheelsGripBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWheels, List<float>> wheelsGripOld = new Dictionary<ModuleWheels, List<float>>(); //[0] = gripFactorLong, [1] = gripFactorLat

        /* WHEELS : SUSPENSION */
        public Dictionary<ModuleBuff, int> wheelsSuspensionBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleWheels, List<float>> wheelsSuspensionOld = new Dictionary<ModuleWheels, List<float>>(); //[0] = suspensionSpring, [1] = suspensionDamper

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
        public bool ItemConAnchorFixed { get { return itemConAnchorBuffBlocks.ContainsValue(true); } } // Priority on True
        public static FieldInfo field_ItemConNeedsAnchor = typeof(ModuleItemConsume)
            .GetField("m_NeedsToBeAnchored", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* HEART SCU : NEEDS ANCHORED */
        public Dictionary<ModuleBuff, bool> heartAnchorBuffBlocks = new Dictionary<ModuleBuff, bool>();
        public Dictionary<ModuleHeart, bool> heartAnchorOld = new Dictionary<ModuleHeart, bool>();
        public bool HeartAnchorFixed { get { return itemConAnchorBuffBlocks.ContainsValue(true); } } // Priority on True
        public static FieldInfo field_HeartNeedsAnchor = typeof(ModuleHeart)
            .GetField("m_HasAnchor", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* ITEM PICKUP : RANGE */
        public Dictionary<ModuleBuff, int> itemPickupRangeBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleItemPickup, float> itemPickupRangeOld = new Dictionary<ModuleItemPickup, float>();
        public static FieldInfo field_ItemPickupRange = typeof(ModuleItemPickup)
            .GetField("m_PickupRange", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* ITEM PRODUCE : SPEED */
        public Dictionary<ModuleBuff, int> itemProSpeedBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleItemProducer, List<float>> itemProSpeedOld = new Dictionary<ModuleItemProducer, List<float>>(); //[0] = m_SecPerItemProduced, [1] = m_MinDispenseInterval
        public static FieldInfo field_ItemProSpeed1 = typeof(ModuleItemProducer)
            .GetField("m_SecPerItemProduced", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_ItemProSpeed2 = typeof(ModuleItemProducer)
            .GetField("m_MinDispenseInterval", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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
            /*for (int i = 0; i < BuffController.allEffects.Length; i++)
            {
                BuffSegment newSegment = new BuffSegment
                {
                    tank = newObject.tank,
                    controller = newObject,
                    effectType = BuffController.allEffects[i],
                    effectPaths = BuffController.allPaths[i]
                };
                newObject.allSegments.Add(BuffController.allEffects[i], newSegment);
            }*/
            foreach (KeyValuePair<string, string[]> entry in BuffController.allEffects)
            {
                BuffSegment newSegment = new BuffSegment
                {
                    tank = newObject.tank,
                    controller = newObject,
                    effectType = entry.Key,
                    effectPaths = entry.Value
                };
                newObject.allSegments.Add(entry.Key, newSegment);
                //Console.WriteLine("FFW: Entry Key Type: " + entry.Key.GetType().Name + " / Entry Value Length: " + entry.Value.Length);
            }
            BuffController.AddObject(newObject);
            Console.WriteLine("FFW: Active BuffControls: " + BuffController.allControllers.Count);
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

            float m = 1.0f;
            List<float> allMults = (List<float>)value_LocalProp.Select(x => x.Key.Strength(x.Key, x.Value)).ToList();
            if (allMults.Count > 0)
            { 
                m = allMults.Average();
            }

            return (value_LocalProp.Count > 0) ? m : 1.0f;
            
        }

        public float GetBuffAddAverage(string prop)
        {
            FieldInfo field_LocalProp = this.GetType().GetField(prop);
            Dictionary<ModuleBuff, int> value_LocalProp = (Dictionary<ModuleBuff, int>)field_LocalProp.GetValue(this);

            float a = 0.0f;
            List<float> allAdds = (List<float>)value_LocalProp.Select(x => x.Key.AddAfter(x.Key, x.Value)).ToList();
            if (allAdds.Count > 0)
            {
                a = allAdds.Average();
            }

            return (value_LocalProp.Count > 0) ? a : 0.0f;
        }

        public void Update(string[] type)
        {
            //this.allSegments["WeaponCooldown"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponRotation"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponSpread"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponVelocity"].UpdateObject(this.weaponListGeneric);
            this.allSegments["HoverForce"].UpdateObject(this.hoverListGeneric);
            this.allSegments["HoverRange"].UpdateObject(this.hoverListGeneric);
            this.allSegments["HoverDamping"].UpdateObject(this.hoverListGeneric);
            this.allSegments["BoosterBurnRate"].UpdateObject(this.boosterListGeneric);
            this.allSegments["ItemProSpeed"].UpdateObject(this.itemProListGeneric);
            // Use "All" to update all, use m_BuffType to update specifics
            //this.allSegments["WheelsRpm"].UpdateObject(this.wheelsListGeneric);

            this.allSegments["WeaponCooldown"].ManipulateObj(this.weaponListGeneric, "UPDATE");
            this.allSegments["WheelsRpm"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.RefreshWheels2(this.wheelsList);

            /*if (type.Contains("WheelsRpm") ||
                type.Contains("WheelsBrake") ||
                type.Contains("WheelsTorque") ||
                type.Contains("WheelsGrip") ||
                type.Contains("WheelsSuspension") ||
                type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * this.GetBuffAverage("wheelsRpmBuffBlocks") + this.GetBuffAddAverage("wheelsRpmBuffBlocks");
                    
                    torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0] * this.GetBuffAverage("wheelsBrakeBuffBlocks") + this.GetBuffAddAverage("wheelsBrakeBuffBlocks");
                    torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1] * this.GetBuffAverage("wheelsBrakeBuffBlocks") + this.GetBuffAddAverage("wheelsBrakeBuffBlocks");
                    
                    torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels] * this.GetBuffAverage("wheelsTorqueBuffBlocks") + this.GetBuffAddAverage("wheelsTorqueBuffBlocks");
                    
                    wheelparams.tireProperties.props.gripFactorLong = this.wheelsGripOld[wheels][0] * this.GetBuffAverage("wheelsGripBuffBlocks") + this.GetBuffAddAverage("wheelsGripBuffBlocks");
                    //wheelparams.tireProperties.props.gripFactorLat = this.wheelsGripOld[wheels][1] * this.GetBuffAverage("wheelsGripBuffBlocks") + this.GetBuffAddAverage("wheelsGripBuffBlocks");
                    
                    wheelparams.suspensionSpring = this.wheelsSuspensionOld[wheels][0] * this.GetBuffAverage("wheelsSuspensionBuffBlocks") + this.GetBuffAddAverage("wheelsSuspensionBuffBlocks");
                    wheelparams.suspensionDamper = this.wheelsSuspensionOld[wheels][1] * this.GetBuffAverage("wheelsSuspensionBuffBlocks") + this.GetBuffAddAverage("wheelsSuspensionBuffBlocks");

                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }*/
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
                    field_Dps.SetValue(drill, this.drillDpsOld[drill] * this.GetBuffAverage("drillDpsBuffBlocks") + this.GetBuffAddAverage("drillDpsBuffBlocks"));
                }
            }
            if (type.Contains("EnergyOps") || type.Contains("All"))
            {
                foreach (ModuleEnergy energy in this.energyList)
                {
                    field_Ops.SetValue(energy, this.energyOpsOld[energy] * this.GetBuffAverage("energyOpsBuffBlocks") + this.GetBuffAddAverage("energyOpsBuffBlocks"));
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
                    field_Capacity.SetValue(store, this.energyStoreCapOld[store] * this.GetBuffAverage("energyStoreCapBuffBlocks") + this.GetBuffAddAverage("energyStoreCapBuffBlocks"));
                }
            }
            if (type.Contains("ItemConAnchored") || type.Contains("All"))
            {
                foreach (ModuleItemConsume item in this.itemConList)
                {
                    if (itemConAnchorBuffBlocks.Count > 0)
                    {
                        field_ItemConNeedsAnchor.SetValue(item, ItemConAnchorFixed);
                    }
                    else
                    {
                        field_ItemConNeedsAnchor.SetValue(item, this.itemConAnchorOld[item]);
                    }
                }
            }
            if (type.Contains("HeartAnchored") || type.Contains("All"))
            {
                foreach (ModuleHeart heart in this.heartList)
                {
                    if (heartAnchorBuffBlocks.Count > 0)
                    {
                        field_HeartNeedsAnchor.SetValue(heart, HeartAnchorFixed);
                    }
                    else
                    {
                        field_HeartNeedsAnchor.SetValue(heart, this.heartAnchorOld[heart]);
                    }
                }
            }
            if (type.Contains("ItemPickupRange") || type.Contains("All"))
            {
                foreach (ModuleItemPickup item in this.itemPickupList)
                {
                    field_ItemPickupRange.SetValue(item, this.itemPickupRangeOld[item] * this.GetBuffAverage("itemPickupRangeBuffBlocks") + this.GetBuffAddAverage("itemPickupRangeBuffBlocks"));
                }
            }
            /*if (type.Contains("ItemProSpeed") || type.Contains("All"))
            {
                foreach (ModuleItemProducer item in this.itemProList)
                {
                    field_ItemProSpeed1.SetValue(item, this.itemProSpeedOld[item][0] * this.GetBuffAverage("itemProSpeedBuffBlocks") + this.GetBuffAddAverage("itemProSpeedBuffBlocks"));
                    field_ItemProSpeed2.SetValue(item, this.itemProSpeedOld[item][1] * this.GetBuffAverage("itemProSpeedBuffBlocks") + this.GetBuffAddAverage("itemProSpeedBuffBlocks"));
                }
            }*/
            /*if (type.Contains("HoverForce") ||
                type.Contains("HoverRange") ||
                type.Contains("HoverDamping") ||
                type.Contains("All"))
            {
                foreach (ModuleHover hover in this.hoverList)
                {
                    List<HoverJet> value_HoverJets = (List<HoverJet>)field_HoverJets.GetValue(hover);
                    foreach (HoverJet jet in value_HoverJets)
                    {
                        field_ForceMax.SetValue(jet, this.hoverForceOld[hover] * this.GetBuffAverage("hoverForceBuffBlocks") + this.GetBuffAddAverage("hoverForceBuffBlocks"));
                        field_ForceRangeMax.SetValue(jet, this.hoverRangeOld[hover] * this.GetBuffAverage("hoverRangeBuffBlocks") + this.GetBuffAddAverage("hoverRangeBuffBlocks"));
                        field_Damping.SetValue(jet, this.hoverDampingOld[hover] * this.GetBuffAverage("hoverDampingBuffBlocks") + this.GetBuffAddAverage("hoverRangeBuffBlocks"));
                    }
                }
            }*/
        }

        public void AddBuff(ModuleBuff buff)
        {
            List<string> effects = buff.AllEffects;
            if (effects.Contains("WeaponCooldown"))
            {
                this.allSegments["WeaponCooldown"].AddBuff(buff);
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.allSegments["WeaponRotation"].AddBuff(buff);
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.allSegments["WeaponSpread"].AddBuff(buff);
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.allSegments["WeaponVelocity"].AddBuff(buff);
            }
            /*if (effects.Contains("WeaponDamage"))
            {
                this.weaponDamageBuffBlocks.Add(buff, buff.GetEffect("WeaponDamage"));
            }*/
            if (effects.Contains("WheelsRpm"))
            {
                this.allSegments["WheelsRpm"].AddBuff(buff);
                //this.wheelsRpmBuffBlocks.Add(buff, buff.GetEffect("WheelsRpm"));
            }
            /*if (effects.Contains("WheelsBrake"))
            {
                this.wheelsBrakeBuffBlocks.Add(buff, buff.GetEffect("WheelsBrake"));
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.wheelsTorqueBuffBlocks.Add(buff, buff.GetEffect("WheelsTorque"));
            }
            if (effects.Contains("WheelsGrip"))
            {
                this.wheelsGripBuffBlocks.Add(buff, buff.GetEffect("WheelsGrip"));
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.wheelsSuspensionBuffBlocks.Add(buff, buff.GetEffect("WheelsSuspension"));
            }*/
            if (effects.Contains("BoosterBurnRate"))
            {
                this.allSegments["BoosterBurnRate"].AddBuff(buff);
                /*this.boosterBurnRateBuffBlocks.Add(buff, buff.GetEffect("BoosterBurnRate"));
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
                }*/
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
            if (effects.Contains("HeartAnchored"))
            {
                this.heartAnchorBuffBlocks.Add(buff, false);
            }
            if (effects.Contains("ItemPickupRange"))
            {
                this.itemPickupRangeBuffBlocks.Add(buff, buff.GetEffect("ItemPickupRange"));
            }
            if (effects.Contains("ItemProSpeed"))
            {
                this.allSegments["ItemProSpeed"].AddBuff(buff);
                //this.itemProSpeedBuffBlocks.Add(buff, buff.GetEffect("ItemProSpeed"));
            }
            if (effects.Contains("HoverForce"))
            {
                this.allSegments["HoverForce"].AddBuff(buff);
            }
            if (effects.Contains("HoverRange"))
            {
                this.allSegments["HoverRange"].AddBuff(buff);
            }
            if (effects.Contains("HoverDamping"))
            {
                this.allSegments["HoverDamping"].AddBuff(buff);
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
                this.allSegments["WeaponCooldown"].RemoveBuff(buff);
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.allSegments["WeaponRotation"].RemoveBuff(buff);
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.allSegments["WeaponSpread"].RemoveBuff(buff);
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.allSegments["WeaponVelocity"].RemoveBuff(buff);
            }
            /*if (effects.Contains("WeaponDamage"))
            {
                this.weaponDamageBuffBlocks.Remove(buff);
            }*/
            if (effects.Contains("WheelsRpm"))
            {
                this.allSegments["WheelsRpm"].RemoveBuff(buff);
                //this.wheelsRpmBuffBlocks.Remove(buff);
            }
            /*if (effects.Contains("WheelsBrake"))
            {
                this.wheelsBrakeBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.wheelsTorqueBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsGrip"))
            {
                this.wheelsGripBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.wheelsSuspensionBuffBlocks.Remove(buff);
            }*/
            if (effects.Contains("BoosterBurnRate"))
            {
                this.allSegments["BoosterBurnRate"].RemoveBuff(buff);
                //this.boosterBurnRateBuffBlocks.Remove(buff);
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
            if (effects.Contains("HeartAnchored"))
            {
                this.heartAnchorBuffBlocks.Remove(buff);
            }
            if (effects.Contains("ItemPickupRange"))
            {
                this.itemPickupRangeBuffBlocks.Remove(buff);
            }
            if (effects.Contains("ItemProSpeed"))
            {
                this.allSegments["ItemProSpeed"].RemoveBuff(buff);
                //this.itemProSpeedBuffBlocks.Remove(buff);
            }
            if (effects.Contains("HoverForce"))
            {
                this.allSegments["HoverForce"].RemoveBuff(buff);
            }
            if (effects.Contains("HoverRange"))
            {
                this.allSegments["HoverRange"].RemoveBuff(buff);
            }
            if (effects.Contains("HoverDamping"))
            {
                this.allSegments["HoverDamping"].RemoveBuff(buff);
            }
            this.Update(buff.m_BuffType);
            //this.Update(new string[] { buff.m_BuffType });
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            this.weaponListGeneric.Add(weapon);
            /*this.allSegments["WeaponCooldown"].SaveObject(weapon);
            this.allSegments["WeaponRotation"].SaveObject(weapon);
            this.allSegments["WeaponSpread"].SaveObject(weapon);
            this.allSegments["WeaponVelocity"].SaveObject(weapon);*/

            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "UPDATE");

            //this.Update(new string[] { "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity" });
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            /*this.allSegments["WeaponCooldown"].CleanObject(weapon);
            this.allSegments["WeaponRotation"].CleanObject(weapon);
            this.allSegments["WeaponSpread"].CleanObject(weapon);
            this.allSegments["WeaponVelocity"].CleanObject(weapon);*/


            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.weaponListGeneric.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
            this.wheelsListGeneric.Add(wheels);
            this.wheelsList.Add(wheels);
            //this.allSegments["WheelsRpm"].SaveObject(wheels);

            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "SAVE");
            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "UPDATE");
            this.RefreshWheels2( new List<ModuleWheels> { wheels });
            /*
            this.wheelsList.Add(wheels);
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
            this.wheelsRpmOld.Add(wheels, torque.torqueCurveMaxRpm);
            this.wheelsBrakeOld.Add(wheels, new List<float>()
            {
                torque.passiveBrakeMaxTorque,
                torque.basicFrictionTorque
            });
            this.wheelsTorqueOld.Add(wheels, torque.torqueCurveMaxTorque);
            this.wheelsGripOld.Add(wheels, new List<float>()
            {
                wheelparams.tireProperties.props.gripFactorLong,
                wheelparams.tireProperties.props.gripFactorLat
            });
            this.wheelsSuspensionOld.Add(wheels, new List<float>()
            {
                wheelparams.suspensionSpring,
                wheelparams.suspensionDamper
            });

            this.Update(new string[] { "WheelsRpm", "WheelsBrake", "WheelsTorque", "WheelsGrip", "WheelsSuspension" });*/
            //this.RefreshWheels(wheels, torque);
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            //this.allSegments["WheelsRpm"].CleanObject(wheels);
            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.RefreshWheels2(new List<ModuleWheels> { wheels });

            this.wheelsListGeneric.Remove(wheels);
            this.wheelsList.Remove(wheels);

            /*ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
            ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
            torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels];
            torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0];
            torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1];
            torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels];
            wheelparams.tireProperties.props.gripFactorLong = this.wheelsGripOld[wheels][0];
            //wheelparams.tireProperties.props.gripFactorLat = this.wheelsGripOld[wheels][1];
            wheelparams.suspensionSpring = this.wheelsSuspensionOld[wheels][0];
            wheelparams.suspensionDamper = this.wheelsSuspensionOld[wheels][1];

            this.RefreshWheels(wheels, torque, wheelparams);
            this.wheelsList.Remove(wheels);
            this.wheelsRpmOld.Remove(wheels);
            this.wheelsBrakeOld.Remove(wheels);
            this.wheelsTorqueOld.Remove(wheels);
            this.wheelsGripOld.Remove(wheels);
            this.wheelsSuspensionOld.Remove(wheels);*/
        }

        public void AddBooster(ModuleBooster booster)
        {
            this.boosterListGeneric.Add(booster);
            this.allSegments["BoosterBurnRate"].SaveObject(booster);

            this.Update(new string[] { "BoosterBurnRate" });
        }

        public void RemoveBooster(ModuleBooster booster)
        {
            this.allSegments["BoosterBurnRate"].CleanObject(booster);
            this.boosterListGeneric.Remove(booster);
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
            this.itemConAnchorOld.Add(item, (bool)field_ItemConNeedsAnchor.GetValue(item));

            this.Update(new string[] { "ItemConAnchored" });
        }

        public void RemoveItemCon(ModuleItemConsume item)
        {
            field_ItemConNeedsAnchor.SetValue(item, this.itemConAnchorOld[item]);
            this.itemConList.Remove(item);
            this.itemConAnchorOld.Remove(item);
        }

        public void AddHeart(ModuleHeart heart)
        {
            this.heartList.Add(heart);
            this.heartAnchorOld.Add(heart, (bool)field_HeartNeedsAnchor.GetValue(heart));

            this.Update(new string[] { "HeartAnchored" });
        }

        public void RemoveHeart(ModuleHeart heart)
        {
            field_HeartNeedsAnchor.SetValue(heart, this.heartAnchorOld[heart]);
            this.heartList.Remove(heart);
            this.heartAnchorOld.Remove(heart);
        }

        public void AddItemPickup(ModuleItemPickup item)
        {
            this.itemPickupList.Add(item);
            this.itemPickupRangeOld.Add(item, (float)field_ItemPickupRange.GetValue(item));

            this.Update(new string[] { "ItemPickupRange" });
        }

        public void RemoveItemPickup(ModuleItemPickup item)
        {
            field_ItemPickupRange.SetValue(item, this.itemPickupRangeOld[item]);
            this.itemPickupList.Remove(item);
            this.itemPickupRangeOld.Remove(item);
        }

        public void AddItemPro(ModuleItemProducer item)
        {
            this.itemProListGeneric.Add(item);
            this.allSegments["ItemProSpeed"].SaveObject(item);

            /*this.itemProList.Add(item);
            this.itemProSpeedOld.Add(item, new List<float>()
            {
                (float)field_ItemProSpeed1.GetValue(item),
                (float)field_ItemProSpeed2.GetValue(item)
            });*/

            this.Update(new string[] { "ItemProSpeed" });
        }

        public void RemoveItemPro(ModuleItemProducer item)
        {
            
            this.allSegments["ItemProSpeed"].CleanObject(item);
            this.itemProListGeneric.Remove(item);

            /*field_ItemProSpeed1.SetValue(item, this.itemProSpeedOld[item][0]);
            field_ItemProSpeed2.SetValue(item, this.itemProSpeedOld[item][1]);
            this.itemProList.Remove(item);
            this.itemProSpeedOld.Remove(item);*/
        }

        public void AddHover(ModuleHover hover)
        {
            this.hoverListGeneric.Add(hover);
            this.allSegments["HoverForce"].SaveObject(hover);
            this.allSegments["HoverRange"].SaveObject(hover);
            this.allSegments["HoverDamping"].SaveObject(hover);
            
            this.Update(new string[] { "HoverForce" , "HoverRange" , "HoverDamping" });
        }

        public void RemoveHover(ModuleHover hover)
        {
            this.allSegments["HoverForce"].CleanObject(hover);
            this.allSegments["HoverRange"].CleanObject(hover);
            this.allSegments["HoverDamping"].CleanObject(hover);
            this.hoverListGeneric.Remove(hover);
        }

        public void RefreshWheels(ModuleWheels wheels, ManWheels.TorqueParams torque, ManWheels.WheelParams wheelparams)
        {
            field_TorqueParams.SetValue(wheels, torque); // Apply new Torque to ModuleWheels
            field_WheelParams.SetValue(wheels, wheelparams); // Apply new WheelParams...

            List<ManWheels.Wheel> value_Wheels = (List<ManWheels.Wheel>)field_Wheels.GetValue(wheels);
            foreach (ManWheels.Wheel wheel in value_Wheels)
            {
                Array value_WheelState = (Array)field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);

                int value_WheelAttachedId = (int)field_AttachedId.GetValue(wheel); // Important, determines what AttachedWheelState is associated
                if (value_WheelAttachedId > -1) // value is -1 if it's unloaded, I think...
                {
                    object value_AttachedWheelState = value_WheelState.GetValue(value_WheelAttachedId); // AttachedWheelState is a PRIVATE STRUCT, `object` neccessary
                    FieldInfo field_p_TorqueParams = value_AttachedWheelState.GetType() // Get types of private struct...
                        .GetField("torqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo field_p_WheelParams = value_AttachedWheelState.GetType()
                        .GetField("wheelParams", BindingFlags.NonPublic | BindingFlags.Instance);

                    field_p_TorqueParams.SetValue(value_AttachedWheelState, torque); // Apply new Torque to ManWheels.Wheel
                    field_p_WheelParams.SetValue(value_AttachedWheelState, wheelparams); // Apply new WheelParams...

                    FieldInfo field_p_Inertia = value_AttachedWheelState.GetType()
                        .GetField("inertia", BindingFlags.NonPublic | BindingFlags.Instance);
                    
                    ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Note: Keep these incase inertia read causes issues
                    //float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
                    ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, (float)field_p_Inertia.GetValue(value_AttachedWheelState), value_Wheels.Count);

                    wheel.UpdateAttachData(moduleData); // Update it! Live! Do it!
                    // Also logs "only for use in Editor" error, annoying...

                    /*MethodInfo method_Init = value_AttachedWheelState.GetType()
                        .GetMethod("Init", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    MethodInfo method_RecalculateDotProducts = value_AttachedWheelState.GetType()
                        .GetMethod("RecalculateDotProducts", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    Console.WriteLine("ffw: invoking...");
                    //object[] parametersArray = new object[] { wheel, moduleData };

                    method_Init.Invoke(value_AttachedWheelState, new object[] { wheel, moduleData });
                    method_RecalculateDotProducts.Invoke(value_AttachedWheelState, null);
                    Console.WriteLine("ffw: invoked.");*/
                }
            }
        }

        public void RefreshWheels2(List<ModuleWheels> wheelsList)
        {
            foreach (ModuleWheels wheels in wheelsList)
            {
                /*field_TorqueParams.SetValue(wheels, torque); // Apply new Torque to ModuleWheels
                field_WheelParams.SetValue(wheels, wheelparams); // Apply new WheelParams...*/
                ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);

                List<ManWheels.Wheel> value_Wheels = (List<ManWheels.Wheel>)field_Wheels.GetValue(wheels);
                foreach (ManWheels.Wheel wheel in value_Wheels)
                {
                    Array value_WheelState = (Array)field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);

                    int value_WheelAttachedId = (int)field_AttachedId.GetValue(wheel); // Important, determines what AttachedWheelState is associated
                    if (value_WheelAttachedId > -1) // value is -1 if it's unloaded, I think...
                    {
                        object value_AttachedWheelState = value_WheelState.GetValue(value_WheelAttachedId); // AttachedWheelState is a PRIVATE STRUCT, `object` neccessary
                        FieldInfo field_p_TorqueParams = value_AttachedWheelState.GetType() // Get types of private struct...
                            .GetField("torqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
                        FieldInfo field_p_WheelParams = value_AttachedWheelState.GetType()
                            .GetField("wheelParams", BindingFlags.NonPublic | BindingFlags.Instance);

                        field_p_TorqueParams.SetValue(value_AttachedWheelState, torque); // Apply new Torque to ManWheels.Wheel
                        field_p_WheelParams.SetValue(value_AttachedWheelState, wheelparams); // Apply new WheelParams...

                        FieldInfo field_p_Inertia = value_AttachedWheelState.GetType()
                            .GetField("inertia", BindingFlags.NonPublic | BindingFlags.Instance);

                        ManWheels.WheelParams value_WheelParams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Note: Keep these incase inertia read causes issues
                                                                                                                             //float i = wheels.block.CurrentMass * 0.9f / (float)value_Wheels.Count * value_WheelParams.radius * value_WheelParams.radius;
                        ModuleWheels.AttachData moduleData = new ModuleWheels.AttachData(wheels, (float)field_p_Inertia.GetValue(value_AttachedWheelState), value_Wheels.Count);

                        wheel.UpdateAttachData(moduleData); // Update it! Live! Do it!
                                                            
                    }
                }
            }
        }
    }
}
