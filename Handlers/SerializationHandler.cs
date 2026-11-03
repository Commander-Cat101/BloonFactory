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
        internal static void EnsureFolderExists()
        {
            if (!Directory.Exists(FolderDirectory))
                Directory.CreateDirectory(FolderDirectory);
        }
        internal static void SaveTemplate(BloonTemplate template, string path)
        {
            EnsureFolderExists();
            var content = JsonConvert.SerializeObject(template, Settings);
            File.WriteAllText(path + FileExtention, content);
        }
        internal static void SaveTemplate(BloonTemplate template)
        {
            SaveTemplate(template, Path.Combine(FolderDirectory, template.GetUUID()));
        }
        internal static bool IsUUIDLoaded(string uuid)
        {
            return Templates.Any(a => a.GetUUID() == uuid);
        }
        internal static bool TryLoadTemplate(BloonTemplate template)
        {
            EnsureFolderExists();

            if (IsUUIDLoaded(template.GetUUID()))
            {
                MelonLogger.Msg("File already exists");
                return false;
            }

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
            if (!IsUUIDLoaded(template.GetUUID()))
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
        }
        internal static BloonTemplate CreateTemplate(string name)
        {
            EnsureFolderExists();
            var template = JsonConvert.DeserializeObject<BloonTemplate>(Assembly.GetCallingAssembly().GetEmbeddedText("DefaultTemplate" + FileExtention), Settings);

            template.Name = name;
            template.SetReferences();
            
            template.CreationTime = DateTime.Now;
            template.UUID = Guid.NewGuid().ToString();
            
            SaveTemplate(template);
            Templates.Add(template);
            return template;
        }
        internal static void DeleteTemplate(BloonTemplate template)
        {
            EnsureFolderExists();
            
            var path = Path.Combine(FolderDirectory, template.GetUUID() + FileExtention);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        internal static bool TryGetTemplate(string uuid, out BloonTemplate template)
        {
            if (Templates.Any(a => a.GetUUID() == uuid))
            {
                template = Templates.First(a => a.GetUUID() == uuid);
                return true;
            }
            template = null;
            return false;
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
