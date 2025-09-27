using BloonFactory.Modules.Core;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.RightMenu.Powers;
using Il2CppAssets.Scripts.Unity.UI_New.Main.PowersSelect;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNewtonsoft.Json;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using NfdSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TaskScheduler = BTD_Mod_Helper.Api.TaskScheduler;

namespace BloonFactory.UI
{
    internal class EditSelectorUI : ModGameMenu<SettingsScreen>
    {
        public ModHelperScrollPanel scrollPanel;
        public Animator bottomGroupAnimator;
        public Animator mainPanelAnimator;
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            GameMenu.transform.DestroyAllChildren();
            CommonForegroundHeader.SetText("Bloon Editor");
            var panel = GameMenu.gameObject.AddModHelperPanel(new Info("Root", InfoPreset.FillParent));

            CreateMainContent(panel);
            CreateExtraContent(panel);
            return false;
        }
        public override void OnMenuClosed()
        {
            bottomGroupAnimator.Play("PopupSlideOut");
            mainPanelAnimator.Play("PopupScaleOut");
        }
        public void CreateMainContent(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("MainPanel", 0, 50, 3500, 1700), VanillaSprites.MainBGPanelBlue);
            scrollPanel = panel.AddScrollPanel(new Info("ScrollPanel", 0, 0, 3400, 1600), UnityEngine.RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 50, 50);

            mainPanelAnimator = panel.AddComponent<Animator>();
            mainPanelAnimator.runtimeAnimatorController = Animations.PopupAnim;
            mainPanelAnimator.speed = .55f;
            mainPanelAnimator.Play("PopupScaleIn");
            AddContent();
        }
        public void CreateExtraContent(ModHelperPanel root)
        {
            var panel = root.AddPanel(new Info("Panel", InfoPreset.FillParent));

            var openBrowser = panel.AddButton(new Info("BloonDownloader", 300, 300, 400, 400, new Vector2(0, 0)), VanillaSprites.AlternateBloonsBtn, new Action(() => Open<BloonBrowserUI>()));
            openBrowser.AddText(new Info("Text", 0, -150, 400, 200), "Bloon\nBrowser").EnableAutoSizing();

            var newBloon = panel.AddButton(new Info("CreateNewBloon", 450, 250, 800, 300, new Vector2(0.5f, 0)), VanillaSprites.GreenBtnLong, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
                PopupScreen.instance.SafelyQueue(screen => screen.ShowSetNamePopup("Create Bloon", "Name of bloon to create.\n", new Action<string>(name =>
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        SerializationHandler.CreateTemplate(name);
                        AddContent();
                    }
                }), null));
                PopupScreen.instance.SafelyQueue(screen => screen.ModifyField(tmpInputField =>
                {
                    tmpInputField.textComponent.font = Fonts.Btd6FontBody;
                    tmpInputField.characterLimit = 20;
                }));
            }));
            newBloon.AddText(new Info("Text", 0, 0, 700, 250), "Create", 120);

            var importBloon = panel.AddButton(new Info("ImportBloon", -450, 250, 800, 300, new Vector2(0.5f, 0)), VanillaSprites.GreenBtnLong, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");

                FileDialogHelper.PrepareNativeDlls();
                
                if (Nfd.OpenDialog("cstmbln", "", out string path) == Nfd.NfdResult.NFD_OKAY)
                {
                    try
                    {
                        var template = SerializationHandler.GetTemplateFromPath(path);
                        if (SerializationHandler.ContainGuid(template.Guid))
                        {
                            PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Failed Import.", "Bloon is already active.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                        }
                        SerializationHandler.LoadTemplate(template);
                        SerializationHandler.SaveTemplate(template);
                        template.IsLoaded = false;

                        AddContent();
                    }
                    catch 
                    {
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Failed Import.", "Failed to import bloon.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                    }
                }
            }));
            importBloon.AddText(new Info("Text", 0, 0, 700, 250), "Import", 120);

            bottomGroupAnimator = panel.AddComponent<Animator>();
            bottomGroupAnimator.runtimeAnimatorController = Animations.PopupAnim;
            bottomGroupAnimator.speed = .55f;
            bottomGroupAnimator.Play("PopupSlideIn");
        }
        public void AddContent()
        {
            scrollPanel.ScrollContent.transform.DestroyAllChildren();

            foreach (var template in SerializationHandler.Templates.Where(a => !a.IsQueueForDeletion))
            {
                scrollPanel.AddScrollContent(CreateContentPanel(template));
            }
        }
        public ModHelperPanel CreateContentPanel(BloonTemplate template)
        {
            var panel = ModHelperPanel.Create(new Info("Template", 3300, 300), VanillaSprites.MainBGPanelBlue);

            panel.AddText(new Info("Name", 850, 0, 1500, 150, new Vector2(0, 0.5f)), template.Name, 100, Il2CppTMPro.TextAlignmentOptions.Left).EnableAutoSizing(150, 10);

            panel.AddButton(new Info("Edit", 1500, 0, 200, 200), VanillaSprites.EditBtn, new Action(() =>
            {
                OpenEditorWithTemplate(template);
            }));

            panel.AddButton(new Info("Delete", 1250, 0, 200, 200), VanillaSprites.CloseBtn, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
                SerializationHandler.DeleteTemplate(template); 
                AddContent();
            }));

            var exportButton = panel.AddButton(new Info("Export", 1000, 0, 200, 200), VanillaSprites.BlueBtn, new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
                FileDialogHelper.PrepareNativeDlls();
                if (Nfd.SaveDialog("cstmbln", "", out string path) == Nfd.NfdResult.NFD_OKAY)
                {
                    try
                    {
                        SerializationHandler.SaveTemplate(template, path);
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Exported Successfully.", $"Bloon has been successfully exported to {path}", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Error(ex);
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Failed Export.", "Failed to export bloon.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                    }
                }
            }));
            exportButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), VanillaSprites.ShareIosIcon);

            panel.AddButton(new Info("Upload", 750, 0, 200, 200), VanillaSprites.BackupBtn, new Action(() =>
            {
                UploadPopup(template);
            }));

            if (!template.IsLoaded)
                panel.AddButton(new Info("NotLoaded", -20, -20, 100, 100, new Vector2(1, 1)), VanillaSprites.NoticeBtn,
                    new Action(() =>
                    {
                        PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "This Bloon is unloaded", "This bloon is unloaded and wont show up ingame, restart the game to load this bloon.", null, "Ok", new Action(() => { ProcessHelper.RestartGame(); }), "Restart Game", Popup.TransitionAnim.Scale));
                    })
                );
            return panel;
        }
        public void OpenEditorWithTemplate(BloonTemplate template)
        {
            BloonEditorUI.Template = template;
            ModGameMenu.Open<BloonEditorUI>();
        }
        public void UploadPopup(BloonTemplate template)
        {
            PopupScreen.instance.SafelyQueue(screen =>
            {
                screen.ShowPopup(PopupScreen.Placement.menuCenter, "Upload Bloon to Server", "Request your bloon to be added to the bloon browser. \nNot all bloons will be added to the browser. \nThis will use your NK account name as the creator name.\nUploading inappropriate content will result in a ban.", new Action(() =>
                {
                    PopupScreen.instance.SafelyQueue(screen =>
                    {
                        ModHelperDropdown dropdown = null;
                        ModHelperInputField description = null;
                        screen.ShowPopup(PopupScreen.Placement.menuCenter, "Bloon Info", "", new Action(async () =>
                        {
                            UploadBloon(template, (BloonCategory)dropdown.Dropdown.value, description.InputField.text);
                        }), "Upload", null, "Cancel", Popup.TransitionAnim.Scale);
                        TaskScheduler.ScheduleTask(() =>
                        {
                            var panel = screen.GetFirstActivePopup().bodyObj.AddModHelperPanel(new Info("BloonsPanel", InfoPreset.FillParent));

                            dropdown = panel.AddDropdown(new Info("Filter", 421.5F * 1.5f, 100F * 1.5f, new Vector2(.7f, 0f)), CategoryExtensions.BloonCategoryNames.ToIl2CppList(), 600, null, VanillaSprites.BlueInsertPanelRound, 70);
                            panel.AddText(new Info("CategoryText", 700, 100, new Vector2(.3f, 0f)), "Category:", 100);

                            description = panel.AddInputField(new Info("Description", 1800, 150, new Vector2(0.5f, 0.5f)), "Enter Description.", VanillaSprites.BlueInsertPanel);
                            description.InputField.characterLimit = 100;
                            description.InputField.GetComponent<Mask>().enabled = false;
                            description.InputField.GetComponent<Mask>().enabled = true;
                            TaskScheduler.ScheduleTask(() =>
                            {
                                screen.GetFirstActivePopup().bodyObj.transform.localPosition = new Vector3(0, 50, 0);
                            });
                        }, () => screen.GetFirstActivePopup()?.bodyObj is not null);
                    });
                }
                ), "Ok", null, "Cancel", Popup.TransitionAnim.Scale);
            });
        }
        public void UploadBloon(BloonTemplate template, BloonCategory category, string description)
        {
            Task.Run(async () =>
            {
                (bool success, string errorCode) result = await ServerHandler.UploadTemplate(template, category, description);

                if (result.success)
                    PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Upload Successful", "Successfully sent upload request to server.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                else
                    PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Upload Failed", $"Failed to send upload request to server.\n{result.errorCode}", null, "Ok", null, null, Popup.TransitionAnim.Scale));
            });
        }
    }
}
