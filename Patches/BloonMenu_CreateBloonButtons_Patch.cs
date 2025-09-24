using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using CommandLine;
using HarmonyLib;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.BloonMenu;
using Il2CppSystem.Collections.Generic;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BloonFactory.Patches
{
    [HarmonyPatch(typeof(GameModel), nameof(GameModel.GetBloon))]
    internal static class GameModel_GetBloon
    {
        [HarmonyPostfix]
        private static bool Prefix(GameModel __instance, string id, ref BloonModel __result)
        {
            if (__instance.bloons == null || __instance.bloons.Any(a => a.id == id))
                return true;

            string newId = id.Replace("Regrow", "").Replace("Camo", "").Replace("Fortified", "");
            var bloon = __instance.bloons.FirstOrDefault(a => a.id == newId);
            if (bloon != null)
            {
                __result = bloon;
                return false;
            }

            if (__instance.bloons.Any(b => b.id.Contains(id)))
            {
                __result = __instance.bloons.First(b => b.id.Contains(id));
                return false;
            }

            return true;
        }
    }
    [HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
    public static class BloonMenu_CreateBloonButtons_Patch
    {
        const string name = "BLOONFACTORYBLOONSPAWNER";

        [HarmonyPrefix]
        private static void Prefix(BloonMenu __instance, ref List<BloonModel> sortedBloons)
        {
            sortedBloons = sortedBloons.Where(b => !CustomBloon.Bloons.Any(a => a.BloonTemplate.TemplateId == b.id));
        }

        [HarmonyPostfix]
        private static void Postfix(BloonMenu __instance)
        {
            foreach (GameObject child in __instance.bloonButtonContainer.GetChildren())
            {
                if (child.name == name)
                {
                    GameObject.Destroy(child);
                }
            }

            foreach (var bloon in CustomBloon.Bloons)
            {
                if (bloon.BloonTemplate.IsQueueForDeletion)
                    continue;

                var obj = GameObject.Instantiate(__instance.spawnBloonButtonPrefab, __instance.bloonButtonContainer.transform);
                obj.name = name;
                obj.RemoveComponent<SpawnBloonButton>();

                BloonModel model = InGame.instance.bridge.Model.GetBloon(bloon.BloonTemplate.TemplateId);

                obj.transform.FindChild("Icon").GetComponent<Image>().SetSprite(model.icon);

                obj.GetComponent<Button>().AddOnClick(() =>
                {
                    int spacing = int.Parse(__instance.bloonRate.text);
                    int count = int.Parse(__instance.bloonCount.text);
                    InGame.instance.bridge.SpawnBloons(InGame.instance.bridge.Model.CreateBloonEmissions(model, count, spacing * 5), 1, 0);
                });
            }
        }
    }
}
