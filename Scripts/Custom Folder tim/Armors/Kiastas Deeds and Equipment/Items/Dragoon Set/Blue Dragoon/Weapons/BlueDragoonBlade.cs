using System;
using Server;
using Server.Items;
using Server.Targets;

namespace Server.Kiasta
{
	[FlipableAttribute( 0x13FF, 0x13FE )]
    public class BlueDragoonBlade : BaseDragoonWeapon
	{
        public override SkillName DefSkill { get { return SkillName.Swords; } }
        public override WeaponType DefType { get { return WeaponType.Slashing; } }
        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.DoubleStrike; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }

        public override int AosStrengthReq { get { return 25; } }
        public override int AosMinDamage { get { return 11; } }
        public override int AosMaxDamage { get { return 13; } }
        public override int AosSpeed { get { return 46; } }
        public override float MlSpeed { get { return 2.50f; } }

        public override int OldStrengthReq { get { return 10; } }
        public override int OldMinDamage { get { return 5; } }
        public override int OldMaxDamage { get { return 26; } }
        public override int OldSpeed { get { return 58; } }

        public override int DefHitSound { get { return 0x23B; } }
        public override int DefMissSound { get { return 0x23A; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 90; } }

		[Constructable]
		public BlueDragoonBlade() : base( 0x13FF )
		{
            Resource = CraftResource.BlueScales;
            Name = "Blade "+Settings.DragoonEquipmentName.Suffix;
			Weight = 6.0;
		}

        public BlueDragoonBlade(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}