using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Engines.Quests;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShrineOfSingularity : Item
    {
        private static readonly TimeSpan FailDelay = TimeSpan.FromHours(24);

        [Constructable]
        public ShrineOfSingularity() : base(0x48A8)
        {
            this.Movable = false;
            this.Name = "Shrine Of Singularity";	
        }

        public ShrineOfSingularity(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech
        {
            get
            {
                return true;
            }
        }
        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            PlayerMobile pm = from as PlayerMobile;

            if (pm is PlayerMobile && !e.Handled && from.InRange(Location, 2) && e.Speech.ToLower().Trim() == "unorus")
            {
                e.Handled = true;
                e.Mobile.PlaySound(0xF9);

                QuestOfSingularity quest = GetSingularityQuest(pm);

                if (HasDelay(pm) && pm.AccessLevel == AccessLevel.Player)
                    pm.SendLocalizedMessage(1112685); //You need more time to contemplate the Book of Circles before trying again.
                else if (pm.AbyssEntry)
                    pm.SendLocalizedMessage(1112697);  //You enter a state of peaceful contemplation, focusing on the meaning of Singularity.
                else if (quest == null)
                {
                    quest = new QuestOfSingularity();

                    quest.Owner = pm;
                    quest.Quester = this;

                    pm.SendGump(new MondainQuestGump(quest));
                }
                else if (quest.Completed)
                    from.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                else if (!pm.HasGump(typeof(QAndAGump)))
                    pm.SendGump(new QAndAGump(pm, quest));
            }
        }

        private QuestOfSingularity GetSingularityQuest(PlayerMobile pm)
        {
            return QuestHelper.GetQuest(pm, typeof(QuestOfSingularity)) as QuestOfSingularity;
        }

        private static Dictionary<Mobile, DateTime> m_RestartTable = new Dictionary<Mobile, DateTime>();

        public static void AddToTable(Mobile from)
        {
            m_RestartTable[from] = DateTime.Now + FailDelay;
        }

        public static bool HasDelay(Mobile from)
        {
            if (m_RestartTable.ContainsKey(from))
            {
                if (m_RestartTable[from] < DateTime.Now)
                    m_RestartTable.Remove(from);
            }

            return m_RestartTable.ContainsKey(from);
        }

        public static void DefragDelays_Callback()
        {
            List<Mobile> list = new List<Mobile>(m_RestartTable.Keys);

            foreach (Mobile mob in list)
            {
                if (m_RestartTable[mob] < DateTime.Now)
                    m_RestartTable.Remove(mob);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(DefragDelays_Callback));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Instance = this;
        }

        public static ShrineOfSingularity Instance { get; set; }

        public static void Initialize()
        {
            if (Instance == null)
            {
                Instance = new ShrineOfSingularity();
                Instance.MoveToWorld(new Point3D(995, 3802, -19), Map.TerMur);
            }
        }
    }
}