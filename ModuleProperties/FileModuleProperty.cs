using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using FactoryCore.API.ModuleValues;
using NfdSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Il2CppNinjaKiwi.GUTS.Models.BossRushRandomizerSettings;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;

namespace BloonFactory.ModuleProperties
{
    internal class FileModuleProperty : ModuleProperty
    {
        public string Filter;
        public bool HasValue => Module.GetValue<byte[]>(Name) != null;

        public ModHelperText Text;
        public FileModuleProperty(string name, string filter)
        {
            Name = name;
            Filter = filter;
        }
        public override ModHelperPanel GetVisual(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("FileModuleProperty", 0, 0, 1000, 300));
            var button = panel.AddButton(new Info("Button", 0, 0, 750, 250, new Vector2(0.5f, 0.5f)), VanillaSprites.GreenBtnLong, new Action(ButtonPressed));
            Text = button.AddText(new Info("Text", 0, 0, 700, 200, new Vector2(0.5f, 0.5f)), "Select File");
            Text.EnableAutoSizing(100, 0);

            SetText();
            return panel;
        }
        public void SetText()
        {
            Text.SetText(HasValue ? "Clear File" : "Select File");
        }
        public void ButtonPressed()
        {
            if (!HasValue)
            {
                FileDialogHelper.PrepareNativeDlls();

                if (Nfd.OpenDialog(Filter, "", out string path) == Nfd.NfdResult.NFD_OKAY)
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    Module.SetValue(bytes, Name);
                    SetText();
                }
            }
            else
            {
                Module.SetValue(null, Name);
                SetText();
            }
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(null, Name);
        }
    }
}
