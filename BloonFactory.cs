using BloonFactory;
using BloonFactory.Modules.Core;
using BloonFactory.UI;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using MelonLoader;
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
    }
    public override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ModGameMenu.Open<EditSelectorUI>();
        }
    }
}