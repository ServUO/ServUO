using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class BaseQuest
    { 
        public virtual bool AllObjectives
        {
            get
            {
                return true;
            }
        }
        public virtual bool DoneOnce
        {
            get
            {
                return false;
            }
        }
        public virtual TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.Zero;
            }
        }
        public virtual bool ForceRemember
        {
            get
            {
                return false;
            }
        }
		
        public virtual int AcceptSound
        {
            get
            {
                return 0x5B4;
            }
        }
        public virtual int ResignSound
        {
            get
            {
                return 0x5B3;
            }
        }
        public virtual int CompleteSound
        {
            get
            {
                return 0x5B5;
            }
        }
        public virtual int UpdateSound
        {
            get
            {
                return 0x5B6;
            }
        }
		
        #region Quest Chain
        public virtual QuestChain ChainID
        {
            get
            {
                return QuestChain.None;
            }
        }
        public virtual Type NextQuest
        {
            get
            {
                return null;
            }
        }
        #endregion
		
        public virtual object Title
        {
            get
            {
                return null;
            }
        }
        public virtual object Description
        {
            get
            {
                return null;
            }
        }
        public virtual object Refuse
        {
            get
            {
                return null;
            }
        }
        public virtual object Uncomplete
        {
            get
            {
                return null;
            }
        }
        public virtual object Complete
        {
            get
            {
                return null;
            }
        }

        public virtual object FailedMsg { get { return null; } }

        public virtual bool ShowDescription { get { return true; } }
        public virtual bool CanRefuseReward { get { return false; } }
		
        private List<BaseObjective> m_Objectives;		
        private List<BaseReward> m_Rewards;
        private PlayerMobile m_Owner;
        private object m_Quester;
        private Timer m_Timer;

        public List<BaseObjective> Objectives
        {
            get
            {
                return this.m_Objectives;
            }
        }
		
        public List<BaseReward> Rewards
        {
            get
            {
                return this.m_Rewards;
            }
        }
		
        public PlayerMobile Owner
        {
            get
            {
                return this.m_Owner;
            }
            set
            {
                this.m_Owner = value;
            }
        }
		
        public object Quester
        {
            get
            {
                return this.m_Quester;
            }
            set
            {
                this.m_Quester = value;
            }
        }
		
        public BaseQuestItem StartingItem
        {
            get
            {
                return this.m_Quester is BaseQuestItem ? (BaseQuestItem)this.m_Quester : null;
            }
        }
		
        public MondainQuester StartingMobile
        {
            get
            {
                return this.m_Quester is MondainQuester ? (MondainQuester)this.m_Quester : null;
            }
        }
		
        public bool Completed
        {
            get
            {
                for (int i = 0; i < this.m_Objectives.Count; i++)
                {
                    if (this.m_Objectives[i].Completed)
                    {
                        if (!this.AllObjectives)
                            return true;
                    }
                    else
                    {
                        if (this.AllObjectives)
                            return false;
                    }
                }

                return this.AllObjectives;
            }
        }
		
        public bool Failed
        {
            get
            {
                for (int i = 0; i < this.m_Objectives.Count; i++)
                {
                    if (this.m_Objectives[i].Failed)
                    {
                        if (this.AllObjectives)
                            return true;
                    }
                    else
                    {
                        if (!this.AllObjectives)
                            return false;
                    }
                }
				
                return !this.AllObjectives;
            }
        }
		
        public BaseQuest()
        { 
            this.m_Objectives = new List<BaseObjective>();
            this.m_Rewards = new List<BaseReward>();
        }
		
        public void StartTimer()
        {
            if (this.m_Timer != null)
                return;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(Slice));
        }
		
        public void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }
		
        public virtual void Slice()
        {
            for (int i = 0; i < this.m_Objectives.Count; i ++)
            {
                BaseObjective obj = this.m_Objectives[i];

                obj.UpdateTime();
            }
        }
		
        public virtual bool CanOffer()
        {
            return true;
        }
		
        public virtual void UpdateChain()
        {
            if (this.ChainID != QuestChain.None && this.StartingMobile != null)
            {
                if (this.m_Owner.Chains.ContainsKey(this.ChainID))
                {
                    BaseChain chain = this.m_Owner.Chains[this.ChainID];
					
                    chain.CurrentQuest = this.GetType();
                    chain.Quester = this.StartingMobile.GetType();
                }
                else
                {
                    this.m_Owner.Chains.Add(this.ChainID, new BaseChain(this.GetType(), this.StartingMobile.GetType()));		
                }
            }
        }
		
        public virtual void OnAccept()
        {
            this.m_Owner.PlaySound(this.AcceptSound);
            this.m_Owner.SendLocalizedMessage(1049019); // You have accepted the Quest.
            this.m_Owner.Quests.Add(this);
			
            // give items if any		
            for (int i = 0; i < this.m_Objectives.Count; i ++)
            {
                BaseObjective objective = this.m_Objectives[i];
				
                objective.OnAccept();
            }
			
            if (this.m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)this.m_Quester;
				
                if (escort.SetControlMaster(this.m_Owner))
                {
                    escort.Quest = this;					
                    escort.LastSeenEscorter = DateTime.UtcNow;
                    escort.StartFollow();
                    escort.AddHash(this.Owner);

                    Region region = escort.GetDestination();

                    if (region != null)
                        escort.Say(1042806, region.Name); // Lead on! Payment will be made when we arrive at ~1_DESTINATION~!
                    else
                        escort.Say(1042806, "destination"); // Lead on! Payment will be made when we arrive at ~1_DESTINATION~!

                    this.m_Owner.LastEscortTime = DateTime.UtcNow;
                }
            }
			
            // tick tack	
            this.StartTimer();
        }
		
        public virtual void OnRefuse()
        { 
            if (!QuestHelper.FirstChainQuest(this, this.Quester))
                this.UpdateChain();			
        }
		
        public virtual void OnResign(bool resignChain)
        { 
            this.m_Owner.PlaySound(this.ResignSound);	
			
            // update chain
            if (!resignChain && !QuestHelper.FirstChainQuest(this, this.Quester))
                this.UpdateChain();	
										
            // delete items	that were given on quest start
            for (int i = 0; i < this.m_Objectives.Count; i ++)
            { 
                if (this.m_Objectives[i] is ObtainObjective)
                {
                    ObtainObjective obtain = (ObtainObjective)this.m_Objectives[i];
					
                    QuestHelper.RemoveStatus(this.m_Owner, obtain.Obtain);
                }
                else if (this.m_Objectives[i] is DeliverObjective)
                {
                    DeliverObjective deliver = (DeliverObjective)this.m_Objectives[i];
					
                    QuestHelper.DeleteItems(this.m_Owner, deliver.Delivery, deliver.MaxProgress, true);
                }
            }
			
            // delete escorter
            if (this.m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)this.m_Quester;

                escort.Say(1005653); // Hmmm.  I seem to have lost my master.
                escort.PlaySound(0x5B3);
                escort.BeginDelete(this.m_Owner);
            }
			
            this.RemoveQuest(resignChain);
        }
		
        public virtual void InProgress()
        {
        }
		
        public virtual void OnCompleted()
        { 
            this.m_Owner.SendLocalizedMessage(1072273, null, 0x23); // You've completed a quest!  Don't forget to collect your reward.							
            this.m_Owner.PlaySound(this.CompleteSound);
        }
		
        public virtual void GiveRewards()
        { 
            // give rewards
            for (int i = 0; i < this.m_Rewards.Count; i ++)
            { 
                Type type = this.m_Rewards[i].Type;
				
                this.m_Rewards[i].GiveReward();
				
                if (type == null)
                    continue;
				
                Item reward;
				
                try
                {
                    reward = Activator.CreateInstance(type) as Item;
                }
                catch
                {
                    reward = null;
                }
				
                if (reward != null)
                { 
                    if (reward.Stackable)
                    {
                        reward.Amount = this.m_Rewards[i].Amount;
                        this.m_Rewards[i].Amount = 1;
                    }
					
                    for (int j = 0; j < this.m_Rewards[i].Amount; j ++)
                    {
                        if (!this.m_Owner.PlaceInBackpack(reward))
                        {
                            reward.MoveToWorld(this.m_Owner.Location);
                        }
						
                        if (this.m_Rewards[i].Name is int)
                            this.m_Owner.SendLocalizedMessage(1074360, "#" + (int)this.m_Rewards[i].Name); // You receive a reward: ~1_REWARD~
                        else if (this.m_Rewards[i].Name is string)
                            this.m_Owner.SendLocalizedMessage(1074360, (string)this.m_Rewards[i].Name); // You receive a reward: ~1_REWARD~		
                    }
                }
            }
			
            // remove quest
            if (this.NextQuest == null)
                this.RemoveQuest(true);
            else
                this.RemoveQuest();
			
            // offer next quest if present
            if (this.NextQuest != null)
            {
                BaseQuest quest = QuestHelper.RandomQuest(this.m_Owner, new Type[] { this.NextQuest }, this.StartingMobile);
					
                if (quest != null && quest.ChainID == this.ChainID)
                    this.m_Owner.SendGump(new MondainQuestGump(quest));
            }

            Server.Engines.Points.PointsSystem.HandleQuest(Owner, this);
        }

        public virtual void RefuseRewards()
        {
            // remove quest
            if (NextQuest == null)
                RemoveQuest(true);
            else
                RemoveQuest();

            // offer next quest if present
            if (NextQuest != null)
            {
                BaseQuest quest = QuestHelper.RandomQuest(m_Owner, new Type[] { NextQuest }, StartingMobile);

                if (quest != null && quest.ChainID == ChainID)
                    m_Owner.SendGump(new MondainQuestGump(quest));
            }
        }
		
        public virtual void AddObjective(BaseObjective objective)
        {
            if (this.m_Objectives == null)
                this.m_Objectives = new List<BaseObjective>();
			
            if (objective != null)
            {
                objective.Quest = this;
                this.m_Objectives.Add(objective);
            }
        }
		
        public virtual void AddReward(BaseReward reward)
        {
            if (this.m_Rewards == null)
                this.m_Rewards = new List<BaseReward>();
				
            if (reward != null)
            {
                reward.Quest = this;
                this.m_Rewards.Add(reward);
            }
        }
		
        public virtual void RemoveQuest()
        {
            this.RemoveQuest(false);
        }
		
        public virtual void RemoveQuest(bool removeChain)
        {
            this.StopTimer();
			
            if (removeChain)
                this.m_Owner.Chains.Remove(this.ChainID);
			
            if (this.Completed && (this.RestartDelay > TimeSpan.Zero || this.ForceRemember || this.DoneOnce) && this.NextQuest == null)
            {
                Type type = this.GetType();	
				
                if (this.ChainID != QuestChain.None)
                    type = QuestHelper.FindFirstChainQuest(this);

                QuestHelper.Delay(this.Owner, type, this.RestartDelay);
            }
			
            QuestHelper.RemoveAcceleratedSkillgain(this.Owner);
				
            for (int i = this.m_Owner.Quests.Count - 1; i >= 0; i --)
            {
                if (this.m_Owner.Quests[i] == this)
                {
                    this.m_Owner.Quests.RemoveAt(i);
					
                    break;
                }
            }
        }

        public virtual bool RenderDescription(MondainQuestGump g, bool offer)
        {
            return false;
        }

        public virtual bool RenderObjective(MondainQuestGump g, bool offer)
        {
            return false;
        }
		
        public virtual void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version	
			
            if (this.m_Quester == null)
                writer.Write((int)0x0);
            else if (this.m_Quester is Mobile)
            {
                writer.Write((int)0x1);
                writer.Write((Mobile)this.m_Quester);
            }
            else if (this.m_Quester is Item)
            {
                writer.Write((int)0x2);
                writer.Write((Item)this.m_Quester);
            }
							
            for (int i = 0; i < this.m_Objectives.Count; i ++)
            {
                BaseObjective objective = this.m_Objectives[i];
				
                objective.Serialize(writer);
            }
        }
		
        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
			
            switch ( reader.ReadInt() )
            {
                case 0x0:
                    this.m_Quester = null;
                    break;
                case 0x1:
                    this.m_Quester = reader.ReadMobile() as MondainQuester;
                    break;
                case 0x2:
                    this.m_Quester = reader.ReadItem() as BaseQuestItem;
                    break;
            }
			
            if (this.m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)this.m_Quester;
				
                escort.Quest = this;
            }
            else if (this.m_Quester is BaseQuestItem)
            {
                BaseQuestItem item = (BaseQuestItem)this.m_Quester;
				
                item.Quest = this;
            }
			
            for (int i = 0; i < this.m_Objectives.Count; i ++)
            {
                BaseObjective objective = this.m_Objectives[i];
				
                objective.Deserialize(reader);
            }
        }
    }
}