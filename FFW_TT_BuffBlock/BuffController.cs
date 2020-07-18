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
            { "WeaponVelocity" , new string[] { "m_FiringData.m_MuzzleVelocity" } }
        };
        public Dictionary<string, BuffSegment> allSegments = new Dictionary<string, BuffSegment>();
        public List<object> weaponListGeneric = new List<object>();

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
        public List<ModuleHeart> heartList = new List<ModuleHeart>();
        public List<ModuleItemPickup> itemPickupList = new List<ModuleItemPickup>();
        public List<ModuleItemProducer> itemProList = new List<ModuleItemProducer>();
        public List<ModuleHover> hoverList = new List<ModuleHover>();

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

        /* HOVER : FORCE */
        public Dictionary<ModuleBuff, int> hoverForceBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleHover, float> hoverForceOld = new Dictionary<ModuleHover, float>();
        public static FieldInfo field_HoverJets = typeof(ModuleHover)
            .GetField("jets", BindingFlags.NonPublic | BindingFlags.Instance);
        public static FieldInfo field_ForceMax = typeof(HoverJet)
            .GetField("forceMax", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* HOVER : RANGE */
        public Dictionary<ModuleBuff, int> hoverRangeBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleHover, float> hoverRangeOld = new Dictionary<ModuleHover, float>();
        public static FieldInfo field_ForceRangeMax = typeof(HoverJet)
            .GetField("forceRangeMax", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        /* HOVER : DAMPING */
        public Dictionary<ModuleBuff, int> hoverDampingBuffBlocks = new Dictionary<ModuleBuff, int>();
        public Dictionary<ModuleHover, float> hoverDampingOld = new Dictionary<ModuleHover, float>();
        public static FieldInfo field_Damping = typeof(HoverJet)
            .GetField("m_DampingScale", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

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
            this.allSegments["WeaponCooldown"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponRotation"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponSpread"].UpdateObject(this.weaponListGeneric);
            this.allSegments["WeaponVelocity"].UpdateObject(this.weaponListGeneric);
            // Use "All" to update all, use m_BuffType to update specifics
            /*if (type.Contains("WeaponCooldown") || type.Contains("All"))
            {
                this.allSegments["WeaponCooldown"].UpdateObject(this.weaponListGeneric);
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0] * this.GetBuffAverage("weaponCooldownBuffBlocks") + this.GetBuffAddAverage("weaponCooldownBuffBlocks"));
                    field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1] * this.GetBuffAverage("weaponCooldownBuffBlocks") + this.GetBuffAddAverage("weaponCooldownBuffBlocks"));
                    ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                    field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0] * this.GetBuffAverage("weaponCooldownBuffBlocks") + this.GetBuffAddAverage("weaponCooldownBuffBlocks"));
                }
            }
            if (type.Contains("WeaponRotation") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                    field_Rotation.SetValue(value_ModuleWeapon, this.weaponRotationOld[weapon] * this.GetBuffAverage("weaponRotationBuffBlocks") + this.GetBuffAddAverage("weaponRotationBuffBlocks"));
                }
            }
            if (type.Contains("WeaponSpread") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                    field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon] * this.GetBuffAverage("weaponSpreadBuffBlocks") + this.GetBuffAddAverage("weaponSpreadBuffBlocks"));
                }
            }
            if (type.Contains("WeaponVelocity") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                    field_Velocity.SetValue(value_FiringData, this.weaponVelocityOld[weapon] * this.GetBuffAverage("weaponVelocityBuffBlocks") + this.GetBuffAddAverage("weaponVelocityBuffBlocks"));
                }
            }*/
            /*if (type.Contains("WeaponDamage") || type.Contains("All"))
            {
                foreach (ModuleWeaponGun weapon in this.weaponList)
                {
                    FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
                    if (value_FiringData.m_BulletPrefab.GetType() == typeof(Projectile))
                    {
                        Projectile bullet = (Projectile)value_FiringData.m_BulletPrefab;
                        //this.weaponDamageOld[weapon].Add((float)field_Damage.GetValue(bullet));
                        Console.WriteLine("ffw: in...");
                        Console.WriteLine(this.GetBuffAverage("weaponDamageBuffBlocks"));
                        Console.WriteLine("ffw: from...");
                        Console.WriteLine(field_Damage.GetValue(bullet));
                        field_Damage.SetValue(bullet, (int)Math.Ceiling(this.weaponDamageOld[weapon][0] * this.GetBuffAverage("weaponDamageBuffBlocks") + this.GetBuffAddAverage("weaponDamageBuffBlocks")));
                        Console.WriteLine("ffw: to...");
                        Console.WriteLine(field_Damage.GetValue(bullet));
                        if (field_Explosion.GetValue(bullet) != null)
                        {
                            Transform transform = (Transform)field_Explosion.GetValue(bullet);
                            Explosion explosion = transform.GetComponent<Explosion>();
                            if (explosion != null)
                            {
                                //this.weaponDamageOld[weapon].Add((float)field_ExplDamage.GetValue(explosion));
                                field_ExplDamage.SetValue(explosion, this.weaponDamageOld[weapon][1] * this.GetBuffAverage("weaponDamageBuffBlocks") + this.GetBuffAddAverage("weaponDamageBuffBlocks"));
                                //field_ExplRadius.SetValue(explosion, 99.0f);
                            }
                        }
                    }

                    ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
                    Array value_CannonBarrel = (Array)field_CannonBarrels.GetValue(weapon);
                    if (value_CannonBarrel.Length != 0)
                    {
                        foreach (CannonBarrel cannonBarrel in value_CannonBarrel)
                        {
                            //cannonBarrel.Setup(value_FiringData, value_ModuleWeapon);
                            field_CannonBarrelFiringData.SetValue(cannonBarrel, value_FiringData);
                            Console.WriteLine("ffw: applied to barrel");
                            //cannonBarrel.CapRecoilDuration(this.m_ShotCooldown);
                        }
                    }
                }
            }*/
            /*if (type.Contains("WheelsRpm") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    torque.torqueCurveMaxRpm = this.wheelsRpmOld[wheels] * this.GetBuffAverage("wheelsRpmBuffBlocks") + this.GetBuffAddAverage("wheelsRpmBuffBlocks");
                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }
            if (type.Contains("WheelsBrake") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    torque.passiveBrakeMaxTorque = this.wheelsBrakeOld[wheels][0] * this.GetBuffAverage("wheelsBrakeBuffBlocks") + this.GetBuffAddAverage("wheelsBrakeBuffBlocks");
                    torque.basicFrictionTorque = this.wheelsBrakeOld[wheels][1] * this.GetBuffAverage("wheelsBrakeBuffBlocks") + this.GetBuffAddAverage("wheelsBrakeBuffBlocks");
                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }
            if (type.Contains("WheelsTorque") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    torque.torqueCurveMaxTorque = this.wheelsTorqueOld[wheels] * this.GetBuffAverage("wheelsTorqueBuffBlocks") + this.GetBuffAddAverage("wheelsTorqueBuffBlocks");
                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }
            if (type.Contains("WheelsGrip") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    wheelparams.tireProperties.props.gripFactorLong = this.wheelsGripOld[wheels][0] * this.GetBuffAverage("wheelsGripBuffBlocks") + this.GetBuffAddAverage("wheelsGripBuffBlocks");
                    //wheelparams.tireProperties.props.gripFactorLat = this.wheelsGripOld[wheels][1] * this.GetBuffAverage("wheelsGripBuffBlocks") + this.GetBuffAddAverage("wheelsGripBuffBlocks");
                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }
            if (type.Contains("WheelsSuspension") || type.Contains("All"))
            {
                foreach (ModuleWheels wheels in this.wheelsList)
                {
                    ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
                    ManWheels.WheelParams wheelparams = (ManWheels.WheelParams)field_WheelParams.GetValue(wheels);
                    wheelparams.suspensionSpring = this.wheelsSuspensionOld[wheels][0] * this.GetBuffAverage("wheelsSuspensionBuffBlocks") + this.GetBuffAddAverage("wheelsSuspensionBuffBlocks");
                    wheelparams.suspensionDamper = this.wheelsSuspensionOld[wheels][1] * this.GetBuffAverage("wheelsSuspensionBuffBlocks") + this.GetBuffAddAverage("wheelsSuspensionBuffBlocks");
                    this.RefreshWheels(wheels, torque, wheelparams);
                }
            }*/
            if (type.Contains("WheelsRpm") ||
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
            }
            if (type.Contains("BoosterBurnRate") || type.Contains("All"))
            {
                foreach (ModuleBooster booster in this.boosterList)
                {
                    List<BoosterJet> value_Jets = (List<BoosterJet>)field_Jets.GetValue(booster);
                    foreach (BoosterJet jet in value_Jets)
                    {
                        field_BurnRate.SetValue(jet, this.boosterBurnRateOld[booster] * this.GetBuffAverage("boosterBurnRateBuffBlocks") + this.GetBuffAddAverage("boosterBurnRateBuffBlocks"));
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
            if (type.Contains("ItemProSpeed") || type.Contains("All"))
            {
                foreach (ModuleItemProducer item in this.itemProList)
                {
                    field_ItemProSpeed1.SetValue(item, this.itemProSpeedOld[item][0] * this.GetBuffAverage("itemProSpeedBuffBlocks") + this.GetBuffAddAverage("itemProSpeedBuffBlocks"));
                    field_ItemProSpeed2.SetValue(item, this.itemProSpeedOld[item][1] * this.GetBuffAverage("itemProSpeedBuffBlocks") + this.GetBuffAddAverage("itemProSpeedBuffBlocks"));
                }
            }
            if (type.Contains("HoverForce") ||
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
            }
        }

        public void AddBuff(ModuleBuff buff)
        {
            List<string> effects = buff.AllEffects;
            if (effects.Contains("WeaponCooldown"))
            {
                this.allSegments["WeaponCooldown"].AddBuff(buff);
                this.weaponCooldownBuffBlocks.Add(buff, buff.GetEffect("WeaponCooldown"));
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.allSegments["WeaponRotation"].AddBuff(buff);
                this.weaponRotationBuffBlocks.Add(buff, buff.GetEffect("WeaponRotation"));
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.allSegments["WeaponSpread"].AddBuff(buff);
                this.weaponSpreadBuffBlocks.Add(buff, buff.GetEffect("WeaponSpread"));
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.allSegments["WeaponVelocity"].AddBuff(buff);
                this.weaponVelocityBuffBlocks.Add(buff, buff.GetEffect("WeaponVelocity"));
            }
            /*if (effects.Contains("WeaponDamage"))
            {
                this.weaponDamageBuffBlocks.Add(buff, buff.GetEffect("WeaponDamage"));
            }*/
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
            if (effects.Contains("WheelsGrip"))
            {
                this.wheelsGripBuffBlocks.Add(buff, buff.GetEffect("WheelsGrip"));
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.wheelsSuspensionBuffBlocks.Add(buff, buff.GetEffect("WheelsSuspension"));
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
                this.itemProSpeedBuffBlocks.Add(buff, buff.GetEffect("ItemProSpeed"));
            }
            if (effects.Contains("HoverForce"))
            {
                this.hoverForceBuffBlocks.Add(buff, buff.GetEffect("HoverForce"));
            }
            if (effects.Contains("HoverRange"))
            {
                this.hoverRangeBuffBlocks.Add(buff, buff.GetEffect("HoverRange"));
            }
            if (effects.Contains("HoverDamping"))
            {
                this.hoverDampingBuffBlocks.Add(buff, buff.GetEffect("HoverDamping"));
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
                this.weaponCooldownBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponRotation"))
            {
                this.allSegments["WeaponRotation"].RemoveBuff(buff);
                this.weaponRotationBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponSpread"))
            {
                this.allSegments["WeaponSpread"].RemoveBuff(buff);
                this.weaponSpreadBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WeaponVelocity"))
            {
                this.allSegments["WeaponVelocity"].RemoveBuff(buff);
                this.weaponVelocityBuffBlocks.Remove(buff);
            }
            /*if (effects.Contains("WeaponDamage"))
            {
                this.weaponDamageBuffBlocks.Remove(buff);
            }*/
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
            if (effects.Contains("WheelsGrip"))
            {
                this.wheelsGripBuffBlocks.Remove(buff);
            }
            if (effects.Contains("WheelsSuspension"))
            {
                this.wheelsSuspensionBuffBlocks.Remove(buff);
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
                this.itemProSpeedBuffBlocks.Remove(buff);
            }
            if (effects.Contains("HoverForce"))
            {
                this.hoverForceBuffBlocks.Remove(buff);
            }
            if (effects.Contains("HoverRange"))
            {
                this.hoverRangeBuffBlocks.Remove(buff);
            }
            if (effects.Contains("HoverDamping"))
            {
                this.hoverDampingBuffBlocks.Remove(buff);
            }
            this.Update(buff.m_BuffType);
            //this.Update(new string[] { buff.m_BuffType });
        }

        public void AddWeapon(ModuleWeaponGun weapon)
        {
            this.weaponListGeneric.Add(weapon);
            this.allSegments["WeaponCooldown"].SaveObject(weapon);
            this.allSegments["WeaponRotation"].SaveObject(weapon);
            this.allSegments["WeaponSpread"].SaveObject(weapon);
            this.allSegments["WeaponVelocity"].SaveObject(weapon);

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
            /*this.weaponDamageOld.Add(weapon, new List<float>()
            {
                0.0f,
                0.0f
            });
            
            if (value_FiringData.m_BulletPrefab.GetType() == typeof(Projectile))
            {
                Console.WriteLine("ffw 1");
                Projectile bullet = (Projectile)value_FiringData.m_BulletPrefab;
                Console.WriteLine("ffw 2");
                this.weaponDamageOld[weapon][0] = Convert.ToSingle((int)field_Damage.GetValue(bullet));
                Console.WriteLine("ffw 3");
                if (field_Explosion.GetValue(bullet) != null)
                {
                    Console.WriteLine("ffw 4");
                    Transform transform = (Transform)field_Explosion.GetValue(bullet);
                    Console.WriteLine("ffw 5");
                    Explosion explosion = transform.GetComponent<Explosion>();
                    Console.WriteLine("ffw 6");
                    if (explosion != null)
                    {
                        Console.WriteLine("ffw 7");
                        this.weaponDamageOld[weapon][1] = (float)field_ExplDamage.GetValue(explosion);
                        //field_ExplRadius.SetValue(explosion, 99.0f);
                    }
                }
            }*/

            this.Update(new string[] { "WeaponCooldown", "WeaponRotation", "WeaponSpread", "WeaponVelocity" });//, "WeaponDamage" });
        }

        public void RemoveWeapon(ModuleWeaponGun weapon)
        {
            this.allSegments["WeaponCooldown"].CleanObject(weapon);
            this.allSegments["WeaponRotation"].CleanObject(weapon);
            this.allSegments["WeaponSpread"].CleanObject(weapon);
            this.allSegments["WeaponVelocity"].CleanObject(weapon);
            this.weaponListGeneric.Remove(weapon);

            field_ShotCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][0]);
            field_BurstCooldown.SetValue(weapon, this.weaponCooldownOld[weapon][1]);
            ModuleWeapon value_ModuleWeapon = (ModuleWeapon)field_ModuleWeapon.GetValue(weapon);
            field_MW_ShotCooldown.SetValue(value_ModuleWeapon, this.weaponCooldownOld[weapon][0]);
            FireData value_FiringData = (FireData)field_FiringData.GetValue(weapon);
            field_Spread.SetValue(value_FiringData, this.weaponSpreadOld[weapon]);
            field_Velocity.SetValue(value_FiringData, this.weaponVelocityOld[weapon]);
            
            /*if (value_FiringData.m_BulletPrefab.GetType() == typeof(Projectile))
            {
                Projectile bullet = (Projectile)value_FiringData.m_BulletPrefab;
                field_Damage.SetValue(bullet, (int)Math.Ceiling(this.weaponDamageOld[weapon][0]));
                if (field_Explosion.GetValue(bullet) != null)
                {
                    Transform transform = (Transform)field_Explosion.GetValue(bullet);
                    Explosion explosion = transform.GetComponent<Explosion>();
                    if (explosion != null)
                    {
                        field_ExplDamage.SetValue(explosion, this.weaponDamageOld[weapon][1]);
                    }
                }
            }
            
            Array value_CannonBarrel = (Array)field_CannonBarrels.GetValue(weapon);
            if (value_CannonBarrel.Length != 0)
            {
                foreach (CannonBarrel cannonBarrel in value_CannonBarrel)
                {
                    //cannonBarrel.Setup(value_FiringData, value_ModuleWeapon);
                    field_CannonBarrelFiringData.SetValue(cannonBarrel, value_FiringData);
                    //cannonBarrel.CapRecoilDuration(this.m_ShotCooldown);
                }
            }*/

            this.weaponList.Remove(weapon);
            this.weaponCooldownOld.Remove(weapon);
            this.weaponRotationOld.Remove(weapon);
            this.weaponSpreadOld.Remove(weapon);
            this.weaponVelocityOld.Remove(weapon);
            //this.weaponDamageOld.Remove(weapon);
        }

        public void AddWheels(ModuleWheels wheels)
        {
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

            this.Update(new string[] { "WheelsRpm", "WheelsBrake", "WheelsTorque", "WheelsGrip", "WheelsSuspension" });
            //this.RefreshWheels(wheels, torque);
        }

        public void RemoveWheels(ModuleWheels wheels)
        {
            ManWheels.TorqueParams torque = (ManWheels.TorqueParams)field_TorqueParams.GetValue(wheels);
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
            this.wheelsSuspensionOld.Remove(wheels);
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
            this.itemProList.Add(item);
            this.itemProSpeedOld.Add(item, new List<float>()
            {
                (float)field_ItemProSpeed1.GetValue(item),
                (float)field_ItemProSpeed2.GetValue(item)
            });

            this.Update(new string[] { "ItemProSpeed" });
        }

        public void RemoveItemPro(ModuleItemProducer item)
        {
            field_ItemProSpeed1.SetValue(item, this.itemProSpeedOld[item][0]);
            field_ItemProSpeed2.SetValue(item, this.itemProSpeedOld[item][1]);
            this.itemProList.Remove(item);
            this.itemProSpeedOld.Remove(item);
        }

        public void AddHover(ModuleHover hover)
        {
            this.hoverList.Add(hover);
            /*this.hoverForceOld.Add(item, new List<float>()
            {
                (float)field_ItemProSpeed1.GetValue(item),
                (float)field_ItemProSpeed2.GetValue(item)
            });*/
            List<HoverJet> value_HoverJets = (List<HoverJet>)field_HoverJets.GetValue(hover);
            foreach (HoverJet jet in value_HoverJets)
            {
                if (!hoverForceOld.ContainsKey(hover))
                {
                    float value_ForceMax = (float)field_ForceMax.GetValue(jet);
                    this.hoverForceOld.Add(hover, value_ForceMax);
                }
                if (!hoverRangeOld.ContainsKey(hover))
                {
                    float value_ForceRangeMax = (float)field_ForceRangeMax.GetValue(jet);
                    this.hoverRangeOld.Add(hover, value_ForceRangeMax);
                }
                if (!hoverDampingOld.ContainsKey(hover))
                {
                    float value_Damping = (float)field_Damping.GetValue(jet);
                    this.hoverDampingOld.Add(hover, value_Damping);
                }
            }
            this.Update(new string[] { "HoverForce" , "HoverRange" , "HoverDamping" });
        }

        public void RemoveHover(ModuleHover hover)
        {
            List<HoverJet> value_HoverJets = (List<HoverJet>)field_HoverJets.GetValue(hover);
            foreach (HoverJet jet in value_HoverJets)
            {
                if (this.hoverForceOld.ContainsKey(hover))
                {
                    field_ForceMax.SetValue(jet, this.hoverForceOld[hover]);
                    field_ForceRangeMax.SetValue(jet, this.hoverRangeOld[hover]);
                    field_Damping.SetValue(jet, this.hoverDampingOld[hover]);
                }
            }
            this.hoverList.Remove(hover);
            this.hoverForceOld.Remove(hover);
            this.hoverRangeOld.Remove(hover);
            this.hoverDampingOld.Remove(hover);
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
    }
}
