using System;

namespace Server.Items
{
    public class CocoaLiquor : Item
    {
        [Constructable]
        public CocoaLiquor()
            : base(0x103F)
        {
            this.Hue = 0x46A;
        }

        public CocoaLiquor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080007;
            }
        }// Cocoa liquor
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
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

    public class SackOfSugar : Item
    {
        [Constructable]
        public SackOfSugar()
            : this(1)
        {
        }

        [Constructable]
        public SackOfSugar(int amount)
            : base(0x1039)
        {
            this.Hue = 0x461;
            this.Stackable = true;
            this.Amount = amount;
        }

        public SackOfSugar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080003;
            }
        }// Sack of sugar
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
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

    public class CocoaButter : Item
    {
        [Constructable]
        public CocoaButter()
            : base(0x1044)
        {
            this.Hue = 0x457;
        }

        public CocoaButter(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080005;
            }
        }// Cocoa butter
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
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

    public class Vanilla : Item
    {
        [Constructable]
        public Vanilla()
            : this(1)
        {
        }

        [Constructable]
        public Vanilla(int amount)
            : base(0xE2A)
        {
            this.Hue = 0x462;
            this.Stackable = true;
            this.Amount = amount;
        }

        public Vanilla(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080009;
            }
        }// Vanilla
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
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

    public class CocoaPulp : Item
    {
        [Constructable]
        public CocoaPulp()
            : this(1)
        {
        }

        [Constructable]
        public CocoaPulp(int amount)
            : base(0xF7C)
        {
            this.Hue = 0x219;
            this.Stackable = true;
            this.Amount = amount;
        }

        public CocoaPulp(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080530;
            }
        }// cocoa pulp
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
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

    public class DarkChocolate : CandyCane
    {
        [Constructable]
        public DarkChocolate()
            : base(0xF10)
        {
            this.Hue = 0x465;
            this.LootType = LootType.Regular;
        }

        public DarkChocolate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079994;
            }
        }// Dark chocolate
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
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
        }
    }

    public class MilkChocolate : CandyCane
    {
        [Constructable]
        public MilkChocolate()
            : base(0xF18)
        {
            this.Hue = 0x461;
            this.LootType = LootType.Regular;
        }

        public MilkChocolate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079995;
            }
        }// Milk chocolate
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
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
        }
    }

    public class WhiteChocolate : CandyCane
    {
        [Constructable]
        public WhiteChocolate()
            : base(0xF11)
        {
            this.Hue = 0x47E;
            this.LootType = LootType.Regular;
        }

        public WhiteChocolate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1079996;
            }
        }// White chocolate
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
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
        }
    }
}