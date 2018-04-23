using System;
using System.IO;
using System.Collections.Generic;
using Server.Gumps;
using Server.Network;
using Server.Commands;

namespace Server.ACC.PG
{
    public enum Pages
    {
        Manage,
        Import
    };

    public enum ImportSelections
    {
        Systems,
        Categories,
        Locations
    };

    public class PGGumpParams : ACCGumpParams
    {
        public Pages Page;
        public ImportSelections ImportSelection;
        public KeyValuePair<PGCategory, int> SelectedCategory;
        public KeyValuePair<PGLocation, int> SelectedLocation;

        public PGGumpParams()
            : base()
        {
            SelectedCategory = new KeyValuePair<PGCategory, int>(null, -1);
            SelectedLocation = new KeyValuePair<PGLocation, int>(null, -1);
        }
    }

    public partial class PGSystem : ACCSystem
    {
        #region System
        private static List<PGCategory> m_CategoryList = new List<PGCategory>();
        public static List<PGCategory> CategoryList { get { return m_CategoryList; } set { m_CategoryList = value; } }

        public override string Name() { return "Public Gates"; }

        public override void Enable()
        {
            Console.WriteLine("{0} has just been enabled!", Name());
            GenGates();
        }

        public override void Disable()
        {
            Console.WriteLine("{0} has just been disabled!", Name());
            RemGates();
        }

        public static void Configure()
        {
            ACC.RegisterSystem("Server.ACC.PG.PGSystem");
        }

        public static bool Running
        {
            get { return ACC.SysEnabled("Server.ACC.PG.PGSystem"); }
        }

        public static void Initialize()
        {
            CommandSystem.Register("GenGates", PGSystem.PGAccessLevel, new CommandEventHandler(OnGenGates));
            CommandSystem.Register("RemGates", PGSystem.PGAccessLevel, new CommandEventHandler(OnRemGates));
        }

        [Usage("GenGates")]
        [Description("Generates a gate at each location of the control stone.")]
        private static void OnGenGates(CommandEventArgs e)
        {
            if (Running)
                GenGates();
            else
                e.Mobile.SendMessage("The Public Gate System is not active.");
        }

        [Usage("RemGates")]
        [Description("Removes all Public Gates from the world.")]
        private static void OnRemGates(CommandEventArgs e)
        {
            if (Running)
                RemGates();
            else
                e.Mobile.SendMessage("The Public Gate System is not active.");
        }

        public static void GenGates()
        {
            RemGates();

            if (m_CategoryList == null)
                m_CategoryList = new List<PGCategory>();

            int count = 0;
            IEnumerator<PGCategory> PGC = m_CategoryList.GetEnumerator();
            while (PGC.MoveNext())
            {
                if (PGC.Current.GetFlag(EntryFlag.Generate))
                {
                    IEnumerator<PGLocation> PGL = PGC.Current.Locations.GetEnumerator();
                    while (PGL.MoveNext())
                    {
                        if (PGL.Current.GetFlag(EntryFlag.Generate))
                        {
                            PublicGate gate = new PublicGate();

                            gate.MoveToWorld(PGL.Current.Location, PGL.Current.Map);
                            gate.Name = "Public Gate: " + PGL.Current.Name;
                            gate.Hue = PGL.Current.Hue;
                            count++;
                        }
                    }
                }
            }

            World.Broadcast(6, true, "{0} public gates generated.", count);
        }

        public static void RemGates()
        {
            List<PublicGate> list = new List<PublicGate>();

            foreach (Item item in World.Items.Values)
            {
                if (item is PublicGate && item.Parent == null)
                    list.Add((PublicGate)item);
            }

            foreach (Item item in list)
                item.Delete();

            if (list.Count > 0)
                World.Broadcast(6, true, "{0} public gates removed.", list.Count);
        }

        public override void Save(GenericWriter idx, GenericWriter tdb, GenericWriter writer)
        {
            writer.Write((int)0); //version

            writer.Write(m_CategoryList.Count);
            IEnumerator<PGCategory> Cats = m_CategoryList.GetEnumerator();
            while (Cats.MoveNext())
                Cats.Current.Serialize(writer);
        }

        public override void Load(BinaryReader idx, BinaryReader tdb, BinaryFileReader reader)
        {
            int version = reader.ReadInt();

            m_CategoryList = new List<PGCategory>();

            for (int i = reader.ReadInt(); i > 0; i--)
            {
                m_CategoryList.Add(new PGCategory(reader));
            }
        }
        #endregion //System

        #region Gumps
        private PGGumpParams Params;
        private string[] Dirs = null;

        public override void Gump(Mobile from, Gump gump, ACCGumpParams subParams)
        {
            gump.AddButton(195, 40, 2445, 2445, 101, GumpButtonType.Reply, 0);
            gump.AddLabel(200, 41, 1153, "Manage System");
            gump.AddButton(310, 40, 2445, 2445, 102, GumpButtonType.Reply, 0);
            gump.AddLabel(342, 41, 1153, "Import");

            if (subParams == null || !(subParams is PGGumpParams))
            {
                gump.AddHtml(215, 65, 300, 25, "<basefont size=7 color=white><center>Public Gates</center></font>", false, false);
                gump.AddHtml(140, 95, 450, 250, "<basefont color=white><center>Welcome to the Public Gate Admin Gump!</center><br>With this gump, you can manage the entire system and import and export locations or full categories.  Please choose an option from the top bar.<br><br>Manage System allows you to add/change/delete locations and categories from anywhere in the world.<br><br>Im/Ex port allows you to import or export categories and locations to files that you can distribute to other servers that use this system.</font>", false, false);
                return;
            }

            Params = subParams as PGGumpParams;

            switch ((int)Params.Page)
            {
                #region Manage Gump Code
                case (int)Pages.Manage:
                    {
                        gump.AddBackground(640, 0, 160, 400, 5120);
                        gump.AddButton(425, 40, 2445, 2445, 123, GumpButtonType.Reply, 0);
                        gump.AddLabel(456, 41, 1153, "Export");

                        for (int i = 0; i < m_CategoryList.Count && i < 50; i++)
                        {
                            PGCategory PGC = m_CategoryList[i];
                            if (PGC != null)
                            {
                                gump.AddButton(650, 10 + i * 30, 2501, 2501, 150 + i, GumpButtonType.Reply, 0);
                                gump.AddButton(655, 12 + i * 30, (Params.SelectedCategory.Key == PGC ? 5401 : 5402), (Params.SelectedCategory.Key == PGC ? 5402 : 5401), 150 + i, GumpButtonType.Reply, 0);
                                gump.AddLabel(675, 10 + i * 30, 1153, PGC.Name);
                            }
                        }

                        if (Params.SelectedCategory.Key != null)
                        {
                            gump.AddBackground(425, 75, 170, 285, 5120);
                            gump.AddButton(195, 65, 2445, 2445, 121, GumpButtonType.Reply, 0);
                            gump.AddLabel(206, 66, 1153, "Add Category");
                            gump.AddButton(310, 65, 2445, 2445, 122, GumpButtonType.Reply, 0);
                            gump.AddLabel(322, 66, 1153, "Add Location");

                            for (int i = 0, c = 0, r = 0; i < Params.SelectedCategory.Key.Locations.Count; i++)
                            {
                                PGLocation PGL = Params.SelectedCategory.Key.Locations[i];
                                if (PGL != null)
                                {
                                    gump.AddButton(120 + c * 150, 100 + r * 30, 2501, 2501, 200 + i, GumpButtonType.Reply, 0);
                                    gump.AddButton(125 + c * 150, 102 + r * 30, (Params.SelectedLocation.Key == PGL ? 5401 : 5402), (Params.SelectedLocation.Key == PGL ? 5402 : 5401), 200 + i, GumpButtonType.Reply, 0);
                                    gump.AddLabel(145 + c * 150, 100 + r * 30, 1153, PGL.Name);
                                    r += (c == 1 ? 1 : 0);
                                    c += (c == 1 ? -1 : 1);
                                }
                            }

                            if (Params.SelectedLocation.Key != null)
                            {
                                gump.AddButton(550, 265, 2642, 2643, 103, GumpButtonType.Reply, 0); //Apply Location

                                gump.AddImage(440, 85, 2501);
                                gump.AddTextEntry(446, 85, 130, 20, 0, 105, Params.SelectedLocation.Key.Name);

                                gump.AddImage(445, 110, 2443);
                                gump.AddImage(513, 110, 2443);
                                gump.AddImage(445, 135, 2443);
                                gump.AddImage(513, 135, 2443);
                                gump.AddImage(445, 160, 2443);

                                gump.AddTextEntry(450, 110, 53, 20, 0, 106, Params.SelectedLocation.Key.Location.X.ToString());
                                gump.AddTextEntry(518, 110, 53, 20, 0, 107, Params.SelectedLocation.Key.Location.Y.ToString());
                                gump.AddTextEntry(450, 135, 53, 20, 0, 108, Params.SelectedLocation.Key.Location.Z.ToString());
                                gump.AddTextEntry(518, 135, 53, 20, 0, 109, Params.SelectedLocation.Key.Hue.ToString());
                                gump.AddTextEntry(450, 160, 53, 20, 0, 110, Params.SelectedLocation.Key.Cost.ToString());

                                gump.AddLabel(435, 112, 1153, "X");
                                gump.AddLabel(578, 112, 1153, "Y");
                                gump.AddLabel(435, 137, 1153, "Z");
                                gump.AddLabel(578, 137, 1153, "H");
                                gump.AddLabel(435, 162, 1153, "C");

                                gump.AddRadio(435, 190, 208, 209, (Params.SelectedLocation.Key.Map == Map.Trammel), 111);
                                gump.AddRadio(570, 190, 208, 209, (Params.SelectedLocation.Key.Map == Map.Malas), 112);
                                gump.AddRadio(435, 215, 208, 209, (Params.SelectedLocation.Key.Map == Map.Felucca), 113);
                                gump.AddRadio(570, 215, 208, 209, (Params.SelectedLocation.Key.Map == Map.Ilshenar), 114);
                                gump.AddRadio(435, 240, 208, 209, (Params.SelectedLocation.Key.Map == Map.Tokuno), 115);
                                gump.AddRadio(570, 240, 208, 209, (Params.SelectedLocation.Key.Map == Map.TerMur), 127);

                                gump.AddLabel(460, 192, 1153, "Tram");
                                gump.AddLabel(530, 192, 1153, "Malas");
                                gump.AddLabel(460, 217, 1153, "Fel");
                                gump.AddLabel(542, 217, 1153, "Ilsh");
                                gump.AddLabel(460, 242, 1153, "Tokuno");
                                gump.AddLabel(530, 242, 1153, "TerMur");

                                gump.AddLabel(465, 282, 1153, "Young?");
                                gump.AddCheck(440, 280, 210, 211, Params.SelectedLocation.Key.GetFlag(EntryFlag.Young), 120);
                                gump.AddLabel(465, 307, 1153, "Gen?");
                                gump.AddCheck(440, 305, 210, 211, Params.SelectedLocation.Key.GetFlag(EntryFlag.Generate), 116);
                                gump.AddLabel(515, 307, 1153, "Staff?");
                                gump.AddCheck(565, 305, 210, 211, Params.SelectedLocation.Key.GetFlag(EntryFlag.StaffOnly), 117);
                                gump.AddLabel(465, 332, 1153, "Reds?");
                                gump.AddCheck(440, 330, 210, 211, Params.SelectedLocation.Key.GetFlag(EntryFlag.Reds), 118);
                                gump.AddLabel(522, 332, 1153, "Chrg?");
                                gump.AddCheck(565, 330, 210, 211, Params.SelectedLocation.Key.GetFlag(EntryFlag.Charge), 119);
                            }

                            else
                            {
                                gump.AddButton(550, 265, 2642, 2643, 104, GumpButtonType.Reply, 0); //Apply Category

                                gump.AddImage(440, 110, 2501);
                                gump.AddTextEntry(446, 110, 130, 20, 0, 105, Params.SelectedCategory.Key.Name);

                                gump.AddImage(445, 160, 2443);
                                gump.AddTextEntry(450, 160, 53, 20, 0, 110, Params.SelectedCategory.Key.Cost.ToString());
                                gump.AddLabel(435, 162, 1153, "C");

                                gump.AddLabel(465, 282, 1153, "Young?");
                                gump.AddCheck(440, 280, 210, 211, Params.SelectedCategory.Key.GetFlag(EntryFlag.Young), 120);
                                gump.AddLabel(465, 307, 1153, "Gen?");
                                gump.AddCheck(440, 305, 210, 211, Params.SelectedCategory.Key.GetFlag(EntryFlag.Generate), 116);
                                gump.AddLabel(515, 307, 1153, "Staff?");
                                gump.AddCheck(565, 305, 210, 211, Params.SelectedCategory.Key.GetFlag(EntryFlag.StaffOnly), 117);
                                gump.AddLabel(465, 332, 1153, "Reds?");
                                gump.AddCheck(440, 330, 210, 211, Params.SelectedCategory.Key.GetFlag(EntryFlag.Reds), 118);
                                gump.AddLabel(522, 332, 1153, "Chrg?");
                                gump.AddCheck(565, 330, 210, 211, Params.SelectedCategory.Key.GetFlag(EntryFlag.Charge), 119);
                            }
                        }
                        break;
                    }
                #endregion //Manage Gump Code

                #region Import Gump Code
                case (int)Pages.Import:
                    {//Import
                        if (!Directory.Exists("ACC Exports"))
                        {
                            from.SendMessage("There are no files to import!");
                            return;
                        }

                        gump.AddButton(195, 65, 2445, 2445, 124, GumpButtonType.Reply, 0); //Switch to Systems
                        gump.AddLabel(220, 66, 1153, "Systems");

                        gump.AddButton(310, 65, 2445, 2445, 125, GumpButtonType.Reply, 0); //Switch to Categories
                        gump.AddLabel(328, 66, 1153, "Categories");

                        gump.AddButton(425, 65, 2445, 2445, 126, GumpButtonType.Reply, 0); //Switch to Locations
                        gump.AddLabel(447, 66, 1153, "Locations");
                        switch ((int)Params.ImportSelection)
                        {
                            case (int)ImportSelections.Systems: { Dirs = Directory.GetFiles("ACC Exports/", "*.pgs"); break; }
                            case (int)ImportSelections.Categories: { Dirs = Directory.GetFiles("ACC Exports/", "*.pgc"); break; }
                            case (int)ImportSelections.Locations: { Dirs = Directory.GetFiles("ACC Exports/", "*.pgl"); break; }
                            default: { return; }
                        }
                        if (Dirs == null || Dirs.Length == 0)
                        {
                            from.SendMessage("There are no files of that type!");
                            return;
                        }
                        for (int i = 0, r = 0, c = 0; i < Dirs.Length && c < 3; i++)
                        {
                            string s = Dirs[i];
                            s = s.Remove(0, 12);
                            s = s.Remove(s.Length - 4, 4);
                            if (Params.ImportSelection == ImportSelections.Systems)
                                s = s.Remove(0, 9);

                            gump.AddButton(120 + c * 150, 100 + r * 30, 2501, 2501, 300 + i, GumpButtonType.Reply, 0);
                            gump.AddLabelCropped(125 + c * 150, 101 + r * 30, 140, 30, 1153, s);

                            c += (r == 7 ? 1 : 0);
                            r += (r == 7 ? -7 : 1);
                        }
                        break;
                    }
                #endregion //Import Gump Code
            }
        }

        public override void Help(Mobile from, Gump gump)
        {
        }
        /* ID's:
         101 = Button Manage System
         102 = Button Import Page
         103 = Button Apply Location
         104 = Button Apply Category
         105 = Text   Name
         106 = Text   X
         107 = Text   Y
         108 = Text   Z
         109 = Text   Hue
         110 = Text   Cost
         111 = Radio  Trammel
         112 = Radio  Malas
         113 = Radio  Felucca
         114 = Radio  Ilshenar
         115 = Radio  Tokuno
         116 = Check  Generate
         117 = Check  StaffOnly
         118 = Check  Reds
         119 = Check  Charge
         120 = Check  Young
         121 = Button Add Category
         122 = Button Add Location
         123 = Button Export
         124 = Button Import Systems
         125 = Button Import Categories
         126 = Button Import Locations
         127 = Radio  TerMur
         300+ = Imports 
          */
        public override void OnResponse(NetState state, RelayInfo info, ACCGumpParams subParams)
        {
            if (info.ButtonID == 0 || state.Mobile.AccessLevel < ACC.GlobalMinimumAccessLevel)
                return;

            if (subParams is PGGumpParams)
                Params = subParams as PGGumpParams;

            PGGumpParams newParams = new PGGumpParams();

            if (info.ButtonID == 101)
                newParams.Page = Pages.Manage;

            else if (info.ButtonID == 102)
                newParams.Page = Pages.Import;

            #region Add/Remove
            else if (info.ButtonID == 103 || info.ButtonID == 104 || info.ButtonID == 121 || info.ButtonID == 122)
            {
                SetFlag(EntryFlag.Generate, info.IsSwitched(116));
                SetFlag(EntryFlag.StaffOnly, info.IsSwitched(117));
                SetFlag(EntryFlag.Reds, info.IsSwitched(118));
                SetFlag(EntryFlag.Charge, info.IsSwitched(119));

                Map Map = info.IsSwitched(111) ? Map.Trammel : info.IsSwitched(112) ? Map.Malas : info.IsSwitched(113) ? Map.Felucca : info.IsSwitched(114) ? Map.Ilshenar : info.IsSwitched(115) ? Map.Tokuno : info.IsSwitched(127) ? Map.TerMur : Map.Trammel;

                string Name = GetString(info, 105);
                string X = GetString(info, 106);
                string Y = GetString(info, 107);
                string Z = GetString(info, 108);
                string H = GetString(info, 109);
                string C = GetString(info, 110);

                if (Name == null || Name.Length == 0)
                {
                    try
                    {
                        state.Mobile.SendMessage("Removed the entry");
                        if (info.ButtonID == 103)
                            Params.SelectedCategory.Key.Locations.Remove(Params.SelectedLocation.Key);
                        else
                            m_CategoryList.Remove(Params.SelectedCategory.Key);
                    }
                    catch
                    {
                        Console.WriteLine("Exception caught removing entry");
                    }
                }

                else
                {
                    if (info.ButtonID == 103 || info.ButtonID == 122)
                    {
                        int x, y, z, h, c = 0;
                        Point3D Loc;
                        int Hue;
                        int Cost;
                        PGLocation PGL;

                        if (X == null || X.Length == 0 ||
                            Y == null || Y.Length == 0 ||
                            Z == null || Z.Length == 0 ||
                            H == null || H.Length == 0 ||
                            C == null || C.Length == 0)
                        {
                            if (info.ButtonID == 122)
                            {
                                Hue = 0;
                                Loc = new Point3D(0, 0, 0);
                                Cost = 0;

                                PGL = new PGLocation(Name, Flags, Loc, Map, Hue, Cost);
                                if (PGL == null)
                                {
                                    state.Mobile.SendMessage("Error adding Location.");
                                    return;
                                }

                                m_CategoryList[Params.SelectedCategory.Value].Locations.Add(PGL);
                            }

                            state.Mobile.SendMessage("Please fill in each field.");
                            state.Mobile.SendGump(new ACCGump(state.Mobile, this.ToString(), Params));
                            return;
                        }

                        try
                        {
                            x = Int32.Parse(X);
                            y = Int32.Parse(Y);
                            z = Int32.Parse(Z);
                            h = Int32.Parse(H);
                            c = Int32.Parse(C);
                            Loc = new Point3D(x, y, z);
                            Hue = h;
                            Cost = c;
                        }
                        catch
                        {
                            state.Mobile.SendMessage("Please enter an integer in each of the info fields. (X, Y, Z, H, Cost)");
                            state.Mobile.SendGump(new ACCGump(state.Mobile, this.ToString(), Params));
                            return;
                        }

                        PGL = new PGLocation(Name, Flags, Loc, Map, Hue, Cost);
                        if (PGL == null)
                        {
                            state.Mobile.SendMessage("Bad Location information, can't add!");
                        }
                        else
                        {
                            try
                            {
                                if (info.ButtonID == 122)
                                {
                                    m_CategoryList[Params.SelectedCategory.Value].Locations.Add(PGL);
                                    state.Mobile.SendMessage("Added the Location.");
                                }
                                else
                                {
                                    state.Mobile.SendMessage("Changed the Location.");
                                    m_CategoryList[Params.SelectedCategory.Value].Locations[Params.SelectedLocation.Value] = PGL;
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Problem adding/changing Location!");
                            }
                        }
                    }

                    else
                    {
                        if (C == null || C.Length == 0)
                        {
                            state.Mobile.SendMessage("Please fill in each field.");
                            state.Mobile.SendGump(new ACCGump(state.Mobile, this.ToString(), Params));
                            return;
                        }

                        int c = 0;
                        int Cost;
                        try
                        {
                            c = Int32.Parse(C);
                            Cost = c;
                        }
                        catch
                        {
                            state.Mobile.SendMessage("Please enter an integer for the Cost");
                            state.Mobile.SendGump(new ACCGump(state.Mobile, this.ToString(), Params));
                            return;
                        }

                        try
                        {
                            if (info.ButtonID == 121)
                            {
                                m_CategoryList.Add(new PGCategory(Name, Flags, Cost));
                                state.Mobile.SendMessage("Added the Category.");
                            }
                            else
                            {
                                m_CategoryList[Params.SelectedCategory.Value].Name = Name;
                                m_CategoryList[Params.SelectedCategory.Value].Flags = Flags;
                                m_CategoryList[Params.SelectedCategory.Value].Cost = Cost;
                                state.Mobile.SendMessage("Changed the Category.");
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Problems adding/changing Category!");
                        }
                    }
                }
            }
            #endregion //Add/Remove

            #region Imports/Exports
            #region Exports
            else if (info.ButtonID == 123)
            {
                if (!Directory.Exists("ACC Exports"))
                    Directory.CreateDirectory("ACC Exports");

                string fileName;
                string Path = "ACC Exports/";

                if (Params.SelectedLocation.Key != null)
                    fileName = String.Format("Location - {0}.pgl", Params.SelectedLocation.Key.Name);
                else if (Params.SelectedCategory.Key != null)
                    fileName = String.Format("Category - {0}.pgc", Params.SelectedCategory.Key.Name);
                else
                    fileName = String.Format("System - {0:yyMMdd-HHmmss}.pgs", DateTime.UtcNow);

                try
                {
                    using (FileStream m_FileStream = new FileStream(Path + fileName, FileMode.Create, FileAccess.Write))
                    {
                        GenericWriter writer = new BinaryFileWriter(m_FileStream, true);

                        if (Params.SelectedLocation.Key != null)
                        {
                            Params.SelectedLocation.Key.Serialize(writer);
                            state.Mobile.SendMessage("Exported the Location to {0}{1}", Path, fileName);
                        }
                        else if (Params.SelectedCategory.Key != null)
                        {
                            Params.SelectedCategory.Key.Serialize(writer);
                            state.Mobile.SendMessage("Exported the Category (and all Locations contained within) to {0}{1}", Path, fileName);
                        }
                        else
                        {
                            writer.Write((int)0); //version

                            writer.Write(m_CategoryList.Count);
                            for (int i = 0; i < m_CategoryList.Count; i++)
                            {
                                m_CategoryList[i].Serialize(writer);
                            }
                            state.Mobile.SendMessage("Exported the entire Public Gates System to {0}{1}", Path, fileName);
                        }

                        writer.Close();
                        m_FileStream.Close();
                    }
                }
                catch (Exception e)
                {
                    state.Mobile.SendMessage("Problem exporting the selection.  Please contact the admin.");
                    Console.WriteLine("Error exporting PGSystem : {0}", e);
                }
            }
            #endregion //Exports
            #region Imports
            //Switch between import types
            else if (info.ButtonID == 124 || info.ButtonID == 125 || info.ButtonID == 126)
            {
                newParams.Page = Pages.Import;
                switch (info.ButtonID)
                {
                    case 124: newParams.ImportSelection = ImportSelections.Systems; break;
                    case 125: newParams.ImportSelection = ImportSelections.Categories; break;
                    case 126: newParams.ImportSelection = ImportSelections.Locations; break;
                }
            }
            //Perform the import
            else if (info.ButtonID >= 300 && Dirs != null && Dirs.Length > 0)
            {
                if (!Directory.Exists("ACC Exports"))
                    Directory.CreateDirectory("ACC Exports");

                string Path = null;
                try
                {
                    Path = Dirs[info.ButtonID - 300];

                    if (File.Exists(Path))
                    {
                        using (FileStream m_FileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            BinaryFileReader reader = new BinaryFileReader(new BinaryReader(m_FileStream));

                            switch ((int)Params.ImportSelection)
                            {
                                case (int)ImportSelections.Systems:
                                    {//Systems
                                        int version = reader.ReadInt();
                                        int count = reader.ReadInt();
                                        List<PGCategory> list = new List<PGCategory>();
                                        for (int i = 0; i < count; i++)
                                            list.Add(new PGCategory(reader));

                                        state.Mobile.CloseGump(typeof(SysChoiceGump));
                                        state.Mobile.SendGump(new SysChoiceGump(this.ToString(), Params, list));
                                        reader.Close();
                                        return;
                                    }
                                case (int)ImportSelections.Categories:
                                    {//Categories
                                        if (m_CategoryList == null)
                                            m_CategoryList = new List<PGCategory>();

                                        m_CategoryList.Add(new PGCategory(reader));
                                        state.Mobile.SendMessage("Added the Category.");
                                        break;
                                    }
                                case (int)ImportSelections.Locations:
                                    {//Locations
                                        state.Mobile.CloseGump(typeof(CatSelGump));
                                        state.Mobile.SendMessage("Please choose a Category to put this Location in.");
                                        state.Mobile.SendGump(new CatSelGump(this.ToString(), Params, new PGLocation(reader)));
                                        reader.Close();
                                        return;
                                    }
                            }

                            reader.Close();
                        }
                    }
                }
                catch
                {
                }
            }
            #endregion //Imports
            #endregion //Imports/Exports

            else if (info.ButtonID >= 150 && info.ButtonID < m_CategoryList.Count + 150)
                newParams.SelectedCategory = new KeyValuePair<PGCategory, int>(m_CategoryList[info.ButtonID - 150], info.ButtonID - 150);

            else if (info.ButtonID >= 200 && info.ButtonID < 200 + Params.SelectedCategory.Key.Locations.Count)
            {
                newParams.SelectedCategory = Params.SelectedCategory;
                newParams.SelectedLocation = new KeyValuePair<PGLocation, int>(Params.SelectedCategory.Key.Locations[info.ButtonID - 200], info.ButtonID - 200);
            }

            state.Mobile.SendGump(new ACCGump(state.Mobile, this.ToString(), newParams));
        }

        private string GetString(RelayInfo info, int id)
        {
            TextRelay t = info.GetTextEntry(id);
            return (t == null ? null : t.Text.Trim());
        }

        private EntryFlag Flags;
        private void SetFlag(EntryFlag flag, bool value)
        {
            if (value)
                Flags |= flag;
            else Flags &= ~flag;
        }

        private class SysChoiceGump : Gump
        {
            private string Sys = null;
            private List<PGCategory> List;
            private PGGumpParams Params;
            public SysChoiceGump(string sys, ACCGumpParams subParams, List<PGCategory> list)
                : base(0, 0)
            {
                Sys = sys;
                if (subParams is PGGumpParams)
                    Params = subParams as PGGumpParams;
                List = list;
                AddPage(0);
                AddBackground(250, 245, 300, 90, 5120);
                AddLabel(282, 260, 1153, "Overwrite or Add To current system?");
                AddButton(280, 290, 2445, 2445, 1, GumpButtonType.Reply, 0);
                AddButton(410, 290, 2445, 2445, 2, GumpButtonType.Reply, 0);
                AddLabel(300, 291, 1153, "Overwrite");
                AddLabel(442, 291, 1153, "Add To");
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                if (info.ButtonID == 1)
                    m_CategoryList = new List<PGCategory>();

                for (int i = 0; i < List.Count; i++)
                    m_CategoryList.Add(List[i]);

                state.Mobile.SendGump(new ACCGump(state.Mobile, Sys, Params));
                state.Mobile.SendMessage("Import successful.");
            }
        }

        private class CatSelGump : Gump
        {
            private string Sys = null;
            private PGGumpParams Params;
            private PGLocation Loc = null;
            public CatSelGump(string sys, ACCGumpParams subParams, PGLocation loc)
                : base(0, 0)
            {
                if (sys == null || subParams == null || loc == null || m_CategoryList == null)
                    return;

                Sys = sys;
                if (subParams is PGGumpParams)
                    Params = subParams as PGGumpParams;
                Loc = loc;

                AddPage(0);
                AddBackground(640, 0, 160, 400, 5120);

                for (int i = 0; i < PGSystem.CategoryList.Count; i++)
                {
                    PGCategory PGC = m_CategoryList[i];
                    if (PGC != null)
                    {
                        AddButton(650, 10 + i * 30, 2501, 2501, 1 + i, GumpButtonType.Reply, 0);
                        AddLabel(675, 10 + i * 30, 1153, PGC.Name);
                    }
                }
            }

            public override void OnResponse(NetState state, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                m_CategoryList[info.ButtonID - 1].Locations.Add(Loc);
                state.Mobile.SendGump(new ACCGump(state.Mobile, Sys, Params));
                state.Mobile.SendMessage("Location added to {0}.", m_CategoryList[info.ButtonID - 1].Name);
            }
        }
        #endregion //Gumps
    }
}