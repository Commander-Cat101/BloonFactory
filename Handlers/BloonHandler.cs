using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using MelonLoader;
using static BTD_Mod_Helper.Extensions.PopupScreenExt;

namespace BloonFactory.Handlers;

internal static class BloonHandler
{
    internal static CustomBloon[] AllBloons;
    internal static Dictionary<BloonTemplate, CustomBloon> bloonByTemplate;
    public static void Initialize(CustomBloon[] registeredBloons)
    {
        AllBloons = registeredBloons;
    }
    
    public static void UpdateGameModel(GameModel result)
    {
        Stack<CustomBloon> stack = new Stack<CustomBloon>(AllBloons);
        bloonByTemplate = new Dictionary<BloonTemplate, CustomBloon>();
        
        int unloadedBloons = 0;
        foreach (var template in SerializationHandler.Templates)
        {
            if (!stack.TryPop(out CustomBloon bloon))
            {
                MelonLogger.Error($"{template.Name} was unable to load because CustomBloon pool is empty. Please increase CustomBloonAmount in mod settings.");
                unloadedBloons++;
                continue;
            }
            bloonByTemplate.Add(template, bloon);
        }

        foreach (var (template, bloon) in bloonByTemplate)
        {
            try
            {
                var bloonModel = result.bloons.First(bl => bl.id == bloon.BloonGUID.ToString());
                bloon.ModifyExistingBloonModel(template, bloonModel, result.roundSet);
                result.bloonsByName[bloonModel.name] = bloonModel;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
        
        if (unloadedBloons > 0)
            PopupScreen.instance.SafelyQueue(screen =>
            {
                screen.ShowOkPopup($"{unloadedBloons} {(unloadedBloons == 1 ? "bloon was" : "bloons were")} unable to be loaded.\nPlease increase Custom Bloon Amount in BloonFactory settings and restart your game.");
            });
            
    }

    public static bool TryParseBloonId(string input, out string bloonId)
    {
        if (Game.instance.model.bloons.Any(a => a.id == input))
        {
            bloonId = input;
            return true;
        }
        if (SerializationHandler.TryGetTemplate(input, out var template))
        {
            TryGetTemplateBloonId(template, out bloonId);
            return true;
        }
        bloonId = "";
        return false;
    }
    public static bool TryGetTemplateBloonId(BloonTemplate template, out string id)
    {
        if (bloonByTemplate.TryGetValue(template, out CustomBloon bloon))
        {
            id = bloon.BloonGUID.ToString();
            return true;
        }
        id = "";
        return false;
    }
    public static bool IsIdActive(string id)
    {
        return bloonByTemplate.Any(bl => bl.Value.BloonGUID.ToString() == id);
    }
}