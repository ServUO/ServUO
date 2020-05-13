namespace Server.Mobiles
{
    public class CrystalWisp : Wisp
    {
        [Constructable]
        public CrystalWisp()
        {
            Name = "a crystal wisp";
            Hue = 0x482;
        }

        public CrystalWisp(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.MedScrolls);
            AddLoot(LootPack.ArcanistScrolls, 0, 1);
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
