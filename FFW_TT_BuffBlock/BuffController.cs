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
            { "WheelsBrake" , new string[] { "m_TorqueParams.passiveBrakeMaxTorque", "m_TorqueParams.basicFrictionTorque" } },
            { "WheelsTorque" , new string[] { "m_TorqueParams.torqueCurveMaxTorque" } },
            { "WheelsGrip" , new string[] { "m_WheelParams.tireProperties.props.gripFactorLong" } },
            { "WheelsSuspension" , new string[] { "m_WheelParams.suspensionSpring", "m_WheelParams.suspensionDamper" } },

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
            // Use "All" to update all, use m_BuffType to update specifics

            this.allSegments["WeaponCooldown"].ManipulateObj(this.weaponListGeneric, "UPDATE");
            this.allSegments["WeaponRotation"].ManipulateObj(this.weaponListGeneric, "UPDATE");
            this.allSegments["WeaponSpread"].ManipulateObj(this.weaponListGeneric, "UPDATE");
            this.allSegments["WeaponVelocity"].ManipulateObj(this.weaponListGeneric, "UPDATE");
            this.allSegments["HoverForce"].ManipulateObj(this.hoverListGeneric, "UPDATE");
            this.allSegments["HoverRange"].ManipulateObj(this.hoverListGeneric, "UPDATE");
            this.allSegments["HoverDamping"].ManipulateObj(this.hoverListGeneric, "UPDATE");
            this.allSegments["BoosterBurnRate"].ManipulateObj(this.boosterListGeneric, "UPDATE");
            this.allSegments["ItemProSpeed"].ManipulateObj(this.itemProListGeneric, "UPDATE");

            this.allSegments["WheelsRpm"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.allSegments["WheelsBrake"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.allSegments["WheelsTorque"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.allSegments["WheelsGrip"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.allSegments["WheelsSuspension"].ManipulateObj(this.wheelsListGeneric, "UPDATE");
            this.RefreshWheels(this.wheelsList);

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
            if (effects.Contains("WheelsRpm"))
            {
                this.allSegments["WheelsRpm"].AddBuff(buff);
            }
            if (effects.Contains("WheelsBrake"))
            {
                this.allSegments["WheelsBrake"].AddBuff(buff);
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.allSegments["WheelsTorque"].AddBuff(buff);
            }
            if (effects.Contains("WheelsGrip"))
            {
                this.allSegments["WheelsGrip"].AddBuff(buff);
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.allSegments["WheelsSuspension"].AddBuff(buff);
            }
            if (effects.Contains("BoosterBurnRate"))
            {
                this.allSegments["BoosterBurnRate"].AddBuff(buff);
                
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
            if (effects.Contains("WheelsRpm"))
            {
                this.allSegments["WheelsRpm"].RemoveBuff(buff);
            }
            if (effects.Contains("WheelsBrake"))
            {
                this.allSegments["WheelsBrake"].RemoveBuff(buff);
            }
            if (effects.Contains("WheelsTorque"))
            {
                this.allSegments["WheelsTorque"].RemoveBuff(buff);
            }
            if (effects.Contains("WheelsGrip"))
            {
                this.allSegments["WheelsGrip"].RemoveBuff(buff);
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.allSegments["WheelsSuspension"].RemoveBuff(buff);
            }
            if (effects.Contains("BoosterBurnRate"))
            {
                this.allSegments["BoosterBurnRate"].RemoveBuff(buff);
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
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            this.weaponListGeneric.Add(weapon);

            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponRotation"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponSpread"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponVelocity"].ManipulateObj(new List<object> { weapon }, "SAVE");

            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "UPDATE");
            this.allSegments["WeaponRotation"].ManipulateObj(new List<object> { weapon }, "UPDATE");
            this.allSegments["WeaponSpread"].ManipulateObj(new List<object> { weapon }, "UPDATE");
            this.allSegments["WeaponVelocity"].ManipulateObj(new List<object> { weapon }, "UPDATE");
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponRotation"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponSpread"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponVelocity"].ManipulateObj(new List<object> { weapon }, "CLEAN");

            this.weaponListGeneric.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
            this.wheelsListGeneric.Add(wheels);
            this.wheelsList.Add(wheels);

            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "SAVE");
            this.allSegments["WheelsBrake"].ManipulateObj(new List<object> { wheels }, "SAVE");
            this.allSegments["WheelsTorque"].ManipulateObj(new List<object> { wheels }, "SAVE");
            this.allSegments["WheelsGrip"].ManipulateObj(new List<object> { wheels }, "SAVE");
            this.allSegments["WheelsSuspension"].ManipulateObj(new List<object> { wheels }, "SAVE");

            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "UPDATE");
            this.allSegments["WheelsBrake"].ManipulateObj(new List<object> { wheels }, "UPDATE");
            this.allSegments["WheelsTorque"].ManipulateObj(new List<object> { wheels }, "UPDATE");
            this.allSegments["WheelsGrip"].ManipulateObj(new List<object> { wheels }, "UPDATE");
            this.allSegments["WheelsSuspension"].ManipulateObj(new List<object> { wheels }, "UPDATE");

            this.RefreshWheels( new List<ModuleWheels> { wheels });
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsBrake"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsTorque"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsGrip"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsSuspension"].ManipulateObj(new List<object> { wheels }, "CLEAN");

            this.RefreshWheels(new List<ModuleWheels> { wheels });

            this.wheelsListGeneric.Remove(wheels);
            this.wheelsList.Remove(wheels);
        }

        public void AddBooster(ModuleBooster booster)
        {
            this.boosterListGeneric.Add(booster);

            this.allSegments["BoosterBurnRate"].ManipulateObj(new List<object> { booster }, "SAVE");
            this.allSegments["BoosterBurnRate"].ManipulateObj(new List<object> { booster }, "UPDATE");
        }

        public void RemoveBooster(ModuleBooster booster)
        {
            this.allSegments["BoosterBurnRate"].ManipulateObj(new List<object> { booster }, "CLEAN");

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

            this.allSegments["ItemProSpeed"].ManipulateObj(new List<object> { item }, "SAVE");
            this.allSegments["ItemProSpeed"].ManipulateObj(new List<object> { item }, "UPDATE");
        }

        public void RemoveItemPro(ModuleItemProducer item)
        {

            this.allSegments["ItemProSpeed"].ManipulateObj(new List<object> { item }, "CLEAN");

            this.itemProListGeneric.Remove(item);
        }

        public void AddHover(ModuleHover hover)
        {
            this.hoverListGeneric.Add(hover);

            this.allSegments["HoverForce"].ManipulateObj(new List<object> { hover }, "SAVE");
            this.allSegments["HoverRange"].ManipulateObj(new List<object> { hover }, "SAVE");
            this.allSegments["HoverDamping"].ManipulateObj(new List<object> { hover }, "SAVE");

            this.allSegments["HoverForce"].ManipulateObj(new List<object> { hover }, "UPDATE");
            this.allSegments["HoverRange"].ManipulateObj(new List<object> { hover }, "UPDATE");
            this.allSegments["HoverDamping"].ManipulateObj(new List<object> { hover }, "UPDATE");
        }

        public void RemoveHover(ModuleHover hover)
        {
            this.allSegments["HoverForce"].ManipulateObj(new List<object> { hover }, "CLEAN");
            this.allSegments["HoverRange"].ManipulateObj(new List<object> { hover }, "CLEAN");
            this.allSegments["HoverDamping"].ManipulateObj(new List<object> { hover }, "CLEAN");

            this.hoverListGeneric.Remove(hover);
        }
        
        public void RefreshWheels(List<ModuleWheels> wheelsList)
        {
            FieldInfo field_TorqueParams = typeof(ModuleWheels)
                .GetField("m_TorqueParams", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_Wheels = typeof(ModuleWheels)
                .GetField("m_Wheels", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WheelParams = typeof(ModuleWheels)
                .GetField("m_WheelParams", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_AttachedId = typeof(ManWheels.Wheel)
                .GetField("attachedID", BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WheelState = typeof(ManWheels)
                .GetField("m_WheelState", BindingFlags.NonPublic | BindingFlags.Instance);
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
