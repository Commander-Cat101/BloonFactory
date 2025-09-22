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
    public static readonly ModSettingInt BloonsPerPage = new(20)
    {
        category = BloonBrowser,
        displayName = "Bloons Per Page"
    };
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BloonFactory>("BloonFactory loaded!");
        SerializationHandler.LoadAllTemplates();
        ValueColors.ColorByLinkType[typeof(BloonModel)] = Color.green;
        ValueColors.ColorByLinkType[typeof(Trigger)] = Color.magenta;
        ValueColors.ColorByLinkType[typeof(Visuals)] = Color.cyan;
    }
    public override void OnMainMenu()
    {
        /*foreach (var prop in GameData.Instance.mapEditorData.mapEditorProps)
        {
            MelonLogger.Msg($"Prop: {prop.name}");
            if (GameData.Instance.mapEditorData.TryGetMapEditorProp(prop.id, out var data))
            {
                MelonLogger.Msg("Found it");
            }
        }*/
    }
    public override void OnNewGameModel(GameModel result)
    {
        foreach (var bloon in CustomBloon.Bloons)
        {
            try
            {
                var bloonModel = result.bloons.First(bl => bl.id == bloon.BloonTemplate.TemplateId);
                MelonLogger.Msg($"Updating Existing BloonModel - {bloon.BloonTemplate.Name}");

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