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
    internal class TintModule : Module
    {
        public override string Name => "Tint";

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

            Color color = GetValue<SavedColor>("Color");

            for (int x = 0; x < bloonTexture.texture.width; x++)
            {
                for (int y = 0; y < bloonTexture.texture.height; y++)
                {
                    Color textureColor = bloonTexture.texture.GetPixel(x, y);
                    bloonTexture.texture.SetPixel(x, y, textureColor * color);
                }
            }
            bloonTexture.texture.Apply();

            bloon = bloonTexture;
            GetOutputsModules("Texture").ProcessAll();
        }
    }
}
