using Server.Engines.Quests;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ShrineOfSingularity : Item
    {
        private static readonly TimeSpan FailDelay = TimeSpan.FromHours(2);

        [Constructable]
        public ShrineOfSingularity() : base(0x48A8)
        {
            Movable = false;
            Name = "Shrine Of Singularity";
        }

        public ShrineOfSingularity(Serial serial)
            : base(serial)
        {
        }

        public override bool HandlesOnSpeech => true;
        public override void OnSpeech(SpeechEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm.AbyssEntry)
            {
                pm.SendLocalizedMessage(1112697);  //You enter a state of peaceful contemplation, focusing on the meaning of Singularity.
            }
            else if (!e.Handled && pm.InRange(Location, 2) && e.Speech.ToLower().Trim() == "unorus" && QuestHelper.CheckDoneOnce(pm, typeof(TheArisenQuest), null, false))
            {
                e.Handled = true;
                e.Mobile.PlaySound(0xF9);

                var quest = QuestHelper.GetQuest<QuestOfSingularity>(pm);

                if (HasDelay(pm) && pm.AccessLevel == AccessLevel.Player)
                {
                    pm.PublicOverheadMessage(MessageType.Regular, 0x47E, 1112685); // You need more time to contemplate the Book of Circles before trying again.
                }
                else if (quest == null)
                {
                    quest = new QuestOfSingularity
                    {
                        Owner = pm,
                        Quester = this
                    };

                    pm.SendGump(new MondainQuestGump(quest));
                }
                else if (quest.Completed)
                {
                    pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                }
                else if (!pm.HasGump(typeof(QAndAGump)))
                {
                    pm.SendGump(new QAndAGump(pm, quest));
                }
            }
        }

        private static readonly Dictionary<Mobile, DateTime> m_RestartTable = new Dictionary<Mobile, DateTime>();

        public static void AddToTable(Mobile from)
        {
            m_RestartTable[from] = DateTime.UtcNow + FailDelay;
        }

        public static bool HasDelay(Mobile from)
        {
            if (m_RestartTable.ContainsKey(from))
            {
                if (m_RestartTable[from] < DateTime.UtcNow)
                    m_RestartTable.Remove(from);
            }

            return m_RestartTable.ContainsKey(from);
        }

        public static void DefragDelays_Callback()
        {
            List<Mobile> list = new List<Mobile>(m_RestartTable.Keys);

            foreach (Mobile mob in list)
            {
                if (m_RestartTable[mob] < DateTime.UtcNow)
                    m_RestartTable.Remove(mob);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version

            Timer.DelayCall(TimeSpan.FromSeconds(10), DefragDelays_Callback);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            Instance = this;

            if (version == 0)
            {
                Item book = new BookOfCircles
                {
                    Movable = false
                };
                book.MoveToWorld(new Point3D(1000, 3991, -33), Map.TerMur);

                book = new ShrineMantra
                {
                    Movable = false
                };
                book.MoveToWorld(new Point3D(994, 3991, -33), Map.TerMur);
            }

            if (version == 1)
            {
                Timer.DelayCall(() => SpawnerPersistence.Delete("shrineofsingularity"));
            }
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
