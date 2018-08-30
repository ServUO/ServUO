using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x1452, 0x1457 )]
	public class GrizzleLegs : BaseArmor
	{
		public override int LabelNumber{ get{ return 1072119; } }
		public override int BasePhysicalResistance{ get{ return 6; } }
		public override int BaseFireResistance{ get{ return 10; } }
		public override int BaseColdResistance{ get{ return 5; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		public override int InitMinHits{ get{ return 25; } }
		public override int InitMaxHits{ get{ return 30; } }

		public override int AosStrReq{ get{ return 60; } }
		public override int OldStrReq{ get{ return 40; } }

		public override int ArmorBase{ get{ return 30; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Bone; } }


		[Constructable]
		public GrizzleLegs() : base( 0x1452 )
		{
			Weight = 7.0;
			Attributes.BonusHits = 5;
			ArmorAttributes.MageArmor = 1;

		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			list.Add( 1072376, "5" );
			
			if ( this.Parent is Mobile )
			{
				if ( this.Hue == 0x279 )
				{
					list.Add( 1072377 );
					list.Add( 1073493, "10" );
					list.Add( 1072514, "12" );
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
				list.Add( 1072383, "5" );
				list.Add( 1072384, "3" );
				list.Add( 1072385, "3" );
				list.Add( 1072386, "5" );
				list.Add( 1060450, "3" );
				
			}
		}

		public override bool OnEquip( Mobile from )
		{
			
			Item shirt = from.FindItemOnLayer( Layer.InnerTorso );
			Item glove = from.FindItemOnLayer( Layer.Gloves );
			Item arms = from.FindItemOnLayer( Layer.Arms );
			Item helm = from.FindItemOnLayer( Layer.Helm );
			
			if ( helm != null && helm.GetType() == typeof( GrizzleHelm ) && shirt != null && shirt.GetType() == typeof( GrizzleChest ) && glove != null && glove.GetType() == typeof( GrizzleGloves ) && arms != null && arms.GetType() == typeof( GrizzleArms ) )
			{
				Effects.PlaySound( from.Location, from.Map, 503 );
				from.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );

				Hue = 0x279;
				ArmorAttributes.SelfRepair = 3;
				PhysicalBonus = 4;
				FireBonus = 5;
				ColdBonus = 3;
				PoisonBonus = 4;
				EnergyBonus = 4;


				GrizzleChest chest = from.FindItemOnLayer( Layer.InnerTorso ) as GrizzleChest;
				GrizzleGloves gloves = from.FindItemOnLayer( Layer.Gloves ) as GrizzleGloves;
				GrizzleArms arm = from.FindItemOnLayer( Layer.Arms ) as GrizzleArms;
				GrizzleHelm helmet = from.FindItemOnLayer( Layer.Helm ) as GrizzleHelm;

				chest.Hue = 0x279;
				chest.Attributes.BonusStr = 12;
				chest.Attributes.DefendChance = 10;
				chest.ArmorAttributes.SelfRepair = 3;
				chest.Attributes.NightSight = 1;
				chest.PhysicalBonus = 3;
				chest.FireBonus = 5;
				chest.ColdBonus = 3;
				chest.PoisonBonus = 3;
				chest.EnergyBonus = 5;
				
				gloves.Hue = 0x279;
				gloves.ArmorAttributes.SelfRepair = 3;
				gloves.PhysicalBonus = 3;
				gloves.FireBonus = 5;
				gloves.ColdBonus = 3;
				gloves.PoisonBonus = 3;
				gloves.EnergyBonus = 5;

				arm.Hue = 0x279;
				arm.ArmorAttributes.SelfRepair = 3;
				arm.PhysicalBonus = 3;
				arm.FireBonus = 5;
				arm.ColdBonus = 3;
				arm.PoisonBonus = 3;
				arm.EnergyBonus = 5;

				helmet.Hue = 0x279;
				helmet.ArmorAttributes.SelfRepair = 3;
				helmet.PhysicalBonus = 3;
				helmet.FireBonus = 5;
				helmet.ColdBonus = 3;
				helmet.PoisonBonus = 3;
				helmet.EnergyBonus = 5;
						
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
				if ( m.FindItemOnLayer( Layer.Helm ) is GrizzleHelm && m.FindItemOnLayer( Layer.InnerTorso ) is GrizzleChest && m.FindItemOnLayer( Layer.Gloves ) is GrizzleGloves && m.FindItemOnLayer( Layer.Arms ) is GrizzleArms )
				{
					GrizzleChest chest = m.FindItemOnLayer( Layer.InnerTorso ) as GrizzleChest;
					chest.Hue = 0x0;
					chest.Attributes.BonusStr = 0;
					chest.Attributes.DefendChance = 0;
					chest.ArmorAttributes.SelfRepair = 0;
					chest.Attributes.NightSight = 0;
					chest.PhysicalBonus = 0;
					chest.FireBonus = 0;
					chest.ColdBonus = 0;
					chest.PoisonBonus = 0;
					chest.EnergyBonus = 0;

					GrizzleGloves gloves = m.FindItemOnLayer( Layer.Gloves ) as GrizzleGloves;
					gloves.Hue = 0x0;
					gloves.ArmorAttributes.SelfRepair = 0;
					gloves.PhysicalBonus = 0;
					gloves.FireBonus = 0;
					gloves.ColdBonus = 0;
					gloves.PoisonBonus = 0;
					gloves.EnergyBonus = 0;
					
					GrizzleArms arm = m.FindItemOnLayer( Layer.Arms ) as GrizzleArms;
					arm.Hue = 0x0;
					arm.ArmorAttributes.SelfRepair = 0;
					arm.PhysicalBonus = 0;
					arm.FireBonus = 0;
					arm.ColdBonus = 0;
					arm.PoisonBonus = 0;
					arm.EnergyBonus = 0;

					GrizzleHelm helmet = m.FindItemOnLayer( Layer.Helm ) as GrizzleHelm;
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

		public GrizzleLegs( Serial serial ) : base( serial )
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
		}
	}
}