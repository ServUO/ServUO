using Server.Commands;
using Server.Network;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Server.Gumps
{
    public abstract class CAGNode
    {
        public abstract string Caption { get; }
        public abstract void OnClick(Mobile from, int page);
    }

    public class CAGObject : CAGNode
    {
        private readonly Type m_Type;
        private readonly int m_ItemID;
        private readonly int m_Hue;
        private readonly CAGCategory m_Parent;
        public CAGObject(CAGCategory parent, XmlTextReader xml)
        {
            m_Parent = parent;

            if (xml.MoveToAttribute("type"))
                m_Type = ScriptCompiler.FindTypeByFullName(xml.Value, false);

            if (xml.MoveToAttribute("gfx"))
                m_ItemID = XmlConvert.ToInt32(xml.Value);

            if (xml.MoveToAttribute("hue"))
                m_Hue = XmlConvert.ToInt32(xml.Value);
        }

        public Type Type => m_Type;
        public int ItemID => m_ItemID;
        public int Hue => m_Hue;
        public CAGCategory Parent => m_Parent;
        public override string Caption => (m_Type == null ? "bad type" : m_Type.Name);
        public override void OnClick(Mobile from, int page)
        {
            if (m_Type == null)
            {
                from.SendMessage("That is an invalid type name.");
            }
            else
            {
                CommandSystem.Handle(from, string.Format("{0}Add {1}", CommandSystem.Prefix, m_Type.Name));

                from.SendGump(new CategorizedAddGump(from, m_Parent, page));
            }
        }
    }

    public class CAGCategory : CAGNode
    {
        private static CAGCategory m_Root;
        private readonly string m_Title;
        private readonly CAGNode[] m_Nodes;
        private readonly CAGCategory m_Parent;
        public CAGCategory(CAGCategory parent, XmlTextReader xml)
        {
            m_Parent = parent;

            if (xml.MoveToAttribute("title"))
                m_Title = xml.Value;
            else
                m_Title = "empty";

            if (m_Title == "Docked")
                m_Title = "Docked 2";

            if (xml.IsEmptyElement)
            {
                m_Nodes = new CAGNode[0];
            }
            else
            {
                ArrayList nodes = new ArrayList();

                while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
                {
                    if (xml.NodeType == XmlNodeType.Element && xml.Name == "object")
                        nodes.Add(new CAGObject(this, xml));
                    else if (xml.NodeType == XmlNodeType.Element && xml.Name == "category")
                    {
                        if (!xml.IsEmptyElement)
                            nodes.Add(new CAGCategory(this, xml));
                    }
                    else
                        xml.Skip();
                }

                m_Nodes = (CAGNode[])nodes.ToArray(typeof(CAGNode));
            }
        }

        private CAGCategory()
        {
            m_Title = "no data";
            m_Nodes = new CAGNode[0];
        }

        public static CAGCategory Root
        {
            get
            {
                if (m_Root == null)
                    m_Root = Load("Data/objects.xml");

                return m_Root;
            }
        }
        public string Title => m_Title;
        public CAGNode[] Nodes => m_Nodes;
        public CAGCategory Parent => m_Parent;
        public override string Caption => m_Title;
        public static CAGCategory Load(string path)
        {
            if (File.Exists(path))
            {
                XmlTextReader xml = new XmlTextReader(path)
                {
                    WhitespaceHandling = WhitespaceHandling.None
                };

                while (xml.Read())
                {
                    if (xml.Name == "category" && xml.NodeType == XmlNodeType.Element)
                    {
                        CAGCategory cat = new CAGCategory(null, xml);

                        xml.Close();

                        return cat;
                    }
                }
            }

            return new CAGCategory();
        }

        public override void OnClick(Mobile from, int page)
        {
            from.SendGump(new CategorizedAddGump(from, this, 0));
        }
    }

    public class CategorizedAddGump : Gump
    {
        public static readonly int EntryHeight = 24;//PropsConfig.EntryHeight;
        public static readonly int OffsetSize = PropsConfig.OffsetSize;
        public static readonly int BorderSize = PropsConfig.BorderSize;
        public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
        public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;
        public static readonly int TextHue = PropsConfig.TextHue;
        public static readonly int TextOffsetX = PropsConfig.TextOffsetX;
        public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
        public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
        public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
        public static readonly int BackGumpID = PropsConfig.BackGumpID;
        public static readonly int SetGumpID = PropsConfig.SetGumpID;
        public static readonly int SetWidth = PropsConfig.SetWidth;
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY + (((EntryHeight - 20) / 2) / 2);
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;
        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY + (((EntryHeight - 20) / 2) / 2);
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;
        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY + (((EntryHeight - 20) / 2) / 2);
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;
        public static bool OldStyle = PropsConfig.OldStyle;
        private static readonly bool PrevLabel = false;
        private static readonly bool NextLabel = false;
        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
        private static readonly int PrevLabelOffsetY = 0;
        private static readonly int NextLabelOffsetX = -29;
        private static readonly int NextLabelOffsetY = 0;
        private static readonly int EntryWidth = 180;
        private static readonly int EntryCount = 15;
        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
        private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));
        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;
        private readonly Mobile m_Owner;
        private readonly CAGCategory m_Category;
        private int m_Page;
        public CategorizedAddGump(Mobile owner)
            : this(owner, CAGCategory.Root, 0)
        {
        }

        public CategorizedAddGump(Mobile owner, CAGCategory category, int page)
            : base(GumpOffsetX, GumpOffsetY)
        {
            owner.CloseGump(typeof(WhoGump));

            m_Owner = owner;
            m_Category = category;

            Initialize(page);
        }

        public void Initialize(int page)
        {
            m_Page = page;

            CAGNode[] nodes = m_Category.Nodes;

            int count = nodes.Length - (page * EntryCount);

            if (count < 0)
                count = 0;
            else if (count > EntryCount)
                count = EntryCount;

            int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

            AddPage(0);

            AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
            AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            if (OldStyle)
                AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            if (m_Category.Parent != null)
            {
                AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

                if (PrevLabel)
                    AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
            }

            x += PrevWidth + OffsetSize;

            int emptyWidth = TotalWidth - (PrevWidth * 2) - NextWidth - (OffsetSize * 5) - (OldStyle ? SetWidth + OffsetSize : 0);

            if (!OldStyle)
                AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

            AddHtml(x + TextOffsetX, y + ((EntryHeight - 20) / 2), emptyWidth - TextOffsetX, EntryHeight, string.Format("<center>{0}</center>", m_Category.Caption), false, false);

            x += emptyWidth + OffsetSize;

            if (OldStyle)
                AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            if (page > 0)
            {
                AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 2, GumpButtonType.Reply, 0);

                if (PrevLabel)
                    AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
            }

            x += PrevWidth + OffsetSize;

            if (!OldStyle)
                AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

            if ((page + 1) * EntryCount < nodes.Length)
            {
                AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 3, GumpButtonType.Reply, 1);

                if (NextLabel)
                    AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
            }

            for (int i = 0, index = page * EntryCount; i < EntryCount && index < nodes.Length; ++i, ++index)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                CAGNode node = nodes[index];

                AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                AddLabelCropped(x + TextOffsetX, y + ((EntryHeight - 20) / 2), EntryWidth - TextOffsetX, EntryHeight, TextHue, node.Caption);

                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 4, GumpButtonType.Reply, 0);

                if (node is CAGObject)
                {
                    CAGObject obj = (CAGObject)node;
                    int itemID = obj.ItemID;

                    Rectangle2D bounds = ItemBounds.Table[itemID];

                    if (itemID != 1 && bounds.Height < (EntryHeight * 2))
                    {
                        if (bounds.Height < EntryHeight)
                            AddItem(x - OffsetSize - 22 - ((i % 2) * 44) - (bounds.Width / 2) - bounds.X, y + (EntryHeight / 2) - (bounds.Height / 2) - bounds.Y, itemID);
                        else
                            AddItem(x - OffsetSize - 22 - ((i % 2) * 44) - (bounds.Width / 2) - bounds.X, y + EntryHeight - 1 - bounds.Height - bounds.Y, itemID);
                    }
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = m_Owner;

            switch (info.ButtonID)
            {
                case 0: // Closed
                    {
                        return;
                    }
                case 1: // Up
                    {
                        if (m_Category.Parent != null)
                        {
                            int index = Array.IndexOf(m_Category.Parent.Nodes, m_Category) / EntryCount;

                            if (index < 0)
                                index = 0;

                            from.SendGump(new CategorizedAddGump(from, m_Category.Parent, index));
                        }

                        break;
                    }
                case 2: // Previous
                    {
                        if (m_Page > 0)
                            from.SendGump(new CategorizedAddGump(from, m_Category, m_Page - 1));

                        break;
                    }
                case 3: // Next
                    {
                        if ((m_Page + 1) * EntryCount < m_Category.Nodes.Length)
                            from.SendGump(new CategorizedAddGump(from, m_Category, m_Page + 1));

                        break;
                    }
                default:
                    {
                        int index = (m_Page * EntryCount) + (info.ButtonID - 4);

                        if (index >= 0 && index < m_Category.Nodes.Length)
                            m_Category.Nodes[index].OnClick(from, m_Page);

                        break;
                    }
            }
        }
    }
}