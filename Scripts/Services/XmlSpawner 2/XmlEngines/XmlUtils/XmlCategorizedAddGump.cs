using System;
using System.Collections;
using System.IO;
using System.Xml;
using Server.Mobiles;
using Server.Network;

/*
** Modified from RunUO 1.0.0 CategorizedAddGump.cs
** by ArteGordon
** 2/5/05
*/
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
        private readonly int m_ItemID;
        private readonly int m_Hue;
        private readonly XmlAddCAGCategory m_Parent;
        public XmlAddCAGObject(XmlAddCAGCategory parent, XmlTextReader xml)
        {
            this.m_Parent = parent;

            if (xml.MoveToAttribute("type"))
                this.m_Type = ScriptCompiler.FindTypeByFullName(xml.Value, false);

            if (xml.MoveToAttribute("gfx"))
                this.m_ItemID = XmlConvert.ToInt32(xml.Value);

            if (xml.MoveToAttribute("hue"))
                this.m_Hue = XmlConvert.ToInt32(xml.Value);
        }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
        }
        public XmlAddCAGCategory Parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public override string Caption
        {
            get
            {
                return (this.m_Type == null ? "bad type" : this.m_Type.Name);
            }
        }
        public override void OnClick(Mobile from, int page, int index, Gump gump)
        {
            if (this.m_Type == null)
            {
                from.SendMessage("That is an invalid type name.");
            }
            else
            {
                if (gump is XmlAddGump)
                {
                    XmlAddGump xmladdgump = (XmlAddGump)gump;

                    //Commands.Handle( from, String.Format( "{0}Add {1}", Commands.CommandPrefix, m_Type.Name ) );
                    if (xmladdgump != null && xmladdgump.defs != null && xmladdgump.defs.NameList != null &&
                        index >= 0 && index < xmladdgump.defs.NameList.Length)
                    {
                        xmladdgump.defs.NameList[index] = this.m_Type.Name;
                        XmlAddGump.Refresh(from, true);
                    }
                    from.SendGump(new XmlCategorizedAddGump(from, this.m_Parent, page, index, xmladdgump));
                }
                else if (gump is XmlSpawnerGump)
                {
                    XmlSpawner m_Spawner = ((XmlSpawnerGump)gump).m_Spawner;

                    if (m_Spawner != null)
                    { 
                        XmlSpawnerGump xg = m_Spawner.SpawnerGump; 

                        if (xg != null)
                        {
                            xg.Rentry = new XmlSpawnerGump.ReplacementEntry();
                            xg.Rentry.Typename = this.m_Type.Name;
                            xg.Rentry.Index = index;
                            xg.Rentry.Color = 0x1436;

                            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(XmlSpawnerGump.Refresh_Callback), new object[] { from });
                            //from.CloseGump(typeof(XmlSpawnerGump));
                            //from.SendGump( new XmlSpawnerGump(xg.m_Spawner, xg.X, xg.Y, xg.m_ShowGump, xg.xoffset, xg.page, xg.Rentry) );
                        }
                    }
                }
            }
        }
    }

    public class XmlAddCAGCategory : XmlAddCAGNode
    {
        private static XmlAddCAGCategory m_Root;
        private readonly string m_Title;
        private readonly XmlAddCAGNode[] m_Nodes;
        private readonly XmlAddCAGCategory m_Parent;
        public XmlAddCAGCategory(XmlAddCAGCategory parent, XmlTextReader xml)
        {
            this.m_Parent = parent;

            if (xml.MoveToAttribute("title"))
            {
                if (xml.Value == "Add Menu")
                    this.m_Title = "XmlAdd Menu";
                else
                    this.m_Title = xml.Value;
            }
            else
                this.m_Title = "empty";

            if (this.m_Title == "Docked")
                this.m_Title = "Docked 2";

            if (xml.IsEmptyElement)
            {
                this.m_Nodes = new XmlAddCAGNode[0];
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

                this.m_Nodes = (XmlAddCAGNode[])nodes.ToArray(typeof(XmlAddCAGNode));
            }
        }

        private XmlAddCAGCategory()
        {
            this.m_Title = "no data";
            this.m_Nodes = new XmlAddCAGNode[0];
        }

        public static XmlAddCAGCategory Root
        {
            get
            {
                if (m_Root == null)
                    m_Root = Load("Data/objects.xml");

                return m_Root;
            }
        }
        public string Title
        {
            get
            {
                return this.m_Title;
            }
        }
        public XmlAddCAGNode[] Nodes
        {
            get
            {
                return this.m_Nodes;
            }
        }
        public XmlAddCAGCategory Parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public override string Caption
        {
            get
            {
                return this.m_Title;
            }
        }
        public static XmlAddCAGCategory Load(string path)
        {
            if (File.Exists(path))
            {
                XmlTextReader xml = new XmlTextReader(path);

                xml.WhitespaceHandling = WhitespaceHandling.None;

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

        public override void OnClick(Mobile from, int page, int index, Gump gump)
        {
            from.SendGump(new XmlCategorizedAddGump(from, this, 0, index, gump));
        }
    }

    public class XmlCategorizedAddGump : Gump
    {
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
        public static readonly int EntryHeight = 24;//PropsConfig.EntryHeight;
        public static readonly int BorderSize = PropsConfig.BorderSize;
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
        private readonly XmlAddCAGCategory m_Category;
        private readonly int m_Index = -1;
        private readonly Gump m_Gump;
        private readonly XmlSpawner m_Spawner;
        private int m_Page;
        public XmlCategorizedAddGump(Mobile owner, int index, Gump gump)
            : this(owner, XmlAddCAGCategory.Root, 0, index, gump)
        {
        }

        public XmlCategorizedAddGump(Mobile owner, XmlAddCAGCategory category, int page, int index, Gump gump)
            : base(GumpOffsetX, GumpOffsetY)
        {
            if (category == null)
            {
                category = XmlAddCAGCategory.Root;
                page = 0;
            }

            owner.CloseGump(typeof(WhoGump));

            this.m_Owner = owner;
            this.m_Category = category;
			
            this.m_Index = index;
            this.m_Gump = gump;
            if (gump is XmlAddGump)
            {
                XmlAddGump xmladdgump = (XmlAddGump)gump;

                if (xmladdgump != null && xmladdgump.defs != null)
                {
                    xmladdgump.defs.CurrentCategory = category;
                    xmladdgump.defs.CurrentCategoryPage = page;
                }
            }
            else if (gump is XmlSpawnerGump)
            {
                this.m_Spawner = ((XmlSpawnerGump)gump).m_Spawner;
            }

            this.Initialize(page);
        }

        public void Initialize(int page)
        {
            this.m_Page = page;

            XmlAddCAGNode[] nodes = this.m_Category.Nodes;

            int count = nodes.Length - (page * EntryCount);

            if (count < 0)
                count = 0;
            else if (count > EntryCount)
                count = EntryCount;

            int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

            this.AddPage(0);

            this.AddBackground(0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID);
            this.AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

            int x = BorderSize + OffsetSize;
            int y = BorderSize + OffsetSize;

            if (OldStyle)
                this.AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            if (this.m_Category.Parent != null)
            {
                this.AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

                if (PrevLabel)
                    this.AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
            }

            x += PrevWidth + OffsetSize;

            int emptyWidth = TotalWidth - (PrevWidth * 2) - NextWidth - (OffsetSize * 5) - (OldStyle ? SetWidth + OffsetSize : 0);

            if (!OldStyle)
                this.AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

            this.AddHtml(x + TextOffsetX, y + ((EntryHeight - 20) / 2), emptyWidth - TextOffsetX, EntryHeight, String.Format("<center>{0}</center>", this.m_Category.Caption), false, false);

            x += emptyWidth + OffsetSize;

            if (OldStyle)
                this.AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
            else
                this.AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

            if (page > 0)
            {
                this.AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 2, GumpButtonType.Reply, 0);

                if (PrevLabel)
                    this.AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
            }

            x += PrevWidth + OffsetSize;

            if (!OldStyle)
                this.AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

            if ((page + 1) * EntryCount < nodes.Length)
            {
                this.AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 3, GumpButtonType.Reply, 1);

                if (NextLabel)
                    this.AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
            }

            for (int i = 0, index = page * EntryCount; i < EntryCount && index < nodes.Length; ++i, ++index)
            {
                x = BorderSize + OffsetSize;
                y += EntryHeight + OffsetSize;

                XmlAddCAGNode node = nodes[index];

                this.AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
                this.AddLabelCropped(x + TextOffsetX, y + ((EntryHeight - 20) / 2), EntryWidth - TextOffsetX, EntryHeight, TextHue, node.Caption);

                x += EntryWidth + OffsetSize;

                if (SetGumpID != 0)
                    this.AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

                this.AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 4, GumpButtonType.Reply, 0);

                if (node is XmlAddCAGObject)
                {
                    XmlAddCAGObject obj = (XmlAddCAGObject)node;
                    int itemID = obj.ItemID;

                    Rectangle2D bounds = ItemBounds.Table[itemID];

                    if (itemID != 1 && bounds.Height < (EntryHeight * 2))
                    {
                        if (bounds.Height < EntryHeight)
                            this.AddItem(x - OffsetSize - 22 - ((i % 2) * 44) - (bounds.Width / 2) - bounds.X, y + (EntryHeight / 2) - (bounds.Height / 2) - bounds.Y, itemID);
                        else
                            this.AddItem(x - OffsetSize - 22 - ((i % 2) * 44) - (bounds.Width / 2) - bounds.X, y + EntryHeight - 1 - bounds.Height - bounds.Y, itemID);
                    }
                }
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = this.m_Owner;

            switch ( info.ButtonID )
            {
                case 0: // Closed
                    {
                        return;
                    }
                case 1: // Up
                    {
                        if (this.m_Category.Parent != null)
                        {
                            int index = Array.IndexOf(this.m_Category.Parent.Nodes, this.m_Category) / EntryCount;

                            if (index < 0)
                                index = 0;

                            from.SendGump(new XmlCategorizedAddGump(from, this.m_Category.Parent, index, this.m_Index, this.m_Gump));
                        }

                        break;
                    }
                case 2: // Previous
                    {
                        if (this.m_Page > 0)
                            from.SendGump(new XmlCategorizedAddGump(from, this.m_Category, this.m_Page - 1, this.m_Index, this.m_Gump));

                        break;
                    }
                case 3: // Next
                    {
                        if ((this.m_Page + 1) * EntryCount < this.m_Category.Nodes.Length)
                            from.SendGump(new XmlCategorizedAddGump(from, this.m_Category, this.m_Page + 1, this.m_Index, this.m_Gump));

                        break;
                    }
                default:
                    {
                        int index = (this.m_Page * EntryCount) + (info.ButtonID - 4);

                        if (index >= 0 && index < this.m_Category.Nodes.Length)
                        {
                            this.m_Category.Nodes[index].OnClick(from, this.m_Page, this.m_Index, this.m_Gump);
                        }

                        break;
                    }
            }
        }
    }
}