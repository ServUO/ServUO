using System;

namespace Server.Items
{
    public class RecarosRiposte : WarFork
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RecarosRiposte()
        {
            this.LootType = LootType.Blessed;

            this.Attributes.AttackChance = 5;
            this.Attributes.WeaponSpeed = 10;
            this.Attributes.WeaponDamage = 25;
        }

        public RecarosRiposte(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1078195;
            }
        }// Recaro's Riposte
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