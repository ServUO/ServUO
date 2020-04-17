namespace Server.Items
{
    public class DragonsEnd : Longsword
    {
        public override bool IsArtifact => true;
        [Constructable]
        public DragonsEnd()
            : base()
        {
            Hue = 0x554;
            Slayer = SlayerName.DragonSlaying;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 60;
            WeaponAttributes.ResistFireBonus = 20;
        }

        public DragonsEnd(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1079791;// Dragon's End
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 225;
        public override int InitMaxHits => 225;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = nrgy = pois = direct = chaos = 0;
            cold = 100;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NegativeAttributes.NoRepair = 0;
            }
        }
    }
}