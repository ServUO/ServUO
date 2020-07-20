namespace Server.Items
{
    public class ArielHavenWritofMembership : Item
    {
        public override int LabelNumber => 1094998; //Ariel Haven Writ of Membership

        [Constructable]
        public ArielHavenWritofMembership()
            : base(0x14ED)
        {
            LootType = LootType.Blessed;
        }

        public ArielHavenWritofMembership(Serial serial)
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
            reader.ReadEncodedInt();
        }
    }
}
