namespace Server.Items
{
    public class PixieSwatter : Scepter
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PixieSwatter()
        {
            Hue = 0x8A;
            WeaponAttributes.HitPoisonArea = 75;
            Attributes.WeaponSpeed = 30;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.ResistFireBonus = 12;
            WeaponAttributes.ResistEnergyBonus = 12;
            Slayer = SlayerName.Fey;
        }

        public PixieSwatter(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1070854;// Pixie Swatter
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            fire = 100;

            cold = pois = phys = nrgy = chaos = direct = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}