using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Bloons;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppNinjaKiwi.NKMulti.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BloonFactory.API.Bloons
{
    public class CustomBloon : CustomBloonSerializable
    {
        public static List<CustomBloon> RegisteredBloons = new List<CustomBloon>();

        internal static readonly Dictionary<string, BloonModel> BloonCache = new();
        
        public CustomBloon(CustomBloonSerializable serializable)
        {
            BaseColor = new Color(serializable.R, serializable.G, serializable.B);
            Name = serializable.Name;
            Speed = serializable.Speed;
            Health = serializable.Health;
        }
        public CustomBloon(string name)
        {
            Name = name;
        }

        [JsonIgnore]
        public Color BaseColor { get => new Color(R, G, B); 
            set
            {
                R = value.r;
                G = value.g;
                B = value.b;
            }
        }

       

        public void Register()
        {
            BloonModel model = Game.instance.model.GetBloon(BloonType.Red);

            model.disallowCosmetics = true;

            var display = new CustomBloonDisplay(ModHelper.GetMod<BloonFactory>(), "BaseBloon", BaseColor);
            display.Apply(model);

            model.icon = ModContent.GetSpriteReference<BloonFactory>("BaseBloon");

            model.RemoveTag(model.GetBaseID());

            model.AddTag(model.baseId);

            model.id = model.name = Name;

            model.SetCamo(Camo);
            model.SetFortified(Fortified);

            if (Regrow)
            {
                model.SetRegrow(model.id, RegrowRate);
            }

            model.updateChildBloonModels = true;

            Game.instance.model.bloons = Game.instance.model.bloons.AddTo(model);
            Game.instance.model.AddChildDependant(model);
            Game.instance.model.bloonsByName[model.name] = model;

            BloonCache[Name] = model;
        }
    }
}
