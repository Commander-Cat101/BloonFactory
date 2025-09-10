using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Api;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppAssets.Scripts.Utils.ObjectCache;

namespace BloonFactory.Modules.Display
{
    internal class BloonTextureModule : Module
    {
        public override string Name => "Bloon Texture";

        BloonTexture bloon;
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
            AddOutput<BloonTexture>("Texture", () => bloon);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new ColorModuleProperty("Color", Color.white));
        }
        public override void ProcessModule()
        {
            var bloonTexture = GetInputValue<BloonTexture>("Texture");

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

            bloonTexture.texture = texture;
            bloon = bloonTexture;
            GetOutputsModules("Texture").ProcessAll();
        }
    }
}
