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
        public static string[] FileNameFromDecal => ["FortifiedDecal"]; 
        public override string Name => "Decal";
        public override void GetLinkNodes()
        {
            AddInput<BloonTexture>("Texture");
        }
        public override void GetModuleProperties()
        {
            AddProperty(new EnumModuleProperty("Decal", ["Fortified"], 0));
        }
        public override void ProcessModule()
        {
            var texture = GetInputValue<BloonTexture>("Texture");
            if (texture.texture == null)
                return;

            var decalTexture = ModContent.GetTexture<BloonFactory>(FileNameFromDecal[GetValue<int>("Decal")]);
            for (int x = 0; x < decalTexture.width; x++)
            {
                for (int y = 0; y < decalTexture.height; y++)
                {
                    Color decalColor = decalTexture.GetPixel(x, y);
                    Color textureColor = texture.texture.GetPixel(x, y);
                    texture.texture.SetPixel(x, y, decalColor.a < 0.1f ? textureColor : decalColor);
                }
            }
            texture.texture.Apply();
        }
    }
}
