using BloonFactory.LinkTypes;
using BTD_Mod_Helper.Api;
using FactoryCore.API;
using FactoryCore.API.ModuleProperties;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Store.Loot;
using JetBrains.Annotations;
using MelonLoader;
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
        public static string[] FileNameFromDecal => ["FortifiedDecal", "CamoDecal", "HorizontalStripesDecal", "VerticalStripesDecal", "HorizontalSplitDecal", "VerticalSplitDecal", "BucketHatDecal", "CapDecal", "ClownHairDecal", "CowboyHatDecal", "DisguiseDecal", "SunglassesDecal", "CagedDecal", "GlueDecal", "GoldenDecal", "HackedDecal", "IceDecal", "NailsDecal", "ShieldDecal"]; 
        public override string Name => "Decal";

        BloonTexture bloonTexture;
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
            AddOutput<BloonTexture>("Texture", () => bloonTexture);
        }
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Decal", ["Fortified", "Camo", "Horizontal Stripes", "Vertical Stripes", "Horizontal Split", "Vertical Split", "Bucket Hat", "Cap", "Clown Hair", "Cowboy Hat", "Disguise", "Sunglasses", "Caged", "Glue", "Golden", "Hacked", "Ice", "Nails", "Shielded"], 0));
            AddProperty(new ColorModuleProperty("Color", Color.white, false));

            AddProperty(new IntModuleProperty("X Offset", 0, int.MinValue, int.MaxValue));
            AddProperty(new IntModuleProperty("Y Offset", 0, int.MinValue, int.MaxValue));
        }
        public override void ProcessModule()
        {
            var texture = GetInputValue<BloonTexture>("Texture");
            if (texture.texture == null)
                return;

            int decal = GetValue<int>("Decal");

            if (decal >= FileNameFromDecal.Length)
                return;

            int xOffset = GetValue<int>("X Offset");
            int yOffset = GetValue<int>("Y Offset");

            Color color = GetValue<SavedColor>("Color");
            var decalTexture = ModContent.GetTexture<BloonFactory>(FileNameFromDecal[decal]);
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
