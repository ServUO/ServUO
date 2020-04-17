namespace Server.Items
{
    public class RubyMace : DiamondMace
    {
        public override int LabelNumber => 1073529; // ruby mace

        [Constructable]
        public RubyMace()
        {
            Attributes.WeaponDamage = 5;
        }

        public RubyMace(Serial serial)
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