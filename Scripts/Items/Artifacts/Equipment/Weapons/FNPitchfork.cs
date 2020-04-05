using System;

namespace Server.Items
{
    [FlipableAttribute(0xE87, 0xE88)]
    public class FNPitchfork : BaseSpear
	{
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get { return 1113498; } }// Farmer Nash's Pitchfork		
		
        [Constructable]
        public FNPitchfork()
            : base(0xE87)
        {
            Weight = 11.0;
        }

        public FNPitchfork(Serial serial)
            : base(serial)
        {
        }
        
        public override WeaponAbility PrimaryAbility
        {
            get
            {
                return WeaponAbility.BleedAttack;
            }
        }
        public override WeaponAbility SecondaryAbility
        {
            get
            {
                return WeaponAbility.Dismount;
            }
        }
        public override int StrengthReq
        {
            get
            {
                return 50;
            }
        }
        public override int MinDamage
        {
            get
            {
                return 13;
            }
        }
        public override int MaxDamage
        {
            get
            {
                return 14;
            }
        }
        public override float Speed
        {
            get
            {
                return 2.50f;
            }
        }
        
        public override int InitMinHits
        {
            get
            {
                return 31;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 60;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
