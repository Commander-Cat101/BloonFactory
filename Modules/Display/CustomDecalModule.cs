using BloonFactory.LinkTypes;
using BloonFactory.ModuleProperties;
using BTD_Mod_Helper.Api;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Store.Loot;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class CustomDecalModule : Module
    {
        public override string Name => "Custom Decal";

        BloonTexture bloonTexture;
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
            AddOutput<BloonTexture>("Texture", () => bloonTexture);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new FileModuleProperty("Decal Image", "png,jpg"));
            AddProperty(new ColorModuleProperty("Color", Color.white, false));

            AddProperty(new IntModuleProperty("X Offset", 0, int.MinValue, int.MaxValue));
            AddProperty(new IntModuleProperty("Y Offset", 0, int.MinValue, int.MaxValue));
        }
        public override void ProcessModule()
        {
            var texture = GetInputValue<BloonTexture>("Texture");
            if (texture.texture == null)
            {
                bloonTexture = texture;
                GetOutputsModules("Texture").ProcessAll();
            }

            byte[] bytes = GetValue<byte[]>("Decal Image");

            if (bytes == null || bytes.Length == 0)
            {
                bloonTexture = texture;
                GetOutputsModules("Texture").ProcessAll();
                return;
            }

            int xOffset = GetValue<int>("X Offset");
            int yOffset = GetValue<int>("Y Offset");

            Color color = GetValue<SavedColor>("Color");

            Texture2D decalTexture = new Texture2D(2, 2) { filterMode = FilterMode.Bilinear, mipMapBias = -0.5f };
            if (!ImageConversion.LoadImage(decalTexture, bytes))
                return;

            for (int x = 0; x < decalTexture.width; x++)
            {
                for (int y = 0; y < decalTexture.height; y++)
                {
                    if (x + xOffset < 0 || x + xOffset >= texture.texture.width || y + yOffset < 0 || y + yOffset >= texture.texture.height)
                        continue;

                    Color decalColor = decalTexture.GetPixel(x, y) * color;
                    Color textureColor = texture.texture.GetPixel(x + xOffset, y + yOffset);
                    texture.texture.SetPixel(x + xOffset, y + yOffset, OverlayColor(textureColor, decalColor));
                }
            }
            texture.texture.Apply();

            bloonTexture = texture;
            GetOutputsModules("Texture").ProcessAll();
        }
        public static Color OverlayColor(Color baseColor, Color overlayColor)
        {
            float a = overlayColor.a;
            var color = overlayColor;
            color.a = 1;
            return Color.Lerp(baseColor, color, a);
        }
    }
}
