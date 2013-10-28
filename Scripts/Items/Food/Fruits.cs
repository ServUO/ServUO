using System;

namespace Server.Items
{
    public class FruitBasket : Food
    {
        [Constructable]
        public FruitBasket()
            : base(1, 0x993)
        {
            this.Weight = 2.0;
            this.FillFactor = 5;
            this.Stackable = false;
        }

        public FruitBasket(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new Basket());
            return true;
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

    [FlipableAttribute(0x171f, 0x1720)]
    public class Banana : Food
    {
        [Constructable]
        public Banana()
            : this(1)
        {
        }

        [Constructable]
        public Banana(int amount)
            : base(amount, 0x171f)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Banana(Serial serial)
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
    }

    [Flipable(0x1721, 0x1722)]
    public class Bananas : Food
    {
        [Constructable]
        public Bananas()
            : this(1)
        {
        }

        [Constructable]
        public Bananas(int amount)
            : base(amount, 0x1721)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Bananas(Serial serial)
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
    }

    public class SplitCoconut : Food
    {
        [Constructable]
        public SplitCoconut()
            : this(1)
        {
        }

        [Constructable]
        public SplitCoconut(int amount)
            : base(amount, 0x1725)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public SplitCoconut(Serial serial)
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
    }

    public class Lemon : Food
    {
        [Constructable]
        public Lemon()
            : this(1)
        {
        }

        [Constructable]
        public Lemon(int amount)
            : base(amount, 0x1728)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Lemon(Serial serial)
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
    }

    public class Lemons : Food
    {
        [Constructable]
        public Lemons()
            : this(1)
        {
        }

        [Constructable]
        public Lemons(int amount)
            : base(amount, 0x1729)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Lemons(Serial serial)
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
    }

    public class Lime : Food
    {
        [Constructable]
        public Lime()
            : this(1)
        {
        }

        [Constructable]
        public Lime(int amount)
            : base(amount, 0x172a)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Lime(Serial serial)
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
    }

    public class Limes : Food
    {
        [Constructable]
        public Limes()
            : this(1)
        {
        }

        [Constructable]
        public Limes(int amount)
            : base(amount, 0x172B)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Limes(Serial serial)
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
    }

    public class Coconut : Food
    {
        [Constructable]
        public Coconut()
            : this(1)
        {
        }

        [Constructable]
        public Coconut(int amount)
            : base(amount, 0x1726)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Coconut(Serial serial)
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
    }

    public class OpenCoconut : Food
    {
        [Constructable]
        public OpenCoconut()
            : this(1)
        {
        }

        [Constructable]
        public OpenCoconut(int amount)
            : base(amount, 0x1723)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public OpenCoconut(Serial serial)
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
    }

    public class Dates : Food
    {
        [Constructable]
        public Dates()
            : this(1)
        {
        }

        [Constructable]
        public Dates(int amount)
            : base(amount, 0x1727)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Dates(Serial serial)
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
    }

    public class Grapes : Food
    {
        [Constructable]
        public Grapes()
            : this(1)
        {
        }

        [Constructable]
        public Grapes(int amount)
            : base(amount, 0x9D1)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Grapes(Serial serial)
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
    }

    public class Peach : Food
    {
        [Constructable]
        public Peach()
            : this(1)
        {
        }

        [Constructable]
        public Peach(int amount)
            : base(amount, 0x9D2)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Peach(Serial serial)
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
    }

    public class Pear : Food
    {
        [Constructable]
        public Pear()
            : this(1)
        {
        }

        [Constructable]
        public Pear(int amount)
            : base(amount, 0x994)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Pear(Serial serial)
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
    }

    public class Apple : Food
    {
        [Constructable]
        public Apple()
            : this(1)
        {
        }

        [Constructable]
        public Apple(int amount)
            : base(amount, 0x9D0)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Apple(Serial serial)
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
    }

    public class Watermelon : Food
    {
        [Constructable]
        public Watermelon()
            : this(1)
        {
        }

        [Constructable]
        public Watermelon(int amount)
            : base(amount, 0xC5C)
        {
            this.Weight = 5.0;
            this.FillFactor = 5;
        }

        public Watermelon(Serial serial)
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

            if (version < 1)
            {
                if (this.FillFactor == 2)
                    this.FillFactor = 5;

                if (this.Weight == 2.0)
                    this.Weight = 5.0;
            }
        }
    }

    public class SmallWatermelon : Food
    {
        [Constructable]
        public SmallWatermelon()
            : this(1)
        {
        }

        [Constructable]
        public SmallWatermelon(int amount)
            : base(amount, 0xC5D)
        {
            this.Weight = 5.0;
            this.FillFactor = 5;
        }

        public SmallWatermelon(Serial serial)
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
    }

    [FlipableAttribute(0xc72, 0xc73)]
    public class Squash : Food
    {
        [Constructable]
        public Squash()
            : this(1)
        {
        }

        [Constructable]
        public Squash(int amount)
            : base(amount, 0xc72)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Squash(Serial serial)
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
    }

    [FlipableAttribute(0xc79, 0xc7a)]
    public class Cantaloupe : Food
    {
        [Constructable]
        public Cantaloupe()
            : this(1)
        {
        }

        [Constructable]
        public Cantaloupe(int amount)
            : base(amount, 0xc79)
        {
            this.Weight = 1.0;
            this.FillFactor = 1;
        }

        public Cantaloupe(Serial serial)
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
    }
}