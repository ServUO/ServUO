using System;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Gumps;

namespace Server.Items
{
    public abstract class BalmOrLotion : Item
    {
        public abstract int ApplyMessage { get; }

        public void AddBuff(Mobile m)
        {
            AddBuff(m, Duration);
        }

        public virtual void AddBuff(Mobile m, TimeSpan duration) { }
        public virtual void RemoveBuff(Mobile m) { }

        public virtual TimeSpan Duration { get { return TimeSpan.FromMinutes(30.0); } }

        public BalmOrLotion(int itemId)
            : base(itemId)
        {
            Weight = 1.0;
        }

        private static Dictionary<Mobile, BalmOrLotionContext> m_Table = new Dictionary<Mobile, BalmOrLotionContext>();

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); // You must have the object in your backpack to use it.
            }
            else if (m_Table.ContainsKey(from))
            {
                from.SendLocalizedMessage(1095133); // You are already under the effect of a balm or lotion.

                from.CloseGump(typeof(ReplaceBalmOrLotionGump));
                from.SendGump(new ReplaceBalmOrLotionGump(this));
            }
            else if (FountainOfFortune.HasAnyBlessing(from))
            {
                from.SendLocalizedMessage(1095133); // You are already under the effect of a balm or lotion.
            }
            else
            {
                Use(from);
            }
        }

        public void Use(Mobile from)
        {
            from.PlaySound(0x1ED);
            from.SendLocalizedMessage(ApplyMessage);

            Effects.SendPacket(from, from.Map, new GraphicalEffect(Network.EffectType.FixedFrom, from.Serial, Serial.Zero, 0x375A, from.Location, from.Location, 9, 20, true, false));

            AddBuff(from);

            Timer expireTimer = Timer.DelayCall(Duration, new TimerStateCallback<Mobile>(RemoveBuffCallback), from);
            BalmOrLotionContext context = new BalmOrLotionContext(expireTimer, this);

            m_Table[from] = context;

            Delete();
        }

        public void RemoveBuffCallback(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                m.SendLocalizedMessage(1095134); // The effects of the balm or lotion have worn off.

                RemoveBuff(m);

                m_Table.Remove(m);
            }
        }

        public static bool IncreaseDuration(Mobile from)
        {
            if (m_Table.ContainsKey(from))
            {
                BalmOrLotionContext context = m_Table[from];

                if (context.Item is BalmOfStrength || context.Item is BalmOfSwiftness || context.Item is BalmOfSwiftness)
                {
                    TimeSpan left = context.ExpireTimer.Next - DateTime.UtcNow;

                    context.ExpireTimer.Stop();
                    context.ExpireTimer = Timer.DelayCall(left + TimeSpan.FromHours(1.0), new TimerStateCallback<Mobile>(context.Item.RemoveBuffCallback), from);
                    context.ExpireTimer.Start();

                    context.Item.RemoveBuff(from);
                    context.Item.AddBuff(from);

                    return true;
                }
            }

            return false;
        }

        public static void ReplaceBalmOrLotion(Mobile m, BalmOrLotion newBalm)
        {
            if (m_Table.ContainsKey(m))
            {
                BalmOrLotionContext oldBalmContext = m_Table[m];

                oldBalmContext.Item.RemoveBuff(m);
                oldBalmContext.ExpireTimer.Stop();
            }

            newBalm.Use(m);
        }

        protected static BalmOrLotion GetActiveBalmFor(Mobile m)
        {
            if (m_Table.ContainsKey(m))
                return m_Table[m].Item;
            else
                return null;
        }

        public BalmOrLotion(Serial serial)
            : base(serial)
        {
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

        private class BalmOrLotionContext
        {
            private Timer m_ExpireTimer;
            private BalmOrLotion m_Item;

            public Timer ExpireTimer { get { return m_ExpireTimer; } set { m_ExpireTimer = value; } }
            public BalmOrLotion Item { get { return m_Item; } }

            public BalmOrLotionContext(Timer expireTimer, BalmOrLotion item)
            {
                m_ExpireTimer = expireTimer;
                m_Item = item;
            }
        }
    }

    public class ReplaceBalmOrLotionGump : Gump
    {
        private BalmOrLotion m_NewBalm;

        public ReplaceBalmOrLotionGump(BalmOrLotion newBalm)
            : base(340, 340)
        {
            m_NewBalm = newBalm;

            AddPage(0);

            AddBackground(0, 0, 291, 99, 0x13BE);
            AddImageTiled(5, 6, 280, 20, 0xA40);

            AddHtmlLocalized(9, 8, 280, 20, 1095145, 0x7FFF, false, false); // Replace active balm or lotion
            AddImageTiled(5, 31, 280, 40, 0xA40);

            AddHtmlLocalized(9, 35, 272, 40, 1095144, 0x7FFF, false, false); // Applying this will replace the effects currently active. Do you wish to proceed?

            AddButton(215, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(250, 75, 65, 20, 1006044, 0x7FFF, false, false); // OK

            AddButton(5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 75, 100, 20, 1060051, 0x7FFF, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && !m_NewBalm.Deleted && m_NewBalm.IsChildOf(sender.Mobile.Backpack))
                BalmOrLotion.ReplaceBalmOrLotion(sender.Mobile, m_NewBalm);
        }
    }
}