/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 12/18/2010
 * Time: 5:03 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
	public class SantasQuest : BaseQuest
	{

		/* Santa's Helpers */
		public override object Title
		{ 
			get
			{ 
				return "Santa's Helpers"; 
			} 
		}
		
		public override object Description
		{ 
			get
			{ 
				return 
					"<I>*Santa looks a bit worried as you approach*</I><BR><BR>" +
					"Well, welcome traveler to my workshop! Tis the season to be jolly but I am in a little trouble.<BR><BR>" +
					"It seams that Rudolph took my sleigh for a joy ride and had a crash landing.<BR>" +
					"The holidays are just a short time away and all the presents for the children are scattered about and my reindeer are still out there gathering up the presents from the crash!!!<BR><BR>" +
					"If that evil Grinch finds out, he might keep them for himself! They were all wrapped and ready to go.<BR><BR>" +
					"You look trustworthy and I am sure you wouldn't peek inside any of those gifts. Would you??<BR><BR>" +
					"I have repaired the sleigh. If you could get those presents back here to my workshop and return some of my reindeer that would save the Holidays!";
					
			} 
		}
		public override object Refuse{ get{ return "<I>*sigh*</I> I understand. But I dont think all the children will."; } }
		
	
		public override object Uncomplete{ get{ return "Please hurry! Time is running out."; } }
		

		public override object Complete
		{ get
			{ 
				return 
					"<I>*Santa takes the presents and places them in his sleigh*</I><BR><BR>" +
					"You have saved Christmas and now children everywhere will be able to have a wonderful holiday!<BR><BR>" +
					"<I>*HO HO HO!!*</I> Happy Holidays my friend!";
			} 
		}
		
		public SantasQuest() : base()
		{					

			AddObjective( new ObtainObjective( typeof( PlayerGiftBox2010 ), "Lost Christmas Present", 10, 0x232A ) );	
			AddReward( new BaseReward( typeof( HolidayRobe ), "Santa's Gratitude" ) ); // Holiday Robe
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
	
	public class SantasQuester : MondainQuester
	{
		public override Type[] Quests
		{ 
			get{ return new Type[] 
			{ 
				typeof( SantasQuest )
			};} 
		}
		
		[Constructable]
		public SantasQuester() : base( "Santa", "the Jolly Ol' Fat Man" )
		{			
		}
		
		public SantasQuester( Serial serial ) : base( serial )
		{
		}		
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Female = false;
			Race = Race.Human;
			
			Hue = 0x83F8;			
			HairItemID = 0x203C;
      		HairHue = 1151;
      
			FacialHairItemID = 0x204B;
      		FacialHairHue = 1151;
		}
		
		public override void InitOutfit()
		{
			AddItem( new Backpack() );			
			AddItem( new FancyShirt( 32 )  );
      		AddItem( new Surcoat( 32 )  );
			AddItem( new LongPants( 32 )  );
      		AddItem( new FurCape( 1175 )  );
      		AddItem( new SantasElfBoots() );
			

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

