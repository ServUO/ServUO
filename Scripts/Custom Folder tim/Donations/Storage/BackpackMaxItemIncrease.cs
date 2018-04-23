using System;
using Server;
using Server.Mobiles;
using Server.Engines.VeteranRewards;
using Server.Network;

namespace Server.Items
{
    public class BackpackMaxItemIncrease : Item
	{
		// Maximum bonus these deeds can give is...
		public const int MaxIncreaseCap = 1000;

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
		public BackpackMaxItemIncrease() : base( 0x14F0 )
		{
			Name = "Backpack Max Item Increase Deed";
			_AmountToIncrese = 25; // set the increase in items here
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
			else if ( from.Backpack != null && !from.Backpack.Deleted && from.Backpack.MaxItems + AmountToIncrese <= MaxIncreaseCap )
			{
				Container pack = (from.FindItemOnLayer(Layer.Backpack) as Container);
                pack.MaxItems += AmountToIncrese;
				
				this.Delete();
				from.SendMessage("You have increased the Max Items cap of your backpack!");
			}
			else
				from.SendMessage("The Max Items cap of your backpack cannot be increased any further!");
		}

		public BackpackMaxItemIncrease( Serial serial ) : base( serial )
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