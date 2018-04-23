using System;
using Server;
using Server.Mobiles;
using Server.Engines.VeteranRewards;
using Server.Network;

namespace Server.Items
{
    public class BackpackMaxWeightIncrease : Item
	{
		// Maximum bonus these deeds can give is...
		public const int MaxIncreaseCap = 2500;

		private int _AmountToIncrese;
		
		[CommandProperty(AccessLevel.GameMaster)]
        public int AmountToIncrese
        {
            get
            {
                return _AmountToIncrese;
            }
            set
            {
                _AmountToIncrese = value;
            }
        }
		
		[Constructable]
		public BackpackMaxWeightIncrease() : base( 0x14F0 )
		{
			Name = "Backpack Max Weight Increase Deed";
			_AmountToIncrese = 50; // set the weight increase here
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			list.Add( "Increases cap by {0}", _AmountToIncrese );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.Backpack == null || Parent != from.Backpack )
			{
				from.SendLocalizedMessage( 1080058 ); // This must be in your backpack to use it.
			}
			else if ( from.Backpack != null && !from.Backpack.Deleted )
			{
				Backpack pack = (from.FindItemOnLayer(Layer.Backpack) as Backpack);
				
				if( pack.NewMaxWeight + AmountToIncrese <= MaxIncreaseCap )
				{
					pack.NewMaxWeight += AmountToIncrese;
					this.Delete();
					from.SendMessage("You have increased the Max Weight cap of your backpack!");
				}
				else
					from.SendMessage("The Max Weight cap of your backpack cannot be increased any further!");
			}
			
		}

		public BackpackMaxWeightIncrease( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			
			writer.Write( (int)_AmountToIncrese);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			_AmountToIncrese = reader.ReadInt();
		}
	}
}