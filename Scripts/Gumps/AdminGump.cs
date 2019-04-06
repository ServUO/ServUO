using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Server.Accounting;
using Server.Commands;
using Server.Items;
using Server.Misc;
using Server.Multis;
using Server.Network;
using Server.Prompts;

namespace Server.Gumps
{
    public enum AdminGumpPage
    {
        Information_General,
        Information_Perf,
        Administer,
        Clients,
        Accounts,
        Accounts_Shared,
        Firewall,
        Administer_WorldBuilding,
        Administer_Server,
        Administer_Access,
        Administer_Access_Lockdown,
        Administer_Commands,
        Administer_Maintenance,
        ClientInfo,
        AccountDetails,
        AccountDetails_Information,
        AccountDetails_Characters,
        AccountDetails_Access,
        AccountDetails_Access_ClientIPs,
        AccountDetails_Access_Restrictions,
        AccountDetails_Comments,
        AccountDetails_Tags,
        AccountDetails_ChangePassword,
        AccountDetails_ChangeAccess,
        FirewallInfo
    }

    public class AdminGump : Gump
    {
        private readonly Mobile m_From;
        private readonly AdminGumpPage m_PageType;
        private readonly ArrayList m_List;
        private readonly int m_ListPage;
        private readonly object m_State;

        private const int LabelColor = 0x7FFF;
        private const int SelectedColor = 0x421F;
        private const int DisabledColor = 0x4210;

        private const int LabelColor32 = 0xFFFFFF;
        private const int SelectedColor32 = 0x8080FF;
        private const int DisabledColor32 = 0x808080;

        private const int LabelHue = 0x480;
        private const int GreenHue = 0x40;
        private const int RedHue = 0x20;

        public void AddPageButton(int x, int y, int buttonID, string text, AdminGumpPage page, params AdminGumpPage[] subPages)
        {
            bool isSelection = (this.m_PageType == page);

            for (int i = 0; !isSelection && i < subPages.Length; ++i)
                isSelection = (this.m_PageType == subPages[i]);

            this.AddSelectedButton(x, y, buttonID, text, isSelection);
        }

        public void AddSelectedButton(int x, int y, int buttonID, string text, bool isSelection)
        {
            this.AddButton(x, y - 1, isSelection ? 4006 : 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            this.AddHtml(x + 35, y, 200, 20, this.Color(text, isSelection ? SelectedColor32 : LabelColor32), false, false);
        }

        public void AddButtonLabeled(int x, int y, int buttonID, string text)
        {
            this.AddButton(x, y - 1, 4005, 4007, buttonID, GumpButtonType.Reply, 0);
            this.AddHtml(x + 35, y, 240, 20, this.Color(text, LabelColor32), false, false);
        }

        public string Center(string text)
        {
            return String.Format("<CENTER>{0}</CENTER>", text);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public void AddBlackAlpha(int x, int y, int width, int height)
        {
            this.AddImageTiled(x, y, width, height, 2624);
            this.AddAlphaRegion(x, y, width, height);
        }

        public int GetButtonID(int type, int index)
        {
            return 1 + (index * 11) + type;
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return String.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60);
        }

        public static string FormatByteAmount(long totalBytes)
        {
            if (totalBytes > 1000000000)
                return String.Format("{0:F1} GB", (double)totalBytes / 1073741824);

            if (totalBytes > 1000000)
                return String.Format("{0:F1} MB", (double)totalBytes / 1048576);

            if (totalBytes > 1000)
                return String.Format("{0:F1} KB", (double)totalBytes / 1024);

            return String.Format("{0} Bytes", totalBytes);
        }

        public static void Initialize()
        {
            CommandSystem.Register("Admin", AccessLevel.Administrator, new CommandEventHandler(Admin_OnCommand));
        }

        [Usage("Admin")]
        [Description("Opens an interface providing server information and administration features including client, account, and firewall management.")]
        public static void Admin_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendGump(new AdminGump(e.Mobile, AdminGumpPage.Clients, 0, null, null, null));
        }

        public static int GetHueFor(Mobile m)
        {
            if (m == null)
                return LabelHue;

            switch ( m.AccessLevel )
            {
                case AccessLevel.Owner:
                    return 0x516;
                case AccessLevel.Developer:
                    return 0x516;
                case AccessLevel.Administrator:
                    return 0x516;
                case AccessLevel.Seer:
                    return 0x144;
                case AccessLevel.GameMaster:
                    return 0x21;
                case AccessLevel.Counselor:
                    return 0x2;
                case AccessLevel.VIP:
                case AccessLevel.Player:
                default:
                    {
                        if (m.Murderer)
                            return 0x21;
                        else if (m.Criminal)
                            return 0x3B1;

                        return 0x58;
                    }
            }
        }

        private static readonly string[] m_AccessLevelStrings = new string[]
        {
            "Player",
            "VIP",
            "Counselor",
            "Decorator",
            "Spawner",
            "Game Master",
            "Seer",
            "Administrator",
            "Developer",
            "Co-Owner",
            "Owner"
        };

        public static string FormatAccessLevel(AccessLevel level)
        {
            int v = (int)level;

            if (v >= 0 && v < m_AccessLevelStrings.Length)
                return m_AccessLevelStrings[v];

            return "Unknown";
        }

        public AdminGump(Mobile from, AdminGumpPage pageType, int listPage, ArrayList list, string notice, object state)
            : base(50, 40)
        {
            from.CloseGump(typeof(AdminGump));

            this.m_From = from;
            this.m_PageType = pageType;
            this.m_ListPage = listPage;
            this.m_State = state;
            this.m_List = list;

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 440, 5054);

            this.AddBlackAlpha(10, 10, 170, 100);
            this.AddBlackAlpha(190, 10, 220, 100);
            this.AddBlackAlpha(10, 120, 400, 260);
            this.AddBlackAlpha(10, 390, 400, 40);

            this.AddPageButton(10, 10, this.GetButtonID(0, 0), "INFORMATION", AdminGumpPage.Information_General, AdminGumpPage.Information_Perf);
            this.AddPageButton(10, 30, this.GetButtonID(0, 1), "ADMINISTER", AdminGumpPage.Administer, AdminGumpPage.Administer_Access, AdminGumpPage.Administer_Commands, AdminGumpPage.Administer_Server, AdminGumpPage.Administer_WorldBuilding, AdminGumpPage.Administer_Access_Lockdown, AdminGumpPage.Administer_Maintenance);
            this.AddPageButton(10, 50, this.GetButtonID(0, 2), "CLIENT LIST", AdminGumpPage.Clients, AdminGumpPage.ClientInfo);
            this.AddPageButton(10, 70, this.GetButtonID(0, 3), "ACCOUNT LIST", AdminGumpPage.Accounts, AdminGumpPage.Accounts_Shared, AdminGumpPage.AccountDetails, AdminGumpPage.AccountDetails_Information, AdminGumpPage.AccountDetails_Characters, AdminGumpPage.AccountDetails_Access, AdminGumpPage.AccountDetails_Access_ClientIPs, AdminGumpPage.AccountDetails_Access_Restrictions, AdminGumpPage.AccountDetails_Comments, AdminGumpPage.AccountDetails_Tags, AdminGumpPage.AccountDetails_ChangeAccess, AdminGumpPage.AccountDetails_ChangePassword);
            this.AddPageButton(10, 90, this.GetButtonID(0, 4), "FIREWALL", AdminGumpPage.Firewall, AdminGumpPage.FirewallInfo);

            if (notice != null)
                this.AddHtml(12, 392, 396, 36, this.Color(notice, LabelColor32), false, false);

            switch ( pageType )
            {
                case AdminGumpPage.Information_General:
                    {
                        int banned = 0;
                        int active = 0;

                        foreach (Account acct in Accounts.GetAccounts())
                        {
                            if (acct.Banned)
                                ++banned;
                            else
                                ++active;
                        }

                        this.AddLabel(20, 130, LabelHue, "Active Accounts:");
                        this.AddLabel(150, 130, LabelHue, active.ToString());

                        this.AddLabel(20, 150, LabelHue, "Banned Accounts:");
                        this.AddLabel(150, 150, LabelHue, banned.ToString());

                        this.AddLabel(20, 170, LabelHue, "Firewalled:");
                        this.AddLabel(150, 170, LabelHue, Firewall.List.Count.ToString());

                        this.AddLabel(20, 190, LabelHue, "Clients:");
                        this.AddLabel(150, 190, LabelHue, NetState.Instances.Count.ToString());

                        this.AddLabel(20, 210, LabelHue, "Mobiles:");
                        this.AddLabel(150, 210, LabelHue, World.Mobiles.Count.ToString());

                        this.AddLabel(20, 230, LabelHue, "Mobile Scripts:");
                        this.AddLabel(150, 230, LabelHue, Core.ScriptMobiles.ToString());

                        this.AddLabel(20, 250, LabelHue, "Items:");
                        this.AddLabel(150, 250, LabelHue, World.Items.Count.ToString());

                        this.AddLabel(20, 270, LabelHue, "Item Scripts:");
                        this.AddLabel(150, 270, LabelHue, Core.ScriptItems.ToString());

                        this.AddLabel(20, 290, LabelHue, "Uptime:");
                        this.AddLabel(150, 290, LabelHue, FormatTimeSpan(DateTime.UtcNow - Clock.ServerStart));

                        this.AddLabel(20, 310, LabelHue, "Memory:");
                        this.AddLabel(150, 310, LabelHue, FormatByteAmount(GC.GetTotalMemory(false)));

                        this.AddLabel(20, 330, LabelHue, "Framework:");
                        this.AddLabel(150, 330, LabelHue, Environment.Version.ToString());

                        this.AddLabel(20, 350, LabelHue, "Operating System: ");
                        string os = Environment.OSVersion.ToString();

                        os = os.Replace("Service Pack", "SP");

                        this.AddLabel(150, 350, LabelHue, os);

                        /*string str;

                        try{ str = FormatTimeSpan( Core.Process.TotalProcessorTime ); }
                        catch{ str = "(unable to retrieve)"; }

                        AddLabel( 20, 330, LabelHue, "Process Time:" );
                        AddLabel( 250, 330, LabelHue, str );*/

                        /*try{ str = Core.Process.PriorityClass.ToString(); }
                        catch{ str = "(unable to retrieve)"; }

                        AddLabel( 20, 350, LabelHue, "Process Priority:" );
                        AddLabel( 250, 350, LabelHue, str );*/

                        this.AddPageButton(200, 20, this.GetButtonID(0, 0), "General", AdminGumpPage.Information_General);
                        this.AddPageButton(200, 40, this.GetButtonID(0, 5), "Performance", AdminGumpPage.Information_Perf);

                        break;
                    }
                case AdminGumpPage.Information_Perf:
                    {
                        this.AddLabel(20, 130, LabelHue, "Cycles Per Second:");
                        this.AddLabel(40, 150, LabelHue, "Current: " + Core.CyclesPerSecond.ToString("N2"));
                        this.AddLabel(40, 170, LabelHue, "Average: " + Core.AverageCPS.ToString("N2"));

                        StringBuilder sb = new StringBuilder();

                        int curUser, maxUser;
                        int curIOCP, maxIOCP;

                        System.Threading.ThreadPool.GetAvailableThreads(out curUser, out curIOCP);
                        System.Threading.ThreadPool.GetMaxThreads(out maxUser, out maxIOCP);

                        sb.Append("Worker Threads:<br>Capacity: ");
                        sb.Append(maxUser);
                        sb.Append("<br>Available: ");
                        sb.Append(curUser);
                        sb.Append("<br>Usage: ");
                        sb.Append(((maxUser - curUser) * 100) / maxUser);
                        sb.Append("%<br><br>IOCP Threads:<br>Capacity: ");
                        sb.Append(maxIOCP);
                        sb.Append("<br>Available: ");
                        sb.Append(curIOCP);
                        sb.Append("<br>Usage: ");
                        sb.Append(((maxIOCP - curIOCP) * 100) / maxIOCP);
                        sb.Append("%");

                        List<BufferPool> pools = BufferPool.Pools;

                        lock (pools)
                        {
                            for (int i = 0; i < pools.Count; ++i)
                            {
                                BufferPool pool = pools[i];
                                string name;
                                int freeCount;
                                int initialCapacity;
                                int currentCapacity;
                                int bufferSize;
                                int misses;

                                pool.GetInfo(out name, out freeCount, out initialCapacity, out currentCapacity, out bufferSize, out misses);

                                if (sb.Length > 0)
                                    sb.Append("<br><br>");

                                sb.Append(name);
                                sb.Append("<br>Size: ");
                                sb.Append(FormatByteAmount(bufferSize));
                                sb.Append("<br>Capacity: ");
                                sb.Append(currentCapacity);
                                sb.Append(" (");
                                sb.Append(misses);
                                sb.Append(" misses)<br>Available: ");
                                sb.Append(freeCount);
                                sb.Append("<br>Usage: ");
                                sb.Append(((currentCapacity - freeCount) * 100) / currentCapacity);
                                sb.Append("% : ");
                                sb.Append(FormatByteAmount((currentCapacity - freeCount) * bufferSize));
                                sb.Append(" of ");
                                sb.Append(FormatByteAmount(currentCapacity * bufferSize));
                            }
                        }

                        this.AddLabel(20, 200, LabelHue, "Pooling:");
                        this.AddHtml(20, 220, 380, 150, sb.ToString(), true, true);

                        this.AddPageButton(200, 20, this.GetButtonID(0, 0), "General", AdminGumpPage.Information_General);
                        this.AddPageButton(200, 40, this.GetButtonID(0, 5), "Performance", AdminGumpPage.Information_Perf);

                        break;
                    }
                case AdminGumpPage.Administer_WorldBuilding:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Generating"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(3, 101), "Create World");
                        this.AddButtonLabeled(20, 175, this.GetButtonID(3, 102), "Delete World");
                        this.AddButtonLabeled(20, 200, this.GetButtonID(3, 103), "Recreate World");

                        this.AddHtml(20, 275, 400, 30, this.Color(this.Center("Statics"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 300, this.GetButtonID(3, 110), "Freeze (Target)");
                        this.AddButtonLabeled(20, 325, this.GetButtonID(3, 111), "Freeze (World)");
                        this.AddButtonLabeled(20, 350, this.GetButtonID(3, 112), "Freeze (Map)");

                        this.AddButtonLabeled(220, 300, this.GetButtonID(3, 120), "Unfreeze (Target)");
                        this.AddButtonLabeled(220, 325, this.GetButtonID(3, 121), "Unfreeze (World)");
                        this.AddButtonLabeled(220, 350, this.GetButtonID(3, 122), "Unfreeze (Map)");

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer_Server:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Server"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(3, 200), "Save");

                        /*if ( !Core.Service )
                        {*/
                        this.AddButtonLabeled(20, 180, this.GetButtonID(3, 201), "Shutdown (With Save)");
                        this.AddButtonLabeled(20, 200, this.GetButtonID(3, 202), "Shutdown (Without Save)");

                        this.AddButtonLabeled(20, 230, this.GetButtonID(3, 203), "Shutdown & Restart (With Save)");
                        this.AddButtonLabeled(20, 250, this.GetButtonID(3, 204), "Shutdown & Restart (Without Save)");
                        /*}
                        else
                        {
                        AddLabel( 20, 215, LabelHue, "Shutdown/Restart not available." );
                        }*/

                        this.AddHtml(10, 295, 400, 20, this.Color(this.Center("Broadcast"), LabelColor32), false, false);

                        this.AddTextField(20, 320, 380, 20, 0);
                        this.AddButtonLabeled(20, 350, this.GetButtonID(3, 210), "To Everyone");
                        this.AddButtonLabeled(220, 350, this.GetButtonID(3, 211), "To Staff");

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer_Access_Lockdown:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Server Lockdown"), LabelColor32), false, false);

                        this.AddHtml(20, 150, 380, 80, this.Color("When enabled, only clients with an access level equal to or greater than the specified lockdown level may access the server. After setting a lockdown level, use the <em>Purge Invalid Clients</em> button to disconnect those clients without access.", LabelColor32), false, false);

                        AccessLevel level = Misc.AccountHandler.LockdownLevel;
                        bool isLockedDown = (level > AccessLevel.VIP);

                        this.AddSelectedButton(20, 230, this.GetButtonID(3, 500), "Not Locked Down", !isLockedDown);
                        this.AddSelectedButton(20, 260, this.GetButtonID(3, 504), "Administrators", (isLockedDown && level <= AccessLevel.Administrator));
                        this.AddSelectedButton(20, 280, this.GetButtonID(3, 503), "Seers", (isLockedDown && level <= AccessLevel.Seer));
                        this.AddSelectedButton(20, 300, this.GetButtonID(3, 502), "Game Masters", (isLockedDown && level <= AccessLevel.GameMaster));
                        this.AddSelectedButton(20, 320, this.GetButtonID(3, 501), "Counselors", (isLockedDown && level <= AccessLevel.Counselor));

                        this.AddButtonLabeled(20, 350, this.GetButtonID(3, 510), "Purge Invalid Clients");

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer_Access:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Access"), LabelColor32), false, false);

                        this.AddHtml(10, 155, 400, 20, this.Color(this.Center("Connectivity"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 180, this.GetButtonID(3, 300), "Kick");
                        this.AddButtonLabeled(220, 180, this.GetButtonID(3, 301), "Ban");

                        this.AddButtonLabeled(20, 210, this.GetButtonID(3, 302), "Firewall");
                        this.AddButtonLabeled(220, 210, this.GetButtonID(3, 303), "Lockdown");

                        this.AddHtml(10, 245, 400, 20, this.Color(this.Center("Staff"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 270, this.GetButtonID(3, 310), "Make Player");
                        this.AddButtonLabeled(20, 290, this.GetButtonID(3, 311), "Make Counselor");
                        this.AddButtonLabeled(20, 310, this.GetButtonID(3, 312), "Make Game Master");
                        this.AddButtonLabeled(20, 330, this.GetButtonID(3, 313), "Make Seer");

                        if (from.AccessLevel > AccessLevel.Administrator)
                        {
                            this.AddButtonLabeled(220, 270, this.GetButtonID(3, 314), "Make Administrator");

                            if (from.AccessLevel > AccessLevel.Developer)
                            {
                                this.AddButtonLabeled(220, 290, this.GetButtonID(3, 315), "Make Developer");

                                if (from.AccessLevel >= AccessLevel.Owner)
                                    this.AddButtonLabeled(220, 310, this.GetButtonID(3, 316), "Make Owner");
                            }
                        }

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer_Maintenance:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Maintenance"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(3, 600), "Rebuild Categorization");
                        this.AddButtonLabeled(220, 150, this.GetButtonID(3, 601), "Generate Documentation");

                        if (Ultima.Files.MulPath["artlegacymul.uop"] != null || (Ultima.Files.MulPath["art.mul"] != null && Ultima.Files.MulPath["artidx.mul"] != null))
                        {
                            this.AddButtonLabeled(20, 180, this.GetButtonID(3, 602), "Rebuild Bounds.bin");
                        }
                        else
                        {
                            this.AddLabelCropped(55, 180, 120, 20, RedHue, "Rebuild Bounds.bin");
                        }

                        this.AddButtonLabeled(220, 180, this.GetButtonID(3, 603), "Generate Reports");

                        this.AddHtml(10, 210, 400, 20, this.Color(this.Center("Profiling"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 240, this.GetButtonID(3, 604), "Dump Timers");
                        this.AddButtonLabeled(220, 240, this.GetButtonID(3, 605), "Count Objects");

                        this.AddButtonLabeled(20, 270, this.GetButtonID(3, 606), "Profile World");
                        this.AddButtonLabeled(220, 270, this.GetButtonID(3, 607), "Write Profiles");

                        this.AddButtonLabeled(20, 300, this.GetButtonID(3, 608), "Trace Internal");
                        this.AddButtonLabeled(220, 300, this.GetButtonID(3, 609), "Trace Expanded");

                        this.AddButtonLabeled(20, 330, this.GetButtonID(3, 610), "Toggle Profiles");

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer_Commands:
                    {
                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Commands"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(3, 400), "Add");
                        this.AddButtonLabeled(220, 150, this.GetButtonID(3, 401), "Remove");

                        this.AddButtonLabeled(20, 170, this.GetButtonID(3, 402), "Dupe");
                        this.AddButtonLabeled(220, 170, this.GetButtonID(3, 403), "Dupe in bag");

                        this.AddButtonLabeled(20, 200, this.GetButtonID(3, 404), "Properties");
                        this.AddButtonLabeled(220, 200, this.GetButtonID(3, 405), "Skills");

                        this.AddButtonLabeled(20, 230, this.GetButtonID(3, 406), "Mortal");
                        this.AddButtonLabeled(220, 230, this.GetButtonID(3, 407), "Immortal");

                        this.AddButtonLabeled(20, 250, this.GetButtonID(3, 408), "Squelch");
                        this.AddButtonLabeled(220, 250, this.GetButtonID(3, 409), "Unsquelch");

                        this.AddButtonLabeled(20, 270, this.GetButtonID(3, 410), "Freeze");
                        this.AddButtonLabeled(220, 270, this.GetButtonID(3, 411), "Unfreeze");

                        this.AddButtonLabeled(20, 290, this.GetButtonID(3, 412), "Hide");
                        this.AddButtonLabeled(220, 290, this.GetButtonID(3, 413), "Unhide");

                        this.AddButtonLabeled(20, 310, this.GetButtonID(3, 414), "Kill");
                        this.AddButtonLabeled(220, 310, this.GetButtonID(3, 415), "Resurrect");

                        this.AddButtonLabeled(20, 330, this.GetButtonID(3, 416), "Move");
                        this.AddButtonLabeled(220, 330, this.GetButtonID(3, 417), "Wipe");

                        this.AddButtonLabeled(20, 350, this.GetButtonID(3, 418), "Teleport");
                        this.AddButtonLabeled(220, 350, this.GetButtonID(3, 419), "Teleport (Multiple)");

                        goto case AdminGumpPage.Administer;
                    }
                case AdminGumpPage.Administer:
                    {
                        this.AddPageButton(200, 10, this.GetButtonID(3, 0), "World Building", AdminGumpPage.Administer_WorldBuilding);
                        this.AddPageButton(200, 30, this.GetButtonID(3, 1), "Server", AdminGumpPage.Administer_Server);
                        this.AddPageButton(200, 50, this.GetButtonID(3, 2), "Access", AdminGumpPage.Administer_Access, AdminGumpPage.Administer_Access_Lockdown);
                        this.AddPageButton(200, 70, this.GetButtonID(3, 3), "Commands", AdminGumpPage.Administer_Commands);
                        this.AddPageButton(200, 90, this.GetButtonID(3, 4), "Maintenance", AdminGumpPage.Administer_Maintenance);

                        break;
                    }
                case AdminGumpPage.Clients:
                    {
                        if (this.m_List == null)
                        {
                            this.m_List = new ArrayList(NetState.Instances);
                            this.m_List.Sort(NetStateComparer.Instance);
                        }

                        this.AddClientHeader();

                        this.AddLabelCropped(12, 120, 81, 20, LabelHue, "Name");
                        this.AddLabelCropped(95, 120, 81, 20, LabelHue, "Account");
                        this.AddLabelCropped(178, 120, 81, 20, LabelHue, "Access Level");
                        this.AddLabelCropped(273, 120, 109, 20, LabelHue, "IP Address");

                        if (listPage > 0)
                            this.AddButton(375, 122, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(375, 122, 0x25EA);

                        if ((listPage + 1) * 12 < this.m_List.Count)
                            this.AddButton(392, 122, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(392, 122, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddLabel(12, 140, LabelHue, "There are no clients to display.");

                        for (int i = 0, index = (listPage * 12); i < 12 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            NetState ns = this.m_List[index] as NetState;

                            if (ns == null)
                                continue;

                            Mobile m = ns.Mobile;
                            Account a = ns.Account as Account;
                            int offset = 140 + (i * 20);

                            if (m == null)
                            {
                                if (RemoteAdmin.AdminNetwork.IsAuth(ns))
                                    this.AddLabelCropped(12, offset, 81, 20, LabelHue, "(remote admin)");
                                else
                                    this.AddLabelCropped(12, offset, 81, 20, LabelHue, "(logging in)");
                            }
                            else
                            {
                                this.AddLabelCropped(12, offset, 81, 20, GetHueFor(m), m.Name);
                            }
                            this.AddLabelCropped(95, offset, 81, 20, LabelHue, a == null ? "(no account)" : a.Username);
                            this.AddLabelCropped(178, offset, 81, 20, LabelHue, m == null ? (a != null ? FormatAccessLevel(a.AccessLevel) : "") : FormatAccessLevel(m.AccessLevel));
                            this.AddLabelCropped(273, offset, 109, 20, LabelHue, ns.ToString());

                            if (a != null || m != null)
                                this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(4, index + 2), GumpButtonType.Reply, 0);
                        }

                        break;
                    }
                case AdminGumpPage.ClientInfo:
                    {
                        Mobile m = state as Mobile;

                        if (m == null)
                            break;

                        this.AddClientHeader();

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Information"), LabelColor32), false, false);

                        int y = 146;

                        this.AddLabel(20, y, LabelHue, "Name:");
                        this.AddLabel(200, y, GetHueFor(m), m.Name);
                        y += 20;

                        Account a = m.Account as Account;

                        this.AddLabel(20, y, LabelHue, "Account:");
                        this.AddLabel(200, y, (a != null && a.Banned) ? RedHue : LabelHue, a == null ? "(no account)" : a.Username);
                        this.AddButton(380, y, 0xFA5, 0xFA7, this.GetButtonID(7, 14), GumpButtonType.Reply, 0);
                        y += 20;

                        NetState ns = m.NetState;

                        if (ns == null)
                        {
                            this.AddLabel(20, y, LabelHue, "Address:");
                            this.AddLabel(200, y, RedHue, "Offline");
                            y += 20;

                            this.AddLabel(20, y, LabelHue, "Location:");
                            this.AddLabel(200, y, LabelHue, String.Format("{0} [{1}]", m.Location, m.Map));
                            y += 44;
                        }
                        else
                        {
                            this.AddLabel(20, y, LabelHue, "Address:");
                            this.AddLabel(200, y, GreenHue, ns.ToString());
                            y += 20;

                            ClientVersion v = ns.Version;

                            this.AddLabel(20, y, LabelHue, "Version:");
                            this.AddLabel(200, y, LabelHue, v == null ? "(null)" : v.ToString());
                            y += 20;

                            this.AddLabel(20, y, LabelHue, "Location:");
                            this.AddLabel(200, y, LabelHue, String.Format("{0} [{1}]", m.Location, m.Map));
                            y += 24;
                        }

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 0), "Go to");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 1), "Get");
                        y += 20;

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 2), "Kick");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 3), "Ban");
                        y += 20;

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 4), "Properties");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 5), "Skills");
                        y += 20;

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 6), "Mortal");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 7), "Immortal");
                        y += 20;

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 8), "Squelch");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 9), "Unsquelch");
                        y += 20;

                        /*AddButtonLabeled(  20, y, GetButtonID( 7, 10 ), "Hide" );
                        AddButtonLabeled( 200, y, GetButtonID( 7, 11 ), "Unhide" );
                        y += 20;*/

                        this.AddButtonLabeled(20, y, this.GetButtonID(7, 12), "Kill");
                        this.AddButtonLabeled(200, y, this.GetButtonID(7, 13), "Resurrect");
                        y += 20;

                        break;
                    }
                case AdminGumpPage.Accounts_Shared:
                    {
                        if (this.m_List == null)
                            this.m_List = GetAllSharedAccounts();

                        this.AddLabelCropped(12, 120, 60, 20, LabelHue, "Count");
                        this.AddLabelCropped(72, 120, 120, 20, LabelHue, "Address");
                        this.AddLabelCropped(192, 120, 180, 20, LabelHue, "Accounts");

                        if (listPage > 0)
                            this.AddButton(375, 122, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(375, 122, 0x25EA);

                        if ((listPage + 1) * 12 < this.m_List.Count)
                            this.AddButton(392, 122, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(392, 122, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddLabel(12, 140, LabelHue, "There are no accounts to display.");

                        StringBuilder sb = new StringBuilder();

                        for (int i = 0, index = (listPage * 12); i < 12 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            DictionaryEntry de = (DictionaryEntry)this.m_List[index];

                            IPAddress ipAddr = (IPAddress)de.Key;
                            ArrayList accts = (ArrayList)de.Value;

                            int offset = 140 + (i * 20);

                            this.AddLabelCropped(12, offset, 60, 20, LabelHue, accts.Count.ToString());
                            this.AddLabelCropped(72, offset, 120, 20, LabelHue, ipAddr.ToString());

                            if (sb.Length > 0)
                                sb.Length = 0;

                            for (int j = 0; j < accts.Count; ++j)
                            {
                                if (j > 0)
                                    sb.Append(", ");

                                if (j < 4)
                                {
                                    Account acct = (Account)accts[j];

                                    sb.Append(acct.Username);
                                }
                                else
                                {
                                    sb.Append("...");
                                    break;
                                }
                            }

                            this.AddLabelCropped(192, offset, 180, 20, LabelHue, sb.ToString());

                            this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(5, index + 56), GumpButtonType.Reply, 0);
                        }

                        break;
                    }
                case AdminGumpPage.Accounts:
                    {
                        if (this.m_List == null)
                        {
                            this.m_List = new ArrayList((ICollection)Accounts.GetAccounts());
                            this.m_List.Sort(AccountComparer.Instance);
                        }

                        ArrayList rads = (state as ArrayList);

                        this.AddAccountHeader();

                        if (rads == null)
                            this.AddLabelCropped(12, 120, 120, 20, LabelHue, "Name");
                        else
                            this.AddLabelCropped(32, 120, 100, 20, LabelHue, "Name");

                        this.AddLabelCropped(132, 120, 120, 20, LabelHue, "Access Level");
                        this.AddLabelCropped(252, 120, 120, 20, LabelHue, "Status");

                        if (listPage > 0)
                            this.AddButton(375, 122, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(375, 122, 0x25EA);

                        if ((listPage + 1) * 12 < this.m_List.Count)
                            this.AddButton(392, 122, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(392, 122, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddLabel(12, 140, LabelHue, "There are no accounts to display.");

                        if (rads != null && notice == null)
                        {
                            this.AddButtonLabeled(10, 390, this.GetButtonID(5, 27), "Ban marked");
                            this.AddButtonLabeled(10, 410, this.GetButtonID(5, 28), "Delete marked");

                            this.AddButtonLabeled(210, 390, this.GetButtonID(5, 29), "Mark all");
                            this.AddButtonLabeled(210, 410, this.GetButtonID(5, 35), "Unmark house owners");
                        }

                        for (int i = 0, index = (listPage * 12); i < 12 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            Account a = this.m_List[index] as Account;

                            if (a == null)
                                continue;

                            int offset = 140 + (i * 20);

                            AccessLevel accessLevel;
                            bool online;

                            GetAccountInfo(a, out accessLevel, out online);

                            if (rads == null)
                            {
                                this.AddLabelCropped(12, offset, 120, 20, LabelHue, a.Username);
                            }
                            else
                            {
                                this.AddCheck(10, offset, 0xD2, 0xD3, rads.Contains(a), index);
                                this.AddLabelCropped(32, offset, 100, 20, LabelHue, a.Username);
                            }

                            this.AddLabelCropped(132, offset, 120, 20, LabelHue, FormatAccessLevel(accessLevel));

                            if (online)
                                this.AddLabelCropped(252, offset, 120, 20, GreenHue, "Online");
                            else if (a.Banned)
                                this.AddLabelCropped(252, offset, 120, 20, RedHue, "Banned");
                            else
                                this.AddLabelCropped(252, offset, 120, 20, RedHue, "Offline");

                            this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(5, index + 56), GumpButtonType.Reply, 0);
                        }

                        break;
                    }
                case AdminGumpPage.AccountDetails:
                    {
                        this.AddPageButton(190, 10, this.GetButtonID(5, 0), "Information", AdminGumpPage.AccountDetails_Information, AdminGumpPage.AccountDetails_ChangeAccess, AdminGumpPage.AccountDetails_ChangePassword);
                        this.AddPageButton(190, 30, this.GetButtonID(5, 1), "Characters", AdminGumpPage.AccountDetails_Characters);
                        this.AddPageButton(190, 50, this.GetButtonID(5, 13), "Access", AdminGumpPage.AccountDetails_Access, AdminGumpPage.AccountDetails_Access_ClientIPs, AdminGumpPage.AccountDetails_Access_Restrictions);
                        this.AddPageButton(190, 70, this.GetButtonID(5, 2), "Comments", AdminGumpPage.AccountDetails_Comments);
                        this.AddPageButton(190, 90, this.GetButtonID(5, 3), "Tags", AdminGumpPage.AccountDetails_Tags);
                        break;
                    }
                case AdminGumpPage.AccountDetails_ChangePassword:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Change Password"), LabelColor32), false, false);

                        this.AddLabel(20, 150, LabelHue, "Username:");
                        this.AddLabel(200, 150, LabelHue, a.Username);

                        this.AddLabel(20, 180, LabelHue, "Password:");
                        this.AddTextField(200, 180, 160, 20, 0);

                        this.AddLabel(20, 210, LabelHue, "Confirm:");
                        this.AddTextField(200, 210, 160, 20, 1);

                        this.AddButtonLabeled(20, 240, this.GetButtonID(5, 12), "Submit Change");

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_ChangeAccess:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Change Access Level"), LabelColor32), false, false);

                        this.AddLabel(20, 150, LabelHue, "Username:");
                        this.AddLabel(200, 150, LabelHue, a.Username);

                        this.AddLabel(20, 170, LabelHue, "Current Level:");
                        this.AddLabel(200, 170, LabelHue, FormatAccessLevel(a.AccessLevel));

                        this.AddButtonLabeled(20, 200, this.GetButtonID(5, 20), "Player");
                        this.AddButtonLabeled(20, 220, this.GetButtonID(5, 21), "Counselor");
                        this.AddButtonLabeled(20, 240, this.GetButtonID(5, 22), "Game Master");
                        this.AddButtonLabeled(20, 260, this.GetButtonID(5, 23), "Seer");

                        if (from.AccessLevel > AccessLevel.Administrator)
                        {
                            this.AddButtonLabeled(20, 280, this.GetButtonID(5, 24), "Administrator");

                            if (from.AccessLevel > AccessLevel.Developer)
                            {
                                this.AddButtonLabeled(20, 300, this.GetButtonID(5, 33), "Developer");

                                if (from.AccessLevel >= AccessLevel.Owner)
                                    this.AddButtonLabeled(20, 320, this.GetButtonID(5, 34), "Owner");
                            }
                        }

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_Information:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        int charCount = 0;

                        for (int i = 0; i < a.Length; ++i)
                        {
                            if (a[i] != null)
                                ++charCount;
                        }

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Information"), LabelColor32), false, false);

                        this.AddLabel(20, 150, LabelHue, "Username:");
                        this.AddLabel(200, 150, LabelHue, a.Username);

                        this.AddLabel(20, 170, LabelHue, "Access Level:");
                        this.AddLabel(200, 170, LabelHue, FormatAccessLevel(a.AccessLevel));

                        this.AddLabel(20, 190, LabelHue, "Status:");
                        this.AddLabel(200, 190, a.Banned ? RedHue : GreenHue, a.Banned ? "Banned" : "Active");

                        DateTime banTime;
                        TimeSpan banDuration;

                        if (a.Banned && a.GetBanTags(out banTime, out banDuration))
                        {
                            if (banDuration == TimeSpan.MaxValue)
                            {
                                this.AddLabel(250, 190, LabelHue, "(Infinite)");
                            }
                            else if (banDuration == TimeSpan.Zero)
                            {
                                this.AddLabel(250, 190, LabelHue, "(Zero)");
                            }
                            else
                            {
                                TimeSpan remaining = (DateTime.UtcNow - banTime);

                                if (remaining < TimeSpan.Zero)
                                    remaining = TimeSpan.Zero;
                                else if (remaining > banDuration)
                                    remaining = banDuration;

                                double remMinutes = remaining.TotalMinutes;
                                double totMinutes = banDuration.TotalMinutes;

                                double perc = remMinutes / totMinutes;

                                this.AddLabel(250, 190, LabelHue, String.Format("{0} [{1:F0}%]", FormatTimeSpan(banDuration), perc * 100));
                            }
                        }
                        else if (a.Banned)
                        {
                            this.AddLabel(250, 190, LabelHue, "(Unspecified)");
                        }

                        this.AddLabel(20, 210, LabelHue, "Created:");
                        this.AddLabel(200, 210, LabelHue, a.Created.ToString());

                        this.AddLabel(20, 230, LabelHue, "Last Login:");
                        this.AddLabel(200, 230, LabelHue, a.LastLogin.ToString());

                        this.AddLabel(20, 250, LabelHue, "Character Count:");
                        this.AddLabel(200, 250, LabelHue, charCount.ToString());

                        this.AddLabel(20, 270, LabelHue, "Comment Count:");
                        this.AddLabel(200, 270, LabelHue, a.Comments.Count.ToString());

                        this.AddLabel(20, 290, LabelHue, "Tag Count:");
                        this.AddLabel(200, 290, LabelHue, a.Tags.Count.ToString());

                        this.AddButtonLabeled(20, 320, this.GetButtonID(5, 8), "Change Password");
                        this.AddButtonLabeled(200, 320, this.GetButtonID(5, 9), "Change Access Level");

                        if (!a.Banned)
                            this.AddButtonLabeled(20, 350, this.GetButtonID(5, 10), "Ban Account");
                        else
                            this.AddButtonLabeled(20, 350, this.GetButtonID(5, 11), "Unban Account");

                        this.AddButtonLabeled(200, 350, this.GetButtonID(5, 25), "Delete Account");

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_Access:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Access"), LabelColor32), false, false);

                        this.AddPageButton(20, 150, this.GetButtonID(5, 14), "View client addresses", AdminGumpPage.AccountDetails_Access_ClientIPs);
                        this.AddPageButton(20, 170, this.GetButtonID(5, 15), "Manage restrictions", AdminGumpPage.AccountDetails_Access_Restrictions);

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_Access_ClientIPs:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        if (this.m_List == null)
                            this.m_List = new ArrayList(a.LoginIPs);

                        this.AddHtml(10, 195, 400, 20, this.Color(this.Center("Client Addresses"), LabelColor32), false, false);

                        this.AddButtonLabeled(227, 225, this.GetButtonID(5, 16), "View all shared accounts");
                        this.AddButtonLabeled(227, 245, this.GetButtonID(5, 17), "Ban all shared accounts");
                        this.AddButtonLabeled(227, 265, this.GetButtonID(5, 18), "Firewall all addresses");
                        this.AddButtonLabeled(227, 285, this.GetButtonID(5, 36), "Clear all addresses");

                        this.AddHtml(225, 315, 180, 80, this.Color("List of IP addresses which have accessed this account.", LabelColor32), false, false);

                        this.AddImageTiled(15, 219, 206, 156, 0xBBC);
                        this.AddBlackAlpha(16, 220, 204, 154);

                        this.AddHtml(18, 221, 114, 20, this.Color("IP Address", LabelColor32), false, false);

                        if (listPage > 0)
                            this.AddButton(184, 223, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(184, 223, 0x25EA);

                        if ((listPage + 1) * 6 < this.m_List.Count)
                            this.AddButton(201, 223, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(201, 223, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddHtml(18, 243, 200, 60, this.Color("This account has not yet been accessed.", LabelColor32), false, false);

                        for (int i = 0, index = (listPage * 6); i < 6 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            this.AddHtml(18, 243 + (i * 22), 114, 20, this.Color(this.m_List[index].ToString(), LabelColor32), false, false);
                            this.AddButton(130, 242 + (i * 22), 0xFA2, 0xFA4, this.GetButtonID(8, index), GumpButtonType.Reply, 0);
                            this.AddButton(160, 242 + (i * 22), 0xFA8, 0xFAA, this.GetButtonID(9, index), GumpButtonType.Reply, 0);
                            this.AddButton(190, 242 + (i * 22), 0xFB1, 0xFB3, this.GetButtonID(10, index), GumpButtonType.Reply, 0);
                        }

                        goto case AdminGumpPage.AccountDetails_Access;
                    }
                case AdminGumpPage.AccountDetails_Access_Restrictions:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        if (this.m_List == null)
                            this.m_List = new ArrayList(a.IPRestrictions);

                        this.AddHtml(10, 195, 400, 20, this.Color(this.Center("Address Restrictions"), LabelColor32), false, false);

                        this.AddTextField(227, 225, 120, 20, 0);

                        this.AddButtonLabeled(352, 225, this.GetButtonID(5, 19), "Add");

                        this.AddHtml(225, 255, 180, 120, this.Color("Any clients connecting from an address not in this list will be rejected. Or, if the list is empty, any client may connect.", LabelColor32), false, false);

                        this.AddImageTiled(15, 219, 206, 156, 0xBBC);
                        this.AddBlackAlpha(16, 220, 204, 154);

                        this.AddHtml(18, 221, 114, 20, this.Color("IP Address", LabelColor32), false, false);

                        if (listPage > 0)
                            this.AddButton(184, 223, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(184, 223, 0x25EA);

                        if ((listPage + 1) * 6 < this.m_List.Count)
                            this.AddButton(201, 223, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(201, 223, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddHtml(18, 243, 200, 60, this.Color("There are no addresses in this list.", LabelColor32), false, false);

                        for (int i = 0, index = (listPage * 6); i < 6 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            this.AddHtml(18, 243 + (i * 22), 114, 20, this.Color(this.m_List[index].ToString(), LabelColor32), false, false);
                            this.AddButton(190, 242 + (i * 22), 0xFB1, 0xFB3, this.GetButtonID(8, index), GumpButtonType.Reply, 0);
                        }

                        goto case AdminGumpPage.AccountDetails_Access;
                    }
                case AdminGumpPage.AccountDetails_Characters:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Characters"), LabelColor32), false, false);

                        this.AddLabelCropped(12, 150, 120, 20, LabelHue, "Name");
                        this.AddLabelCropped(132, 150, 120, 20, LabelHue, "Access Level");
                        this.AddLabelCropped(252, 150, 120, 20, LabelHue, "Status");

                        int index = 0;

                        for (int i = 0; i < a.Length; ++i)
                        {
                            Mobile m = a[i];

                            if (m == null)
                                continue;

                            int offset = 170 + (index * 20);

                            this.AddLabelCropped(12, offset, 120, 20, GetHueFor(m), m.Name);
                            this.AddLabelCropped(132, offset, 120, 20, LabelHue, FormatAccessLevel(m.AccessLevel));

                            if (m.NetState != null)
                                this.AddLabelCropped(252, offset, 120, 20, GreenHue, "Online");
                            else
                                this.AddLabelCropped(252, offset, 120, 20, RedHue, "Offline");

                            this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(5, i + 50), GumpButtonType.Reply, 0);

                            ++index;
                        }

                        if (index == 0)
                            this.AddLabel(12, 170, LabelHue, "The character list is empty.");

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_Comments:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Comments"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(5, 4), "Add Comment");

                        StringBuilder sb = new StringBuilder();

                        if (a.Comments.Count == 0)
                            sb.Append("There are no comments for this account.");

                        for (int i = 0; i < a.Comments.Count; ++i)
                        {
                            if (i > 0)
                                sb.Append("<BR><BR>");

                            AccountComment c = a.Comments[i];

                            sb.AppendFormat("[{0} on {1}]<BR>{2}", c.AddedBy, c.LastModified, c.Content);
                        }

                        this.AddHtml(20, 180, 380, 190, sb.ToString(), true, true);

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.AccountDetails_Tags:
                    {
                        Account a = state as Account;

                        if (a == null)
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center("Tags"), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(5, 5), "Add Tag");

                        StringBuilder sb = new StringBuilder();

                        if (a.Tags.Count == 0)
                            sb.Append("There are no tags for this account.");

                        for (int i = 0; i < a.Tags.Count; ++i)
                        {
                            if (i > 0)
                                sb.Append("<BR>");

                            AccountTag tag = a.Tags[i];

                            sb.AppendFormat("{0} = {1}", tag.Name, tag.Value);
                        }

                        this.AddHtml(20, 180, 380, 190, sb.ToString(), true, true);

                        goto case AdminGumpPage.AccountDetails;
                    }
                case AdminGumpPage.Firewall:
                    {
                        this.AddFirewallHeader();

                        if (this.m_List == null)
                            this.m_List = new ArrayList(Firewall.List);

                        this.AddLabelCropped(12, 120, 358, 20, LabelHue, "IP Address");

                        if (listPage > 0)
                            this.AddButton(375, 122, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(375, 122, 0x25EA);

                        if ((listPage + 1) * 12 < this.m_List.Count)
                            this.AddButton(392, 122, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(392, 122, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddLabel(12, 140, LabelHue, "The firewall list is empty.");

                        for (int i = 0, index = (listPage * 12); i < 12 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            object obj = this.m_List[index];

                            if (!(obj is Firewall.IFirewallEntry))
                                break;

                            int offset = 140 + (i * 20);

                            this.AddLabelCropped(12, offset, 358, 20, LabelHue, obj.ToString());
                            this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(6, index + 4), GumpButtonType.Reply, 0);
                        }

                        break;
                    }
                case AdminGumpPage.FirewallInfo:
                    {
                        this.AddFirewallHeader();

                        if (!(state is Firewall.IFirewallEntry))
                            break;

                        this.AddHtml(10, 125, 400, 20, this.Color(this.Center(state.ToString()), LabelColor32), false, false);

                        this.AddButtonLabeled(20, 150, this.GetButtonID(6, 3), "Remove");

                        this.AddHtml(10, 175, 400, 20, this.Color(this.Center("Potentially Affected Accounts"), LabelColor32), false, false);

                        if (this.m_List == null)
                        {
                            this.m_List = new ArrayList();

                            foreach (Account acct in Accounts.GetAccounts())
                            {
                                IPAddress[] loginList = acct.LoginIPs;

                                bool contains = false;

                                for (int i = 0; !contains && i < loginList.Length; ++i)
                                {
                                    if (((Firewall.IFirewallEntry)state).IsBlocked(loginList[i]))
                                    {
                                        this.m_List.Add(acct);
                                        break;
                                    }
                                }
                            }

                            this.m_List.Sort(AccountComparer.Instance);
                        }

                        if (listPage > 0)
                            this.AddButton(375, 177, 0x15E3, 0x15E7, this.GetButtonID(1, 0), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(375, 177, 0x25EA);

                        if ((listPage + 1) * 12 < this.m_List.Count)
                            this.AddButton(392, 177, 0x15E1, 0x15E5, this.GetButtonID(1, 1), GumpButtonType.Reply, 0);
                        else
                            this.AddImage(392, 177, 0x25E6);

                        if (this.m_List.Count == 0)
                            this.AddLabelCropped(12, 200, 398, 20, LabelHue, "No accounts found.");

                        for (int i = 0, index = (listPage * 9); i < 9 && index >= 0 && index < this.m_List.Count; ++i, ++index)
                        {
                            Account a = this.m_List[index] as Account;

                            if (a == null)
                                continue;

                            int offset = 200 + (i * 20);

                            AccessLevel accessLevel;
                            bool online;

                            GetAccountInfo(a, out accessLevel, out online);

                            this.AddLabelCropped(12, offset, 120, 20, LabelHue, a.Username);
                            this.AddLabelCropped(132, offset, 120, 20, LabelHue, FormatAccessLevel(accessLevel));

                            if (online)
                                this.AddLabelCropped(252, offset, 120, 20, GreenHue, "Online");
                            else if (a.Banned)
                                this.AddLabelCropped(252, offset, 120, 20, RedHue, "Banned");
                            else
                                this.AddLabelCropped(252, offset, 120, 20, RedHue, "Offline");

                            this.AddButton(380, offset - 1, 0xFA5, 0xFA7, this.GetButtonID(5, index + 56), GumpButtonType.Reply, 0);
                        }

                        break;
                    }
            }
        }

        public void AddTextField(int x, int y, int width, int height, int index)
        {
            this.AddBackground(x - 2, y - 2, width + 4, height + 4, 0x2486);
            this.AddTextEntry(x + 2, y + 2, width - 4, height - 4, 0, index, "");
        }

        public void AddClientHeader()
        {
            this.AddTextField(200, 20, 200, 20, 0);
            this.AddButtonLabeled(200, 50, this.GetButtonID(4, 0), "Search For Name");
            this.AddButtonLabeled(200, 80, this.GetButtonID(4, 1), "Search For IP Address");
        }

        public void AddAccountHeader()
        {
            this.AddPage(1);

            this.AddLabel(200, 20, LabelHue, "Name:");
            this.AddTextField(250, 20, 150, 20, 0);

            this.AddLabel(200, 50, LabelHue, "Pass:");
            this.AddTextField(250, 50, 150, 20, 1);

            this.AddButtonLabeled(200, 80, this.GetButtonID(5, 6), "Add");
            this.AddButtonLabeled(290, 80, this.GetButtonID(5, 7), "Search");

            this.AddButton(384, 84, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 2);

            this.AddPage(2);

            this.AddButtonLabeled(200, 10, this.GetButtonID(5, 31), "View All: Inactive");
            this.AddButtonLabeled(200, 30, this.GetButtonID(5, 32), "View All: Banned");
            this.AddButtonLabeled(200, 50, this.GetButtonID(5, 26), "View All: Shared");
            this.AddButtonLabeled(200, 70, this.GetButtonID(5, 33), "View All: Empty");
            this.AddButtonLabeled(200, 90, this.GetButtonID(5, 30), "View All: TotalGameTime");

            this.AddButton(384, 84, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);

            this.AddPage(0);
        }

        public void AddFirewallHeader()
        {
            this.AddTextField(200, 20, 200, 20, 0);
            this.AddButtonLabeled(320, 50, this.GetButtonID(6, 0), "Search");
            this.AddButtonLabeled(200, 50, this.GetButtonID(6, 1), "Add (Input)");
            this.AddButtonLabeled(200, 80, this.GetButtonID(6, 2), "Add (Target)");
        }

        private static ArrayList GetAllSharedAccounts()
        {
            Hashtable table = new Hashtable();
            ArrayList list;

            foreach (Account acct in Accounts.GetAccounts())
            {
                IPAddress[] theirAddresses = acct.LoginIPs;

                for (int i = 0; i < theirAddresses.Length; ++i)
                {
                    list = (ArrayList)table[theirAddresses[i]];

                    if (list == null)
                        table[theirAddresses[i]] = list = new ArrayList();

                    list.Add(acct);
                }
            }

            list = new ArrayList(table);

            for (int i = 0; i < list.Count; ++i)
            {
                DictionaryEntry de = (DictionaryEntry)list[i];
                ArrayList accts = (ArrayList)de.Value;

                if (accts.Count == 1)
                    list.RemoveAt(i--);
                else
                    accts.Sort(AccountComparer.Instance);
            }

            list.Sort(SharedAccountComparer.Instance);

            return list;
        }

        private class SharedAccountComparer : IComparer
        {
            public static readonly IComparer Instance = new SharedAccountComparer();

            public SharedAccountComparer()
            {
            }

            public int Compare(object x, object y)
            {
                DictionaryEntry a = (DictionaryEntry)x;
                DictionaryEntry b = (DictionaryEntry)y;

                ArrayList aList = (ArrayList)a.Value;
                ArrayList bList = (ArrayList)b.Value;

                return bList.Count - aList.Count;
            }
        }

        private static ArrayList GetSharedAccounts(IPAddress ipAddress)
        {
            ArrayList list = new ArrayList();

            foreach (Account acct in Accounts.GetAccounts())
            {
                IPAddress[] theirAddresses = acct.LoginIPs;
                bool contains = false;

                for (int i = 0; !contains && i < theirAddresses.Length; ++i)
                    contains = ipAddress.Equals(theirAddresses[i]);

                if (contains)
                    list.Add(acct);
            }

            list.Sort(AccountComparer.Instance);
            return list;
        }

        private static ArrayList GetSharedAccounts(IPAddress[] ipAddresses)
        {
            ArrayList list = new ArrayList();

            foreach (Account acct in Accounts.GetAccounts())
            {
                IPAddress[] theirAddresses = acct.LoginIPs;
                bool contains = false;

                for (int i = 0; !contains && i < theirAddresses.Length; ++i)
                {
                    IPAddress check = theirAddresses[i];

                    for (int j = 0; !contains && j < ipAddresses.Length; ++j)
                        contains = check.Equals(ipAddresses[j]);
                }

                if (contains)
                    list.Add(acct);
            }

            list.Sort(AccountComparer.Instance);
            return list;
        }

        public static void BanShared_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            string notice;
            ArrayList list = null;

            if (okay)
            {
                Account a = (Account)state;
                list = GetSharedAccounts(a.LoginIPs);

                for (int i = 0; i < list.Count; ++i)
                {
                    ((Account)list[i]).SetUnspecifiedBan(from);
                    ((Account)list[i]).Banned = true;
                }

                notice = "All addresses in the list have been banned.";
            }
            else
            {
                notice = "You have chosen not to ban all shared accounts.";
            }

            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, notice, state));

            if (okay)
                from.SendGump(new BanDurationGump(list));
        }

        public static void AccountDelete_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            if (okay)
            {
                Account a = (Account)state;

                CommandLogging.WriteLine(from, "{0} {1} deleting account {2}", from.AccessLevel, CommandLogging.Format(from), a.Username);
                a.Delete();

                from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, null, String.Format("{0} : The account has been deleted.", a.Username), null));
            }
            else
            {
                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "You have chosen not to delete the account.", state));
            }
        }

        public static void ResendGump_Callback(Mobile from, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            object[] states = (object[])state;
            ArrayList list = (ArrayList)states[0];
            ArrayList rads = (ArrayList)states[1];
            int page = (int)states[2];

            from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, page, list, null, rads));
        }

        public static void Marked_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            object[] states = (object[])state;
            bool ban = (bool)states[0];
            ArrayList list = (ArrayList)states[1];
            ArrayList rads = (ArrayList)states[2];
            int page = (int)states[3];

            if (okay)
            {
                if (!ban)
                    NetState.Pause();

                for (int i = 0; i < rads.Count; ++i)
                {
                    Account acct = (Account)rads[i];

                    if (ban)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} banning account {2}", from.AccessLevel, CommandLogging.Format(from), acct.Username);
                        acct.SetUnspecifiedBan(from);
                        acct.Banned = true;
                    }
                    else
                    {
                        CommandLogging.WriteLine(from, "{0} {1} deleting account {2}", from.AccessLevel, CommandLogging.Format(from), acct.Username);
                        acct.Delete();
                        rads.RemoveAt(i--);
                        list.Remove(acct);
                    }
                }

                if (!ban)
                    NetState.Resume();

                from.SendGump(new NoticeGump(1060637, 30720, String.Format("You have {0} the account{1}.", ban ? "banned" : "deleted", rads.Count == 1 ? "" : "s"), 0xFFC000, 420, 280, new NoticeGumpCallback(ResendGump_Callback), new object[] { list, rads, ban ? page : 0 }));

                if (ban)
                    from.SendGump(new BanDurationGump(rads));
            }
            else
            {
                from.SendGump(new NoticeGump(1060637, 30720, String.Format("You have chosen not to {0} the account{1}.", ban ? "ban" : "delete", rads.Count == 1 ? "" : "s"), 0xFFC000, 420, 280, new NoticeGumpCallback(ResendGump_Callback), new object[] { list, rads, page }));
            }
        }

        public static void FirewallShared_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            string notice;

            if (okay)
            {
                Account a = (Account)state;

                for (int i = 0; i < a.LoginIPs.Length; ++i)
                    Firewall.Add(a.LoginIPs[i]);

                notice = "All addresses in the list have been firewalled.";
            }
            else
            {
                notice = "You have chosen not to firewall all addresses.";
            }

            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, notice, state));
        }

        public static void Firewall_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            object[] states = (object[])state;

            Account a = (Account)states[0];
            object toFirewall = states[1];

            string notice;

            if (okay)
            {
                Firewall.Add(toFirewall);

                notice = String.Format("{0} : Added to firewall.", toFirewall);
            }
            else
            {
                notice = "You have chosen not to firewall the address.";
            }

            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, notice, a));
        }

        public static void RemoveLoginIP_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            object[] states = (object[])state;

            Account a = (Account)states[0];
            IPAddress ip = (IPAddress)states[1];

            string notice;

            if (okay)
            {
                IPAddress[] ips = a.LoginIPs;

                if (ips.Length != 0 && ip == ips[0] && AccountHandler.IPTable.ContainsKey(ips[0]))
                    --AccountHandler.IPTable[ip];

                List<IPAddress> newList = new List<IPAddress>(ips);
                newList.Remove(ip);
                a.LoginIPs = newList.ToArray();

                notice = String.Format("{0} : Removed address.", ip);
            }
            else
            {
                notice = "You have chosen not to remove the address.";
            }

            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, notice, a));
        }

        public static void RemoveLoginIPs_Callback(Mobile from, bool okay, object state)
        {
            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            Account a = (Account)state;

            string notice;

            if (okay)
            {
                IPAddress[] ips = a.LoginIPs;

                if (ips.Length != 0 && AccountHandler.IPTable.ContainsKey(ips[0]))
                    --AccountHandler.IPTable[ips[0]];

                a.LoginIPs = new IPAddress[0];

                notice = "All addresses in the list have been removed.";
            }
            else
            {
                notice = "You have chosen not to clear all addresses.";
            }

            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, notice, a));
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            int val = info.ButtonID - 1;

            if (val < 0)
                return;

            Mobile from = this.m_From;

            if (from.AccessLevel < AccessLevel.Administrator)
                return;

            if (this.m_PageType == AdminGumpPage.Accounts)
            {
                ArrayList list = this.m_List;
                ArrayList rads = this.m_State as ArrayList;

                if (list != null && rads != null)
                {
                    for (int i = 0, v = this.m_ListPage * 12; i < 12 && v < list.Count; ++i, ++v)
                    {
                        object obj = list[v];

                        if (info.IsSwitched(v))
                        {
                            if (!rads.Contains(obj))
                                rads.Add(obj);
                        }
                        else if (rads.Contains(obj))
                        {
                            rads.Remove(obj);
                        }
                    }
                }
            }

            int type = val % 11;
            int index = val / 11;

            switch ( type )
            {
                case 0:
                    {
                        AdminGumpPage page;

                        switch ( index )
                        {
                            case 0:
                                page = AdminGumpPage.Information_General;
                                break;
                            case 1:
                                page = AdminGumpPage.Administer;
                                break;
                            case 2:
                                page = AdminGumpPage.Clients;
                                break;
                            case 3:
                                page = AdminGumpPage.Accounts;
                                break;
                            case 4:
                                page = AdminGumpPage.Firewall;
                                break;
                            case 5:
                                page = AdminGumpPage.Information_Perf;
                                break;
                            default:
                                return;
                        }

                        from.SendGump(new AdminGump(from, page, 0, null, null, null));
                        break;
                    }
                case 1:
                    {
                        switch ( index )
                        {
                            case 0:
                                {
                                    if (this.m_List != null && this.m_ListPage > 0)
                                        from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage - 1, this.m_List, null, this.m_State));

                                    break;
                                }
                            case 1:
                                {
                                    if (this.m_List != null /*&& (m_ListPage + 1) * 12 < m_List.Count*/)
                                        from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage + 1, this.m_List, null, this.m_State));

                                    break;
                                }
                        }

                        break;
                    }
                case 3:
                    {
                        string notice = null;
                        AdminGumpPage page = AdminGumpPage.Administer;

			if (index >= 600)
			    page = AdminGumpPage.Administer_Maintenance;
			else if (index >= 500)
                            page = AdminGumpPage.Administer_Access_Lockdown;
                        else if (index >= 400)
                            page = AdminGumpPage.Administer_Commands;
                        else if (index >= 300)
                            page = AdminGumpPage.Administer_Access;
                        else if (index >= 200)
                            page = AdminGumpPage.Administer_Server;
                        else if (index >= 100)
                            page = AdminGumpPage.Administer_WorldBuilding;

                        switch ( index )
                        {
                            case 0:
                                page = AdminGumpPage.Administer_WorldBuilding;
                                break;
                            case 1:
                                page = AdminGumpPage.Administer_Server;
                                break;
                            case 2:
                                page = AdminGumpPage.Administer_Access;
                                break;
                            case 3:
                                page = AdminGumpPage.Administer_Commands;
                                break;
			    case 4:
				page = AdminGumpPage.Administer_Maintenance;
				break;
                            case 101:
                                this.InvokeCommand("CreateWorld nogump");
                                notice = "The world has been created.";
                                break;
                            case 102:
                                this.InvokeCommand("DeleteWorld nogump");
                                notice = "The world has been deleted.";
                                break;
                            case 103:
                                this.InvokeCommand("RecreateWorld nogump");
                                notice = "The world has been recreated.";
                                break;
                            case 110:
                                this.InvokeCommand("Freeze");
                                notice = "Target bounding points.";
                                break;
                            case 120:
                                this.InvokeCommand("Unfreeze");
                                notice = "Target bounding points.";
                                break;
                            case 200:
                                this.InvokeCommand("Save");
                                notice = "The world has been saved.";
                                break;
                            case 201:
                                this.Shutdown(false, true);
                                break;
                            case 202:
                                this.Shutdown(false, false);
                                break;
                            case 203:
                                this.Shutdown(true, true);
                                break;
                            case 204:
                                this.Shutdown(true, false);
                                break;
                            case 210:
                            case 211:
                                {
                                    TextRelay relay = info.GetTextEntry(0);
                                    string text = (relay == null ? null : relay.Text.Trim());

                                    if (text == null || text.Length == 0)
                                    {
                                        notice = "You must enter text to broadcast it.";
                                    }
                                    else
                                    {
                                        notice = "Your message has been broadcasted.";
                                        this.InvokeCommand(String.Format("{0} {1}", index == 210 ? "BC" : "SM", text));
                                    }

                                    break;
                                }

                            case 300:
                                this.InvokeCommand("Kick");
                                notice = "Target the player to kick.";
                                break;
                            case 301:
                                this.InvokeCommand("Ban");
                                notice = "Target the player to ban.";
                                break;
                            case 302:
                                this.InvokeCommand("Firewall");
                                notice = "Target the player to firewall.";
                                break;
                            case 303:
                                page = AdminGumpPage.Administer_Access_Lockdown;
                                break;
                            case 310:
                                this.InvokeCommand("Set AccessLevel Player");
                                notice = "Target the player to change their access level. (Player)";
                                break;
                            case 311:
                                this.InvokeCommand("Set AccessLevel Counselor");
                                notice = "Target the player to change their access level. (Counselor)";
                                break;
                            case 312:
                                this.InvokeCommand("Set AccessLevel GameMaster");
                                notice = "Target the player to change their access level. (Game Master)";
                                break;
                            case 313:
                                this.InvokeCommand("Set AccessLevel Seer");
                                notice = "Target the player to change their access level. (Seer)";
                                break;
                            case 314:
                                {
                                    if (from.AccessLevel > AccessLevel.Administrator)
                                    {
                                        this.InvokeCommand("Set AccessLevel Administrator");
                                        notice = "Target the player to change their access level. (Administrator)";
                                    }

                                    break;
                                }

                            case 315:
                                {
                                    if (from.AccessLevel > AccessLevel.Developer)
                                    {
                                        this.InvokeCommand("Set AccessLevel Developer");
                                        notice = "Target the player to change their access level. (Developer)";
                                    }

                                    break;
                                }

                            case 316:
                                {
                                    if (from.AccessLevel >= AccessLevel.Owner)
                                    {
                                        this.InvokeCommand("Set AccessLevel Owner");
                                        notice = "Target the player to change their access level. (Owner)";
                                    }

                                    break;
                                }

                            case 400:
                                notice = "Enter search terms to add objects.";
                                break;
                            case 401:
                                this.InvokeCommand("Remove");
                                notice = "Target the item or mobile to remove.";
                                break;
                            case 402:
                                this.InvokeCommand("Dupe");
                                notice = "Target the item to dupe.";
                                break;
                            case 403:
                                this.InvokeCommand("DupeInBag");
                                notice = "Target the item to dupe. The item will be duped at it's current location.";
                                break;
                            case 404:
                                this.InvokeCommand("Props");
                                notice = "Target the item or mobile to inspect.";
                                break;
                            case 405:
                                this.InvokeCommand("Skills");
                                notice = "Target a mobile to view their skills.";
                                break;
                            case 406:
                                this.InvokeCommand("Set Blessed False");
                                notice = "Target the mobile to make mortal.";
                                break;
                            case 407:
                                this.InvokeCommand("Set Blessed True");
                                notice = "Target the mobile to make immortal.";
                                break;
                            case 408:
                                this.InvokeCommand("Set Squelched True");
                                notice = "Target the mobile to squelch.";
                                break;
                            case 409:
                                this.InvokeCommand("Set Squelched False");
                                notice = "Target the mobile to unsquelch.";
                                break;
                            case 410:
                                this.InvokeCommand("Set Frozen True");
                                notice = "Target the mobile to freeze.";
                                break;
                            case 411:
                                this.InvokeCommand("Set Frozen False");
                                notice = "Target the mobile to unfreeze.";
                                break;
                            case 412:
                                this.InvokeCommand("Set Hidden True");
                                notice = "Target the mobile to hide.";
                                break;
                            case 413:
                                this.InvokeCommand("Set Hidden False");
                                notice = "Target the mobile to unhide.";
                                break;
                            case 414:
                                this.InvokeCommand("Kill");
                                notice = "Target the mobile to kill.";
                                break;
                            case 415:
                                this.InvokeCommand("Resurrect");
                                notice = "Target the mobile to resurrect.";
                                break;
                            case 416:
                                this.InvokeCommand("Move");
                                notice = "Target the item or mobile to move.";
                                break;
                            case 417:
                                this.InvokeCommand("Wipe");
                                notice = "Target bounding points.";
                                break;
                            case 418:
                                this.InvokeCommand("Tele");
                                notice = "Choose your destination.";
                                break;
                            case 419:
                                this.InvokeCommand("Multi Tele");
                                notice = "Choose your destination.";
                                break;
                            case 500:
                            case 501:
                            case 502:
                            case 503:
                            case 504:
                                {
                                    Misc.AccountHandler.LockdownLevel = (AccessLevel)(index - 500);

                                    if (Misc.AccountHandler.LockdownLevel > AccessLevel.VIP)
                                        notice = "The lockdown level has been changed.";
                                    else
                                        notice = "The server is now accessible to everyone.";

                                    break;
                                }

                            case 510:
                                {
                                    AccessLevel level = Misc.AccountHandler.LockdownLevel;

                                    if (level > AccessLevel.VIP)
                                    {
                                        List<NetState> clients = NetState.Instances;
                                        int count = 0;

                                        for (int i = 0; i < clients.Count; ++i)
                                        {
                                            NetState ns = clients[i];
                                            IAccount a = ns.Account;

                                            if (a == null)
                                                continue;

                                            bool hasAccess = false;

                                            if (a.AccessLevel >= level)
                                            {
                                                hasAccess = true;
                                            }
                                            else
                                            {
                                                for (int j = 0; !hasAccess && j < a.Length; ++j)
                                                {
                                                    Mobile m = a[j];

                                                    if (m != null && m.AccessLevel >= level)
                                                        hasAccess = true;
                                                }
                                            }

                                            if (!hasAccess)
                                            {
                                                ns.Dispose();
                                                ++count;
                                            }
                                        }

                                        if (count == 0)
                                            notice = "Nobody without access was found to disconnect.";
                                        else
                                            notice = String.Format("Number of players disconnected: {0}", count);
                                    }
                                    else
                                    {
                                        notice = "The server is not currently locked down.";
                                    }

                                    break;
				}
			    case 600:
                                this.InvokeCommand("RebuildCategorization");
                                notice = "Categorization menu has been regenerated. The server should be restarted.";
                                break;
                            case 601:
                                this.InvokeCommand("DocGen");
                                notice = "Documentation has been generated.";
                                break;
			    case 602:
                                this.InvokeCommand("GenBounds");
                                notice = "Bounds.bin rebuild. Restart server to take effect.";
                                break;
			    case 603:
                                this.InvokeCommand("GenReports");
                                notice = "Reports generated.";
                                break;
		            case 604:
                                this.InvokeCommand("DumpTimers");
                                notice = "Timers dumped.";
                                break;
			    case 605:
                                this.InvokeCommand("CountObjects");
                                notice = "Objects counted.";
                                break;
			    case 606:
                                this.InvokeCommand("ProfileWorld");
                                notice = "World profiled.";
                                break;
			    case 607:
                                this.InvokeCommand("WriteProfiles");
                                notice = "Profiles written.";
                                break;
			    case 608:
                                this.InvokeCommand("TraceInternal");
                                notice = "Tracing completed.";
                                break;
			    case 609:
                                this.InvokeCommand("TraceExpanded");
                                notice = "Tracing completed.";
                                break;
			    case 610:
                                this.InvokeCommand("SetProfiles");
                                notice = "Profiles toggled. Use with caution. This increases server load.";
                                break;	
                        }

                        from.SendGump(new AdminGump(from, page, 0, null, notice, null));

                        switch ( index )
                        {
                            case 400:
                                this.InvokeCommand("Add");
                                break;
                            case 111:
                                this.InvokeCommand("FreezeWorld");
                                break;
                            case 112:
                                this.InvokeCommand("FreezeMap");
                                break;
                            case 121:
                                this.InvokeCommand("UnfreezeWorld");
                                break;
                            case 122:
                                this.InvokeCommand("UnfreezeMap");
                                break;
                        }

                        break;
                    }
                case 4:
                    {
                        switch ( index )
                        {
                            case 0:
                            case 1:
                                {
                                    bool forName = (index == 0);

                                    ArrayList results = new ArrayList();

                                    TextRelay matchEntry = info.GetTextEntry(0);
                                    string match = (matchEntry == null ? null : matchEntry.Text.Trim().ToLower());
                                    string notice = null;

                                    if (match == null || match.Length == 0)
                                    {
                                        notice = String.Format("You must enter {0} to search.", forName ? "a name" : "an ip address");
                                    }
                                    else
                                    {
                                        List<NetState> instances = NetState.Instances;

                                        for (int i = 0; i < instances.Count; ++i)
                                        {
                                            NetState ns = instances[i];

                                            bool isMatch;

                                            if (forName)
                                            {
                                                Mobile m = ns.Mobile;
                                                IAccount a = ns.Account;

                                                isMatch = (m != null && m.Name.ToLower().IndexOf(match) >= 0) ||
                                                          (a != null && a.Username.ToLower().IndexOf(match) >= 0);
                                            }
                                            else
                                            {
                                                isMatch = (ns.ToString().IndexOf(match) >= 0);
                                            }

                                            if (isMatch)
                                                results.Add(ns);
                                        }

                                        results.Sort(NetStateComparer.Instance);
                                    }

                                    if (results.Count == 1)
                                    {
                                        NetState ns = (NetState)results[0];
                                        object state = ns.Mobile;

                                        if (state == null)
                                            state = ns.Account;

                                        if (state is Mobile)
                                            from.SendGump(new AdminGump(from, AdminGumpPage.ClientInfo, 0, null, "One match found.", state));
                                        else if (state is Account)
                                            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", state));
                                        else
                                            from.SendGump(new AdminGump(from, AdminGumpPage.Clients, 0, results, "One match found.", null));
                                    }
                                    else
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Clients, 0, results, notice == null ? (results.Count == 0 ? "Nothing matched your search terms." : null) : notice, null));
                                    }

                                    break;
                                }
                            default:
                                {
                                    index -= 2;

                                    if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                                    {
                                        NetState ns = this.m_List[index] as NetState;

                                        if (ns == null)
                                            break;

                                        Mobile m = ns.Mobile;
                                        Account a = ns.Account as Account;

                                        if (m != null)
                                            from.SendGump(new AdminGump(from, AdminGumpPage.ClientInfo, 0, null, null, m));
                                        else if (a != null)
                                            from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, null, a));
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case 5:
                    {
                        switch ( index )
                        {
                            case 0:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, null, this.m_State));
                                break;
                            case 1:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Characters, 0, null, null, this.m_State));
                                break;
                            case 2:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Comments, 0, null, null, this.m_State));
                                break;
                            case 3:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Tags, 0, null, null, this.m_State));
                                break;
                            case 13:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access, 0, null, null, this.m_State));
                                break;
                            case 14:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, null, this.m_State));
                                break;
                            case 15:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_Restrictions, 0, null, null, this.m_State));
                                break;
                            case 4:
                                from.Prompt = new AddCommentPrompt(this.m_State as Account);
                                from.SendMessage("Enter the new account comment.");
                                break;
                            case 5:
                                from.Prompt = new AddTagNamePrompt(this.m_State as Account);
                                from.SendMessage("Enter the new tag name.");
                                break;
                            case 6:
                                {
                                    TextRelay unEntry = info.GetTextEntry(0);
                                    TextRelay pwEntry = info.GetTextEntry(1);

                                    string un = (unEntry == null ? null : unEntry.Text.Trim());
                                    string pw = (pwEntry == null ? null : pwEntry.Text.Trim());

                                    Account dispAccount = null;
                                    string notice;

                                    if (un == null || un.Length == 0)
                                    {
                                        notice = "You must enter a username to add an account.";
                                    }
                                    else if (pw == null || pw.Length == 0)
                                    {
                                        notice = "You must enter a password to add an account.";
                                    }
                                    else
                                    {
                                        IAccount account = Accounts.GetAccount(un);

                                        if (account != null)
                                        {
                                            notice = "There is already an account with that username.";
                                        }
                                        else
                                        {
                                            dispAccount = new Account(un, pw);
                                            notice = String.Format("{0} : Account added.", un);
                                            CommandLogging.WriteLine(from, "{0} {1} adding new account: {2}", from.AccessLevel, CommandLogging.Format(from), un);
                                        }
                                    }

                                    from.SendGump(new AdminGump(from, dispAccount != null ? AdminGumpPage.AccountDetails_Information : this.m_PageType, this.m_ListPage, this.m_List, notice, dispAccount != null ? dispAccount : this.m_State));
                                    break;
                                }
                            case 7:
                                {
                                    ArrayList results;

                                    TextRelay matchEntry = info.GetTextEntry(0);
                                    string match = (matchEntry == null ? null : matchEntry.Text.Trim().ToLower());
                                    string notice = null;

                                    if (match == null || match.Length == 0)
                                    {
                                        results = new ArrayList((ICollection)Accounts.GetAccounts());
                                        results.Sort(AccountComparer.Instance);
                                        //notice = "You must enter a username to search.";
                                    }
                                    else
                                    {
                                        results = new ArrayList();
                                        foreach (Account check in Accounts.GetAccounts())
                                        {
                                            if (check.Username.ToLower().IndexOf(match) >= 0)
                                                results.Add(check);
                                        }

                                        results.Sort(AccountComparer.Instance);
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", results[0]));
                                    else
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, results, notice == null ? (results.Count == 0 ? "Nothing matched your search terms." : null) : notice, new ArrayList()));

                                    break;
                                }
                            case 8:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_ChangePassword, 0, null, null, this.m_State));
                                break;
                            case 9:
                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_ChangeAccess, 0, null, null, this.m_State));
                                break;
                            case 10:
                            case 11:
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    a.SetUnspecifiedBan(from);
                                    a.Banned = (index == 10);
                                    CommandLogging.WriteLine(from, "{0} {1} {3} account {2}", from.AccessLevel, CommandLogging.Format(from), a.Username, a.Banned ? "banning" : "unbanning");
                                    from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage, this.m_List, String.Format("The account has been {0}.", a.Banned ? "banned" : "unbanned"), this.m_State));

                                    if (index == 10)
                                        from.SendGump(new BanDurationGump(a));

                                    break;
                                }
                            case 12:
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    TextRelay passwordEntry = info.GetTextEntry(0);
                                    TextRelay confirmEntry = info.GetTextEntry(1);

                                    string password = (passwordEntry == null ? null : passwordEntry.Text.Trim());
                                    string confirm = (confirmEntry == null ? null : confirmEntry.Text.Trim());

                                    string notice;
                                    AdminGumpPage page = AdminGumpPage.AccountDetails_ChangePassword;

                                    if (password == null || password.Length == 0)
                                    {
                                        notice = "You must enter the password.";
                                    }
                                    else if (confirm != password)
                                    {
                                        notice = "You must confirm the password. That field must precisely match the password field.";
                                    }
                                    else
                                    {
                                        notice = "The password has been changed.";
                                        a.SetPassword(password);
                                        page = AdminGumpPage.AccountDetails_Information;
                                        CommandLogging.WriteLine(from, "{0} {1} changing password of account {2}", from.AccessLevel, CommandLogging.Format(from), a.Username);
                                    }

                                    from.SendGump(new AdminGump(from, page, 0, null, notice, this.m_State));

                                    break;
                                }
                            case 16: // view shared
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    ArrayList list = GetSharedAccounts(a.LoginIPs);

                                    if (list.Count > 1 || (list.Count == 1 && !list.Contains(a)))
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, list, null, new ArrayList()));
                                    }
                                    else if (a.LoginIPs.Length > 0)
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "There are no other accounts which share an address with this one.", this.m_State));
                                    }
                                    else
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "This account has not yet been accessed.", this.m_State));
                                    }

                                    break;
                                }
                            case 17: // ban shared
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    ArrayList list = GetSharedAccounts(a.LoginIPs);

                                    if (list.Count > 0)
                                    {
                                        StringBuilder sb = new StringBuilder();

                                        sb.AppendFormat("You are about to ban {0} account{1}. Do you wish to continue?", list.Count, list.Count != 1 ? "s" : "");

                                        for (int i = 0; i < list.Count; ++i)
                                            sb.AppendFormat("<br>- {0}", ((Account)list[i]).Username);

                                        from.SendGump(new WarningGump(1060635, 30720, sb.ToString(), 0xFFC000, 420, 400, new WarningGumpCallback(BanShared_Callback), a));
                                    }
                                    else if (a.LoginIPs.Length > 0)
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "There are no accounts which share an address with this one.", this.m_State));
                                    }
                                    else
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "This account has not yet been accessed.", this.m_State));
                                    }

                                    break;
                                }
                            case 18: // firewall all
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    if (a.LoginIPs.Length > 0)
                                    {
                                        from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to firewall {0} address{1}. Do you wish to continue?", a.LoginIPs.Length, a.LoginIPs.Length != 1 ? "s" : ""), 0xFFC000, 420, 400, new WarningGumpCallback(FirewallShared_Callback), a));
                                    }
                                    else
                                    {
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "This account has not yet been accessed.", this.m_State));
                                    }

                                    break;
                                }
                            case 19: // add
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    TextRelay entry = info.GetTextEntry(0);
                                    string ip = (entry == null ? null : entry.Text.Trim());

                                    string notice;

                                    if (ip == null || ip.Length == 0)
                                    {
                                        notice = "You must enter an address to add.";
                                    }
                                    else
                                    {
                                        string[] list = a.IPRestrictions;

                                        bool contains = false;
                                        for (int i = 0; !contains && i < list.Length; ++i)
                                            contains = (list[i] == ip);

                                        if (contains)
                                        {
                                            notice = "That address is already contained in the list.";
                                        }
                                        else
                                        {
                                            string[] newList = new string[list.Length + 1];

                                            for (int i = 0; i < list.Length; ++i)
                                                newList[i] = list[i];

                                            newList[list.Length] = ip;

                                            a.IPRestrictions = newList;

                                            notice = String.Format("{0} : Added to restriction list.", ip);
                                        }
                                    }

                                    from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_Restrictions, 0, null, notice, this.m_State));

                                    break;
                                }
                            case 20: // Change access level
                            case 21:
                            case 22:
                            case 23:
                            case 24:
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    AccessLevel newLevel;

                                    switch ( index )
                                    {
                                        default:
                                        case 20:
                                            newLevel = AccessLevel.Player;
                                            break;
                                        case 21:
                                            newLevel = AccessLevel.Counselor;
                                            break;
                                        case 22:
                                            newLevel = AccessLevel.GameMaster;
                                            break;
                                        case 23:
                                            newLevel = AccessLevel.Seer;
                                            break;
                                        case 24:
                                            newLevel = AccessLevel.Administrator;
                                            break;
                                        case 33:
                                            newLevel = AccessLevel.Developer;
                                            break;
                                        case 34:
                                            newLevel = AccessLevel.Owner;
                                            break;
                                    }

                                    if (newLevel < from.AccessLevel || from.AccessLevel == AccessLevel.Owner)
                                    {
                                        a.AccessLevel = newLevel;

                                        CommandLogging.WriteLine(from, "{0} {1} changing access level of account {2} to {3}", from.AccessLevel, CommandLogging.Format(from), a.Username, a.AccessLevel);
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "The access level has been changed.", this.m_State));
                                    }

                                    break;
                                }
                            case 25:
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    from.SendGump(new WarningGump(1060635, 30720, String.Format("<center>Account of {0}</center><br>You are about to <em><basefont color=red>permanently delete</basefont></em> the account. Likewise, all characters on the account will be deleted, including equiped, inventory, and banked items. Any houses tied to the account will be demolished.<br><br>Do you wish to continue?", a.Username), 0xFFC000, 420, 280, new WarningGumpCallback(AccountDelete_Callback), this.m_State));
                                    break;
                                }
                            case 26: // View all shared accounts
                                {
                                    from.SendGump(new AdminGump(from, AdminGumpPage.Accounts_Shared, 0, null, null, null));
                                    break;
                                }
                            case 27: // Ban marked
                                {
                                    ArrayList list = this.m_List;
                                    ArrayList rads = this.m_State as ArrayList;

                                    if (list == null || rads == null)
                                        break;

                                    if (rads.Count > 0)
                                        from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to ban {0} marked account{1}. Be cautioned, the only way to reverse this is by hand--manually unbanning each account.<br><br>Do you wish to continue?", rads.Count, rads.Count == 1 ? "" : "s"), 0xFFC000, 420, 280, new WarningGumpCallback(Marked_Callback), new object[] { true, list, rads, this.m_ListPage }));
                                    else
                                        from.SendGump(new NoticeGump(1060637, 30720, "You have not yet marked any accounts. Place a check mark next to the accounts you wish to ban and then try again.", 0xFFC000, 420, 280, new NoticeGumpCallback(ResendGump_Callback), new object[] { list, rads, this.m_ListPage }));

                                    break;
                                }
                            case 28: // Delete marked
                                {
                                    ArrayList list = this.m_List;
                                    ArrayList rads = this.m_State as ArrayList;

                                    if (list == null || rads == null)
                                        break;

                                    if (rads.Count > 0)
                                        from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to <em><basefont color=red>permanently delete</basefont></em> {0} marked account{1}. Likewise, all characters on the account{1} will be deleted, including equiped, inventory, and banked items. Any houses tied to the account{1} will be demolished.<br><br>Do you wish to continue?", rads.Count, rads.Count == 1 ? "" : "s"), 0xFFC000, 420, 280, new WarningGumpCallback(Marked_Callback), new object[] { false, list, rads, this.m_ListPage }));
                                    else
                                        from.SendGump(new NoticeGump(1060637, 30720, "You have not yet marked any accounts. Place a check mark next to the accounts you wish to ban and then try again.", 0xFFC000, 420, 280, new NoticeGumpCallback(ResendGump_Callback), new object[] { list, rads, this.m_ListPage }));

                                    break;
                                }
                            case 29: // Mark all
                                {
                                    ArrayList list = this.m_List;
                                    ArrayList rads = this.m_State as ArrayList;

                                    if (list == null || rads == null)
                                        break;

                                    from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, this.m_ListPage, this.m_List, null, new ArrayList(list)));

                                    break;
                                }
                                #region case 30: 3 minute game time account check
                            case 30: // View all accounts less than 3 minutes of total online time.
                                {
                                    //Change the "3" in the following line, to adjust deletion time.
                                    TimeSpan unusedGracePeriod = TimeSpan.FromMinutes(3);

                                    ArrayList results = new ArrayList();

                                    foreach (Account acct in Accounts.GetAccounts())
                                    {
                                        TimeSpan time = acct.TotalGameTime; //TotalGameTime from Account file.

                                        if (time <= unusedGracePeriod)
                                        {
                                            Console.WriteLine("Remove: " + acct.Username + " TotalGameTime: " + time + " <= " + unusedGracePeriod + ":Grace Period");
                                            results.Add(acct);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Keep: " + acct.Username + " TotalGameTime: " + time + " <= " + unusedGracePeriod + ":Grace Period");
                                        }
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", results[0]));
                                    else
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, results, (results.Count == 0 ? "Nothing matched your search terms." : null), new ArrayList()));

                                    break;
                                }
                                #endregion
                            case 31: // View all inactive accounts
                                {
                                    ArrayList results = new ArrayList();

                                    foreach (Account acct in Accounts.GetAccounts())
                                    {
                                        if (acct.Inactive)
                                            results.Add(acct);
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", results[0]));
                                    else
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, results, (results.Count == 0 ? "Nothing matched your search terms." : null), new ArrayList()));

                                    break;
                                }
                            case 32: // View all banned accounts
                                {
                                    ArrayList results = new ArrayList();

                                    foreach (Account acct in Accounts.GetAccounts())
                                    {
                                        if (acct.Banned)
                                            results.Add(acct);
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", results[0]));
                                    else
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, results, (results.Count == 0 ? "Nothing matched your search terms." : null), new ArrayList()));

                                    break;
                                }
                                #region original case 30
                            case 33: // View all empty accounts
                                {
                                    ArrayList results = new ArrayList();

                                    foreach (Account acct in Accounts.GetAccounts())
                                    {
                                        bool empty = true;

                                        for (int i = 0; empty && i < acct.Length; ++i)
                                            empty = (acct[i] == null);

                                        if (empty)
                                            results.Add(acct);
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, "One match found.", results[0]));
                                    else
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, results, (results.Count == 0 ? "Nothing matched your search terms." : null), new ArrayList()));

                                    break;
                                }
                                #endregion
                            case 34:
                                {
                                    goto case 20;
                                }
                            case 35: // Unmark house owners
                                {
                                    ArrayList list = this.m_List;
                                    ArrayList rads = this.m_State as ArrayList;

                                    if (list == null || rads == null)
                                        break;

                                    ArrayList newRads = new ArrayList();

                                    foreach (Account acct in rads)
                                    {
                                        bool hasHouse = false;

                                        for (int i = 0; i < acct.Length && !hasHouse; ++i)
                                            if (acct[i] != null && BaseHouse.HasHouse(acct[i]))
                                                hasHouse = true;

                                        if (!hasHouse)
                                            newRads.Add(acct);
                                    }

                                    from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, this.m_ListPage, this.m_List, null, newRads));

                                    break;
                                }
                            case 36: // Clear login addresses
                                {
                                    Account a = this.m_State as Account;

                                    if (a == null)
                                        break;

                                    IPAddress[] ips = a.LoginIPs;

                                    if (ips.Length == 0)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "This account has not yet been accessed.", this.m_State));
                                    else
                                        from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to clear the address list for account {0} containing {1} {2}. Do you wish to continue?", a, ips.Length, (ips.Length == 1) ? "entry" : "entries"), 0xFFC000, 420, 280, new WarningGumpCallback(RemoveLoginIPs_Callback), a));

                                    break;
                                }
                            default:
                                {
                                    index -= 50;

                                    Account a = this.m_State as Account;

                                    if (a != null && index >= 0 && index < a.Length)
                                    {
                                        Mobile m = a[index];

                                        if (m != null)
                                            from.SendGump(new AdminGump(from, AdminGumpPage.ClientInfo, 0, null, null, m));
                                    }
                                    else
                                    {
                                        index -= 6;

                                        if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                                        {
                                            if (this.m_List[index] is Account)
                                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, null, this.m_List[index]));
                                            else if (this.m_List[index] is DictionaryEntry)
                                                from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, (ArrayList)(((DictionaryEntry)this.m_List[index]).Value), null, new ArrayList()));
                                        }
                                    }

                                    break;
                                }
                        }

                        break;
                    }
                case 6:
                    {
                        switch ( index )
                        {
                            case 0:
                                {
                                    TextRelay matchEntry = info.GetTextEntry(0);
                                    string match = (matchEntry == null ? null : matchEntry.Text.Trim());

                                    string notice = null;
                                    ArrayList results = new ArrayList();

                                    if (match == null || match.Length == 0)
                                    {
                                        notice = "You must enter a username to search.";
                                    }
                                    else
                                    {
                                        for (int i = 0; i < Firewall.List.Count; ++i)
                                        {
                                            string check = Firewall.List[i].ToString();

                                            if (check.IndexOf(match) >= 0)
                                                results.Add(Firewall.List[i]);
                                        }
                                    }

                                    if (results.Count == 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.FirewallInfo, 0, null, "One match found.", results[0]));
                                    else if (results.Count > 1)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Firewall, 0, results, String.Format("Search results for : {0}", match), this.m_State));
                                    else
                                        from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage, this.m_List, notice == null ? "Nothing matched your search terms." : notice, this.m_State));

                                    break;
                                }
                            case 1:
                                {
                                    TextRelay relay = info.GetTextEntry(0);
                                    string text = (relay == null ? null : relay.Text.Trim());

                                    if (text == null || text.Length == 0)
                                    {
                                        from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage, this.m_List, "You must enter an address or pattern to add.", this.m_State));
                                    }
                                    else if (!Utility.IsValidIP(text))
                                    {
                                        from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage, this.m_List, "That is not a valid address or pattern.", this.m_State));
                                    }
                                    else
                                    {
                                        object toAdd = Firewall.ToFirewallEntry(text);

                                        CommandLogging.WriteLine(from, "{0} {1} firewalling {2}", from.AccessLevel, CommandLogging.Format(from), toAdd);

                                        Firewall.Add(toAdd);
                                        from.SendGump(new AdminGump(from, AdminGumpPage.FirewallInfo, 0, null, String.Format("{0} : Added to firewall.", toAdd), toAdd));
                                    }

                                    break;
                                }
                            case 2:
                                {
                                    this.InvokeCommand("Firewall");
                                    from.SendGump(new AdminGump(from, this.m_PageType, this.m_ListPage, this.m_List, "Target the player to firewall.", this.m_State));
                                    break;
                                }
                            case 3:
                                {
                                    if (this.m_State is Firewall.IFirewallEntry)
                                    {
                                        CommandLogging.WriteLine(from, "{0} {1} removing {2} from firewall list", from.AccessLevel, CommandLogging.Format(from), this.m_State);

                                        Firewall.Remove(this.m_State);
                                        from.SendGump(new AdminGump(from, AdminGumpPage.Firewall, 0, null, String.Format("{0} : Removed from firewall.", this.m_State), null));
                                    }

                                    break;
                                }
                            default:
                                {
                                    index -= 4;

                                    if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                                        from.SendGump(new AdminGump(from, AdminGumpPage.FirewallInfo, 0, null, null, this.m_List[index]));

                                    break;
                                }
                        }

                        break;
                    }
                case 7:
                    {
                        Mobile m = this.m_State as Mobile;

                        if (m == null)
                            break;

                        string notice = null;
                        bool sendGump = true;

                        switch ( index )
                        {
                            case 0:
                                {
                                    Map map = m.Map;
                                    Point3D loc = m.Location;

                                    if (map == null || map == Map.Internal)
                                    {
                                        map = m.LogoutMap;
                                        loc = m.LogoutLocation;
                                    }

                                    if (map != null && map != Map.Internal)
                                    {
                                        from.MoveToWorld(loc, map);
                                        notice = "You have been teleported to their location.";
                                    }

                                    break;
                                }
                            case 1:
                                {
                                    m.MoveToWorld(from.Location, from.Map);
                                    notice = "They have been teleported to your location.";
                                    break;
                                }
                            case 2:
                                {
                                    NetState ns = m.NetState;

                                    if (ns != null)
                                    {
                                        CommandLogging.WriteLine(from, "{0} {1} {2} {3}", from.AccessLevel, CommandLogging.Format(from), "kicking", CommandLogging.Format(m));
                                        ns.Dispose();
                                        notice = "They have been kicked.";
                                    }
                                    else
                                    {
                                        notice = "They are already disconnected.";
                                    }

                                    break;
                                }
                            case 3:
                                {
                                    Account a = m.Account as Account;

                                    if (a != null)
                                    {
                                        CommandLogging.WriteLine(from, "{0} {1} {2} {3}", from.AccessLevel, CommandLogging.Format(from), "banning", CommandLogging.Format(m));
                                        a.Banned = true;

                                        NetState ns = m.NetState;

                                        if (ns != null)
                                            ns.Dispose();

                                        notice = "They have been banned.";
                                    }

                                    break;
                                }
                            case 6:
                                {
                                    Properties.SetValue(from, m, "Blessed", "False");
                                    notice = "They are now mortal.";
                                    break;
                                }
                            case 7:
                                {
                                    Properties.SetValue(from, m, "Blessed", "True");
                                    notice = "They are now immortal.";
                                    break;
                                }
                            case 8:
                                {
                                    Properties.SetValue(from, m, "Squelched", "True");
                                    notice = "They are now squelched.";
                                    break;
                                }
                            case 9:
                                {
                                    Properties.SetValue(from, m, "Squelched", "False");
                                    notice = "They are now unsquelched.";
                                    break;
                                }
                            case 10:
                                {
                                    Properties.SetValue(from, m, "Hidden", "True");
                                    notice = "They are now hidden.";
                                    break;
                                }
                            case 11:
                                {
                                    Properties.SetValue(from, m, "Hidden", "False");
                                    notice = "They are now unhidden.";
                                    break;
                                }
                            case 12:
                                {
                                    CommandLogging.WriteLine(from, "{0} {1} killing {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(m));
                                    m.Kill();
                                    notice = "They have been killed.";
                                    break;
                                }
                            case 13:
                                {
                                    CommandLogging.WriteLine(from, "{0} {1} resurrecting {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(m));
                                    m.Resurrect();
                                    notice = "They have been resurrected.";
                                    break;
                                }
                            case 14:
                                {
                                    from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Information, 0, null, null, m.Account));
                                    sendGump = false;
                                    break;
                                }
                        }

                        if (sendGump)
                            from.SendGump(new AdminGump(from, AdminGumpPage.ClientInfo, 0, null, notice, this.m_State));

                        switch ( index )
                        {
                            case 3:
                                {
                                    Account a = m.Account as Account;

                                    if (a != null)
                                        from.SendGump(new BanDurationGump(a));

                                    break;
                                }
                            case 4:
                                {
                                    from.SendGump(new PropertiesGump(from, m));
                                    break;
                                }
                            case 5:
                                {
                                    from.SendGump(new SkillsGump(from, m));
                                    break;
                                }
                        }

                        break;
                    }
                case 8:
                    {
                        if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                        {
                            Account a = this.m_State as Account;

                            if (a == null)
                                break;

                            if (this.m_PageType == AdminGumpPage.AccountDetails_Access_ClientIPs)
                            {
                                from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to firewall {0}. All connection attempts from a matching IP will be refused. Are you sure?", this.m_List[index]), 0xFFC000, 420, 280, new WarningGumpCallback(Firewall_Callback), new object[] { a, this.m_List[index] }));
                            }
                            else if (this.m_PageType == AdminGumpPage.AccountDetails_Access_Restrictions)
                            {
                                ArrayList list = new ArrayList(a.IPRestrictions);

                                list.Remove(this.m_List[index]);

                                a.IPRestrictions = (string[])list.ToArray(typeof(string));

                                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_Restrictions, 0, null, String.Format("{0} : Removed from list.", this.m_List[index]), a));
                            }
                        }

                        break;
                    }
                case 9:
                    {
                        if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                        {
                            if (this.m_PageType == AdminGumpPage.AccountDetails_Access_ClientIPs)
                            {
                                object obj = this.m_List[index];

                                if (!(obj is IPAddress))
                                    break;

                                Account a = this.m_State as Account;

                                if (a == null)
                                    break;

                                ArrayList list = GetSharedAccounts((IPAddress)obj);

                                if (list.Count > 1 || (list.Count == 1 && !list.Contains(a)))
                                    from.SendGump(new AdminGump(from, AdminGumpPage.Accounts, 0, list, null, new ArrayList()));
                                else
                                    from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Access_ClientIPs, 0, null, "There are no other accounts which share that address.", this.m_State));
                            }
                        }

                        break;
                    }
                case 10:
                    {
                        if (this.m_List != null && index >= 0 && index < this.m_List.Count)
                        {
                            if (this.m_PageType == AdminGumpPage.AccountDetails_Access_ClientIPs)
                            {
                                IPAddress ip = this.m_List[index] as IPAddress;

                                if (ip == null)
                                    break;

                                Account a = this.m_State as Account;

                                if (a == null)
                                    break;

                                from.SendGump(new WarningGump(1060635, 30720, String.Format("You are about to remove address {0} from account {1}. Do you wish to continue?", ip, a), 0xFFC000, 420, 280, new WarningGumpCallback(RemoveLoginIP_Callback), new object[] { a, ip }));
                            }
                        }

                        break;
                    }
            }
        }

        private void Shutdown(bool restart, bool save)
        {
            CommandLogging.WriteLine(this.m_From, "{0} {1} shutting down server (Restart: {2}) (Save: {3})", this.m_From.AccessLevel, CommandLogging.Format(this.m_From), restart, save);

            if (save)
                this.InvokeCommand("Save");

            Core.Kill(restart);
        }

        private void InvokeCommand(string c)
        {
            CommandSystem.Handle(this.m_From, String.Format("{0}{1}", CommandSystem.Prefix, c));
        }

        public static void GetAccountInfo(Account a, out AccessLevel accessLevel, out bool online)
        {
            accessLevel = a.AccessLevel;
            online = false;

            for (int j = 0; j < a.Length; ++j)
            {
                Mobile check = a[j];

                if (check == null)
                    continue;

                if (check.AccessLevel > accessLevel)
                    accessLevel = check.AccessLevel;

                if (check.NetState != null)
                    online = true;
            }
        }

        private class AddCommentPrompt : Prompt
        {
            private readonly Account m_Account;

            public AddCommentPrompt(Account acct)
            {
                this.m_Account = acct;
            }

            public override void OnCancel(Mobile from)
            {
                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Comments, 0, null, "Request to add comment was canceled.", this.m_Account));
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Account != null)
                {
                    this.m_Account.Comments.Add(new AccountComment(from.RawName, text));
                    from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Comments, 0, null, "Comment added.", this.m_Account));
                }
            }
        }

        private class AddTagNamePrompt : Prompt
        {
            private readonly Account m_Account;

            public AddTagNamePrompt(Account acct)
            {
                this.m_Account = acct;
            }

            public override void OnCancel(Mobile from)
            {
                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Tags, 0, null, "Request to add tag was canceled.", this.m_Account));
            }

            public override void OnResponse(Mobile from, string text)
            {
                from.Prompt = new AddTagValuePrompt(this.m_Account, text);
                from.SendMessage("Enter the new tag value.");
            }
        }

        private class AddTagValuePrompt : Prompt
        {
            private readonly Account m_Account;
            private readonly string m_Name;

            public AddTagValuePrompt(Account acct, string name)
            {
                this.m_Account = acct;
                this.m_Name = name;
            }

            public override void OnCancel(Mobile from)
            {
                from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Tags, 0, null, "Request to add tag was canceled.", this.m_Account));
            }

            public override void OnResponse(Mobile from, string text)
            {
                if (this.m_Account != null)
                {
                    this.m_Account.AddTag(this.m_Name, text);
                    from.SendGump(new AdminGump(from, AdminGumpPage.AccountDetails_Tags, 0, null, "Tag added.", this.m_Account));
                }
            }
        }

        private class NetStateComparer : IComparer
        {
            public static readonly IComparer Instance = new NetStateComparer();

            public NetStateComparer()
            {
            }

            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                NetState a = x as NetState;
                NetState b = y as NetState;

                if (a == null || b == null)
                    throw new ArgumentException();

                Mobile aMob = a.Mobile;
                Mobile bMob = b.Mobile;

                if (aMob == null && bMob == null)
                    return 0;
                else if (aMob == null)
                    return 1;
                else if (bMob == null)
                    return -1;

                if (aMob.AccessLevel > bMob.AccessLevel)
                    return -1;
                else if (aMob.AccessLevel < bMob.AccessLevel)
                    return 1;
                else
                    return Insensitive.Compare(aMob.Name, bMob.Name);
            }
        }

        private class AccountComparer : IComparer
        {
            public static readonly IComparer Instance = new AccountComparer();

            public AccountComparer()
            {
            }

            public int Compare(object x, object y)
            {
                if (x == null && y == null)
                    return 0;
                else if (x == null)
                    return -1;
                else if (y == null)
                    return 1;

                Account a = x as Account;
                Account b = y as Account;

                if (a == null || b == null)
                    throw new ArgumentException();

                AccessLevel aLevel, bLevel;
                bool aOnline, bOnline;

                GetAccountInfo(a, out aLevel, out aOnline);
                GetAccountInfo(b, out bLevel, out bOnline);

                if (aOnline && !bOnline)
                    return -1;
                else if (bOnline && !aOnline)
                    return 1;
                else if (aLevel > bLevel)
                    return -1;
                else if (aLevel < bLevel)
                    return 1;
                else
                    return Insensitive.Compare(a.Username, b.Username);
            }
        }
    }
}
