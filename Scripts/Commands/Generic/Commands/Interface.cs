using Server.Gumps;
using Server.Network;
using Server.Targets;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Server.Commands.Generic
{
    public class InterfaceCommand : BaseCommand
    {
        public InterfaceCommand()
        {
            AccessLevel = AccessLevel.GameMaster;
            Supports = CommandSupport.Complex | CommandSupport.Simple;
            Commands = new string[] { "Interface" };
            ObjectTypes = ObjectTypes.Both;
            Usage = "Interface [view <properties ...>]";
            Description = "Opens an interface to interact with matched objects. Generally used with condition arguments.";
            ListOptimized = true;
        }

        public override void ExecuteList(CommandEventArgs e, ArrayList list)
        {
            if (list.Count > 0)
            {
                List<string> columns = new List<string>();

                columns.Add("Object");

                if (e.Length > 0)
                {
                    int offset = 0;

                    if (Insensitive.Equals(e.GetString(0), "view"))
                        ++offset;

                    while (offset < e.Length)
                        columns.Add(e.GetString(offset++));
                }

                e.Mobile.SendGump(new InterfaceGump(e.Mobile, columns.ToArray(), list, 0, null));
            }
            else
            {
                AddResponse("No matching objects found.");
            }
        }
    }

    public class InterfaceGump : BaseGridGump
    {
        private const int EntriesPerPage = 15;
        private readonly Mobile m_From;
        private readonly string[] m_Columns;
        private readonly ArrayList m_List;
        private readonly int m_Page;
        private readonly object m_Select;
        public InterfaceGump(Mobile from, string[] columns, ArrayList list, int page, object select)
            : base(30, 30)
        {
            m_From = from;

            m_Columns = columns;

            m_List = list;
            m_Page = page;

            m_Select = select;

            Render();
        }

        public void Render()
        {
            AddNewPage();

            if (m_Page > 0)
                AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            else
                AddEntryHeader(20);

            AddEntryHtml(40 + (m_Columns.Length * 130) - 20 + ((m_Columns.Length - 2) * OffsetSize), Center(string.Format("Page {0} of {1}", m_Page + 1, (m_List.Count + EntriesPerPage - 1) / EntriesPerPage)));

            if ((m_Page + 1) * EntriesPerPage < m_List.Count)
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);
            else
                AddEntryHeader(20);

            if (m_Columns.Length > 1)
            {
                AddNewLine();

                for (int i = 0; i < m_Columns.Length; ++i)
                {
                    if (i > 0 && m_List.Count > 0)
                    {
                        object obj = m_List[0];

                        if (obj != null)
                        {
                            string failReason = null;
                            PropertyInfo[] chain = Properties.GetPropertyInfoChain(m_From, obj.GetType(), m_Columns[i], PropertyAccess.Read, ref failReason);

                            if (chain != null && chain.Length > 0)
                            {
                                m_Columns[i] = "";

                                for (int j = 0; j < chain.Length; ++j)
                                {
                                    if (j > 0)
                                        m_Columns[i] += '.';

                                    m_Columns[i] += chain[j].Name;
                                }
                            }
                        }
                    }

                    AddEntryHtml(130 + (i == 0 ? 40 : 0), m_Columns[i]);
                }

                AddEntryHeader(20);
            }

            for (int i = m_Page * EntriesPerPage, line = 0; line < EntriesPerPage && i < m_List.Count; ++i, ++line)
            {
                AddNewLine();

                object obj = m_List[i];
                bool isDeleted = false;

                if (obj is Item)
                {
                    Item item = (Item)obj;

                    if (!(isDeleted = item.Deleted))
                        AddEntryHtml(40 + 130, item.GetType().Name);
                }
                else if (obj is Mobile)
                {
                    Mobile mob = (Mobile)obj;

                    if (!(isDeleted = mob.Deleted))
                        AddEntryHtml(40 + 130, mob.Name);
                }

                if (isDeleted)
                {
                    AddEntryHtml(40 + 130, "(deleted)");

                    for (int j = 1; j < m_Columns.Length; ++j)
                        AddEntryHtml(130, "---");

                    AddEntryHeader(20);
                }
                else
                {
                    for (int j = 1; j < m_Columns.Length; ++j)
                    {
                        object src = obj;

                        string value;
                        string failReason = "";

                        PropertyInfo[] chain = Properties.GetPropertyInfoChain(m_From, src.GetType(), m_Columns[j], PropertyAccess.Read, ref failReason);

                        if (chain == null || chain.Length == 0)
                        {
                            value = "---";
                        }
                        else
                        {
                            PropertyInfo p = Properties.GetPropertyInfo(ref src, chain, ref failReason);

                            if (p == null)
                                value = "---";
                            else
                                value = PropertiesGump.ValueToString(src, p);
                        }

                        AddEntryHtml(130, value);
                    }

                    bool isSelected = (m_Select != null && obj == m_Select);

                    AddEntryButton(20, (isSelected ? 9762 : ArrowRightID1), (isSelected ? 9763 : ArrowRightID2), 3 + i, ArrowRightWidth, ArrowRightHeight);
                }
            }

            FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 1:
                    {
                        if (m_Page > 0)
                            m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page - 1, m_Select));

                        break;
                    }
                case 2:
                    {
                        if ((m_Page + 1) * EntriesPerPage < m_List.Count)
                            m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page + 1, m_Select));

                        break;
                    }
                default:
                    {
                        int v = info.ButtonID - 3;

                        if (v >= 0 && v < m_List.Count)
                        {
                            object obj = m_List[v];

                            if (!BaseCommand.IsAccessible(m_From, obj))
                            {
                                m_From.SendMessage("That is not accessible.");
                                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Select));
                                break;
                            }

                            if (obj is Item && !((Item)obj).Deleted)
                                m_From.SendGump(new InterfaceItemGump(m_From, m_Columns, m_List, m_Page, (Item)obj));
                            else if (obj is Mobile && !((Mobile)obj).Deleted)
                                m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, (Mobile)obj));
                            else
                                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Select));
                        }

                        break;
                    }
            }
        }
    }

    public class InterfaceItemGump : BaseGridGump
    {
        private readonly Mobile m_From;
        private readonly string[] m_Columns;
        private readonly ArrayList m_List;
        private readonly int m_Page;
        private readonly Item m_Item;
        public InterfaceItemGump(Mobile from, string[] columns, ArrayList list, int page, Item item)
            : base(30, 30)
        {
            m_From = from;

            m_Columns = columns;

            m_List = list;
            m_Page = page;

            m_Item = item;

            Render();
        }

        public void Render()
        {
            AddNewPage();

            AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            AddEntryHtml(160, m_Item.GetType().Name);
            AddEntryHeader(20);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Properties");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Delete");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 3, ArrowRightWidth, ArrowRightHeight);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Go there");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 4, ArrowRightWidth, ArrowRightHeight);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Move to target");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 5, ArrowRightWidth, ArrowRightHeight);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Bring to pack");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 6, ArrowRightWidth, ArrowRightHeight);

            FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Item.Deleted)
            {
                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Item));
                return;
            }
            else if (!BaseCommand.IsAccessible(m_From, m_Item))
            {
                m_From.SendMessage("That is no longer accessible.");
                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Item));
                return;
            }

            switch (info.ButtonID)
            {
                case 0:
                case 1:
                    {
                        m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Item));
                        break;
                    }
                case 2: // Properties
                    {
                        m_From.SendGump(new InterfaceItemGump(m_From, m_Columns, m_List, m_Page, m_Item));
                        m_From.SendGump(new PropertiesGump(m_From, m_Item));
                        break;
                    }
                case 3: // Delete
                    {
                        CommandLogging.WriteLine(m_From, "{0} {1} deleting {2}", m_From.AccessLevel, CommandLogging.Format(m_From), CommandLogging.Format(m_Item));
                        m_Item.Delete();
                        m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Item));
                        break;
                    }
                case 4: // Go there
                    {
                        m_From.SendGump(new InterfaceItemGump(m_From, m_Columns, m_List, m_Page, m_Item));
                        InvokeCommand(string.Format("Go {0}", m_Item.Serial.Value));
                        break;
                    }
                case 5: // Move to target
                    {
                        m_From.SendGump(new InterfaceItemGump(m_From, m_Columns, m_List, m_Page, m_Item));
                        m_From.Target = new MoveTarget(m_Item);
                        break;
                    }
                case 6: // Bring to pack
                    {
                        Mobile owner = m_Item.RootParent as Mobile;

                        if (owner != null && (owner.Map != null && owner.Map != Map.Internal) && !BaseCommand.IsAccessible(m_From, owner) /* !m_From.CanSee( owner )*/)
                        {
                            m_From.SendMessage("You can not get what you can not see.");
                        }
                        else if (owner != null && (owner.Map == null || owner.Map == Map.Internal) && owner.Hidden && owner.AccessLevel >= m_From.AccessLevel)
                        {
                            m_From.SendMessage("You can not get what you can not see.");
                        }
                        else
                        {
                            m_From.SendGump(new InterfaceItemGump(m_From, m_Columns, m_List, m_Page, m_Item));
                            m_From.AddToBackpack(m_Item);
                        }

                        break;
                    }
            }
        }

        private void InvokeCommand(string ip)
        {
            CommandSystem.Handle(m_From, string.Format("{0}{1}", CommandSystem.Prefix, ip));
        }
    }

    public class InterfaceMobileGump : BaseGridGump
    {
        private readonly Mobile m_From;
        private readonly string[] m_Columns;
        private readonly ArrayList m_List;
        private readonly int m_Page;
        private readonly Mobile m_Mobile;
        public InterfaceMobileGump(Mobile from, string[] columns, ArrayList list, int page, Mobile mob)
            : base(30, 30)
        {
            m_From = from;

            m_Columns = columns;

            m_List = list;
            m_Page = page;

            m_Mobile = mob;

            Render();
        }

        public void Render()
        {
            AddNewPage();

            AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            AddEntryHtml(160, m_Mobile.Name);
            AddEntryHeader(20);

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Properties");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);

            if (!m_Mobile.Player)
            {
                AddNewLine();
                AddEntryHtml(20 + OffsetSize + 160, "Delete");
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 3, ArrowRightWidth, ArrowRightHeight);
            }

            if (m_Mobile != m_From)
            {
                AddNewLine();
                AddEntryHtml(20 + OffsetSize + 160, "Go to there");
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 4, ArrowRightWidth, ArrowRightHeight);

                AddNewLine();
                AddEntryHtml(20 + OffsetSize + 160, "Bring them here");
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 5, ArrowRightWidth, ArrowRightHeight);
            }

            AddNewLine();
            AddEntryHtml(20 + OffsetSize + 160, "Move to target");
            AddEntryButton(20, ArrowRightID1, ArrowRightID2, 6, ArrowRightWidth, ArrowRightHeight);

            if (m_From == m_Mobile || m_From.AccessLevel > m_Mobile.AccessLevel)
            {
                AddNewLine();
                if (m_Mobile.Alive)
                {
                    AddEntryHtml(20 + OffsetSize + 160, "Kill");
                    AddEntryButton(20, ArrowRightID1, ArrowRightID2, 7, ArrowRightWidth, ArrowRightHeight);
                }
                else
                {
                    AddEntryHtml(20 + OffsetSize + 160, "Resurrect");
                    AddEntryButton(20, ArrowRightID1, ArrowRightID2, 8, ArrowRightWidth, ArrowRightHeight);
                }
            }

            if (m_Mobile.NetState != null)
            {
                AddNewLine();
                AddEntryHtml(20 + OffsetSize + 160, "Client");
                AddEntryButton(20, ArrowRightID1, ArrowRightID2, 9, ArrowRightWidth, ArrowRightHeight);
            }

            FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Mobile.Deleted)
            {
                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                return;
            }
            else if (!BaseCommand.IsAccessible(m_From, m_Mobile))
            {
                m_From.SendMessage("That is no longer accessible.");
                m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                return;
            }

            switch (info.ButtonID)
            {
                case 0:
                case 1:
                    {
                        m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                        break;
                    }
                case 2: // Properties
                    {
                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                        m_From.SendGump(new PropertiesGump(m_From, m_Mobile));
                        break;
                    }
                case 3: // Delete
                    {
                        if (!m_Mobile.Player)
                        {
                            CommandLogging.WriteLine(m_From, "{0} {1} deleting {2}", m_From.AccessLevel, CommandLogging.Format(m_From), CommandLogging.Format(m_Mobile));
                            m_Mobile.Delete();
                            m_From.SendGump(new InterfaceGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                        }

                        break;
                    }
                case 4: // Go there
                    {
                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                        InvokeCommand(string.Format("Go {0}", m_Mobile.Serial.Value));
                        break;
                    }
                case 5: // Bring them here
                    {
                        if (m_From.Map == null || m_From.Map == Map.Internal)
                        {
                            m_From.SendMessage("You cannot bring that person here.");
                        }
                        else
                        {
                            m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                            m_Mobile.MoveToWorld(m_From.Location, m_From.Map);
                        }

                        break;
                    }
                case 6: // Move to target
                    {
                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));
                        m_From.Target = new MoveTarget(m_Mobile);
                        break;
                    }
                case 7: // Kill
                    {
                        if (m_From == m_Mobile || m_From.AccessLevel > m_Mobile.AccessLevel)
                            m_Mobile.Kill();

                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));

                        break;
                    }
                case 8: // Res
                    {
                        if (m_From == m_Mobile || m_From.AccessLevel > m_Mobile.AccessLevel)
                        {
                            m_Mobile.PlaySound(0x214);
                            m_Mobile.FixedEffect(0x376A, 10, 16);

                            m_Mobile.Resurrect();
                        }

                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));

                        break;
                    }
                case 9: // Client
                    {
                        m_From.SendGump(new InterfaceMobileGump(m_From, m_Columns, m_List, m_Page, m_Mobile));

                        if (m_Mobile.NetState != null)
                            m_From.SendGump(new ClientGump(m_From, m_Mobile.NetState));

                        break;
                    }
            }
        }

        private void InvokeCommand(string ip)
        {
            CommandSystem.Handle(m_From, string.Format("{0}{1}", CommandSystem.Prefix, ip));
        }
    }
}