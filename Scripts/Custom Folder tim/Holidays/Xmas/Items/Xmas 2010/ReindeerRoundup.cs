using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{	
	public class ReindeerRoundup : BaseQuest
	{					
		
		/* ReindeerRoundup */
		public override object Title{ get{ return "Reindeer Roundup"; } }
		
		public override object Description
		{ 
			get
			{ 
				return 
					"*the reindeer look worried*<BR><BR>" +
					"I have only recovered one present for Santa and time is running short.<BR><BR>" +
					"Please take me to his workshop so we can deliver these presents to the children!"; } }
		
		public override object Refuse{ get{ return "I guess I'll continue looking for the lost presents."; } }
		
		public override object Uncomplete{ get{ return "Quickly, we must hurry back to the workshop."; } }
				
		public ReindeerRoundup() : base()
		{								
			AddObjective( new EscortObjective( "Santas' Workshop" ) );
						
			AddReward( new BaseReward( typeof( PlayerGiftBox2010 ), "A Lost Present" ) );
		}		
		
		public override void GiveRewards()
		{			
			base.GiveRewards();
			
			Owner.SendMessage( "You receive a Present for returning one of the reindeer back to Santas' Workshop.", null, 0x23 ); 
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
