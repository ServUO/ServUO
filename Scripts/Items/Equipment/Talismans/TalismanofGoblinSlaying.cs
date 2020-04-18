namespace Server.Items
{
    public class TalismanofGoblinSlaying : BaseTalisman
    {
        public override int LabelNumber => 1095011; //Talisman of Goblin Slaying
        public override bool ForceShowName => true;

        [Constructable]
        public TalismanofGoblinSlaying()
            : base(0x2F58)
        {
            Slayer = TalismanSlayerName.Goblin;
            MaxChargeTime = 1200;
        }

        public TalismanofGoblinSlaying(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}