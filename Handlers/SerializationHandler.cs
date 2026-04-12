using BloonFactory.Modules.Actions.Stats;
using BloonFactory.Modules.Conditionals;
using BloonFactory.Modules.Deprecated;
using BTD_Mod_Helper.Extensions;
using Harmony;
using Il2CppAssets.Scripts.GameEditor.UI;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.GameEditor;
using Il2CppAssets.Scripts.Unity.UI_New;
using Il2CppAssets.Scripts.Unity.UI_New.DailyChallenge;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.EditorMenus;
using Il2CppNinjaKiwi.LiNK.Transfer;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace BloonFactory.Handlers
{
    internal static class SerializationHandler
    {
        internal static List<BloonTemplate> Templates = new List<BloonTemplate>();
        internal static JsonSerializerSettings Settings => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented, SerializationBinder = new ResolverSerializationBinder() };
        internal static string FolderDirectory => Path.Combine(MelonEnvironment.ModsDirectory, "Factory");

        internal const string FileExtention = ".cstmbln";

        public static bool HasLoaded = false;
        internal static void EnsureFolderExists()
        {
            if (!Directory.Exists(FolderDirectory))
                Directory.CreateDirectory(FolderDirectory);
        }
        internal static void SaveTemplate(BloonTemplate template, string path)
        {
            EnsureFolderExists();

            if (template.IsQueueForDeletion)
                return;

            var content = JsonConvert.SerializeObject(template, Settings);
            File.WriteAllText(path + FileExtention, content);
        }
        internal static void SaveTemplate(BloonTemplate template)
        {
            EnsureFolderExists();

            if (template.IsQueueForDeletion)
                return;

            var content = JsonConvert.SerializeObject(template, Settings);
            var path = Path.Combine(FolderDirectory, template.Guid.ToString() + FileExtention);
            File.WriteAllText(path, content);
        }
        internal static bool ContainGuid(Guid guid)
        {
            return Templates.Any(a => a.Guid == guid);
        }
        internal static bool TryLoadTemplate(BloonTemplate template)
        {
            EnsureFolderExists();

            if (ContainGuid(template.Guid))
            {
                MelonLogger.Msg("File already exists");
                return false;
            }

            template.IsLoaded = false;
            SaveTemplate(template);
            Templates.Add(template);
            return true;
        }
        internal static BloonTemplate GetTemplateFromPath(string path)
        {
            EnsureFolderExists();

            if (!File.Exists(path))
                return null;
            File.ReadAllText(path);
            var content = JsonConvert.DeserializeObject<BloonTemplate>(File.ReadAllText(path), Settings);
            return content;
        }
        internal static void LoadTemplate(BloonTemplate template)
        {
            if (!ContainGuid(template.Guid))
            {
                template.SetReferences();
                Templates.Add(template);
            }
        }
        internal static void LoadAllTemplates()
        {
            EnsureFolderExists();

            foreach (var path in Directory.GetFiles(FolderDirectory).Where(f => f.EndsWith(".cstmbln")))
            {
                var template = GetTemplateFromPath(path);
                LoadTemplate(template);
            }
            HasLoaded = true;
        }
        internal static BloonTemplate CreateTemplate(string name)
        {
            EnsureFolderExists();
            var template = JsonConvert.DeserializeObject<BloonTemplate>(Assembly.GetCallingAssembly().GetEmbeddedText("DefaultTemplate" + FileExtention), Settings);

            template.IsLoaded = false;
            template.Name = name;
            template.Guid = Guid.NewGuid();
            template.SetReferences();
            SaveTemplate(template);
            Templates.Add(template);
            return template;
        }
        internal static void DeleteTemplate(BloonTemplate template)
        {
            EnsureFolderExists();
            var path = Path.Combine(FolderDirectory, template.Guid.ToString() + FileExtention);
            template.IsQueueForDeletion = true;
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }    
    public class ResolverSerializationBinder : DefaultSerializationBinder
    {
        public Dictionary<string, Type> ResolveTypes = new Dictionary<string, Type>()
        {
            { "BloonFactory.Modules.Actions.SetSpeedActionModule", typeof(SetSpeedActionModule) },
            { "BloonFactory.Modules.Actions.WaitTimeActionModule", typeof(WaitTimeActionModule) },
            { "BloonFactory.Modules.Actions.SetImmuneActionModule", typeof(SetImmuneActionModule) },
            { "BloonFactory.Modules.Actions.DrainLivesActionModule", typeof(DrainLivesActionModule) },
            { "BloonFactory.Modules.Actions.HealBloonActionModule", typeof(HealBloonActionModule) }
        };
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (ResolveTypes.TryGetValue(typeName, out var type))
            {
                return type;
            }
            return base.BindToType(assemblyName, typeName);
        }
    }
}
