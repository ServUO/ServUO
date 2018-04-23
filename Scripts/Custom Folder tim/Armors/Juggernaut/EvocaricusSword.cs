using System;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	[FlipableAttribute( 0x13B9, 0x13Ba )]
	public class EvocaricusSword : BaseSword
	{
		public override int LabelNumber{ get{ return 1074309; } } 
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.CrushingBlow; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override int AosStrengthReq{ get{ return 40; } }
		public override int AosMinDamage{ get{ return 15; } }
		public override int AosMaxDamage{ get{ return 17; } }
		public override int AosSpeed{ get{ return 28; } }

		public override int OldStrengthReq{ get{ return 40; } }
		public override int OldMinDamage{ get{ return 6; } }
		public override int OldMaxDamage{ get{ return 34; } }
		public override int OldSpeed{ get{ return 30; } }

		public override int DefHitSound{ get{ return 0x237; } }
		public override int DefMissSound{ get{ return 0x23A; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 100; } }
				
		[Constructable]
		public EvocaricusSword() : base( 0x13B9 )
		{
			Weight = 6.0;
			Attributes.WeaponDamage = 50;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1073491, "2" );
			
			if ( this.Parent is Mobile )
			{
				if ( this.Hue == 0x388 )
				{
					list.Add( 1073492 );
					list.Add( 1072514, "10" );
					list.Add( 1073493, "10" );
					list.Add( 1074323, "35" );
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( this.Hue == 0x0 )
			{
				list.Add( 1072378 );
				list.Add( 1060450, "3" );
				list.Add( 1073493, "10" );
				list.Add( 1073493, "10" );
				list.Add( 1074323, "35" );
			}
		}
        			
		public override bool OnEquip( Mobile from )
		{
			Item item = from.FindItemOnLayer( Layer.TwoHanded );
			
			if ( item != null && item.GetType() == typeof( MalekisHonor ) )
			{
				
				Effects.PlaySound( from.Location, from.Map, 503 );
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
				
				Hue = 0x388;
				WeaponAttributes.SelfRepair = 3;
				Attributes.WeaponSpeed = 35;
				MalekisHonor shield = from.FindItemOnLayer( Layer.TwoHanded ) as MalekisHonor;
				shield.Hue = 0x388;
				shield.Attributes.BonusStr = 10;
				shield.Attributes.DefendChance = 10;
				shield.ArmorAttributes.SelfRepair = 3;
				
				from.SendLocalizedMessage( 1072391 );
				
			}
			
			this.InvalidateProperties();
			return base.OnEquip( from );
			
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = ( Mobile )parent;
				Hue = 0x0;
				Attributes.WeaponSpeed = 0;
				WeaponAttributes.SelfRepair = 0;
				if ( m.FindItemOnLayer( Layer.TwoHanded ) is MalekisHonor )
				{
					MalekisHonor shield = m.FindItemOnLayer( Layer.TwoHanded ) as MalekisHonor;
					shield.Hue = 0x0;
					shield.Attributes.BonusStr = 0;
					shield.Attributes.DefendChance = 0;
					shield.ArmorAttributes.SelfRepair = 0;
				}
				this.InvalidateProperties();
			}
			base.OnRemoved( parent );
		}
        
		public EvocaricusSword( Serial serial ) : base( serial )
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