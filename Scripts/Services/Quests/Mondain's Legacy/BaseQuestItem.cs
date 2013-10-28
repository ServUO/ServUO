using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public abstract class BaseQuestItem : Item
    {
        private bool m_InDelivery;
        private int m_Duration;
        private BaseQuest m_Quest;
        private Timer m_Timer;
        public BaseQuestItem(int itemID)
            : base(itemID)
        {
            this.LootType = LootType.Blessed;
			
            if (this.Lifespan > 0)				
                this.StartTimer();
        }

        public BaseQuestItem(Serial serial)
            : base(serial)
        {
        }

        public virtual Type[] Quests
        {
            get
            {
                return null;
            }
        }
        public virtual int Lifespan
        {
            get
            {
                return 0;
            }
        }
        public int Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
                this.InvalidateProperties();
            }
        }
        public BaseQuest Quest
        {
            get
            {
                return this.m_Quest;
            }
            set
            {
                this.m_Quest = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        { 
            if (!this.IsChildOf(from.Backpack) && this.Movable)
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                return;
            }
			
            if (!(from is PlayerMobile))
                return;
			
            PlayerMobile player = (PlayerMobile)from;
			
            if (QuestHelper.InProgress(player, this.Quests))
                return;
			
            if (QuestHelper.QuestLimitReached(player))
                return;			
			
            // check if this quester can offer quest chain (already started)
            foreach (KeyValuePair<QuestChain, BaseChain> pair in player.Chains)
            {
                BaseChain chain = pair.Value;
																			
                if (chain != null && chain.Quester != null && chain.Quester.IsAssignableFrom(this.GetType()))
                {
                    BaseQuest quest = QuestHelper.RandomQuest(player, new Type[] { chain.CurrentQuest }, this);
					
                    if (quest != null)
                    {
                        player.CloseGump(typeof(MondainQuestGump));
                        player.SendGump(new MondainQuestGump(quest));
                        return;
                    }
                }
            }
				
            BaseQuest questt = QuestHelper.RandomQuest(player, this.Quests, this);
					
            if (questt != null)
            {
                player.CloseGump(typeof(MondainQuestGump));
                player.SendGump(new MondainQuestGump(questt));
            }
            else
                player.SendLocalizedMessage(1075141); // You are too busy with other tasks at this time.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
				
            if (this.m_Duration > 0)
                list.Add(1072517, this.m_Duration.ToString()); // Lifespan: ~1_val~ seconds
				
            if (!this.QuestItem)
                list.Add(1072351); // Quest Item
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((int)this.m_Duration);
            writer.Write((bool)this.m_InDelivery);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Duration = reader.ReadInt();
            this.m_InDelivery = reader.ReadBool();
			
            if (this.m_Duration > 0)
                this.StartTimer();
        }

        public virtual void StartTimer()
        {
            if (this.m_Timer != null)
                return;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), new TimerCallback(Slice));
        }

        public virtual void StopTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;
        }

        public virtual void Slice()
        {
            if (this.m_Duration + 10 < this.Lifespan)
                this.m_Duration += 10;
            else
            { 
                this.StopTimer();
				
                if (this.Parent is Backpack)
                {
                    Backpack pack = (Backpack)this.Parent;
					
                    if (pack.Parent is PlayerMobile)
                        QuestHelper.RemoveStatus((PlayerMobile)pack.Parent, this);				
                }
				
                this.Delete();
            }
        }
    }
}