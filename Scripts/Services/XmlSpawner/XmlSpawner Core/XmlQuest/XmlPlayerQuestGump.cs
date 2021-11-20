using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
using Server.Engines.XmlSpawner2;

//
// XmlPlayerQuestGump
//

namespace Server.Gumps
{
	public class XmlPlayerQuestGump : Gump
	{
		private PlayerMobile m_From;
		private IXmlQuest m_QuestItem;

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
            if(info == null || sender == null || sender.Mobile == null) return;

            // read the text entries for the search criteria
      	    TextRelay tr = info.GetTextEntry( 100 );        // quest name
      	    if(tr != null)
                m_QuestItem.Name = tr.Text.Trim();

      	    tr = info.GetTextEntry( 102 );        // title
      	    if(tr != null)
                m_QuestItem.TitleString  = tr.Text.Trim();

      	    tr = info.GetTextEntry( 103 );        // notestring
      	    if(tr != null)
                m_QuestItem.NoteString = tr.Text;

      	    tr = info.GetTextEntry( 200 );        // objectives
      	    if(tr != null)
                m_QuestItem.Objective1 = tr.Text.Trim();

      	    tr = info.GetTextEntry( 201 );
      	    if(tr != null)
                m_QuestItem.Objective2 = tr.Text.Trim();

      	    tr = info.GetTextEntry( 202 );
      	    if(tr != null)
      	    m_QuestItem.Objective3 = tr.Text.Trim();

      	    tr = info.GetTextEntry( 203 );
			if (tr != null)
                m_QuestItem.Objective4 = tr.Text.Trim();

      	    tr = info.GetTextEntry( 204 );
      	    if(tr != null)
                m_QuestItem.Objective5 = tr.Text.Trim();

            tr = info.GetTextEntry( 205 );
            if(tr != null && tr.Text != null && tr.Text.Length > 0)       // descriptions
      	       m_QuestItem.Description1 = tr.Text.Trim();
      	    else
      	       m_QuestItem.Description1 = null;

      	    tr = info.GetTextEntry( 206 );
      	    if(tr != null && tr.Text != null && tr.Text.Length > 0)
      	       m_QuestItem.Description2 = tr.Text.Trim();
      	    else
      	       m_QuestItem.Description2 = null;

      	    tr = info.GetTextEntry( 207 );
      	    if(tr != null && tr.Text != null && tr.Text.Length > 0)
      	       m_QuestItem.Description3 = tr.Text.Trim();
      	    else
      	       m_QuestItem.Description3 = null;

      	    tr = info.GetTextEntry( 208 );
      	    if(tr != null && tr.Text != null && tr.Text.Length > 0)
      	       m_QuestItem.Description4 = tr.Text.Trim();
      	    else
      	       m_QuestItem.Description4 = null;

      	    tr = info.GetTextEntry( 209 );
      	    if(tr != null && tr.Text != null && tr.Text.Length > 0)
      	       m_QuestItem.Description5 = tr.Text.Trim();
      	    else
      	       m_QuestItem.Description5 = null;

      	    tr = info.GetTextEntry( 210 );         // expiration
      	    if(tr != null && tr.Text != null && tr.Text.Length > 0){
                  try{m_QuestItem.Expiration = double.Parse(tr.Text.Trim());} catch{}
              }

      	    // check all of the check boxes
  			m_QuestItem.PartyEnabled = info.IsSwitched(300);
  			m_QuestItem.CanSeeReward = info.IsSwitched(301);
  			
  			// refresh the time created
  			m_QuestItem.TimeCreated = DateTime.UtcNow;


			switch ( info.ButtonID )
			{
				case 0: // Okay
				{

					break;
				}
				case 1: // Select Reward
				{
				    sender.Mobile.Target = new RewardTarget(m_QuestItem);
				    break;
				}
				case 2: // Select Reward Return
				{
				    sender.Mobile.Target = new ReturnTarget(m_QuestItem);
				    break;
				}

			}
		}


		public XmlPlayerQuestGump( PlayerMobile from, IXmlQuest questitem ) : base( 12, 140 )
		{

			from.CloseGump( typeof( XmlPlayerQuestGump ) );

			if(from == null || from.Deleted || questitem == null || questitem.Deleted) return;

			m_From = from;
			m_QuestItem = questitem;

			int width = 600;

			//width = 516;

			X = (624 - width) / 2;

			AddPage( 0 );

			AddBackground( 10, 10, width, 439, 5054 );
			AddImageTiled( 18, 20, width - 17, 420, 2624 );

			AddAlphaRegion( 18, 20, width - 17, 420 );
			AddImage( 5, 5, 10460 );
			AddImage( width - 15, 5, 10460 );
			AddImage( 5, 424, 10460 );
			AddImage( width - 15, 424, 10460 );

			// add the Quest Title
            AddLabel( width/2 - 50, 15, 0x384, "Player Quest Maker" );
			
			int y = 35;

			// add the Quest Name
            AddLabel( 28, y, 0x384, "Quest Name" );
            string name = questitem.Name;
            if(name != null)
            {
                name = name.Substring(4);
            }
            AddImageTiled( 26, y + 20, 232, 20, 0xBBC );
            AddTextEntry( 26, y + 20, 250, 20, 0, 100, name );

            // add the Quest Title
            AddLabel( 328, y, 0x384, "Quest Title" );
            AddImageTiled( 306, y + 20, 232, 20, 0xBBC );
            AddTextEntry( 306, y + 20, 230, 20, 0, 102, questitem.TitleString );

            y += 50;
            // add the Quest Text
            AddLabel( 28, y, 0x384, "Quest Text" );
            AddImageTiled( 26, y + 20, 532, 80, 0xBBC );
            AddTextEntry( 26, y + 20, 530, 80, 0, 103, questitem.NoteString );

            y += 110;
            // add the Quest Expiration
            AddLabel( 28, y, 0x384, "Expiration" );
            AddLabel( 98, y + 20, 0x384, "Hours" );
            AddImageTiled( 26, y + 20, 52, 20, 0xBBC );
            AddTextEntry( 26, y + 20, 50, 20, 0, 210, questitem.Expiration.ToString() );

            y += 50;
            // add the Quest Objectives
            AddLabel( 28, y, 0x384, "Quest Objectives" );

            AddImageTiled( 26, y + 20, 252, 19, 0xBBC );
            AddTextEntry( 26, y + 20, 250, 19, 0, 200, questitem.Objective1 );

            AddImageTiled( 26, y + 40, 252, 19, 0xBBC );
            AddTextEntry( 26, y + 40, 250, 19, 0, 201, questitem.Objective2 );

            AddImageTiled( 26, y + 60, 252, 19, 0xBBC );
            AddTextEntry( 26, y + 60, 250, 19, 0, 202, questitem.Objective3 );

            AddImageTiled( 26, y + 80, 252, 19, 0xBBC );
            AddTextEntry( 26, y + 80, 250, 19, 0, 203, questitem.Objective4 );

            AddImageTiled( 26, y + 100, 252, 19, 0xBBC );
            AddTextEntry( 26, y + 100, 250, 19, 0, 204, questitem.Objective5 );

            // add the Quest Objectives
            AddLabel( 328, y, 0x384, "Objective Descriptions" );
            AddImageTiled( 306, y + 20, 252, 19, 0xBBC );
            AddTextEntry( 306, y + 20, 250, 19, 0, 205, questitem.Description1 );

            AddImageTiled( 306, y + 40, 252, 19, 0xBBC );
            AddTextEntry( 306, y + 40, 250, 19, 0, 206, questitem.Description2 );

            AddImageTiled( 306, y + 60, 252, 19, 0xBBC );
            AddTextEntry( 306, y + 60, 250, 19, 0, 207, questitem.Description3 );

            AddImageTiled( 306, y + 80, 252, 19, 0xBBC );
            AddTextEntry( 306, y + 80, 250, 19, 0, 208, questitem.Description4 );

            AddImageTiled( 306, y + 100, 252, 19, 0xBBC );
            AddTextEntry( 306, y + 100, 250, 19, 0, 209, questitem.Description5 );

            y += 130;
            // party enable toggle
            AddCheck( 25, y, 0xD2, 0xD3, questitem.PartyEnabled, 300);
            AddLabel( 48, y, 0x384, "PartyEnabled" );
            y += 20;
            // can see toggle
            AddCheck( 25, y, 0xD2, 0xD3, questitem.CanSeeReward, 301);
            AddLabel( 48, y, 0x384, "CanSeeReward" );
            
            // select reward button
            AddButton( 225, y+3, 2103, 2103, 1, GumpButtonType.Reply, 0 );
            AddLabel( 245, y, 0x384, "Select Reward" );

            // select reward button
            AddButton( 375, y+3, 2103, 2103, 2, GumpButtonType.Reply, 0 );
            AddLabel( 395, y, 0x384, "Select Return Container" );


            AddButton( 45, 416, 2130, 2129, 0, GumpButtonType.Reply, 0 ); // Okay button

			//AddButton( 375 - xoffset, 416, 4017, 4018, 0, GumpButtonType.Reply, 0 );

			//AddHtmlLocalized( 410 - xoffset, 416, 120, 20, 1011441, LabelColor, false, false ); // EXIT

		}
		
		private class RewardTarget : Target
        {
            IXmlQuest m_QuestItem;

            public RewardTarget(IXmlQuest questitem) :  base ( 30, true, TargetFlags.None )
            {
                m_QuestItem = questitem;

            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if(m_QuestItem == null || m_QuestItem.Deleted) return;

                // first check to see if you are too far from the return container.  This is to avoid exploits involving targeting a container
                // then using the return reward feature as a free transport of items back to that container
                if(m_QuestItem.ReturnContainer != null && !m_QuestItem.ReturnContainer.Deleted)
                {
                    Point3D returnloc;

                    if(m_QuestItem.ReturnContainer.Parent == null)
                    {
                        returnloc = m_QuestItem.ReturnContainer.Location;
                    } else
                    if(m_QuestItem.ReturnContainer.RootParent != null)
                    {

                        returnloc = ((IEntity)m_QuestItem.ReturnContainer.RootParent).Location;
                    } else
                    {
                        from.SendMessage("Invalid container location");
                        return;
                    }

                    if(!Utility.InRange( returnloc, from.Location, 10))
                    {
                        // out of range
                        from.SendMessage("Too far away from the reward return container");
                        return;
                    }
                }
                // try to add the item as the reward item
                if(m_QuestItem.PlayerMade && (from != null) && !from.Deleted && (from is PlayerMobile) && 
                (from == m_QuestItem.Creator) && (from == m_QuestItem.Owner) && (targeted is Item) && 
                !(targeted is IXmlQuest))
                {
                    Item i = targeted as Item;

                    // make sure the target item is in the oweners backpack
                    if(i != null && !i.Deleted && i.RootParent == m_QuestItem.Owner)
                    {
                        m_QuestItem.RewardItem = i;
                        m_QuestItem.AutoReward = true;
                    } else
                    {
                        from.SendMessage("Targeted item must be in the owners pack");
                    }
                }
            }
        }
        
        private class ReturnTarget : Target
        {
            IXmlQuest m_QuestItem;

            public ReturnTarget(IXmlQuest questitem) :  base ( 30, true, TargetFlags.None )
            {
                m_QuestItem = questitem;

            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if(m_QuestItem == null || m_QuestItem.Deleted) return;

                // try to add the item as the reward item
                if(m_QuestItem.PlayerMade && (from != null) && !from.Deleted && (from is PlayerMobile) && 
                (from == m_QuestItem.Creator) && (from == m_QuestItem.Owner) && targeted is Container)
                {
                    Container i = targeted as Container;

                    // make sure the target item is in the oweners backpack
                    if(i != null && !i.Deleted )
                    {
                        m_QuestItem.ReturnContainer = i;
                        from.SendMessage("Reward return container set");

                    } else
                    {
                        from.SendMessage("Targeted item must be a valid container");
                    }
                }
            }
        }
	}
}

