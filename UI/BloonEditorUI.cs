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
        public override List<Category> Categories => [new TagsCategory(), new BehaviorsCategory(), new TriggerCategory(), new ActionCategory(), new DisplayCategory() ];

        public override void SaveTemplate()
        {
            SerializationHandler.SaveTemplate((BloonTemplate)Template);
        }
    }
}
