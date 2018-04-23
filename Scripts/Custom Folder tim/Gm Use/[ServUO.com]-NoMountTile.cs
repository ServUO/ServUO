using System; 
using Server; 
using Server.Items;
using Server.Gumps; 
using Server.Network; 
using Server.Menus; 
using Server.Mobiles;
using Server.Menus.Questions;

namespace Server.Items
{ 
	public class NoMountTile : Item 
	{ 

		[Constructable] 
		public NoMountTile() : base( 6108 ) 
		{ 
			Movable = false; 
			Name = "No Mount Tile"; 
			Visible = false;
		} 

		public override bool OnMoveOver( Mobile from )
		{
            if (from.Mounted)
            {
                //if (from.AccessLevel > AccessLevel.Player)
                    from.SendMessage("You can not be Mounted to Enter Here.");
                    return false;
            }
            else
            {
                return true;
            }
            return base.OnMoveOver( from );
		}
		public NoMountTile( Serial serial ) : base( serial ) 
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