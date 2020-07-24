using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace FFW_TT_BuffBlock
{
    public static class WrappedDataHolder
    {
        /*[HarmonyPatch(typeof(ManWheels.Wheel), "UpdateAttachData")] // Thanks Aceba!
        private static class FixUpdateAttachData
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);

                codes = codes.Skip(2).ToList();
                Console.WriteLine("FFW: Transpiled ManWheels.Wheel.UpdateAttachData()");
                return codes;
            }
        }

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

        [HarmonyPatch(typeof(ModuleItemProducer), "OnAttach")]
        class ModuleItemProducer_Attach_Patch
        {
            static bool Prefix(ref ModuleItemProducer __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddItemPro(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleItemProducer), "OnDetach")]
        class ModuleItemProducer_Detach_Patch
        {
            static bool Prefix(ref ModuleItemProducer __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveItemPro(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleHover), "OnAttach")]
        class ModuleHover_Attach_Patch
        {
            static bool Prefix(ref ModuleHover __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.AddHover(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleHover), "OnDetach")]
        class ModuleHover_Detach_Patch
        {
            static bool Prefix(ref ModuleHover __instance)
            {
                BuffController buff = BuffController.MakeNewIfNone(__instance.block.tank);
                buff.RemoveHover(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(ModuleRemoteCharger), "OnPool")]
        class TankBlock_Pool_Patch
        {
            static bool Prefix(ref ModuleRemoteCharger __instance)
            {
                ModuleRemoteChargerWrapper comp = __instance.gameObject.AddComponent<ModuleRemoteChargerWrapper>();
                comp.pointer = __instance;
                comp.OnPool();
                return true;
            }
        }*/
    }
}

