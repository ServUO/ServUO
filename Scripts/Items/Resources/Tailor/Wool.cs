using System;
using Server.Targeting;

namespace Server.Items
{
    public class Wool : Item, IDyable
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
            this.Stackable = true;
            this.Weight = 4.0;
            this.Amount = amount;
        }

        public Wool(Serial serial)
            : base(serial)
        {
        }

        public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new DarkYarn(3);
            item.Hue = hue;

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010576); // You put the balls of yarn in your backpack.
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
            private readonly Wool m_Wool;
            public PickWheelTarget(Wool wool)
                : base(3, false, TargetFlags.None)
            {
                this.m_Wool = wool;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Wool.Deleted)
                    return;

                ISpinningWheel wheel = targeted as ISpinningWheel;

                if (wheel == null && targeted is AddonComponent)
                    wheel = ((AddonComponent)targeted).Addon as ISpinningWheel;

                if (wheel is Item)
                {
                    Item item = (Item)wheel;

                    if (!this.m_Wool.IsChildOf(from.Backpack))
                    {
                        from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                    }
                    else if (wheel.Spinning)
                    {
                        from.SendLocalizedMessage(502656); // That spinning wheel is being used.
                    }
                    else
                    {
                        this.m_Wool.Consume();
                        if (this.m_Wool is TaintedWool)
                            wheel.BeginSpin(new SpinCallback(TaintedWool.OnSpun), from, this.m_Wool.Hue);
                        else
                            wheel.BeginSpin(new SpinCallback(Wool.OnSpun), from, this.m_Wool.Hue);
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
            this.Stackable = true;
            this.Weight = 4.0;
            this.Amount = amount;
        }

        public TaintedWool(Serial serial)
            : base(serial)
        {
        }

        new public static void OnSpun(ISpinningWheel wheel, Mobile from, int hue)
        {
            Item item = new DarkYarn(1);
            item.Hue = hue;

            from.AddToBackpack(item);
            from.SendLocalizedMessage(1010574); // You put a ball of yarn in your backpack.
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
    }
}