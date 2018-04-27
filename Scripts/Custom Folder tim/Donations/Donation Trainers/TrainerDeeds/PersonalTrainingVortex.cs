using System; 
using System.Collections;
using Server.Items; 
using Server.Mobiles; 
using Server.Misc;
using Server.Network;

namespace Server.Items
{

	public class PersonalVortex : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public PersonalVortex () : base( 0x14F0 )
		{
			Weight = 1.0;
            Name = "A Personal Trainer Vortex Deed";
			LootType = LootType.Blessed;
		}

        public PersonalVortex(Serial serial)
            : base(serial)
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				this.Delete();
				from.SendMessage( "You created your Personal Trainer" );

                TrainingVortex cow = new TrainingVortex();
         			cow.Map = from.Map; 
         			cow.Location = from.Location; 
				cow.Controlled = false;
	
			}
		}	
	}
}


