using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;

namespace Server.Engines.Quests
{
    public class QuestHelper
    { 
        public static void RemoveAcceleratedSkillgain(PlayerMobile from)
        {
            Region region = from.Region;
			
            while (region != null)
            { 
                if (region is ApprenticeRegion && ((ApprenticeRegion)region).Table[from] is BuffInfo)
                {
                    BuffInfo.RemoveBuff(from, (BuffInfo)((ApprenticeRegion)region).Table[from]);
                    ((ApprenticeRegion)region).Table[from] = null;
                }
					
                region = region.Parent;
            }
        }

        public static BaseQuest RandomQuest(PlayerMobile from, Type[] quests, object quester)
        {
            if (quests == null || (quests != null && quests.Length == 0))
                return null;
				
            // give it 10 tries to generate quest
            for (int i = 0; i < 10; i ++)
            {
                BaseQuest quest = Construct(quests[Utility.Random(quests.Length)]) as BaseQuest;
				
                if (quest != null)
                {
                    quest.Owner = from;
                    quest.Quester = quester;					
				
                    if (CanOffer(from, quest, quests.Length == 1))
                        return quest;
                    else if (quest.StartingMobile != null && !quest.DoneOnce && quests.Length == 1)
                        quest.StartingMobile.OnOfferFailed();
                }
				
                if (quests.Length == 1)
                    return null;
            }
			
            return null;
        }

        public static bool CanOffer(PlayerMobile from, BaseQuest quest, bool message)
        {
            if (!quest.CanOffer())
                return false;
			
            // if a player wants to start quest chain (already started) again (not osi)			
            if (quest.ChainID != QuestChain.None && FirstChainQuest(quest, quest.Quester) && from.Chains.ContainsKey(quest.ChainID))
                return false;
				
            if (!Delayed(from, quest, message))
                return false;
		
            for (int i = quest.Objectives.Count - 1; i >= 0; i --)
            {
                Type type = quest.Objectives[i].Type();
				
                if (type == null)
                    continue;
			
                for (int j = from.Quests.Count - 1; j >= 0; j --)
                {
                    BaseQuest pQuest = from.Quests[j];
					
                    for (int k = pQuest.Objectives.Count - 1; k >= 0; k --)
                    {
                        BaseObjective obj = pQuest.Objectives[k];
						
                        if (type == obj.Type())
                            return false;					
                    }
                }
            }
			
            return true;
        }

        public static bool Delayed(PlayerMobile player, BaseQuest quest, bool message)
        {
            List<QuestRestartInfo> doneQuests = player.DoneQuests;
																					
            for (int i = doneQuests.Count - 1; i >= 0; i --)
            { 
                QuestRestartInfo restartInfo = doneQuests[i];

                if (restartInfo.QuestType == quest.GetType())
                {
                    if (quest.DoneOnce)
                    {
                        if (message && quest.StartingMobile != null)
                            quest.StartingMobile.Say(1075454); // I can not offer you the quest again.
					
                        return false;
                    }
						
                    DateTime endTime = restartInfo.RestartTime;
					
                    if (DateTime.UtcNow < endTime)
                        return false;
					
                    if (quest.RestartDelay > TimeSpan.Zero)
                        doneQuests.RemoveAt(i);
													
                    return true;
                }
            }
			
            return true;
        }

        public static void Delay(PlayerMobile player, Type type, TimeSpan delay)
        { 
            for (int i = 0; i < player.DoneQuests.Count; i ++)
            {
                QuestRestartInfo restartInfo = player.DoneQuests[i];

                if (restartInfo.QuestType == type)
                {
                    restartInfo.Reset(delay);
                    return;
                }
            }
			
            player.DoneQuests.Add(new QuestRestartInfo(type, delay));
        }

        public static bool InProgress(PlayerMobile player, Type[] quests)
        { 
            if (quests == null)
                return false;
				
            for (int i = 0; i < quests.Length; i ++)
            { 
                for (int j = 0; j < player.Quests.Count; j ++)
                {
                    BaseQuest quest = player.Quests[j];
					
                    if (quests[i].IsAssignableFrom(quest.GetType()))
                    {
                        if (quest.Completed)		
                            player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                        else
                        {
                            player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                            quest.InProgress();
                        }
							
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool InProgress(PlayerMobile player, MondainQuester quester)
        { 
            for (int i = 0; i < player.Quests.Count; i ++)
            {
                BaseQuest quest = player.Quests[i];
				
                if (quest.Quester == null)
                    continue;
					
                if (quest.Quester.GetType() == quester.GetType())
                {
                    if (quest.Completed)		
                    {
                        if (quest.Complete == null && !AnyRewards(quest))
                            quest.GiveRewards();
                        else 
                            player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                    }
                    else
                    {
                        player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                        quest.InProgress();
                    }
						
                    return true;
                }
            }
			
            return false;
        }

        public static bool AnyRewards(BaseQuest quest)
        {
            for (int i = 0; i < quest.Rewards.Count; i ++)
            {
                BaseReward reward = quest.Rewards[i];
				
                if (reward.Type != null)
                    return true;
            }
			
            return false;
        }

        public static bool DeliveryArrived(PlayerMobile player, BaseVendor vendor)
        {
            for (int i = 0; i < player.Quests.Count; i ++)
            {
                BaseQuest quest = player.Quests[i];
				
                for (int j = 0; j < quest.Objectives.Count; j ++)
                {
                    BaseObjective objective = quest.Objectives[j];
					
                    if (objective is DeliverObjective)
                    {
                        DeliverObjective deliver = (DeliverObjective)objective;
						
                        if (deliver.Update(vendor))
                        {
                            if (quest.Completed)
                            { 
                                player.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.												
                                player.PlaySound(quest.CompleteSound);	
								
                                quest.OnCompleted();
								
                                if (vendor is MondainQuester)
                                    player.SendGump(new MondainQuestGump(player, quest, MondainQuestGump.Section.Complete, false, true, (MondainQuester)vendor));
                                else
                                    player.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));									
                            }
							
                            return true;
                        }
                    }
                }
            }
			
            return false;
        }

        public static bool QuestLimitReached(PlayerMobile player)
        {
            if (player.Quests.Count >= 10)
            {
                player.SendLocalizedMessage(1075141); // You are too busy with other tasks at this time.
                return true;					
            }
			
            return false;
        }

        public static bool FirstChainQuest(BaseQuest quest, object quester)
        {
            Type[] quests = null;
		
            if (quester is MondainQuester)
            {
                MondainQuester mQuester = (MondainQuester)quester;
				
                quests = mQuester.Quests;
            }
            else if (quester is BaseQuestItem)
            {
                BaseQuestItem iQuester = (BaseQuestItem)quester;
				
                quests = iQuester.Quests;
            }
			
            if (quests != null)
            {
                for (int i = 0; i < quests.Length; i ++)
                {
                    if (quests[i] == quest.GetType())
                        return true;
                }
            }
			
            return false;
        }

        public static Type FindFirstChainQuest(BaseQuest quest)
        {
            if (quest == null)
                return null;
				
            Type[] quests = null;
		
            if (quest.Quester is MondainQuester)
            {
                MondainQuester mQuester = (MondainQuester)quest.Quester;
				
                quests = mQuester.Quests;
            }
            else if (quest.Quester is BaseQuestItem)
            {
                BaseQuestItem iQuester = (BaseQuestItem)quest.Quester;
				
                quests = iQuester.Quests;
            }
			
            if (quests != null)
            {
                for (int i = 0; i < quests.Length; i ++)
                {
                    BaseQuest fQuest = Construct(quests[i]) as BaseQuest;
					
                    if (fQuest != null && fQuest.ChainID == quest.ChainID)
                        return fQuest.GetType();
                }
            }
			
            return null;
        }

        public static Region FindRegion(string name)
        {
            if (name == null)
                return null;
				
            Region reg = null;
			
            if (Map.Trammel.Regions.TryGetValue(name, out reg))
                return reg;
				
            if (Map.Felucca.Regions.TryGetValue(name, out reg))
                return reg; 
				
            if (Map.Ilshenar.Regions.TryGetValue(name, out reg))
                return reg; 
			
            if (Map.Malas.Regions.TryGetValue(name, out reg))
                return reg; 
				
            if (Map.Tokuno.Regions.TryGetValue(name, out reg))
                return reg;

            if (Map.TerMur.Regions.TryGetValue(name, out reg))
                return reg;
				
            return reg;
        }

        public static void CompleteQuest(PlayerMobile from, BaseQuest quest)
        {
            if (quest == null)
                return;
				
            for (int i = 0; i < quest.Objectives.Count; i ++)
            {
                BaseObjective obj = quest.Objectives[i];
				
                obj.Complete();
            }
			
            from.SendLocalizedMessage(1046258, null, 0x23); // Your quest is complete.							
            from.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));							
            from.PlaySound(quest.CompleteSound);
			
            quest.OnCompleted();
        }

        public static void DeleteItems(PlayerMobile from, Type itemType, int amount, bool questItem)
        {
            if (from.Backpack == null || itemType == null || amount <= 0)
                return;
								
            Item[] items = from.Backpack.FindItemsByType(itemType);
			
            int deleted = 0;
			
            for (int i = items.Length - 1; i >= 0 && deleted < amount; i --)
            {
                Item item = items[i];
				
                if (item.QuestItem || !questItem)
                {
                    item.QuestItem = false;
					
                    if (deleted + item.Amount > amount)
                    {
                        item.Amount -= amount - deleted;
                        deleted += amount - deleted;
                    }
                    else
                    {
                        item.Delete();
                        deleted += item.Amount;
                    }
                }
            }
			
            if (deleted < amount)
            {
                for (int i = from.Items.Count - 1; i >= 0 && deleted < amount; i --)
                {
                    Item item = from.Items[i];
					
                    if (item.QuestItem || !questItem)
                    { 
                        if (itemType.IsAssignableFrom(item.GetType()))
                        { 
                            deleted += item.Amount;
												
                            item.Delete();
                        }
                    }
                }
            }
        }

        public static void DeleteItems(BaseQuest quest)
        { 
            for (int i = 0; i < quest.Objectives.Count; i ++)
            { 
                BaseObjective objective = quest.Objectives[i];				
			
                DeleteItems(quest.Owner, objective.Type(), objective.MaxProgress, true);
                RemoveStatus(quest.Owner, objective.Type());		
            }
        }

        public static bool TryDeleteItems(BaseQuest quest)
        {
            if (quest == null)
                return false;
				
            for (int i = 0; i < quest.Objectives.Count; i ++)
            {
                if (quest.Objectives[i] is ObtainObjective)
                {
                    ObtainObjective obtain = (ObtainObjective)quest.Objectives[i];
					
                    if (obtain.MaxProgress > CountQuestItems(quest.Owner, obtain.Obtain))
                        return false;
                }
                else if (quest.Objectives[i] is DeliverObjective)
                {
                    DeliverObjective deliver = (DeliverObjective)quest.Objectives[i];
					
                    if (quest.StartingItem != null)
                        continue;
                    else if (deliver.MaxProgress > CountQuestItems(quest.Owner, deliver.Delivery))
                    { 
                        quest.Owner.SendLocalizedMessage(1074813);  // You have failed to complete your delivery.
                        deliver.Fail();
						
                        return false;
                    }
                }
            }
			
            DeleteItems(quest);
			
            return true;
        }

        public static int CountQuestItems(PlayerMobile from, Type type)
        {
            int count = 0;
			
            if (from.Backpack == null)
                return count;
			
            Item[] items = from.Backpack.FindItemsByType(type);
			
            for (int i = 0; i < items.Length; i ++)
            {
                Item item = items[i];
				
                if (item.QuestItem)
                    count += item.Amount;
            }
			
            return count;
        }

        public static int RemoveStatus(PlayerMobile from, Type type)
        {
            if (type == null)
                return 0;
				
            Item[] items = from.Backpack.FindItemsByType(type);
			
            int count = 0;
			
            for (int i = 0; i < items.Length; i ++)
            {
                Item item = items[i];
				
                if (item.QuestItem)
                {
                    count += 1;
                    item.QuestItem = false;
                }
            }
			
            return count;
        }

        public static void RemoveStatus(PlayerMobile from, Item item)
        {
            for (int i = from.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = from.Quests[i];
				
                for (int j = quest.Objectives.Count - 1; j >= 0; j --)
                {
                    if (quest.Objectives[j] is ObtainObjective)
                    {
                        ObtainObjective obtain = (ObtainObjective)quest.Objectives[j];
						
                        if (obtain.Obtain != null && obtain.Obtain.IsAssignableFrom(item.GetType()))
                        {
                            obtain.CurProgress -= item.Amount;									
                            item.QuestItem = false;							
                            from.SendLocalizedMessage(1074769); // An item must be in your backpack (and not in a container within) to be toggled as a quest item. 	
                            return;					
                        }
                    }
                    else if (quest.Objectives[j] is DeliverObjective)
                    {
                        DeliverObjective deliver = (DeliverObjective)quest.Objectives[j];
						
                        if (deliver.Delivery != null && deliver.Delivery.IsAssignableFrom(item.GetType()))
                        {
                            from.SendLocalizedMessage(1074813);  // You have failed to complete your delivery.							
                            DeleteItems(from, deliver.Delivery, deliver.MaxProgress, false);						
                            deliver.Fail();
                            item.Delete();							
                            return;
                        }
                    }
                }
            }
        }

        public static bool CheckCreature(PlayerMobile player, BaseCreature creature)
        {
            for (int i = player.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = player.Quests[i];
				
                for (int j = quest.Objectives.Count - 1; j >= 0; j --)
                {
                    if (quest.Objectives[j] is SlayObjective)
                    {
                        SlayObjective slay = (SlayObjective)quest.Objectives[j];
						
                        if (slay.Update(creature))
                        {
                            if (quest.Completed)
                                quest.OnCompleted();
                            else if (slay.Completed)
                                player.PlaySound(quest.UpdateSound);	
								
                            return true;
                        }
                    }
                }
            }
			
            return false;
        }

        public static bool CheckItem(PlayerMobile player, Item item)
        {
            for (int i = player.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = player.Quests[i];
				
                for (int j = quest.Objectives.Count - 1; j >= 0; j --)
                {
                    BaseObjective objective = quest.Objectives[j];
					
                    if (objective is ObtainObjective)
                    {
                        ObtainObjective obtain = (ObtainObjective)objective;
						
                        if (obtain.Update(item))
                        {
                            if (quest.Completed)
                                quest.OnCompleted();
                            else if (obtain.Completed)
                                player.PlaySound(quest.UpdateSound);
									
                            return true;
                        }
                    }
                }
            }
			
            return false;
        }

        public static bool CheckSkill(PlayerMobile player, Skill skill)
        { 
            for (int i = player.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = player.Quests[i];
				
                for (int j = quest.Objectives.Count - 1; j >= 0; j --)
                {
                    BaseObjective objective = quest.Objectives[j];
					
                    if (objective is ApprenticeObjective)
                    {
                        ApprenticeObjective apprentice = (ApprenticeObjective)objective;
						
                        if (apprentice.Update(skill))
                        { 
                            if (quest.Completed)
                                quest.OnCompleted();
                            else if (apprentice.Completed)
                                player.PlaySound(quest.UpdateSound);
                        }
                    }
                }
            }
			
            return false;
        }

        public static bool EnhancedSkill(PlayerMobile player, Skill skill)
        {
            if (player == null || player.Region == null || skill == null)
                return false;
				
            for (int i = player.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = player.Quests[i];
				
                for (int j = quest.Objectives.Count - 1; j >= 0; j --)
                {
                    BaseObjective objective = quest.Objectives[j];
					
                    if (objective is ApprenticeObjective && !objective.Completed)
                    {
                        ApprenticeObjective apprentice = (ApprenticeObjective)objective;
						
                        if (apprentice.Region != null)
                        {
                            if (player.Region.IsPartOf(apprentice.Region) && skill.SkillName == apprentice.Skill)
                                return true;
                        }
                    }
                }
            }
			
            return false;
        }

        public static object Construct(Type type)
        {
            if (type == null)
                return null;
				
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null;
            }
        }

        public static void StartTimer(PlayerMobile player)
        {
            if (player == null || player.Quests == null)
                return;
		
            for (int i = player.Quests.Count - 1; i >= 0; i --)
                player.Quests[i].StartTimer();
        }

        public static void StopTimer(PlayerMobile player)
        {
            if (player == null || player.Quests == null)
                return;
				
            for (int i = player.Quests.Count - 1; i >= 0; i --)
                player.Quests[i].StopTimer();
        }

        public static void GetContextMenuEntries(List<ContextMenuEntry> list)
        {
            if (list == null)
                return;
				
            list.Add(new SelectQuestItem());
        }

        public static bool FindCompletedQuest(PlayerMobile from, Type type, bool delete)
        {
            if (type == null)
                return false;
				
            for (int i = from.DoneQuests.Count - 1; i >= 0; i --)
            {
                QuestRestartInfo restartInfo = from.DoneQuests[i];

                if (restartInfo.QuestType == type)
                {
                    if (delete)
                        from.DoneQuests.RemoveAt(i);
						
                    return true;
                }
            }
			
            return false;
        }

        public static bool HasQuest<T>( PlayerMobile from ) where T : BaseQuest
        {
            return GetQuest( from, typeof( T ) ) != null;
        }

        public static BaseQuest GetQuest(PlayerMobile from, Type type)
        {
            if (type == null)
                return null;
				
            for (int i = from.Quests.Count - 1; i >= 0; i --)
            {
                BaseQuest quest = from.Quests[i];

                if (quest.GetType() == type)
                    return quest;
            }
			
            return null;
        }
    }

    public class SelectQuestItem : ContextMenuEntry
    {
        public SelectQuestItem()
            : base(6169)
        {
        }

        public override void OnClick()
        {
            if (!this.Owner.From.Alive)
                return;
				
            this.Owner.From.SendLocalizedMessage(1072352); // Target the item you wish to toggle Quest Item status on <ESC> to cancel			
            this.Owner.From.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleQuestItem_Callback));
        }

        private void ToggleQuestItem_Callback(Mobile from, object obj)
        {
            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
		
                if (obj is Item)
                {
                    Item item = (Item)obj;
					
                    if (item.IsChildOf(player.Backpack))
                    {
                        if (!QuestHelper.CheckItem(player, item))
                            player.SendLocalizedMessage(1072355, null, 0x23); // That item does not match any of your quest criteria
                    }
                }
                else
                    player.SendLocalizedMessage(1074769); // An item must be in your backpack (and not in a container within) to be toggled as a quest item.
				
                player.BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleQuestItem_Callback));
            }
        }
    }
}