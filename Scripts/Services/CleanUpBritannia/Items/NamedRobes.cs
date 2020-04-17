namespace Server.Items
{
    public class HumansAndElvesRobe : Robe
    {
        public override int LabelNumber => 1151202;  // Humans & Elves are our friends!

        [Constructable]
        public HumansAndElvesRobe()
        {
            LootType = LootType.Blessed;
            Hue = Utility.RandomDyedHue();
        }

        public HumansAndElvesRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GargoylesAreOurFriendsRobe : Robe
    {
        public override int LabelNumber => 1151203;  // Gargoyles are our friends!

        [Constructable]
        public GargoylesAreOurFriendsRobe()
        {
            LootType = LootType.Blessed;
            Hue = Utility.RandomDyedHue();
        }

        public GargoylesAreOurFriendsRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WeArePiratesRobe : Robe
    {
        public override int LabelNumber => 1151204;  // We are pirates!

        [Constructable]
        public WeArePiratesRobe()
        {
            LootType = LootType.Blessed;
            Hue = Utility.RandomDyedHue();
        }

        public WeArePiratesRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FollowerOfBaneRobe : Robe
    {
        public override int LabelNumber => 1151205;  // Follower of Bane

        [Constructable]
        public FollowerOfBaneRobe()
        {
            LootType = LootType.Blessed;
            Hue = Utility.RandomDyedHue();
        }

        public FollowerOfBaneRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class QueenDawnForeverRobe : Robe
    {
        public override int LabelNumber => 1151206;  // Queen Dawn Forever

        [Constructable]
        public QueenDawnForeverRobe()
        {
            LootType = LootType.Blessed;
            Hue = Utility.RandomDyedHue();
        }

        public QueenDawnForeverRobe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}