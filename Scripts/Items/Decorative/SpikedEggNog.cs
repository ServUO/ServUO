using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpikedEggNog : Item, ISecurable
    {
        public override int LabelNumber => 1157647;  // Spiked Egg Nog

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUseTime { get; set; }

        private Timer m_Timer;

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public SpikedEggNog()
            : base(0x3BBA)
        {
            Hue = 2711;
        }

        public bool CheckAccessible(Mobile from, Item item)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true; // Staff can access anything

            BaseHouse house = BaseHouse.FindHouseAt(item);

            if (house == null)
                return false;

            switch (Level)
            {
                case SecureLevel.Owner: return house.IsOwner(from);
                case SecureLevel.CoOwners: return house.IsCoOwner(from);
                case SecureLevel.Friends: return house.IsFriend(from);
                case SecureLevel.Anyone: return true;
                case SecureLevel.Guild: return house.IsGuildMember(from);
            }

            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (!IsLockedDown && !IsSecure)
            {
                from.SendLocalizedMessage(1112573); // This must be locked down or secured in order to use it.
            }
            else if (CheckAccessible(from, this))
            {
                BeginBleed(from);
            }
        }

        public void BeginBleed(Mobile m)
        {
            m.LocalOverheadMessage(MessageType.Regular, 0x21, 1010571); // Ouch!

            if (DateTime.UtcNow < NextUseTime)
            {
                m.SendLocalizedMessage(1078497); // You cannot use that right now
                return;
            }

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Bleed, 1075829, 1075830, TimeSpan.FromSeconds(12), m, string.Format("{0}\t{1}\t{2}", "1", "10", "2")));

            m_Timer = new BleedTimer(m, this);
            m_Timer.Start();

            NextUseTime = DateTime.UtcNow + TimeSpan.FromMinutes(1);

            m.LocalOverheadMessage(MessageType.Regular, 0x21, 1060757); // You are bleeding profusely

            m.PlaySound(555);
            m.FixedParticles(0x119A, 244, 25, 9950, 31, 0, EffectLayer.Waist);
        }

        public void EndBleed(Mobile m)
        {
            m_Timer.Stop();
            BuffInfo.RemoveBuff(m, BuffIcon.Bleed);

            m.SendLocalizedMessage(1060167); // The bleeding wounds have healed, you are no longer bleeding!
        }

        public void DoBleed(Mobile m)
        {
            if (m.Alive)
            {
                m.PlaySound(0x133);
                AOS.Damage(m, Utility.Random(1, 10), false, 0, 0, 0, 0, 100);

                Blood blood = new Blood
                {
                    ItemID = Utility.Random(0x122A, 5)
                };
                blood.MoveToWorld(m.Location, m.Map);
            }
            else
            {
                EndBleed(m);
            }
        }

        private class BleedTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly SpikedEggNog m_Item;
            private int m_Count;
            private readonly int m_MaxCount;

            public BleedTimer(Mobile m, SpikedEggNog item)
                : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                m_Mobile = m;
                m_Item = item;
                Priority = TimerPriority.TwoFiftyMS;

                m_MaxCount = 6;
            }

            protected override void OnTick()
            {
                if (!m_Mobile.Alive || m_Mobile.Deleted || m_Item.Deleted)
                {
                    m_Item.EndBleed(m_Mobile);
                }
                else
                {
                    m_Item.DoBleed(m_Mobile);

                    if (++m_Count == m_MaxCount)
                        m_Item.EndBleed(m_Mobile);
                }
            }
        }

        public SpikedEggNog(Serial serial)
            : base(serial)
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(NextUseTime);
            writer.Write((int)Level);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            NextUseTime = reader.ReadDateTime();
            Level = (SecureLevel)reader.ReadInt();
        }
    }
}
