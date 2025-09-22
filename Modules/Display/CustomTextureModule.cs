using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using FactoryCore.API;
using Il2CppAssets.Scripts.Simulation.Bloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class CustomTextureModule : Module
    {
        public override string Name => "Custom Texture";

        public override string Description => "The standard size for a bloon texture is 250 x 250.\n  Supports png and jpg files.";

        BloonTexture bloon;
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
            AddOutput<BloonTexture>("Texture", () => bloon);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FileModuleProperty("Image", "png,jpg"));
        }
        public override void ProcessModule()
        {
            byte[] bytes = GetValue<byte[]>("Image");
            
            BloonTexture texture = GetInputValue<BloonTexture>("Texture");
            bloon = texture;

            if (bytes == null)
            {
                GetOutputsModules("Texture").ProcessAll();
                return;
            }

            var customDisplay = new Texture2D(2, 2) { filterMode = FilterMode.Bilinear, mipMapBias = -0.5f };
            if (ImageConversion.LoadImage(customDisplay, bytes))
            {
                texture.texture = customDisplay;
            }
            GetOutputsModules("Texture").ProcessAll();
        }
    }
}
