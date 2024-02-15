using BloonFactory.API.Bloons;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.SocialSharing;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
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
    internal class CustomBloonSelector : ModGameMenu<ExtraSettingsScreen>
    {
        public static ModHelperScrollPanel scrollPanel;

        public override void OnMenuClosed()
        {
            
        }
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            CommonForegroundHeader.SetText("Bloon Editor");

            var panelTransform = GameMenu.gameObject.GetComponentInChildrenByName<RectTransform>("Panel");
            var panel = panelTransform.gameObject;
            panel.DestroyAllChildren();

            var BloonMenu = panel.AddModHelperPanel(new Info("BloonMenu", 3600, 1900));

            CreateMainPanel(BloonMenu);
            CreateExtraButtons(BloonMenu);

            return false;
        }
        public void CreateMainPanel(ModHelperPanel menu)
        {
            var scrollpaneloutline = menu.AddPanel(new Info("ScrollPanelOutline", 0, 200, 3200, 1800), VanillaSprites.MainBGPanelBlue);
            scrollPanel = scrollpaneloutline.AddScrollPanel(new Info("ScrollPanel", 0, 0, 3100, 1700), RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 50, 50);
            LoadBloons();
        }
        public void LoadBloons()
        { 
            scrollPanel.ScrollContent.transform.DestroyAllChildren();
            foreach (var bloon in CustomBloon.RegisteredBloons)
            {
                scrollPanel.AddScrollContent(GenerateScrollContent(bloon));
            }
        }
        public ModHelperPanel GenerateScrollContent(CustomBloon bloon)
        {
            var panel = ModHelperPanel.Create(new Info(bloon.Name, 0, 0, 3000, 250), VanillaSprites.MainBGPanelBlue);
            var text = panel.AddText(new Info("NameText", -1200, 0, 500, 200), bloon.Name);
            text.transform.GetComponent<NK_TextMeshProUGUI>().enableAutoSizing = true;
            panel.AddButton(new Info("EditButton", 1350, 0, 200, 200), VanillaSprites.EditBtn, new System.Action(() =>
            {
                BloonEditor.selectedBloon = bloon;
                ModGameMenu.Open<BloonEditor>();
            }));
            var export = panel.AddButton(new Info("ExportButton", 1100, 0, 200, 200), VanillaSprites.BlueBtn, new System.Action(() =>
            {
                
            }));
            var exit = export.AddImage(new Info("Exit", 130), VanillaSprites.ExitIcon);
            exit.RectTransform.Rotate(0, 0, -90);
            return panel;
        }
        public void CreateExtraButtons(ModHelperPanel menu)
        {
            menu.AddButton(new Info("CreateNewBloon", 500, -900, 800, 300), VanillaSprites.GreenBtnLong, new System.Action(() =>
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowSetNamePopup("Create Bloon", "Name of bloon to create.\n", new Action<string>(name =>
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        CreateBloon(name);
                    }
                }), null));
                PopupScreen.instance.SafelyQueue(screen => screen.ModifyField(tmpInputField =>
                {
                    tmpInputField.textComponent.font = Fonts.Btd6FontBody;
                    tmpInputField.characterLimit = 20;
                    tmpInputField.characterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
                }));
            }))
            .AddText(new Info("Text", 0, 0, 700, 250), "Create", 100);

            menu.AddButton(new Info("ImportNewBloon", -500, -900, 800, 300), VanillaSprites.GreenBtnLong, new System.Action(() =>
            {
                FileDialogHelper.PrepareNativeDlls();

                if (Nfd.OpenDialog("", "", out string path) == Nfd.NfdResult.NFD_OKAY)
                {
                    var bloon = JsonConvert.DeserializeObject<CustomBloon>(File.ReadAllText(path));

                    if(CustomBloon.RegisteredBloons.Any(a => a.Name == bloon.Name))
                    {
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.inGameCenter, "Import Failed!", "Bloon already exists.", null, "Continue", null, null, Popup.TransitionAnim.Scale));
                        return;
                    }

                    if (bloon != null)
                    {
                        CustomBloon.RegisteredBloons.Add(bloon);
                    }
                    LoadBloons();

                    var content = JsonConvert.SerializeObject(bloon);
                    File.WriteAllText(Path.Combine(BloonFactory.FolderDirectory, $"{bloon.Name}.json"), content);
                    PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.inGameCenter, "Import Success!", $"Successfully Imported {bloon.Name}!", null, "Continue", null, null, Popup.TransitionAnim.Scale));
                }
            }))
            .AddText(new Info("Text", 0, 0, 700, 250), "Import", 100);
        }
        public void CreateBloon(string Name)
        {
            var bloon = new CustomBloon(Name);
            CustomBloon.RegisteredBloons.Add(new CustomBloon(bloon));
            LoadBloons();

            var content = JsonConvert.SerializeObject((CustomBloonSerializable)bloon, Formatting.None);
            File.WriteAllText(Path.Combine(BloonFactory.FolderDirectory, $"{Name}.json"), content);
        }
    }
}
