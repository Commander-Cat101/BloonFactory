using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class SimpleDisplayModule : Module
    {
        public override string Name => "Simple Display";
        public override void GetLinkNodes()
        {
            AddInput<Visuals>("Visuals");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new ColorModuleProperty("Color", Color.white));
        }
        public override void ProcessModule()
        {
            Visuals visuals = GetInputValue<Visuals>("Visuals");
            visuals.bloonModel.disallowCosmetics = true;
            var display = new SimpleBloonDisplay(GenerateTexture, (BloonTemplate)Template);
            display.Apply(visuals.bloonModel);
        }
        public Texture2D GenerateTexture()
        {
            var baseBloon = ModContent.GetTexture<BloonFactory>("BaseBloon");
            Color color = GetValue<SavedColor>("Color");
            var colors = baseBloon.GetPixels();
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = colors[i] * color;
            }
            var texture = new Texture2D(baseBloon.width, baseBloon.height);
            texture.SetPixels(colors);
            texture.Apply();

            return texture;
        }
    }
}
