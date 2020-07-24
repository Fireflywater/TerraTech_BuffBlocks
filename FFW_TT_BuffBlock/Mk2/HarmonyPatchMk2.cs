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
    public static class HarmonyPatchMk2
    {
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
