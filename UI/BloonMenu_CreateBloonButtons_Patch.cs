using System;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Internal;
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
using MelonLoader.Utils;
using System.IO;
using System.Linq;
using BloonFactory.Handlers;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace BloonFactory.UI
{
    [HarmonyPatch(typeof(GameModel), nameof(GameModel.GetBloon))]
    internal static class GameModel_GetBloon
    {
        [HarmonyPostfix]
        private static bool Prefix(GameModel __instance, string id, ref BloonModel __result)
        {
            try
            {
                if (__instance.bloons == null || __instance.bloons.Any(a => a.id == id))
                    return true;
                
                if (__instance.bloons.Any(b => b.id.Contains(id)))
                {
                    __result = __instance.bloons.First(b => b.id.Contains(id));
                    return false;
                }
                
                string newId = id.Replace("Regrow", "").Replace("Camo", "").Replace("Fortified", "");
                var bloon = __instance.bloons.FirstOrDefault(a => a.id == newId);
                if (bloon != null)
                {
                    __result = bloon;
                    return false;
                }
                
                if (id.Contains("BloonFactory") && !BloonHandler.IsIdActive(id))
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
                return false;
            }
        }
    }
    [HarmonyPatch(typeof(BloonMenu), nameof(BloonMenu.CreateBloonButtons))]
    public static class BloonMenu_CreateBloonButtons_Patch
    {
        const string name = "BLOONFACTORYBLOONSPAWNER";

        [HarmonyPrefix]
        private static void Prefix(BloonMenu __instance, ref List<BloonModel> sortedBloons)
        {
            sortedBloons = sortedBloons.Where(b => !BloonHandler.AllBloons.Any(a => a.Name == b.id));
        }

        [HarmonyPostfix]
        private static void Postfix(BloonMenu __instance)
        {
            foreach (GameObject child in __instance.bloonButtonContainer.GetChildren())
            {
                if (child.name == name)
                {
                    Object.Destroy(child);
                }
            }

            foreach (var template in SerializationHandler.Templates)
            {
                if (!BloonHandler.TryGetTemplateBloonId(template, out string id))
                {
                    MelonLogger.Msg($"Template \"{template.Name}\" does not have a corresponding bloonId, skipping.");
                    continue;
                }
                
                var obj = Object.Instantiate(__instance.spawnBloonButtonPrefab, __instance.bloonButtonContainer.transform);
                obj.name = name;
                obj.RemoveComponent<SpawnBloonButton>();
                
                BloonModel model = InGame.instance.bridge.Model.GetBloon(id);

                obj.transform.FindChild("Icon").GetComponent<Image>().SetSprite(model.icon);
                obj.transform.FindChild("RawIcon").gameObject.SetActive(false);

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
