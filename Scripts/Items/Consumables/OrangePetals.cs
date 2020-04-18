using Server.Network;
using System;
using System.Collections;

namespace Server.Items
{
    public class OrangePetals : Item, ICommodity
    {
        private static readonly Hashtable m_Table = new Hashtable();
        [Constructable]
        public OrangePetals()
            : this(1)
        {
        }

        [Constructable]
        public OrangePetals(int amount)
            : base(0x1021)
        {
            Stackable = true;
            Hue = 0x2B;
            Amount = amount;
        }

        public OrangePetals(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1053122;// orange petals
        public override double DefaultWeight => 1.0;
        public static void RemoveContext(Mobile m)
        {
            OrangePetalsContext context = GetContext(m);

            if (context != null)
                RemoveContext(m, context);
        }

        public static bool UnderEffect(Mobile m)
        {
            return (GetContext(m) != null);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (item != this)
                return base.CheckItemUse(from, item);

            if (from != RootParent)
            {
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.
                return false;
            }

            return base.CheckItemUse(from, item);
        }

        public override void OnDoubleClick(Mobile from)
        {
            OrangePetalsContext context = GetContext(from);

            if (context != null)
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061904); // * You already feel resilient! You decide to save the petal for later *
                return;
            }

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061905); // * You eat the orange petal.  You feel more resilient! *
            from.PlaySound(0x3B);

            Timer timer = new OrangePetalsTimer(from);
            timer.Start();

            BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.OrangePetals, 1153785, 1153814, TimeSpan.FromMinutes(5.0), from));

            AddContext(from, new OrangePetalsContext(timer));

            Consume();
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

        private static void AddContext(Mobile m, OrangePetalsContext context)
        {
            m_Table[m] = context;
        }

        private static void RemoveContext(Mobile m, OrangePetalsContext context)
        {
            m_Table.Remove(m);

            context.Timer.Stop();
        }

        private static OrangePetalsContext GetContext(Mobile m)
        {
            return (m_Table[m] as OrangePetalsContext);
        }

        private class OrangePetalsTimer : Timer
        {
            private readonly Mobile m_Mobile;
            public OrangePetalsTimer(Mobile from)
                : base(TimeSpan.FromMinutes(5.0))
            {
                m_Mobile = from;
            }

            protected override void OnTick()
            {
                if (!m_Mobile.Deleted)
                {
                    m_Mobile.LocalOverheadMessage(MessageType.Regular, 0x3F, 1053091); // * You feel the effects of your poison resistance wearing off *
                }

                RemoveContext(m_Mobile);
            }
        }

        private class OrangePetalsContext
        {
            private readonly Timer m_Timer;
            public OrangePetalsContext(Timer timer)
            {
                m_Timer = timer;
            }

            public Timer Timer => m_Timer;
        }
    }
}
