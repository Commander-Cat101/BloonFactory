using BloonFactory.Handlers;
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
using System.Linq;
using System.Threading.Tasks;
using Il2CppAssets.Scripts.Unity;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;
using TaskScheduler = BTD_Mod_Helper.Api.TaskScheduler;

namespace BloonFactory.UI
{
    internal class BloonSelectorUI : ModGameMenu<SettingsScreen>
    {
        ModHelperScrollPanel scrollPanel;
        Animator bottomGroupAnimator;
        Animator mainPanelAnimator;

        private ModHelperInputField descriptionInput;
        private ModHelperInputField nameInput;
    
        private ModHelperText moduleText;
        private ModHelperText versionText;
        private ModHelperText dateText;
        private ModHelperText authorText;

        private BloonTemplate selectedBloon;
        private ModHelperButton editButton;
        private ModHelperButton exportButton;
        private ModHelperButton uploadButton;
        private ModHelperButton deleteButton;
        private ModHelperButton reportButton;

        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            GameMenu.transform.DestroyAllChildren();
            CommonForegroundHeader.SetText("Bloon Editor");
            var panel = GameMenu.gameObject.AddModHelperPanel(new Info("Root", InfoPreset.FillParent));

            CreateMainContent(panel);
            CreateExtraContent(panel);
            
            SelectTemplate(null, null);
            
            return false;
        }
        public override void OnMenuClosed()
        {
            bottomGroupAnimator.Play("PopupSlideOut");
            mainPanelAnimator.Play("PopupScaleOut");
        }

        public void CreateMainContent(ModHelperPanel root)
        {
            var mainPanel = root.AddPanel(new Info("MainPanel", InfoPreset.FillParent));
            
            var panel = mainPanel.AddPanel(new Info("LeftPanel", -900, 50, 1700, 1700), VanillaSprites.MainBGPanelBlue);
            scrollPanel = panel.AddScrollPanel(new Info("ScrollPanel", 0, 0, 1600, 1600), UnityEngine.RectTransform.Axis.Vertical, VanillaSprites.BlueInsertPanelRound, 50, 50);

            var rightPanel = mainPanel.AddPanel(new Info("RightPanel", 900, 50, 1700, 1700), VanillaSprites.MainBGPanelBlue);

            var nameInsert = rightPanel.AddPanel(new Info("NameInsert", 0, -125, 1600, 150, new Vector2(0.5f, 1)), VanillaSprites.BlueInsertPanelRound);
            nameInput = nameInsert.AddInputField(new Info("Name", 1500, 100), "No Bloon Selected", null, null, 80, TMP_InputField.CharacterValidation.None, TextAlignmentOptions.Left);
            nameInput.InputField.textComponent.font = Fonts.Btd6FontTitle;
            
            var authorInsert = rightPanel.AddPanel(new Info("AuthorInsert", 0, -285, 1600, 100, new Vector2(0.5f, 1)), VanillaSprites.BlueInsertPanelRound);
            authorText = authorInsert.AddText(new Info("Author", 1500, 100), "N/A", 60, TextAlignmentOptions.Left);
            authorText.Text.color = Color.gray8;
            
            var moduleInsert = rightPanel.AddPanel(new Info("ModuleInsert", -550, -420, 500, 100, new Vector2(0.5f, 1)), VanillaSprites.BlueInsertPanelRound);
            moduleText = moduleInsert.AddText(new Info("Module", 400, 100), "N/A", 50);
            
            var dateInsert = rightPanel.AddPanel(new Info("DateInsert", 100, -420, 700, 100, new Vector2(0.5f, 1)), VanillaSprites.BlueInsertPanelRound);
            dateText = dateInsert.AddText(new Info("Date", 600, 50), "N/A", 50);
            
            var versionInsert = rightPanel.AddPanel(new Info("VersionInsert", 650, -420, 300, 100, new Vector2(0.5f, 1)), VanillaSprites.BlueInsertPanelRound);
            versionText = versionInsert.AddText(new Info("Version", 200, 100), "N/A", 50);
            
            var descriptionInsert = rightPanel.AddPanel(new Info("Description", 0, 750, 1600, 900, new Vector2(0.5f, 0)), VanillaSprites.BlueInsertPanelRound);
            descriptionInput = descriptionInsert.AddInputField(new Info("Description", 0, 0, 1500, 800), "", null, new Action<string>(value =>
            {
                
            }), 50, TMP_InputField.CharacterValidation.None, TextAlignmentOptions.TopLeft);
            descriptionInput.InputField.textComponent.font = Fonts.Btd6FontTitle;
            descriptionInput.InputField.textComponent.overflowMode = TextOverflowModes.Truncate;
            descriptionInput.InputField.isRichTextEditingAllowed = true;
            descriptionInput.InputField.richText = true;

            editButton = rightPanel.AddButton(new Info("EditButton", -550, 150, 500, 200, new Vector2(0.5f, 0)),
                VanillaSprites.GreenBtnLong, new Action(() =>
                {
                    if (selectedBloon == null)
                        return;
                    BloonEditorUI.Template = selectedBloon;
                    Open<BloonEditorUI>();
                }));
            editButton.AddText(new Info("Text", 0, 10, 400, 150), "Edit", 80, TextAlignmentOptions.Center);

            deleteButton = rightPanel.AddButton(new Info("DeleteButton", 700, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.CloseBtn, new Action(() =>
                {
                    MenuManager.instance.buttonClickSound.Play("ClickSounds");
                    if (selectedBloon == null)
                        return;
                    PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Delete Bloon", "Are you sure you want to delete this Bloon?", new Action(() =>
                        {
                            MenuManager.instance.buttonClickSound.Play("ClickSounds");
                            SerializationHandler.DeleteTemplate(selectedBloon); 
                            AddContent();
                            SelectTemplate(null, null);
                        }), "Yes",
                        new Action(() =>
                        {
                            
                        }), "Return", Popup.TransitionAnim.Scale));
                }));
            
            uploadButton = rightPanel.AddButton(new Info("UploadButton", 450, 150, 0, new Vector2(0.5f, 0)),
                VanillaSprites.BackupBtn, new Action(() =>
                {
                    
                }));

            reportButton = rightPanel.AddButton(new Info("ReportButton", 450, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.RedBtn, new Action(() =>
                {
                    
                }));
            reportButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), GetSprite("ReportIcon"));

            exportButton = rightPanel.AddButton(new Info("ExportButton", 200, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.BlueBtn,
                new Action(() =>
                {
                    MenuManager.instance.buttonClickSound.Play("ClickSounds");
                    FileDialogHelper.PrepareNativeDlls();
                    if (Nfd.SaveDialog("cstmbln", "", out string path) == Nfd.NfdResult.NFD_OKAY)
                    {
                        try
                        {
                            SerializationHandler.SaveTemplate(selectedBloon, path);
                            PopupScreen.instance.SafelyQueue(screen =>
                                screen.ShowPopup(PopupScreen.Placement.menuCenter, "Exported Successfully.",
                                    $"Bloon has been successfully exported to {path}", null, "Ok", null, null,
                                    Popup.TransitionAnim.Scale));
                        }
                        catch (Exception ex)
                        {
                            MelonLogger.Error(ex);
                            PopupScreen.instance.SafelyQueue(screen =>
                                screen.ShowPopup(PopupScreen.Placement.menuCenter, "Failed Export.",
                                    "Failed to export bloon.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                        }
                    }
                }));
            exportButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), VanillaSprites.ShareIosIcon);
            
            mainPanelAnimator = mainPanel.AddComponent<Animator>();
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
        public ModHelperButton CreateContentPanel(BloonTemplate template)
        {
            var panel = ModHelperButton.Create(new Info("Template", 1500, 200), VanillaSprites.MainBGPanelBlue, null);

            var text = panel.AddText(new Info("Name", 0, 0, 1000, 100, new Vector2(0.5f, 0.5f)), template.Name, 80,
                Il2CppTMPro.TextAlignmentOptions.Center);
            text.EnableAutoSizing(80, 10);
            
            panel.Button.onClick.AddListener(() =>
            {
                SelectTemplate(template, text);
            });
            
            return panel;
        }

        public void SelectTemplate(BloonTemplate? template, ModHelperText? nameLabel)
        {
            selectedBloon = template;

            editButton.Button.interactable = template != null;
            deleteButton.Button.interactable = template != null;
            uploadButton.Button.interactable = template != null;
            exportButton.Button.interactable = template != null;
            reportButton.Button.interactable = template != null;
            
            if (template == null)
            {
                nameInput.SetText("No Bloon Selected", false);
                descriptionInput.SetText("", false);
                dateText.SetText("N/A");
                moduleText.SetText($"N/A");
                authorText.SetText("N/A");

                uploadButton.SetActive(true);
                reportButton.SetActive(false);
                return;
            }
            uploadButton.SetActive(!template.IsDownloaded);
            reportButton.SetActive(template.IsDownloaded);
            
            nameInput.SetText(template.GetName(), false);
            nameInput.InputField.onEndEdit.RemoveAllListeners();
            nameInput.InputField.onEndEdit.AddListener(new Action<string>(value =>
            {
                template.Name = value;
                nameLabel?.SetText(value);
            }));
            
            descriptionInput.SetText(template.GetDescription(), false);
            descriptionInput.InputField.onEndEdit.RemoveAllListeners();
            descriptionInput.InputField.onEndEdit.AddListener(new Action<string>(value =>
            {
                template.Description = value; 
            }));
            
            dateText.SetText(template.IsDownloaded ? $"Uploaded {template.uploadData?.UploadTime.ToShortDateString()}" : $"Created {template.CreationTime.ToShortDateString()}");
            moduleText.SetText($"{template.modules.Count} Modules");
            authorText.SetText(template.IsDownloaded ? template.uploadData?.Author : Game.Player.LiNKAccount.DisplayName);
            versionText.SetText($"v{(template.IsDownloaded ? template.uploadData?.ModVersion : ModHelperData.Version)}");
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

                            description = panel.AddInputField(new Info("Description", 1800, 150, new Vector2(0.5f, 0.5f)), string.Empty, VanillaSprites.BlueInsertPanel, null, 70, Il2CppTMPro.TMP_InputField.CharacterValidation.None, Il2CppTMPro.TextAlignmentOptions.Center, "Enter Description.");
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
            if (string.IsNullOrEmpty(description))
            {
                PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Upload Failed", $"Failed to send upload request to server.\nDescription must not be empty.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                return;
            }
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
