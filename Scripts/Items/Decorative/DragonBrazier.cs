namespace Server.Items
{
    public class DragonBrazier : BaseLight
    {
        public override int LabelNumber => 1075501;  // Dragon Brazier

        public override int LitItemID => 0x194D;
        public override int UnlitItemID => 0x194E;

        [Constructable]
        public DragonBrazier()
            : base(0x194D)
        {
            LootType = LootType.Blessed;
            Weight = 10;
            Light = LightType.Circle150;
            Burning = true;
        }

        public DragonBrazier(Serial serial)
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
            reader.ReadInt();
        }
    }
}
