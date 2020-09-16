namespace Server.Items
{
    public class GiftBoxHues
    {
        /* there's possibly a couple more, but this is what we could verify on OSI */
        private static readonly int[] m_NormalHues =
        {
            0x672,
            0x454,
            0x507,
            0x4ac,
            0x504,
            0x84b,
            0x495,
            0x97c,
            0x493,
            0x4a8,
            0x494,
            0x4aa,
            0xb8b,
            0x84f,
            0x491,
            0x851,
            0x503,
            0xb8c,
            0x4ab,
            0x84B
        };
        private static readonly int[] m_NeonHues =
        {
            0x438,
            0x424,
            0x433,
            0x445,
            0x42b,
            0x448
        };
        public static int RandomGiftBoxHue => m_NormalHues[Utility.Random(m_NormalHues.Length)];
        public static int RandomNeonBoxHue => m_NeonHues[Utility.Random(m_NeonHues.Length)];
    }

    [Flipable(0x46A5, 0x46A6)]
    public class GiftBoxRectangle : BaseContainer
    {
        [Constructable]
        public GiftBoxRectangle()
            : base(Utility.RandomBool() ? 0x46A5 : 0x46A6)
        {
            Hue = GiftBoxHues.RandomGiftBoxHue;
        }

        public GiftBoxRectangle(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x11E;
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

    public class GiftBoxCube : BaseContainer
    {
        [Constructable]
        public GiftBoxCube()
            : base(0x46A2)
        {
            Hue = GiftBoxHues.RandomGiftBoxHue;
        }

        public GiftBoxCube(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x11B;
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

    public class GiftBoxCylinder : BaseContainer
    {
        [Constructable]
        public GiftBoxCylinder()
            : base(0x46A3)
        {
            Hue = GiftBoxHues.RandomGiftBoxHue;
        }

        public GiftBoxCylinder(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x11C;
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

    public class GiftBoxOctogon : BaseContainer
    {
        [Constructable]
        public GiftBoxOctogon()
            : base(0x46A4)
        {
            Hue = GiftBoxHues.RandomGiftBoxHue;
        }

        public GiftBoxOctogon(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x11D;
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

    public class GiftBoxAngel : BaseContainer
    {
        [Constructable]
        public GiftBoxAngel()
            : base(0x46A7)
        {
            Hue = GiftBoxHues.RandomGiftBoxHue;
        }

        public GiftBoxAngel(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultGumpID => 0x11F;
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

    [Flipable(0x232A, 0x232B)]
    public class GiftBoxNeon : BaseContainer
    {
        [Constructable]
        public GiftBoxNeon()
            : base(Utility.RandomBool() ? 0x232A : 0x232B)
        {
            Hue = GiftBoxHues.RandomNeonBoxHue;
        }

        public GiftBoxNeon(Serial serial)
            : base(serial)
        {
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