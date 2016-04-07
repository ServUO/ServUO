using System;
using Server.Gumps;

namespace Server.Engines.Quests
{
    public class QuestItemInfo
    {
        private object m_Name;
        private int m_ItemID;
        public QuestItemInfo(object name, int itemID)
        {
            this.m_Name = name;
            this.m_ItemID = itemID;
        }

        public object Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.m_Name = value;
            }
        }
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
            set
            {
                this.m_ItemID = value;
            }
        }
    }

    public class QuestItemInfoGump : BaseQuestGump
    {
        public QuestItemInfoGump(QuestItemInfo[] info)
            : base(485, 75)
        {
            int height = 100 + (info.Length * 75);

            this.AddPage(0);

            this.AddBackground(5, 10, 145, height, 5054);

            this.AddImageTiled(13, 20, 125, 10, 2624);
            this.AddAlphaRegion(13, 20, 125, 10);

            this.AddImageTiled(13, height - 10, 128, 10, 2624);
            this.AddAlphaRegion(13, height - 10, 128, 10);

            this.AddImageTiled(13, 20, 10, height - 30, 2624);
            this.AddAlphaRegion(13, 20, 10, height - 30);

            this.AddImageTiled(131, 20, 10, height - 30, 2624);
            this.AddAlphaRegion(131, 20, 10, height - 30);

            this.AddHtmlLocalized(67, 35, 120, 20, 1011233, White, false, false); // INFO

            this.AddImage(62, 52, 9157);
            this.AddImage(72, 52, 9157);
            this.AddImage(82, 52, 9157);

            this.AddButton(25, 31, 1209, 1210, 777, GumpButtonType.Reply, 0);

            this.AddPage(1);

            for (int i = 0; i < info.Length; ++i)
            {
                QuestItemInfo cur = info[i];

                this.AddHtmlObject(25, 65 + (i * 75), 110, 20, cur.Name, 1153, false, false);
                this.AddItem(45, 85 + (i * 75), cur.ItemID);
            }
        }
    }
}