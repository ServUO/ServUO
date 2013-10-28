using System;
using Server.Targeting;

namespace Server.Items
{
    public class Cotton : Item, IDyable
    {
        [Constructable]
        public Cotton()
            : this(1)
        {
        }

        [Constructable]
        public Cotton(int amount)
            : base(0xDF9)
        {
            this.Stackable = true;
            this.Weight = 4.0;
            this.Amount = amount;
        }

        public Cotton(Serial serial)
            : base(serial)
        {
        }

        public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new SpoolOfThread(6);
            item.Hue = hue;

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010577); // You put the spools of thread in your backpack.
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

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(502655); // What spinning wheel do you wish to spin this on?
                from.Target = new PickWheelTarget(this);
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        private class PickWheelTarget : Target
        {
            private readonly Cotton m_Cotton;
            public PickWheelTarget(Cotton cotton)
                : base(3, false, TargetFlags.None)
            {
                this.m_Cotton = cotton;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Cotton.Deleted)
                    return;

                ISpinningWheel wheel = targeted as ISpinningWheel;

                if (wheel == null && targeted is AddonComponent)
                    wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

                if (wheel is Item)
                {
                    Item item = (Item)wheel;

                    if (!this.m_Cotton.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else if (wheel.Spinning)
                    {
                        from.SendLocalizedMessage(502656); // That spinning wheel is being used.
                    }
                    else
                    {
                        this.m_Cotton.Consume();
                        wheel.BeginSpin(new SpinCallback(Cotton.OnSpun), from, this.m_Cotton.Hue);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502658); // Use that on a spinning wheel.
                }
            }
        }
    }
}