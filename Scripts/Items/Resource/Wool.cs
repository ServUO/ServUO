using Server.Targeting;

namespace Server.Items
{
    public class Wool : Item, IDyable, ICommodity
    {
        [Constructable]
        public Wool()
            : this(1)
        {
        }

        [Constructable]
        public Wool(int amount)
            : base(0xDF8)
        {
            Stackable = true;
            Weight = 4.0;
            Amount = amount;
        }

        public Wool(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new DarkYarn(3)
            {
                Hue = hue
            };

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010576); // You put the balls of yarn in your backpack.
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
            private readonly Wool m_Wool;
            public PickWheelTarget(Wool wool)
                : base(3, false, TargetFlags.None)
            {
                m_Wool = wool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Wool.Deleted)
                    return;

                ISpinningWheel wheel = targeted as ISpinningWheel;

                if (wheel == null && targeted is AddonComponent)
                    wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

                if (wheel is Item)
                {
                    Item item = (Item)wheel;

                    if (!m_Wool.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else if (wheel.Spinning)
                    {
                        from.SendLocalizedMessage(502656); // That spinning wheel is being used.
                    }
                    else
                    {
                        m_Wool.Consume();
                        if (m_Wool is TaintedWool)
                            wheel.BeginSpin(TaintedWool.OnSpun, from, m_Wool.Hue);
                        else
                            wheel.BeginSpin(OnSpun, from, m_Wool.Hue);
                    }
                }
                else
                {
                    from.SendLocalizedMessage(502658); // Use that on a spinning wheel.
                }
            }
        }
    }

    public class TaintedWool : Wool
    {
        [Constructable]
        public TaintedWool()
            : this(1)
        {
        }

        [Constructable]
        public TaintedWool(int amount)
            : base(0x101F)
        {
            Stackable = true;
            Weight = 4.0;
            Amount = amount;
        }

        public TaintedWool(Serial serial)
            : base(serial)
        {
        }

        new public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new DarkYarn(1)
            {
                Hue = hue
            };

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010574); // You put a ball of yarn in your backpack.
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
    }
}
