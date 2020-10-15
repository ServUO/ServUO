using Server.Engines.Craft;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Items
{
    public class SnakeCharmerFlute : BambooFlute
    {
        public override int LabelNumber => 1112174;  // snake charmer flute

        public override int InitMinUses => 50;
        public override int InitMaxUses => 80;

        [Constructable]
        public SnakeCharmerFlute()
        {
            Hue = 0x187;
        }

        public SnakeCharmerFlute(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.CanBeginAction(typeof(SnakeCharmerFlute)))
            {
                from.SendLocalizedMessage(1072306); // You must wait a moment for it to recharge.
            }
            else
            {
                from.SendLocalizedMessage(1112175); // Target the serpent you wish to entice.
                from.Target = new CharmTarget(this);
            }
        }

        private class CharmTarget : Target
        {
            private readonly SnakeCharmerFlute m_Flute;

            public CharmTarget(SnakeCharmerFlute flute)
                : base(12, false, TargetFlags.None)
            {
                m_Flute = flute;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature bc = targeted as BaseCreature;

                if (bc != null && IsSnake(bc))
                {
                    if (bc.CharmMaster != null)
                    {
                        bc.PrivateOverheadMessage(MessageType.Regular, 0x3B2, 502802, from.NetState); // Someone else is already taming this.
                    }
                    else
                    {
                        from.SendLocalizedMessage(502475); // Click where you wish the animal to go.
                        from.Target = new InternalTarget(bc, m_Flute);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1112176); // That is not a snake or serpent.
                }
            }

            private class InternalTarget : Target
            {
                private readonly BaseCreature m_Snake;
                private readonly SnakeCharmerFlute m_Flute;

                public InternalTarget(BaseCreature snake, SnakeCharmerFlute flute)
                    : base(10, true, TargetFlags.None)
                {
                    m_Snake = snake;
                    m_Flute = flute;
                }

                protected override void OnTarget(Mobile from, object targ)
                {
                    if (targ is IPoint2D)
                    {
                        Point2D p = new Point2D((IPoint2D)targ);

                        if (!m_Snake.InRange(p, 10))
                        {
                            from.SendLocalizedMessage(500643); // Target is too far away.
                        }
                        else
                        {
                            m_Snake.BeginCharm(from, p);

                            from.SendLocalizedMessage(502479); // The animal walks where it was instructed to.

                            from.BeginAction(typeof(SnakeCharmerFlute));

                            Timer.DelayCall(TimeSpan.FromSeconds(5.0), delegate { from.EndAction(typeof(SnakeCharmerFlute)); });

                            m_Flute.PlayInstrumentWell(from);
                            m_Flute.UsesRemaining--;

                            if (m_Flute.UsesRemaining == 0)
                            {
                                from.SendLocalizedMessage(1112177); // You broke your snake charmer flute.

                                m_Flute.Delete();
                            }
                        }
                    }
                }
            }
        }

        private static bool IsSnake(BaseCreature bc)
        {
            return m_SnakeTypes.Any(t => t == bc.GetType());
        }

        private static readonly Type[] m_SnakeTypes = new Type[]
        {
            typeof(LavaSnake),    typeof(Snake),
            typeof(CoralSnake),   typeof(GiantSerpent),
            typeof(SilverSerpent)
        };

        public override int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            base.OnCraft(quality, makersMark, from, craftSystem, typeRes, tool, craftItem, resHue);

            Hue = 0x187;

            return quality;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
