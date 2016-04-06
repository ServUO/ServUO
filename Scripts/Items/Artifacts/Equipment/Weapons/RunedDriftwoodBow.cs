using Server;
using System;

namespace Server.Items
{
    public class RunedDriftwoodBow : Bow
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1149961; } }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        [Constructable]
        public RunedDriftwoodBow()
        {
            Hue = 2955;

            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            Attributes.LowerAmmoCost = 15;
        }

        public RunedDriftwoodBow(Serial serial)
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