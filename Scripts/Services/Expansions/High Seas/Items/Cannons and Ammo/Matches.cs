using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class Matches : Item
    {
        public override int LabelNumber { get { return 1116112; } }

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

                    if(pack != null)
                    {
                        Matches match = new Matches();

                        if (pack.CheckHold(from, match, true))
                        {
                            pack.DropItem(match);
                            this.Amount--;

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
            private Matches m_Match;

            public InternalTimer(Matches match) : base(Matches.LightDuration)
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
            private Matches m_Match;

            public InternalTarget(Matches match) : base (3, false, TargetFlags.None)
            {
                m_Match = match;
            }

            protected override void OnTarget(Mobile from, object obj)
            {
                if (obj is BaseCannon)
                {
                    BaseCannon cannon = (BaseCannon)obj;

                    if (cannon.CanLight)
                        cannon.LightFuse(from);
                    else
                        from.SendLocalizedMessage(1116078); //There is no fuse to light! Prime the cannon first.
                }
            }
        }

        public Matches(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
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