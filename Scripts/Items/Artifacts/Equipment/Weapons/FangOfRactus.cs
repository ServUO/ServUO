using System;

namespace Server.Items
{
    public class FangOfRactus : Kryss
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public FangOfRactus()
        {
            this.Hue = 0x117;

            this.Attributes.SpellChanneling = 1;
            this.Attributes.AttackChance = 5;
            this.Attributes.DefendChance = 5;
            this.Attributes.WeaponDamage = 35;

            this.WeaponAttributes.HitPoisonArea = 20;
            this.WeaponAttributes.ResistPoisonBonus = 15;
        }

        public FangOfRactus(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1094892;
            }
        }// Fang of Ractus [Replica]
        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override bool CanFortify
        {
            get
            {
                return false;
            }
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