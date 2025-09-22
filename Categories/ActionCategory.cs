using BloonFactory.Modules.Actions;
using FactoryCore.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloonFactory.Categories
{
    internal class ActionCategory : Category
    {
        public override string Name => "Actions";

        public override Type[] Modules => [ typeof(SellNearbyTowersActionModule), typeof(BuffNearbyBloonsActionModule), typeof(DestroyNearbyProjectilesActionModule), typeof(DrainLivesActionModule), typeof(DashActionModule), typeof(SetImmuneActionModule), typeof(SpawnBloonsActionModule), typeof(RedirectBloonSpawnActionModule), typeof(StunTowersActionModule), typeof(WaitTimeActionModule), typeof(RemoveAllEffectsActionModule)];
    }
}
