using System;

namespace Server.Items
{
    public class CaptainJohnesBlade : Scimitar
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154475; } } // CaptainJohnesBlade

        [Constructable]
        public CaptainJohnesBlade()
        {
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.Attributes.AttackChance = 15;
            this.Attributes.DefendChance = 15;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 60;
            this.ExtendedWeaponAttributes.Bane = 1;

            this.Hue = 2124;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 75; cold = 25;
            phys = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public CaptainJohnesBlade(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishCaptainJohnesBlade : GlassSword
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154475; } } // GargishCaptainJohnesBlade

        [Constructable]
        public GargishCaptainJohnesBlade()
        {
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.Attributes.AttackChance = 15;
            this.Attributes.DefendChance = 15;
            this.Attributes.WeaponSpeed = 30;
            this.Attributes.WeaponDamage = 60;
            this.ExtendedWeaponAttributes.Bane = 1;

            this.Hue = 2124;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = 75; cold = 25;
            phys = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishCaptainJohnesBlade(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}