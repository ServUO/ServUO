using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Server.Gumps
{
    public abstract class XmlAddCAGNode
    {
        public abstract string Caption { get; }
        public abstract void OnClick(Mobile from, int page, int index, Gump gump);
    }

    public class XmlAddCAGObject : XmlAddCAGNode
    {
        private readonly Type m_Type;
        private readonly XmlAddCAGCategory m_Parent;

        public int ItemID { get; }

        public override string Caption => (m_Type == null ? "bad type" : m_Type.Name);

        public override void OnClick(Mobile from, int page, int index, Gump gump)
        {
            if (m_Type == null)
            {
                from.SendMessage("That is an invalid type name.");
            }
            else
            {
                if (gump is XmlAddGump xmladdgump)
                {
                    //Commands.Handle( from, String.Format( "{0}Add {1}", Commands.CommandPrefix, m_Type.Name ) );
                    if (xmladdgump.defs?.NameList != null && index >= 0 && index < xmladdgump.defs.NameList.Length)
                    {
                        xmladdgump.defs.NameList[index] = m_Type.Name;
                        XmlAddGump.Refresh(from, true);
                    }
                    from.SendGump(new XmlCategorizedAddGump(from, m_Parent, page, index, xmladdgump));
                }
                else if (gump is XmlSpawnerGump spawnerGump)
                {
                    XmlSpawner m_Spawner = spawnerGump.m_Spawner;

                    if (m_Spawner != null)
                    {
                        XmlSpawnerGump xg = m_Spawner.SpawnerGump;

                        if (xg != null)
                        {
                            xg.Rentry = new XmlSpawnerGump.ReplacementEntry
                            {
                                Typename = m_Type.Name,
                                Index = index,
                                Color = 0x1436
                            };

                            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(XmlSpawnerGump.Refresh_Callback), new object[] { from });
                        }
                    }
                }
            }
        }

        public XmlAddCAGObject(XmlAddCAGCategory parent, XmlReader xml)
        {
            m_Parent = parent;

            if (xml.MoveToAttribute("type"))
                m_Type = ScriptCompiler.FindTypeByFullName(xml.Value, false);

            if (xml.MoveToAttribute("gfx"))
                ItemID = XmlConvert.ToInt32(xml.Value);

            if (xml.MoveToAttribute("hue"))
                XmlConvert.ToInt32(xml.Value);
        }
    }

    public class XmlAddCAGCategory : XmlAddCAGNode
    {
        private readonly string m_Title;

        public XmlAddCAGNode[] Nodes { get; }

        public XmlAddCAGCategory Parent { get; }

        public override string Caption => m_Title;

        public override void OnClick(Mobile from, int page, int index, Gump gump)
        {
            from.SendGump(new XmlCategorizedAddGump(from, this, 0, index, gump));
        }

        private XmlAddCAGCategory()
        {
            m_Title = "no data";
            Nodes = new XmlAddCAGNode[0];
        }

        public XmlAddCAGCategory(XmlAddCAGCategory parent, XmlReader xml)
        {
            Parent = parent;

            if (xml.MoveToAttribute("title"))
            {
                m_Title = xml.Value == "Add Menu" ? "XmlAdd Menu" : xml.Value;
            }
            else
                m_Title = "empty";

            if (m_Title == "Docked")
                m_Title = "Docked 2";

            if (xml.IsEmptyElement)
            {
                Nodes = new XmlAddCAGNode[0];
            }
            else
            {
                ArrayList nodes = new ArrayList();

                try
                {
                    while (xml.Read() && xml.NodeType != XmlNodeType.EndElement)
                    {

                        if (xml.NodeType == XmlNodeType.Element && xml.Name == "object")
                            nodes.Add(new XmlAddCAGObject(this, xml));
                        else if (xml.NodeType == XmlNodeType.Element && xml.Name == "category")
                        {
                            if (!xml.IsEmptyElement)
                            {
                                nodes.Add(new XmlAddCAGCategory(this, xml));
                            }
                        }
                        else
                            xml.Skip();

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("XmlCategorizedAddGump: Corrupted Data/objects.xml file detected. Not all XmlCAG objects loaded. {0}", ex);
                }

                Nodes = (XmlAddCAGNode[])nodes.ToArray(typeof(XmlAddCAGNode));
            }
        }

        private static XmlAddCAGCategory m_Root;
        public static XmlAddCAGCategory Root => m_Root ?? (m_Root = Load("Data/objects.xml"));

        public static XmlAddCAGCategory Load(string path)
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
                        XmlAddCAGCategory cat = new XmlAddCAGCategory(null, xml);

                        xml.Close();

                        return cat;
                    }
                }
            }

            return new XmlAddCAGCategory();
        }
    }

    public class XmlCategorizedAddGump : Gump
    {
        public static bool OldStyle = PropsConfig.OldStyle;

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
        public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY /*+ (((EntryHeight - 20) / 2) / 2)*/;
        public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
        public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

        public static readonly int PrevWidth = PropsConfig.PrevWidth;
        public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY /*+ (((EntryHeight - 20) / 2) / 2)*/;
        public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
        public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

        public static readonly int NextWidth = PropsConfig.NextWidth;
        public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY /*+ (((EntryHeight - 20) / 2) / 2)*/;
        public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
        public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

        public static readonly int OffsetSize = PropsConfig.OffsetSize;

        public static readonly int EntryHeight = 24;
        public static readonly int BorderSize = PropsConfig.BorderSize;

        private static readonly bool PrevLabel = false, NextLabel = false;

        private static readonly int PrevLabelOffsetX = PrevWidth + 1;
        private const int PrevLabelOffsetY = 0;

        private const int NextLabelOffsetX = -29;
        private const int NextLabelOffsetY = 0;

        private const int EntryWidth = 180;
        private const int EntryCount = 15;

        private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;

        private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
        private readonly Mobile m_Owner;
        private readonly XmlAddCAGCategory m_Category;
        private int m_Page;

        private readonly int m_Index;
        private readonly Gump m_Gump;

        public XmlCategorizedAddGump(Mobile owner, int index, Gump gump) : this(owner, XmlAddCAGCategory.Root, 0, index, gump)
        {
        }

        public XmlCategorizedAddGump(Mobile owner, XmlAddCAGCategory category, int page, int index, Gump gump) : base(GumpOffsetX, GumpOffsetY)
        {
            if (category == null)
            {
                category = XmlAddCAGCategory.Root;
                page = 0;
            }

            owner.CloseGump(typeof(WhoGump));

            m_Owner = owner;
            m_Category = category;

            m_Index = index;
            m_Gump = gump;

            if (gump is XmlAddGump xmladdgump)
            {
                if (xmladdgump.defs != null)
                {
                    xmladdgump.defs.CurrentCategory = category;
                    xmladdgump.defs.CurrentCategoryPage = page;
                }
            }

            Initialize(page);
        }

        public void Initialize(int page)
        {
            m_Page = page;

            XmlAddCAGNode[] nodes = m_Category.Nodes;

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

                XmlAddCAGNode node = nodes[index];

                AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                AddLabelCropped(x + TextOffsetX, y + ((EntryHeight - 20) / 2), EntryWidth - TextOffsetX, EntryHeight, TextHue, node.Caption);

                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 4, GumpButtonType.Reply, 0);

                if (node is XmlAddCAGObject obj)
                {
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

                            from.SendGump(new XmlCategorizedAddGump(from, m_Category.Parent, index, m_Index, m_Gump));
                        }

                        break;
                    }
                case 2: // Previous
                    {
                        if (m_Page > 0)
                            from.SendGump(new XmlCategorizedAddGump(from, m_Category, m_Page - 1, m_Index, m_Gump));

                        break;
                    }
                case 3: // Next
                    {
                        if ((m_Page + 1) * EntryCount < m_Category.Nodes.Length)
                            from.SendGump(new XmlCategorizedAddGump(from, m_Category, m_Page + 1, m_Index, m_Gump));

                        break;
                    }
                default:
                    {
                        int index = (m_Page * EntryCount) + (info.ButtonID - 4);

                        if (index >= 0 && index < m_Category.Nodes.Length)
                        {
                            m_Category.Nodes[index].OnClick(from, m_Page, m_Index, m_Gump);
                        }

                        break;
                    }
            }
        }
    }
}
