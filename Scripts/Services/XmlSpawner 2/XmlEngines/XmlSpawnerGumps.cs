#define NEWPROPSGUMP
#define BOOKTEXTENTRY

using System;
using System.Collections;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Network;

/*
** Changelog
**
** 8/15/04
** - fixed a crash bug when using the goto spawn button on an empty spawn entry
**
** 8/10/04
** - added a goto-spawn button in the spawner gump (to the right of the text entry area, next to the text entry gump button) that will take you to the location of
** currently spawned objects for a given spawner entry.  If there are multiple spawned objects for an entry, it will cycle through them with repeated clicks.
** Useful for tracking down spawns.
** 3/23/04
** changed spawner name font color for 3dclient compatibility
*/

namespace Server.Mobiles
{
    public class HelpGump : Gump
    {
        public XmlSpawner m_Spawner;
        private readonly XmlSpawnerGump m_SpawnerGump;

        public HelpGump(XmlSpawner spawner, XmlSpawnerGump spawnergump, int X, int Y)
            : base(X, Y)
        {
            if (spawner == null || spawner.Deleted)
                return;
            this.m_Spawner = spawner;
            this.m_SpawnerGump = spawnergump;

            this.AddPage(0);

            int width = 370;

            this.AddBackground(20, 0, width, 480, 5054);

            this.AddPage(1);
            //AddAlphaRegion( 20, 0, 220, 554 );
            this.AddImageTiled(20, 0, width, 480, 0x52);
            //AddImageTiled( 24, 6, 213, 261, 0xBBC );

            this.AddLabel(27, 2, 0x384, "Standalone Keywords");
            this.AddHtml(25, 20, width - 10, 440,
                                                 "spawntype[,arg1,arg2,...]\n" +
                                                 "SET[,itemname or serialno][,itemtype]/property/value/...\n" +
                                                 "SETVAR,varname/value\n" +
                                                 "SETONMOB,mobname[,mobtype]/property/value/...\n" +
                                                 "SETONTRIGMOB/property/value/...\n" +
                                                 "SETONTHIS/property/value/...\n" +
                                                 "SETONPARENT/property/value/...\n" +
                                                 "SETONNEARBY,range,name[,type][,searchcontainers]/prop/value/prop/value...\n" +
                                                 "SETONPETS,range/prop/value/prop/value...\n" +
                                                 "SETONCARRIED,itemname[,itemtype][,equippedonly]/property/value/...\n" +
                                                 "SETONSPAWN[,spawnername],subgroup/property/value/...\n" +
                                                 "SETONSPAWNENTRY[,spawnername],entrystring/property/value/...\n" +
                                                 "SPAWN[,spawnername],subgroup\n" +
                                                 "DESPAWN[,spawnername],subgroup\n" +
                                                 "{GET or RND keywords}\n" +
                                                 "GIVE[,prob]/itemtype\n" +
                                                 "GIVE[,prob]/&lt;itemtype/property/value...>\n" +
                                                 "TAKE[,prob[,quantity[,true[,itemtype]]]]/itemname\n" +
                                                 "TAKEBYTYPE[,prob[,quantity[,true]]]/itemtype\n" +
                                                 "IF/condition/thenspawn[/elsespawn]\n" +
                                                 "WAITUNTIL[,duration][,timeout][/condition][/spawngroup]\n" +
                                                 "WHILE/condition/spawngroup\n" +
                                                 "GOTO/subgroup\n" +
                                                 "BROWSER/url\n" +
                                                 "MUSIC,musicname[,range]\n" +
                                                 "SOUND,value\n" +
                                                 "EFFECT,itemid,duration[,x,y,z]\n" +
                                                 "MEFFECT,itemid[,speed][,x,y,z][,x2,y2,z2]" +
                                                 "RESURRECT[,range][,PETS]\n" +
                                                 "POISON,level[,range][,playeronly]\n" +
                                                 "DAMAGE,dmg,phys,fire,cold,pois,energy[,range][,playeronly]\n" +
                                                 "CAST,spellname[,arg] or CAST,spellnumber[,arg]\n" +
                                                 "SENDMSG/text\n" +
                                                 "BCAST[,hue][,font]/text\n" +
                                                 "GUMP,title,number[,gumpconstructor]/text",
                false, true);
            this.AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 2);
            this.AddLabel(width - 38, 2, 0x384, "1");
            this.AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 4);

            this.AddPage(2);
            this.AddLabel(27, 2, 0x384, "Value and Itemtype Keywords");
            this.AddHtml(25, 20, width - 10, 440,
                                                 "property/@value\n" +
                                                 "ARMOR,minlevel,maxlevel\n" +
                                                 "WEAPON,minlevel,maxlevel\n" +
                                                 "JARMOR,minlevel,maxlevel\n" +
                                                 "JWEAPON,minlevel,maxlevel\n" +
                                                 "JEWELRY,minlevel,maxlevel\n" +
                                                 "SARMOR,minlevel,maxlevel\n" +
                                                 "SHIELD,minlevel,maxlevel\n" +
                                                 "POTION\n" +
                                                 "SCROLL,mincircle,maxcircle\n" +
                                                 "NECROSCROLL,index\n" +
                                                 "LOOT,methodname\n" +
                                                 "LOOTPACK,loottype\n" +
                                                 "MOB,name[,mobtype]\n" +
                                                 "TRIGMOB\n" +
                                                 "TAKEN\n" +
                                                 "GIVEN\n" +
                                                 "GET,itemname or serialno[,itemtype],property\n" +
                                                 "GETVAR,varname\n" +
                                                 "GETONCARRIED,itemname[,itemtype][,equippedonly],property\n" +
                                                 "GETONNEARBY,range,name[,type][,searchcontainers],property\n" +
                                                 "GETONMOB,mobname[,mobtype],property\n" +
                                                 "GETONGIVEN,property\n" +
                                                 "GETONTAKEN,property\n" +
                                                 "GETONPARENT,property\n" +
                                                 "GETONSPAWN[,spawnername],subgroup,property\n" +
                                                 "GETONSPAWN[,spawnername],subgroup,COUNT\n" +
                                                 "GETONTHIS,property\n" +
                                                 "GETONTRIGMOB,property\n" +
                                                 "GETONATTACH,type[,name],property\n" +
                                                 "...&lt;ATTACHMENT,type,name,property> as GET property\n" +
                                                 "{GET or RND keywords}\n" +
                                                 "RND,min,max\n" +
                                                 "RNDLIST,int1[,int2,...]\n" +
                                                 "RNDSTRLIST,str1[,str2,...]\n" +
                                                 "RNDBOOL\n" +
                                                 "RANDNAME,nametype\n" +
                                                 "PLAYERSINRANGE,range\n" +
                                                 "AMOUNTCARRIED,itemtype\n" +
                                                 "OFFSET,x,y,[,z]\n" +
                                                 "ANIMATE,action[,nframes][,nrepeat][,forward][,repeat][delay]\n" +
                                                 "MSG[,prob]/text\n" +
                                                 "SENDASCIIMSG[,probability][,hue][,font/text\n" +
                                                 "SENDMSG[,probability][,hue]/text\n" +
                                                 "SAY[,prob]/text\n" +
                                                 "SKILL,skillname\n" +
                                                 "TRIGSKILL,name|value|base|cap\n" +
                                                 "MUSIC,musicname[,range]\n" +
                                                 "SOUND,value\n" +
                                                 "EFFECT,itemid,duration[,x,y,z]\n" +
                                                 "MEFFECT,itemid[,speed][,x,y,z]" +
                                                 "POISON,level[,range][,playeronly]\n" +
                                                 "DAMAGE,dmg,phys,fire,cold,pois,energy[,range][,playeronly]\n" +
                                                 "INC,value or INC,min,max\n" +
                                                 "MUL,value or MUL,min,max\n" +
                                                 "ATTACH[,prob]/attachmenttype[,args]\n" +
                                                 "ATTACH[,prob]/&lt;attachmenttype[,args]/property/value...>\n" +
                                                 "ADD[,prob]/itemtype[,args]\n" +
                                                 "ADD[,prob]/&lt;itemtype[,args]/property/value...>\n" +
                                                 "DELETE\n" +
                                                 "KILL\n" +
                                                 "UNEQUIP,layer[,delete]\n" +
                                                 "EQUIP[,prob]/itemtype[,args]\n" +
                                                 "EQUIP[,prob]/&lt;itemtype[,args]/property/value...>",
                false, true);
            this.AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 3);
            this.AddLabel(width - 41, 2, 0x384, "2");
            this.AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 1);

            this.AddPage(3);
            this.AddLabel(27, 2, 0x384, "[ Commands");

            this.AddHtml(25, 20, width - 10, 440,
                                                 "XmlAdd [-defaults]\n" +
                                                 "XmlShow\n" +
                                                 "XmlHide\n" +
                                                 "XmlFind\n" +
                                                 "AddAtt type [args]\n" +
                                                 "GetAtt [type]\n" +
                                                 "DelAtt [type][serialno]\n" +
                                                 "AvailAtt\n" +
                                                 "SmartStat [accesslevel AccessLevel]\n" +
                                                 "OptimalSmartSpawning [maxdiff]\n" +
                                                 "XmlSpawnerWipe [prefix]\n" +
                                                 "XmlSpawnerWipeAll [prefix]\n" +
                                                 "XmlSpawnerRespawn [prefix]\n" +
                                                 "XmlSpawnerRespawnAll [prefix]\n" +
                                                 "XmlHome [go][gump][send]\n" +
                                                 "XmlUnLoad filename [prefix]\n" +
                                                 "XmlLoad filename [prefix]\n" +
                                                 "XmlLoadHere filename [prefix][-maxrange range]\n" +
                                                 "XmlNewLoad filename [prefix]\n" +
                                                 "XmlNewLoadHere filename [prefix][-maxrange range]\n" +
                                                 "XmlSave filename [prefix]\n" +
                                                 "XmlSaveAll filename [prefix]\n" +
                                                 "XmlSaveOld filename [prefix]\n" +
                                                 "XmlSpawnerSaveAll filename [prefix]\n" +
                                                 "XmlImportSpawners filename\n" +
                                                 "XmlImportMap filename\n" +
                                                 "XmlImportMSF filename\n" +
                                                 "XmlDefaults [defaultpropertyname value]\n" +
                                                 "XmlGet property\n" +
                                                 "XmlSet property value",
                false, true);

            this.AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 4);
            this.AddLabel(width - 41, 2, 0x384, "3");
            this.AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 2);

            this.AddPage(4);
            this.AddLabel(27, 2, 0x384, "Quest types");
            this.AddHtml(25, 20, width - 10, 180,
                                                 "KILL,mobtype[,count][,proptest]\n" +
                                                 "KILLNAMED,mobname[,type][,count][,proptest]\n" +
                                                 "GIVE,mobname,itemtype[,count][,proptest]\n" +
                                                 "GIVENAMED,mobname,itemname[,type][,count][,proptest]\n" +
                                                 "COLLECT,itemtype[,count][,proptest]\n" +
                                                 "COLLECTNAMED,itemname[,itemtype][,count][,proptest]\n" +
                                                 "ESCORT[,mobname][,proptest]\n",
                false, true);

            this.AddLabel(27, 200, 0x384, "Trigger/NoTriggerOnCarried");
            this.AddHtml(25, 220, width - 10, 50,
                                                 "ATTACHMENT,name,type\n" +
                                                 "itemname[,type][,EQUIPPED][,objective#,objective#,...]\n",
                false, true);

            this.AddLabel(27, 300, 0x384, "GUMPITEMS");
            this.AddHtml(25, 320, width - 10, 150,
                                                  "BUTTON,gumpid,x,y\n" +
                                                  "HTML,x,y,width,height,text\n" +
                                                  "IMAGE,gumpid,x,y[,hue]\n" +
                                                  "IMAGETILED,gumpid,x,y,width,height\n" +
                                                  "ITEM,itemid,x,y[,hue]\n" +
                                                  "LABEL,x,y,labelstring[,labelcolor]\n" +
                                                  "RADIO,gumpid1,gumpid2,x,y[,initialstate]\n" +
                                                  "TEXTENTRY,x,y,width,height[,text][,textcolor]\n",
                false, true);

            this.AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 1);
            this.AddLabel(width - 41, 2, 0x384, "4");
            this.AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 3);
        }
    }

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
            this.m_Spawner = spawner;
            this.m_index = index;
            this.m_SpawnerGump = spawnergump;

            this.AddPage(0);

            this.AddBackground(20, 0, 220, 354, 5054);
            this.AddAlphaRegion(20, 0, 220, 354);
            this.AddImageTiled(23, 5, 214, 270, 0x52);
            this.AddImageTiled(24, 6, 213, 261, 0xBBC);

            string label = spawner.Name + " entry " + index;
            this.AddLabel(28, 10, 0x384, label);

            // OK button
            this.AddButton(25, 325, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0);
            // Close button
            this.AddButton(205, 325, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0);
            // Edit button
            this.AddButton(100, 325, 0xEF, 0xEE, 2, GumpButtonType.Reply, 0);
            string str = null;
            if (index < this.m_Spawner.SpawnObjects.Length)
            {
                str = (string)this.m_Spawner.SpawnObjects[index].TypeName;
            }
            // main text entry area
            this.AddTextEntry(35, 30, 200, 251, 0, 0, str);

            // editing text entry areas
            // background for text entry area
            this.AddImageTiled(23, 275, 214, 23, 0x52);
            this.AddImageTiled(24, 276, 213, 21, 0xBBC);
            this.AddImageTiled(23, 300, 214, 23, 0x52);
            this.AddImageTiled(24, 301, 213, 21, 0xBBC);

            this.AddTextEntry(35, 275, 200, 21, 0, 1, null);
            this.AddTextEntry(35, 300, 200, 21, 0, 2, null);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info == null || state == null || state.Mobile == null)
                return;

            if (this.m_Spawner == null || this.m_Spawner.Deleted)
                return;
            bool update_entry = false;
            bool edit_entry = false;
            switch (info.ButtonID)
            {
                case 0: // Close
                    {
                        update_entry = false;
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

                            if (this.m_index < this.m_Spawner.SpawnObjects.Length)
                            {
                                this.m_Spawner.SpawnObjects[this.m_index].TypeName = editedtext;
                            }
                            else
                            {
                                // Update the creature list
                                this.m_Spawner.SpawnObjects = this.m_SpawnerGump.CreateArray(info, state.Mobile);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
                // open a new text entry gump
                state.Mobile.SendGump(new TextEntryGump(this.m_Spawner, this.m_SpawnerGump, this.m_index, this.X, this.Y));
                return;
            }
            if (update_entry)
            {
                TextRelay entry = info.GetTextEntry(0);
                if (this.m_index < this.m_Spawner.SpawnObjects.Length)
                {
                    this.m_Spawner.SpawnObjects[this.m_index].TypeName = entry.Text;
                }
                else
                {
                    // Update the creature list
                    this.m_Spawner.SpawnObjects = this.m_SpawnerGump.CreateArray(info, state.Mobile);
                }
            }
            // Create a new gump

            //m_Spawner.OnDoubleClick( state.Mobile);
            // open a new spawner gump
            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_SpawnerGump.m_ShowGump, this.m_SpawnerGump.xoffset, this.m_SpawnerGump.page));
        }
    }

    public class XmlSpawnerGump : Gump
    {
        private static int nclicks = 0;
        public XmlSpawner m_Spawner;
        public const int MaxSpawnEntries = 60;
        private const int MaxEntriesPerPage = 15;
        public int m_ShowGump = 0;
        public bool AllowGumpUpdate = true;
        public int xoffset = 0;
        public int initial_maxcount;
        public int page;
        public ReplacementEntry Rentry;

        public class ReplacementEntry
        {
            public string Typename;
            public int Index;
            public int Color;

            public ReplacementEntry()
            {
            }
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

            this.m_Spawner = spawner;
            spawner.SpawnerGump = this;
            this.xoffset = textextension;
            this.initial_maxcount = spawner.MaxCount;
            this.page = newpage;
            this.Rentry = rentry;

            this.AddPage(0);

            // automatically change the gump depending on whether sequential spawning and/or subgroups are activated

            if (spawner.SequentialSpawn >= 0 || spawner.HasSubGroups() || spawner.HasIndividualSpawnTimes())
            {
                // show the fully extended gump with subgroups and reset timer info
                this.m_ShowGump = 2;
            }
            /*
            else
            if(spawner.HasSubGroups() || spawner.SequentialSpawn >= 0)
            {
            // show partially extended gump with subgroups
            m_ShowGump = 1;
            }
            */

            if (extension > 0)
            {
                this.m_ShowGump = extension;
            }
            if (extension < 0)
            {
                this.m_ShowGump = 0;
            }

            // if the expanded gump toggle has been activated then override the auto settings.

            if (this.m_ShowGump > 1)
            {
                this.AddBackground(0, 0, 670 + this.xoffset + 30, 474, 5054);
                this.AddAlphaRegion(0, 0, 670 + this.xoffset + 30, 474);
            }
            else if (this.m_ShowGump > 0)
            {
                this.AddBackground(0, 0, 335 + this.xoffset, 474, 5054);
                this.AddAlphaRegion(0, 0, 335 + this.xoffset, 474);
            }
            else
            {
                this.AddBackground(0, 0, 305 + this.xoffset, 474, 5054);
                this.AddAlphaRegion(0, 0, 305 + this.xoffset, 474);
            }

            // spawner name area
            this.AddImageTiled(3, 5, 227, 23, 0x52);
            this.AddImageTiled(4, 6, 225, 21, 0xBBC);
            this.AddTextEntry(6, 5, 222, 21, 0, 999, spawner.Name); // changed from color 50

            this.AddButton(5, 450, 0xFAE, 0xFAF, 4, GumpButtonType.Reply, 0);
            this.AddLabel(38, 450, 0x384, "Goto");

            //AddButton( 5, 428, 0xFB7, 0xFB9, 1, GumpButtonType.Reply, 0 );
            this.AddButton(5, 428, 0xFAE, 0xFAF, 1, GumpButtonType.Reply, 0);
            this.AddLabel(38, 428, 0x384, "Help");

            this.AddButton(80, 428, 0xFB4, 0xFB6, 2, GumpButtonType.Reply, 0);
            this.AddLabel(113, 428, 0x384, "Bring Home");

            this.AddButton(80, 450, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
            this.AddLabel(113, 450, 0x384, "Respawn");

            // Props button
            this.AddButton(200, 428, 0xFAB, 0xFAD, 9999, GumpButtonType.Reply, 0);
            this.AddLabel(233, 428, 0x384, "Props");

            // Sort button
            this.AddButton(200, 450, 0xFAB, 0xFAD, 702, GumpButtonType.Reply, 0);
            this.AddLabel(233, 450, 0x384, "Sort");

            // Reset button
            this.AddButton(80, 406, 0xFA2, 0xFA3, 701, GumpButtonType.Reply, 0);
            this.AddLabel(113, 406, 0x384, "Reset");

            // Refresh button
            this.AddButton(200, 406, 0xFBD, 0xFBE, 9998, GumpButtonType.Reply, 0);
            this.AddLabel(233, 406, 0x384, "Refresh");

            // add run status display
            if (this.m_Spawner.Running)
            {
                this.AddButton(5, 399, 0x2A4E, 0x2A3A, 700, GumpButtonType.Reply, 0);
                this.AddLabel(38, 406, 0x384, "On");
            }
            else
            {
                this.AddButton(5, 399, 0x2A62, 0x2A3A, 700, GumpButtonType.Reply, 0);
                this.AddLabel(38, 406, 0x384, "Off");
            }

            // Add sequential spawn state
            if (this.m_Spawner.SequentialSpawn >= 0)
            {
                this.AddLabel(15, 365, 33, String.Format("{0}", this.m_Spawner.SequentialSpawn));
            }

            // Add Current / Max count labels
            this.AddLabel(231 + this.xoffset, 9, 68, "Count");
            this.AddLabel(270 + this.xoffset, 9, 33, "Max");

            if (this.m_ShowGump > 0)
            {
                // Add subgroup label
                this.AddLabel(334 + this.xoffset, 9, 68, "Sub");
            }
            if (this.m_ShowGump > 1)
            {
                // Add entry field labels
                this.AddLabel(303 + this.xoffset, 9, 68, "Per");
                this.AddLabel(329 + this.xoffset + 30, 9, 68, "Reset");
                this.AddLabel(368 + this.xoffset + 30, 9, 68, "To");
                this.AddLabel(392 + this.xoffset + 30, 9, 68, "Kills");
                this.AddLabel(432 + this.xoffset + 30, 9, 68, "MinD");
                this.AddLabel(472 + this.xoffset + 30, 9, 68, "MaxD");
                this.AddLabel(515 + this.xoffset + 30, 9, 68, "Rng");
                this.AddLabel(545 + this.xoffset + 30, 9, 68, "RK");
                this.AddLabel(565 + this.xoffset + 30, 9, 68, "Clr");
                this.AddLabel(590 + this.xoffset + 30, 9, 68, "NextSpawn");
            }

            // add area for spawner max
            this.AddLabel(180 + this.xoffset, 365, 50, "Spawner");
            this.AddImageTiled(267 + this.xoffset, 365, 35, 23, 0x52);
            this.AddImageTiled(268 + this.xoffset, 365, 32, 21, 0xBBC);
            this.AddTextEntry(273 + this.xoffset, 365, 33, 33, 33, 300, this.m_Spawner.MaxCount.ToString());

            // add area for spawner count
            this.AddImageTiled(231 + this.xoffset, 365, 35, 23, 0x52);
            this.AddImageTiled(232 + this.xoffset, 365, 32, 21, 0xBBC);
            this.AddLabel(233 + this.xoffset, 365, 68, this.m_Spawner.CurrentCount.ToString());

            // add the status string
            this.AddTextEntry(38, 384, 235, 33, 33, 900, this.m_Spawner.status_str);
            // add the page buttons
            for (int i = 0; i < (int)(MaxSpawnEntries / MaxEntriesPerPage); i++)
            {
                //AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
                this.AddButton(38 + i * 25, 365, 0x8B1 + i, 0x8B1 + i, 4000 + i, GumpButtonType.Reply, 0);
            }

            // add gump extension button
            if (this.m_ShowGump > 1)
                this.AddButton(645 + this.xoffset + 30, 450, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0);
            else if (this.m_ShowGump > 0)
                this.AddButton(315 + this.xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);
            else
                this.AddButton(285 + this.xoffset, 450, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);

            // add the textentry extender button
            if (this.xoffset > 0)
            {
                this.AddButton(160, 365, 0x15E3, 0x15E7, 201, GumpButtonType.Reply, 0);
            }
            else
            {
                this.AddButton(160, 365, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0);
            }

            for (int i = 0; i < MaxSpawnEntries; i++)
            {
                if (this.page != (int)(i / MaxEntriesPerPage))
                    continue;

                string str = String.Empty;
                int texthue = 0;
                int background = 0xBBC;

                if (i % MaxEntriesPerPage == 0)
                {
                    //AddPage(page+1);
                    // add highlighted page button
                    this.AddImageTiled(35 + this.page * 25, 363, 25, 25, 0xBBC);
                    this.AddImage(38 + this.page * 25, 365, 0x8B1 + this.page);
                }

                if (i < this.m_Spawner.SpawnObjects.Length)
                {
                    // disable button
                    if (this.m_Spawner.SpawnObjects[i].Disabled)
                    {
                        // change the background for the spawn text entry if disabled
                        background = 0x23F4;
                        this.AddButton(2, 22 * (i % MaxEntriesPerPage) + 34, 0x82C, 0x82C, 6000 + i, GumpButtonType.Reply, 0);
                    }
                    else
                    {
                        this.AddButton(2, 22 * (i % MaxEntriesPerPage) + 36, 0x837, 0x837, 6000 + i, GumpButtonType.Reply, 0);
                    }
                }

                bool hasreplacement = false;

                // check for replacement entries
                if (this.Rentry != null && this.Rentry.Index == i)
                {
                    hasreplacement = true;
                    str = this.Rentry.Typename;
                    background = this.Rentry.Color;
                    // replacement is one time only.
                    this.Rentry = null;
                }

                // increment/decrement buttons
                this.AddButton(15, 22 * (i % MaxEntriesPerPage) + 34, 0x15E0, 0x15E4, 6 + (i * 2), GumpButtonType.Reply, 0);
                this.AddButton(30, 22 * (i % MaxEntriesPerPage) + 34, 0x15E2, 0x15E6, 7 + (i * 2), GumpButtonType.Reply, 0);

                // categorization gump button
                this.AddButton(171 + this.xoffset - 18, 22 * (i % MaxEntriesPerPage) + 34, 0x15E1, 0x15E5, 5000 + i, GumpButtonType.Reply, 0);

                // goto spawn button
                this.AddButton(171 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 0xFAE, 0xFAF, 1300 + i, GumpButtonType.Reply, 0);

                // text entry gump button
                this.AddButton(200 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 0xFAB, 0xFAD, 800 + i, GumpButtonType.Reply, 0);

                // background for text entry area
                this.AddImageTiled(48, 22 * (i % MaxEntriesPerPage) + 30, 133 + this.xoffset - 25, 23, 0x52);
                this.AddImageTiled(49, 22 * (i % MaxEntriesPerPage) + 31, 131 + this.xoffset - 25, 21, background);

                // Add page number
                //AddLabel( 15, 365, 33, String.Format("{0}",(int)(i/MaxEntriesPerPage + 1)) );
                //AddButton( 38+page*25, 365, 0x8B1+i, 0x8B1+i, 0, GumpButtonType.Page, 1+i );

                if (i < this.m_Spawner.SpawnObjects.Length)
                {
                    if (!hasreplacement)
                    {
                        str = (string)this.m_Spawner.SpawnObjects[i].TypeName;
                    }

                    int count = this.m_Spawner.SpawnObjects[i].SpawnedObjects.Count;
                    int max = this.m_Spawner.SpawnObjects[i].ActualMaxCount;
                    int subgrp = this.m_Spawner.SpawnObjects[i].SubGroup;
                    int spawnsper = this.m_Spawner.SpawnObjects[i].SpawnsPerTick;

                    texthue = subgrp * 11;
                    if (texthue < 0)
                        texthue = 0;

                    // Add current count
                    this.AddImageTiled(231 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 35, 23, 0x52);
                    this.AddImageTiled(232 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 31, 32, 21, 0xBBC);
                    this.AddLabel(233 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 68, count.ToString());

                    // Add maximum count
                    this.AddImageTiled(267 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 35, 23, 0x52);
                    this.AddImageTiled(268 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 31, 32, 21, 0xBBC);
                    // AddTextEntry(x,y,w,ht,color,id,str)
                    this.AddTextEntry(270 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 30, 30, 33, 500 + i, max.ToString());

                    if (this.m_ShowGump > 0)
                    {
                        // Add subgroup
                        this.AddImageTiled(334 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 25, 23, 0x52);
                        this.AddImageTiled(335 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 31, 22, 21, 0xBBC);
                        this.AddTextEntry(338 + this.xoffset, 22 * (i % MaxEntriesPerPage) + 30, 17, 33, texthue, 600 + i, subgrp.ToString());
                    }
                    if (this.m_ShowGump > 1)
                    {
                        // Add subgroup timer fields
                        string strrst = null;
                        string strto = null;
                        string strkill = null;
                        string strmind = null;
                        string strmaxd = null;
                        string strnext = null;
                        string strpackrange = null;
                        string strspawnsper = spawnsper.ToString();

                        if (this.m_Spawner.SpawnObjects[i].SequentialResetTime > 0 && this.m_Spawner.SpawnObjects[i].SubGroup > 0)
                        {
                            strrst = this.m_Spawner.SpawnObjects[i].SequentialResetTime.ToString();
                            strto = this.m_Spawner.SpawnObjects[i].SequentialResetTo.ToString();
                        }
                        if (this.m_Spawner.SpawnObjects[i].KillsNeeded > 0)
                        {
                            strkill = this.m_Spawner.SpawnObjects[i].KillsNeeded.ToString();
                        }

                        if (this.m_Spawner.SpawnObjects[i].MinDelay >= 0)
                        {
                            strmind = this.m_Spawner.SpawnObjects[i].MinDelay.ToString();
                        }

                        if (this.m_Spawner.SpawnObjects[i].MaxDelay >= 0)
                        {
                            strmaxd = this.m_Spawner.SpawnObjects[i].MaxDelay.ToString();
                        }

                        if (this.m_Spawner.SpawnObjects[i].PackRange >= 0)
                        {
                            strpackrange = this.m_Spawner.SpawnObjects[i].PackRange.ToString();
                        }

                        if (this.m_Spawner.SpawnObjects[i].NextSpawn > DateTime.UtcNow)
                        {
                            // if the next spawn tick of the spawner will occur after the subgroup is available for spawning
                            // then report the next spawn tick since that is the earliest that the subgroup can actually be spawned
                            if ((DateTime.UtcNow + this.m_Spawner.NextSpawn) > this.m_Spawner.SpawnObjects[i].NextSpawn)
                            {
                                strnext = this.m_Spawner.NextSpawn.ToString();
                            }
                            else
                            {
                                // estimate the earliest the next spawn could occur as the first spawn tick after reaching the subgroup nextspawn 
                                strnext = (this.m_Spawner.SpawnObjects[i].NextSpawn - DateTime.UtcNow + this.m_Spawner.NextSpawn).ToString();
                            }
                        }
                        else
                        {
                            strnext = this.m_Spawner.NextSpawn.ToString();
                        }

                        int yoff = 22 * (i % MaxEntriesPerPage) + 30;

                        // spawns per tick
                        this.AddImageTiled(303 + this.xoffset, yoff, 30, 23, 0x52);
                        this.AddImageTiled(304 + this.xoffset, yoff + 1, 27, 21, 0xBBC);
                        this.AddTextEntry(307 + this.xoffset, yoff, 22, 33, texthue, 1500 + i, strspawnsper);
                        // reset time
                        this.AddImageTiled(329 + this.xoffset + 30, yoff, 35, 23, 0x52);
                        this.AddImageTiled(330 + this.xoffset + 30, yoff + 1, 32, 21, 0xBBC);
                        this.AddTextEntry(333 + this.xoffset + 30, yoff, 27, 33, texthue, 1000 + i, strrst);
                        // reset to
                        this.AddImageTiled(365 + this.xoffset + 30, yoff, 26, 23, 0x52);
                        this.AddImageTiled(366 + this.xoffset + 30, yoff + 1, 23, 21, 0xBBC);
                        this.AddTextEntry(369 + this.xoffset + 30, yoff, 18, 33, texthue, 1100 + i, strto);
                        // kills needed
                        this.AddImageTiled(392 + this.xoffset + 30, yoff, 35, 23, 0x52);
                        this.AddImageTiled(393 + this.xoffset + 30, yoff + 1, 32, 21, 0xBBC);
                        this.AddTextEntry(396 + this.xoffset + 30, yoff, 27, 33, texthue, 1200 + i, strkill);

                        // mindelay
                        this.AddImageTiled(428 + this.xoffset + 30, yoff, 41, 23, 0x52);
                        this.AddImageTiled(429 + this.xoffset + 30, yoff + 1, 38, 21, 0xBBC);
                        this.AddTextEntry(432 + this.xoffset + 30, yoff, 33, 33, texthue, 1300 + i, strmind);

                        // maxdelay
                        this.AddImageTiled(470 + this.xoffset + 30, yoff, 41, 23, 0x52);
                        this.AddImageTiled(471 + this.xoffset + 30, yoff + 1, 38, 21, 0xBBC);
                        this.AddTextEntry(474 + this.xoffset + 30, yoff, 33, 33, texthue, 1400 + i, strmaxd);

                        // packrange
                        this.AddImageTiled(512 + this.xoffset + 30, yoff, 33, 23, 0x52);
                        this.AddImageTiled(513 + this.xoffset + 30, yoff + 1, 30, 21, 0xBBC);
                        this.AddTextEntry(516 + this.xoffset + 30, yoff, 25, 33, texthue, 1600 + i, strpackrange);

                        if (this.m_Spawner.SequentialSpawn >= 0)
                        {
                            // restrict kills button
                            this.AddButton(545 + this.xoffset + 30, yoff, this.m_Spawner.SpawnObjects[i].RestrictKillsToSubgroup ? 0xD3 : 0xD2,
                                this.m_Spawner.SpawnObjects[i].RestrictKillsToSubgroup ? 0xD2 : 0xD3, 300 + i, GumpButtonType.Reply, 0);

                            //clear on advance button for spawn entries in subgroups that require kills
                            this.AddButton(565 + this.xoffset + 30, yoff, this.m_Spawner.SpawnObjects[i].ClearOnAdvance ? 0xD3 : 0xD2,
                                this.m_Spawner.SpawnObjects[i].ClearOnAdvance ? 0xD2 : 0xD3, 400 + i, GumpButtonType.Reply, 0);
                        }

                        // add the next spawn time
                        this.AddLabelCropped(590 + this.xoffset + 30, yoff, 70, 20, 55, strnext);
                    }
                    //AddButton( 20, 22 * (i%MaxEntriesPerPage) + 34, 0x15E3, 0x15E7, 5 + (i * 2), GumpButtonType.Reply, 0 );
                }
                // the spawn specification text
                //if(str != null)
                this.AddTextEntry(52, 22 * (i % MaxEntriesPerPage) + 31, 119 + this.xoffset - 25, 21, texthue, i, str);
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
                        #if(BOOKTEXTENTRY)
                        if (i < this.m_Spawner.SpawnObjects.Length)
                        {
                            string currenttext = this.m_Spawner.SpawnObjects[i].TypeName;
                            if (currenttext != null && currenttext.Length >= 230)
                            {
                                str = currenttext;
                            }
                        }
                        #endif
                        string typestr = BaseXmlSpawner.ParseObjectType(str);

                        Type type = null;
                        if (typestr != null)
                        {
                            try
                            {
                                type = SpawnerType.GetType(typestr);
                            }
                            catch
                            {
                            }
                        }

                        if (type != null)
                            SpawnObjects.Add(new XmlSpawner.SpawnObject(from, this.m_Spawner, str, 0));
                        else
                        {
                            // check for special keywords
                            if (typestr != null && (BaseXmlSpawner.IsTypeOrItemKeyword(typestr) || typestr.IndexOf("{") != -1 || typestr.StartsWith("*") || typestr.StartsWith("#")))
                            {
                                SpawnObjects.Add(new XmlSpawner.SpawnObject(from, this.m_Spawner, str, 0));
                            }
                            else
                                this.m_Spawner.status_str = String.Format("{0} is not a valid type name.", str);
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
                        if (i < this.m_Spawner.SpawnObjects.Length)
                        {
                            // check to see if the existing typename is longer than the max textentry buffer
                            // if it is then dont update it since we will assume that the textentry has truncated the actual string
                            // that could be longer than the buffer if booktextentry is used
                            #if(BOOKTEXTENTRY)
                            string currentstr = this.m_Spawner.SpawnObjects[i].TypeName;
                            if (currentstr != null && currentstr.Length < 230)
                            #endif
                            {
                                if (this.m_Spawner.SpawnObjects[i].TypeName != str)
                                {
                                    CommandLogging.WriteLine(from, "{0} {1} changed XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7} to {8}", from.AccessLevel, CommandLogging.Format(from), this.m_Spawner.Serial, this.m_Spawner.Name, this.m_Spawner.GetWorldLocation().X, this.m_Spawner.GetWorldLocation().Y, this.m_Spawner.Map, this.m_Spawner.SpawnObjects[i].TypeName, str);
                                }

                                this.m_Spawner.SpawnObjects[i].TypeName = str;
                            }
                        }
                    }
                }
            }
        }

        #if(BOOKTEXTENTRY)

        public static void ProcessSpawnerBookEntry(Mobile from, object[] args, string entry)
        {
            if (from == null || args == null || args.Length < 6)
                return;

            XmlSpawner m_Spawner = (XmlSpawner)args[0];
            int m_Index = (int)args[1];
            int m_X = (int)args[2];
            int m_Y = (int)args[3];
            int m_Extension = (int)args[4];
            int m_page = (int)args[5];
            if (m_Spawner == null || m_Spawner.SpawnObjects == null)
                return;

            // place the book text into the spawn entry
            if (m_Index < m_Spawner.SpawnObjects.Length)
            {
                XmlSpawner.SpawnObject so = m_Spawner.SpawnObjects[m_Index];

                if (so.TypeName != entry)
                {
                    CommandLogging.WriteLine(from, "{0} {1} changed XmlSpawner {2} '{3}' [{4}, {5}] ({6}) : {7} to {8}", from.AccessLevel, CommandLogging.Format(from), m_Spawner.Serial, m_Spawner.Name, m_Spawner.GetWorldLocation().X, m_Spawner.GetWorldLocation().Y, m_Spawner.Map, so.TypeName, entry);
                }

                so.TypeName = entry;
            }
            else
            {
                // add a new spawn entry
                m_Spawner.m_SpawnObjects.Add(new XmlSpawner.SpawnObject(from, m_Spawner, entry, 1));

                m_Index = m_Spawner.SpawnObjects.Length - 1;

                // and bump the maxcount of the spawner
                m_Spawner.MaxCount++;
            }

            // refresh the spawner gumps			
            RefreshSpawnerGumps(from);
            // and refresh the current one
            //from.SendGump( new XmlSpawnerGump(m_Spawner, m_X, m_Y, m_Extension,0, m_page) );
            // return the text entry focus to the book.  Havent figured out how to do that yet.
        }

        #endif

        public static void Refresh_Callback(object state)
        {
            object[] args = (object[])state;
            Mobile m = (Mobile)args[0];
            // refresh the spawner gumps			
            RefreshSpawnerGumps(m);
        }

        public static void RefreshSpawnerGumps(Mobile from)
        {
            if (from == null)
                return;

            NetState ns = from.NetState;

            if (ns != null && ns.Gumps != null)
            {
                ArrayList refresh = new ArrayList();

                foreach (Gump g in ns.Gumps)
                {
                    if (g is XmlSpawnerGump)
                    {
                        XmlSpawnerGump xg = (XmlSpawnerGump)g;

                        // clear the gump status on the spawner associated with the gump
                        if (xg.m_Spawner != null)
                        {
                            // and add the old gump to the removal list
                            refresh.Add(xg);
                        }
                    }
                }

                // close all of the currently opened spawner gumps
                from.CloseGump(typeof(XmlSpawnerGump));

                // reopen the closed gumps from the gump collection
                foreach (XmlSpawnerGump g in refresh)
                {
                    // reopen a new gump for the spawner
                    if (g.m_Spawner != null /*&& g.m_Spawner.SpawnerGump == g */)
                    {
                        // flag the current gump on the spawner as closed
                        g.m_Spawner.GumpReset = true;

                        XmlSpawnerGump xg = new XmlSpawnerGump(g.m_Spawner, g.X, g.Y, g.m_ShowGump, g.xoffset, g.page, g.Rentry);

                        from.SendGump(xg);
                    }
                }
            }
        }

        private bool ValidGotoObject(Mobile from, object o)
        {
            if (o is Item)
            {
                Item i = o as Item;
                if (!i.Deleted && (i.Map != null) && (i.Map != Map.Internal))
                    return true;

                if (from != null && !from.Deleted)
                {
                    from.SendMessage("{0} is not available", i);
                }
            }
            else if (o is Mobile)
            {
                Mobile m = o as Mobile;
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
            if (this.m_Spawner == null || this.m_Spawner.Deleted || state == null || info == null)
            {
                if (this.m_Spawner != null)
                    this.m_Spawner.SpawnerGump = null;
                return;
            }

            // restrict access to the original creator or someone of higher access level
            //if (m_Spawner.FirstModifiedBy != null && m_Spawner.FirstModifiedBy != state.Mobile && state.Mobile.AccessLevel <= m_Spawner.FirstModifiedBy.AccessLevel)
            //return;

            // Get the current name
            TextRelay tr = info.GetTextEntry(999);
            if (tr != null)
            {
                this.m_Spawner.Name = tr.Text;
            }

            // update typenames of the spawn objects based upon the current text entry strings
            this.UpdateTypeNames(state.Mobile, info);

            // Update the creature list
            this.m_Spawner.SpawnObjects = this.CreateArray(info, state.Mobile);

            if (this.m_Spawner.SpawnObjects == null)
            {
                this.m_Spawner.SpawnerGump = null;
                return;
            }

            this.AllowGumpUpdate = true;

            for (int i = 0; i < this.m_Spawner.SpawnObjects.Length; i++)
            {
                if (this.page != (int)(i / MaxEntriesPerPage))
                    continue;

                // check the max count entry
                TextRelay temcnt = info.GetTextEntry(500 + i);
                if (temcnt != null)
                {
                    int maxval = 0;
                    try
                    {
                        maxval = Convert.ToInt32(temcnt.Text, 10);
                    }
                    catch
                    {
                    }
                    if (maxval < 0)
                        maxval = 0;

                    this.m_Spawner.SpawnObjects[i].MaxCount = maxval;
                }

                if (this.m_ShowGump > 0)
                {
                    // check the subgroup entry
                    TextRelay tegrp = info.GetTextEntry(600 + i);
                    if (tegrp != null)
                    {
                        int grpval = 0;
                        try
                        {
                            grpval = Convert.ToInt32(tegrp.Text, 10);
                        }
                        catch
                        {
                        }
                        if (grpval < 0)
                            grpval = 0;

                        this.m_Spawner.SpawnObjects[i].SubGroup = grpval;
                    }
                }

                if (this.m_ShowGump > 1)
                {
                    // note, while these values can be entered in any spawn entry, they will only be assigned to the subgroup leader
                    int subgroupindex = this.m_Spawner.GetCurrentSequentialSpawnIndex(this.m_Spawner.SpawnObjects[i].SubGroup);
                    TextRelay tegrp;

                    if (subgroupindex >= 0 && subgroupindex < this.m_Spawner.SpawnObjects.Length)
                    {
                        // check the reset time entry
                        tegrp = info.GetTextEntry(1000 + i);
                        if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            double grpval = 0;
                            try
                            {
                                grpval = Convert.ToDouble(tegrp.Text);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = 0;

                            this.m_Spawner.SpawnObjects[i].SequentialResetTime = 0;

                            this.m_Spawner.SpawnObjects[subgroupindex].SequentialResetTime = grpval;
                        }
                        // check the reset to entry
                        tegrp = info.GetTextEntry(1100 + i);
                        if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 0;
                            try
                            {
                                grpval = Convert.ToInt32(tegrp.Text, 10);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = 0;

                            this.m_Spawner.SpawnObjects[subgroupindex].SequentialResetTo = grpval;
                        }
                        // check the kills entry
                        tegrp = info.GetTextEntry(1200 + i);
                        if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 0;
                            try
                            {
                                grpval = Convert.ToInt32(tegrp.Text, 10);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = 0;

                            this.m_Spawner.SpawnObjects[subgroupindex].KillsNeeded = grpval;
                        }
                    }

                    // check the mindelay
                    tegrp = info.GetTextEntry(1300 + i);
                    if (tegrp != null)
                    {
                        if (tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            double grpval = -1;
                            try
                            {
                                grpval = Convert.ToDouble(tegrp.Text);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != this.m_Spawner.SpawnObjects[i].MinDelay)
                            {
                                this.m_Spawner.SpawnObjects[i].MinDelay = grpval;
                                this.m_Spawner.RefreshNextSpawnTime(this.m_Spawner.SpawnObjects[i]);
                            }
                        }
                        else
                        {
                            this.m_Spawner.SpawnObjects[i].MinDelay = -1;
                            this.m_Spawner.SpawnObjects[i].MaxDelay = -1;
                            this.m_Spawner.RefreshNextSpawnTime(this.m_Spawner.SpawnObjects[i]);
                        }
                    }

                    // check the maxdelay
                    tegrp = info.GetTextEntry(1400 + i);
                    if (tegrp != null)
                    {
                        if (tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            double grpval = -1;
                            try
                            {
                                grpval = Convert.ToDouble(tegrp.Text);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = -1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != this.m_Spawner.SpawnObjects[i].MaxDelay)
                            {
                                this.m_Spawner.SpawnObjects[i].MaxDelay = grpval;
                                this.m_Spawner.RefreshNextSpawnTime(this.m_Spawner.SpawnObjects[i]);
                            }
                        }
                        else
                        {
                            this.m_Spawner.SpawnObjects[i].MinDelay = -1;
                            this.m_Spawner.SpawnObjects[i].MaxDelay = -1;
                            this.m_Spawner.RefreshNextSpawnTime(this.m_Spawner.SpawnObjects[i]);
                        }
                    }

                    // check the spawns per tick
                    tegrp = info.GetTextEntry(1500 + i);
                    if (tegrp != null)
                    {
                        if (tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 1;
                            try
                            {
                                grpval = int.Parse(tegrp.Text);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = 1;

                            // if this value has changed, then update the next spawn time
                            if (grpval != this.m_Spawner.SpawnObjects[i].SpawnsPerTick)
                            {
                                this.m_Spawner.SpawnObjects[i].SpawnsPerTick = grpval;
                            }
                        }
                        else
                        {
                            this.m_Spawner.SpawnObjects[i].SpawnsPerTick = 1;
                        }
                    }

                    // check the packrange
                    tegrp = info.GetTextEntry(1600 + i);
                    if (tegrp != null)
                    {
                        if (tegrp.Text != null && tegrp.Text.Length > 0)
                        {
                            int grpval = 1;
                            try
                            {
                                grpval = int.Parse(tegrp.Text);
                            }
                            catch
                            {
                            }
                            if (grpval < 0)
                                grpval = 1;

                            // if this value has changed, then update 
                            if (grpval != this.m_Spawner.SpawnObjects[i].PackRange)
                            {
                                this.m_Spawner.SpawnObjects[i].PackRange = grpval;
                            }
                        }
                        else
                        {
                            this.m_Spawner.SpawnObjects[i].PackRange = -1;
                        }
                    }
                }
            }

            // Update the maxcount
            TextRelay temax = info.GetTextEntry(300);
            if (temax != null)
            {
                int maxval = 0;
                try
                {
                    maxval = Convert.ToInt32(temax.Text, 10);
                }
                catch
                {
                }
                if (maxval < 0)
                    maxval = 0;
                // if the maxcount of the spawner has been altered external to the interface (e.g. via props, or by the running spawner itself
                // then that change will override the text entry
                if (this.m_Spawner.MaxCount == this.initial_maxcount)
                {
                    this.m_Spawner.MaxCount = maxval;
                }
            }

            switch (info.ButtonID)
            {
                case 0: // Close
                    {
                        // clear any text entry books
                        this.m_Spawner.DeleteTextEntryBook();
                        // and reset the gump status
                        this.m_Spawner.GumpReset = true;

                        return;
                    }
                case 1: // Help
                    {
                        //state.Mobile.SendGump( new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump));
                        state.Mobile.SendGump(new HelpGump(this.m_Spawner, this, this.X + 290, this.Y));
                        break;
                    }
                case 2: // Bring everything home
                    {
                        this.m_Spawner.BringToHome();
                        break;
                    }
                case 3: // Complete respawn
                    {
                        this.m_Spawner.Respawn();
                        //m_Spawner.AdvanceSequential();
                        this.m_Spawner.m_killcount = 0;
                        break;
                    }
                case 4: // Goto
                    {
                        state.Mobile.Location = this.m_Spawner.Location;
                        state.Mobile.Map = this.m_Spawner.Map;
                        break;
                    }
                case 200: // gump extension
                    {
                        if (this.m_ShowGump > 1)
                            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, -1, this.xoffset, this.page));
                        else
                            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump + 2, this.xoffset, this.page));
                        return;
                    }
                case 201: // gump text extension
                    {
                        if (this.xoffset > 0)
                            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, 0, this.page));
                        else
                            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, 250, this.page));
                        return;
                    }
                case 700: // Start/stop spawner
                    {
                        if (this.m_Spawner.Running)
                            this.m_Spawner.Running = false;
                        else
                            this.m_Spawner.Running = true;
                        break;
                    }
                case 701: // Complete reset
                    {
                        this.m_Spawner.Reset();
                        break;
                    }
                case 702: // Sort spawns
                    {
                        this.m_Spawner.SortSpawns();
                        break;
                    }
                case 900: // empty the status string
                    {
                        this.m_Spawner.status_str = "";
                        break;
                    }
                case 9998:  // refresh the gump
                    {
                        state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page));
                        return;
                    }
                case 9999:
                    {
                        // Show the props window for the spawner, as well as a new gump
                        state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page));
                        #if(NEWPROPSGUMP)
                        state.Mobile.SendGump(new XmlPropertiesGump(state.Mobile, this.m_Spawner));
                        #else
                    state.Mobile.SendGump( new PropertiesGump( state.Mobile, m_Spawner ) );
                        #endif
                        return;
                    }
                default:
                    {
                        // check the restrict kills flag
                        if (info.ButtonID >= 300 && info.ButtonID < 300 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 300;
                            if (index < this.m_Spawner.SpawnObjects.Length)
                                this.m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup = !this.m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup;
                        }
                        else if (info.ButtonID >= 400 && info.ButtonID < 400 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 400;
                            if (index < this.m_Spawner.SpawnObjects.Length)
                                this.m_Spawner.SpawnObjects[index].ClearOnAdvance = !this.m_Spawner.SpawnObjects[index].ClearOnAdvance;
                        }
                        else if (info.ButtonID >= 800 && info.ButtonID < 800 + MaxSpawnEntries)
                        {
                            // open the text entry gump
                            int index = info.ButtonID - 800;
                            // open a text entry gump
                            #if(BOOKTEXTENTRY)
                            // display a new gump
                            XmlSpawnerGump newgump = new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page);
                            state.Mobile.SendGump(newgump);

                            // is there an existing book associated with the gump?
                            if (this.m_Spawner.m_TextEntryBook == null)
                            {
                                this.m_Spawner.m_TextEntryBook = new ArrayList();
                            }

                            object[] args = new object[6];

                            args[0] = this.m_Spawner;
                            args[1] = index;
                            args[2] = this.X;
                            args[3] = this.Y;
                            args[4] = this.m_ShowGump;
                            args[5] = this.page;

                            XmlTextEntryBook book = new XmlTextEntryBook(0, String.Empty, this.m_Spawner.Name, 20, true, new XmlTextEntryBookCallback(ProcessSpawnerBookEntry), args);

                            this.m_Spawner.m_TextEntryBook.Add(book);

                            book.Title = String.Format("Entry {0}", index);
                            book.Author = this.m_Spawner.Name;

                            // fill the contents of the book with the current text entry data
                            string text = String.Empty;
                            if (this.m_Spawner.SpawnObjects != null && index < this.m_Spawner.SpawnObjects.Length)
                            {
                                text = this.m_Spawner.SpawnObjects[index].TypeName;
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
                            if (index < this.m_Spawner.SpawnObjects.Length)
                            {
                                int scount = this.m_Spawner.SpawnObjects[index].SpawnedObjects.Count;
                                if (scount > 0)
                                {
                                    object so = this.m_Spawner.SpawnObjects[index].SpawnedObjects[nclicks % scount];
                                    if (this.ValidGotoObject(state.Mobile, so))
                                    {
                                        IPoint3D o = so as IPoint3D;
                                        if (o != null)
                                        {
                                            Map m = this.m_Spawner.Map;
                                            if (o is Item)
                                                m = ((Item)o).Map;
                                            if (o is Mobile)
                                                m = ((Mobile)o).Map;

                                            state.Mobile.Location = new Point3D(o);
                                            state.Mobile.Map = m;
                                        }
                                    }
                                }
                            }
                        }
                        else if (info.ButtonID >= 4000 && info.ButtonID < 4001 + (int)(MaxSpawnEntries / MaxEntriesPerPage))
                        {
                            // which page
                            this.page = info.ButtonID - 4000;
                        }
                        else if (info.ButtonID >= 6000 && info.ButtonID < 6000 + MaxSpawnEntries)
                        {
                            int index = info.ButtonID - 6000;

                            if (index < this.m_Spawner.SpawnObjects.Length)
                            {
                                this.m_Spawner.SpawnObjects[index].Disabled = !this.m_Spawner.SpawnObjects[index].Disabled;

                                // clear any current spawns on the disabled entry
                                if (this.m_Spawner.SpawnObjects[index].Disabled)
                                    this.m_Spawner.RemoveSpawnObjects(this.m_Spawner.SpawnObjects[index]);
                            }
                        }
                        else if (info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxSpawnEntries)
                        {
                            int i = info.ButtonID - 5000;

                            string categorystring = null;
                            string entrystring = null;

                            TextRelay te = info.GetTextEntry(i);

                            if (te != null && te.Text != null)
                            {
                                // get the string
                                string[] cargs = te.Text.Split(',');

                                // parse out any comma separated args
                                categorystring = cargs[0];

                                entrystring = te.Text;
                            }

                            if (categorystring == null || categorystring.Length == 0)
                            {
                                XmlSpawnerGump newg = new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page);
                                state.Mobile.SendGump(newg);

                                // if no string has been entered then just use the full categorized add gump
                                state.Mobile.CloseGump(typeof(Server.Gumps.XmlCategorizedAddGump));
                                state.Mobile.SendGump(new Server.Gumps.XmlCategorizedAddGump(state.Mobile, i, newg));
                            }
                            else
                            {
                                // use the XmlPartialCategorizedAddGump
                                state.Mobile.CloseGump(typeof(Server.Gumps.XmlPartialCategorizedAddGump));

                                //Type [] types = (Type[])XmlPartialCategorizedAddGump.Match( categorystring ).ToArray( typeof( Type ) );
                                ArrayList types = XmlPartialCategorizedAddGump.Match(categorystring);

                                XmlSpawnerGump.ReplacementEntry re = new XmlSpawnerGump.ReplacementEntry();
                                re.Typename = entrystring;
                                re.Index = i;
                                re.Color = 0x1436;

                                XmlSpawnerGump newg = new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page, re);

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

                                #if(BOOKTEXTENTRY)
                                if (index < this.m_Spawner.SpawnObjects.Length)
                                {
                                    string str = this.m_Spawner.SpawnObjects[index].TypeName;

                                    if (str != null && str.Length >= 230)
                                        entrystr = str;
                                }
                                #endif

                                if (type == 0) // Add creature
                                {
                                    this.m_Spawner.AddSpawnObject(entrystr);
                                }
                                else // Remove creatures
                                {
                                    this.m_Spawner.DeleteSpawnObject(state.Mobile, entrystr);
                                }
                            }
                        }
                        break;
                    }
            }
            // Create a new gump
            //m_Spawner.OnDoubleClick( state.Mobile);
            state.Mobile.SendGump(new XmlSpawnerGump(this.m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page, this.Rentry));
        }
    }
}