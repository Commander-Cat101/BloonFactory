using HarmonyLib;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Simulation.Track;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.UI.Patches
{
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.Open))]
    internal static class MainMenu_Open
    {
        [HarmonyPostfix]
        private static void Postfix(MainMenu __instance)
        {
            BloonEditorButton.Create(__instance);
        }
    }
    [HarmonyPatch(typeof(MainMenu), nameof(MainMenu.ReOpen))]
    internal static class MainMenu_ReOpen
    {
        [HarmonyPostfix]
        private static void Postfix(MainMenu __instance)
        {
            BloonEditorButton.Create(__instance);
        }
    }
}
