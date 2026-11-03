using BloonFactory;
using BloonFactory.Handlers;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models;
using MelonLoader;
using System;
using System.Linq;

[assembly: MelonInfo(typeof(BloonFactory.BloonFactory), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonFactory;

public class BloonFactory : BloonsTD6Mod
{
    public static readonly ModSettingInt CustomBloonAmount = new ModSettingInt(16) { displayName = "Custom Bloon Amount", min = 1, max = 256, description = "The amount of custom bloons that can be loaded at one time. Increase if you have lots of bloons.", requiresRestart = true };
    
    public static readonly ModSettingCategory BloonBrowser = new ModSettingCategory("Bloon Browser");

    public static readonly ModSettingBool HideIncompatibleBloons = new(true)
    {
        category = BloonBrowser,
        displayName = "Hide Incompatible Bloons",
        description = "Hides bloons that have a version higher than the client in the bloon browser. These bloons are likely not to work on this version."
    };
    
    
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BloonFactory>("BloonFactory loaded!");
        SerializationHandler.LoadAllTemplates();
    }
    
    public override void OnNewGameModel(GameModel result)
    {
        BloonHandler.UpdateGameModel(result);
    }
}