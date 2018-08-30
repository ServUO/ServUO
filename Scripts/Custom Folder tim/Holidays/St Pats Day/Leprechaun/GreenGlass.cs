using System;
using Server;
using Server.Network;
using Server.Targeting;
using Server.Engines.Plants;
using System.Collections;
using Server.Mobiles;

namespace Server.Items
{
    public class GreenGlass : BaseBeverage
	{
		public override int EmptyLabelNumber{ get{ return 1022456; } } // mug
		public override int BaseLabelNumber{ get{ return 1042976; } } // a mug of Ale
		public override int MaxQuantity{ get{ return 5; } }

		public override int ComputeItemID()
		{
			if ( IsEmpty )
				return (ItemID >= 0x1F81 && ItemID <= 0x1F84 ? ItemID : 0x1F81);

			switch ( Content )
			{
				case BeverageType.Ale: return (ItemID == 0x9EF ? 0x9EF : 0x9EE);
				case BeverageType.Cider: return (ItemID >= 0x1F7D && ItemID <= 0x1F80 ? ItemID : 0x1F7D);
				case BeverageType.Liquor: return (ItemID >= 0x1F85 && ItemID <= 0x1F88 ? ItemID : 0x1F85);
				case BeverageType.Milk: return (ItemID >= 0x1F89 && ItemID <= 0x1F8C ? ItemID : 0x1F89);
				case BeverageType.Wine: return (ItemID >= 0x1F8D && ItemID <= 0x1F90 ? ItemID : 0x1F8D);
				case BeverageType.Water: return (ItemID >= 0x1F91 && ItemID <= 0x1F94 ? ItemID : 0x1F91);
			}

			return 0;
		}

		[Constructable]
		public GreenGlass()
		{
			Weight = 1.0;
			Hue = 52;
		}

		[Constructable]
		public GreenGlass( BeverageType type ) : base( type )
		{
			Weight = 1.0;
		}

		public GreenGlass( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					if ( CheckType( "MugAle" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Ale;
					}
					else if ( CheckType( "GlassCider" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Cider;
					}
					else if ( CheckType( "GlassLiquor" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Liquor;
					}
					else if ( CheckType( "GlassMilk" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Milk;
					}
					else if ( CheckType( "GlassWine" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Wine;
					}
					else if ( CheckType( "GlassWater" ) )
					{
						Quantity = MaxQuantity;
						Content = BeverageType.Water;
					}
					else
					{
						throw new Exception( World.LoadingType );
					}

					break;
				}
			}
		}
	}
}
