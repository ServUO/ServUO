#define NEWPROPSGUMP
#define BOOKTEXTENTRY
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class TextEntryGump : Gump
    {
        private readonly XmlSpawner m_Spawner;
        private readonly int m_index;
        private readonly XmlSpawnerGump m_SpawnerGump;

        public TextEntryGump(XmlSpawner spawner, XmlSpawnerGump spawnergump, int index, int X, int Y)
            : base(X, Y)
        {
            if (spawner == null || spawner.Deleted)
                return;
            m_Spawner = spawner;
            m_index = index;
            m_SpawnerGump = spawnergump;

            AddPage(0);

            AddBackground(20, 0, 220, 354, 5054);
            AddAlphaRegion(20, 0, 220, 354);
            AddImageTiled(23, 5, 214, 270, 0x52);
            AddImageTiled(24, 6, 213, 261, 0xBBC);

            string label = spawner.Name + " entry " + index;
            AddLabel(28, 10, 0x384, label);

            // OK button
            AddButton(25, 325, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            // Close button
            AddButton(205, 325, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            // Edit button
            AddButton(100, 325, 0xEF, 0xEE, 2, GumpButtonType.Reply, 0);
            string str = null;
            if (index < m_Spawner.SpawnObjects.Length)
            {
                str = m_Spawner.SpawnObjects[index].TypeName;
            }
            // main text entry area
            AddTextEntry(35, 30, 200, 251, 0, 0, str);

            // editing text entry areas
            // background for text entry area
            AddImageTiled(23, 275, 214, 23, 0x52);
            AddImageTiled(24, 276, 213, 21, 0xBBC);
            AddImageTiled(23, 300, 214, 23, 0x52);
            AddImageTiled(24, 301, 213, 21, 0xBBC);

            AddTextEntry(35, 275, 200, 21, 0, 1, null);
            AddTextEntry(35, 300, 200, 21, 0, 2, null);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info == null || state?.Mobile == null)
                return;

            if (m_Spawner == null || m_Spawner.Deleted)
                return;

            bool update_entry = false;
            bool edit_entry = false;

            switch (info.ButtonID)
            {
                case 0: // Close
                    {
                        break;
                    }
                case 1: // Okay
                    {
                        update_entry = true;
                        break;
                    }
                case 2: // Edit
                    {
                        edit_entry = true;
                        break;
                    }
                default:
                    update_entry = true;
                    break;
            }
            if (edit_entry)
            {
                // get the old text
                TextRelay entry = info.GetTextEntry(1);
                string oldtext = entry.Text;
                // get the new text
                entry = info.GetTextEntry(2);
                string newtext = entry.Text;
                // make the substitution
                entry = info.GetTextEntry(0);
                string origtext = entry.Text;
                if (origtext != null && oldtext != null && newtext != null)
                {
                    try
                    {
                        int firstindex = origtext.IndexOf(oldtext);
                        if (firstindex >= 0)
                        {


                            int secondindex = firstindex + oldtext.Length;

                            int lastindex = origtext.Length - 1;

                            string editedtext;
                            if (firstindex > 0)
                            {
                                editedtext = origtext.Substring(0, firstindex) + newtext + origtext.Substring(secondindex, lastindex - secondindex + 1);
                            }
                            else
                            {
                                editedtext = newtext + origtext.Substring(secondindex, lastindex - secondindex + 1);
                            }

                            if (m_index < m_Spawner.SpawnObjects.Length)
                            {
                                m_Spawner.SpawnObjects[m_index].TypeName = editedtext;
                            }
                            else
                            {
                                // Update the creature list
                                m_Spawner.SpawnObjects = m_SpawnerGump.CreateArray(info, state.Mobile);
                            }
                        }
                    }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }

                }
                // open a new text entry gump
                state.Mobile.SendGump(new TextEntryGump(m_Spawner, m_SpawnerGump, m_index, X, Y));
                return;
            }
            if (update_entry)
            {
                TextRelay entry = info.GetTextEntry(0);
                if (m_index < m_Spawner.SpawnObjects.Length)
                {
                    m_Spawner.SpawnObjects[m_index].TypeName = entry.Text;
                }
                else
                {
                    // Update the creature list
                    m_Spawner.SpawnObjects = m_SpawnerGump.CreateArray(info, state.Mobile);
                }
            }

            // open a new spawner gump
            state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, X, Y, m_SpawnerGump.m_ShowGump, m_SpawnerGump.xoffset, m_SpawnerGump.page));
        }
    }

    public class XmlSpawnerGump : Gump
    {
        private static int nclicks = 0;
        public XmlSpawner m_Spawner;
        public const int MaxSpawnEntries = 60;
        private const int MaxEntriesPerPage = 15;
        public int m_ShowGump = 0;
        public int xoffset = 0;
        public int initial_maxcount;
        public int page;
        public ReplacementEntry Rentry;

        public class ReplacementEntry
        {
            public string Typename;
            public int Index;
            public int Color;
        }

        public XmlSpawnerGump(XmlSpawner spawner, int X, int Y, int extension, int textextension, int newpage)
            : this(spawner, X, Y, extension, textextension, newpage, null)
        {
        }

        public XmlSpawnerGump(XmlSpawner spawner, int X, int Y, int extension, int textextension, int newpage, ReplacementEntry rentry)
            : base(X, Y)
        {
            if (spawner == null || spawner.Deleted)
                return;

            m_Spawner = spawner;
            spawner.SpawnerGump = this;
            xoffset = textextension;
            initial_maxcount = spawner.MaxCount;
            page = newpage;
            Rentry = rentry;

            AddPage(0);

            // automatically change the gump depending on whether sequential spawning and/or subgroups are activated

            if (spawner.SequentialSpawn >= 0 || spawner.HasSubGroups() || spawner.HasIndividualSpawnTimes())
            {
                // show the fully extended gump with subgroups and reset timer info
                m_ShowGump = 2;
            }

            if (extension > 0)
            {
                m_ShowGump = extension;
            }
            if (extension < 0)
            {
                m_ShowGump = 0;
            }

            // if the expanded gump toggle has been activated then override the auto settings.
            if (m_ShowGump > 1)
            {
                AddBackground(0, 0, 670 + xoffset + 30, 474, 5054);
                AddAlphaRegion(0, 0, 670 + xoffset + 30, 474);
            }
            else
                if (m_ShowGump > 0)
            {
                AddBackground(0, 0, 335 + xoffset, 474, 5054);
                AddAlphaRegion(0, 0, 335 + xoffset, 474);
            }
            else
            {
                AddBackground(0, 0, 305 + xoffset, 474, 5054);
                AddAlphaRegion(0, 0, 305 + xoffset, 474);
            }

            // spawner name area
            AddImageTiled(3, 5, 227, 23, 0x52);
            AddImageTiled(4, 6, 225, 21, 0xBBC);
            AddTextEntry(6, 5, 222, 21, 0, 999, spawner.Name); // changed from color 50

            AddButton(5, 450, 0xFAE, 0xFAF, 4, GumpButtonType.Reply, 0);
            AddLabel(38, 450, 0x384, "Goto");

            //AddButton( 5, 428, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 );
            AddButton(5, 428, 0xFAE, 0xFAF, 1, GumpButtonType.Reply, 0);
            AddLabel(38, 428, 0x384, "Help");

            AddButton(71, 428, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0);
            AddLabel(104, 428, 0x384, "Bring Home");
            AddButton(71, 450, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
            AddLabel(104, 450, 0x384, "Respawn");

            // Props button
            AddButton(168, 428, 0xFAB, 0xFAD, 9999, GumpButtonType.Reply, 0);
            AddLabel(201, 428, 0x384, "Props");

            // Sort button
            AddButton(168, 450, 0xFAB, 0xFAD, 702, GumpButtonType.Reply, 0);
            AddLabel(201, 450, 0x384, "Sort");

            // Reset button
            AddButton(71, 406, 0xFA2, 0xFA3, 701, GumpButtonType.Reply, 0);
            AddLabel(104, 406, 0x384, "Reset");

            // Refresh button
            AddButton(168, 406, 0xFBD, 0xFBE, 9998, GumpButtonType.Reply, 0);
            AddLabel(201, 406, 0x384, "Refresh");

            AddButton(280, 395, m_Spawner.DisableGlobalAutoReset ? 0xD3 : 0xD2,
                                m_Spawner.DisableGlobalAutoReset ? 0xD2 : 0xD3, 9997, GumpButtonType.Reply, 0);
            AddLabel(263, 410, m_Spawner.DisableGlobalAutoReset ? 68 : 33, "Disable");
            AddLabel(247, 424, m_Spawner.DisableGlobalAutoReset ? 68 : 33, "TickReset");

            // add run status display
            if (m_Spawner.Running)
            {
                AddButton(5, 399, 0x2A4E, 0x2A3A, 700, GumpButtonType.Reply, 0);
                AddLabel(38, 406, 0x384, "On");
            }
            else
            {
                AddButton(5, 399, 0x2A62, 0x2A3A, 700, GumpButtonType.Reply, 0);
                AddLabel(38, 406, 0x384, "Off");
            }

            // Add sequential spawn state
            if (m_Spawner.SequentialSpawn >= 0)
            {
                AddLabel(15, 365, 33, string.Format("{0}", m_Spawner.SequentialSpawn));
            }

            // Add Current / Max count labels
            AddLabel(231 + xoffset, 9, 68, "Count");
            AddLabel(270 + xoffset, 9, 33, "Max");

            if (m_ShowGump > 0)
            {
                // Add subgroup label
                AddLabel(334 + xoffset, 9, 68, "Sub");
            }
            if (m_ShowGump > 1)
            {
                // Add entry field labels
                AddLabel(303 + xoffset, 9, 68, "Per");
                AddLabel(329 + xoffset + 30, 9, 68, "Reset");
                AddLabel(368 + xoffset + 30, 9, 68, "To");
                AddLabel(392 + xoffset + 30, 9, 68, "Kills");
                AddLabel(432 + xoffset + 30, 9, 68, "MinD");
                AddLabel(472 + xoffset + 30, 9, 68, "MaxD");
                AddLabel(515 + xoffset + 30, 9, 68, "Rng");
                AddLabel(545 + xoffset + 30, 9, 68, "RK");
                AddLabel(565 + xoffset + 30, 9, 68, "Clr");
                AddLabel(590 + xoffset + 30, 9, 68, "NextSpawn");
            }

            // add area for spawner max
            AddLabel(180 + xoffset, 365, 50, "Spawner");
            AddImageTiled(267 + xoffset, 365, 35, 23, 0x52);
            AddImageTiled(268 + xoffset, 365, 32, 21, 0xBBC);
            AddTextEntry(273 + xoffset, 365, 33, 33, 33, 300, m_Spawner.MaxCount.ToString());

            // add area for spawner count
            AddImageTiled(231 + xoffset, 365, 35, 23, 0x52);
            AddImageTiled(232 + xoffset, 365, 32, 21, 0xBBC);
            AddLabel(233 + xoffset, 365, 68, m_Spawner.CurrentCount.ToString());

            // add the status string
            AddTextEntry(38, 384, 235, 33, 33, 900, m_Spawner.status_str);
            // add the page buttons
            for (int i = 0; i < MaxSpawnEntries / MaxEntriesPerPage; i++)
            {
                //AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
                AddButton(38 + i * 25, 365, 0x8B1 + i, 0x8B1 + i, 4000 + i, GumpButtonType.Reply, 0);
            }

            // add gump extension button
            if (m_ShowGump > 1)
                AddButton(645 + xoffset + 30, 450, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0);
            else
                if (m_ShowGump > 0)
                AddButton(315 + xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);
            else
                AddButton(285 + xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);

            // add the textentry extender button
            if (xoffset > 0)
            {
                AddButton(160, 365, 0x15E3, 0x15E7, 201, GumpButtonType.Reply, 0);
            }
            else
            {
                AddButton(160, 365, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0);
            }


            for (int i = 0; i < MaxSpawnEntries; i++)
            {
                if (page != i / MaxEntriesPerPage) continue;

                string str = string.Empty;
                int texthue = 0;
                int background = 0xBBC;

                if (i % MaxEntriesPerPage == 0)
                {
                    //AddPage(page+1);
                    // add highlighted page button
                    AddImageTiled(35 + page * 25, 363, 25, 25, 0xBBC);
                    AddImage(38 + page * 25, 365, 0x8B1 + page);
                }

                if (i < m_Spawner.SpawnObjects.Length)
                {
                    // disable button

                    if (m_Spawner.SpawnObjects[i].Disabled)
                    {
                        // change the background for the spawn text entry if disabled
                        background = 0x23F4;
                        AddButton(2, 22 * (i % MaxEntriesPerPage) + 34, 0x82C, 0x82C, 6000 + i, GumpButtonType.Reply, 0);
                    }
                    else
                    {
                        AddButton(2, 22 * (i % MaxEntriesPerPage) + 36, 0x837, 0x837, 6000 + i, GumpButtonType.Reply, 0);
                    }
                }

                bool hasreplacement = false;

                // check for replacement entries
                if (Rentry != null && Rentry.Index == i)
                {
                    hasreplacement = true;
                    str = Rentry.Typename;
                    background = Rentry.Color;
                    // replacement is one time only.
                    Rentry = null;
                }

                // increment/decrement buttons
                AddButton(15, 22 * (i % MaxEntriesPerPage) + 34, 0x15E0, 0x15E4, 6 + (i * 2), GumpButtonType.Reply, 0);
                AddButton(30, 22 * (i % MaxEntriesPerPage) + 34, 0x15E2, 0x15E6, 7 + (i * 2), GumpButtonType.Reply, 0);

                // categorization gump button
                AddButton(171 + xoffset - 18, 22 * (i % MaxEntriesPerPage) + 34, 0x15E1, 0x15E5, 5000 + i, GumpButtonType.Reply, 0);

                // goto spawn button
                AddButton(171 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 0xFAE, 0xFAF, 1300 + i, GumpButtonType.Reply, 0);

                // text entry gump button
                AddButton(200 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 0xFAB, 0xFAD, 800 + i, GumpButtonType.Reply, 0);

                // background for text entry area
                AddImageTiled(48, 22 * (i % MaxEntriesPerPage) + 30, 133 + xoffset - 25, 23, 0x52);
                AddImageTiled(49, 22 * (i % MaxEntriesPerPage) + 31, 131 + xoffset - 25, 21, background);

                if (i < m_Spawner.SpawnObjects.Length)
                {
                    if (!hasreplacement)
                    {
                        str = m_Spawner.SpawnObjects[i].TypeName;
                    }

                    int count = m_Spawner.SpawnObjects[i].SpawnedObjects.Count;
                    int max = m_Spawner.SpawnObjects[i].ActualMaxCount;
                    int subgrp = m_Spawner.SpawnObjects[i].SubGroup;
                    int spawnsper = m_Spawner.SpawnObjects[i].SpawnsPerTick;

                    texthue = subgrp * 11;
                    if (texthue < 0) texthue = 0;

                    // Add current count
                    AddImageTiled(231 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 35, 23, 0x52);
                    AddImageTiled(232 + xoffset, 22 * (i % MaxEntriesPerPage) + 31, 32, 21, 0xBBC);
                    AddLabel(233 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 68, count.ToString());

                    // Add maximum count
                    AddImageTiled(267 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 35, 23, 0x52);
                    AddImageTiled(268 + xoffset, 22 * (i % MaxEntriesPerPage) + 31, 32, 21, 0xBBC);
                    // AddTextEntry(x,y,w,ht,color,id,str)
                    AddTextEntry(270 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 30, 30, 33, 500 + i, max.ToString());

                    if (m_ShowGump > 0)
                    {
                        // Add subgroup
                        AddImageTiled(334 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 25, 23, 0x52);
                        AddImageTiled(335 + xoffset, 22 * (i % MaxEntriesPerPage) + 31, 22, 21, 0xBBC);
                        AddTextEntry(338 + xoffset, 22 * (i % MaxEntriesPerPage) + 30, 17, 33, texthue, 600 + i, subgrp.ToString());
                    }
                    if (m_ShowGump > 1)
                    {
                        // Add subgroup timer fields
                        string strrst = null;
                        string strto = null;
                        string strkill = null;
                        string strmind = null;
                        string strmaxd = null;
                        string strpackrange = null;
                        string strspawnsper = spawnsper.ToString();

                        if (m_Spawner.SpawnObjects[i].SequentialResetTime > 0 && m_Spawner.SpawnObjects[i].SubGroup > 0)
                        {
                            strrst = m_Spawner.SpawnObjects[i].SequentialResetTime.ToString();
                            strto = m_Spawner.SpawnObjects[i].SequentialResetTo.ToString();
                        }
                        if (m_Spawner.SpawnObjects[i].KillsNeeded > 0)
                        {
                            strkill = m_Spawner.SpawnObjects[i].KillsNeeded.ToString();
                        }

                        if (m_Spawner.SpawnObjects[i].MinDelay >= 0)
                        {
                            strmind = m_Spawner.SpawnObjects[i].MinDelay.ToString();
                        }

                        if (m_Spawner.SpawnObjects[i].MaxDelay >= 0)
                        {
                            strmaxd = m_Spawner.SpawnObjects[i].MaxDelay.ToString();
                        }

                        if (m_Spawner.SpawnObjects[i].PackRange >= 0)
                        {
                            strpackrange = m_Spawner.SpawnObjects[i].PackRange.ToString();
                        }

                        string strnext;
                        if (m_Spawner.SpawnObjects[i].NextSpawn > DateTime.UtcNow)
                        {
                            // if the next spawn tick of the spawner will occur after the subgroup is available for spawning
                            // then report the next spawn tick since that is the earliest that the subgroup can actually be spawned
                            if ((DateTime.UtcNow + m_Spawner.NextSpawn) > m_Spawner.SpawnObjects[i].NextSpawn)
                            {
                                strnext = m_Spawner.NextSpawn.ToString();
                            }
                            else
                            {
                                // estimate the earliest the next spawn could occur as the first spawn tick after reaching the subgroup nextspawn 
                                strnext = (m_Spawner.SpawnObjects[i].NextSpawn - DateTime.UtcNow + m_Spawner.NextSpawn).ToString();
                            }
                        }
                        else
                        {
                            strnext = m_Spawner.NextSpawn.ToString();
                        }

                        int yoff = 22 * (i % MaxEntriesPerPage) + 30;

                        // spawns per tick
                        AddImageTiled(303 + xoffset, yoff, 30, 23, 0x52);
                        AddImageTiled(304 + xoffset, yoff + 1, 27, 21, 0xBBC);
                        AddTextEntry(307 + xoffset, yoff, 22, 33, texthue, 1500 + i, strspawnsper);
                        // reset time
                        AddImageTiled(329 + xoffset + 30, yoff, 35, 23, 0x52);
                        AddImageTiled(330 + xoffset + 30, yoff + 1, 32, 21, 0xBBC);
                        AddTextEntry(333 + xoffset + 30, yoff, 27, 33, texthue, 1000 + i, strrst);
                        // reset to
                        AddImageTiled(365 + xoffset + 30, yoff, 26, 23, 0x52);
                        AddImageTiled(366 + xoffset + 30, yoff + 1, 23, 21, 0xBBC);
                        AddTextEntry(369 + xoffset + 30, yoff, 18, 33, texthue, 1100 + i, strto);
                        // kills needed
                        AddImageTiled(392 + xoffset + 30, yoff, 35, 23, 0x52);
                        AddImageTiled(393 + xoffset + 30, yoff + 1, 32, 21, 0xBBC);
                        AddTextEntry(396 + xoffset + 30, yoff, 27, 33, texthue, 1200 + i, strkill);

                        // mindelay
                        AddImageTiled(428 + xoffset + 30, yoff, 41, 23, 0x52);
                        AddImageTiled(429 + xoffset + 30, yoff + 1, 38, 21, 0xBBC);
                        AddTextEntry(432 + xoffset + 30, yoff, 33, 33, texthue, 1300 + i, strmind);

                        // maxdelay
                        AddImageTiled(470 + xoffset + 30, yoff, 41, 23, 0x52);
                        AddImageTiled(471 + xoffset + 30, yoff + 1, 38, 21, 0xBBC);
                        AddTextEntry(474 + xoffset + 30, yoff, 33, 33, texthue, 1400 + i, strmaxd);

                        // packrange
                        AddImageTiled(512 + xoffset + 30, yoff, 33, 23, 0x52);
                        AddImageTiled(513 + xoffset + 30, yoff + 1, 30, 21, 0xBBC);
                        AddTextEntry(516 + xoffset + 30, yoff, 25, 33, texthue, 1600 + i, strpackrange);

                        if (m_Spawner.SequentialSpawn >= 0)
                        {
                            // restrict kills button
                            AddButton(545 + xoffset + 30, yoff, m_Spawner.SpawnObjects[i].RestrictKillsToSubgroup ? 0xD3 : 0xD2,
                                m_Spawner.SpawnObjects[i].RestrictKillsToSubgroup ? 0xD2 : 0xD3, 300 + i, GumpButtonType.Reply, 0);

                            //clear on advance button for spawn entries in subgroups that require kills
                            AddButton(565 + xoffset + 30, yoff, m_Spawner.SpawnObjects[i].ClearOnAdvance ? 0xD3 : 0xD2,
                                m_Spawner.SpawnObjects[i].ClearOnAdvance ? 0xD2 : 0xD3, 400 + i, GumpButtonType.Reply, 0);
                        }

                        // add the next spawn time
                        AddLabelCropped(590 + xoffset + 30, yoff, 70, 20, 55, strnext);
                    }
                }

                AddTextEntry(52, 22 * (i % MaxEntriesPerPage) + 31, 119 + xoffset - 25, 21, texthue, i, str);
            }
        }

        public XmlSpawner.SpawnObject[] CreateArray(RelayInfo info, Mobile from)
        {
            ArrayList SpawnObjects = new ArrayList();

            for (int i = 0; i < MaxSpawnEntries; i++)
            {
                TextRelay te = info.GetTextEntry(i);

                if (te != null)
                {
                    string str = te.Text;

                    if (str.Length > 0)
                    {
                        str = str.Trim();
#if (BOOKTEXTENTRY)
                        if (i < m_Spawner.SpawnObjects.Length)
                        {
                            string currenttext = m_Spawner.SpawnObjects[i].TypeName;
                            if (currenttext != null && currenttext.Length >= 230)
                            {
                                str = currenttext;
                            }
                        }
#endif
                        string typestr = BaseXmlSpawner.ParseObjectType(str);

                        Type type = SpawnerType.GetType(typestr);

                        if (type != null)
                            SpawnObjects.Add(new XmlSpawner.SpawnObject(from, m_Spawner, str, 0));
                        else
                        {
                            // check for special keywords
                            if (typestr != null && (BaseXmlSpawner.IsTypeOrItemKeyword(typestr) || typestr.IndexOf("{") != -1 || typestr.StartsWith("*") || typestr.StartsWith("#")))
                            {
                                SpawnObjects.Add(new XmlSpawner.SpawnObject(from, m_Spawner, str, 0));
                            }
                            else
                                m_Spawner.status_str = string.Format("{0} is not a valid type name.", str);
                            //from.SendMessage( "{0} is not a valid type name.", str );
                        }

                    }
                }
            }

            return (XmlSpawner.SpawnObject[])SpawnObjects.ToArray(typeof(XmlSpawner.SpawnObject));
        }

        public void UpdateTypeNames(Mobile from, RelayInfo info)
        {
            for (int i = 0; i < MaxSpawnEntries; i++)
            {
                TextRelay te = info.GetTextEntry(i);

                if (te != null)
                {
                    string str = te.Text;

                    if (str.Length > 0)
                    {
                        str = str.Trim();
                        if (i < m_Spawner.SpawnObjects.Length)
                        {
                            // check to see if the existing typename is longer than the max textentry buffer
                            // if it is then dont update it since we will assume that the textentry has truncated the actual string
                            // that could be longer than the buffer if booktextentry is used

#if (BOOKTEXTENTRY)
                            string currentstr = m_Spawner.SpawnObjects[i].TypeName;
                            if (currentstr != null && currentstr.Length < 230)
#endif
                            {
                                if (m_Spawner.SpawnObjects[i].TypeName != str)
                                {
                                    CommandLogging.WriteLine(from, "{0} {1} changed XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7} to {8}", from.AccessLevel, CommandLogging.Format(from), m_Spawner.Serial, m_Spawner.Name, m_Spawner.GetWorldLocation().X, m_Spawner.GetWorldLocation().Y, m_Spawner.Map, m_Spawner.SpawnObjects[i].TypeName, str);

                                }

                                m_Spawner.SpawnObjects[i].TypeName = str;
                            }
                        }
                    }
                }
            }
        }

        public static void Refresh_Callback(object state)
        {
            object[] args = (object[])state;
            Mobile m = (Mobile)args[0];
            // refresh the spawner gumps			
            RefreshSpawnerGumps(m);
        }

        public static void RefreshSpawnerGumps(Mobile from)
        {
            if (from == null) return;

            NetState ns = from.NetState;

            if (ns?.Gumps != null)
            {
                ArrayList refresh = new ArrayList();

                foreach (Gump g in ns.Gumps)
                {
                    // clear the gump status on the spawner associated with the gump
                    if (g is XmlSpawnerGump xg && xg.m_Spawner != null)
                    {
                        refresh.Add(xg);
                    }
                }

                // close all of the currently opened spawner gumps
                from.CloseGump(typeof(XmlSpawnerGump));

                // reopen the closed gumps from the gump collection
                foreach (XmlSpawnerGump g in refresh)
                {
                    // reopen a new gump for the spawner
                    if (g.m_Spawner != null)
                    {
                        // flag the current gump on the spawner as closed
                        g.m_Spawner.GumpReset = true;

                        XmlSpawnerGump xg = new XmlSpawnerGump(g.m_Spawner, g.X, g.Y, g.m_ShowGump, g.xoffset, g.page, g.Rentry);

                        from.SendGump(xg);
                    }
                }
            }
        }

        private static bool ValidGotoObject(Mobile from, object o)
        {
            if (o is Item i)
            {
                if (!i.Deleted && (i.Map != null) && (i.Map != Map.Internal))
                    return true;

                if (from != null && !from.Deleted)
                {
                    from.SendMessage("{0} is not available", i);
                }
            }
            else if (o is Mobile m)
            {
                if (!m.Deleted && (m.Map != null) && (m.Map != Map.Internal))
                    return true;

                if (from != null && !from.Deleted)
                {
                    from.SendMessage("{0} is not available", m);
                }
            }

            return false;
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_Spawner == null || m_Spawner.Deleted || state == null || info == null)
            {
                if (m_Spawner != null) m_Spawner.SpawnerGump = null;
                return;
            }

            // Get the current name
            TextRelay tr = info.GetTextEntry(999);
            if (tr != null)
            {
                m_Spawner.Name = tr.Text;
            }

            // update typenames of the spawn objects based upon the current text entry strings
            UpdateTypeNames(state.Mobile, info);

            // Update the creature list
            m_Spawner.SpawnObjects = CreateArray(info, state.Mobile);

            if (m_Spawner.SpawnObjects == null)
            {
                m_Spawner.SpawnerGump = null;
                return;
            }

            for (int i = 0; i < m_Spawner.SpawnObjects.Length; i++)
            {
                if (page != i / MaxEntriesPerPage) continue;

                // check the max count entry
                TextRelay temcnt = info.GetTextEntry(500 + i);
                if (temcnt != null)
                {
                    int maxval = 0;
                    try { maxval = Convert.ToInt32(temcnt.Text, 10); }
                    catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                    if (maxval < 0) maxval = 0;

                    m_Spawner.SpawnObjects[i].MaxCount = maxval;
                }

                if (m_ShowGump > 0)
                {
                    // check the subgroup entry
                    TextRelay tegrp = info.GetTextEntry(600 + i);
                    if (tegrp != null)
                    {
                        int grpval = 0;
                        try { grpval = Convert.ToInt32(tegrp.Text, 10); }
                        catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                        if (grpval < 0) grpval = 0;

                        m_Spawner.SpawnObjects[i].SubGroup = grpval;
                    }
                }

                if (m_ShowGump > 1)
                {
                    // note, while these values can be entered in any spawn entry, they will only be assigned to the subgroup leader
                    int subgroupindex = m_Spawner.GetCurrentSequentialSpawnIndex(m_Spawner.SpawnObjects[i].SubGroup);
                    TextRelay tegrp;

                    if (subgroupindex >= 0 && subgroupindex < m_Spawner.SpawnObjects.Length)
                    {
                        // check the *reset time* entry
                        tegrp = info.GetTextEntry(1000 + i);
                        if (tegrp?.Text != null && tegrp.Text.Length > 0)
                        {
                            double grpval = 0;
                            try { grpval = Convert.ToDouble(tegrp.Text); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = 0;

                            m_Spawner.SpawnObjects[i].SequentialResetTime = 0;

                            m_Spawner.SpawnObjects[subgroupindex].SequentialResetTime = grpval;
                        }
                        // check the *reset to* entry
                        tegrp = info.GetTextEntry(1100 + i);
                        if (tegrp?.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 0;
                            try { grpval = Convert.ToInt32(tegrp.Text, 10); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = 0;

                            m_Spawner.SpawnObjects[subgroupindex].SequentialResetTo = grpval;
                        }
                        // check the kills entry
                        tegrp = info.GetTextEntry(1200 + i);
                        if (tegrp?.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 0;
                            try { grpval = Convert.ToInt32(tegrp.Text, 10); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = 0;

                            m_Spawner.SpawnObjects[subgroupindex].KillsNeeded = grpval;
                        }
                    }

                    // check the mindelay
                    tegrp = info.GetTextEntry(1300 + i);
                    if (tegrp != null)
                    {
                        if (!string.IsNullOrEmpty(tegrp.Text))
                        {
                            double grpval = -1;
                            try { grpval = Convert.ToDouble(tegrp.Text); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != m_Spawner.SpawnObjects[i].MinDelay)
                            {
                                m_Spawner.SpawnObjects[i].MinDelay = grpval;
                                m_Spawner.RefreshNextSpawnTime(m_Spawner.SpawnObjects[i]);
                            }
                        }
                        else
                        {
                            m_Spawner.SpawnObjects[i].MinDelay = -1;
                            m_Spawner.SpawnObjects[i].MaxDelay = -1;
                            m_Spawner.RefreshNextSpawnTime(m_Spawner.SpawnObjects[i]);
                        }
                    }

                    // check the maxdelay
                    tegrp = info.GetTextEntry(1400 + i);
                    if (tegrp != null)
                    {
                        if (!string.IsNullOrEmpty(tegrp.Text))
                        {
                            double grpval = -1;
                            try { grpval = Convert.ToDouble(tegrp.Text); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != m_Spawner.SpawnObjects[i].MaxDelay)
                            {
                                m_Spawner.SpawnObjects[i].MaxDelay = grpval;
                                m_Spawner.RefreshNextSpawnTime(m_Spawner.SpawnObjects[i]);
                            }
                        }
                        else
                        {
                            m_Spawner.SpawnObjects[i].MinDelay = -1;
                            m_Spawner.SpawnObjects[i].MaxDelay = -1;
                            m_Spawner.RefreshNextSpawnTime(m_Spawner.SpawnObjects[i]);
                        }
                    }

                    // check the spawns per tick
                    tegrp = info.GetTextEntry(1500 + i);
                    if (tegrp != null)
                    {
                        if (!string.IsNullOrEmpty(tegrp.Text))
                        {
                            int grpval = 1;
                            try { grpval = int.Parse(tegrp.Text); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = 1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != m_Spawner.SpawnObjects[i].SpawnsPerTick)
                            {
                                m_Spawner.SpawnObjects[i].SpawnsPerTick = grpval;
                            }
                        }
                        else
                        {
                            m_Spawner.SpawnObjects[i].SpawnsPerTick = 1;
                        }
                    }

                    // check the packrange
                    tegrp = info.GetTextEntry(1600 + i);
                    if (tegrp != null)
                    {
                        if (!string.IsNullOrEmpty(tegrp.Text))
                        {
                            int grpval = 1;
                            try { grpval = int.Parse(tegrp.Text); }
                            catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                            if (grpval < 0) grpval = 1;

                            // if this value has changed, then update 
                            if (grpval != m_Spawner.SpawnObjects[i].PackRange)
                            {
                                m_Spawner.SpawnObjects[i].PackRange = grpval;
                            }
                        }
                        else
                        {
                            m_Spawner.SpawnObjects[i].PackRange = -1;
                        }
                    }
                }
            }

            // Update the maxcount
            TextRelay temax = info.GetTextEntry(300);
            if (temax != null)
            {
                int maxval = 0;
                try { maxval = Convert.ToInt32(temax.Text, 10); }
                catch (Exception e) { Diagnostics.ExceptionLogging.LogException(e); }
                if (maxval < 0) maxval = 0;
                // if the maxcount of the spawner has been altered external to the interface (e.g. via props, or by the running spawner itself
                // then that change will override the text entry
                if (m_Spawner.MaxCount == initial_maxcount)
                {
                    m_Spawner.MaxCount = maxval;
                }
            }

            switch (info.ButtonID)
            {
                case 0: // Close
                    {
                        // clear any text entry books
                        m_Spawner.DeleteTextEntryBook();
                        // and reset the gump status
                        m_Spawner.GumpReset = true;

                        return;
                    }
                case 1: // Help
                    {
                        break;
                    }
                case 2: // Bring everything home
                    {
                        m_Spawner.BringToHome();
                        break;
                    }
                case 3: // Complete respawn
                    {
                        m_Spawner.Respawn();
                        //m_Spawner.AdvanceSequential();
                        m_Spawner.m_killcount = 0;
                        break;
                    }
                case 4: // Goto
                    {
                        state.Mobile.Location = m_Spawner.Location;
                        state.Mobile.Map = m_Spawner.Map;
                        break;
                    }
                case 200: // gump extension
                {
                    state.Mobile.SendGump(m_ShowGump > 1
                        ? new XmlSpawnerGump(m_Spawner, X, Y, -1, xoffset, page)
                        : new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump + 2, xoffset, page));
                    return;
                }
                case 201: // gump text extension
                {
                    state.Mobile.SendGump(xoffset > 0
                        ? new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, 0, page)
                        : new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, 250, page));
                    return;
                }
                case 700: // Start/stop spawner
                {
                    m_Spawner.Running = !m_Spawner.Running;
                    break;
                }
                case 701: // Complete reset
                    {
                        m_Spawner.Reset();
                        break;
                    }
                case 702: // Sort spawns
                    {
                        m_Spawner.SortSpawns();
                        break;
                    }
                case 900: // empty the status string
                    {
                        m_Spawner.status_str = "";
                        break;
                    }
                case 9997:
                    {
                        m_Spawner.DisableGlobalAutoReset = !m_Spawner.DisableGlobalAutoReset;
                        break;
                    }
                case 9998:  // refresh the gump
                    {
                        state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page));
                        return;
                    }
                case 9999:
                    {
                        // Show the props window for the spawner, as well as a new gump
                        state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page));

                        state.Mobile.SendGump(new XmlPropertiesGump(state.Mobile, m_Spawner));
                        return;
                    }
                default:
                    {
                        // check the restrict kills flag
                        if (info.ButtonID >= 300 && info.ButtonID < 300 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 300;
                            if (index < m_Spawner.SpawnObjects.Length)
                                m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup = !m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup;

                        }
                        else if (info.ButtonID >= 400 && info.ButtonID < 400 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 400;
                            if (index < m_Spawner.SpawnObjects.Length)
                                m_Spawner.SpawnObjects[index].ClearOnAdvance = !m_Spawner.SpawnObjects[index].ClearOnAdvance;
                        }
                        else if (info.ButtonID >= 800 && info.ButtonID < 800 + MaxSpawnEntries)
                        {
                            // open the text entry gump
                            int index = info.ButtonID - 800;
                            // open a text entry gump
#if (BOOKTEXTENTRY)
                            // display a new gump
                            XmlSpawnerGump newgump = new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page);
                            state.Mobile.SendGump(newgump);

                            // is there an existing book associated with the gump?
                            if (m_Spawner.m_TextEntryBook == null)
                            {
                                m_Spawner.m_TextEntryBook = new List<XmlTextEntryBook>();
                            }

                            object[] args = new object[6];

                            args[0] = m_Spawner;
                            args[1] = index;
                            args[2] = X;
                            args[3] = Y;
                            args[4] = m_ShowGump;
                            args[5] = page;

                            XmlTextEntryBook book = new XmlTextEntryBook(0, string.Empty, m_Spawner.Name, 20, true);

                            m_Spawner.m_TextEntryBook.Add(book);

                            book.Title = string.Format("Entry {0}", index);
                            book.Author = m_Spawner.Name;

                            // fill the contents of the book with the current text entry data
                            string text = string.Empty;
                            if (m_Spawner.SpawnObjects != null && index < m_Spawner.SpawnObjects.Length)
                            {
                                text = m_Spawner.SpawnObjects[index].TypeName;
                            }
                            book.FillTextEntryBook(text);

                            // put the book at the location of the player so that it can be opened, but drop it below visible range
                            book.Visible = false;
                            book.Movable = false;
                            book.MoveToWorld(new Point3D(state.Mobile.Location.X, state.Mobile.Location.Y, state.Mobile.Location.Z - 100), state.Mobile.Map);

                            // and open it
                            book.OnDoubleClick(state.Mobile);

#else
							state.Mobile.SendGump( new TextEntryGump(m_Spawner,this, index, this.X, this.Y));
#endif
                            return;
                        }
                        else if (info.ButtonID >= 1300 && info.ButtonID < 1300 + MaxSpawnEntries)
                        {
                            nclicks++;
                            // find the location of the spawn at the specified index
                            int index = info.ButtonID - 1300;
                            if (index < m_Spawner.SpawnObjects.Length)
                            {
                                int scount = m_Spawner.SpawnObjects[index].SpawnedObjects.Count;
                                if (scount > 0)
                                {
                                    object so = m_Spawner.SpawnObjects[index].SpawnedObjects[nclicks % scount];

                                    if (ValidGotoObject(state.Mobile, so))
                                    {
                                        IPoint3D o = so as IPoint3D;

                                        if (o != null)
                                        {
                                            Map m = m_Spawner.Map;

                                            if (o is Item item)
                                                m = item.Map;

                                            if (o is Mobile mobile)
                                                m = mobile.Map;

                                            state.Mobile.Location = new Point3D(o);
                                            state.Mobile.Map = m;
                                        }
                                    }
                                }
                            }
                        }
                        else if (info.ButtonID >= 4000 && info.ButtonID < 4001 + MaxSpawnEntries / MaxEntriesPerPage)
                        {
                            // which page
                            page = info.ButtonID - 4000;

                        }
                        else if (info.ButtonID >= 6000 && info.ButtonID < 6000 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 6000;

                            if (index < m_Spawner.SpawnObjects.Length)
                            {
                                m_Spawner.SpawnObjects[index].Disabled = !m_Spawner.SpawnObjects[index].Disabled;

                                // clear any current spawns on the disabled entry
                                if (m_Spawner.SpawnObjects[index].Disabled)
                                    m_Spawner.RemoveSpawnObjects(m_Spawner.SpawnObjects[index]);
                            }
                        }
                        else if (info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxSpawnEntries)
                        {
                            int i = info.ButtonID - 5000;

                            string categorystring = null;
                            string entrystring = null;

                            TextRelay te = info.GetTextEntry(i);

                            if (te?.Text != null)
                            {
                                // get the string

                                string[] cargs = te.Text.Split(',');

                                // parse out any comma separated args
                                categorystring = cargs[0];

                                entrystring = te.Text;
                            }

                            if (string.IsNullOrEmpty(categorystring))
                            {

                                XmlSpawnerGump newg = new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page);
                                state.Mobile.SendGump(newg);

                                // if no string has been entered then just use the full categorized add gump
                                state.Mobile.CloseGump(typeof(XmlCategorizedAddGump));
                                state.Mobile.SendGump(new XmlCategorizedAddGump(state.Mobile, i, newg));
                            }
                            else
                            {
                                // use the XmlPartialCategorizedAddGump
                                state.Mobile.CloseGump(typeof(XmlPartialCategorizedAddGump));

                                //Type [] types = (Type[])XmlPartialCategorizedAddGump.Match( categorystring ).ToArray( typeof( Type ) );
                                ArrayList types = XmlPartialCategorizedAddGump.Match(categorystring);


                                ReplacementEntry re = new ReplacementEntry
                                {
                                    Typename = entrystring,
                                    Index = i,
                                    Color = 0x1436
                                };

                                XmlSpawnerGump newg = new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page, re);

                                state.Mobile.SendGump(new XmlPartialCategorizedAddGump(state.Mobile, categorystring, 0, types, true, i, newg));

                                state.Mobile.SendGump(newg);
                            }

                            return;
                        }
                        else
                        {
                            // up and down arrows
                            int buttonID = info.ButtonID - 6;
                            int index = buttonID / 2;
                            int type = buttonID % 2;

                            TextRelay entry = info.GetTextEntry(index);

                            if (entry != null && entry.Text.Length > 0)
                            {
                                string entrystr = entry.Text;

#if (BOOKTEXTENTRY)
                                if (index < m_Spawner.SpawnObjects.Length)
                                {
                                    string str = m_Spawner.SpawnObjects[index].TypeName;

                                    if (str != null && str.Length >= 230)
                                        entrystr = str;
                                }
#endif
                                if (type == 0) // Add creature
                                {
                                    m_Spawner.AddSpawnObject(entrystr);
                                }
                                else // Remove creatures
                                {
                                    m_Spawner.DeleteSpawnObject(state.Mobile, entrystr);

                                }
                            }
                        }
                        break;
                    }
            }

            state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, X, Y, m_ShowGump, xoffset, page, Rentry));
        }
    }
}
