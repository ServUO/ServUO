using Server.Engines.JollyRoger;
using Server.Items;
using Server.Network;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using System.Collections.Generic;
using System.Linq;

namespace Server.Regions
{
    public class WellOfSoulsRegion : Region
    {
        public static void Initialize()
        {
            var wellOfSoulsRegion = new WellOfSoulsRegion();
        }

        private static Rectangle2D _Bound = new Rectangle2D(2246, 1537, 36, 40);

        public WellOfSoulsRegion()
            : base("Well Of Souls", Map.Ilshenar, DefaultPriority, _Bound)
        {
            Register();
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.IsPlayer())
            {
                if (s is MarkSpell)
                {
                    m.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
                    return false;
                }

                if (s is GateTravelSpell)
                {
                    m.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                    return false;
                }
            }

            return base.OnBeginSpellCast(m, s);
        }
    }

    public class VirtueDef
    {
        public Shrine Shrine { get; set; }
        public Rectangle2D Area { get; set; }
        public string Title { get; set; }

        public VirtueDef(Shrine shrine, Rectangle2D area, string title)
        {
            Shrine = shrine;
            Area = area;
            Title = title;
        }
    }

    public class WellOfSoulsVirtuesRegion : Region
    {
        private static readonly List<VirtueDef> Virtue = new List<VirtueDef>()
        {
            new VirtueDef(Shrine.Spirituality, new Rectangle2D(2262, 1561, 4, 4), "Spiritual"),
            new VirtueDef(Shrine.Compassion, new Rectangle2D(2248, 1557, 4, 4), "Compassionate"),
            new VirtueDef(Shrine.Honor, new Rectangle2D(2248, 1547, 4, 4), "Honorable"),
            new VirtueDef(Shrine.Honesty, new Rectangle2D(2255, 1541, 4, 4), "Honest"),
            new VirtueDef(Shrine.Humility, new Rectangle2D(2262, 1539, 4, 4), "Humble"),
            new VirtueDef(Shrine.Justice, new Rectangle2D(2269, 1541, 4, 4) , "Just"),
            new VirtueDef(Shrine.Valor, new Rectangle2D(2276, 1547, 4, 4), "Valiant"),
            new VirtueDef(Shrine.Sacrifice, new Rectangle2D(2276, 1557, 4, 4), "Sacrificing"),
        };

        public WellOfSoulsVirtuesRegion()
            : base("Well Of Souls Virtues", Map.Ilshenar, DefaultPriority, Virtue.Select(x => x.Area).ToArray())
        {
            Register();
        }

        public override void OnEnter(Mobile m)
        {
            var virtue = Virtue.FirstOrDefault(x => x.Area.Contains(m.Location));

            var list = JollyRogerData.GetList(m);

            if (list != null && list.Shrine != null)
            {
                var s = list.Shrine.FirstOrDefault(x => x.Shrine == virtue.Shrine);

                if (s != null && s.MasterDeath >= 3)
                {
                    if (!list.Cloak && list.Shrine.Count == 8 && !list.Shrine.Any(x => x.MasterDeath < 3))
                    {
                        var item = new CloakOfTheVirtuous();

                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                        {
                            m.SendLocalizedMessage(1152337,
                                item.ToString()); // A reward of ~1_ITEM~ will be delivered to you once you free up room in your backpack.
                            item.Delete();
                        }
                        else
                        {
                            m.PrivateOverheadMessage(MessageType.Regular, 0x47E, 1159339,
                                m.NetState); // Thous hast proven thou walks the path of Virtue!
                            JollyRogerData.SetCloak(m, true);
                            m.SendLocalizedMessage(1152339,
                                item.ToString()); // A reward of ~1_ITEM~ has been placed in your backpack.
                            m.PlaySound(0x419);
                        }
                    }
                    else
                    {
                        m.PrivateOverheadMessage(MessageType.Regular, 0x47E, false,
                            string.Format("*Thou are truly {0}...*", virtue.Title), m.NetState);

                        m.FixedParticles(0x376A, 1, 72, 0x13B5, EffectLayer.Waist);
                        m.PlaySound(0x1F2);
                    }
                }
                else
                {
                    m.PrivateOverheadMessage(MessageType.Regular, 0x47E, false, string.Format("*Thou are not truly {0}...*", virtue.Title), m.NetState);
                }
            }
            else
            {
                m.PrivateOverheadMessage(MessageType.Regular, 0x47E, false, string.Format("*Thou are not truly {0}...*", virtue.Title), m.NetState);
            }
        }
    }
}
