namespace Server.Items
{
    public class TheTaskmaster : WarFork
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TheTaskmaster()
        {
            Hue = 0x4F8;
            WeaponAttributes.HitPoisonArea = 100;
            Attributes.BonusDex = 5;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 50;
        }

        public TheTaskmaster(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061110;// The Taskmaster
        public override int ArtifactRarity => 10;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = chaos = direct = 0;
            pois = 100;
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