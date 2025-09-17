using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Unity.Display;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloonFactory.Modules.Display
{
    internal class BloonDisplay : ModDisplay2D
    {
        private static Dictionary<string, Texture2D> Cache = new Dictionary<string, Texture2D>();
        protected override string TextureName => "BaseBloon";
        public override DisplayCategory DisplayCategory => DisplayCategory.Bloon;
        public override string BaseDisplay => "9d3c0064c3ace7448bf8fefa4a97a70f";

        public override string Name => Guid;

        public string Guid;
        public const int Width = 250;
        public const int Height = 250;
        public const float PixelsPerUnit = 20;

        public Func<Texture2D> GenerateTexture;

        public BloonTemplate BloonTemplate;
        public BloonDisplay(Func<Texture2D> getTexture, BloonTemplate template, string id)
        {
            GenerateTexture = getTexture;
            BloonTemplate = template;
            mod = ModHelper.GetMod<BloonFactory>();
            Guid = id;

            Register();
        }
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            if (!Cache.TryGetValue(Guid, out var cachedTexture) || cachedTexture == null)
            {
                var texture = GenerateTexture?.Invoke();
                Cache.Add(Guid, texture);
                cachedTexture = texture;
            }
            var sprite = Sprite.Create(cachedTexture, new Rect(0, 0, Width, Height), new Vector2(0.5f, 0.5f), PixelsPerUnit);
            node.GetRenderer<SpriteRenderer>().sprite = sprite;
            node.isSprite = true;
        }
    }
}
