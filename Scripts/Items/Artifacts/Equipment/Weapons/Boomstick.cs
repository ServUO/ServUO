namespace Server.Items
{
    public class Boomstick : WildStaff
    {
        public override bool IsArtifact => true;

        [Constructable]
        public Boomstick()
            : base()
        {
            Hue = 0x25;
            Attributes.SpellChanneling = 1;
            Attributes.RegenMana = 3;
            Attributes.CastSpeed = 1;
            Attributes.LowerRegCost = 20;
        }

        public Boomstick(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075032;// Boomstick
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = pois = nrgy = direct = 0;
            chaos = 100;
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
