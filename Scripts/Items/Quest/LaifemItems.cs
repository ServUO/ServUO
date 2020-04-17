namespace Server.Items
{
    public class LetterOfIntroduction : Item
    {
        public override int LabelNumber => 1113243;  // Laifem's Letter of Introduction

        [Constructable]
        public LetterOfIntroduction()
            : base(0x1F23)
        {
            Hue = 1167;
            Weight = 2.0;
            QuestItem = true;
        }

        public LetterOfIntroduction(Serial serial) : base(serial)
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

    public class MasteringWeaving : Item
    {
        public override int LabelNumber => 1113244;  // Mastering the Art of Weaving

        [Constructable]
        public MasteringWeaving()
            : base(0x1E20)
        {
            Hue = 744;
            Weight = 2.0;
            QuestItem = true;
        }

        public MasteringWeaving(Serial serial)
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
