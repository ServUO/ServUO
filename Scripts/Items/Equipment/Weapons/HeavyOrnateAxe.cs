namespace Server.Items
{
    public class HeavyOrnateAxe : OrnateAxe
    {
        public override int LabelNumber => 1073548; // heavy ornate axe

        [Constructable]
        public HeavyOrnateAxe()
        {
            Attributes.WeaponDamage = 8;
        }

        public HeavyOrnateAxe(Serial serial)
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