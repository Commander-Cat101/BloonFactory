using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Components;
using BTD_Mod_Helper.Api.Enums;
using BTD_Mod_Helper.Api.Legends;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Menu;
using Il2CppAssets.Scripts.Unity.UI_New.ChallengeEditor;
using Il2CppNinjaKiwi.Common;
using MelonLoader;
using Newtonsoft.Json;
using Octokit.Internal;
using Semver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static BloonFactory.UI.BloonBrowserMenuPanel;
using static Il2CppTMPro.TMP_InputField;
using TaskScheduler = BTD_Mod_Helper.Api.TaskScheduler;

namespace BloonFactory.UI
{
    /// <summary>
    /// Some of this code is adapted from BTD6 Mod Helper's Content Browser
    /// Thanks doombubbles!
    /// </summary>
    internal class BloonBrowserUI : ModGameMenu<ContentBrowser>
    {
        public bool generatedContentReady = false;

        public BloonBrowserMenuPanel[] bloonPanels;

        public List<BloonBrowserEntry> allEntries = new List<BloonBrowserEntry>();

        public List<BloonBrowserEntry> filteredEntries = new List<BloonBrowserEntry>();

        public static int bloonsPerPage = 20;
        public int TotalPages => 1 + ((filteredEntries?.Count ?? 1) - 1) / bloonsPerPage;
        public int currentPage = 0;

        ModHelperInputField searchField;

        ModHelperDropdown filterDropdown;

        SortingMethod SortingMethod = SortingMethod.Popular;
        public override bool OnMenuOpened(Il2CppSystem.Object data)
        {
            bloonPanels = new BloonBrowserMenuPanel[bloonsPerPage];

            BloonBrowserMenuPanel template = null;

            for (int i = 0; i < bloonPanels.Length; i++)
            {
                if (template != null)
                    bloonPanels[i] = template.Duplicate($"{i}");
                else
                    template = bloonPanels[i] = GameMenu.scrollRect.content.gameObject.AddModHelperComponent(BloonBrowserMenuPanel.CreateTemplate());
                bloonPanels[i].AddTo(GameMenu.scrollRect.content);
            }
            TaskScheduler.ScheduleTask(() =>
            {
                foreach (var panel in bloonPanels)
                {
                    panel.gameObject.SetActive(false);
                }
            });
            

            ModifyElements();
            AddElements();

            RefreshPage();
            return false;
        }
        public override void OnMenuUpdate()
        {
            if (generatedContentReady)
            {
                generatedContentReady = false;
                SetPage(0);

                GameMenu.searchingImg.gameObject.SetActive(false);
                GameMenu.refreshBtn.interactable = true;
                GenerateContentForPage();
            }
        }
        public void ModifyElements()
        {
            GameMenu.GetComponentFromChildrenByName<RectTransform>("TopBar").gameObject.active = false;
            GameMenu.GetComponentFromChildrenByName<RectTransform>("Tabs").gameObject.active = false;

            var verticalLayoutGroup = GameMenu.scrollRect.content.GetComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.SetPadding(50);
            verticalLayoutGroup.spacing = 50;
            verticalLayoutGroup.childControlWidth = false;
            verticalLayoutGroup.childControlHeight = false;
            GameMenu.scrollRect.rectTransform.sizeDelta += new Vector2(0, 200);
            GameMenu.scrollRect.rectTransform.localPosition += new Vector3(0, 100, 0);

            GameMenu.refreshBtn.SetOnClick(() => { 
                RefreshPage();
                MenuManager.instance.buttonClick3Sound.Play("ClickSounds");
            });
            GameMenu.firstPageBtn.SetOnClick(() => { 
                SetPage(0);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            });
            GameMenu.previousPageBtn.SetOnClick(() =>{ 
                SetPage(currentPage - 1);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            });
            GameMenu.nextPageBtn.SetOnClick(() => {
                SetPage(currentPage + 1);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            });
            GameMenu.lastPageBtn.SetOnClick(() => {
                SetPage(TotalPages - 1);
                MenuManager.instance.buttonClick2Sound.Play("ClickSounds");
            });
        }
        public void AddElements()
        {
            var container = GameMenu.GetComponentFromChildrenByName<RectTransform>("Container").gameObject.AddModHelperPanel(new Info("SearchBar", 0, -475, new Vector2(0.5f, 1)));

            container.AddDropdown(new Info("Ordering", 1400, -425, 900, 150), Enum.GetNames(typeof(SortingMethod)).ToIl2CppList(), 450, new Action<int>((val) =>
            {
                SortingMethod = (SortingMethod)val;
                GenerateContentForPage();
            }), VanillaSprites.BlueInsertPanelRound, 70);

            filterDropdown = container.AddDropdown(new Info("Filter", -1400, -425, 900, 150), CategoryExtensions.BloonCategoryNames.Prepend("All").ToIl2CppList(), 450, new Action<int>((val) =>
            {
                GenerateContentForPage();
            }), VanillaSprites.BlueInsertPanelRound, 70);

            searchField = container.AddInputField(new Info("Search", 0, -425, 1700, 150), "", VanillaSprites.BlueInsertPanelRound, new Action<string>((string val) =>
            {
                GenerateContentForPage();
            }), 60, CharacterValidation.None, Il2CppTMPro.TextAlignmentOptions.Left, "Search...", 50);
            searchField.InputField.textComponent.enableAutoSizing = true;
        }
        public void RefreshPage()
        {
            foreach (var panel in bloonPanels)
            {
                panel.SetEntry(null);
            }
            GameMenu.searchingImg.gameObject.SetActive(true);
            GameMenu.refreshBtn.interactable = false;
            Task.Run(async () =>
            {
                PageUpdateRequest request = await ServerHandler.RequestPageUpdate();
                allEntries = request.Data;
                generatedContentReady = true;
            });
        }
        public void GenerateContentForPage()
        {
            FilterEntries();

            for (int i = currentPage * bloonsPerPage; i < (currentPage + 1) * bloonsPerPage; i++)
            {
                int panelIndex = i - (currentPage * bloonsPerPage);
                
                if (i >= filteredEntries.Count)
                {
                    bloonPanels[panelIndex].SetEntry(null);
                    continue;
                }

                var entry = filteredEntries[i];

                bloonPanels[panelIndex].SetEntry(entry);
            }
            LayoutRebuilder.MarkLayoutRootForRebuild(GameMenu.scrollRect.content);
            UpdateBottomBar();
        }
        public void FilterEntries()
        {
            string search = searchField.InputField.text?.ToLower() ?? "";

            filteredEntries = allEntries.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                filteredEntries = filteredEntries.Where(entry =>
                    entry.Name.ToLower().Contains(search) ||
                    entry.Creator.ToLower().Contains(search)
                ).ToList();
            }

            if (BloonFactory.HideIncompatibleBloons)
            {
                var clientVersion = SemVersion.Parse(ModHelperData.Version);
                filteredEntries = filteredEntries.Where(entry =>
                {
                    if (SemVersion.TryParse(entry.Version, out var version))
                    {
                        return clientVersion.Major > version.Major || (clientVersion.Major == version.Major && clientVersion.Minor >= version.Minor);
                    }
                    return false;
                }).ToList();
            }

            filteredEntries = (filterDropdown.Dropdown.value switch
            {
                0 => filteredEntries,
                int val => filteredEntries.Where(entry => (int)entry.CategoryEnum == val - 1).ToList(),
            });

            filteredEntries = (SortingMethod switch
            {
                SortingMethod.Popular => filteredEntries = filteredEntries.OrderByDescending(entry => entry.Downloads).ToList(),
                SortingMethod.New => filteredEntries = filteredEntries.OrderByDescending(entry => entry.UploadTime).ToList(),
                SortingMethod.Old => filteredEntries = filteredEntries.OrderBy(entry => entry.UploadTime).ToList(),
            });

            
        }
        public void UpdateBottomBar()
        {
            GameMenu.firstPageBtn.interactable = TotalPages >= 2 && currentPage > 0;
            GameMenu.previousPageBtn.interactable = TotalPages >= 2 && currentPage > 0;

            GameMenu.nextPageBtn.interactable = TotalPages >= 2 && currentPage < TotalPages - 1;
            GameMenu.lastPageBtn.interactable = TotalPages >= 2 && currentPage < TotalPages - 1;

            GameMenu.totalPages = TotalPages;
            GameMenu.SetCurrentPage(currentPage + 1);
        }
        public void SetPage(int page)
        {
            if (currentPage != page)
            {
                GameMenu.scrollRect.verticalNormalizedPosition = 1f;
            }
            currentPage = page;
            if (page < 0)
                page = 0; 
            if (currentPage > TotalPages - 1)
                currentPage = TotalPages - 1;

            GenerateContentForPage();
        }
    }
    public enum SortingMethod
    {
        Popular,
        New,
        Old
    }
}
