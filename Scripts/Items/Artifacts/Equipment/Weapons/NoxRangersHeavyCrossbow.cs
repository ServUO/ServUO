namespace Server.Items
{
    public class NoxRangersHeavyCrossbow : HeavyCrossbow
    {
        public override bool IsArtifact => true;
        [Constructable]
        public NoxRangersHeavyCrossbow()
        {
            Hue = 0x58C;
            WeaponAttributes.HitLeechStam = 40;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 20;
            WeaponAttributes.ResistPoisonBonus = 10;
        }

        public NoxRangersHeavyCrossbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063485;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 50;
            phys = 50;

            fire = cold = nrgy = chaos = direct = 0;
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