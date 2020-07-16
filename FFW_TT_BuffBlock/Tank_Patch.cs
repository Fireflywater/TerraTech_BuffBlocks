using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace FFW_TT_BuffBlock
{
    public static class WrappedDataHolder
    {


        /*[HarmonyPatch(typeof(ManWheels.Wheel), "UpdateAttachData")]
        class ManWheelsWheel_AttachData_Patch
        {
            static bool Prefix(ref ManWheels.Wheel __instance, ref ModuleWheels.AttachData moduleData)
            {
                FieldInfo field_WheelState = typeof(ManWheels)
                    .GetField("m_WheelState", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo field_AttachedId = typeof(ManWheels.Wheel)
                    .GetField("attachedID", BindingFlags.NonPublic | BindingFlags.Instance);
                
                int value_WheelAttachedId = (int)field_AttachedId.GetValue(__instance);
                Array value_WheelState = (Array)field_WheelState.GetValue(Singleton.Manager<ManWheels>.inst);
                object value_AttachedWheelState = value_WheelState.GetValue(value_WheelAttachedId);
                
                MethodInfo method_Init = value_AttachedWheelState.GetType().GetMethod("Init");
                MethodInfo method_RecalculateDotProducts = value_AttachedWheelState.GetType().GetMethod("RecalculateDotProducts");

                object[] parametersArray = new object[] { __instance, moduleData };

                method_Init.Invoke(value_AttachedWheelState, parametersArray);
                method_RecalculateDotProducts.Invoke(value_AttachedWheelState, null);
                return false;
            }
        }*/

        /*[HarmonyPatch(typeof(Extensions), "RandomVariance")]
        class Vector3_RandomVariance_Patch
        {
            static Vector3 Postfix(ref Vector3 v, ref float variance, ref Vector3 __result)
            {
                Console.WriteLine("FFW?");
                return new Vector3(
                    v.x + UnityEngine.Random.Range(-variance, variance), 
                    v.y + UnityEngine.Random.Range(-variance, variance), 
                    v.z + UnityEngine.Random.Range(-variance, variance)
                );
            }
        }*/
        
        [HarmonyPatch(typeof(ModuleWeaponGun), "OnAttach")]
        class ModuleWeaponGun_Attach_Patch
        {
            static bool Prefix(ref ModuleWeaponGun __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddWeapon(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleWeaponGun), "OnDetach")]
        class ModuleWeaponGun_Detach_Patch
        {
            static void Postfix(ref ModuleWeaponGun __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveWeapon(__instance);
            }
        }

        [HarmonyPatch(typeof(ModuleWheels), "OnAttach")]
        class ModuleWheels_Attach_Patch
        {
            static bool Prefix(ref ModuleWheels __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddWheels(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleWheels), "OnDetach")]
        class ModuleWheels_Detach_Patch
        {
            static bool Prefix(ref ModuleWheels __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveWheels(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleBooster), "OnAttach")]
        class ModuleBooster_Attach_Patch
        {
            static bool Prefix(ref ModuleBooster __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddBooster(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleBooster), "OnDetach")]
        class ModuleBooster_Detach_Patch
        {
            static bool Prefix(ref ModuleBooster __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveBooster(__instance);
                return true;
            }
        }

        /*[HarmonyPatch(typeof(ModuleShieldGenerator), "OnAttach")]
        class ModuleShieldGenerator_Attach_Patch
        {
            static bool Prefix(ref ModuleShieldGenerator __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddShield(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleShieldGenerator), "OnDetach")]
        class ModuleShieldGenerator_Detach_Patch
        {
            static bool Prefix(ref ModuleShieldGenerator __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveShield(__instance);
                return true;
            }
        }*/

        [HarmonyPatch(typeof(ModuleDrill), "OnAttach")]
        class ModuleDrill_Attach_Patch
        {
            static bool Prefix(ref ModuleDrill __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddDrill(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleDrill), "OnDetach")]
        class ModuleDrill_Detach_Patch
        {
            static bool Prefix(ref ModuleDrill __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveDrill(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleEnergy), "OnAttach")]
        class ModuleEnergy_Attach_Patch
        {
            static bool Prefix(ref ModuleEnergy __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddEnergy(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleEnergy), "OnDetach")]
        class ModuleEnergy_Detach_Patch
        {
            static bool Prefix(ref ModuleEnergy __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveEnergy(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleEnergyStore), "OnAttach")]
        class ModuleEnergyStore_Attach_Patch
        {
            static bool Prefix(ref ModuleEnergyStore __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddEnergyStore(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleEnergyStore), "OnDetach")]
        class ModuleEnergyStore_Detach_Patch
        {
            static bool Prefix(ref ModuleEnergyStore __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveEnergyStore(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleItemConsume), "OnAttach")]
        class ModuleItemConsume_Attach_Patch
        {
            static bool Prefix(ref ModuleItemConsume __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddItemCon(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleItemConsume), "OnDetach")]
        class ModuleItemConsume_Detach_Patch
        {
            static bool Prefix(ref ModuleItemConsume __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveItemCon(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleHeart), "OnAttach")]
        class ModuleHeart_Attach_Patch
        {
            static bool Prefix(ref ModuleHeart __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddHeart(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleHeart), "OnDetach")]
        class ModuleHeart_Detach_Patch
        {
            static bool Prefix(ref ModuleHeart __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveHeart(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleItemPickup), "OnAttach")]
        class ModuleItemPickup_Attach_Patch
        {
            static bool Prefix(ref ModuleItemPickup __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddItemPickup(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleItemPickup), "OnDetach")]
        class ModuleItemPickup_Detach_Patch
        {
            static bool Prefix(ref ModuleItemPickup __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveItemPickup(__instance);
                return true;
            }
        }
    }
}

