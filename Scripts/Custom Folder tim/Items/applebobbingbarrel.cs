using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class AppleBobbingBarrel : Item
    {
        private Timer m_Timer;

        [Constructable]
        public AppleBobbingBarrel()
            : base(0x154D)
        {
            Weight = 2.0;
            Name = "Apple bobbing barrel";
        }

        public AppleBobbingBarrel(Serial serial)
            : base(serial)
        {
            Name = "Apple bobbing barrel";
        }

        public override void OnDoubleClick(Mobile from)
        {
			if (from.InRange(GetWorldLocation(), 1))
            {
				from.SendMessage("You dunk your head in the water trying franticly to sink your teeth into an apple!");
				m_Timer = new InternalTimer(from, this);
				m_Timer.Start();
			}
            else
                from.SendAsciiMessage("You are to far away to use that!");
        }

        private class InternalTimer : Timer
        {
            private Mobile m_Owner;
            private Item m_bob;
            private DateTime shutitoff = DateTime.Now + TimeSpan.FromSeconds(6);
            public InternalTimer(Mobile m, Item bob)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_Owner = m;
                m_bob = bob;
            }

            protected override void OnTick()
            {
                if (DateTime.Now < shutitoff)
                {
                    m_Owner.CantWalk = true;
                    m_Owner.Direction = m_Owner.GetDirectionTo(m_bob);
                    m_Owner.Animate(32, 5, 1, true, true, 0);
 
                    if (m_Owner != null)
                        m_Owner.PlaySound(37);
                }
                else
                {
                    Stop();
                    m_Owner.CantWalk = false;

                    double AppleChance = Utility.RandomDouble();

                    if (AppleChance <= .20)
                    {
                        m_Owner.AddToBackpack(new Apple(1));
                        m_Owner.SendMessage("You bite into an apple and pull your soaking wet head out of the water!");
                        m_Owner.PublicOverheadMessage(MessageType.Regular, 0xFE, false, "*" + m_Owner.Name + " victoriously pulls an apple from the barrel using only their teeth!*");
                    }
                    else
                    {
                        m_Owner.SendMessage("You fail to bite into any of the apples in the barrel...");
                        m_Owner.PublicOverheadMessage(MessageType.Regular, 0xFE, false, "*" + m_Owner.Name + " is soaking wet without an apple to show for it...*");
                    }
                }
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Name = "Apple bobbing barrel";
        }

    }
}
