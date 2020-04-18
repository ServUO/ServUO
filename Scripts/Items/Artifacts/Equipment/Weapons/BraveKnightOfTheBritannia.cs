namespace Server.Items
{
    public class BraveKnightOfTheBritannia : Katana
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BraveKnightOfTheBritannia()
        {
            Hue = 0x47e;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.HitLeechStam = 48;
            WeaponAttributes.HitHarm = 26;
            WeaponAttributes.HitLeechHits = 22;
        }

        public BraveKnightOfTheBritannia(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094909;// Brave Knight of The Britannia [Replica]
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = chaos = direct = 0;
            fire = 40;
            cold = 30;
            pois = 10;
            nrgy = 20;
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