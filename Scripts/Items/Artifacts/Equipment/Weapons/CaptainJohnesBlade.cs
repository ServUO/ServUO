namespace Server.Items
{
    public class CaptainJohnesBlade : Scimitar
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154475;  // CaptainJohnesBlade

        [Constructable]
        public CaptainJohnesBlade()
        {
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 60;
            ExtendedWeaponAttributes.Bane = 1;

            Hue = 2124;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 75; cold = 25;
            phys = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public CaptainJohnesBlade(Serial serial)
            : base(serial)
        {
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

    public class GargishCaptainJohnesBlade : GlassSword
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154475;  // GargishCaptainJohnesBlade

        [Constructable]
        public GargishCaptainJohnesBlade()
        {
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 15;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 60;
            ExtendedWeaponAttributes.Bane = 1;

            Hue = 2124;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 75; cold = 25;
            phys = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GargishCaptainJohnesBlade(Serial serial)
            : base(serial)
        {
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