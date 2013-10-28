using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Mobiles;
using Server.Network;

/*
** PointsRewardGump
** ArteGordon
** updated 11/08/04
**
** Gives out rewards based on the XmlMobFactionsReward reward list entries and the players Credits that are accumulated through monster kills 
** with the XmlMobFactions attachment.
** The Gump supports Item, Mobile, and Attachment type rewards.
*/
namespace Server.Gumps 
{ 
    public class MobFactionsRewardGump : Gump
    {
        private readonly ArrayList Rewards;
        private readonly int y_inc = 35;
        private readonly int x_creditoffset = 350;
        private readonly int x_factionoffset = 470;
        private readonly int maxItemsPerPage = 9;
        private readonly int viewpage;
        public MobFactionsRewardGump(Mobile from, int page)
            : base(20, 30)
        { 
            from.CloseGump(typeof(MobFactionsRewardGump));

            // determine the gump size based on the number of rewards
            
            this.Rewards = XmlMobFactionsRewards.RewardsList;
            
            this.viewpage = page;
            
            int height = this.maxItemsPerPage * this.y_inc + 120;
            int width = this.x_factionoffset + 180;

            /*
            if(Rewards != null && Rewards.Count > 0)
            {
            height = Rewards.Count*y_inc + 120;
            }
            */

            this.AddBackground(0, 0, width, height, 0xDAC);

            this.AddHtml(30, 20, 350, 50, "Rewards Available for Purchase with Mob Faction Credits", false, false);

            this.AddLabel(440, 20, 0, String.Format("Available Credits: {0}", XmlMobFactions.GetCredits(from)));

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
			
            this.AddLabel(this.x_factionoffset, 50, 40, "Minimum Required Faction");

            // display the items with their selection buttons
            if (this.Rewards != null)
            {
                int y = 50;
                for (int i = 0; i < this.Rewards.Count; i++)
                {
                    if ((int)(i / this.maxItemsPerPage) != this.viewpage)
                        continue;

                    XmlMobFactionsRewards r = this.Rewards[i] as XmlMobFactionsRewards;
                    if (r == null)
                        continue;

                    y += this.y_inc;

                    int texthue = 0;

                    bool meetsrequirement = true;
                        
                    // display the item
                    if (r.RequiredFaction != XmlMobFactions.GroupTypes.End_Unused)
                    {
                        if (XmlMobFactions.GetFactionLevel(from, r.RequiredFaction) > r.MinFaction)
                        {
                            meetsrequirement = true;
                        }
                        else
                        {
                            meetsrequirement = false;
                            texthue = 33;
                        }
                        
                        // display the faction requirement
                        this.AddLabel(this.x_factionoffset, y + 3, texthue, r.RequiredFaction.ToString());
                        this.AddLabel(this.x_factionoffset + 110, y + 3, texthue, r.MinFaction.ToString());
                    }
                    
                    // display the name
                    this.AddLabel(70, y + 3, texthue, r.Name);
        			
                    // display the cost
                    this.AddLabel(this.x_creditoffset, y + 3, texthue, r.Cost.ToString());
        			
                    // display the item
                    if (r.ItemID > 0)
                        this.AddItem(this.x_creditoffset + 60, y, r.ItemID);
                    
                    if (meetsrequirement)
                    {
                        // add the selection button
                        this.AddButton(30, y, 0xFA5, 0xFA7, 1000 + i, GumpButtonType.Reply, 0);
                    }
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
                    state.Mobile.SendGump(new MobFactionsRewardGump(state.Mobile, page));
                    break;
                case 13:
                    // page down
                    page = this.viewpage - 1;
                    if (page < 0)
                    {
                        page = 0;
                    }
                    state.Mobile.SendGump(new MobFactionsRewardGump(state.Mobile, page));
                    break;
                default:
                    {
                        if (info.ButtonID >= 1000)
                        {
                            int selection = info.ButtonID - 1000;
                            if (selection < this.Rewards.Count)
                            {
                                XmlMobFactionsRewards r = this.Rewards[selection] as XmlMobFactionsRewards;
    
                                // check the price
                                if (XmlMobFactions.HasCredits(from, r.Cost))
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
                                        XmlMobFactions.TakeCredits(from, r.Cost);
                                        from.SendMessage("You have purchased {0} for {1} credits.", r.Name, r.Cost);
                                    }
                                }
                                else
                                {
                                    from.SendMessage("Insufficient Credits for {0}.", r.Name);
                                }
                                from.SendGump(new MobFactionsRewardGump(from, this.viewpage));
                            }
                        }
                        break;
                    }
            }
        }
    }
}