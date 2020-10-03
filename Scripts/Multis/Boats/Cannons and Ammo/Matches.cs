using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class Matches : Item, ICommodity
    {
        public override int LabelNumber => 1116112;

        private static readonly TimeSpan LightDuration = TimeSpan.FromMinutes(60);

        private bool m_IsLight;

        public bool IsLight { get { return m_IsLight; } set { m_IsLight = value; } }

        [Constructable]
        public Matches() : this(1)
        {
        }

        [Constructable]
        public Matches(int amount) : base(3947)
        {
            Stackable = true;
            Layer = Layer.TwoHanded;
            Amount = amount;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                if (Amount > 1)
                {
                    Container pack = from.Backpack;

                    if (pack != null)
                    {
                        Matches match = new Matches();

                        if (pack.CheckHold(from, match, true))
                        {
                            pack.DropItem(match);
                            Amount--;

                            match.ItemID = 2578;
                            from.SendSound(0x047);
                            match.IsLight = true;
                        }
                        else
                            match.Delete();
                    }
                }
                else if (!m_IsLight)
                {
                    new InternalTimer(this);
                    from.SendLocalizedMessage(1116114); //You ignite the match.

                    ItemID = 2578;
                    from.SendSound(0x047);
                    m_IsLight = true;
                }
                else
                {
                    from.Target = new InternalTarget(this);
                    from.SendLocalizedMessage(1116113); //Target the cannon whose fuse you wish to light.
                }
            }
        }


        public void BurnOut()
        {
            if (RootParent is PlayerMobile)
                ((PlayerMobile)RootParent).SendLocalizedMessage(1116115); //Your match splutters and dies.

            Delete();
        }

        private class InternalTimer : Timer
        {
            private readonly Matches m_Match;

            public InternalTimer(Matches match) : base(LightDuration)
            {
                m_Match = match;
                Start();
            }

            protected override void OnTick()
            {
                if (m_Match != null)
                    m_Match.BurnOut();
            }
        }

        private class InternalTarget : Target
        {
            private readonly Matches m_Match;

            public InternalTarget(Matches match) : base(3, false, TargetFlags.None)
            {
                m_Match = match;
            }

            protected override void OnTarget(Mobile from, object obj)
            {
                if (obj is IShipCannon)
                {
                    IShipCannon cannon = (IShipCannon)obj;

                    if (cannon.CanLight)
                    {
                        cannon.LightFuse(from);
                    }
                    else
                    {
                        from.SendLocalizedMessage(1116078); //There is no fuse to light! Prime the cannon first.
                    }
                }
            }
        }

        public Matches(Serial serial) : base(serial) { }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_IsLight);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            if (reader.ReadBool())
                Delete();
        }
    }
}
