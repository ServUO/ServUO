using System;
using Server;
using Server.Guilds;

namespace Server.Items
{
	public class ShieldofLordNast : BaseShield
	{
                public override int ArtifactRarity{ get{ return 5001; } }
		public override int BasePhysicalResistance{ get{ return 70; } }
		public override int BaseFireResistance{ get{ return 70; } }
		public override int BaseColdResistance{ get{ return 70; } }
		public override int BasePoisonResistance{ get{ return 70; } }
		public override int BaseEnergyResistance{ get{ return 70; } }

		public override int InitMinHits{ get{ return 561; } }
		public override int InitMaxHits{ get{ return 561; } }

		public override int AosStrReq{ get{ return 100; } }

                public override int AosIntReq{ get{ return 100; } }

                public override int AosDexReq{ get{ return 100; } }

		public override int ArmorBase{ get{ return 30; } }

		[Constructable]
		public ShieldofLordNast() : base( 0x1BC4 )
		{
			if ( !Core.AOS )
				
                                LootType = LootType.Blessed ;
                                Attributes.SpellChanneling = 1 ;
                                Name = "ShieldofLordNast" ;
                              Hue = 1161 ;

			Weight = 7.0;
		}

		public ShieldofLordNast( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 7.0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}

		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Validate( Parent as Mobile ) )
				base.OnSingleClick( from );
		}

		public virtual bool Validate( Mobile m )
		{
			if ( Core.AOS || m == null || !m.Player || m.AccessLevel != AccessLevel.Player )
				return true;

			Guild g = m.Guild as Guild;

			if ( g == null || g.Type != GuildType.Order )
			{
				m.FixedEffect( 0x3728, 10, 13 );
				Delete();

				return false;
			}

			return true;
		}
	}
}