using BloonFactory;
using BloonFactory.Modules.Core;
using BloonFactory.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using FactoryCore.API;
using FactoryCore.UI;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Bloons;
using MelonLoader;
using System.Linq;
using UnityEngine;
using static MelonLoader.MelonLogger;

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
            var bloonModel = result.bloons.First(bl => bl.id == bloon.BloonTemplate.TemplateId);

            MelonLogger.Msg($"Updating Existing BloonModel - {bloon.BloonTemplate.Name}");

            bloon.ModifyExistingBloonModel(bloonModel);
            result.bloonsByName[bloonModel.name] = bloonModel;
        }
    }
}