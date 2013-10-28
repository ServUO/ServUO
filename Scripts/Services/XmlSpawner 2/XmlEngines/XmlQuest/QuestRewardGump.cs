using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;

/*
** QuestRewardGump
** ArteGordon
** updated 9/18/05
**
** Gives out rewards based on the XmlQuestReward reward list entries and the players Credits that are accumulated through quests with the XmlQuestPoints attachment.
** The Gump supports Item, Mobile, and Attachment type rewards.
*/
namespace Server.Gumps 
{ 
    public class QuestRewardGump : Gump
    {
        private readonly ArrayList Rewards;
        private readonly int y_inc = 35;
        private readonly int x_creditoffset = 350;
        private readonly int x_pointsoffset = 480;
        private readonly int maxItemsPerPage = 9;
        private readonly int viewpage;
        public QuestRewardGump(Mobile from, int page)
            : base(20, 30)
        { 
            from.CloseGump(typeof(QuestRewardGump));

            // determine the gump size based on the number of rewards
            this.Rewards = XmlQuestPointsRewards.RewardsList;
            
            this.viewpage = page;
            
            int height = this.maxItemsPerPage * this.y_inc + 120;
            int width = this.x_pointsoffset + 110;

            /*
            if(Rewards != null && Rewards.Count > 0)
            {
            height = Rewards.Count*y_inc + 120;
            }
            */

            this.AddBackground(0, 0, width, height, 0xDAC);

            this.AddHtml(40, 20, 350, 50, "Rewards Available for Purchase with QuestPoints Credits", false, false);
			
            this.AddLabel(400, 20, 0, String.Format("Available Credits: {0}", XmlQuestPoints.GetCredits(from)));

            //AddButton( 30, height - 35, 0xFB7, 0xFB9, 0, GumpButtonType.Reply, 0 );
            //AddLabel( 70, height - 35, 0, "Close" );
 
            // put the page buttons in the lower right corner
            if (this.Rewards != null && this.Rewards.Count > 0)
            {
                this.AddLabel(width - 165, height - 35, 0, String.Format("Page: {0}/{1}", this.viewpage + 1, (int)(this.Rewards.Count / this.maxItemsPerPage) + 1));

                // page up and down buttons
                this.AddButton(width - 55, height - 35, 0x15E0, 0x15E4, 13, GumpButtonType.Reply, 0);
                this.AddButton(width - 35, height - 35, 0x15E2, 0x15E6, 12, GumpButtonType.Reply, 0);
            }

            this.AddLabel(70, 50, 40, "Reward");
            this.AddLabel(this.x_creditoffset, 50, 40, "Credits");
            this.AddLabel(this.x_pointsoffset, 50, 40, "Min Points");
			
            // display the items with their selection buttons
            if (this.Rewards != null)
            {
                int y = 50;
                for (int i = 0; i < this.Rewards.Count; i++)
                {
                    if ((int)(i / this.maxItemsPerPage) != this.viewpage)
                        continue;

                    XmlQuestPointsRewards r = this.Rewards[i] as XmlQuestPointsRewards;
                    if (r == null)
                        continue;

                    y += this.y_inc;
                    
                    int texthue = 0;

                    // display the item
                    if (r.MinPoints > XmlQuestPoints.GetPoints(from))
                    {
                        texthue = 33;
                    }
                    else
                    {
                        // add the selection button
                        this.AddButton(30, y, 0xFA5, 0xFA7, 1000 + i, GumpButtonType.Reply, 0);
                    }

                    // display the name
                    this.AddLabel(70, y + 3, texthue, r.Name);
        			
                    // display the cost
                    this.AddLabel(this.x_creditoffset, y + 3, texthue, r.Cost.ToString());
        			
                    // display the item
                    if (r.ItemID > 0)
                        this.AddItem(this.x_creditoffset + 60, y, r.ItemID);
                        
                    // display the min points requirement
                    this.AddLabel(this.x_pointsoffset, y + 3, texthue, r.MinPoints.ToString());
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info == null || state == null || state.Mobile == null || this.Rewards == null)
                return;

            Mobile from = state.Mobile;

            switch ( info.ButtonID ) 
            {
                case 12:
                    // page up
                    int nitems = 0;
                    if (this.Rewards != null)
                        nitems = this.Rewards.Count;

                    int page = this.viewpage + 1;
                    if (page > (int)(nitems / this.maxItemsPerPage))
                    {
                        page = (int)(nitems / this.maxItemsPerPage);
                    }
                    state.Mobile.SendGump(new QuestRewardGump(state.Mobile, page));
                    break;
                case 13:
                    // page down
                    page = this.viewpage - 1;
                    if (page < 0)
                    {
                        page = 0;
                    }
                    state.Mobile.SendGump(new QuestRewardGump(state.Mobile, page));
                    break;
                default:
                    {
                        if (info.ButtonID >= 1000)
                        {
                            int selection = info.ButtonID - 1000;
                            if (selection < this.Rewards.Count)
                            {
                                XmlQuestPointsRewards r = this.Rewards[selection] as XmlQuestPointsRewards;
    
                                // check the price
                                if (XmlQuestPoints.HasCredits(from, r.Cost))
                                {
                                    // create an instance of the reward type
                                    object o = null;
                                
                                    try
                                    {
                                        o = Activator.CreateInstance(r.RewardType, r.RewardArgs);
                                    }
                                    catch
                                    {
                                    }

                                    bool received = true;

                                    if (o is Item)
                                    {
                                        // and give them the item
                                        from.AddToBackpack((Item)o);
                                    }
                                    else if (o is Mobile)
                                    {
                                        // if it is controllable then set the buyer as master.  Note this does not check for control slot limits.
                                        if (o is BaseCreature)
                                        {
                                            BaseCreature b = o as BaseCreature;
                                            b.Controlled = true;
                                            b.ControlMaster = from;
                                        }

                                        ((Mobile)o).MoveToWorld(from.Location, from.Map);
                                    }
                                    else if (o is XmlAttachment)
                                    {
                                        XmlAttachment a = o as XmlAttachment;

                                        XmlAttach.AttachTo(from, a);
                                    }
                                    else
                                    {
                                        from.SendMessage(33, "unable to create {0}.", r.RewardType.Name);
                                        received = false;
                                    }
                                
                                    // complete the transaction
                                    if (received)
                                    {
                                        // charge them
                                        XmlQuestPoints.TakeCredits(from, r.Cost);
                                        from.SendMessage("You have purchased {0} for {1} credits.", r.Name, r.Cost);
                                    }
                                }
                                else
                                {
                                    from.SendMessage("Insufficient Credits for {0}.", r.Name);
                                }
                                from.SendGump(new QuestRewardGump(from, this.viewpage));
                            }
                        }
                        break;
                    }
            }
        }
    }
}