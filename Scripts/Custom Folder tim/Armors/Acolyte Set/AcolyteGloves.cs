using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13C6, 0x13CE )]
	public class AcolyteGloves : BaseArmor
	{
		public override int LabelNumber{ get{ return 1074307; } } 
		public override int BasePhysicalResistance{ get{ return 7; } }
		public override int BaseFireResistance{ get{ return 7; } }
		public override int BaseColdResistance{ get{ return 3; } }
		public override int BasePoisonResistance{ get{ return 4; } }
		public override int BaseEnergyResistance{ get{ return 4; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 15; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }


		[Constructable]
		public AcolyteGloves() : base( 0x13C6 )
		{
			Weight = 2.0;
			Attributes.BonusMana = 2;
			Attributes.SpellDamage = 2;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1072376, "4" );
			
			if ( this.Parent is Mobile )
			{
				if ( this.Hue == 0x2 )
				{
					list.Add( 1072377 );
					list.Add( 1073489, "100" );
					list.Add( 1060441 );
					list.Add( 1060450, "3" );
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( this.Hue == 0x0 )
			{
				list.Add( 1072378 );
				list.Add( 1072382, "3" );
				list.Add( 1072383, "3" );
				list.Add( 1072384, "3" );
				list.Add( 1072385, "3" );
				list.Add( 1072386, "3" );
				list.Add( 1060450, "3" );
				list.Add( 1073489, "100" );
				list.Add( 1060441 );
			}
		}

		public override bool OnEquip( Mobile from )
		{
			
			Item shirt = from.FindItemOnLayer( Layer.InnerTorso );
			Item pants = from.FindItemOnLayer( Layer.Pants );
			Item arms = from.FindItemOnLayer( Layer.Arms );
			
			if ( shirt != null && shirt.GetType() == typeof( AcolyteChest ) && pants != null && pants.GetType() == typeof( AcolyteLegs ) && arms != null && arms.GetType() == typeof( AcolyteArms ) )
			{
				Effects.PlaySound( from.Location, from.Map, 503 );
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );

				Hue = 0x2;
				ArmorAttributes.SelfRepair = 3;
				PhysicalBonus = 3;
				FireBonus = 3;
				ColdBonus = 3;
				PoisonBonus = 3;
				EnergyBonus = 3;

				AcolyteChest chest = from.FindItemOnLayer( Layer.InnerTorso ) as AcolyteChest;
				AcolyteLegs legs = from.FindItemOnLayer( Layer.Pants ) as AcolyteLegs;
				AcolyteArms arm = from.FindItemOnLayer( Layer.Arms ) as AcolyteArms;

				chest.Hue = 0x2;
				chest.Attributes.Luck = 100;
				chest.Attributes.NightSight = 1;
				chest.ArmorAttributes.SelfRepair = 3;
				chest.PhysicalBonus = 3;
				chest.FireBonus = 3;
				chest.ColdBonus = 3;
				chest.PoisonBonus = 3;
				chest.EnergyBonus = 3;

				legs.Hue = 0x2;
				legs.ArmorAttributes.SelfRepair = 3;
				legs.PhysicalBonus = 3;
				legs.FireBonus = 3;
				legs.ColdBonus = 3;
				legs.PoisonBonus = 3;
				legs.EnergyBonus = 3;

				arm.Hue = 0x2;
				arm.ArmorAttributes.SelfRepair = 3;
				arm.PhysicalBonus = 3;
				arm.FireBonus = 3;
				arm.ColdBonus = 3;
				arm.PoisonBonus = 3;
				arm.EnergyBonus = 3;
				
						
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
				ArmorAttributes.SelfRepair = 0;
				PhysicalBonus = 0;
				FireBonus = 0;
				ColdBonus = 0;
				PoisonBonus = 0;
				EnergyBonus = 0;
				if ( m.FindItemOnLayer( Layer.InnerTorso ) is AcolyteChest && m.FindItemOnLayer( Layer.Pants ) is AcolyteLegs && m.FindItemOnLayer( Layer.Arms ) is AcolyteArms )
				{
					AcolyteChest chest = m.FindItemOnLayer( Layer.InnerTorso ) as AcolyteChest;
					chest.Hue = 0x0;
					chest.Attributes.Luck = 0;
					chest.Attributes.NightSight = 0;
					chest.ArmorAttributes.SelfRepair = 0;
					chest.PhysicalBonus = 0;
					chest.FireBonus = 0;
					chest.ColdBonus = 0;
					chest.PoisonBonus = 0;
					chest.EnergyBonus = 0;


					AcolyteLegs legs = m.FindItemOnLayer( Layer.Pants ) as AcolyteLegs;
					legs.Hue = 0x0;
					legs.ArmorAttributes.SelfRepair = 0;
					legs.PhysicalBonus = 0;
					legs.FireBonus = 0;
					legs.ColdBonus = 0;
					legs.PoisonBonus = 0;
					legs.EnergyBonus = 0;
					
					AcolyteArms arm = m.FindItemOnLayer( Layer.Arms ) as AcolyteArms;
					arm.Hue = 0x0;
					arm.ArmorAttributes.SelfRepair = 0;
					arm.PhysicalBonus = 0;
					arm.FireBonus = 0;
					arm.ColdBonus = 0;
					arm.PoisonBonus = 0;
					arm.EnergyBonus = 0;

				}
				this.InvalidateProperties();
			}
			base.OnRemoved( parent );
		}
		
		public AcolyteGloves( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 2.0;
		}
	}
}