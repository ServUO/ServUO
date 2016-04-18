using System;

namespace Server.Items
{
    public class RuneCarvingKnife : AssassinSpike
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RuneCarvingKnife()
        {
            this.Hue = 0x48D;

            this.WeaponAttributes.HitLeechMana = 40;
            this.Attributes.RegenStam = 2;
            this.Attributes.LowerManaCost = 10;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 30;
        }

        public RuneCarvingKnife(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072915;
            }
        }// Rune Carving Knife
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