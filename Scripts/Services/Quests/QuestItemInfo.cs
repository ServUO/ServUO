using Server.Gumps;

namespace Server.Engines.Quests
{
    public class QuestItemInfo
    {
        private object m_Name;
        private int m_ItemID;
        public QuestItemInfo(object name, int itemID)
        {
            m_Name = name;
            m_ItemID = itemID;
        }

        public object Name
        {
            get
            {
                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }
        public int ItemID
        {
            get
            {
                return m_ItemID;
            }
            set
            {
                m_ItemID = value;
            }
        }
    }

    public class QuestItemInfoGump : BaseQuestGump
    {
        public QuestItemInfoGump(QuestItemInfo[] info)
            : base(485, 75)
        {
            int height = 100 + (info.Length * 75);

            AddPage(0);

            AddBackground(5, 10, 145, height, 5054);

            AddImageTiled(13, 20, 125, 10, 2624);
            AddAlphaRegion(13, 20, 125, 10);

            AddImageTiled(13, height - 10, 128, 10, 2624);
            AddAlphaRegion(13, height - 10, 128, 10);

            AddImageTiled(13, 20, 10, height - 30, 2624);
            AddAlphaRegion(13, 20, 10, height - 30);

            AddImageTiled(131, 20, 10, height - 30, 2624);
            AddAlphaRegion(131, 20, 10, height - 30);

            AddHtmlLocalized(67, 35, 120, 20, 1011233, White, false, false); // INFO

            AddImage(62, 52, 9157);
            AddImage(72, 52, 9157);
            AddImage(82, 52, 9157);

            AddButton(25, 31, 1209, 1210, 777, GumpButtonType.Reply, 0);

            AddPage(1);

            for (int i = 0; i < info.Length; ++i)
            {
                QuestItemInfo cur = info[i];

                AddHtmlObject(25, 65 + (i * 75), 110, 20, cur.Name, 1153, false, false);
                AddItem(45, 85 + (i * 75), cur.ItemID);
            }
        }
    }
}