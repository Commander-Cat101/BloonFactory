using BloonFactory.Modules.Tags;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class TagsCategory : Category
    {
        public override string Name => "Tags";

        public override Type[] Modules => [typeof(BloonPropertyModule), typeof(CamoTagModule), typeof(FortifiedTagModule), typeof(MoabTagModule), typeof(RegrowTagModule), typeof(BadTagModule)];
    }
}
