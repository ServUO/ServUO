using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class TheChallengeRite : Item
    {
        public override int LabelNumber { get { return 1150904; } } //The Challenge Rite

        [Constructable]
        public TheChallengeRite()
            : base(0x0FF2)
        {
            Hue = 447;
        }

        public TheChallengeRite(Serial serial)
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

    public class OnTheVoid : Item
    {
        public override int LabelNumber { get { return 1150907; } } //On the Void

        [Constructable]
        public OnTheVoid()
            : base(0x0FF2)
        {
            Hue = 447;
        }

        public OnTheVoid(Serial serial)
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

    public class InMemory : Item
    {
        public override int LabelNumber { get { return 1150913; } } //In Memory

        [Constructable]
        public InMemory()
            : base(0x0FF2)
        {
            Hue = 447;
        }

        public InMemory(Serial serial)
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

    public class AthenaeumDecree : Item
    {
        public override int LabelNumber { get { return 1150905; } } //Athenaeum Decree

        [Constructable]
        public AthenaeumDecree()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public AthenaeumDecree(Serial serial)
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

    public class ALetterFromTheKing : Item
    {
        public override int LabelNumber { get { return 1150906; } } //A Letter from the King

        [Constructable]
        public ALetterFromTheKing()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public ALetterFromTheKing(Serial serial)
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

    public class ShilaxrinarsMemorial : Item
    {
        public override int LabelNumber { get { return 1150908; } } //Shilaxrinar's Memorial

        [Constructable]
        public ShilaxrinarsMemorial()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public ShilaxrinarsMemorial(Serial serial)
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

    public class ToTheHighScholar : Item
    {
        public override int LabelNumber { get { return 1150909; } } //To the High Scholar

        [Constructable]
        public ToTheHighScholar()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public ToTheHighScholar(Serial serial)
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

    public class ToTheHighBroodmother : Item
    {
        public override int LabelNumber { get { return 1150910; } } //To the High Broodmother

        [Constructable]
        public ToTheHighBroodmother()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public ToTheHighBroodmother(Serial serial)
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

    public class ReplyToTheHighScholar : Item
    {
        public override int LabelNumber { get { return 1150911; } } //Reply to the High Scholar

        [Constructable]
        public ReplyToTheHighScholar()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public ReplyToTheHighScholar(Serial serial)
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

    public class AccessToTheIsle : Item
    {
        public override int LabelNumber { get { return 1150912; } } //Access to the Isle

        [Constructable]
        public AccessToTheIsle()
            : base(0x14ED)
        {
            Hue = 447;
        }

        public AccessToTheIsle(Serial serial)
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