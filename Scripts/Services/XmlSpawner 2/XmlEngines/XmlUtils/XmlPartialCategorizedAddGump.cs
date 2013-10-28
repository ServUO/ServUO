using System;
using System.Collections;
using System.Reflection;
using Server.Mobiles;

/*
** Modified from RunUO 1.0.0 AddGump.cs
** by ArteGordon
** 3/13/05
*/
namespace Server.Gumps
{
    public class XmlPartialCategorizedAddGump : Gump
    {
        private static readonly Type typeofItem = typeof(Item);
        private static readonly Type typeofMobile = typeof(Mobile);
        private readonly string m_SearchString;
        private readonly ArrayList m_SearchResults;
        private readonly int m_Page;
        private readonly Gump m_Gump;
        private readonly int m_EntryIndex = -1;
        private readonly XmlSpawner m_Spawner;
        public XmlPartialCategorizedAddGump(Mobile from, string searchString, int page, ArrayList searchResults, bool explicitSearch, int entryindex, Gump gump)
            : base(50, 50)
        {
            if (gump is XmlSpawnerGump)
            {
                // keep track of the spawner for xmlspawnergumps
                this.m_Spawner = ((XmlSpawnerGump)gump).m_Spawner;
            }

            // keep track of the gump
            this.m_Gump = gump;

            this.m_SearchString = searchString;
            this.m_SearchResults = searchResults;
            this.m_Page = page;
            
            this.m_EntryIndex = entryindex;

            from.CloseGump(typeof(XmlPartialCategorizedAddGump));

            this.AddPage(0);

            this.AddBackground(0, 0, 420, 280, 5054);

            this.AddImageTiled(10, 10, 400, 20, 2624);
            this.AddAlphaRegion(10, 10, 400, 20);
            this.AddImageTiled(41, 11, 184, 18, 0xBBC);
            this.AddImageTiled(42, 12, 182, 16, 2624);
            this.AddAlphaRegion(42, 12, 182, 16);

            this.AddButton(10, 9, 4011, 4013, 1, GumpButtonType.Reply, 0);
            this.AddTextEntry(44, 10, 180, 20, 0x480, 0, searchString);

            this.AddHtmlLocalized(230, 10, 100, 20, 3010005, 0x7FFF, false, false);

            this.AddImageTiled(10, 40, 400, 200, 2624);
            this.AddAlphaRegion(10, 40, 400, 200);

            if (searchResults.Count > 0)
            {
                for (int i = (page * 10); i < ((page + 1) * 10) && i < searchResults.Count; ++i)
                {
                    int index = i % 10;

                    SearchEntry se = (SearchEntry)searchResults[i];

                    string labelstr = se.EntryType.Name;

                    if (se.Parameters.Length > 0)
                    {
                        for (int j = 0; j < se.Parameters.Length; j++)
                        {
                            labelstr += ", " + se.Parameters[j].Name;
                        }
                    }
					
                    this.AddLabel(44, 39 + (index * 20), 0x480, labelstr);
                    this.AddButton(10, 39 + (index * 20), 4023, 4025, 4 + i, GumpButtonType.Reply, 0);
                }
            }
            else
            {
                this.AddLabel(15, 44, 0x480, explicitSearch ? "Nothing matched your search terms." : "No results to display.");
            }

            this.AddImageTiled(10, 250, 400, 20, 2624);
            this.AddAlphaRegion(10, 250, 400, 20);

            if (this.m_Page > 0)
                this.AddButton(10, 249, 4014, 4016, 2, GumpButtonType.Reply, 0);
            else
                this.AddImage(10, 249, 4014);

            this.AddHtmlLocalized(44, 250, 170, 20, 1061028, this.m_Page > 0 ? 0x7FFF : 0x5EF7, false, false); // Previous page

            if (((this.m_Page + 1) * 10) < searchResults.Count)
                this.AddButton(210, 249, 4005, 4007, 3, GumpButtonType.Reply, 0);
            else
                this.AddImage(210, 249, 4005);

            this.AddHtmlLocalized(244, 250, 170, 20, 1061027, ((this.m_Page + 1) * 10) < searchResults.Count ? 0x7FFF : 0x5EF7, false, false); // Next page
        }

        public static ArrayList Match(string match)
        {
            ArrayList results = new ArrayList();
            Type[] types;

            Assembly[] asms = ScriptCompiler.Assemblies;

            for (int i = 0; i < asms.Length; ++i)
            {
                types = ScriptCompiler.GetTypeCache(asms[i]).Types;
                Match(match, types, results);
            }

            types = ScriptCompiler.GetTypeCache(Core.Assembly).Types;
            Match(match, types, results);

            results.Sort(new TypeNameComparer());

            return results;
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch ( info.ButtonID )
            {
                case 1: // Search
                    {
                        TextRelay te = info.GetTextEntry(0);
                        string match = (te == null ? "" : te.Text.Trim());

                        if (match.Length < 3)
                        {
                            from.SendMessage("Invalid search string.");
                            from.SendGump(new XmlPartialCategorizedAddGump(from, match, this.m_Page, this.m_SearchResults, false, this.m_EntryIndex, this.m_Gump));
                        }
                        else
                        {
                            from.SendGump(new XmlPartialCategorizedAddGump(from, match, 0, Match(match) , true, this.m_EntryIndex, this.m_Gump));
                        }

                        break;
                    }
                case 2: // Previous page
                    {
                        if (this.m_Page > 0)
                            from.SendGump(new XmlPartialCategorizedAddGump(from, this.m_SearchString, this.m_Page - 1, this.m_SearchResults, true, this.m_EntryIndex, this.m_Gump));

                        break;
                    }
                case 3: // Next page
                    {
                        if ((this.m_Page + 1) * 10 < this.m_SearchResults.Count)
                            from.SendGump(new XmlPartialCategorizedAddGump(from, this.m_SearchString, this.m_Page + 1, this.m_SearchResults, true, this.m_EntryIndex, this.m_Gump));

                        break;
                    }
                default:
                    {
                        int index = info.ButtonID - 4;

                        if (index >= 0 && index < this.m_SearchResults.Count)
                        {
                            Type type = ((SearchEntry)this.m_SearchResults[index]).EntryType;

                            if (this.m_Gump is XmlAddGump && type != null)
                            {
                                XmlAddGump m_XmlAddGump = (XmlAddGump)this.m_Gump;
                                if (type != null && m_XmlAddGump.defs != null && m_XmlAddGump.defs.NameList != null &&
                                    this.m_EntryIndex >= 0 && this.m_EntryIndex < m_XmlAddGump.defs.NameList.Length)
                                {
                                    m_XmlAddGump.defs.NameList[this.m_EntryIndex] = type.Name;
                                    XmlAddGump.Refresh(from, true);
                                }
                            }
                            else if (this.m_Spawner != null && type != null)
                            { 
                                XmlSpawnerGump xg = this.m_Spawner.SpawnerGump; 

                                if (xg != null)
                                {
                                    xg.Rentry = new XmlSpawnerGump.ReplacementEntry();
                                    xg.Rentry.Typename = type.Name;
                                    xg.Rentry.Index = this.m_EntryIndex;
                                    xg.Rentry.Color = 0x1436;

                                    Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(XmlSpawnerGump.Refresh_Callback), new object[] { from });
                                    //from.CloseGump(typeof(XmlSpawnerGump));
                                    //from.SendGump( new XmlSpawnerGump(xg.m_Spawner, xg.X, xg.Y, xg.m_ShowGump, xg.xoffset, xg.page, xg.Rentry) );
                                }
                            }
                        }

                        break;
                    }
            }
        }

        private static void Match(string match, Type[] types, ArrayList results)
        {
            if (match.Length == 0)
                return;

            match = match.ToLower();

            for (int i = 0; i < types.Length; ++i)
            {
                Type t = types[i];

                if ((typeofMobile.IsAssignableFrom(t) || typeofItem.IsAssignableFrom(t)) && t.Name.ToLower().IndexOf(match) >= 0 && !results.Contains(t))
                {
                    ConstructorInfo[] ctors = t.GetConstructors();

                    for (int j = 0; j < ctors.Length; ++j)
                    {
                        if (/*ctors[j].GetParameters().Length == 0 && */ ctors[j].IsDefined(typeof(ConstructableAttribute), false))
                        {
                            SearchEntry s = new SearchEntry();
                            s.EntryType = t;
                            s.Parameters = ctors[j].GetParameters();
                            //results.Add( t );
                            results.Add(s);
                            //break;
                        }
                    }
                }
            }
        }

        private class SearchEntry
        {
            public Type EntryType;
            public ParameterInfo[] Parameters;
            public SearchEntry()
            {
            }
        }

        private class TypeNameComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                SearchEntry a = x as SearchEntry;
                SearchEntry b = y as SearchEntry;

                return a.EntryType.Name.CompareTo(b.EntryType.Name);
            }
        }
    }
}