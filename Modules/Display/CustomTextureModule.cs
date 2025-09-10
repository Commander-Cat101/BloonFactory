using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using FactoryCore.API;
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

        public override string Description => "The standard size for a bloon texture is 250 x 250.\n Supports png and jpg files.";
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FileModuleProperty(null, "Image", "png,jpg"));
        }
        public override void ProcessModule()
        {
            BloonTexture texture = GetInputValue<BloonTexture>("Texture");
            var customDisplay = new Texture2D(2, 2) { filterMode = FilterMode.Bilinear, mipMapBias = -0.5f };
            if (ImageConversion.LoadImage(customDisplay, GetValue<byte[]>("Image")))
            {
                texture.texture = customDisplay;
            }
        }
    }
}
