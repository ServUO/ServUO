namespace Server.Items
{
    public class FaerieFire : ElvenCompositeLongbow
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1072908; // Faerie Fire
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public FaerieFire()
            : base()
        {
            Hue = 0x489;
            Balanced = true;
            Attributes.BonusDex = 3;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.HitFireball = 25;
        }

        public FaerieFire(Serial serial)
            : base(serial)
        {
        }

        #region Mondain's Legacy
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = cold = pois = nrgy = chaos = direct = 0;
            fire = 100;
        }

        #endregion

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