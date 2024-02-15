using BloonFactory.API.Bloons;
using BloonFactory.API.Enums;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppAssets.Scripts.Utils;
using Il2CppNinjaKiwi.Common;
using Il2CppTMPro;
using MelonLoader;
using Newtonsoft.Json;
using NfdSharp;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace BloonFactory.UI
{
    internal class BloonEditor : ModGameMenu<SettingsScreen>
    {
        internal SpriteReference Bloon = ModContent.GetSpriteReference<BloonFactory>("BaseBloon");

        ModHelperButton Visuals;
        ModHelperButton Stats;

        public ModHelperPanel Visualizer;
        public ModHelperPanel Settings;

        public static CustomBloon selectedBloon;

        public ModHelperImage Bloonimage;

        public override void OnMenuClosed()
        {
            try
            {
                var content = JsonConvert.SerializeObject((CustomBloonSerializable)selectedBloon, Formatting.None);
                File.WriteAllText(Path.Combine(BloonFactory.FolderDirectory, $"{selectedBloon.Name}.json"), content);
            }
            catch (Exception ex)
            {
                MelonLogger.Log($"Failed to save bloon {selectedBloon.Name}: {ex.Message}");
            }
            MelonLogger.Log("Saved Bloon!");
        }
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            CommonForegroundHeader.SetText("Bloon Editor");

            GameMenu.transform.DestroyAllChildren();

            var BloonMenu = GameMenu.gameObject.AddModHelperPanel(new Info("BloonMenu", 3600, 1900));

            CreateLeftPanel(BloonMenu);
            CreateRightPanel(BloonMenu);
            
            UpdateVisuals();

            return false;
        }
        public void CreateRightPanel(ModHelperPanel panel)
        {
            CreateRightPanelButtons(panel);
            var outline = panel.AddPanel(new Info("EditorPanel", 600, -100, 2000, 1600), VanillaSprites.MainBGPanelBlue);
            Settings = outline.AddPanel(new Info("Settings", 0, 0, 1900, 1500), VanillaSprites.BlueInsertPanelRound);

            SelectEditorPanel(EditorPanel.Visuals);
        }
        public void CreateRightPanelButtons(ModHelperPanel panel)
        {
            Visuals = panel.AddButton(new Info("VisualsButton", -175, 800, 450, 175), VanillaSprites.BlueBtnLong, new Action(() => { }));
            Visuals.Button.AddOnClick(() =>
            {
                SelectEditorPanel(EditorPanel.Visuals);
            });
            Visuals.AddText(new Info("Text", 0, 0, 450, 175), "Visuals", 75);

            Stats = panel.AddButton(new Info("StatsButton", 325, 800, 450, 175), VanillaSprites.BlueBtnLong, new Action(() => { }));
            Stats.Button.AddOnClick(() =>
            {
                SelectEditorPanel(EditorPanel.Stats);
            });
            Stats.AddText(new Info("Text", 0, 0, 450, 175), "Stats", 75);
        }
        public void CreateLeftPanel(ModHelperPanel panel)
        {
            var outline = panel.AddPanel(new Info("VisualizerOutline", -1100, 0, 1200, 1800), VanillaSprites.MainBGPanelBlue);
            Visualizer = outline.AddPanel(new Info("Visualizer", 0, 0, 1100, 1700), VanillaSprites.BlueInsertPanel);
            Bloonimage = Visualizer.AddImage(new Info("BloonImage", 0, 0, 1000, 1000), VanillaSprites.Red);
            Bloonimage.Image.SetSprite(Bloon);
            Bloonimage.Image.color = selectedBloon.BaseColor;
        }

        public void SelectEditorPanel(EditorPanel panel)
        {
            Settings.transform.DestroyAllChildren();

            Visuals.Button.interactable = true;
            Stats.Button.interactable = true;

            switch (panel)
            {
                case EditorPanel.Visuals:
                    Visuals.Button.interactable = false;

                    var BaseColorPanel = Settings.AddPanel(new Info("BaseColor", -617, 0, 566, 1400), VanillaSprites.MainBGPanelBlue);
                    BaseColorPanel.AddText(new Info("Text", 0, 550, 550, 200), "Base Color").GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
                    var RSlider = BaseColorPanel.AddSlider(new Info("RSlider", 0, 0, 400, 50), 0, 0, 255, 1, new Vector2(100, 100), new Action<float>(value =>
                    {
                        selectedBloon.R = value / 255;
                        UpdateVisuals();
                    }));
                    var GSlider = BaseColorPanel.AddSlider(new Info("GSlider", 0, -200, 400, 50), 0, 0, 255, 1, new Vector2(100, 100), new Action<float>(value =>
                    {
                        selectedBloon.G = value / 255;
                        UpdateVisuals();
                    }));
                    var BSlider = BaseColorPanel.AddSlider(new Info("BSlider", 0, -400, 400, 50), 0, 0, 255, 1, new Vector2(100, 100), new Action<float>(value =>
                    {
                        selectedBloon.B = value / 255;
                        UpdateVisuals();
                    }));
                    RSlider.SetCurrentValue(selectedBloon.BaseColor.r * 255);
                    GSlider.SetCurrentValue(selectedBloon.BaseColor.g * 255);
                    BSlider.SetCurrentValue(selectedBloon.BaseColor.b * 255);
                    break;
                case EditorPanel.Stats:
                    #region StatsPanel
                    var BaseStatsPanel = Settings.AddPanel(new Info("BaseStats", -617, 0, 566, 1400), VanillaSprites.MainBGPanelBlue);
                    BaseStatsPanel.AddText(new Info("Text", 0, 550, 550, 200), "Base Stats").GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;

                    BaseStatsPanel.AddText(new Info("HealthStatText", -75, 200, 350, 200), "Health:", 65, TextAlignmentOptions.MidlineLeft);
                    var healthInput = BaseStatsPanel.AddInputField(new Info("SetHealth", 150, 200, 200, 100), $"{selectedBloon.Health}", VanillaSprites.BlueInsertPanelRound, new Action<string>(value => { }), 75, TMP_InputField.CharacterValidation.Integer);
                    healthInput.InputField.onValueChanged.AddListener(new Action<string>(value =>
                    {
                        int.TryParse(value, out var intvalue);
                        selectedBloon.Health = intvalue;
                    }));
                    healthInput.Text.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
                    healthInput.InputField.characterLimit = 9;

                    BaseStatsPanel.AddText(new Info("SpeedStatText", -75, 50, 350, 200), "Speed:", 65, TextAlignmentOptions.MidlineLeft);
                    var speedInput = BaseStatsPanel.AddInputField(new Info("SetSpeed", 150, 50, 200, 100), $"{selectedBloon.Speed}", VanillaSprites.BlueInsertPanelRound, new Action<string>(value => { }), 75, TMP_InputField.CharacterValidation.Integer);
                    speedInput.InputField.onValueChanged.AddListener(new Action<string>(value =>
                    {
                        int.TryParse(value, out var intvalue);
                        selectedBloon.Speed = intvalue;
                    }));
                    speedInput.Text.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
                    speedInput.InputField.characterLimit = 9;

                    BaseStatsPanel.AddText(new Info("CashDropStatText", -75, -100, 350, 200), "Cash Drop:", 55, TextAlignmentOptions.MidlineLeft);
                    var cashdropInput = BaseStatsPanel.AddInputField(new Info("SetCashdrop", 150, -100, 200, 100), $"{selectedBloon.CashDrop}", VanillaSprites.BlueInsertPanelRound, new Action<string>(value => { }), 75, TMP_InputField.CharacterValidation.Integer);
                    cashdropInput.InputField.onValueChanged.AddListener(new Action<string>(value =>
                    {
                        int.TryParse(value, out var intvalue);
                        selectedBloon.CashDrop = intvalue;
                    }));
                    cashdropInput.Text.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
                    cashdropInput.InputField.characterLimit = 9;

                    BaseStatsPanel.AddText(new Info("SizeStatText", -75, -250, 350, 200), "Size:", 65, TextAlignmentOptions.MidlineLeft);
                    var sizeInput = BaseStatsPanel.AddInputField(new Info("SetSize", 150, -250, 200, 100), $"{selectedBloon.Size}", VanillaSprites.BlueInsertPanelRound, new Action<string>(value => { }), 75, TMP_InputField.CharacterValidation.Integer);
                    sizeInput.InputField.onValueChanged.AddListener(new Action<string>(value =>
                    {
                        int.TryParse(value, out var intvalue);
                        selectedBloon.Size = intvalue;
                    }));
                    sizeInput.Text.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
                    sizeInput.InputField.characterLimit = 9;

                    var BasePropertiesPanel = Settings.AddPanel(new Info("Properties", 0, 0, 566, 1400), VanillaSprites.MainBGPanelBlue);
                    BasePropertiesPanel.AddText(new Info("Text", 0, 550, 550, 200), "Properties").GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;

                    Stats.Button.interactable = false;
                    #endregion 
                    break;
            }
        }

        public void UpdateVisuals()
        {
            Bloonimage.Image.color = selectedBloon.BaseColor;
        }
    }
}
