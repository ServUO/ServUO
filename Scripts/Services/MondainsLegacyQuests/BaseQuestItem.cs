using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;

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
            LootType = LootType.Blessed;

            if (Lifespan > 0)
                StartTimer();
        }

        public BaseQuestItem(Serial serial)
            : base(serial)
        {
        }

        public virtual Type[] Quests => null;
        public virtual int Lifespan => 0;
        public int Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                m_Duration = value;
                InvalidateProperties();
            }
        }
        public BaseQuest Quest
        {
            get
            {
                return m_Quest;
            }
            set
            {
                m_Quest = value;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack) && Movable)
            {
                from.SendLocalizedMessage(1060640); // The item must be in your backpack to use it.
                return;
            }

            if (!(from is PlayerMobile))
                return;

            PlayerMobile player = (PlayerMobile)from;

            if (QuestHelper.InProgress(player, Quests))
                return;

            if (QuestHelper.QuestLimitReached(player))
                return;

            // check if this quester can offer quest chain (already started)
            foreach (KeyValuePair<QuestChain, BaseChain> pair in player.Chains)
            {
                BaseChain chain = pair.Value;

                if (chain != null && chain.Quester != null && chain.Quester.IsAssignableFrom(GetType()))
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

            BaseQuest questt = QuestHelper.RandomQuest(player, Quests, this);

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

            if (m_Duration > 0)
                list.Add(1072517, m_Duration.ToString()); // Lifespan: ~1_val~ seconds

            if (!QuestItem)
                list.Add(1072351); // Quest Item
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Duration);
            writer.Write(m_InDelivery);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Duration = reader.ReadInt();
            m_InDelivery = reader.ReadBool();

            if (m_Duration > 0)
                StartTimer();
        }

        public virtual void StartTimer()
        {
            if (m_Timer != null)
                return;

            m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10), Slice);
        }

        public virtual void StopTimer()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;
        }

        public virtual void Slice()
        {
            if (m_Duration + 10 < Lifespan)
                m_Duration += 10;
            else
            {
                StopTimer();

                if (Parent is Backpack)
                {
                    Backpack pack = (Backpack)Parent;

                    if (pack.Parent is PlayerMobile)
                        QuestHelper.RemoveStatus((PlayerMobile)pack.Parent, this);
                }

                Delete();
            }
        }
    }
}
