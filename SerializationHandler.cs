using BloonFactory.Modules.Core;
using BTD_Mod_Helper;
using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Extensions;
using FactoryCore.API;
using Il2CppSystem.Security.Cryptography;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BloonFactory
{
    internal static class SerializationHandler
    {
        internal static List<BloonTemplate> Templates = new List<BloonTemplate>();
        internal static JsonSerializerSettings Settings => new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        internal static string FolderDirectory => Path.Combine(MelonEnvironment.ModsDirectory, "Factory");

        internal const string FileExtention = ".cstmbln";

        public static bool HasLoaded = false;
        internal static void EnsureFolderExists()
        {
            if (!Directory.Exists(FolderDirectory))
                Directory.CreateDirectory(FolderDirectory);
        }
        internal static void SaveTemplate(BloonTemplate template)
        {
            EnsureFolderExists();

            var content = JsonConvert.SerializeObject(template, Settings);
            var path = Path.Combine(FolderDirectory, template.Guid.ToString() + FileExtention);
            File.WriteAllText(path, content);
        }
        internal static bool ContainGuid(Guid guid)
        {
            return Templates.Any(a => a.Guid == guid);
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
            foreach (string thing in Assembly.GetCallingAssembly().GetManifestResourceNames())
            {
                MelonLogger.Msg(thing);
            }
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
}
