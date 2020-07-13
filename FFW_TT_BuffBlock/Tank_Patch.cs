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
    }
}

