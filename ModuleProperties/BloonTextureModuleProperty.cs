using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;
using static Il2CppNinjaKiwi.GUTS.Models.BossRushRandomizerSettings;

namespace BloonFactory.ModuleProperties
{
    internal class BloonTextureModuleProperty : ModuleProperty
    {
        public Func<Texture2D> GenerateTexture;

        public BloonTextureModuleProperty(Func<Texture2D> getTexture)
        {
            GenerateTexture = getTexture;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("BloonTextureModuleValue", 0, 0, 1000, 500));
            var image = panel.AddImage(new Info("BloonTexture", 0, 0, 500, 500, new Vector2(0.5f, 0.5f)), ModContent.GetSprite<BloonFactory>("BaseBloon"));
            
            panel.AddButton(new Info("Reload", -100, -50, 100, 100, new Vector2(1, 1)), VanillaSprites.RestartBtn, new Action(() =>
            {
                UpdateImage(image);
            }));
            UpdateImage(image);

            return panel;
        }
        public void UpdateImage(ModHelperImage image)
        {
            var texture = GenerateTexture.Invoke();
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 200);
            image.Image.sprite = sprite;
        }
        public override void LoadData()
        {

        }
    }
}
