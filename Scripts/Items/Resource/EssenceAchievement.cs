namespace Server.Items
{
    public class EssenceAchievement : Item, ICommodity
    {
        [Constructable]
        public EssenceAchievement()
            : this(1)
        {
        }

        [Constructable]
        public EssenceAchievement(int amount)
            : base(0x571C)
        {
            Stackable = true;
            Amount = amount;
            Hue = 1724;
        }

        public EssenceAchievement(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1113325;// essence of achievement
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;
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
