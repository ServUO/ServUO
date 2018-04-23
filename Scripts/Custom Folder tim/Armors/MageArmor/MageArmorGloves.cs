using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2B75, 0x316C )]
	public class MageArmorGloves : BaseArmor
	{
		public override int LabelNumber{ get{ return 1074299; } } 
		public override int BasePhysicalResistance{ get{ return 4; } }
		public override int BaseFireResistance{ get{ return 9; } }
		public override int BaseColdResistance{ get{ return 6; } }
		public override int BasePoisonResistance{ get{ return 3; } }
		public override int BaseEnergyResistance{ get{ return 8; } }

		public override int InitMinHits{ get{ return 30; } }
		public override int InitMaxHits{ get{ return 40; } }

		public override int AosStrReq{ get{ return 20; } }
		public override int OldStrReq{ get{ return 15; } }

		public override int ArmorBase{ get{ return 13; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Leather; } }
		public override ArmorMeditationAllowance DefMedAllowance{ get{ return ArmorMeditationAllowance.All; } }


		[Constructable]
		public MageArmorGloves() : base( 0x2B75 )
		{
			Weight = 2.0;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1072376, "4" );
			
			if ( this.Parent is Mobile )
			{
				if ( this.Hue == 0x47E )
				{
					list.Add( 1072377 );
					list.Add( 1072380, "15" );
					list.Add( 1072381, "10" );
				}
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( this.Hue == 0x0 )
			{
				list.Add( 1072378 );
				list.Add( 1072382, "4" );
				list.Add( 1072383, "5" );
				list.Add( 1072384, "3" );
				list.Add( 1072385, "4" );
				list.Add( 1072386, "4" );
				list.Add( 1060450, "3" );
				list.Add( 1072380, "15" );
				list.Add( 1072381, "10" );
			}
		}

		public override bool OnEquip( Mobile from )
		{
			
			Item shirt = from.FindItemOnLayer( Layer.InnerTorso );
			Item pants = from.FindItemOnLayer( Layer.Pants );
			Item arms = from.FindItemOnLayer( Layer.Arms );
			
			if ( shirt != null && shirt.GetType() == typeof( MageArmorChest ) && pants != null && pants.GetType() == typeof( MageArmorLegs ) && arms != null && arms.GetType() == typeof( MageArmorArms ) )
			{
				Effects.PlaySound( from.Location, from.Map, 503 );
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );

				Hue = 0x47E;
				ArmorAttributes.SelfRepair = 3;
				PhysicalBonus = 4;
				FireBonus = 5;
				ColdBonus = 3;
				PoisonBonus = 4;
				EnergyBonus = 4;

				MageArmorChest chest = from.FindItemOnLayer( Layer.InnerTorso ) as MageArmorChest;
				MageArmorLegs legs = from.FindItemOnLayer( Layer.Pants ) as MageArmorLegs;
				MageArmorArms arm = from.FindItemOnLayer( Layer.Arms ) as MageArmorArms;

				chest.Hue = 0x47E;
				chest.Attributes.BonusInt = 10;
				chest.Attributes.SpellDamage = 15;
				chest.ArmorAttributes.SelfRepair = 3;
				chest.PhysicalBonus = 4;
				chest.FireBonus = 5;
				chest.ColdBonus = 3;
				chest.PoisonBonus = 4;
				chest.EnergyBonus = 4;

				legs.Hue = 0x47E;
				legs.ArmorAttributes.SelfRepair = 3;
				legs.PhysicalBonus = 4;
				legs.FireBonus = 5;
				legs.ColdBonus = 3;
				legs.PoisonBonus = 4;
				legs.EnergyBonus = 4;

				arm.Hue = 0x47E;
				arm.ArmorAttributes.SelfRepair = 3;
				arm.PhysicalBonus = 4;
				arm.FireBonus = 5;
				arm.ColdBonus = 3;
				arm.PoisonBonus = 4;
				arm.EnergyBonus = 4;
				
						
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
				if ( m.FindItemOnLayer( Layer.InnerTorso ) is MageArmorChest && m.FindItemOnLayer( Layer.Pants ) is MageArmorLegs && m.FindItemOnLayer( Layer.Arms ) is MageArmorArms )
				{
					MageArmorChest chest = m.FindItemOnLayer( Layer.InnerTorso ) as MageArmorChest;
					chest.Hue = 0x0;
					chest.Attributes.BonusInt = 0;
					chest.Attributes.SpellDamage = 0;
					chest.ArmorAttributes.SelfRepair = 0;
					chest.PhysicalBonus = 0;
					chest.FireBonus = 0;
					chest.ColdBonus = 0;
					chest.PoisonBonus = 0;
					chest.EnergyBonus = 0;


					MageArmorLegs legs = m.FindItemOnLayer( Layer.Pants ) as MageArmorLegs;
					legs.Hue = 0x0;
					legs.ArmorAttributes.SelfRepair = 0;
					legs.PhysicalBonus = 0;
					legs.FireBonus = 0;
					legs.ColdBonus = 0;
					legs.PoisonBonus = 0;
					legs.EnergyBonus = 0;
					
					MageArmorArms arm = m.FindItemOnLayer( Layer.Arms ) as MageArmorArms;
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
		
		public MageArmorGloves( Serial serial ) : base( serial )
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