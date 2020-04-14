using Server.Engines.VvV;
using Server.Mobiles;
using System.Linq;
using System.Xml;

namespace Server.Regions
{
    public class TownRegion : GuardedRegion
    {
        public TownRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);

            if (ViceVsVirtueSystem.EnhancedRules &&
                IsVvVBattleRegion() &&
                ViceVsVirtueSystem.IsVvVCombatant(m) &&
                ViceVsVirtueSystem.Instance != null &&
                ViceVsVirtueSystem.Instance.Battle != null &&
                ViceVsVirtueSystem.Instance.Battle.OnGoing &&
                ViceVsVirtueSystem.Instance.Battle.Region == this)
            {
                ViceVsVirtueSystem.Instance.Battle.AddAggression(m);
            }
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (IsVvVBattleRegion() && m is PlayerMobile && m.HasGump(typeof(BattleWarningGump)))
            {
                m.CloseGump(typeof(BattleWarningGump));
            }
        }

        private bool IsVvVBattleRegion()
        {
            return CityInfo.Infos.Any(kvp => IsPartOf(kvp.Value.Name) && Map == Map.Felucca);
        }
    }
}