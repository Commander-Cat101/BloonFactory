using BloonFactory;
using BloonFactory.LinkTypes;
using BloonFactory.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.ModOptions;
using FactoryCore.UI;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using Il2CppAssets.Scripts.Unity;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

[assembly: MelonInfo(typeof(BloonFactory.BloonFactory), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonFactory;

public class BloonFactory : BloonsTD6Mod
{
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
        ValueColors.ColorByLinkType[typeof(BloonModel)] = Color.green;
        ValueColors.ColorByLinkType[typeof(Trigger)] = Color.magenta;
        ValueColors.ColorByLinkType[typeof(Visuals)] = Color.cyan;
        ValueColors.ColorByLinkType[typeof(RoundModel)] = Color.yellow;
        ValueColors.ColorByLinkType[typeof(RoundSetModel)] = Color.blue;
        ValueColors.ColorByLinkType[typeof(BloonTexture)] = Color.red;
    }
    public override void OnNewGameModel(GameModel result)
    {
        foreach (var bloon in CustomBloon.Bloons)
        {
            try
            {
                var bloonModel = result.bloons.First(bl => bl.id == bloon.BloonTemplate.TemplateId);

                bloon.ModifyExistingBloonModel(bloonModel, result.roundSet);
                result.bloonsByName[bloonModel.name] = bloonModel;
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
    }
}