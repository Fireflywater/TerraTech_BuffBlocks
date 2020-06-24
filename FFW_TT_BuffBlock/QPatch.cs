using System;
using System.Reflection;
using Harmony;
using Nuterra.BlockInjector;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    internal class QPatch
    {
        public static void Main()
        {
            HarmonyInstance harmonyInstance = HarmonyInstance.Create("ffw.ttmm.buffblock.mod");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
