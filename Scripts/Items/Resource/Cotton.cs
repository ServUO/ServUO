using Server.Targeting;

namespace Server.Items
{
    public class Cotton : Item, IDyable, ICommodity
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
            Stackable = true;
            Weight = 4.0;
            Amount = amount;
        }

        public Cotton(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new SpoolOfThread(6)
            {
                Hue = hue
            };

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010577); // You put the spools of thread in your backpack.
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

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
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
                m_Cotton = cotton;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Cotton.Deleted)
                    return;

                ISpinningWheel wheel = targeted as ISpinningWheel;

                if (wheel == null && targeted is AddonComponent)
                    wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

                if (wheel is Item)
                {
                    Item item = (Item)wheel;

                    if (!m_Cotton.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else if (wheel.Spinning)
                    {
                        from.SendLocalizedMessage(502656); // That spinning wheel is being used.
                    }
                    else
                    {
                        m_Cotton.Consume();
                        wheel.BeginSpin(OnSpun, from, m_Cotton.Hue);
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
