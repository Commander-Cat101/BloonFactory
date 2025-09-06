using FactoryCore.API;
using Il2CppAssets.Scripts.Models.Bloons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.LinkTypes
{
    internal class Trigger
    {
        public BloonModel bloonModel;

        public List<Module> TriggeredModules = new();
        public Trigger(BloonModel model)
        {
            bloonModel = model;
        }

        public Trigger With(Module module)
        {
            TriggeredModules.Add(module);
            return this;
        }
    }
}
