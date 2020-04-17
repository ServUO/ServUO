namespace Server.Items
{
    public class MischiefMaker : MagicalShortbow
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1072910;// Mischief Maker

        [Constructable]
        public MischiefMaker()
            : base()
        {
            Hue = 0x8AB;
            Balanced = true;
            Slayer = SlayerName.Silver;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 45;
        }

        public MischiefMaker(Serial serial)
            : base(serial)
        {
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = fire = phys = nrgy = chaos = direct = 0;
            cold = 100;
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