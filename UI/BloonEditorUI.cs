using BloonFactory.Categories;
using FactoryCore.API;
using FactoryCore.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.UI
{
    internal class BloonEditorUI : EditorUI
    {
        public override List<Category> Categories => new List<Category>() { new LogicCategory() };

        public override void SaveTemplate()
        {
            SerializationHandler.SaveTemplate((BloonTemplate)Template);
        }
    }
}
