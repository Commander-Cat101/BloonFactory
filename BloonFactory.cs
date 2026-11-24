using BloonFactory;
using BloonFactory.Handlers;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.ModOptions;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Unity;
using MelonLoader;

[assembly: MelonInfo(typeof(BloonFactory.BloonFactory), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonFactory;

public class BloonFactory : BloonsTD6Mod
{
    public static readonly ModSettingInt CustomBloonAmount = new(32) { displayName = "Custom Bloon Amount", min = 1, max = 256, description = "The amount of custom bloons that can be loaded at one time. Increase if you have lots of bloons.", requiresRestart = true };
    
    public static readonly ModSettingBool HideIncompatibleBloons = new(true)
    {
        displayName = "Hide Incompatible Bloons",
        description = "Hides bloons that have a version higher than the client in the bloon browser. These bloons are likely not to work on this version.",
    };
    public static readonly ModSettingBool ShowUnverifiedBloons = new(false);

    public static bool DisableOnlineFeatures = true;
    
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BloonFactory>("BloonFactory loaded!");
        SerializationHandler.LoadAllTemplates();
    }

    public override void OnMainMenu()
    {
        
    }

    public override void OnNewGameModel(GameModel result)
    {
        BloonHandler.UpdateGameModel(result);
    }
}