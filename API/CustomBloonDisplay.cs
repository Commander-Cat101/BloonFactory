using BTD_Mod_Helper;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Models.GenericBehaviors;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.API
{
    internal class CustomBloonDisplay : ModDisplay2D
    {
        public Color BloonColor = Color.white;
        protected override string TextureName { get; }

        public override string BaseDisplay { get; }
        public override float Scale { get; }
        public override DisplayCategory DisplayCategory { get; }

        public CustomBloonDisplay(BloonsMod bloonsmod, string textureName, Color color)
        {
            mod = bloonsmod;
            TextureName = textureName;
            Scale = 10f;
            BaseDisplay = Generic2dDisplay;
            DisplayCategory = DisplayCategory.Bloon;
            BloonColor = color;
        }
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            base.ModifyDisplayNode(node);
            node.sprite.color = BloonColor;
        }
    }
}
