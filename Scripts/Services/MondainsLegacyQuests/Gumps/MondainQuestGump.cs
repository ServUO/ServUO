using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class MondainQuestGump : BaseQuestGump
    {
        private const int ButtonOffset = 11;
        private readonly object m_Quester;
        private readonly PlayerMobile m_From;
        private readonly BaseQuest m_Quest;
        private readonly bool m_Offer;
        private readonly bool m_Completed;
        private Section m_Section;
        public MondainQuestGump(PlayerMobile from)
            : this(from, null, Section.Main, false, false)
        {
        }

        public MondainQuestGump(BaseQuest quest)
            : this(quest, Section.Description, true)
        {
        }

        public MondainQuestGump(BaseQuest quest, Section section, bool offer)
            : this(null, quest, section, offer, false)
        {
        }

        public MondainQuestGump(BaseQuest quest, Section section, bool offer, bool completed)
            : this(null, quest, section, offer, completed)
        {
        }

        public MondainQuestGump(PlayerMobile owner, BaseQuest quest, Section section, bool offer, bool completed)
            : this(owner, quest, section, offer, completed, null)
        {
        }

        public MondainQuestGump(PlayerMobile owner, BaseQuest quest, Section section, bool offer, bool completed, object quester)
            : base(75, 25)
        {
            this.m_Quester = quester;
            this.m_Quest = quest;
            this.m_Section = section;
            this.m_Offer = offer;
            this.m_Completed = completed;
			
            if (quest != null)
                this.m_From = quest.Owner;
            else
                this.m_From = owner;
		
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.AddPage(0);

            AddImageTiled(50, 20, 400, 460, 0x1404);
            AddImageTiled(50, 29, 30, 450, 0x28DC);
            AddImageTiled(34, 140, 17, 339, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 479, 0x2842);
            AddImage(442, 479, 0x2840);
            AddImageTiled(51, 479, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 450, 0xA2D);
            AddImageTiled(415, 29, 30, 450, 0x28DC);
            AddImage(370, 50, 0x589);
			
            if ((int)this.m_From.AccessLevel > (int)AccessLevel.Counselor && quest != null)
                this.AddButton(379, 60, 0x15A9, 0x15A9, (int)Buttons.CompleteQuest, GumpButtonType.Reply, 0);
            else
                this.AddImage(379, 60, 0x15A9);
						
            this.AddImage(425, 0, 0x28C9);
            this.AddImage(90, 33, 0x232D);
            this.AddImageTiled(130, 65, 175, 1, 0x238D);
			
            switch ( this.m_Section )
            {
                case Section.Main:
                    this.SecMain();
                    break;
                case Section.Description:
                    this.SecDescription();
                    break;
                case Section.Objectives:
                    this.SecObjectives();
                    break; 
                case Section.Rewards:
                    this.SecRewards();
                    break;
                case Section.Refuse:
                    this.SecRefuse();
                    break;
                case Section.Complete:
                    this.SecComplete();
                    break;
                case Section.InProgress:
                    this.SecInProgress();
                    break;
                case Section.Failed:
                    SecFailed();
                    break;
            }
        }

        public enum Section
        {
            Main,
            Description,
            Objectives,
            Rewards,
            Refuse,
            Complete,
            InProgress,
            Failed
        }

        private enum Buttons
        {
            Close,
            CloseQuest,
            RefuseQuest,
            ResignQuest,
            AcceptQuest,
            AcceptReward,
            PreviousPage,
            NextPage,
            Complete,
            CompleteQuest,
            RefuseReward
        }
        public virtual void SecMain()
        {
            if (this.m_From == null)
                return;			
		
            this.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log
				
            int offset = 140;
			
            for (int i = this.m_From.Quests.Count - 1; i >= 0; i--)
            {
                BaseQuest quest = this.m_From.Quests[i];
				
                this.AddHtmlObject(98, offset, 270, 21, quest.Title, quest.Failed ? 0x3C00 : White, false, false);					
                this.AddButton(368, offset, 0x26B0, 0x26B1, ButtonOffset + i, GumpButtonType.Reply, 0);	
				
                offset += 21;			
            }
				
            this.AddButton(313, 455, 0x2EEC, 0x2EEE, (int)Buttons.Close, GumpButtonType.Reply, 0);
        }

        public virtual void SecDescription()
        {
            if (this.m_Quest == null)
                return;

            if (!m_Quest.RenderDescription(this, m_Offer))
            {
                if (this.m_Offer)
                    this.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
                else
                    this.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

                if (this.m_Quest.Failed)
                    this.AddHtmlLocalized(160, 80, 200, 32, 500039, 0x3C00, false, false); // Failed!

                this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);

                if (this.m_Quest.ChainID != QuestChain.None)
                    this.AddHtmlLocalized(98, 140, 312, 16, 1075024, 0x2710, false, false); // Description (quest chain)
                else
                    this.AddHtmlLocalized(98, 140, 312, 16, 1072202, 0x2710, false, false); // Description

                this.AddHtmlObject(98, 156, 312, 180, this.m_Quest.Description, LightGreen, false, true);
            }

            if (this.m_Offer)
            {
                this.AddButton(95, 455, 0x2EE0, 0x2EE2, (int)Buttons.AcceptQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EF2, 0x2EF4, (int)Buttons.RefuseQuest, GumpButtonType.Reply, 0);
            }
            else
            {
                this.AddButton(95, 455, 0x2EF5, 0x2EF7, (int)Buttons.ResignQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EEC, 0x2EEE, (int)Buttons.CloseQuest, GumpButtonType.Reply, 0);
            }
			
            if(m_Quest.ShowDescription)
                this.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);
        }

        public virtual void SecObjectives()
        {
            if (this.m_Quest == null)
                return;

            if (!m_Quest.RenderObjective(this, m_Offer))
            {
                if (this.m_Offer)
                    this.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
                else
                    this.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

                this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);
                this.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:

                if (this.m_Quest.AllObjectives)
                    this.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	
                else
                    this.AddHtmlLocalized(98, 156, 312, 16, 1072209, 0x2710, false, false); // Only one of the following

                int offset = 172;

                for (int i = 0; i < this.m_Quest.Objectives.Count; i++)
                {
                    BaseObjective objective = this.m_Quest.Objectives[i];

                    if (objective is SlayObjective)
                    {
                        SlayObjective slay = (SlayObjective)objective;

                        if (slay != null)
                        {
                            this.AddHtmlLocalized(98, offset, 30, 16, 1072204, 0x15F90, false, false); // Slay	
                            this.AddLabel(133, offset, 0x481, slay.MaxProgress + " " + slay.Name); // %count% + %name%

                            offset += 16;

                            if (this.m_Offer)
                            {
                                if (slay.Timed)
                                {
                                    this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                    this.AddLabel(223, offset, 0x481, this.FormatSeconds(slay.Seconds)); // %est. time remaining%

                                    offset += 16;
                                }
                                continue;
                            }

                            if (slay.Region != null)
                            {
                                this.AddHtmlLocalized(103, offset, 312, 20, 1018327, 0x15F90, false, false); // Location
                                this.AddHtmlObject(223, offset, 312, 20, slay.Region.Name, White, false, false); // %location%

                                offset += 16;
                            }

                            this.AddHtmlLocalized(103, offset, 120, 16, 3000087, 0x15F90, false, false); // Total			
                            this.AddLabel(223, offset, 0x481, slay.CurProgress.ToString());  // %current progress%

                            offset += 16;

                            if (this.ReturnTo() != null)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1074782, 0x15F90, false, false); // Return to	
                                this.AddLabel(223, offset, 0x481, this.ReturnTo());  // %return to%		

                                offset += 16;
                            }

                            if (slay.Timed)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                this.AddLabel(223, offset, 0x481, this.FormatSeconds(slay.Seconds)); // %est. time remaining%

                                offset += 16;
                            }
                        }
                    }
                    else if (objective is ObtainObjective)
                    {
                        ObtainObjective obtain = (ObtainObjective)objective;

                        if (obtain != null)
                        {
                            this.AddHtmlLocalized(98, offset, 40, 16, 1072205, 0x15F90, false, false); // Obtain						
                            this.AddLabel(143, offset, 0x481, obtain.MaxProgress + " " + obtain.Name); // %count% + %name%

                            if (obtain.Image > 0)
                                this.AddItem(350, offset, obtain.Image); // Image

                            offset += 16;

                            if (this.m_Offer)
                            {
                                if (obtain.Timed)
                                {
                                    this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                    this.AddLabel(223, offset, 0x481, this.FormatSeconds(obtain.Seconds)); // %est. time remaining%

                                    offset += 16;
                                }
                                else if (obtain.Image > 0)
                                    offset += 16;

                                continue;
                            }
                            this.AddHtmlLocalized(103, offset, 120, 16, 3000087, 0x15F90, false, false); // Total			
                            this.AddLabel(223, offset, 0x481, obtain.CurProgress.ToString());    // %current progress%

                            offset += 16;

                            if (this.ReturnTo() != null)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1074782, 0x15F90, false, false); // Return to	
                                this.AddLabel(223, offset, 0x481, this.ReturnTo());  // %return to%

                                offset += 16;
                            }

                            if (obtain.Timed)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                this.AddLabel(223, offset, 0x481, this.FormatSeconds(obtain.Seconds)); // %est. time remaining%

                                offset += 16;
                            }
                        }
                    }
                    else if (objective is DeliverObjective)
                    {
                        DeliverObjective deliver = (DeliverObjective)objective;

                        if (deliver != null)
                        {
                            this.AddHtmlLocalized(98, offset, 40, 16, 1072207, 0x15F90, false, false); // Deliver						
                            this.AddLabel(143, offset, 0x481, deliver.MaxProgress + " " + deliver.DeliveryName);     // %name%

                            offset += 16;

                            this.AddHtmlLocalized(103, offset, 120, 16, 1072379, 0x15F90, false, false); // Deliver to						
                            this.AddLabel(223, offset, 0x481, deliver.DestName); // %deliver to%

                            offset += 16;

                            if (deliver.Timed)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                this.AddLabel(223, offset, 0x481, this.FormatSeconds(deliver.Seconds)); // %est. time remaining%

                                offset += 16;
                            }
                        }
                    }
                    else if (objective is EscortObjective)
                    {
                        EscortObjective escort = (EscortObjective)objective;

                        if (escort != null)
                        {

                            this.AddHtmlLocalized(98, offset, 50, 16, 1072206, 0x15F90, false, false); // Escort to
                            this.AddHtmlObject(153, offset, 200, 16, escort.Region.Name, White, false, false);

                            offset += 16;

                            if (escort.Timed)
                            {
                                this.AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                                this.AddLabel(223, offset, 0x481, this.FormatSeconds(escort.Seconds)); // %est. time remaining%

                                offset += 16;
                            }
                        }
                    }
                    else if (objective is ApprenticeObjective)
                    {
                        ApprenticeObjective apprentice = (ApprenticeObjective)objective;

                        if (apprentice != null)
                        {

                            this.AddHtmlLocalized(98, offset, 200, 16, 1077485, "#" + (1044060 + (int)apprentice.Skill) + "\t" + apprentice.MaxProgress, 0x15F90, false, false); // Increase ~1_SKILL~ to ~2_VALUE~

                            offset += 16;
                        }
                    }
                    else if (objective is SimpleObjective && ((SimpleObjective)objective).Descriptions != null)
                    {
                        SimpleObjective obj = (SimpleObjective)objective;

                        for (int j = 0; j < obj.Descriptions.Count; j++)
                        {
                            offset += 16;
                            AddLabel(98, offset, 0x481, obj.Descriptions[j]);
                        }

                        if (obj.Timed)
                        {
                            offset += 16;
                            AddHtmlLocalized(103, offset, 120, 16, 1062379, 0x15F90, false, false); // Est. time remaining:
                            AddLabel(223, offset, 0x481, FormatSeconds(obj.Seconds)); // %est. time remaining%
                        }
                    }
                }
            }

            if (this.m_Offer)
            {
                this.AddButton(95, 455, 0x2EE0, 0x2EE2, (int)Buttons.AcceptQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EF2, 0x2EF4, (int)Buttons.RefuseQuest, GumpButtonType.Reply, 0);
            }
            else
            {
                this.AddButton(95, 455, 0x2EF5, 0x2EF7, (int)Buttons.ResignQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EEC, 0x2EEE, (int)Buttons.CloseQuest, GumpButtonType.Reply, 0);
            }


            this.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            this.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);
        }

        public virtual void SecRewards()
        {
            if (this.m_Quest == null)
                return;
		
            if (this.m_Offer)
                this.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                this.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log	
			
            this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);
            this.AddHtmlLocalized(98, 140, 312, 16, 1072201, 0x2710, false, false); // Reward	
			
            int offset = 163;
			
            for (int i = 0; i < this.m_Quest.Rewards.Count; i++)
            {
                BaseReward reward = this.m_Quest.Rewards[i];

                if (reward != null)
                {
                    this.AddImage(105, offset, 0x4B9);
                    this.AddHtmlObject(133, offset, 280, 32, reward.Name, LightGreen, false, false);

                    offset += 16;
                }
            }
			
            if (this.m_Completed)
            {
                this.AddButton(95, 455, 0x2EE0, 0x2EE2, (int)Buttons.AcceptReward, GumpButtonType.Reply, 0);

                if (m_Quest.CanRefuseReward)
                    AddButton(313, 430, 0x2EF2, 0x2EF4, (int)Buttons.RefuseReward, GumpButtonType.Reply, 0);
                else
                    AddButton(313, 455, 0x2EE6, 0x2EE8, (int)Buttons.Close, GumpButtonType.Reply, 0);
            }
            else if (this.m_Offer)
            {
                this.AddButton(95, 455, 0x2EE0, 0x2EE2, (int)Buttons.AcceptQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EF2, 0x2EF4, (int)Buttons.RefuseQuest, GumpButtonType.Reply, 0);
                this.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            }
            else
            {
                this.AddButton(95, 455, 0x2EF5, 0x2EF7, (int)Buttons.ResignQuest, GumpButtonType.Reply, 0);
                this.AddButton(313, 455, 0x2EEC, 0x2EEE, (int)Buttons.CloseQuest, GumpButtonType.Reply, 0);
                this.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            }
        }

        public virtual void SecRefuse()
        {
            if (this.m_Quest == null)
                return;
		
            if (this.m_Offer)
            {
                this.AddHtmlLocalized(130, 45, 270, 16, 3006156, 0xFFFFFF, false, false); // Quest Conversation
                this.AddImage(140, 110, 0x4B9);
                this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);
                this.AddHtmlObject(98, 140, 312, 180, this.m_Quest.Refuse, LightGreen, false, true);
				
                this.AddButton(313, 455, 0x2EE6, 0x2EE8, (int)Buttons.Close, GumpButtonType.Reply, 0);
            }
        }

        public virtual void SecInProgress()
        {
            if (this.m_Quest == null)
                return;
				
            this.AddHtmlLocalized(130, 45, 270, 16, 3006156, 0xFFFFFF, false, false); // Quest Conversation				
            this.AddImage(140, 110, 0x4B9);
            this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);	
            this.AddHtmlObject(98, 140, 312, 180, this.m_Quest.Uncomplete, LightGreen, false, true);
							
            this.AddButton(313, 455, 0x2EE6, 0x2EE8, (int)Buttons.Close, GumpButtonType.Reply, 0);
        }

        public virtual void SecComplete()
        {
            if (this.m_Quest == null)
                return;
				
            if (this.m_Quest.Complete == null)
            {
                if (QuestHelper.TryDeleteItems(this.m_Quest))
                {
                    if (QuestHelper.AnyRewards(this.m_Quest))
                    {
                        this.m_Section = Section.Rewards;
                        this.SecRewards();
                    }
                    else
                        this.m_Quest.GiveRewards();
                }
					
                return;
            }
				
            this.AddHtmlLocalized(130, 45, 270, 16, 3006156, 0xFFFFFF, false, false); // Quest Conversation
            this.AddImage(140, 110, 0x4B9);
            this.AddHtmlObject(160, 70, 200, 40, this.m_Quest.Title, DarkGreen, false, false);	
            this.AddHtmlObject(98, 140, 312, 180, this.m_Quest.Complete, LightGreen, false, true);
				
            this.AddButton(313, 455, 0x2EE6, 0x2EE8, (int)Buttons.Close, GumpButtonType.Reply, 0);
            this.AddButton(95, 455, 0x2EE9, 0x2EEB, (int)Buttons.Complete, GumpButtonType.Reply, 0);
        }

        public virtual void SecFailed()
        {
            if (m_Quest == null)
                return;

            object fail = m_Quest.FailedMsg;

            if (fail == null)
                fail = "You have failed to meet the conditions of the quest.";

            AddHtmlLocalized(130, 45, 270, 16, 3006156, 0xFFFFFF, false, false); // Quest Conversation				
            AddImage(140, 110, 0x4B9);
            AddHtmlObject(160, 70, 200, 40, m_Quest.Title, DarkGreen, false, false);
            AddHtmlObject(98, 140, 312, 240, fail, LightGreen, false, true);

            AddButton(313, 455, 0x2EE6, 0x2EE8, (int)Buttons.Close, GumpButtonType.Reply, 0);
        }

        public virtual string FormatSeconds(int seconds)
        {
            int hours = seconds / 3600;
			
            seconds -= hours * 3600;
			
            int minutes = seconds / 60;
			
            seconds -= minutes * 60;
			
            if (hours > 0 && minutes > 0)
                return hours + ":" + minutes + ":" + seconds;
            else if (minutes > 0)
                return minutes + ":" + seconds;
            else
                return seconds.ToString();
        }

        public virtual string ReturnTo()
        {
            if (this.m_Quest == null)
                return null;
				
            if (this.m_Quest.StartingMobile != null)
            {
                string returnTo = this.m_Quest.StartingMobile.Name;				
				
                if (this.m_Quest.StartingMobile.Region != null)
                    returnTo = String.Format("{0} ({1})", returnTo, this.m_Quest.StartingMobile.Region.Name);
                else
                    returnTo = String.Format("{0}", returnTo);
					
                return returnTo;
            }
			
            return null;
        }

        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        { 
            if (this.m_From != null)
                this.m_From.CloseGump(typeof(MondainQuestGump));
				
            switch ( info.ButtonID )
            { 
                // close quest list
                case (int)Buttons.Close:
                    break;
                    // close quest
                case (int)Buttons.CloseQuest:
                    this.m_From.SendGump(new MondainQuestGump(this.m_From));
                    break;
                    // accept quest
                case (int)Buttons.AcceptQuest:
                    if (this.m_Offer)
                        this.m_Quest.OnAccept();						
                    break;
                    // refuse quest
                case (int)Buttons.RefuseQuest:
                    if (this.m_Offer)
                    {
                        this.m_Quest.OnRefuse();
                        this.m_From.SendGump(new MondainQuestGump(this.m_Quest, Section.Refuse, true));
                    }
                    break;
                    // resign quest
                case (int)Buttons.ResignQuest:
                    if (!this.m_Offer)
                        this.m_From.SendGump(new MondainResignGump(this.m_Quest));
                    break;
                    // accept reward
                case (int)Buttons.AcceptReward:
                    if (!this.m_Offer && this.m_Section == Section.Rewards && this.m_Completed)
                        this.m_Quest.GiveRewards();
                    break;
                    // refuse reward
                case (int)Buttons.RefuseReward:
                    if (!m_Offer && m_Section == Section.Rewards && m_Completed)
                        m_Quest.RefuseRewards();
                    break;
                    // previous page
                case (int)Buttons.PreviousPage:
                    if (this.m_Section == Section.Objectives || (this.m_Section == Section.Rewards && !this.m_Completed))
                    {
                        this.m_Section = (Section)((int)this.m_Section - 1);
                        this.m_From.SendGump(new MondainQuestGump(this.m_Quest, this.m_Section, this.m_Offer));						
                    }
                    break;
                    // next page
                case (int)Buttons.NextPage:
                    if (this.m_Section == Section.Description || this.m_Section == Section.Objectives)
                    {
                        this.m_Section = (Section)((int)this.m_Section + 1);
                        this.m_From.SendGump(new MondainQuestGump(this.m_Quest, this.m_Section, this.m_Offer));						
                    }
                    break;
                    // player complete quest
                case (int)Buttons.Complete:
                    if (!this.m_Offer && this.m_Section == Section.Complete)
                    { 
                        if (!this.m_Quest.Completed)
                            this.m_From.SendLocalizedMessage(1074861); // You do not have everything you need!
                        else
                        {
                            QuestHelper.DeleteItems(this.m_Quest);
					
                            if (this.m_Quester != null)
                                this.m_Quest.Quester = this.m_Quester;
								
                            if (!QuestHelper.AnyRewards(this.m_Quest))		
                                this.m_Quest.GiveRewards();
                            else
                                this.m_From.SendGump(new MondainQuestGump(this.m_Quest, Section.Rewards, false, true));
                        }
                    }
                    break;
                    // admin complete quest
                case (int)Buttons.CompleteQuest:
                    if ((int)this.m_From.AccessLevel > (int)AccessLevel.Counselor && this.m_Quest != null)
                        QuestHelper.CompleteQuest(this.m_From, this.m_Quest);
                    break;
                    // show quest
                default:
                    if (this.m_Section != Section.Main || info.ButtonID >= this.m_From.Quests.Count + ButtonOffset || info.ButtonID < ButtonOffset)
                        break;
					
                    this.m_From.SendGump(new MondainQuestGump(this.m_From.Quests[(int)info.ButtonID - ButtonOffset], Section.Description, false));										
                    break;
            }
        }
    }
}