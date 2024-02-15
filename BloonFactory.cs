using MelonLoader;
using BTD_Mod_Helper;
using BloonFactory;
using BloonFactory.UI;
using Il2CppAssets.Scripts.Unity.UI_New.Main;
using UnityEngine;
using System.IO;
using MelonLoader.Utils;
using Newtonsoft.Json;
using BloonFactory.API.Bloons;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Models.Bloons;

[assembly: MelonInfo(typeof(BloonFactory.BloonFactory), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BloonFactory;

public class BloonFactory : BloonsTD6Mod
{
    public static string FolderDirectory = Path.Combine(MelonEnvironment.ModsDirectory, "BloonFactory");
    public override void OnApplicationStart()
    {
        LoadBloons();
        ModHelper.Msg<BloonFactory>("BloonFactory loaded!");
    }
    public override void OnMainMenu()
    {
        foreach (var bloon in CustomBloon.RegisteredBloons)
        {
            bloon.Register();
        }
    }
    public void LoadBloons()
    {
        if (!Directory.Exists(FolderDirectory))
        {
            Directory.CreateDirectory(FolderDirectory);

            return;
        }
        foreach (var file in Directory.GetFiles(FolderDirectory))
        {
            var bloon = JsonConvert.DeserializeObject<CustomBloonSerializable>(File.ReadAllText(file));
            if (bloon != null)
            {
                CustomBloon customBloon = new CustomBloon(bloon);
                CustomBloon.RegisteredBloons.Add(customBloon);
            }
        }
    }
}