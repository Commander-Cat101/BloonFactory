using BloonFactory.LinkTypes;
using BloonFactory.Modules.Actions;
using BloonFactory.Modules.Actions.Stats;
using BloonFactory.Modules.Behaviors;
using BloonFactory.Modules.Conditionals;
using BloonFactory.Modules.Core;
using BloonFactory.Modules.Display;
using BloonFactory.Modules.Spawning;
using BloonFactory.Modules.Tags;
using BloonFactory.Modules.Triggers;
using FactoryCore.API;
using FactoryCore.UI;
using FactoryCore.UI.ContextMenus;
using Il2CppAssets.Scripts.Models.Bloons;
using Il2CppAssets.Scripts.Models.Rounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UIElements.StylePropertyAnimationSystem;

namespace BloonFactory.UI
{
    internal class BloonEditorUI : EditorUI
    {
        public override Dictionary<Type, Color> NodeColors => new Dictionary<Type, Color>()
        {
            { typeof(BloonModel), Color.green },
            { typeof(Trigger), Color.magenta },
            { typeof(Visuals), Color.cyan },
            { typeof(RoundModel), Color.yellow },
            { typeof(RoundSetModel), Color.blue},
            { typeof(BloonTexture), Color.red}
        };
        public override void SaveTemplate()
        {
            SerializationHandler.SaveTemplate((BloonTemplate)Template);
        }
        public override Type CenteredModule => typeof(BloonModule);
        public override ContextMenuInfo ContextMenu => ContextMenuBuilder.Create(550)
            .WithNested("Tags", 700, menu =>
            {
                menu.WithButton(typeof(BloonPropertyModule))
                .WithButton(typeof(CamoTagModule))
                .WithButton(typeof(FortifiedTagModule))
                .WithButton(typeof(MoabTagModule))
                .WithButton(typeof(RegrowTagModule))
                .WithButton(typeof(BadTagModule));
            })
            .WithNested("Behaviors", 800, menu =>
            {
                menu.WithButton(typeof(DamageReductionModule))
                .WithButton(typeof(QuickEntryModule))
                .WithButton(typeof(SpeedUpNearbyBloonsModule))
                .WithButton(typeof(CashOnPopModule))
                .WithButton(typeof(AddChildrenModule));
            })
            .WithNested("Triggers", 800, menu =>
            {
                menu.WithButton(typeof(DamagedTriggerModule))
                .WithButton(typeof(HealthPercentTriggerModule))
                .WithButton(typeof(SellTowerTriggerModule))
                .WithButton(typeof(TimeTriggerModule))
                .WithButton(typeof(TrackPercentTriggerModule))
                .WithButton(typeof(LivesChangeTriggerModule))
                .WithButton(typeof(OnEnterTriggerModule));
            })
            .WithNested("Actions", 800, menu =>
            {
                menu.WithNested("Stats", 800, subMenu =>
                {
                    subMenu.WithButton(typeof(ModifyHealthActionModule))
                    .WithButton(typeof(ModifySpeedActionModule))
                    .WithButton(typeof(ModifyDamageActionModule))
                    .WithButton(typeof(ModifyPropertyActionModule));
                })
                .WithNested("Game", 800, subMenu =>
                {

                })
                .WithButton(typeof(SellNearbyTowersActionModule))
                .WithButton(typeof(BuffNearbyBloonsActionModule))
                .WithButton(typeof(HealBloonActionModule))
                .WithButton(typeof(DrainLivesActionModule))
                .WithButton(typeof(DashActionModule))
                .WithButton(typeof(SetImmuneActionModule))
                .WithButton(typeof(SpawnBloonsActionModule))
                .WithButton(typeof(RedirectBloonSpawnActionModule))
                .WithButton(typeof(StunTowersActionModule))
                .WithButton(typeof(RemoveAllEffectsActionModule))
                .WithButton(typeof(SetSpeedActionModule));
            })
            .WithNested("Conditionals", 800, menu =>
            {
                menu.WithButton(typeof(WaitTimeActionModule))
                .WithButton(typeof(CompareActionModule))
                .WithButton(typeof(RandomActionModule));
            })
            .WithNested("Display", 800, menu =>
            {
                menu.WithButton(typeof(SimpleDisplayModule))
                .WithButton(typeof(DamageStateDisplayModule))
                .WithButton(typeof(BloonTextureModule))
                .WithButton(typeof(CustomTextureModule))
                .WithButton(typeof(DecalModule))
                .WithButton(typeof(CustomDecalModule))
                .WithButton(typeof(TintModule));
            })
            .WithNested("Spawning", 800, menu =>
            {
                menu.WithButton(typeof(BloonGroupModule))
                .WithButton(typeof(MultipleRoundsModule))
                .WithButton(typeof(SingleRoundModule))
                .WithButton(typeof(ReplaceBloonModule));
            });
    }
}
