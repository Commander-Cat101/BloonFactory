using BloonFactory.Handlers;
using BloonFactory.Modules.Core;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Helpers;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.Popups;
using Il2CppAssets.Scripts.Unity.UI_New.Settings;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using NfdSharp;
using System;
using System.Threading.Tasks;
using BloonFactory.Commands;
using Il2CppAssets.Scripts.Simulation.Bloons;
using Il2CppAssets.Scripts.Unity;
using Il2CppTMPro;
using TextCopy;
using UnityEngine;
using UnityEngine.UI;
using TaskScheduler = BTD_Mod_Helper.Api.TaskScheduler;

namespace BloonFactory.UI
{
    internal class BloonSelectorUI : ModGameMenu<SettingsScreen>
    {
        const int BloonNameCharacterLimit = 20;
        static Color UninteractableColor => new Color(0.6431f, 0.7255f, 0.8235f, 1);
        
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
        private ModHelperImage exportButtonIcon;
        private ModHelperButton uploadButton;
        private ModHelperButton deleteButton;
        private ModHelperButton reportButton;
        private ModHelperButton likeButton;
        private ModHelperImage likeButtonIcon;
        
        private bool awaitingLikeResponse;

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
            nameInput.InputField.characterLimit = BloonNameCharacterLimit;
            
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
            
            uploadButton = rightPanel.AddButton(new Info("UploadButton", 450, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.BackupBtn, new Action(() =>
                {
                    UploadPopup(selectedBloon);
                }));

            reportButton = rightPanel.AddButton(new Info("ReportButton", 450, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.RedBtn, new Action(() =>
                {
                    
                }));
            reportButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), GetSprite("ReportIcon"));
            
            likeButton = rightPanel.AddButton(new Info("LikeButton", -50, 150, 200, new Vector2(0.5f, 0)),
                VanillaSprites.BlueBtn, new Action(() =>
                {
                    
                }));
            likeButtonIcon = likeButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), VanillaSprites.ChallengeThumbsUpIcon);

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
            exportButtonIcon = exportButton.AddImage(new Info("Icon", 0, 5, 140, 140, new Vector2(0.5f, 0.5f)), VanillaSprites.ShareIosIcon);
            
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

            if (BloonFactory.DisableOnlineFeatures)
                openBrowser.Button.interactable = false;

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
                    tmpInputField.characterLimit = BloonNameCharacterLimit;
                    tmpInputField.characterValidation = TMP_InputField.CharacterValidation.None;
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
                        if (SerializationHandler.IsUUIDLoaded(template.GetUUID()))
                        {
                            PopupScreen.instance.SafelyQueue(screen => screen.ShowPopup(PopupScreen.Placement.menuCenter, "Failed Import.", "Bloon is already active.", null, "Ok", null, null, Popup.TransitionAnim.Scale));
                        }
                        SerializationHandler.LoadTemplate(template);
                        SerializationHandler.SaveTemplate(template);

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

            foreach (var template in SerializationHandler.Templates)
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

            if (AdminHandler.AdminModeEnabled)
            {
                panel.AddButton(new Info("CopyUUID", -10, 10, 75, 75, new Vector2(1f, 0f)), VanillaSprites.CopyIcon,
                    new Action(() =>
                    {
                        ClipboardService.SetText(template.GetUUID());
                    }));
            }
            
            return panel;
        }

        public void SelectTemplate(BloonTemplate? template, ModHelperText? nameLabel)
        {
            selectedBloon = template;

            editButton.Button.interactable = template != null;
            deleteButton.Button.interactable = template != null;
            uploadButton.Button.interactable = template != null;
            exportButton.Button.interactable = template != null;
            exportButtonIcon.Image.color = template != null ? Color.white : UninteractableColor;
            reportButton.Button.interactable = template != null;
            likeButton.Button.interactable = template != null;
            likeButtonIcon.Image.color = template == null ? Color.white : UninteractableColor;
            
            if (template == null)
            {
                nameInput.InputField.interactable = false;
                descriptionInput.InputField.interactable = false;
                
                nameInput.SetText("No Bloon Selected", false);
                descriptionInput.SetText("", false);
                dateText.SetText("N/A");
                moduleText.SetText($"N/A");
                authorText.SetText("N/A");

                uploadButton.SetActive(true);
                reportButton.SetActive(false);
                likeButton.SetActive(false);
                return;
            }
            uploadButton.SetActive(!template.IsDownloaded);
            reportButton.SetActive(template.IsDownloaded);
            likeButton.SetActive(template.IsDownloaded);

            if (BloonFactory.DisableOnlineFeatures)
                uploadButton.Button.interactable = false;
            
            if (template.IsDownloaded)
            {
                likeButtonIcon.Image.color = template.HasBeenLiked ? Color.green : Color.white;
            }
            
            nameInput.InputField.interactable = !template.IsDownloaded;
            nameInput.SetText(template.GetName(), false);
            nameInput.InputField.onEndEdit.RemoveAllListeners();
            nameInput.InputField.onEndEdit.AddListener(new Action<string>(value =>
            {
                template.Name = value;
                nameLabel?.SetText(value);
            }));
            
            descriptionInput.InputField.interactable = !template.IsDownloaded;
            descriptionInput.SetText(template.GetDescription(), false);
            descriptionInput.InputField.onEndEdit.RemoveAllListeners();
            descriptionInput.InputField.onEndEdit.AddListener(new Action<string>(value =>
            {
                template.Description = value; 
            }));
            
            reportButton.Button.onClick.RemoveAllListeners();
            reportButton.Button.onClick.AddListener(new Action(() =>
            {
                ReportPopup(template);
            }));
            
            likeButton.Button.onClick.RemoveAllListeners();
            likeButton.Button.onClick.AddListener(new Action(() =>
            {
                MenuManager.instance.buttonClickSound.Play("ClickSounds");
                if (awaitingLikeResponse)
                    return;
                awaitingLikeResponse = true;
                Task.Run(async () =>
                {
                    likeButtonIcon.Image.color = !template.HasBeenLiked ? Color.green : Color.white;
                    var result = await ServerHandler.LikeTemplate(template);
                    awaitingLikeResponse = false;
                    if (result.errorCode != string.Empty)
                    {
                        PopupScreen.instance.SafelyQueue(screen =>
                        {
                            screen.ShowErrorPopup(result.errorCode, true);
                        });
                        likeButtonIcon.Image.color = result.isLiked ? Color.green : Color.white;
                    }
                    else
                    {
                        if (template.uploadData != null)
                        {
                            template.HasBeenLiked = result.isLiked;
                            if (template == selectedBloon)
                                likeButtonIcon.Image.color = result.isLiked ? Color.green : Color.white;
                            SerializationHandler.SaveTemplate(template);
                        }
                    }
                });
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
                screen.ShowPopup(PopupScreen.Placement.menuCenter, "Upload Bloon to Server", "Upload your bloon to the bloon browser.\nYour bloon will be unverified unless verified by the developer.\nPrivacy Notice: Public account details such as your account ID and display name will be saved and displayed.\nUploading inappropriate content will result in a permanent ban.", new Action(() =>
                {
                    PopupScreen.instance.SafelyQueue(screen =>
                    {
                        ModHelperDropdown dropdown = null;
                        BloonCategory category = BloonCategory.None;
                        
                        screen.ShowPopup(PopupScreen.Placement.menuCenter, "Bloon Info", "Select your bloons category.", new Action(() =>
                        {
                            UploadTemplate(template, category);
                        }), "Upload", null, "Cancel", Popup.TransitionAnim.Scale);
                        TaskScheduler.ScheduleTask(() =>
                        {
                            var popup = screen.GetFirstActivePopup();
                            var panel = popup.bodyObj.AddModHelperPanel(new Info("BloonsPanel", InfoPreset.FillParent));
                            popup.SetOkButtonInteractible(false);

                            dropdown = panel.AddDropdown(new Info("Filter", 421.5F * 1.5f, 100F * 1.5f, new Vector2(.7f, 0f)), CategoryExtensions.BloonCategoryNames.ToIl2CppList(), 600, new Action<int>(value =>
                            {
                                category = (BloonCategory)value;
                                popup.SetOkButtonInteractible(category != BloonCategory.None);
                            }) , VanillaSprites.BlueInsertPanelRound, 70);
                            panel.AddText(new Info("CategoryText", 700, 100, new Vector2(.3f, 0f)), "Category:", 100);
                            TaskScheduler.ScheduleTask(() =>
                            {
                                screen.GetFirstActivePopup().bodyObj.transform.localPosition = new Vector3(0, 50, 0);
                            });
                        }, () => screen.GetFirstActivePopup()?.bodyObj is not null);
                    });
                }
                ), "Ok", null, "Cancel", Popup.TransitionAnim.Scale);
                TaskScheduler.ScheduleTask(() =>
                {
                    var rect = screen.GetFirstActivePopup().bodyObj.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + 200);
                }, () => screen.GetFirstActivePopup()?.bodyObj is not null);
            });
        }
        public void UploadTemplate(BloonTemplate template, BloonCategory category)
        {
            try
            {
                Task.Run(async () =>
                {
                    var response = await ServerHandler.UploadTemplate(template, category);
                            
                    PopupScreen.instance.SafelyQueue(screen =>
                    {
                        if (response.success)
                        {
                            screen.ShowOkPopup("Your bloon has been successfully uploaded!");
                            SelectTemplate(selectedBloon, null);
                        }
                        else
                        {
                            screen.ShowErrorPopup($"Bloon Failed to Upload.\n{response.errorCode}", true);
                        }
                            
                    });
                });
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
        public void ReportTemplate(BloonTemplate template, ReportFlags flags)
        {
            try
            {
                Task.Run(async () =>
                {
                    var response = await ServerHandler.ReportTemplate(flags, template);
                            
                    PopupScreen.instance.SafelyQueue(screen =>
                    {
                        if (response.success)
                        {
                            screen.ShowOkPopup("Your report has been successfully sent!");
                        }
                        else
                        {
                            screen.ShowErrorPopup($"Report failed to send.\n{response.errorCode}", true);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
        public void ReportPopup(BloonTemplate template)
        {
            PopupScreen.instance.SafelyQueue(screen =>
            {
                screen.ShowPopup(PopupScreen.Placement.menuCenter, "Report Bloon", "Check the option which apply most to this bloon", null, "Confirm", null, "Cancel",  Popup.TransitionAnim.Scale);
                TaskScheduler.ScheduleTask(() =>
                {
                    var popup = screen.GetFirstActivePopup();
                    popup.bodyObj.GetComponent<RectTransform>().sizeDelta =
                        popup.bodyObj.GetComponent<RectTransform>().sizeDelta.WithY(200);
                    
                    var panel = popup.bodyObj.transform.parent.gameObject.AddModHelperPanel(new Info("BloonsPanel", InfoPreset.FillParent));
                    panel.FitContent(ContentSizeFitter.FitMode.Unconstrained, ContentSizeFitter.FitMode.PreferredSize);
                    panel.transform.SetSiblingIndex(popup.bodyObj.transform.GetSiblingIndex() + 1);
                    var layout = panel.AddComponent<VerticalLayoutGroup>();
                    layout.childForceExpandHeight = false;
                    layout.childForceExpandWidth = false;
                    layout.childControlHeight = false;
                    layout.childControlWidth = false;
                    layout.childAlignment = TextAnchor.MiddleCenter;
                    layout.padding = new RectOffset { bottom = 100, left = 0, right = 0, top = 25 };
                    layout.spacing = 25;
                    
                    var option1 = panel.AddPanel(new Info("ReportOption1", 0, 0, 1200, 125), VanillaSprites.SmallSquareLightInner);
                    option1.AddText(new Info("Text", 550, 0, 1000, 100, new Vector2(0, 0.5f)), "Offensive Name or Description", 60);
                    var offensiveText = option1.AddCheckbox(new Info("Checkbox", -75, 0, 90, new Vector2(1, 0.5f)), false, VanillaSprites.SmallSquareDarkInner);
                    var option2 = panel.AddPanel(new Info("ReportOption2", 0, 0, 1200, 125), VanillaSprites.SmallSquareLightInner);
                    option2.AddText(new Info("Text", 550, 0, 1000, 100, new Vector2(0, 0.5f)), "Offensive Imagery/Sounds", 60);
                    var offensiveAssets = option2.AddCheckbox(new Info("Checkbox", -75, 0, 90, new Vector2(1, 0.5f)), false, VanillaSprites.SmallSquareDarkInner);
                    var option3 = panel.AddPanel(new Info("ReportOption3", 0, 0, 1200, 125), VanillaSprites.SmallSquareLightInner);
                    option3.AddText(new Info("Text", 550, 0, 1000, 100, new Vector2(0, 0.5f)), "Spam", 60);
                    var spam = option3.AddCheckbox(new Info("Checkbox", -75, 0, 90, new Vector2(1, 0.5f)), false, VanillaSprites.SmallSquareDarkInner);
                    var option4 = panel.AddPanel(new Info("ReportOption4", 0, 0, 1200, 125), VanillaSprites.SmallSquareLightInner);
                    option4.AddText(new Info("Text", 550, 0, 1000, 100, new Vector2(0, 0.5f)), "Other", 60);
                    var other = option4.AddCheckbox(new Info("Checkbox", -75, 0, 90, new Vector2(1, 0.5f)), false, VanillaSprites.SmallSquareDarkInner);

                    popup.confirmCallback = new Action(() =>
                    {
                        ReportFlags flags = ReportFlags.None;
                        if (offensiveText.CurrentValue) flags |= ReportFlags.OffensiveText;
                        if (offensiveAssets.CurrentValue) flags |= ReportFlags.OffensiveAssets;
                        if (spam.CurrentValue) flags |= ReportFlags.Spam;
                        if (other.CurrentValue) flags |= ReportFlags.Other;
                        
                        PopupScreen.instance.SafelyQueue(screen =>
                        {
                            screen.ShowPopup(PopupScreen.Placement.menuCenter, "Confirm Report", "Your account ID will be attached to this report.\nFalse reports will result in a full ban from the bloon browser.", new Action(() =>
                            {
                                ReportTemplate(template, flags);
                            }),  "Confirm",  null, "Cancel", Popup.TransitionAnim.Scale);
                        });
                    });
                }, () => screen.GetFirstActivePopup()?.bodyObj is not null);
            });
        }
    }
}
