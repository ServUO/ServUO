#define NEWPROPSGUMP
#define BOOKTEXTENTRY

using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using System.Reflection;
using Server.Commands;
using CPA = Server.CommandPropertyAttribute;
using System.Xml;
using Server.Spells;
using System.Text;

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
		private XmlSpawnerGump m_SpawnerGump;

		public HelpGump(XmlSpawner spawner, XmlSpawnerGump spawnergump, int X, int Y)
			: base(X, Y)
		{
			if (spawner == null || spawner.Deleted)
				return;
			m_Spawner = spawner;
			m_SpawnerGump = spawnergump;

			AddPage(0);

            int width = 370;

			AddBackground(20, 0, width, 480, 5054);

			AddPage(1);
			//AddAlphaRegion( 20, 0, 220, 554 );
			AddImageTiled(20, 0, width, 480, 0x52);
			//AddImageTiled( 24, 6, 213, 261, 0xBBC );

			AddLabel(27, 2, 0x384, "Standalone Keywords NB: ( « stands for < ) ( » stands for > )");
			AddHtml(25, 20, width - 10, 440,
				"mobtype[,arg1,arg2,...]\n" +
				"SET[,itemname o seriale][,itemtype]/prop/value/...\n" +
				"SETONMOB,nomemob[,mobtype]/prop/value/...\n" +
				"SETONTHIS[,proptest]/prop/value/...\n" +
				"SETONTRIGMOB/prop/value/...\n" +
				"FOREACH,objecttype[,name][,distance]/action (KILL,DAMAGE,etc)\n" +
				"SETVAR,namevar/value\n" +
				"SETONNEARBY,distance,name[,type][,searchcontainers (true/false)]/prop/value/...\n" +
				"SETONPETS,distance[,name]/prop/value/...\n" +
				"SETONCARRIED,itemname[,itemtype][,equippedonly (true/false)]/prop/value/...\n" +
				"SETONSPAWN[,spawnername],subgroup/prop/value/...\n" +
				"SETONSPAWNENTRY[,spawnername],entrystring/prop/value/...\n" +
				"SETONPARENT/prop/value/...\n" +
				"TAKEGIVE[,quantity[,searchinbank (true/false),[type]]]/itemnametotake/GIVE/itemtypetogive\n" +
				"GIVE[,probability (0.01=1% 1=100%)]/itemtypetogive\n" +
				"GIVE[,probability (0.01=1% 1=100%)]/«itemtypedadare/proprietà/valore/...»\n" +
				"TAKE[,probability (0.01=1% 1=100%)[,quantity[,searchinbank (true/false)[,itemtype]]]]/nomeitem\n" +
				"TAKEBYTYPE[,probability (0.01=1% 1=100%)[,quantity[,searchinbank (true/false)]]]/itemtype\n" +
				"GUMP,titolo,gumptype (*)[,gumpconstructor]/text  *(0=messaggio - 1=yes/no - 2=response - 3=questgump - 4=multiple response)\n" +
				"BROWSER/url\n" +
				"SENDMSG[,color]/text\n" +
				"SENDASCIIMSG[,color][,fontnumber]/text (no accent)\n" +
				"WAITUNTIL[,duration][,timeout][/condition][/spawngroup]\n" +
				"WHILE/condition/spawngroup\n" +
				"SPAWN[,spawnername],subgroup\n" +
				"IF/condition/thenspawn[/elsespawn]\n" +
				"DESPAWN[,spawnername],subgroup\n" +
				"SPAWN[,spawnername],subgroup\n" +
				"GOTO/subgroup\n" +
				"COMMAND/command string (pay attention please!)\n" +
				"MUSIC,musicname[,distance]\n" +
				"SOUND,soundnumber\n" +
				"MEFFECT,itemid[,speed][,x,y,z][,x2,y2,z2]\n" +
				"EFFECT,itemid,duration[,x,y,z] OR EFFECT,itemid,durata[,trigmob]\n" +
				"POISON,levelname[,distance][,onlyplayers (true/false)]\n" +
				"DAMAGE,damage,phys,fire,cold,pois,energy[,distance][,onlyplayers (true/false)]\n" +
				"RESURRECT[,distance][,pets (true/false)]\n" +
				"CAST,spellnumber[,argomenti] OR CAST,spellname[,argomenti]\n" +
				"BCAST[,color][,fontnumber]/text\n" +
				"BSOUND,soundnumber",
				false, true);
			AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 2);
			AddLabel(width - 38, 2, 0x384, "1");
			AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 4);

			AddPage(2);
			AddLabel(27, 2, 0x384, "Value and Itemtype Keywords NB: ( « EQUALE TO < ) ( » EQUALS TO > )");
			AddHtml(25, 20, width - 10, 440,
				"property/@value\n\n" +
				"Special KeyWords\n" +
				"ARMOR,minlevel,maxlevel\n" +
				"WEAPON,minlevel,maxlevel\n" +
				"JARMOR,minlevel,maxlevel\n" +
				"JWEAPON,minlevel,maxlevel\n" +
				"SARMOR,minlevel,maxlevel\n" +
				"SHIELD,minlevel,maxlevel\n" +
				"JEWELRY,minlevel,maxlevel\n" +
				"ITEM,serial (to find that item)\n" +
				"SCROLL,mincircle,maxcircle\n" +
				"LOOT,methodname\n" +
				"NECROSCROLL,index\n" +
				"LOOTPACK,loottype\n" +
				"POTION\n" +
				"TAKEN (it will help you find item taken during quest, xmlspawners and such)\n" +
				"GIVEN (equal to TAKEN, but for item given)\n" +
				"MULTIADDON,filename\n\n" +
				"{the GET or RND keyword are used inside brachets}\n" +
				"RND,min,max\n" +
				"RNDBOOL\n" +
				"RNDLIST,int1[,int2,...]\n" +
				"RNDSTRLIST,str1[,str2,...]\n" +
				"MY,prop (for spawner mob)\n" +
				"GET,[itemname or serial or SETITEM(*)][,itemtype],prop (*: the setitem of spawner, it's written SETITEM, upper char!)\n" +
				"GET,[itemname or serial or SETITEM][,itemtype],«ATTACHMENT,type,name,property»\n" +
				"GETVAR,varname (XmlLocalVariable)\n" +
				"GETONMOB,mobname[,mobtype],prop\n" +
				"GETONMOB,mobname[,mobtype],«ATTACHMENT,type,nome,prop»\n" +
				"GETONCARRIED,itemname[,itemtype][,equippedonly (true/false)],prop\n" +
				"GETONCARRIED,itemname[,itemtype][,equippedonly (true/false)],«ATTACHMENT,type,nome,prop»\n" +
				"GETONTRIGMOB,prop\n" +
				"GETONTRIGMOB,«ATTACHMENT,type,name,prop»\n" +
				"GETONNEARBY,distance,name[,type][,searchcontainers (true/false)],prop\n" +
				"GETONNEARBY,distance,name[,type][,searchcontainers (true/false)],«ATTACHMENT,type,name,prop»\n" +
				"GETONPARENT,prop\n" +
				"GETONPARENT,«ATTACHMENT,type,name,prop»\n" +
				"GETONTHIS,prop\n" +
				"GETONTHIS,«ATTACHMENT,type,name,prop»\n" +
				"GETONTAKEN,prop\n" +
				"GETONTAKEN,«ATTACHMENT,type,name,prop»\n" +
				"GETONGIVEN,prop\n" +
				"GETONGIVEN,«ATTACHMENT,type,name,prop»\n" +
				"GETONSPAWN[,nomespawner],subgroup,prop\n" +
				"GETONSPAWN[,nomespawner],subgroup,COUNT (special - returns the number of objects spawned!)\n" +
				"GETONSPAWN[,spawnername],subgroup,«ATTACHMENT,type,name,prop»\n" +
				"GETFROMFILE,filename\n" +
				"GETACCOUNTTAG,tagname\n" +
				"MUL,value or MUL,min,max (fractal numbers)\n" +
				"INC,value or INC,min,max (integer numbers)\n" +
				"MOB,name[,mobtype] (search the id of a mob by name and eventually by type)\n" +
				"TRIGMOB\n" +
				"AMOUNTCARRIED,itemtype[,searchinbank (true/false)][,itemname]\n" +
				"PLAYERSINRANGE,distance\n" +
				"TRIGSKILL,name or value or base or cap\n" +
				"RANDNAME,nametype (female,male etc)\n" +
				"MUSIC,musicname[,distance]\n" +
				"SOUND,value\n" +
				"EFFECT,itemid,duration[,x,y,z]\n" +
				"BOLTEFFECT[,sound[,color]]\n" +
				"MEFFECT,itemid[,speed][,x,y,z][,x2,y2,z2]\n" +
				"PEFFECT,itemid,speed,[x,y,z]\n" +
				"POISON,level[,distance][,onlyplayers (true/false)]\n" +
				"DAMAGE,damage,phys,fire,cold,pois,energy[,distance][,onlyplayers (true/false)]\n" +
				"ADD[,probability (0.01=1% 1=100%)]/itemtype[,args]\n" +
				"ADD[,probability (0.01=1% 1=100%)]/«itemtype[,args]/prop/value...»\n" +
				"EQUIP[,probability (0.01=1% 1=100%)]/itemtype[,args]\n" +
				"EQUIP[,probability (0.01=1% 1=100%)]/«itemtype[,args]/prop/value...»\n" +
				"DELETE\n" +
				"KILL\n" +
				"UNEQUIP,layer[,delete (true/false)]\n" +
				"ATTACH[,probability (0.01=1% 1=100%)]/attachmenttype[,args]\n" +
				"ATTACH[,probability (0.01=1% 1=100%)]/«attachmenttype[,args]/prop/value...»\n" +
				"MSG[,probability (0.01=1% 1=100%)]/text\n" +
				"ASCIIMSG[,probability (0.01=1% 1=100%)][,color][,fontnumber]/text\n" +
				"SENDMSG[,probability (0.01=1% 1=100%)][,color]/text\n" +
				"SENDASCIIMSG[,probability (0.01=1% 1=100%)][,color][,fontnumber]/text\n" +
				"SAY[,probability (0.01=1% 1=100%)]/text\n" +
				"SPEECH[,probability (0.01=1% 1=100%)][,keywordnumber]\n" +
				"OFFSET,x,y,[,z]\n" +
				"ANIMATE,action[,framecount][,repeatcount][,forward true/false][,repeat true/false][,delay]\n" +
				"FACETO,x,y (turns the mobile to face in a direction by the coords given)\n" +
				"SETVALUE,nomevar,valore,durata (XmlValue)\n" +
				"FLASH,number (1 fade black - 2 fade white - 3 light flash - 4 light to black flash - 5 black flash SOLO PG)\n" +
				"PRIVMSG[,probability (0.01=1% 1=100%)][,color]/text (shows a message only to that player)\n" +
				"BCAST[,color][,font]/message\n" +
				"always...«ATTACHMENT,type,name,prop» as a property on  GET\n" +
				"SKILL,skillname\n" +
				"STEALABLE,stealable (true/false)",
				false, true);
			AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 3);
			AddLabel(width - 41, 2, 0x384, "2");
			AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 1);

			AddPage(3);
			AddLabel(27, 2, 0x384, "[ Commands");

			AddHtml(25, 20, width - 10, 440,
				"XmlAdd [-defaults]\n" +
				"XmlShow\n" +
				"XmlHide\n" +
				"XmlFind\n" +
				"AddAtt type [args]\n" +
				"GetAtt [type]\n" +
				"DelAtt [type][serialno]\n" +
				"AvailAtt\n" +
				"SmartStat [accesslevel GameMaster]\n" +
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

			AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 4);
			AddLabel(width - 41, 2, 0x384, "3");
			AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 2);

			AddPage(4);
			AddLabel(27, 2, 0x384, "Quest types");
			AddHtml(25, 20, width - 10, 180,
				"KILL,mobtype[,count][,proptest]\n" +
				"KILLNAMED,mobname[,type][,count][,proptest]\n" +
				"GIVE,mobname,itemtype[,count][,proptest]\n" +
				"GIVENAMED,mobname,itemname[,type][,count][,proptest]\n" +
				"COLLECT,itemtype[,count][,proptest]\n" +
				"COLLECTNAMED,itemname[,itemtype][,count][,proptest]\n" +
				"ESCORT[,mobname][,proptest]\n",
				false, true);

			AddLabel(27, 200, 0x384, "Trigger/NoTriggerOnCarried");
			AddHtml(25, 220, width - 10, 50,
				"ATTACHMENT,name,type\n" +
				"itemname[,type][,EQUIPPED][,objective#,objective#,...]\n",
				false, true);

			AddLabel(27, 300, 0x384, "GUMPITEMS");
			AddHtml(25, 320, width - 10, 150,
				"BUTTON,gumpid,x,y\n" +
				"HTML,x,y,width,height,text\n" +
				"IMAGE,gumpid,x,y[,hue]\n" +
				"IMAGETILED,gumpid,x,y,width,height\n" +
				"ITEM,itemid,x,y[,hue]\n" +
				"LABEL,x,y,labelstring[,labelcolor]\n" +
				"RADIO,gumpid1,gumpid2,x,y[,initialstate]\n" +
				"TEXTENTRY,x,y,width,height[,text][,textcolor]\n",
				false, true);

			AddButton(width - 30, 5, 0x15E1, 0x15E5, 200, GumpButtonType.Page, 1);
			AddLabel(width - 41, 2, 0x384, "4");
			AddButton(width - 60, 5, 0x15E3, 0x15E7, 200, GumpButtonType.Page, 3);
		}
	}
	public class TextEntryGump : Gump
	{
		private XmlSpawner m_Spawner;
		private int m_index;
		private XmlSpawnerGump m_SpawnerGump;

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
				str = (string)m_Spawner.SpawnObjects[index].TypeName;
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
			if (info == null || state == null || state.Mobile == null) return;

			if (m_Spawner == null || m_Spawner.Deleted)
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
					catch { }

				}
				// open a new text entry gump
				state.Mobile.SendGump(new TextEntryGump(m_Spawner, m_SpawnerGump, m_index, this.X, this.Y));
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
			// Create a new gump

			//m_Spawner.OnDoubleClick( state.Mobile);
			// open a new spawner gump
			state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, m_SpawnerGump.m_ShowGump, m_SpawnerGump.xoffset, m_SpawnerGump.page));
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
				AddLabel(15, 365, 33, String.Format("{0}", m_Spawner.SequentialSpawn));
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
			for (int i = 0; i < (int)(MaxSpawnEntries / MaxEntriesPerPage); i++)
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
				if (page != (int)(i / MaxEntriesPerPage)) continue;

				string str = String.Empty;
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

				// Add page number
				//AddLabel( 15, 365, 33, String.Format("{0}",(int)(i/MaxEntriesPerPage + 1)) );
				//AddButton( 38+page*25, 365, 0x8B1+i, 0x8B1+i, 0, GumpButtonType.Page, 1+i );



				if (i < m_Spawner.SpawnObjects.Length)
				{
					if (!hasreplacement)
					{
						str = (string)m_Spawner.SpawnObjects[i].TypeName;
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
						string strnext = null;
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

					//AddButton( 20, 22 * (i%MaxEntriesPerPage) + 34, 0x15E3, 0x15E7, 5 + (i * 2), GumpButtonType.Reply, 0 );
				}
				// the spawn specification text
				//if(str != null)
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
#if(BOOKTEXTENTRY)
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
								m_Spawner.status_str = String.Format("{0} is not a valid type name.", str);
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

#if(BOOKTEXTENTRY)
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


#if(BOOKTEXTENTRY)

		public static void ProcessSpawnerBookEntry(Mobile from, object[] args, string entry)
		{
			if (from == null || args == null || args.Length < 6) return;

			XmlSpawner m_Spawner = (XmlSpawner)args[0];
			int m_Index = (int)args[1];
			int m_X = (int)args[2];
			int m_Y = (int)args[3];
			int m_Extension = (int)args[4];
			int m_page = (int)args[5];
			if (m_Spawner == null || m_Spawner.SpawnObjects == null) return;

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
			if (from == null) return;

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
			else
				if (o is Mobile)
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
			if (m_Spawner == null || m_Spawner.Deleted || state == null || info == null)
			{
				if (m_Spawner != null) m_Spawner.SpawnerGump = null;
				return;
			}

			// restrict access to the original creator or someone of higher access level
			//if (m_Spawner.FirstModifiedBy != null && m_Spawner.FirstModifiedBy != state.Mobile && state.Mobile.AccessLevel <= m_Spawner.FirstModifiedBy.AccessLevel)
			//return;


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

			AllowGumpUpdate = true;

			for (int i = 0; i < m_Spawner.SpawnObjects.Length; i++)
			{
				if (page != (int)(i / MaxEntriesPerPage)) continue;

				// check the max count entry
				TextRelay temcnt = info.GetTextEntry(500 + i);
				if (temcnt != null)
				{
					int maxval = 0;
					try { maxval = Convert.ToInt32(temcnt.Text, 10); }
					catch { }
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
						catch { }
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
						if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
						{
							double grpval = 0;
							try { grpval = Convert.ToDouble(tegrp.Text); }
							catch { }
							if (grpval < 0) grpval = 0;

							m_Spawner.SpawnObjects[i].SequentialResetTime = 0;

							m_Spawner.SpawnObjects[subgroupindex].SequentialResetTime = grpval;
						}
						// check the *reset to* entry
						tegrp = info.GetTextEntry(1100 + i);
						if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
						{
							int grpval = 0;
							try { grpval = Convert.ToInt32(tegrp.Text, 10); }
							catch { }
							if (grpval < 0) grpval = 0;

							m_Spawner.SpawnObjects[subgroupindex].SequentialResetTo = grpval;
						}
						// check the kills entry
						tegrp = info.GetTextEntry(1200 + i);
						if (tegrp != null && tegrp.Text != null && tegrp.Text.Length > 0)
						{
							int grpval = 0;
							try { grpval = Convert.ToInt32(tegrp.Text, 10); }
							catch { }
							if (grpval < 0) grpval = 0;

							m_Spawner.SpawnObjects[subgroupindex].KillsNeeded = grpval;
						}

					}

					// check the mindelay
					tegrp = info.GetTextEntry(1300 + i);
					if (tegrp != null)
					{
						if (tegrp.Text != null && tegrp.Text.Length > 0)
						{
							double grpval = -1;
							try { grpval = Convert.ToDouble(tegrp.Text); }
							catch { }
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
						if (tegrp.Text != null && tegrp.Text.Length > 0)
						{
							double grpval = -1;
							try { grpval = Convert.ToDouble(tegrp.Text); }
							catch { }
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
						if (tegrp.Text != null && tegrp.Text.Length > 0)
						{
							int grpval = 1;
							try { grpval = int.Parse(tegrp.Text); }
							catch { }
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
						if (tegrp.Text != null && tegrp.Text.Length > 0)
						{
							int grpval = 1;
							try { grpval = int.Parse(tegrp.Text); }
							catch { }
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
				catch { }
				if (maxval < 0) maxval = 0;
				// if the maxcount of the spawner has been altered external to the interface (e.g. via props, or by the running spawner itself
				// then that change will override the text entry
				if (m_Spawner.MaxCount == this.initial_maxcount)
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
						//state.Mobile.SendGump( new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump));
						state.Mobile.SendGump(new HelpGump(m_Spawner, this, this.X + 290, this.Y));
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
						if (this.m_ShowGump > 1)
							state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, -1, this.xoffset, this.page));
						else
							state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump + 2, this.xoffset, this.page));
						return;
					}
				case 201: // gump text extension
					{
						if (this.xoffset > 0)
							state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, 0, this.page));
						else
							state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, 250, this.page));
						return;
					}
				case 700: // Start/stop spawner
					{
						if (m_Spawner.Running)
							m_Spawner.Running = false;
						else
							m_Spawner.Running = true;
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
						m_Spawner.DisableGlobalAutoReset=!m_Spawner.DisableGlobalAutoReset;
						break;
					}
				case 9998:  // refresh the gump
					{
						state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page));
						return;
					}
				case 9999:
					{
						// Show the props window for the spawner, as well as a new gump
						state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page));
#if(NEWPROPSGUMP)
						state.Mobile.SendGump(new XmlPropertiesGump(state.Mobile, m_Spawner));
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
							if (index < m_Spawner.SpawnObjects.Length)
								m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup = !m_Spawner.SpawnObjects[index].RestrictKillsToSubgroup;

						}
						else
							// check the clear on advance flag
							if (info.ButtonID >= 400 && info.ButtonID < 400 + MaxSpawnEntries)
							{
								int index = info.ButtonID - 400;
								if (index < m_Spawner.SpawnObjects.Length)
									m_Spawner.SpawnObjects[index].ClearOnAdvance = !m_Spawner.SpawnObjects[index].ClearOnAdvance;
							}
							else
								// text entry gump scroll buttons
								if (info.ButtonID >= 800 && info.ButtonID < 800 + MaxSpawnEntries)
								{
									// open the text entry gump
									int index = info.ButtonID - 800;
									// open a text entry gump
#if(BOOKTEXTENTRY)
									// display a new gump
									XmlSpawnerGump newgump = new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page);
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

									XmlTextEntryBook book = new XmlTextEntryBook(0, String.Empty, m_Spawner.Name, 20, true, new XmlTextEntryBookCallback(ProcessSpawnerBookEntry), args);

									m_Spawner.m_TextEntryBook.Add(book);

									book.Title = String.Format("Entry {0}", index);
									book.Author = m_Spawner.Name;

									// fill the contents of the book with the current text entry data
									string text = String.Empty;
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
								else
									// goto spawn buttons
									if (info.ButtonID >= 1300 && info.ButtonID < 1300 + MaxSpawnEntries)
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
									else
										// page buttons
										if (info.ButtonID >= 4000 && info.ButtonID < 4001 + (int)(MaxSpawnEntries / MaxEntriesPerPage))
										{
											// which page
											this.page = info.ButtonID - 4000;

										}
										else
											// toggle the disabled state of the entry
											if (info.ButtonID >= 6000 && info.ButtonID < 6000 + MaxSpawnEntries)
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
											else
												if (info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxSpawnEntries)
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

														XmlSpawnerGump newg = new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page);
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

														XmlSpawnerGump newg = new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page, re);

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
			// Create a new gump
			//m_Spawner.OnDoubleClick( state.Mobile);
			state.Mobile.SendGump(new XmlSpawnerGump(m_Spawner, this.X, this.Y, this.m_ShowGump, this.xoffset, this.page, this.Rentry));
		}
	}
}
