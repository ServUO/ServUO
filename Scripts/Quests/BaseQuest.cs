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

        public virtual int CompleteMessage
        {
            get
            {
                return 1072273; // You've completed a quest!  Don't forget to collect your reward.
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
        private Type m_QuesterType;
        private Timer m_Timer;

        public List<BaseObjective> Objectives
        {
            get
            {
                return m_Objectives;
            }
        }
		
        public List<BaseReward> Rewards
        {
            get
            {
                return m_Rewards;
            }
        }
		
        public PlayerMobile Owner
        {
            get
            {
                return m_Owner;
            }
            set
            {
                m_Owner = value;
            }
        }
		
        public object Quester
        {
            get
            {
                return m_Quester;
            }
            set
            {
                m_Quester = value;

                if (m_Quester != null)
                    m_QuesterType = m_Quester.GetType();
            }
        }

        public Type QuesterType
        {
            get
            {
                return m_QuesterType;
            }
            set
            {
                m_QuesterType = value;
            }
        }
		
        public BaseQuestItem StartingItem
        {
            get
            {
                return m_Quester is BaseQuestItem ? (BaseQuestItem)m_Quester : null;
            }
        }
		
        public MondainQuester StartingMobile
        {
            get
            {
                return m_Quester is MondainQuester ? (MondainQuester)m_Quester : null;
            }
        }
		
        public bool Completed
        {
            get
            {
                for (int i = 0; i < m_Objectives.Count; i++)
                {
                    if (m_Objectives[i].Completed)
                    {
                        if (!AllObjectives)
                            return true;
                    }
                    else
                    {
                        if (AllObjectives)
                            return false;
                    }
                }

                return AllObjectives;
            }
        }
		
        public bool Failed
        {
            get
            {
                for (int i = 0; i < m_Objectives.Count; i++)
                {
                    if (m_Objectives[i].Failed)
                    {
                        if (AllObjectives)
                            return true;
                    }
                    else
                    {
                        if (!AllObjectives)
                            return false;
                    }
                }
				
                return !AllObjectives;
            }
        }
		
        public BaseQuest()
        { 
            m_Objectives = new List<BaseObjective>();
            m_Rewards = new List<BaseReward>();
        }
		
        public void StartTimer()
        {
            if (m_Timer != null)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), new TimerCallback(Slice));
        }
		
        public void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }
		
        public virtual void Slice()
        {
            for (int i = 0; i < m_Objectives.Count; i ++)
            {
                BaseObjective obj = m_Objectives[i];

                obj.UpdateTime();
            }
        }

        public virtual void OnObjectiveUpdate(Item item)
        {
        }
		
        public virtual bool CanOffer()
        {
            return true;
        }
		
        public virtual void UpdateChain()
        {
            if (ChainID != QuestChain.None && StartingMobile != null)
            {
                if (m_Owner.Chains.ContainsKey(ChainID))
                {
                    BaseChain chain = m_Owner.Chains[ChainID];
					
                    chain.CurrentQuest = GetType();
                    chain.Quester = StartingMobile.GetType();
                }
                else
                {
                    m_Owner.Chains.Add(ChainID, new BaseChain(GetType(), StartingMobile.GetType()));		
                }
            }
        }
		
        public virtual void OnAccept()
        {
            m_Owner.PlaySound(AcceptSound);
            m_Owner.SendLocalizedMessage(1049019); // You have accepted the Quest.
            m_Owner.Quests.Add(this);
			
            // give items if any		
            for (int i = 0; i < m_Objectives.Count; i ++)
            {
                BaseObjective objective = m_Objectives[i];
				
                objective.OnAccept();
            }
			
            if (m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)m_Quester;
				
                if (escort.SetControlMaster(m_Owner))
                {
                    escort.Quest = this;					
                    escort.LastSeenEscorter = DateTime.UtcNow;
                    escort.StartFollow();
                    escort.AddHash(Owner);

                    Region region = escort.GetDestination();

                    if (region != null)
                        escort.Say(1042806, region.Name); // Lead on! Payment will be made when we arrive at ~1_DESTINATION~!
                    else
                        escort.Say(1042806, "destination"); // Lead on! Payment will be made when we arrive at ~1_DESTINATION~!

                    m_Owner.LastEscortTime = DateTime.UtcNow;
                }
            }
			
            // tick tack	
            StartTimer();
        }
		
        public virtual void OnRefuse()
        { 
            if (!QuestHelper.FirstChainQuest(this, Quester))
                UpdateChain();			
        }
		
        public virtual void OnResign(bool resignChain)
        { 
            m_Owner.PlaySound(ResignSound);	
			
            // update chain
            if (!resignChain && !QuestHelper.FirstChainQuest(this, Quester))
                UpdateChain();	
										
            // delete items	that were given on quest start
            for (int i = 0; i < m_Objectives.Count; i ++)
            { 
                if (m_Objectives[i] is ObtainObjective)
                {
                    ObtainObjective obtain = (ObtainObjective)m_Objectives[i];
					
                    QuestHelper.RemoveStatus(m_Owner, obtain.Obtain);
                }
                else if (m_Objectives[i] is DeliverObjective)
                {
                    DeliverObjective deliver = (DeliverObjective)m_Objectives[i];
					
                    QuestHelper.DeleteItems(m_Owner, deliver.Delivery, deliver.MaxProgress, true);
                }
            }
			
            // delete escorter
            if (m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)m_Quester;

                escort.Say(1005653); // Hmmm.  I seem to have lost my master.
                escort.PlaySound(0x5B3);
                escort.BeginDelete(m_Owner);
            }
			
            RemoveQuest(resignChain);
        }
		
        public virtual void InProgress()
        {
        }
		
        public virtual void OnCompleted()
        { 
            m_Owner.SendLocalizedMessage(CompleteMessage, null, 0x23); // You've completed a quest!  Don't forget to collect your reward.							
            m_Owner.PlaySound(CompleteSound);
        }
		
        public virtual void GiveRewards()
        { 
            // give rewards
            for (int i = 0; i < m_Rewards.Count; i ++)
            { 
                Type type = m_Rewards[i].Type;
				
                m_Rewards[i].GiveReward();
				
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
                        reward.Amount = m_Rewards[i].Amount;
                        m_Rewards[i].Amount = 1;
                    }
					
                    for (int j = 0; j < m_Rewards[i].Amount; j ++)
                    {
                        if (!m_Owner.PlaceInBackpack(reward))
                        {
                            reward.MoveToWorld(m_Owner.Location, m_Owner.Map);
                        }
						
                        if (m_Rewards[i].Name is int)
                            m_Owner.SendLocalizedMessage(1074360, "#" + (int)m_Rewards[i].Name); // You receive a reward: ~1_REWARD~
                        else if (m_Rewards[i].Name is string)
                            m_Owner.SendLocalizedMessage(1074360, (string)m_Rewards[i].Name); // You receive a reward: ~1_REWARD~		
                    }
                }
            }
			
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
            if (m_Objectives == null)
                m_Objectives = new List<BaseObjective>();
			
            if (objective != null)
            {
                objective.Quest = this;
                m_Objectives.Add(objective);
            }
        }
		
        public virtual void AddReward(BaseReward reward)
        {
            if (m_Rewards == null)
                m_Rewards = new List<BaseReward>();
				
            if (reward != null)
            {
                reward.Quest = this;
                m_Rewards.Add(reward);
            }
        }
		
        public virtual void RemoveQuest()
        {
            RemoveQuest(false);
        }
		
        public virtual void RemoveQuest(bool removeChain)
        {
            StopTimer();
			
            if (removeChain)
                m_Owner.Chains.Remove(ChainID);
			
            if (Completed && (RestartDelay > TimeSpan.Zero || ForceRemember || DoneOnce) && NextQuest == null)
            {
                Type type = GetType();	
				
                if (ChainID != QuestChain.None)
                    type = QuestHelper.FindFirstChainQuest(this);

                QuestHelper.Delay(Owner, type, RestartDelay);
            }
			
            QuestHelper.RemoveAcceleratedSkillgain(Owner);
				
            for (int i = m_Owner.Quests.Count - 1; i >= 0; i --)
            {
                if (m_Owner.Quests[i] == this)
                {
                    m_Owner.Quests.RemoveAt(i);
					
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
            writer.WriteEncodedInt((int)1); // version	

            writer.Write(m_QuesterType == null ? null : m_QuesterType.Name);

            if (m_Quester == null)
                writer.Write((int)0x0);
            else if (m_Quester is Mobile)
            {
                writer.Write((int)0x1);
                writer.Write((Mobile)m_Quester);
            }
            else if (m_Quester is Item)
            {
                writer.Write((int)0x2);
                writer.Write((Item)m_Quester);
            }
							
            for (int i = 0; i < m_Objectives.Count; i ++)
            {
                BaseObjective objective = m_Objectives[i];
				
                objective.Serialize(writer);
            }
        }
		
        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            if (version > 0)
            {
                string questerType = reader.ReadString();

                if(questerType != null)
                    m_QuesterType = ScriptCompiler.FindTypeByName(questerType);
            }

            switch ( reader.ReadInt() )
            {
                case 0x0:
                    m_Quester = null;
                    break;
                case 0x1:
                    m_Quester = reader.ReadMobile() as MondainQuester;
                    break;
                case 0x2:
                    m_Quester = reader.ReadItem() as BaseQuestItem;
                    break;
            }
			
            if (m_Quester is BaseEscort)
            {
                BaseEscort escort = (BaseEscort)m_Quester;
				
                escort.Quest = this;
            }
            else if (m_Quester is BaseQuestItem)
            {
                BaseQuestItem item = (BaseQuestItem)m_Quester;
				
                item.Quest = this;
            }

            if (version == 0 && m_Quester != null)
            {
                m_QuesterType = m_Quester.GetType();
            }
			
            for (int i = 0; i < m_Objectives.Count; i ++)
            {
                BaseObjective objective = m_Objectives[i];
				
                objective.Deserialize(reader);
            }
        }
    }
}