using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Api;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using Il2CppAssets.Scripts.Models.Store.Loot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class DecalModule : Module
    {
        public static string[] FileNameFromDecal => ["FortifiedDecal", "CamoDecal", "HorizontalStripesDecal", "VerticalStripesDecal", "HorizontalSplitDecal", "VerticalSplitDecal"]; 
        public override string Name => "Decal";

        BloonTexture bloonTexture;
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
            AddOutput<BloonTexture>("Texture", () => bloonTexture);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Decal", ["Fortified", "Camo", "Horizontal Stripes", "Vertical Stripes", "Horizontal Split", "Vertical Split"], 0));
            AddProperty(new ColorModuleProperty("Color", Color.white, false));
        }
        public override void ProcessModule()
        {
            var texture = GetInputValue<BloonTexture>("Texture");
            if (texture.texture == null)
                return;

            Color color = GetValue<SavedColor>("Color");
            var decalTexture = ModContent.GetTexture<BloonFactory>(FileNameFromDecal[GetValue<int>("Decal")]);
            for (int x = 0; x < decalTexture.width; x++)
            {
                for (int y = 0; y < decalTexture.height; y++)
                {
                    Color decalColor = decalTexture.GetPixel(x, y) * color;
                    Color textureColor = texture.texture.GetPixel(x, y);
                    texture.texture.SetPixel(x, y, OverlayColor(textureColor, decalColor));
                }
            }
            texture.texture.Apply();

            bloonTexture = texture;
            GetOutputsModules("Texture").ProcessAll();
        }
        public Color OverlayColor(Color baseColor, Color overlayColor)
        {
            float a = overlayColor.a;
            var color = overlayColor;
            color.a = 1;
            return Color.Lerp(baseColor, color, a);
        }
    }
}
