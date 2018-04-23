using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x13CD, 0x13C5 )]
	public class DeathEssenceArms : BaseArmor
	{
		public override int LabelNumber{ get{ return 1074305; } }
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
		public DeathEssenceArms() : base( 0x13CD )
		{
			Weight = 5.0;
			Attributes.RegenMana = 1;
			Attributes.RegenHits = 1;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1072376, "5" );
			
			if ( this.Parent is Mobile )
			{
				if ( this.Hue == 0x455 )
				{
					list.Add( 1072377 );
					list.Add( 1073488, "10" );
					list.Add( "Necromancy 10 (total)" );
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
				list.Add( 1073488, "10" );
				list.Add( "Necromancy 10 (total)" );
			}
		}

		public override bool OnEquip( Mobile from )
		{
			
			Item shirt = from.FindItemOnLayer( Layer.InnerTorso );
			Item glove = from.FindItemOnLayer( Layer.Gloves );
			Item pants = from.FindItemOnLayer( Layer.Pants );
			Item helm = from.FindItemOnLayer( Layer.Helm );
			
			if ( helm != null && helm.GetType()== typeof( DeathEssenceHelm ) && shirt != null && shirt.GetType() == typeof( DeathEssenceChest ) && glove != null && glove.GetType() == typeof( DeathEssenceGloves ) && pants != null && pants.GetType() == typeof( DeathEssenceLegs ) )
			{
				Effects.PlaySound( from.Location, from.Map, 503 );
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );

				Hue = 0x455;
				ArmorAttributes.SelfRepair = 3;
				PhysicalBonus = 4;
				FireBonus = 5;
				ColdBonus = 3;
				PoisonBonus = 4;
				EnergyBonus = 4;

				DeathEssenceChest chest = from.FindItemOnLayer( Layer.InnerTorso ) as DeathEssenceChest;
				DeathEssenceGloves gloves = from.FindItemOnLayer( Layer.Gloves ) as DeathEssenceGloves;
				DeathEssenceLegs legs = from.FindItemOnLayer( Layer.Pants ) as DeathEssenceLegs;
				DeathEssenceHelm helmet = from.FindItemOnLayer( Layer.Helm ) as DeathEssenceHelm;

				chest.Hue = 0x455;
				chest.SkillBonuses.SetValues( 0, SkillName.Necromancy, 10.0 );
				chest.Attributes.LowerManaCost = 10;
				chest.ArmorAttributes.SelfRepair = 3;
				chest.PhysicalBonus = 4;
				chest.FireBonus = 5;
				chest.ColdBonus = 3;
				chest.PoisonBonus = 4;
				chest.EnergyBonus = 4;

				gloves.Hue = 0x455;
				gloves.ArmorAttributes.SelfRepair = 3;
				gloves.PhysicalBonus = 4;
				gloves.FireBonus = 5;
				gloves.ColdBonus = 3;
				gloves.PoisonBonus = 4;
				gloves.EnergyBonus = 4;

				legs.Hue = 0x455;
				legs.ArmorAttributes.SelfRepair = 3;
				legs.PhysicalBonus = 4;
				legs.FireBonus = 5;
				legs.ColdBonus = 3;
				legs.PoisonBonus = 4;
				legs.EnergyBonus = 4;

				helmet.Hue = 0x455;
				helmet.ArmorAttributes.SelfRepair = 3;
				helmet.PhysicalBonus = 4;
				helmet.FireBonus = 5;
				helmet.ColdBonus = 3;
				helmet.PoisonBonus = 4;
				helmet.EnergyBonus = 4;

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
				if ( m.FindItemOnLayer( Layer.Helm ) is DeathEssenceHelm && m.FindItemOnLayer( Layer.InnerTorso ) is DeathEssenceChest && m.FindItemOnLayer( Layer.Gloves ) is DeathEssenceGloves && m.FindItemOnLayer( Layer.Pants ) is DeathEssenceLegs )
				{
					DeathEssenceChest chest = m.FindItemOnLayer( Layer.InnerTorso ) as DeathEssenceChest;
					chest.Hue = 0x0;
					chest.SkillBonuses.SetValues( 0, SkillName.Necromancy, 0.0 );
					chest.Attributes.LowerManaCost = 0;
					chest.ArmorAttributes.SelfRepair = 0;
					chest.PhysicalBonus = 0;
					chest.FireBonus = 0;
					chest.ColdBonus = 0;
					chest.PoisonBonus = 0;
					chest.EnergyBonus = 0;

					DeathEssenceGloves gloves = m.FindItemOnLayer( Layer.Gloves ) as DeathEssenceGloves;
					gloves.Hue = 0x0;
					gloves.ArmorAttributes.SelfRepair = 0;
					gloves.PhysicalBonus = 0;
					gloves.FireBonus = 0;
					gloves.ColdBonus = 0;
					gloves.PoisonBonus = 0;
					gloves.EnergyBonus = 0;
					
					DeathEssenceLegs legs = m.FindItemOnLayer( Layer.Pants ) as DeathEssenceLegs;
					legs.Hue = 0x0;
					legs.ArmorAttributes.SelfRepair = 0;
					
					legs.PhysicalBonus = 0;
					legs.FireBonus = 0;
					legs.ColdBonus = 0;
					legs.PoisonBonus = 0;
					legs.EnergyBonus = 0;

					DeathEssenceHelm helmet = m.FindItemOnLayer( Layer.Helm ) as DeathEssenceHelm;
					helmet.Hue = 0x0;
					helmet.ArmorAttributes.SelfRepair = 0;
					
					helmet.PhysicalBonus = 0;
					helmet.FireBonus = 0;
					helmet.ColdBonus = 0;
					helmet.PoisonBonus = 0;
					helmet.EnergyBonus = 0;

				}
				this.InvalidateProperties();
			}
			base.OnRemoved( parent );
		}


		public DeathEssenceArms( Serial serial ) : base( serial )
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
				Weight = 5.0;
		}
	}
}