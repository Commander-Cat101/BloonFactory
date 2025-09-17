using BloonFactory.Modules.Behaviors;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class BehaviorsCategory : Category
    {
        public override string Name => "Behaviors";

        public override Type[] Modules => [typeof(DamageReductionModule), typeof(QuickEntryModule), typeof(SpeedUpNearbyBloonsModule), typeof(CashOnPopModule), typeof(AddChildrenModule)];
    }
}
