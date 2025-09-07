using BloonFactory;
using BloonFactory.LinkTypes;
using BloonFactory.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using FactoryCore.UI;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using MelonLoader;
using System;
using System.Linq;
using UnityEngine;

[assembly: MelonInfo(typeof(BloonFactory.BloonFactory), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonFactory;

public class BloonFactory : BloonsTD6Mod
{
    public override void OnApplicationStart()
    {
        ModHelper.Msg<BloonFactory>("BloonFactory loaded!");
        SerializationHandler.LoadAllTemplates();
        ValueColors.ColorByLinkType[typeof(BloonModel)] = Color.green;
        ValueColors.ColorByLinkType[typeof(Trigger)] = Color.magenta;
        ValueColors.ColorByLinkType[typeof(Visuals)] = Color.cyan;
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ModGameMenu.Open<EditSelectorUI>();
        }
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