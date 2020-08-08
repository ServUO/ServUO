using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using System.Linq;
using System.Xml;

namespace Server.Regions
{
    public class ExploringDeepCreaturesRegion : DungeonRegion
    {
        public ExploringDeepCreaturesRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        Mobile creature;

        public override void OnEnter(Mobile m)
        {
            if ((m is PlayerMobile) && m.Alive)
            {
                PlayerMobile pm = m as PlayerMobile;

                if (m.Region.Name == "Ice Wyrm" && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CusteauPerron)
                {
                    creature = IceWyrm.Spawn(new Point3D(5805 + Utility.RandomMinMax(-5, 5), 240 + Utility.RandomMinMax(-5, 5), 0), Map.Trammel);
                }
                else if (m.Region.Name == "Mercutio The Unsavory" && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    creature = MercutioTheUnsavory.Spawn(new Point3D(2582 + Utility.RandomMinMax(-5, 5), 1118 + Utility.RandomMinMax(-5, 5), 0), Map.Trammel);
                }
                else if (m.Region.Name == "Djinn" && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    creature = Djinn.Spawn(new Point3D(1732 + Utility.RandomMinMax(-5, 5), 520 + Utility.RandomMinMax(-5, 5), 8), Map.Ilshenar);
                }
                else if (m.Region.Name == "Obsidian Wyvern" && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    creature = ObsidianWyvern.Spawn(new Point3D(5136, 966, 0), Map.Trammel);
                }
                else if (m.Region.Name == "Orc Engineer" && pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                {
                    creature = OrcEngineer.Spawn(new Point3D(5311 + Utility.RandomMinMax(-5, 5), 1968 + Utility.RandomMinMax(-5, 5), 0), Map.Trammel);
                }

                if (creature == null)
                    return;
            }
        }
    }

    public class CusteauPerronHouseRegion : GuardedRegion
    {
        public CusteauPerronHouseRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool OnBeginSpellCast(Mobile from, ISpell s)
        {
            if ((s is TeleportSpell || s is GateTravelSpell || s is RecallSpell || s is MarkSpell || s is SacredJourneySpell) && from.IsPlayer())
            {
                from.SendLocalizedMessage(500015); // You do not have that spell!
                return false;
            }
            else
            {
                return base.OnBeginSpellCast(from, s);
            }
        }
    }

    public class NoTravelSpellsAllowed : DungeonRegion
    {
        public NoTravelSpellsAllowed(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool CheckTravel(Mobile m, Point3D newLocation, Spells.TravelCheckType travelType)
        {
            return false;
        }
    }

    public class Underwater : BaseRegion
    {
        public Underwater(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (!base.OnMoveInto(m, d, newLocation, oldLocation))
                return false;

            if (m is PlayerMobile)
            {
                int equipment = m.Items.Where(i => (i is CanvassRobe || i is BootsOfBallast || i is NictitatingLens || i is AquaPendant || i is GargishNictitatingLens) && (i.Parent is Mobile && ((Mobile)i.Parent).FindItemOnLayer(i.Layer) == i)).Count();

                PlayerMobile pm = m as PlayerMobile;

                if (m.AccessLevel == AccessLevel.Player)
                {
                    if (m.Mounted || m.Flying)
                    {
                        m.SendLocalizedMessage(1154411); // You cannot proceed while mounted or flying!
                        return false;
                    }
                    else if (pm.AllFollowers.Count != 0)
                    {
                        if (pm.AllFollowers.Where(x => x is Paralithode).Count() == 0)
                        {
                            pm.SendLocalizedMessage(1154412); // You cannot proceed while pets are under your control!
                            return false;
                        }
                    }
                    else if (pm.ExploringTheDeepQuest != ExploringTheDeepQuestChain.CollectTheComponentComplete)
                    {
                        m.SendLocalizedMessage(1154325); // You feel as though by doing this you are missing out on an important part of your journey...
                        return false;
                    }
                    else if (equipment < 4)
                    {
                        m.SendLocalizedMessage(1154413); // You couldn't hope to survive proceeding without the proper equipment...
                        return false;
                    }
                }
            }
            else if (m is BaseCreature && !(m is Paralithode))
            {
                return false;
            }

            return true;
        }

        public override void OnExit(Mobile m)
        {
            if (m is Paralithode)
            {
                m.Delete();
            }
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool CheckTravel(Mobile m, Point3D newLocation, Spells.TravelCheckType travelType)
        {
            return false;
        }
    }
}
