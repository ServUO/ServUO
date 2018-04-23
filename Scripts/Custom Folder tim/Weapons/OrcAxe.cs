using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	[FlipableAttribute( 0xF47, 0xF48 )]
	public class OrcAxe : BaseAxe
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }

		public override int AosStrengthReq{ get{ return 90; } }
		public override int AosMinDamage{ get{ return 40; } }
		public override int AosMaxDamage{ get{ return 45; } }
		public override int AosSpeed{ get{ return 31; } }

		public override int OldStrengthReq{ get{ return 90; } }
		public override int OldMinDamage{ get{ return 40; } }
		public override int OldMaxDamage{ get{ return 45; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 70; } }

		[Constructable]
		public OrcAxe() : base( 0xF47 )
		{
			Weight = 4.0;
                        Name = "Orkaxt";
                        Hue = 1000;
			Layer = Layer.TwoHanded;
		}

		public OrcAxe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}