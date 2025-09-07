using BloonFactory.Modules.Spawning;
using FactoryCore.API;
using Il2CppAssets.Scripts.Data.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class SpawningCategory : Category
    {
        public override string Name => "Spawning";

        public override Type[] Modules => [typeof(BloonGroupModule), typeof(MultipleRoundsModule), typeof(SingleRoundModule)];
    }
}
