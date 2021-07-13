using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace FFW_TT_BuffBlock
{
    public static class HarmonyPatchMk2
    {
        [HarmonyPatch(typeof(ManWheels.Wheel), "UpdateAttachData")] // Thanks Aceba!
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

        [HarmonyPatch(typeof(TankBlock), "OnPool")]
        class TankBlock_Pool_Patch
        {
            static bool Prefix(ref TankBlock __instance)
            {
                ModuleBuffWrapperMk2 comp = __instance.gameObject.AddComponent<ModuleBuffWrapperMk2>();
                comp.pointer = __instance;
                comp.Init();
                return true;
            }
        }
    }
}
