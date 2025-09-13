using BloonFactory.Modules.Core;
using FactoryCore.API;
using Il2CppSystem.Security.Cryptography;
using MelonLoader;
using MelonLoader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        internal static BloonTemplate GetTemplate(string path)
        {
            EnsureFolderExists();

            if (!File.Exists(path))
                return null;
            File.ReadAllText(path);
            var content = JsonConvert.DeserializeObject<BloonTemplate>(File.ReadAllText(path), Settings);
            return content;
        }
        internal static bool TryLoadTemplate(BloonTemplate template)
        {
            EnsureFolderExists();

            if (Templates.Any(a => a.Guid == template.Guid))
            {
                MelonLogger.Msg("File already exists");
                return false;
            }

            template.IsLoaded = false;
            SaveTemplate(template);
            Templates.Add(template);
            return true;
        }
        internal static void LoadAllTemplates()
        {
            EnsureFolderExists();

            foreach (var path in Directory.GetFiles(FolderDirectory).Where(f => f.EndsWith(".cstmbln")))
            {
                var template = GetTemplate(path);
                if (!Templates.Any(a => a.Guid == template.Guid))
                {
                    template.LoadModules();
                    Templates.Add(template);
                }
            }
            HasLoaded = true;
        }
        internal static BloonTemplate CreateTemplate(string name)
        {
            EnsureFolderExists();

            var template = new BloonTemplate() { IsLoaded = false, Guid = Guid.NewGuid(), Name = name };
            template.AddModule(new BloonModule());
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
