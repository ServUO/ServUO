using System;
using System.Collections;
using Server.Network;

namespace Server.Items
{
    public class OrangePetals : Item
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
            this.Stackable = true;
            this.Hue = 0x2B;
            this.Amount = amount;
        }

        public OrangePetals(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1053122;
            }
        }// orange petals
        public override double DefaultWeight
        {
            get
            {
                return 0.1;
            }
        }
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

            if (from != this.RootParent)
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
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061904);
                return;
            }

            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061905);
            from.PlaySound(0x3B);

            Timer timer = new OrangePetalsTimer(from);
            timer.Start();
            
            BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.OrangePetals, 1153785, 1153814, TimeSpan.FromMinutes(5.0), from));

            AddContext(from, new OrangePetalsContext(timer));

            this.Consume();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
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
                this.m_Mobile = from;
            }

            protected override void OnTick()
            {
                if (!this.m_Mobile.Deleted)
                {
                    this.m_Mobile.LocalOverheadMessage(MessageType.Regular, 0x3F, true,
                        "* You feel the effects of your poison resistance wearing off *");
                }

                RemoveContext(this.m_Mobile);
            }
        }

        private class OrangePetalsContext
        {
            private readonly Timer m_Timer;
            public OrangePetalsContext(Timer timer)
            {
                this.m_Timer = timer;
            }

            public Timer Timer
            {
                get
                {
                    return this.m_Timer;
                }
            }
        }
    }
}
