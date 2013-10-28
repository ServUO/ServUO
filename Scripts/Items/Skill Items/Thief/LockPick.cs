using System;
using Server.Targeting;

namespace Server.Items
{
    public interface ILockpickable : IPoint2D
    {
        int LockLevel { get; set; }
        bool Locked { get; set; }
        Mobile Picker { get; set; }
        int MaxLockLevel { get; set; }
        int RequiredSkill { get; set; }
        void LockPick(Mobile from);
    }

    [FlipableAttribute(0x14fc, 0x14fb)]
    public class Lockpick : Item
    {
        [Constructable]
        public Lockpick()
            : this(1)
        {
        }

        [Constructable]
        public Lockpick(int amount)
            : base(0x14FC)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public Lockpick(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 0.1)
                this.Weight = -1;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(502068); // What do you want to pick?
            from.Target = new InternalTarget(this);
        }

        private class InternalTarget : Target
        {
            private readonly Lockpick m_Item;
            public InternalTarget(Lockpick item)
                : base(1, false, TargetFlags.None)
            {
                this.m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Item.Deleted)
                    return;

                if (targeted is ILockpickable)
                {
                    Item item = (Item)targeted;
                    from.Direction = from.GetDirectionTo(item);

                    if (((ILockpickable)targeted).Locked)
                    {
                        from.PlaySound(0x241);

                        new InternalTimer(from, (ILockpickable)targeted, this.m_Item).Start();
                    }
                    else
                    {
                        // The door is not locked
                        from.SendLocalizedMessage(502069); // This does not appear to be locked
                    }
                }
                else
                {
                    from.SendLocalizedMessage(501666); // You can't unlock that!
                }
            }

            private class InternalTimer : Timer
            {
                private readonly Mobile m_From;
                private readonly ILockpickable m_Item;
                private readonly Lockpick m_Lockpick;
                public InternalTimer(Mobile from, ILockpickable item, Lockpick lockpick)
                    : base(TimeSpan.FromSeconds(3.0))
                {
                    this.m_From = from;
                    this.m_Item = item;
                    this.m_Lockpick = lockpick;
                    this.Priority = TimerPriority.TwoFiftyMS;
                }

                protected void BrokeLockPickTest()
                {
                    // When failed, a 25% chance to break the lockpick
                    if (Utility.Random(4) == 0)
                    {
                        Item item = (Item)this.m_Item;

                        // You broke the lockpick.
                        item.SendLocalizedMessageTo(this.m_From, 502074);

                        this.m_From.PlaySound(0x3A4);
                        this.m_Lockpick.Consume();
                    }
                }

                protected override void OnTick()
                {
                    Item item = (Item)this.m_Item;

                    if (!this.m_From.InRange(item.GetWorldLocation(), 1))
                        return;

                    if (this.m_Item.LockLevel == 0 || this.m_Item.LockLevel == -255)
                    {
                        // LockLevel of 0 means that the door can't be picklocked
                        // LockLevel of -255 means it's magic locked
                        item.SendLocalizedMessageTo(this.m_From, 502073); // This lock cannot be picked by normal means
                        return;
                    }

                    if (this.m_From.Skills[SkillName.Lockpicking].Value < this.m_Item.RequiredSkill)
                    {
                        /*
                        // Do some training to gain skills
                        m_From.CheckSkill( SkillName.Lockpicking, 0, m_Item.LockLevel );*/
                        // The LockLevel is higher thant the LockPicking of the player
                        item.SendLocalizedMessageTo(this.m_From, 502072); // You don't see how that lock can be manipulated.
                        return;
                    }

                    if (this.m_From.CheckTargetSkill(SkillName.Lockpicking, this.m_Item, this.m_Item.LockLevel, this.m_Item.MaxLockLevel))
                    {
                        // Success! Pick the lock!
                        item.SendLocalizedMessageTo(this.m_From, 502076); // The lock quickly yields to your skill.
                        this.m_From.PlaySound(0x4A);
                        this.m_Item.LockPick(this.m_From);
                    }
                    else
                    {
                        // The player failed to pick the lock
                        this.BrokeLockPickTest();
                        item.SendLocalizedMessageTo(this.m_From, 502075); // You are unable to pick the lock.
                    }
                }
            }
        }
    }
}