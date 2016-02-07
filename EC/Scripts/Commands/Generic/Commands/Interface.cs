using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server.Gumps;
using Server.Network;
using Server.Targets;

namespace Server.Commands.Generic
{
    public class InterfaceCommand : BaseCommand
    {
        public InterfaceCommand()
        {
            this.AccessLevel = AccessLevel.GameMaster;
            this.Supports = CommandSupport.Complex | CommandSupport.Simple;
            this.Commands = new string[] { "Interface" };
            this.ObjectTypes = ObjectTypes.Both;
            this.Usage = "Interface [view <properties ...>]";
            this.Description = "Opens an interface to interact with matched objects. Generally used with condition arguments.";
            this.ListOptimized = true;
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
                this.AddResponse("No matching objects found.");
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
            this.m_From = from;

            this.m_Columns = columns;

            this.m_List = list;
            this.m_Page = page;

            this.m_Select = select;

            this.Render();
        }

        public void Render()
        {
            this.AddNewPage();

            if (this.m_Page > 0)
                this.AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            else
                this.AddEntryHeader(20);

            this.AddEntryHtml(40 + (this.m_Columns.Length * 130) - 20 + ((this.m_Columns.Length - 2) * this.OffsetSize), this.Center(String.Format("Page {0} of {1}", this.m_Page + 1, (this.m_List.Count + EntriesPerPage - 1) / EntriesPerPage)));

            if ((this.m_Page + 1) * EntriesPerPage < this.m_List.Count)
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);
            else
                this.AddEntryHeader(20);

            if (this.m_Columns.Length > 1)
            {
                this.AddNewLine();

                for (int i = 0; i < this.m_Columns.Length; ++i)
                {
                    if (i > 0 && this.m_List.Count > 0)
                    {
                        object obj = this.m_List[0];

                        if (obj != null)
                        {
                            string failReason = null;
                            PropertyInfo[] chain = Properties.GetPropertyInfoChain(this.m_From, obj.GetType(), this.m_Columns[i], PropertyAccess.Read, ref failReason);

                            if (chain != null && chain.Length > 0)
                            {
                                this.m_Columns[i] = "";

                                for (int j = 0; j < chain.Length; ++j)
                                {
                                    if (j > 0)
                                        this.m_Columns[i] += '.';

                                    this.m_Columns[i] += chain[j].Name;
                                }
                            }
                        }
                    }

                    this.AddEntryHtml(130 + (i == 0 ? 40 : 0), this.m_Columns[i]);
                }

                this.AddEntryHeader(20);
            }

            for (int i = this.m_Page * EntriesPerPage, line = 0; line < EntriesPerPage && i < this.m_List.Count; ++i, ++line)
            {
                this.AddNewLine();

                object obj = this.m_List[i];
                bool isDeleted = false;

                if (obj is Item)
                {
                    Item item = (Item)obj;

                    if (!(isDeleted = item.Deleted))
                        this.AddEntryHtml(40 + 130, item.GetType().Name);
                }
                else if (obj is Mobile)
                {
                    Mobile mob = (Mobile)obj;

                    if (!(isDeleted = mob.Deleted))
                        this.AddEntryHtml(40 + 130, mob.Name);
                }

                if (isDeleted)
                {
                    this.AddEntryHtml(40 + 130, "(deleted)");

                    for (int j = 1; j < this.m_Columns.Length; ++j)
                        this.AddEntryHtml(130, "---");

                    this.AddEntryHeader(20);
                }
                else
                {
                    for (int j = 1; j < this.m_Columns.Length; ++j)
                    {
                        object src = obj;

                        string value;
                        string failReason = "";

                        PropertyInfo[] chain = Properties.GetPropertyInfoChain(this.m_From, src.GetType(), this.m_Columns[j], PropertyAccess.Read, ref failReason);

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

                        this.AddEntryHtml(130, value);
                    }

                    bool isSelected = (this.m_Select != null && obj == this.m_Select);

                    this.AddEntryButton(20, (isSelected ? 9762 : ArrowRightID1), (isSelected ? 9763 : ArrowRightID2), 3 + i, ArrowRightWidth, ArrowRightHeight);
                }
            }

            this.FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 1:
                    {
                        if (this.m_Page > 0)
                            this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page - 1, this.m_Select));

                        break;
                    }
                case 2:
                    {
                        if ((this.m_Page + 1) * EntriesPerPage < this.m_List.Count)
                            this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page + 1, this.m_Select));

                        break;
                    }
                default:
                    {
                        int v = info.ButtonID - 3;

                        if (v >= 0 && v < this.m_List.Count)
                        {
                            object obj = this.m_List[v];

                            if (!BaseCommand.IsAccessible(this.m_From, obj))
                            {
                                this.m_From.SendMessage("That is not accessible.");
                                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Select));
                                break;
                            }

                            if (obj is Item && !((Item)obj).Deleted)
                                this.m_From.SendGump(new InterfaceItemGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, (Item)obj));
                            else if (obj is Mobile && !((Mobile)obj).Deleted)
                                this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, (Mobile)obj));
                            else
                                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Select));
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
            this.m_From = from;

            this.m_Columns = columns;

            this.m_List = list;
            this.m_Page = page;

            this.m_Item = item;

            this.Render();
        }

        public void Render()
        {
            this.AddNewPage();

            this.AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            this.AddEntryHtml(160, this.m_Item.GetType().Name);
            this.AddEntryHeader(20);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Properties");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Delete");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 3, ArrowRightWidth, ArrowRightHeight);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Go there");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 4, ArrowRightWidth, ArrowRightHeight);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Move to target");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 5, ArrowRightWidth, ArrowRightHeight);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Bring to pack");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 6, ArrowRightWidth, ArrowRightHeight);

            this.FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Item.Deleted)
            {
                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                return;
            }
            else if (!BaseCommand.IsAccessible(this.m_From, this.m_Item))
            {
                this.m_From.SendMessage("That is no longer accessible.");
                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                return;
            }

            switch ( info.ButtonID )
            {
                case 0:
                case 1:
                    {
                        this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                        break;
                    }
                case 2: // Properties
                    {
                        this.m_From.SendGump(new InterfaceItemGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                        this.m_From.SendGump(new PropertiesGump(this.m_From, this.m_Item));
                        break;
                    }
                case 3: // Delete
                    {
                        CommandLogging.WriteLine(this.m_From, "{0} {1} deleting {2}", this.m_From.AccessLevel, CommandLogging.Format(this.m_From), CommandLogging.Format(this.m_Item));
                        this.m_Item.Delete();
                        this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                        break;
                    }
                case 4: // Go there
                    {
                        this.m_From.SendGump(new InterfaceItemGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                        this.InvokeCommand(String.Format("Go {0}", this.m_Item.Serial.Value));
                        break;
                    }
                case 5: // Move to target
                    {
                        this.m_From.SendGump(new InterfaceItemGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                        this.m_From.Target = new MoveTarget(this.m_Item);
                        break;
                    }
                case 6: // Bring to pack
                    {
                        Mobile owner = this.m_Item.RootParent as Mobile;

                        if (owner != null && (owner.Map != null && owner.Map != Map.Internal) && !BaseCommand.IsAccessible(this.m_From, owner) /* !m_From.CanSee( owner )*/)
                        {
                            this.m_From.SendMessage("You can not get what you can not see.");
                        }
                        else if (owner != null && (owner.Map == null || owner.Map == Map.Internal) && owner.Hidden && owner.AccessLevel >= this.m_From.AccessLevel)
                        {
                            this.m_From.SendMessage("You can not get what you can not see.");
                        }
                        else
                        {
                            this.m_From.SendGump(new InterfaceItemGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Item));
                            this.m_From.AddToBackpack(this.m_Item);
                        }

                        break;
                    }
            }
        }

        private void InvokeCommand(string ip)
        {
            CommandSystem.Handle(this.m_From, String.Format("{0}{1}", CommandSystem.Prefix, ip));
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
            this.m_From = from;

            this.m_Columns = columns;

            this.m_List = list;
            this.m_Page = page;

            this.m_Mobile = mob;

            this.Render();
        }

        public void Render()
        {
            this.AddNewPage();

            this.AddEntryButton(20, ArrowLeftID1, ArrowLeftID2, 1, ArrowLeftWidth, ArrowLeftHeight);
            this.AddEntryHtml(160, this.m_Mobile.Name);
            this.AddEntryHeader(20);

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Properties");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 2, ArrowRightWidth, ArrowRightHeight);

            if (!this.m_Mobile.Player)
            {
                this.AddNewLine();
                this.AddEntryHtml(20 + this.OffsetSize + 160, "Delete");
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 3, ArrowRightWidth, ArrowRightHeight);
            }

            if (this.m_Mobile != this.m_From)
            {
                this.AddNewLine();
                this.AddEntryHtml(20 + this.OffsetSize + 160, "Go to there");
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 4, ArrowRightWidth, ArrowRightHeight);

                this.AddNewLine();
                this.AddEntryHtml(20 + this.OffsetSize + 160, "Bring them here");
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 5, ArrowRightWidth, ArrowRightHeight);
            }

            this.AddNewLine();
            this.AddEntryHtml(20 + this.OffsetSize + 160, "Move to target");
            this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 6, ArrowRightWidth, ArrowRightHeight);

            if (this.m_From == this.m_Mobile || this.m_From.AccessLevel > this.m_Mobile.AccessLevel)
            {
                this.AddNewLine();
                if (this.m_Mobile.Alive)
                {
                    this.AddEntryHtml(20 + this.OffsetSize + 160, "Kill");
                    this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 7, ArrowRightWidth, ArrowRightHeight);
                }
                else
                {
                    this.AddEntryHtml(20 + this.OffsetSize + 160, "Resurrect");
                    this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 8, ArrowRightWidth, ArrowRightHeight);
                }
            }

            if (this.m_Mobile.NetState != null)
            {
                this.AddNewLine();
                this.AddEntryHtml(20 + this.OffsetSize + 160, "Client");
                this.AddEntryButton(20, ArrowRightID1, ArrowRightID2, 9, ArrowRightWidth, ArrowRightHeight);
            }

            this.FinishPage();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (this.m_Mobile.Deleted)
            {
                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                return;
            }
            else if (!BaseCommand.IsAccessible(this.m_From, this.m_Mobile))
            {
                this.m_From.SendMessage("That is no longer accessible.");
                this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                return;
            }

            switch ( info.ButtonID )
            {
                case 0:
                case 1:
                    {
                        this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                        break;
                    }
                case 2: // Properties
                    {
                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                        this.m_From.SendGump(new PropertiesGump(this.m_From, this.m_Mobile));
                        break;
                    }
                case 3: // Delete
                    {
                        if (!this.m_Mobile.Player)
                        {
                            CommandLogging.WriteLine(this.m_From, "{0} {1} deleting {2}", this.m_From.AccessLevel, CommandLogging.Format(this.m_From), CommandLogging.Format(this.m_Mobile));
                            this.m_Mobile.Delete();
                            this.m_From.SendGump(new InterfaceGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                        }

                        break;
                    }
                case 4: // Go there
                    {
                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                        this.InvokeCommand(String.Format("Go {0}", this.m_Mobile.Serial.Value));
                        break;
                    }
                case 5: // Bring them here
                    {
                        if (this.m_From.Map == null || this.m_From.Map == Map.Internal)
                        {
                            this.m_From.SendMessage("You cannot bring that person here.");
                        }
                        else
                        {
                            this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                            this.m_Mobile.MoveToWorld(this.m_From.Location, this.m_From.Map);
                        }

                        break;
                    }
                case 6: // Move to target
                    {
                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));
                        this.m_From.Target = new MoveTarget(this.m_Mobile);
                        break;
                    }
                case 7: // Kill
                    {
                        if (this.m_From == this.m_Mobile || this.m_From.AccessLevel > this.m_Mobile.AccessLevel)
                            this.m_Mobile.Kill();

                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));

                        break;
                    }
                case 8: // Res
                    {
                        if (this.m_From == this.m_Mobile || this.m_From.AccessLevel > this.m_Mobile.AccessLevel)
                        {
                            this.m_Mobile.PlaySound(0x214);
                            this.m_Mobile.FixedEffect(0x376A, 10, 16);

                            this.m_Mobile.Resurrect();
                        }

                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));

                        break;
                    }
                case 9: // Client
                    {
                        this.m_From.SendGump(new InterfaceMobileGump(this.m_From, this.m_Columns, this.m_List, this.m_Page, this.m_Mobile));

                        if (this.m_Mobile.NetState != null)
                            this.m_From.SendGump(new ClientGump(this.m_From, this.m_Mobile.NetState));

                        break;
                    }
            }
        }

        private void InvokeCommand(string ip)
        {
            CommandSystem.Handle(this.m_From, String.Format("{0}{1}", CommandSystem.Prefix, ip));
        }
    }
}