using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using MelonLoader;
using System.Linq;

namespace BloonFactory.Patches
{
    [HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
    public static class BloonMenu_CreateBloonButtons_Patch
    {
        [HarmonyPrefix]
        public static void Prefix(BloonMenu __instance, Il2CppSystem.Collections.Generic.List<BloonModel> sortedBloons)
        {
            foreach (var bloon in CustomBloon.Bloons)
            {  
                if (!sortedBloons.Any(a => a.id == bloon.BloonTemplate.TemplateId) && !bloon.BloonTemplate.IsQueueForDeletion)
                {
                    sortedBloons.Add(Game.instance.model.GetBloon(bloon.BloonTemplate.TemplateId));
                }
            }
        }
    }

    [HarmonyPatch(typeof(GameModel), nameof(GameModel.GetBloon))]
    internal static class GameModel_GetBloon
    {
        [HarmonyPostfix]
        private static bool Prefix(GameModel __instance, string id, ref BloonModel __result)
        {
            if (__instance.bloonsByName == null || __instance.bloonsByName.ContainsKey(id))
                return true;

            string newId = id.Replace("Regrow", "").Replace("Camo", "").Replace("Fortified", "");
            if (__instance.bloonsByName.TryGetValue(newId, out var value))
            {
                __result = value;
                return false;
            }

            return true;
        }
    }
}
