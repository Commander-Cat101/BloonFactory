using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using FactoryCore.API.ModuleValues;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppNinjaKiwi.Common;
using Il2CppNinjaKiwi.Common.ResourceUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using BloonFactory.Handlers;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using static Il2CppSystem.Linq.Expressions.Interpreter.CastInstruction.CastInstructionNoT;
using Action = System.Action;
using BloonType = BTD_Mod_Helper.Api.Enums.BloonType;

namespace BloonFactory.ModuleProperties
{
    public class BloonEnumModuleProperty : ModuleProperty
    {
        private string TemplateUUID;
        private string DisplayName;

        private ModHelperPanel scrollPanel;
        private ModHelperInputField inputField;
        private ModHelperButton button;

        private State currentState;
        public BloonEnumModuleProperty(string displayName, string name, string templateUUID)
        {
            Name = name;
            DisplayName = displayName;
            TemplateUUID = templateUUID;
        }
        public override ModHelperPanel CreateTemplate()
        {
            var panel = ModHelperPanel.Create(new Info("EnumModuleProperty", 0, 0, 1000, 100));

            panel.AddText(new Info("Text", -250, 0, 400, 100), "Property Name", 50, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing();

            var inputField = panel.AddInputField(new Info("Type", 225, 0, 450, 80), "Default Value", VanillaSprites.BlueInsertPanelRound);
            inputField.Text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
            
            inputField.AddButton(new Info("Icon", -40, 0, 60, 60, new Vector2(1, 0.5f)), VanillaSprites.SearchIcon, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
            }));

            var scrollPanel = panel.AddPanel(new Info("Options", 225, 10, 450, 300, new Vector2(0.5f, 0), new Vector2(0.5f, 1)), null);
            var vert = scrollPanel.AddComponent<VerticalLayoutGroup>();
            vert.childControlHeight = false;
            scrollPanel.FitContent(vertical: ContentSizeFitter.FitMode.MinSize);
            return panel;
        }

        public override void SetupProperty(ModHelperPanel root)
        {
            var value = Module.GetValue<string>(Name);
            
            root.transform.Find("Text").GetComponent<ModHelperText>().SetText(DisplayName);
            inputField = root.transform.Find("Type").GetComponent<ModHelperInputField>();
            scrollPanel = root.transform.Find("Options").GetComponent<ModHelperPanel>();
            button = inputField.transform.Find("Icon").GetComponent<ModHelperButton>();
            
            inputField.SetText(!IsBloonAvailable(value, out var bloon) ? "Unavailable Bloon" : bloon.name, false);
            inputField.InputField.onValueChanged.AddListener(new Action<string>(a =>
            {
                CreateContentForInput(a);
            }));
            inputField.InputField.onSelect.AddListener(new Action<string>(_ =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
            }));
            
            if (value == string.Empty)
            {
                SetState(State.Selecting);
                return;
            }
            SetState(State.Selected);
        }

        public void CreateContentForInput(string input)
        {
            string strippedInput = input.ToLowerInvariant().Trim().Replace(" ", "");

            (string name, string id)[] bloons = GetAvailableBloons()
                .Where(a => a.name.ToLowerInvariant().Replace(" ", "").Trim().Contains(strippedInput))
                .OrderBy(a => a.name.Length).ToArray();

            scrollPanel.transform.DestroyAllChildren();

            for (int i = 0; i < 3; i++)
            {
                if (bloons.Length <= i)
                    break;

                CreateOption(bloons[i].name, bloons[i].id).AddTo(scrollPanel);
            }

            LayoutRebuilder.MarkLayoutForRebuild(scrollPanel);
        }
        public ModHelperPanel CreateOption(string name, string id)
        {
            var panel = ModHelperPanel.Create(new Info($"Option {name}", 0, 0, 450, 100), VanillaSprites.BlueInsertPanel);

            var button = panel.AddButton(new Info("Button", 0, 0, 400, 80, new Vector2(0.5f, 0.5f)), null, new Action(() =>
            {
                SelectOption(name, id);
            }));
            
            var text = button.AddText(new Info("Text", 0, 0, 400, 80, new Vector2(0.5f, 0.5f)), name);
            text.EnableAutoSizing(40);
            text.Text.overflowMode = Il2CppTMPro.TextOverflowModes.Overflow;
            return panel;
        }
        public void SelectOption(string name, string id)
        {
            inputField.InputField.interactable = false;
            inputField.SetText(name, false);

            scrollPanel.transform.DestroyAllChildren();

            SetState(State.Selected);
            Module.SetValue(id, Name);
            MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
        }

        private void SetState(State state)
        {
            currentState = state;
            if (state == State.Selected)
            {
                button.Button.SetSprite(ModContent.GetSprite<BloonFactory>("Cross"));

                button.Button.onClick.RemoveAllListeners();
                button.Button.onClick.AddListener(() =>
                {
                    SetState(State.Selecting);
                });
            }
            else
            {
                Module.SetValue("", Name);
                ResourceLoader.LoadSpriteFromSpriteReferenceAsync(ModContent.CreateSpriteReference(VanillaSprites.SearchIcon), button.Button.image, true);
                inputField.SetText("", false);
                inputField.InputField.interactable = true;
            }
        }

        public IEnumerable<(string name, string id)> GetAvailableBloons()
        {
            var bloons = Game.instance.model.bloons.Where(a => !a.name.Contains("BloonFactory")).Select(a => (a.name, a.id)).ToList();

            foreach (var template in SerializationHandler.Templates)
            {
                if (template.GetUUID() == TemplateUUID)
                    continue;
                
                bloons.Add((template.Name, template.GetUUID()));
            }
            
            return bloons;
        }

        public bool IsBloonAvailable(string id, out (string name, string id) bloon)
        {
            bloon = ("", "");
            var availableBloons = GetAvailableBloons();
            if (!availableBloons.Any(a => a.id == id))
            {
                return false;
            }
            bloon = availableBloons.First(a => a.id == id);
            return true;
        }
        public override void LoadData()
        {
            if (!Module.HasValue(Name))
                Module.SetValue(BloonType.Red, Name);
        }

        private enum State
        {
            Selected,
            Selecting
        }
    }
}
