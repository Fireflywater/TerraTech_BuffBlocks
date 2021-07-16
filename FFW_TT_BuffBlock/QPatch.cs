using System;
using System.Reflection;
using HarmonyLib;
using Nuterra.BlockInjector;
using UnityEngine;

namespace FFW_TT_BuffBlock
{
    internal class QPatch
    {
        public static void Main()
        {
            Harmony harmonyInstance = new Harmony("ffw.ttmm.buffblock.mod");
            harmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
