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
        public static readonly Dictionary<string, string[]> allEffects = new Dictionary<string, string[]>
        {
            { "WeaponCooldown" , new string[] { "m_WeaponModule.m_ShotCooldown", "m_ShotCooldown", "m_BurstCooldown" } },
            { "WeaponRotation" , new string[] { "m_WeaponModule.m_RotateSpeed" } },
            { "WeaponSpread" , new string[] { "m_FiringData.m_BulletSprayVariance" } },
            { "WeaponVelocity" , new string[] { "m_FiringData.m_MuzzleVelocity" } },

            { "DrillDps" , new string[] { "damagePerSecond" } },

            { "WheelsRpm" , new string[] { "m_TorqueParams.torqueCurveMaxRpm" } },
            { "WheelsBrake" , new string[] { "m_TorqueParams.passiveBrakeMaxTorque", "m_TorqueParams.basicFrictionTorque" } },
            { "WheelsTorque" , new string[] { "m_TorqueParams.torqueCurveMaxTorque" } },
            { "WheelsGrip" , new string[] { "m_WheelParams.tireProperties.props.gripFactorLong" } },
            { "WheelsSuspension" , new string[] { "m_WheelParams.suspensionSpring", "m_WheelParams.suspensionDamper" } },
            { "WheelsTurnAngle" , new string[] { "m_WheelParams.steerAngleMax" } },

            { "HoverForce" , new string[] { "jets.forceMax" } },
            { "HoverRange" , new string[] { "jets.forceRangeMax" } },
            { "HoverDamping" , new string[] { "jets.m_DampingScale" } },

            { "BoosterBurnRate" , new string[] { "jets.m_BurnRate" } },

            { "ChargerRange" , new string[] { "m_ChargingRadius" } },
            { "ChargerSpeed" , new string[] { "m_ArcFiringInterval" } },

            { "ItemPickupRange" , new string[] { "m_PickupRange" } },
            { "ItemProSpeed" , new string[] { "m_SecPerItemProduced", "m_MinDispenseInterval" } },
            { "ItemStoreCap" , new string[] { "m_Capacity" } },
            { "ItemHoldCap" , new string[] { "m_CapacityPerStack" } },
            { "ItemConAnchor" , new string[] { "m_NeedsToBeAnchored" } },
            { "HeartAnchor" , new string[] { "m_HasAnchor" } }
        };

        public static readonly Dictionary<string, string> segmentListAssociation = new Dictionary<string, string>
        {
            { "WeaponCooldown" , "weaponListGeneric" },
            { "WeaponRotation" , "weaponListGeneric" },
            { "WeaponSpread" , "weaponListGeneric" },
            { "WeaponVelocity" , "weaponListGeneric" },

            { "DrillDps" , "drillListGeneric" },

            { "WheelsRpm" , "wheelsListGeneric" },
            { "WheelsBrake" , "wheelsListGeneric" },
            { "WheelsTorque" , "wheelsListGeneric" },
            { "WheelsGrip" , "wheelsListGeneric" },
            { "WheelsSuspension" , "wheelsListGeneric" },
            { "WheelsTurnAngle" , "wheelsListGeneric" },

            { "HoverForce" , "hoverListGeneric" },
            { "HoverRange" , "hoverListGeneric" },
            { "HoverDamping" , "hoverListGeneric" },

            { "BoosterBurnRate" , "boosterListGeneric" },
            
            { "ChargerRange" , "chargerListGeneric" },
            { "ChargerSpeed" , "chargerListGeneric" },

            { "ItemPickupRange" , "itemPickupListGeneric" },
            { "ItemProSpeed" , "itemProListGeneric" },
            { "ItemStoreCap" , "itemStoreListGeneric" },
            { "ItemHoldCap" , "itemHoldListGeneric" },
            { "ItemConAnchor" , "itemConListGeneric" },
            { "HeartAnchor" , "heartListGeneric" }
        };
        public Dictionary<string, BuffSegment> allSegments = new Dictionary<string, BuffSegment>();

        public List<object> weaponListGeneric = new List<object>();
        public List<ModuleWeaponGun> weaponList = new List<ModuleWeaponGun>();
        public Dictionary<CannonBarrel, float> weaponSpeedMemory = new Dictionary<CannonBarrel, float>();
        public List<object> drillListGeneric = new List<object>();
        public List<object> wheelsListGeneric = new List<object>();
        public List<ModuleWheels> wheelsList = new List<ModuleWheels>();
        public List<object> hoverListGeneric = new List<object>();
        public List<object> boosterListGeneric = new List<object>();
        
        public List<object> chargerListGeneric = new List<object>();

        public List<object> itemPickupListGeneric = new List<object>();
        public List<object> itemProListGeneric = new List<object>();
        public List<object> itemStoreListGeneric = new List<object>();
        public List<object> itemHoldListGeneric = new List<object>();
        public List<object> itemConListGeneric = new List<object>();
        public List<object> heartListGeneric = new List<object>();

        /* PRIME PROPERTIES */
        public static List<BuffController> allControllers = new List<BuffController>();
        public Tank tank;

        public static BuffController MakeNewIfNone(Tank objTank)
        {
            /*Console.WriteLine("FFW! 4 " + BuffController.Clamp(4.0f, 0.0f, 1.0f));
            Console.WriteLine("FFW! 0.5 " + BuffController.Clamp(0.5f, 0.0f, 1.0f));
            Console.WriteLine("FFW! -2 " + BuffController.Clamp(-2.0f, 0.0f, 1.0f));
            Console.WriteLine("FFW! 0 " + BuffController.Clamp(0.0f, 0.0f, 1.0f));*/
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

        public static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        public static void AddObject(BuffController obj)
        {
            BuffController.allControllers.Add(obj);
        }

        public static void RemoveObject(BuffController obj) // Todo: Make "cleaning" function
        {
            BuffController.allControllers.Remove(obj);
        }

        public void Update(string[] effects)
        {
            foreach (string effect in effects)
            {
                if (this.allSegments.ContainsKey(effect))
                {
                    FieldInfo field_associatedArray = typeof(BuffController)
                        .GetField(BuffController.segmentListAssociation[effect], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    List<object> value_associatedArray = (List<object>)field_associatedArray.GetValue(this);
                    this.allSegments[effect].ManipulateObj(value_associatedArray, "UPDATE");
                }
            }
            if (effects.Contains("WeaponCooldown"))
            {
                this.ManipulateBarrels(this.weaponList, "UPDATE");
            }
            if (effects.Contains("WheelsRpm") ||
                effects.Contains("WheelsBrake") ||
                effects.Contains("WheelsTorque") ||
                effects.Contains("WheelsGrip") ||
                effects.Contains("WheelsSuspension") ||
                effects.Contains("WheelsTurnAngle") ||
                effects.Contains("All"))
            {
                this.RefreshWheels(this.wheelsList);
            }
        }

        public void AddBuff(ModuleBuff buff)
        {
            foreach (string effect in buff.AllEffects)
            {
                if (this.allSegments.ContainsKey(effect))
                {
                    this.allSegments[effect].AddBuff(buff);
                }
            }
            this.Update(buff.m_BuffType);
        }

        public void RemoveBuff(ModuleBuff buff)
        {
            foreach (string effect in buff.AllEffects)
            {
                if (this.allSegments.ContainsKey(effect))
                {
                    this.allSegments[effect].RemoveBuff(buff);
                }
            }
            this.Update(buff.m_BuffType);
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            this.weaponListGeneric.Add(weapon);
            this.weaponList.Add(weapon);

            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponRotation"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponSpread"].ManipulateObj(new List<object> { weapon }, "SAVE");
            this.allSegments["WeaponVelocity"].ManipulateObj(new List<object> { weapon }, "SAVE");

            this.ManipulateBarrels(new List<ModuleWeaponGun> { weapon }, "SAVE");
            
            this.ManipulateBarrels(new List<ModuleWeaponGun> { weapon }, "UPDATE");
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            this.allSegments["WeaponCooldown"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponRotation"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponSpread"].ManipulateObj(new List<object> { weapon }, "CLEAN");
            this.allSegments["WeaponVelocity"].ManipulateObj(new List<object> { weapon }, "CLEAN");

            this.ManipulateBarrels(new List<ModuleWeaponGun> { weapon }, "CLEAN");

            this.weaponListGeneric.Remove(weapon);
            this.weaponList.Remove(weapon);
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
            this.allSegments["WheelsTurnAngle"].ManipulateObj(new List<object> { wheels }, "SAVE");
            
            this.RefreshWheels( new List<ModuleWheels> { wheels });
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            this.allSegments["WheelsRpm"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsBrake"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsTorque"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsGrip"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsSuspension"].ManipulateObj(new List<object> { wheels }, "CLEAN");
            this.allSegments["WheelsTurnAngle"].ManipulateObj(new List<object> { wheels }, "CLEAN");

            this.RefreshWheels(new List<ModuleWheels> { wheels });

            this.wheelsListGeneric.Remove(wheels);
            this.wheelsList.Remove(wheels);
        }

        public void AddBooster(ModuleBooster booster)
        {
            this.boosterListGeneric.Add(booster);

            this.allSegments["BoosterBurnRate"].ManipulateObj(new List<object> { booster }, "SAVE");
        }

        public void RemoveBooster(ModuleBooster booster)
        {
            this.allSegments["BoosterBurnRate"].ManipulateObj(new List<object> { booster }, "CLEAN");

            this.boosterListGeneric.Remove(booster);
        }

        public void AddDrill(ModuleDrill drill)
        {
            this.drillListGeneric.Add(drill);

            this.allSegments["DrillDps"].ManipulateObj(new List<object> { drill }, "SAVE");
        }

        public void RemoveDrill(ModuleDrill drill)
        {
            this.allSegments["DrillDps"].ManipulateObj(new List<object> { drill }, "CLEAN");

            this.drillListGeneric.Remove(drill);
        }

        public void AddItemPickup(ModuleItemPickup item)
        {
            this.itemPickupListGeneric.Add(item);

            this.allSegments["ItemPickupRange"].ManipulateObj(new List<object> { item }, "SAVE");
        }

        public void RemoveItemPickup(ModuleItemPickup item)
        {
            this.allSegments["ItemPickupRange"].ManipulateObj(new List<object> { item }, "CLEAN");

            this.itemPickupListGeneric.Remove(item);
        }

        public void AddItemPro(ModuleItemProducer item)
        {
            this.itemProListGeneric.Add(item);

            this.allSegments["ItemProSpeed"].ManipulateObj(new List<object> { item }, "SAVE");
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
        }

        public void RemoveHover(ModuleHover hover)
        {
            this.allSegments["HoverForce"].ManipulateObj(new List<object> { hover }, "CLEAN");
            this.allSegments["HoverRange"].ManipulateObj(new List<object> { hover }, "CLEAN");
            this.allSegments["HoverDamping"].ManipulateObj(new List<object> { hover }, "CLEAN");

            this.hoverListGeneric.Remove(hover);
        }

        public void AddItemStore(ModuleItemStore item)
        {
            this.itemStoreListGeneric.Add(item);

            this.allSegments["ItemStoreCap"].ManipulateObj(new List<object> { item }, "SAVE");
        }

        public void RemoveItemStore(ModuleItemStore item)
        {
            this.allSegments["ItemStoreCap"].ManipulateObj(new List<object> { item }, "CLEAN");

            this.itemStoreListGeneric.Remove(item);
        }

        public void AddItemHolder(ModuleItemHolder item)
        {
            this.itemHoldListGeneric.Add(item);

            this.allSegments["ItemHoldCap"].ManipulateObj(new List<object> { item }, "SAVE");
        }

        public void RemoveItemHolder(ModuleItemHolder item)
        {
            this.allSegments["ItemHoldCap"].ManipulateObj(new List<object> { item }, "CLEAN");

            this.itemHoldListGeneric.Remove(item);
        }

        public void AddCharger(ModuleRemoteCharger charger)
        {
            this.chargerListGeneric.Add(charger);

            this.allSegments["ChargerRange"].ManipulateObj(new List<object> { charger }, "SAVE");
            this.allSegments["ChargerSpeed"].ManipulateObj(new List<object> { charger }, "SAVE");
        }

        public void RemoveCharger(ModuleRemoteCharger charger)
        {
            this.allSegments["ChargerRange"].ManipulateObj(new List<object> { charger }, "CLEAN");
            this.allSegments["ChargerSpeed"].ManipulateObj(new List<object> { charger }, "CLEAN");

            this.chargerListGeneric.Remove(charger);
        }

        public void AddItemCon(ModuleItemConsume item)
        {
            this.itemConListGeneric.Add(item);

            this.allSegments["ItemConAnchor"].ManipulateObj(new List<object> { item }, "SAVE");
        }

        public void RemoveItemCon(ModuleItemConsume item)
        {
            this.allSegments["ItemConAnchor"].ManipulateObj(new List<object> { item }, "CLEAN");

            this.itemConListGeneric.Remove(item);
        }
        
        public void AddHeart(ModuleHeart heart)
        {
            this.heartListGeneric.Add(heart);

            this.allSegments["HeartAnchor"].ManipulateObj(new List<object> { heart }, "SAVE");
        }

        public void RemoveHeart(ModuleHeart heart)
        {
            this.allSegments["HeartAnchor"].ManipulateObj(new List<object> { heart }, "CLEAN");

            this.heartListGeneric.Remove(heart);
        }

        public void RefreshWheels(List<ModuleWheels> wheelsList)
        {
            FieldInfo field_TorqueParams = typeof(ModuleWheels) // Maybe I should find a way to compress these?
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
                ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels); // Read active Torque... 
                ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels); // Read active WheelParams... 

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

        public void ManipulateBarrels(List<ModuleWeaponGun> weaponList, string request)
        {
            FieldInfo field_NumCannonBarrels = typeof(ModuleWeaponGun) // Holy mess!
                .GetField("m_NumCannonBarrels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_CannonBarrels = typeof(ModuleWeaponGun)
                .GetField("m_CannonBarrels", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_BarrelTransform = typeof(ModuleWeaponGun)
                .GetField("m_BarrelTransform", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_FiringData = typeof(ModuleWeaponGun)
                .GetField("m_FiringData", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_WeaponModule = typeof(ModuleWeaponGun)
                .GetField("m_WeaponModule", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_ShotCooldown = typeof(ModuleWeaponGun)
                .GetField("m_ShotCooldown", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_transform = typeof(Transform)
                .GetField("transform", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo field_AnimState = typeof(CannonBarrel)
                .GetField("animState", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            /*MethodInfo method_Setup = typeof(CannonBarrel)
                .GetMethod("Setup", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo method_CapRecoilDuration = typeof(CannonBarrel)
                .GetMethod("CapRecoilDuration", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);*/

            foreach (ModuleWeaponGun weapon in weaponList)
            {
                int value_NumCannonBarrels = (int)field_NumCannonBarrels.GetValue(weapon);
                if (value_NumCannonBarrels != 0)
                {
                    Array value_CannonBarrels = (Array)field_CannonBarrels.GetValue(weapon);
                    for (int i = 0; i < value_CannonBarrels.Length; i++)
                    {
                        /*Transform value_BarrelTransform = (Transform)field_BarrelTransform.GetValue(weapon);
                        if (value_BarrelTransform == null) // Will this ever check true?
                        {
                            field_BarrelTransform.SetValue(weapon, field_transform.GetValue(value_CannonBarrels.GetValue(i)));
                        }*/
                        CannonBarrel thisBarrel = (CannonBarrel)value_CannonBarrels.GetValue(i);
                        FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                        ModuleWeapon value_WeaponModule = (ModuleWeapon)field_WeaponModule.GetValue(weapon);
                        float value_ShotCooldown = (float)field_ShotCooldown.GetValue(weapon);
                        //method_Setup.Invoke(value_CannonBarrels.GetValue(i), new object[] { value_FiringData, value_WeaponModule });
                        //method_CapRecoilDuration.Invoke(value_CannonBarrels.GetValue(i), new object[] { value_ShotCooldown });
                        object value_AnimState = field_AnimState.GetValue(thisBarrel);
                        if (value_AnimState != null)
                        {
                            PropertyInfo prop_GetSpeed = value_AnimState.GetType()
                                .GetProperty("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                            PropertyInfo prop_SetSpeed = value_AnimState.GetType()
                                .GetProperty("speed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
                            PropertyInfo prop_GetLength = value_AnimState.GetType()
                                .GetProperty("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
                            PropertyInfo prop_SetLength = value_AnimState.GetType()
                                .GetProperty("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty);
                            if (request == "SAVE")
                            {
                                this.weaponSpeedMemory.Add(thisBarrel, (float)prop_GetSpeed.GetValue(value_AnimState));
                            }
                            else if (request == "UPDATE")
                            {
                                prop_SetSpeed.SetValue(value_AnimState, this.weaponSpeedMemory[thisBarrel] / this.allSegments["WeaponCooldown"].GetAverages());
                            }
                            else if (request == "CLEAN")
                            {
                                prop_SetSpeed.SetValue(value_AnimState, this.weaponSpeedMemory[thisBarrel]);
                                this.weaponSpeedMemory.Remove(thisBarrel);
                            }
                            //float value_length = (float)prop_GetLength.GetValue(value_AnimState);
                            //Console.WriteLine("FFW! Pre. Length = " + (float)prop_GetLength.GetValue(value_AnimState) + " . Speed = " + (float)prop_GetSpeed.GetValue(value_AnimState));
                            /*float modifier = this.allSegments["WeaponCooldown"].GetAveragesByKey(weapon, 2);
                            if (modifier == 0.0f)
                            {
                                modifier = this.allSegments["WeaponCooldown"].GetAveragesByKey(weapon, 1);
                            }
                            if (value_length > modifier)
                            {
                                //prop_SetSpeed.SetValue(value_AnimState, value_length / modifier);
                            }*/
                            //Console.WriteLine("FFW! Post. Length = " + (float)prop_GetLength.GetValue(value_AnimState) + " . Speed = " + (float)prop_GetSpeed.GetValue(value_AnimState));
                            /*if (value_length > this.allSegments["WeaponCooldown"].GetAveragesByKey(weapon, 2))
                            {
                                prop_SetSpeed.SetValue(value_AnimState, value_length / this.allSegments["WeaponCooldown"].GetAveragesByKey(weapon, 2));
                            }*/
                        }

                    }
                }
            }
        }
    }
}
