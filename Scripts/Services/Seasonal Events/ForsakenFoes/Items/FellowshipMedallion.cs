using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Items
{
    public interface IFellowshipMedallion
    {
        void Start(PlayerMobile pm);
    }

    public class FellowshipMedallion : Item, IFellowshipMedallion
    {
        public override int LabelNumber => 1159248;  // Fellowship Medallion

        [Constructable]
        public FellowshipMedallion()
            : base(0xA429)
        {
            Weight = 1.0;
            Layer = Layer.Neck;
        }

        public FellowshipMedallion(Serial serial)
            : base(serial)
        {
        }

        public static bool IsDressed(Mobile from)
        {
            return CheckMedallion(from) != null;
        }

        public static IFellowshipMedallion CheckMedallion(Mobile from)
        {
            return from.FindItemOnLayer(Layer.Neck) as IFellowshipMedallion;            
        }

        private Timer m_Timer;

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is PlayerMobile pm)
            {
                Start(pm);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            Stop();
        }

        public void Start(PlayerMobile pm)
        {
            if (pm != null && m_Timer == null || !m_Timer.Running)
            {
                m_Timer = new InternalTimer(pm);
                m_Timer.Start();
            }
        }

        public void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(FellowshipMedallionGump)))
            {
                from.SendGump(new FellowshipMedallionGump(this));
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1157722, "its origin"); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                from.PlaySound(1050);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }

        public static void Initialize()
        {
            EventSink.Login += OnLogin;
        }

        public static void OnLogin(LoginEventArgs e)
        {
            var pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                var medallion = CheckMedallion(pm);

                if (medallion != null)
                {
                    medallion.Start(pm);
                }
            }
        }
    }

    public class InternalTimer : Timer
    {
        private readonly Mobile _Mobile;

        public InternalTimer(Mobile m)
            : base(TimeSpan.FromMinutes(12), TimeSpan.FromMinutes(12))
        {
            _Mobile = m;

            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            if (_Mobile != null && _Mobile.NetState != null && _Mobile.Alive)
            {
                _Mobile.PrivateOverheadMessage(MessageType.Regular, 0x21, 1159298 + Utility.Random(11),
                    _Mobile.NetState);
                _Mobile.PlaySound(1664);
            }
            else
            {
                Stop();
            }
        }
    }

    public class FellowshipMedallionGump : Gump
    {
        public FellowshipMedallionGump(Item item)
            : base(100, 100)
        {
            AddPage(0);

            AddBackground(0, 0, 454, 400, 0x24A4);
            AddItem(75, 120, item.ItemID, item.Hue);
            AddHtmlLocalized(177, 50, 250, 18, 1114513, "#1159248", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(177, 77, 250, 36, 1114513, "#1159033", 0x3442, false, false); // <DIV ALIGN=CENTER>~1_TOKEN~</DIV>
            AddHtmlLocalized(177, 122, 250, 228, 1159247, 0xC63, true, true); // This is an otherwise unassuming metal medallion in the shape of a triangle.  The letters T, W, and U are engraved on it. It is almost immediately recognizable as a sign of the Fellowship.
        }
    }

    public class GargishFellowshipMedallion : GargishNecklace, IFellowshipMedallion
    {
        public override int LabelNumber => 1159248;  // Fellowship Medallion

        public override bool IsArtifact => true;

        [Constructable]
        public GargishFellowshipMedallion()
            : base(0xA42A)
        {
            Weight = 1.0;            
            Layer = Layer.Neck;
        }

        public GargishFellowshipMedallion(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(FellowshipMedallionGump)))
            {
                from.SendGump(new FellowshipMedallionGump(this));
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1157722, "its origin"); // *Your proficiency in ~1_SKILL~ reveals more about the item*
                from.PlaySound(1050);
            }
        }

        private Timer m_Timer;

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is PlayerMobile pm)
            {
                Start(pm);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            Stop();
        }

        public void Start(PlayerMobile pm)
        {
            if (pm != null)
            {
                if (m_Timer == null || !m_Timer.Running)
                {
                    m_Timer = new InternalTimer(pm);
                    m_Timer.Start();
                }
            }
        }

        public void Stop()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
