using System;

namespace Server.Items
{
	public class DailyFruitBasket : BaseDailyRareFood
	{
		public override int ArtifactRarity{ get{ return 0; } }
		
		[Constructable]
		public DailyFruitBasket() : base( 1, 0x993 )
		{
			FillFactor = 5;
			Stackable = false;
		}

		public DailyFruitBasket( Serial serial ) : base( serial )
		{
		}

		public override bool Eat( Mobile from )
		{
			int x = this.X;
			int y = this.Y;
			int z = this.Z;
			Map map = this.Map;

			if ( !base.Eat( from ) )
				return false;

			if ( !IsChildOf (from.Backpack) )
			{
				Basket b = new Basket();
				b.MoveToWorld( new Point3D( x, y, z ), map );
				return true;
			}

			if ( IsChildOf (from.Backpack) )
			{
				from.AddToBackpack( new Basket() );
				return true;
			}

			return true;
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Daily Rare" );
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