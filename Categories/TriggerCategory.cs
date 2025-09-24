using BloonFactory.Modules.Triggers;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class TriggerCategory : Category
    {
        public override string Name => "Triggers";

        public override Type[] Modules => [ typeof(DamagedTriggerModule), typeof(HealthPercentTriggerModule), typeof(SellTowerTriggerModule), typeof(TimeTriggerModule), typeof(TrackPercentTriggerModule)];
    }
}
