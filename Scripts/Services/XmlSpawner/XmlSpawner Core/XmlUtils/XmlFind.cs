using System;
using System.Data;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Gumps;
using Server.Targeting;
using System.Reflection;
using Server.Commands;
using Server.Commands.Generic;
using CPA = Server.CommandPropertyAttribute;
using System.Xml;
using Server.Spells;
using Server.Regions;
using System.Text;
using Server.Accounting;
using System.Threading;
using Server.Engines.XmlSpawner2;
using System.Linq;

/*
** XmlFind
** utility for locating objects in the world.
** ArteGordon
** original version 1.0
** 4/13/04
**
*/

namespace Server.Mobiles
{
	public class XmlFindGump : Gump
	{

		public class XmlFindThread
		{
			SearchCriteria m_SearchCriteria;
			Mobile m_From;
			string m_commandstring;

			public XmlFindThread(Mobile from, SearchCriteria criteria, string commandstring)
			{
				m_SearchCriteria = criteria;
				m_From = from;
				m_commandstring = commandstring;
			}


			public void XmlFindThreadMain()
			{

				if (m_From == null) return;

				string status_str;

				ArrayList results = XmlFindGump.Search(m_SearchCriteria, out status_str);

				XmlFindGump gump = new XmlFindGump(m_From, m_From.Location, m_From.Map, true, true, false,

					m_SearchCriteria,

					results, -1, 0, null, m_commandstring,
					false, false, false, false, false, false, 0, 0);

				// display the updated gump synched with the main server thread
				Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(GumpDisplayCallback), new object[] { m_From, gump, status_str });

			}

			public void GumpDisplayCallback(object state)
			{
				object[] args = (object[])state;

				Mobile from = (Mobile)args[0];
				XmlFindGump gump = (XmlFindGump)args[1];
				string status_str = (string)args[2];

				if (from != null && !from.Deleted)
				{
					from.SendGump(gump);
					if (status_str != null)
					{
						from.SendMessage(33, "XmlFind: {0}", status_str);
					}
				}
			}


		}

		private const int MaxEntries = 18;
		private const int MaxEntriesPerPage = 18;

		public class SearchEntry
		{
			public bool Selected;
			public object Object;

			public SearchEntry(object o)
			{
				Object = o;
			}
		}

		public class SearchCriteria
		{
			public bool Dosearchtype;
			public bool Dosearchname;
			public bool Dosearchrange;
			public bool Dosearchregion;
			public bool Dosearchspawnentry;
			public bool Dosearchspawntype;
			public bool Dosearchcondition;
			public bool Dosearchfel;
			public bool Dosearchtram;
			public bool Dosearchmal;
			public bool Dosearchilsh;
			public bool Dosearchtok;
            public bool Dosearchter;
			public bool Dosearchint;
			public bool Dosearchnull;
			public bool Dosearcherr;
			public bool Dosearchage;
			public bool Dosearchwithattach;
			public bool Dosearchattach;
			public bool Dohidevalidint = false;
			public bool Searchagedirection;
			public double Searchage;
			public int Searchrange;
			public string Searchregion;
			public string Searchcondition;
			public string Searchtype;
			public string Searchattachtype;
			public string Searchname;
			public string Searchspawnentry;

			public Map Currentmap;
			public Point3D Currentloc;

			public SearchCriteria(bool dotype, bool doname, bool dorange, bool doregion, bool doentry, bool doentrytype, bool docondition, bool dofel, bool dotram,
				bool domal, bool doilsh, bool dotok, bool doter, bool doint, bool donull, bool doerr, bool doage, bool dowithattach, bool doattach, bool dohidevalid,
				bool agedirection, double age, int range, string region, string condition, string type, string attachtype, string name, string entry
				)
			{
				Dosearchtype = dotype;
				Dosearchname = doname;
				Dosearchrange = dorange;
				Dosearchregion = doregion;
				Dosearchspawnentry = doentry;
				Dosearchspawntype = doentrytype;
				Dosearchcondition = docondition;
				Dosearchfel = dofel;
				Dosearchtram = dotram;
				Dosearchmal = domal;
				Dosearchilsh = doilsh;
				Dosearchtok = dotok;
                Dosearchter = doter;
				Dosearchint = doint;
				Dosearchnull = donull;
				Dosearcherr = doerr;
				Dosearchage = doage;
				Dosearchwithattach = dowithattach;
				Dosearchattach = doattach;
				Dohidevalidint = dohidevalid;
				Searchagedirection = agedirection;
				Searchage = age;
				Searchrange = range;
				Searchregion = region;
				Searchcondition = condition;
				Searchtype = type;
				Searchattachtype = attachtype;
				Searchname = name;
				Searchspawnentry = entry;

			}

			public SearchCriteria()
			{
			}
		}

		private SearchCriteria m_SearchCriteria;
		private bool Sorttype;
		private bool Sortrange;
		private bool Sortname;
		private bool Sortmap;
		private bool Sortselect;
		private Mobile m_From;
		private Point3D StartingLoc;
		private Map StartingMap;
		private bool m_ShowExtension;
		private bool Descendingsort;
		private int Selected;
		private int DisplayFrom;
		private string SaveFilename;
		private string CommandString;

		private bool SelectAll = false;

		private ArrayList m_SearchList;

		public static void Initialize()
		{
			CommandSystem.Register("XmlFind", AccessLevel.GameMaster, new CommandEventHandler(XmlFind_OnCommand));
		}

		private static bool TestRange(object o, int range, Map currentmap, Point3D currentloc)
		{
			if (range < 0) return true;
			if (o is Item)
			{
				Item item = (Item)o;

				if (item.Map != currentmap) return false;

				// is the item in a container?
				// if so, then check the range of the parent rather than the item
				Point3D loc = item.Location;
				if (item.Parent != null && item.RootParent != null)
				{
					if (item.RootParent is Mobile)
					{
						loc = ((Mobile)item.RootParent).Location;
					}
					else
						if (item.RootParent is Container)
						{
							loc = ((Container)item.RootParent).Location;
						}

				}
				return (Utility.InRange(currentloc, loc, range));

			}
			else
				if (o is Mobile)
				{
					Mobile mob = (Mobile)o;

					if (mob.Map != currentmap) return false;
					return (Utility.InRange(currentloc, mob.Location, range));

				}
			return false;
		}

		private static bool TestRegion(object o, string regionname)
		{
			if (regionname == null) return false;
			if (o is Item)
			{
				Item item = (Item)o;
				// is the item in a container?
				// if so, then check the region of the parent rather than the item
				Point3D loc = item.Location;
				if (item.Parent != null && item.RootParent != null)
				{
					if (item.RootParent is Mobile)
					{
						loc = ((Mobile)item.RootParent).Location;
					}
					else
						if (item.RootParent is Container)
						{
							loc = ((Container)item.RootParent).Location;
						}

				}
				//Region r = Region.GetByName(regionname, item.Map);

				Region r = null;

				try
				{
					r = item.Map.Regions[regionname];
				}
				catch { }

				if (r == null) return false;
				return (r.Contains(loc));

			}
			else
				if (o is Mobile)
				{
					Mobile mob = (Mobile)o;

					Region r = null;
					try
					{
						r = mob.Map.Regions[regionname];
					}
					catch { }

					//Region r = Region.GetByName(regionname, mob.Map);
					if (r == null) return false;
					return (r.Contains(mob.Location));

				}
			return false;
		}

		private static bool TestAttach(object o)
		{
			if (XmlAttach.FindAttachments(o) != null) return true;

			return false;
		}

		private static bool TestAge(object o, double age, bool direction)
		{
			if (age <= 0) return true;

			if (o is Mobile)
			{
				Mobile mob = (Mobile)o;

				if (direction)
				{
					// true means allow only mobs greater than the age
					if ((DateTime.UtcNow - mob.CreationTime) > TimeSpan.FromHours(age)) return true;
				}
				else
				{
					// false means allow only mobs less than the age
					if ((DateTime.UtcNow - mob.CreationTime) < TimeSpan.FromHours(age)) return true;
				}

			}
			return false;
		}

		private static void IgnoreManagedInternal(object i, ref ArrayList ignoreList)
		{

			// ignore valid internalized commodity deed items
			if (i is CommodityDeed)
			{
				CommodityDeed deed = (CommodityDeed)i;

				if (deed.Commodity != null && deed.Commodity.Map == Map.Internal)
					ignoreList.Add(deed.Commodity);
			}

			// ignore valid internalized keyring keys
			if (i is KeyRing)
			{
				KeyRing keyring = (KeyRing)i;

				if (keyring.Keys != null)
				{
					foreach (Key k in keyring.Keys)
					{
						ignoreList.Add(k);
					}
				}
			}

			// ignore valid internalized relocatable house items
			if (i is BaseHouse)
			{
				BaseHouse house = (BaseHouse)i;

				foreach (RelocatedEntity relEntity in house.RelocatedEntities)
				{
					if (relEntity.Entity is Item)
						ignoreList.Add(relEntity.Entity);
				}

				foreach (VendorInventory inventory in house.VendorInventories)
				{
					foreach (Item subItem in inventory.Items)
						ignoreList.Add(subItem);
				}
			}
		}

		// test for valid items/mobs on the internal map
		private static bool TestValidInternal(object o)
		{
			if (o is Mobile)
			{
				Mobile m = (Mobile)o;

				if (m.Map != Map.Internal || m.Account != null ||
					(m is IMount && ((IMount)m).Rider != null) ||
                    (m is GalleonPilot) || m is PetParrot ||
                    (GenericBuyInfo.IsDisplayCache(m)) ||
                    (m is EffectMobile) ||
					(m is BaseCreature && ((BaseCreature)m).IsStabled) ||
                    (m is PlayerVendor && BaseHouse.AllHouses.Any(x => x.InternalizedVendors.Contains(m))))
					return true;
			}
			else
				if (o is Item)
				{
					Item i = (Item)o;

					// note, in order to test for a vendors display container that contains valid internal map items 
					if (i.Map != Map.Internal || i.Parent != null || i is Fists || i is MountItem || i is EffectItem || i.HeldBy != null ||
                        i is MovingCrate || i is SpawnPersistence || GenericBuyInfo.IsDisplayCache(i) || i.GetType().DeclaringType == typeof(GenericBuyInfo))
						return true;

                    // boat stuffs
                    if (i is Static && i.Name != null && (i.Name.ToLower() == "weapon pad" || i.Name.ToLower() == "deck"))
                        return true;
                    if (i is GalleonHold || i is MooringLine || i is HoldItem || i is BaseDockedBoat || i is Rudder || i is RudderHandle || i is ShipWheel || i is BaseBoat || i is Plank || i is TillerMan || i is Hold || i is IShipCannon)
                        return true;

                    // Ignores shadowguard addons that are internalized while not in use
                    if (i is AddonComponent)
                    {
                        AddonComponent comp = i as AddonComponent;
                        BaseAddon addon = ((AddonComponent)comp).Addon;

                        if (addon != null && (addon is ArmoryAddon || addon is BarAddon || addon is BelfryAddon || addon is ShadowguardFountainAddon || addon is OrchardAddon))
                            return true;
                    }

                    if (i is BaseAddon && (i is ArmoryAddon || i is BarAddon || i is BelfryAddon || i is ShadowguardFountainAddon || i is OrchardAddon))
                        return true;

                    if (i is BoatMountItem || i is Server.Factions.FactionPersistence || i is Server.Misc.TreasuresOfTokunoPersistence || i is StealableArtifactsSpawner)
                        return true;

                    if (i is ArisenController)
                        return true; 
				}

			return false;
		}

		public static ArrayList Search(SearchCriteria criteria, out string status_str)
		{
			status_str = null;
			ArrayList newarray = new ArrayList();
			ArrayList ignoreList = new ArrayList();

			if (criteria == null)
			{
				status_str = "Empty search criteria";
				return newarray;
			}

			Type targetType = null;
			Type targetattachType = null;

			Map tokunomap = null;
			try
			{
				tokunomap = Map.Parse("Tokuno");
			}
			catch { }

			// if the type is specified then get the search type
			if (criteria.Dosearchtype && criteria.Searchtype != null)
			{
				targetType = SpawnerType.GetType(criteria.Searchtype);
				if (targetType == null)
				{
					status_str = "Invalid type: " + criteria.Searchtype;
					return newarray;
				}
			}

			// if the attachment type is specified then get the search type
			if (criteria.Dosearchattach && criteria.Searchattachtype != null && criteria.Searchattachtype.Length > 0)
			{
				targetattachType = SpawnerType.GetType(criteria.Searchattachtype);
				if (targetattachType == null)
				{
					status_str = "Invalid type: " + criteria.Searchattachtype;
					return newarray;
				}
			}

			// do the search through items

			if (!criteria.Dosearchattach)
			{
				// make a copy so that we dont get enumeration errors if World.Items.Values changes while searching
				ArrayList itemarray = null;

				ICollection itemvalues = World.Items.Values;

				lock (itemvalues.SyncRoot)
				{
					try
					{
						itemarray = new ArrayList(itemvalues);
					}
					catch (SystemException e) { status_str = "Unable to search World.Items: " + e.Message; }
				}

				if (itemarray != null)
				{
					foreach (Item i in itemarray)
					{
						bool hastype = false;
						bool hasname = false;
						bool hasentry = false;
						bool hascondition = false;
						bool hasrange = false;
						bool hasregion = false;
						bool hasmap = false;
						bool hasattach = false;
						bool hasspawnerr = false;
						bool hasvalidhidden = false;


						if (i == null || i.Deleted) continue;

						// this will deal with items that are not on the internal map but hold valid internal items
						if (criteria.Dohidevalidint && i.Map != Map.Internal && i.Map != null)
						{
							IgnoreManagedInternal(i, ref ignoreList);
						}

						// check for map
						if ((i.Map == Map.Felucca && criteria.Dosearchfel) || (i.Map == Map.Trammel && criteria.Dosearchtram) ||
							(i.Map == Map.Malas && criteria.Dosearchmal) || (i.Map == Map.Ilshenar && criteria.Dosearchilsh) || 
                            (i.Map == Map.TerMur && criteria.Dosearchter) || (i.Map == Map.Internal && criteria.Dosearchint) ||
							(i.Map == null && criteria.Dosearchnull))
						{
							hasmap = true;
						}

						if (tokunomap != null && i.Map == tokunomap && criteria.Dosearchtok)
						{
							hasmap = true;
						}


						if (!hasmap) continue;

						// check for type
						if (criteria.Dosearchtype && (i.GetType().IsSubclassOf(targetType) || i.GetType().Equals(targetType)))
						{
							hastype = true;
						}
						if (criteria.Dosearchtype && !hastype) continue;

						// check for name
						if (criteria.Dosearchname && (i.Name != null) && (criteria.Searchname != null) && (i.Name.ToLower().IndexOf(criteria.Searchname.ToLower()) >= 0))
						{
							hasname = true;
						}
						if (criteria.Dosearchname && !hasname) continue;

						// check for valid internal map items
						if (criteria.Dohidevalidint && TestValidInternal(i))
						{
							hasvalidhidden = true;

							// this will deal with items that are on the internal map and hold valid internal items
							IgnoreManagedInternal(i, ref ignoreList);
						}
						if (criteria.Dohidevalidint && hasvalidhidden) continue;

						// check for range
						if (criteria.Dosearchrange && TestRange(i, criteria.Searchrange, criteria.Currentmap, criteria.Currentloc))
						{
							hasrange = true;
						}
						if (criteria.Dosearchrange && !hasrange) continue;

						// check for region
						if (criteria.Dosearchregion && TestRegion(i, criteria.Searchregion))
						{
							hasregion = true;
						}
						if (criteria.Dosearchregion && !hasregion) continue;

						// check for attachments
						if (criteria.Dosearchwithattach && TestAttach(i))
						{
							hasattach = true;
						}
						if (criteria.Dosearchwithattach && !hasattach) continue;

						// check for condition
						if (criteria.Dosearchcondition && (criteria.Searchcondition != null))
						{
							// check the property test
							hascondition = BaseXmlSpawner.CheckPropertyString(null, i, criteria.Searchcondition, null, out status_str);
						}
						if (criteria.Dosearchcondition && !hascondition) continue;

						// check for entry
						if (criteria.Dosearchspawnentry)
						{
							Type targetentrytype = null;

							if (criteria.Dosearchspawntype)
							{
								targetentrytype = SpawnerType.GetType(criteria.Searchspawnentry.ToLower());
							}

							if (criteria.Searchspawnentry == null || (targetentrytype == null && criteria.Dosearchspawntype))
							{
								hasentry = false;
							}
							else
							{
								// see what kind of spawner it is
								if (i is XmlSpawner)
								{

									// search the entries of the spawner
									foreach (XmlSpawner.SpawnObject so in ((XmlSpawner)i).m_SpawnObjects)
									{
										if (criteria.Dosearchspawntype)
										{
											// search by entry type
											Type type = null;

											if (so.TypeName != null)
											{
												string[] args = so.TypeName.Split('/');
												string typestr = null;
												if (args != null && args.Length > 0)
												{
													typestr = args[0];
												}

												type = SpawnerType.GetType(typestr);
											}

											if (type != null && (type == targetentrytype || type.IsSubclassOf(targetentrytype)))
											{
												hasentry = true;
												break;
											}
										}
										else
										{
											// search by entry string
											if (so.TypeName != null && so.TypeName.ToLower().IndexOf(criteria.Searchspawnentry.ToLower()) >= 0)
											{
												hasentry = true;
												break;
											}
										}
									}
								}
								else if (i is Spawner)
								{
									// search the entries of the spawner
									foreach (var obj in ((Spawner)i).SpawnObjects)
									{
                                        string so = obj.SpawnName;

										if (criteria.Dosearchspawntype)
										{
											// search by entry type
											Type type = null;

											if (so != null)
											{
												type = SpawnerType.GetType(so);
											}

											if (type != null && (type == targetentrytype || type.IsSubclassOf(targetentrytype)))
											{
												hasentry = true;
												break;
											}
										}
										else
										{
											if (so != null && so.ToLower().IndexOf(criteria.Searchspawnentry.ToLower()) >= 0)
											{
												hasentry = true;
												break;
											}
										}
									}
								}
								else
								{
									hasentry = false;
								}
							}
						}
						if (criteria.Dosearchspawnentry && !hasentry) continue;

						// check for err
						if (criteria.Dosearcherr)
						{
							// see what kind of spawner it is
							if (i is XmlSpawner)
							{
								// check the status of the spawner
								if (((XmlSpawner)i).status_str != null)
								{
									hasspawnerr = true;
								}
							}
						}
						if (criteria.Dosearcherr && !hasspawnerr) continue;


						// satisfied all conditions so add it
						newarray.Add(new SearchEntry(i));
					}
				}
			}

			// do the search through mobiles
			if (!criteria.Dosearcherr && !criteria.Dosearchattach)
			{
				// make a copy so that we dont get enumeration errors if World.Mobiles.Values changes while searching
				ArrayList mobilearray = null;
				ICollection mobilevalues = World.Mobiles.Values;
				lock (mobilevalues.SyncRoot)
				{
					try
					{
						mobilearray = new ArrayList(mobilevalues);
					}
					catch (SystemException e) { status_str = "Unable to search World.Mobiles: " + e.Message; }
				}

				if (mobilearray != null)
				{
					foreach (Mobile i in mobilearray)
					{
						bool hastype = false;
						bool hasname = false;
						bool hascondition = false;
						bool hasrange = false;
						bool hasregion = false;
						bool hasmap = false;
						bool hasage = false;
						bool hasattach = false;
						bool hasvalidhidden = false;

						if (i == null || i.Deleted) continue;

						// check for map
						if ((i.Map == Map.Felucca && criteria.Dosearchfel) || (i.Map == Map.Trammel && criteria.Dosearchtram) ||
							(i.Map == Map.Malas && criteria.Dosearchmal) || (i.Map == Map.Ilshenar && criteria.Dosearchilsh) || 
                            (i.Map == Map.TerMur && criteria.Dosearchter) || (i.Map == Map.Internal && criteria.Dosearchint) ||
							(i.Map == null && criteria.Dosearchnull))
						{
							hasmap = true;
						}

						if (tokunomap != null && i.Map == tokunomap && criteria.Dosearchtok)
						{
							hasmap = true;
						}

						if (!hasmap) continue;

						// check for range
						if (criteria.Dosearchrange && TestRange(i, criteria.Searchrange, criteria.Currentmap, criteria.Currentloc))
						{
							hasrange = true;
						}
						if (criteria.Dosearchrange && !hasrange) continue;

						// check for region
						if (criteria.Dosearchregion && TestRegion(i, criteria.Searchregion))
						{
							hasregion = true;
						}
						if (criteria.Dosearchregion && !hasregion) continue;

						// check for valid internal map mobiles
						if (criteria.Dohidevalidint && TestValidInternal(i))
						{
							hasvalidhidden = true;
						}
						if (criteria.Dohidevalidint && hasvalidhidden) continue;

						// check for age
						if (criteria.Dosearchage && TestAge(i, criteria.Searchage, criteria.Searchagedirection))
						{
							hasage = true;
						}
						if (criteria.Dosearchage && !hasage) continue;

						// check for type
						if (criteria.Dosearchtype && (i.GetType().IsSubclassOf(targetType) || i.GetType().Equals(targetType)))
						{
							hastype = true;
						}
						if (criteria.Dosearchtype && !hastype) continue;

						// check for name
						if (criteria.Dosearchname && (i.Name != null) && (criteria.Searchname != null) && (i.Name.ToLower().IndexOf(criteria.Searchname.ToLower()) >= 0))
						{
							hasname = true;
						}
						if (criteria.Dosearchname && !hasname) continue;

						// check for attachments
						if (criteria.Dosearchwithattach && TestAttach(i))
						{
							hasattach = true;
						}
						if (criteria.Dosearchwithattach && !hasattach) continue;

						// check for condition
						if (criteria.Dosearchcondition && (criteria.Searchcondition != null))
						{
							// check the property test
							hascondition = BaseXmlSpawner.CheckPropertyString(null, i, criteria.Searchcondition, null, out status_str);
						}
						if (criteria.Dosearchcondition && !hascondition) continue;

						// passed all conditions so add it to the list

						newarray.Add(new SearchEntry(i));
					}
				}

			}

			// need to keep track of valid internalized XmlSaveItem items
			if (criteria.Dohidevalidint)
			{
				foreach (XmlAttachment i in XmlAttach.Values)
				{
					if (i is XmlSaveItem)
					{
						XmlSaveItem s = (XmlSaveItem)i;
						if (s.Container != null)
						{
							ignoreList.Add(s.Container);
						}
					}
				}
			}

			if (criteria.Dosearchattach)
			{

				foreach (XmlAttachment i in XmlAttach.Values)
				{
					// check for type
					if (i != null && !i.Deleted && (targetattachType == null || i.GetType().IsSubclassOf(targetattachType) || i.GetType().Equals(targetattachType)))
					{
						newarray.Add(new SearchEntry(i));
					}
				}
			}

			ArrayList removelist = new ArrayList();
			for (int i = 0; i < ignoreList.Count; ++i)
			{
				foreach (SearchEntry se in newarray)
				{
					if (se.Object == ignoreList[i])
					{
						removelist.Add(se);
						break;
					}
				}
			}

			foreach (SearchEntry se in removelist)
			{
				newarray.Remove(se);
			}

			return newarray;
		}

		[Usage("XmlFind [objecttype] [range]")]
		[Description("Finds objects in the world")]
		public static void XmlFind_OnCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null) return;

			Account acct = e.Mobile.Account as Account;
			int x = 0;
			int y = 0;
			XmlSpawnerDefaults.DefaultEntry defs = null;
			if (acct != null)
				defs = XmlSpawnerDefaults.GetDefaults(acct.ToString(), e.Mobile.Name);

			if (defs != null)
			{
				x = defs.FindGumpX;
				y = defs.FindGumpY;
			}

			string typename = "Xmlspawner";
			int range = -1;
			bool dorange = false;

			if (e.Arguments.Length > 0)
			{
				typename = e.Arguments[0];
			}

			if (e.Arguments.Length > 1)
			{
				dorange = true;
				try
				{
					range = int.Parse(e.Arguments[1]);
				}
				catch
				{
					dorange = false;
					e.Mobile.SendMessage("Invalid range argument {0}", e.Arguments[1]);
				}
			}

			e.Mobile.SendGump(new XmlFindGump(e.Mobile, e.Mobile.Location, e.Mobile.Map, typename, range, dorange, x, y));
		}

		public XmlFindGump(Mobile from, Point3D startloc, Map startmap, int x, int y)
			: this(from, startloc, startmap, null, x, y)
		{
		}

		public XmlFindGump(Mobile from, Point3D startloc, Map startmap, string type, int x, int y)
			: this(from, startloc, startmap, type, -1, false, x, y)
		{
		}

		public XmlFindGump(Mobile from, Point3D startloc, Map startmap, string type, int range, bool dorange, int x, int y)
			: this(from, startloc, startmap, true, false, false,

			new SearchCriteria(
			true, // dotype
			false, // doname
			dorange, // dorange
			false, // doregion
			false, // doentry
			false, // doentrytype
			false, // docondition
			true, // dofel
			true, // dotram
			true, // domal
			true, // doilsh
			true, // dotok
            true, // doter
			false, // doint
			false, // donull
			false, // doerr
			false, // doage
			false, // dowithattach
			false, // doattach
			false, // dohidevalid
			true, // agedirection
			0, // age
			range, // range
			null, // region
			null, // condition
			type, // type
			null, // attachtype
			null, // name
			null // entry 
			),

			null, -1, 0, null, null,
			false, false, false, false, false, false, x, y)
		{
		}


		public XmlFindGump(Mobile from, Point3D startloc, Map startmap, bool firststart, bool extension, bool descend, SearchCriteria criteria, ArrayList searchlist, int selected, int displayfrom, string savefilename,
			string commandstring, bool sorttype, bool sortname, bool sortrange, bool sortmap, bool sortselect, bool selectall, int X, int Y)
			: base(X, Y)
		{

			StartingMap = startmap;
			StartingLoc = startloc;
			if (from != null && !from.Deleted)
			{
				m_From = from;
				if (firststart)
				{
					StartingMap = from.Map;
					StartingLoc = from.Location;
				}
			}

			SaveFilename = savefilename;
			CommandString = commandstring;
			SelectAll = selectall;
			Sorttype = sorttype;
			Sortname = sortname;
			Sortrange = sortrange;
			Sortmap = sortmap;
			Sortselect = sortselect;
			DisplayFrom = displayfrom;
			Selected = selected;
			m_ShowExtension = extension;
			Descendingsort = descend;

			m_SearchCriteria = criteria;

			if (m_SearchCriteria == null) m_SearchCriteria = new SearchCriteria();

			m_SearchList = searchlist;

			// prepare the page
			int height = 500;
			int y = 0;

			AddPage(0);
			if (m_ShowExtension)
			{
				AddBackground(0, 0, 755, height, 5054);
				AddAlphaRegion(0, 0, 755, height);
			}
			else
			{
				AddBackground(0, 0, 170, height, 5054);
				AddAlphaRegion(0, 0, 170, height);
			}


			// Close button
			//AddButton( 5, 450, 0xFB1, 0xFB3, 0, GumpButtonType.Reply, 0 );
			//AddLabel( 38, 450, 0x384, "Close" );

			// add the SubSearch button
			//AddButton( 90, 160, 0xFA8, 0xFAA, 4, GumpButtonType.Reply, 0 );
			//AddLabel( 128, 160, 0x384, "SubSearch" );

			// ----------------
			// SORT section
			// ----------------
			y = 5;
			// add the Sort button
			AddButton(5, y, 0xFAB, 0xFAD, 700, GumpButtonType.Reply, 0);
			AddLabel(38, y, 0x384, "Sort");

			// add the sort direction button
			if (Descendingsort)
			{
				AddButton(75, y + 3, 0x15E2, 0x15E6, 701, GumpButtonType.Reply, 0);
				AddLabel(95, y, 0x384, "descend");
			}
			else
			{
				AddButton(75, y + 3, 0x15E0, 0x15E4, 701, GumpButtonType.Reply, 0);
				AddLabel(95, y, 0x384, "ascend");
			}
			y += 22;
			// add the Sort on type toggle
			AddRadio(5, y, 0xD2, 0xD3, Sorttype, 0);
			AddLabel(28, y, 0x384, "type");

			// add the Sort on name toggle
			AddRadio(75, y, 0xD2, 0xD3, Sortname, 1);
			AddLabel(98, y, 0x384, "name");

			y += 20;
			// add the Sort on range toggle
			AddRadio(5, y, 0xD2, 0xD3, Sortrange, 2);
			AddLabel(28, y, 0x384, "range");

			// add the Sort on map toggle
			AddRadio(75, y, 0xD2, 0xD3, Sortmap, 4);
			AddLabel(98, y, 0x384, "map");

			y += 20;
			// add the Sort on selected toggle
			AddRadio(5, y, 0xD2, 0xD3, Sortselect, 5);
			AddLabel(28, y, 0x384, "select");

			// ----------------
			// SEARCH section
			// ----------------
			y = 85;
			// add the Search button
			AddButton(5, y, 0xFA8, 0xFAA, 3, GumpButtonType.Reply, 0);
			AddLabel(38, y, 0x384, "Search");

			y += 20;
			// add the map buttons
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchint, 312);
			AddLabel(28, y, 0x384, "Int");
			AddCheck(75, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchnull, 314);
			AddLabel(98, y, 0x384, "Null");

			y += 20;
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchfel, 308);
			AddLabel(28, y, 0x384, "Fel");
			AddCheck(75, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchtram, 309);
			AddLabel(98, y, 0x384, "Tram");

			y += 20;
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchmal, 310);
			AddLabel(28, y, 0x384, "Mal");
			AddCheck(75, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchilsh, 311);
			AddLabel(98, y, 0x384, "Ilsh");

			y += 20;
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchtok, 318);
			AddLabel(28, y, 0x384, "Tok");
            AddCheck(75, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchter, 320);
            AddLabel(98, y, 0x384, "Ter");

			y += 20;
			// add the hide valid internal map button
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dohidevalidint, 316);
			AddLabel(28, y, 0x384, "Hide valid internal");

			// ----------------
			// FILTER section
			// ----------------
			y = height - 295;

			// add the search region entry
			AddLabel(28, y, 0x384, "region");
			AddImageTiled(70, y, 68, 19, 0xBBC);
			AddTextEntry(70, y, 250, 19, 0, 106, m_SearchCriteria.Searchregion);
			// add the toggle to enable search region
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchregion, 319);

			y += 20;
			// add the search age entry
			AddLabel(28, y, 0x384, "age");
			//AddImageTiled( 80, 220, 50, 23, 0x52 );
			AddImageTiled(70, y, 45, 19, 0xBBC);
			AddTextEntry(70, y, 45, 19, 0, 105, m_SearchCriteria.Searchage.ToString());
			AddLabel(117, y, 0x384, "Hrs");
			// add the toggle to enable search age
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchage, 303);
			// add the toggle to set the search age test direction
			AddCheck(50, y + 2, 0x1467, 0x1468, m_SearchCriteria.Searchagedirection, 302);

			y += 20;
			// add the search range entry
			AddLabel(28, y, 0x384, "range");
			AddImageTiled(70, y, 45, 19, 0xBBC);
			AddTextEntry(70, y, 45, 19, 0, 100, m_SearchCriteria.Searchrange.ToString());
			// add the toggle to enable search range
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchrange, 304);

			y += 20;
			// add the search type entry
			AddLabel(28, y, 0x384, "type");
			// add the toggle to enable search by type
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchtype, 305);
			//AddImageTiled( 5, 285, 135, 23, 0x52 );
			AddImageTiled(6, y + 20, 132, 19, 0xBBC);
			AddTextEntry(6, y + 20, 250, 19, 0, 101, m_SearchCriteria.Searchtype);

			// add the search for attachments button
			AddLabel(100, y, 0x384, "attach");
			// add the toggle to enable search by attachment
			AddCheck(77, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchwithattach, 317);

			y += 41;
			// add the search condition entry
			AddLabel(28, y, 0x384, "property test");
			// add the toggle to enable search by condition
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchcondition, 315);
			//AddImageTiled( 5, 285, 135, 23, 0x52 );
			AddImageTiled(6, y + 20, 132, 19, 0xBBC);
			AddTextEntry(6, y + 20, 500, 19, 0, 104, m_SearchCriteria.Searchcondition);

			y += 41;
			// add the search name entry
			AddLabel(28, y, 0x384, "name");
			// add the toggle to enable search by name
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchname, 306);
			//AddImageTiled( 5, 350, 135, 23, 0x52 );
			AddImageTiled(6, y + 20, 132, 19, 0xBBC);
			AddTextEntry(6, y + 20, 250, 19, 0, 102, m_SearchCriteria.Searchname);

			y += 41;
			// add the search attachment type entry
			AddLabel(28, y, 0x384, "attachment type");
			// add the toggle to enable search by attachment type
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchattach, 325);
			//AddImageTiled( 5, 285, 135, 23, 0x52 );
			AddImageTiled(6, y + 20, 132, 19, 0xBBC);
			AddTextEntry(6, y + 20, 250, 19, 0, 125, m_SearchCriteria.Searchattachtype);

			y += 41;
			// add the search spawner entries
			AddLabel(28, y, 0x384, "entry");
			// add the toggle to enable search spawner entries
			AddCheck(5, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchspawnentry, 307);

			// add the search spawner entries by type
			AddLabel(88, y, 0x384, "type");
			// add the toggle to enable search spawner entry types
			AddCheck(65, y, 0xD2, 0xD3, m_SearchCriteria.Dosearchspawntype, 326);

			//AddImageTiled( 5, 415, 135, 23, 0x52 );
			AddImageTiled(6, y + 20, 132, 19, 0xBBC);
			AddTextEntry(6, y + 20, 250, 19, 0, 103, m_SearchCriteria.Searchspawnentry);

			// add the search spawner errors
			AddLabel(140, y, 0x384, "err");
			// add the toggle to enable search spawner entries
			AddCheck(117, y, 0xD2, 0xD3, m_SearchCriteria.Dosearcherr, 313);

			// add the Show Map button
			//AddButton( 5, 450, 0xFAB, 0xFAD, 150, GumpButtonType.Reply, 0 );
			//AddLabel( 38, 450, 0x384, "Map" );

			// ----------------
			// CONTROL section
			// ----------------

			y = height - 25;
			// add the Return button
			AddButton(72, y, 0xFAE, 0xFAF, 155, GumpButtonType.Reply, 0);
			AddLabel(105, y, 0x384, "Return");

			y = height - 25;
			// add the Bring button
			AddButton(5, y, 0xFAE, 0xFAF, 154, GumpButtonType.Reply, 0);
			AddLabel(38, y, 0x384, "Bring");


			// add gump extension button
			if (m_ShowExtension)
			{
				AddButton(720, y + 5, 0x15E3, 0x15E7, 200, GumpButtonType.Reply, 0);
			}
			else
			{
				AddButton(150, y + 5, 0x15E1, 0x15E5, 200, GumpButtonType.Reply, 0);
			}

			if (m_ShowExtension)
			{
				AddLabel(143, 5, 0x384, "Gump");
				AddLabel(178, 5, 0x384, "Prop");
				AddLabel(210, 5, 0x384, "Goto");
				AddLabel(250, 5, 0x384, "Name");
				AddLabel(365, 5, 0x384, "Type");
				AddLabel(460, 5, 0x384, "Location");
				AddLabel(578, 5, 0x384, "Map");
				AddLabel(650, 5, 0x384, "Owner");

				// add the Delete button
				AddButton(150, y, 0xFB1, 0xFB3, 156, GumpButtonType.Reply, 0);
				AddLabel(183, height - 25, 0x384, "Delete");

				// add the Reset button
				AddButton(230, y, 0xFA2, 0xFA3, 157, GumpButtonType.Reply, 0);
				AddLabel(263, y, 0x384, "Reset");

				// add the Respawn button
				AddButton(310, y, 0xFA8, 0xFAA, 158, GumpButtonType.Reply, 0);
				AddLabel(343, y, 0x384, "Respawn");

				// add the xmlsave entry
				AddButton(150, y - 25, 0xFA8, 0xFAA, 159, GumpButtonType.Reply, 0);
				AddLabel(183, y - 25, 0x384, "Save to file:");

				AddImageTiled(270, y - 25, 180, 19, 0xBBC);
				AddTextEntry(270, y - 25, 180, 19, 0, 300, SaveFilename);

				// add the commandstring entry
				AddButton(470, y - 25, 0xFA8, 0xFAA, 160, GumpButtonType.Reply, 0);
				AddLabel(503, y - 25, 0x384, "Command:");

				AddImageTiled(560, y - 25, 180, 19, 0xBBC);
				AddTextEntry(560, y - 25, 180, 19, 0, 301, CommandString);


				// add the page buttons
				for (int i = 0; i < (int)(MaxEntries / MaxEntriesPerPage); i++)
				{
					//AddButton( 38+i*30, 365, 2206, 2206, 0, GumpButtonType.Page, 1+i );
					AddButton(418 + i * 25, height - 25, 0x8B1 + i, 0x8B1 + i, 0, GumpButtonType.Page, 1 + i);
				}

				// add the advance pageblock buttons
				AddButton(415 + 25 * (int)(MaxEntries / MaxEntriesPerPage), height - 25, 0x15E1, 0x15E5, 201, GumpButtonType.Reply, 0); // block forward
				AddButton(395, height - 25, 0x15E3, 0x15E7, 202, GumpButtonType.Reply, 0); // block backward

				// add the displayfrom entry
				AddLabel(460, y, 0x384, "Display");
				AddImageTiled(500, y, 60, 21, 0xBBC);
				AddTextEntry(501, y, 60, 21, 0, 400, DisplayFrom.ToString());
				AddButton(560, y, 0xFAB, 0xFAD, 9998, GumpButtonType.Reply, 0);

				// display the item list
				if (m_SearchList != null)
				{
					AddLabel(180, y - 50, 68, String.Format("Found {0} items/mobiles", m_SearchList.Count));
					AddLabel(400, y - 50, 68, String.Format("Displaying {0}-{1}", DisplayFrom,
						(DisplayFrom + MaxEntries < m_SearchList.Count ? DisplayFrom + MaxEntries : m_SearchList.Count)));
					// count the number of selected objects
					int count = 0;
					foreach (SearchEntry e in m_SearchList)
					{
						if (e.Selected) count++;
					}
					AddLabel(600, y - 50, 33, String.Format("Selected {0}", count));
				}

				// display the select-all-displayed toggle
				AddButton(730, 5, 0xD2, 0xD3, 3999, GumpButtonType.Reply, 0);

				AddLabel(610, y, 0x384, "Select All");
				// display the select-all toggle
				AddButton(670, y, (SelectAll ? 0xD3 : 0xD2), (SelectAll ? 0xD2 : 0xD3), 3998, GumpButtonType.Reply, 0);

				for (int i = 0; i < MaxEntries; i++)
				{
					int index = i + DisplayFrom;
					if (m_SearchList == null || index >= m_SearchList.Count) break;

					SearchEntry e = (SearchEntry)m_SearchList[index];

					int page = (int)(i / MaxEntriesPerPage);

					if (i % MaxEntriesPerPage == 0)
					{
						AddPage(page + 1);
						// add highlighted page button
						//AddImageTiled( 235+page*25, 448, 25, 25, 0xBBC );
						//AddImage( 238+page*25, 450, 0x8B1+page );
					}

					// background for search results area
					AddImageTiled(235, 22 * (i % MaxEntriesPerPage) + 30, 386, 23, 0x52);
					AddImageTiled(236, 22 * (i % MaxEntriesPerPage) + 31, 384, 21, 0xBBC);

					// add the Goto button for each entry
					AddButton(205, 22 * (i % MaxEntriesPerPage) + 30, 0xFAE, 0xFAF, 1000 + i, GumpButtonType.Reply, 0);

					object o = e.Object;

					// add the Gump button for spawner entries
					if (o is XmlSpawner || o is Spawner)
					{
						AddButton(145, 22 * (i % MaxEntriesPerPage) + 30, 0xFBD, 0xFBE, 2000 + i, GumpButtonType.Reply, 0);
					}

					// add the Props button for each entry
					AddButton(175, 22 * (i % MaxEntriesPerPage) + 30, 0xFAB, 0xFAD, 3000 + i, GumpButtonType.Reply, 0);

					string namestr = string.Empty;
                    string typestr = string.Empty;
                    string locstr = string.Empty;
                    string mapstr = string.Empty;
                    string ownstr = string.Empty;
					int texthue = 0;

					if (o is Item)
					{
						Item item = (Item)e.Object;
						// change the color if it is in a container
						namestr = item.Name;
						string str = item.GetType().ToString();
						if (str != null)
						{
							string[] arglist = str.Split('.');
							typestr = arglist[arglist.Length - 1];
						}
						// check for in container
						// if so then display parent loc
						// change the color for container held items
						if (item.Parent != null)
						{

							if (item.RootParent is Mobile)
							{

								Mobile m = item.RootParent as Mobile;
								if (m.Player)
									texthue = 44;
								else
									texthue = 24;
								locstr = m.Location.ToString();
								ownstr = m.Name;
							}
							else
								if (item.RootParent is Container)
								{
									texthue = 5;
									Container c = item.RootParent as Container;
									locstr = c.Location.ToString();
									if (c.Name != null)
									{
										ownstr = c.Name;
									}
									else
									{
										ownstr = c.ItemData.Name;
									}
								}

						}
						else
						{
							locstr = item.Location.ToString();
						}

						if (item.Deleted)
							mapstr = "Deleted";
						else
							if (item.Map != null)
								mapstr = item.Map.ToString();

					}
					else
						if (o is Mobile)
						{
							Mobile mob = (Mobile)e.Object;
							// change the color if it is in a container
							namestr = mob.Name;
							string str = mob.GetType().ToString();
							if (str != null)
							{
								string[] arglist = str.Split('.');
								typestr = arglist[arglist.Length - 1];
							}
							locstr = mob.Location.ToString();
							if (mob.Deleted)
								mapstr = "Deleted";
							else
								if (mob.Map != null)
									mapstr = mob.Map.ToString();

						}
						else
							if (o is XmlAttachment)
							{
								XmlAttachment a = (XmlAttachment)e.Object;
								// change the color
								namestr = a.Name;

								string str = a.GetType().ToString();
								if (a.Owner is Mobile)
								{
									Mobile m = a.Owner as Mobile;

									ownstr = m.Name;
									if (m.Player)
										texthue = 44;
									else
										texthue = 24;
								}
								else
									if (a.Owner is Item)
									{
										Item item = a.Owner as Item;

										texthue = 50;
										if (item.Name != null)
										{
											ownstr = item.Name;
										}
										else
										{
											ownstr = item.ItemData.Name;
										}
									}
								if (str != null)
								{
									string[] arglist = str.Split('.');
									typestr = arglist[arglist.Length - 1];
								}
								if (a.AttachedTo is Mobile)
								{
									Mobile m = (Mobile)a.AttachedTo;
									locstr = m.Location.ToString();
									if (m.Map != null)
										mapstr = m.Map.ToString();
								}
								if (a.AttachedTo is Item)
								{
									Item item = (Item)a.AttachedTo;
									if (item.Map != null)
										mapstr = item.Map.ToString();
									if (item.Parent != null)
									{
										if (item.RootParent is Mobile)
										{
											locstr = ((Mobile)item.RootParent).Location.ToString();

										}
										else
											if (item.RootParent is Item)
											{
												locstr = ((Item)item.RootParent).Location.ToString();
											}
									}
									else
										locstr = item.Location.ToString();
								}
								if (a.Deleted)
								{
									mapstr = "Deleted";
								}

							}

					if (e.Selected) texthue = 33;

					if (i == Selected) texthue = 68;

					// display the name
					AddLabelCropped(248, 22 * (i % MaxEntriesPerPage) + 31, 110, 21, texthue, namestr ?? string.Empty);

					// display the attachment button if it has attachments
					if (XmlAttach.HasAttachments(o))
					{
						AddButton(238, 22 * (i % MaxEntriesPerPage) + 35, 2103, 2103, 5000 + i, GumpButtonType.Reply, 0);
					}

					// display the type
					AddImageTiled(360, 22 * (i % MaxEntriesPerPage) + 31, 90, 21, 0xBBC);
					AddLabelCropped(360, 22 * (i % MaxEntriesPerPage) + 31, 90, 21, texthue, typestr);
					// display the loc
					AddImageTiled(450, 22 * (i % MaxEntriesPerPage) + 31, 137, 21, 0xBBC);
					AddLabel(450, 22 * (i % MaxEntriesPerPage) + 31, texthue, locstr);
					// display the map
					AddImageTiled(571, 22 * (i % MaxEntriesPerPage) + 31, 70, 21, 0xBBC);
					AddLabel(571, 22 * (i % MaxEntriesPerPage) + 31, texthue, mapstr);
					// display the owner
					AddImageTiled(640, 22 * (i % MaxEntriesPerPage) + 31, 90, 21, 0xBBC);
					AddLabelCropped(640, 22 * (i % MaxEntriesPerPage) + 31, 90, 21, texthue, ownstr);

					// display the selection button

					AddButton(730, 22 * (i % MaxEntriesPerPage) + 32, (e.Selected ? 0xD3 : 0xD2), (e.Selected ? 0xD2 : 0xD3), 4000 + i, GumpButtonType.Reply, 0);

				}
			}
		}

		private void DoGoTo(int index)
		{
			if (m_From == null || m_From.Deleted) return;

			if (m_SearchList != null && index < m_SearchList.Count)
			{
				object o = ((SearchEntry)m_SearchList[index]).Object;
				if (o is Item)
				{
					Item item = (Item)o;
					Point3D itemloc;
					if (item.Parent != null)
					{
						if (item.RootParent is Mobile)
						{
							itemloc = ((Mobile)(item.RootParent)).Location;
						}
						else
							if (item.RootParent is Container)
							{
								itemloc = ((Container)(item.RootParent)).Location;
							}
							else
							{
								return;
							}
					}
					else
					{
						itemloc = item.Location;
					}
					if (item == null || item.Deleted || item.Map == null || item.Map == Map.Internal) return;
					m_From.Location = itemloc;
					m_From.Map = item.Map;

				}
				else
					if (o is Mobile)
					{
						Mobile mob = (Mobile)o;
						if (mob == null || mob.Deleted || mob.Map == null || mob.Map == Map.Internal) return;
						m_From.Location = mob.Location;
						m_From.Map = mob.Map;
					}
					else
						if (o is XmlAttachment)
						{
							XmlAttachment a = (XmlAttachment)o;
							if (a == null || a.Deleted) return;
							if (a.AttachedTo is Mobile)
							{
								Mobile mob = (Mobile)a.AttachedTo;
								if (mob == null || mob.Deleted || mob.Map == null || mob.Map == Map.Internal) return;
								m_From.Location = mob.Location;
								m_From.Map = mob.Map;
							}
							else
								if (a.AttachedTo is Item)
								{
									Item item = (Item)a.AttachedTo;
									Point3D itemloc;
									if (item.Parent != null)
									{
										if (item.RootParent is Mobile)
										{
											itemloc = ((Mobile)(item.RootParent)).Location;
										}
										else
											if (item.RootParent is Container)
											{
												itemloc = ((Container)(item.RootParent)).Location;
											}
											else
											{
												return;
											}
									}
									else
									{
										itemloc = item.Location;
									}
									if (item == null || item.Deleted || item.Map == null || item.Map == Map.Internal) return;
									m_From.Location = itemloc;
									m_From.Map = item.Map;
								}
						}
			}
		}

		private void DoShowGump(int index)
		{
			if (m_From == null || m_From.Deleted) return;

			if (m_SearchList != null && index < m_SearchList.Count)
			{
				object o = ((SearchEntry)m_SearchList[index]).Object;
				if (o is XmlSpawner)
				{
					// dont open anything with a null map null item or deleted
					XmlSpawner x = (XmlSpawner)o;
					if (x == null || x.Deleted || x.Map == null || x.Map == Map.Internal) return;
					x.OnDoubleClick(m_From);
				}
				else
					if (o is Spawner)
					{
						Spawner x = (Spawner)o;
						if (x == null || x.Deleted || x.Map == null || x.Map == Map.Internal) return;
						x.OnDoubleClick(m_From);
					}
			}
		}

		private void DoShowProps(int index)
		{
			if (m_From == null || m_From.Deleted) return;

			if (m_SearchList != null && index < m_SearchList.Count)
			{
				object o = ((SearchEntry)m_SearchList[index]).Object;
				if (o is Item)
				{
					Item x = (Item)o;
					if (x == null || x.Deleted /*|| x.Map == null*/) return;
					m_From.SendGump(new PropertiesGump(m_From, o));
				}
				else
					if (o is Mobile)
					{
						Mobile x = (Mobile)o;
						if (x == null || x.Deleted /*|| x.Map == null*/) return;
						m_From.SendGump(new PropertiesGump(m_From, o));
					}
					else
						if (o is XmlAttachment)
						{
							XmlAttachment x = (XmlAttachment)o;
							if (x == null || x.Deleted /*|| x.Map == null*/) return;
							m_From.SendGump(new PropertiesGump(m_From, o));
						}

			}
		}

		private void SortFindList()
		{
			if (m_SearchList != null && m_SearchList.Count > 0)
			{
				if (Sorttype)
				{
					this.m_SearchList.Sort(new ListTypeSorter(Descendingsort));
				}
				else
					if (Sortname)
					{
						this.m_SearchList.Sort(new ListNameSorter(Descendingsort));
					}
					else
						if (Sortmap)
						{
							this.m_SearchList.Sort(new ListMapSorter(Descendingsort));
						}
						else
							if (Sortrange)
							{
								this.m_SearchList.Sort(new ListRangeSorter(m_From, Descendingsort));
							}
							else
								if (Sortselect)
								{
									this.m_SearchList.Sort(new ListSelectSorter(m_From, Descendingsort));
								}
			}
		}

		private class ListTypeSorter : IComparer
		{
			private bool Dsort;

			public ListTypeSorter(bool descend)
				: base()
			{
				Dsort = descend;
			}

			public int Compare(object e1, object e2)
			{
				object x = null;
				object y = null;
				if (e1 is SearchEntry)
					x = ((SearchEntry)e1).Object;
				if (e2 is SearchEntry)
					y = ((SearchEntry)e2).Object;

				string xstr = null;
				string ystr = null;
				string str = null;
				if (x is Item)
				{
					str = ((Item)x).GetType().ToString();
				}
				else
					if (x is Mobile)
					{
						str = ((Mobile)x).GetType().ToString();
					}
				if (str != null)
				{
					string[] arglist = str.Split('.');
					xstr = arglist[arglist.Length - 1];
				}

				str = null;
				if (y is Item)
				{
					str = ((Item)y).GetType().ToString();
				}
				else
					if (y is Mobile)
					{
						str = ((Mobile)y).GetType().ToString();
					}
				if (str != null)
				{
					string[] arglist = str.Split('.');
					ystr = arglist[arglist.Length - 1];
				}
				if (Dsort)
					return String.Compare(ystr, xstr, true);
				else
					return String.Compare(xstr, ystr, true);
			}
		}

		private class ListNameSorter : IComparer
		{
			private bool Dsort;

			public ListNameSorter(bool descend)
				: base()
			{
				Dsort = descend;
			}

			public int Compare(object e1, object e2)
			{
				object x = null;
				object y = null;
				if (e1 is SearchEntry)
					x = ((SearchEntry)e1).Object;
				if (e2 is SearchEntry)
					y = ((SearchEntry)e2).Object;

				string xstr = null;
				string ystr = null;

				if (x is Item)
				{
					xstr = ((Item)x).Name;
				}
				else
					if (x is Mobile)
					{
						xstr = ((Mobile)x).Name;
					}

				if (y is Item)
				{
					ystr = ((Item)y).Name;
				}
				else
					if (y is Mobile)
					{
						ystr = ((Mobile)y).Name;
					}
				if (Dsort)
					return String.Compare(ystr, xstr, true);
				else
					return String.Compare(xstr, ystr, true);
			}
		}

		private class ListMapSorter : IComparer
		{
			private bool Dsort;

			public ListMapSorter(bool descend)
				: base()
			{
				Dsort = descend;
			}

			public int Compare(object e1, object e2)
			{
				object x = null;
				object y = null;
				if (e1 is SearchEntry)
					x = ((SearchEntry)e1).Object;
				if (e2 is SearchEntry)
					y = ((SearchEntry)e2).Object;

				string xstr = null;
				string ystr = null;

				if (x is Item)
				{
					if (((Item)x).Map != null)
						xstr = ((Item)x).Map.ToString();
				}
				else
					if (x is Mobile)
					{
						if (((Mobile)x).Map != null)
							xstr = ((Mobile)x).Map.ToString();
					}

				if (y is Item)
				{
					if (((Item)y).Map != null)
						ystr = ((Item)y).Map.ToString();
				}
				else
					if (y is Mobile)
					{
						if (((Mobile)y).Map != null)
							ystr = ((Mobile)y).Map.ToString();
					}
				if (Dsort)
					return String.Compare(ystr, xstr, true);
				else
					return String.Compare(xstr, ystr, true);
			}
		}

		private class ListEntrySorter : IComparer
		{
			private bool Dsort;

			public ListEntrySorter(bool descend)
				: base()
			{
				Dsort = descend;
			}

			public int Compare(object x, object y)
			{
				return 0;
			}
		}

		private class ListRangeSorter : IComparer
		{
			private Mobile From;
			private bool Dsort;

			public ListRangeSorter(Mobile from, bool descend)
				: base()
			{
				From = from;
				Dsort = descend;
			}

			public int Compare(object e1, object e2)
			{
				object x = null;
				object y = null;
				if (e1 is SearchEntry)
					x = ((SearchEntry)e1).Object;
				if (e2 is SearchEntry)
					y = ((SearchEntry)e2).Object;

				Map xmap = null;
				Map ymap = null;
				Point3D xloc = new Point3D(0, 0, 0);
				Point3D yloc = new Point3D(0, 0, 0);

				if (From == null || From.Deleted) return 0;
				if (x is Item)
				{
					xmap = ((Item)x).Map;
					xloc = ((Item)x).Location;
				}
				else
					if (x is Mobile)
					{
						xmap = ((Mobile)x).Map;
						xloc = ((Mobile)x).Location;
					}

				if (y is Item)
				{
					ymap = ((Item)y).Map;
					yloc = ((Item)y).Location;
				}
				else
					if (y is Mobile)
					{
						ymap = ((Mobile)y).Map;
						yloc = ((Mobile)y).Location;
					}

				if (xmap != From.Map && ymap != From.Map) return 0;


				if (Dsort)
				{
					if (xmap == From.Map && ymap != From.Map) return 1;
					if (xmap != From.Map && ymap == From.Map) return -1;
					return From.GetDistanceToSqrt(yloc).CompareTo(From.GetDistanceToSqrt(xloc));
				}
				else
				{
					if (xmap == From.Map && ymap != From.Map) return -1;
					if (xmap != From.Map && ymap == From.Map) return 1;
					return From.GetDistanceToSqrt(xloc).CompareTo(From.GetDistanceToSqrt(yloc));
				}
			}
		}

		private class ListSelectSorter : IComparer
		{
			private Mobile From;
			private bool Dsort;

			public ListSelectSorter(Mobile from, bool descend)
				: base()
			{
				From = from;
				Dsort = descend;
			}

			public int Compare(object e1, object e2)
			{
				int x = 0;
				int y = 0;
				if (e1 is SearchEntry)
					x = ((SearchEntry)e1).Selected ? 1 : 0;
				if (e2 is SearchEntry)
					y = ((SearchEntry)e2).Selected ? 1 : 0;


				if (Dsort)
				{
					return x - y;
				}
				else
				{
					return y - x;
				}
			}
		}

		private void Refresh(NetState state)
		{
			state.Mobile.SendGump(new XmlFindGump(this.m_From, this.StartingLoc, this.StartingMap, false, this.m_ShowExtension, this.Descendingsort, this.m_SearchCriteria, this.m_SearchList, this.Selected, this.DisplayFrom, this.SaveFilename,
				this.CommandString, this.Sorttype, this.Sortname, this.Sortrange,
				this.Sortmap, this.Sortselect, this.SelectAll, this.X, this.Y));
		}

		private void ResetList()
		{
			if (m_SearchList == null) return;

			for (int i = 0; i < m_SearchList.Count; i++)
			{
				SearchEntry e = (SearchEntry)m_SearchList[i];

				if (e.Selected)
				{
					object o = e.Object;

					if (o is XmlSpawner)
					{
						((XmlSpawner)o).DoReset = true;
					}
				}
			}
		}

		private void RespawnList()
		{
			if (m_SearchList == null) return;

			for (int i = 0; i < m_SearchList.Count; i++)
			{
				SearchEntry e = (SearchEntry)m_SearchList[i];

				if (e.Selected)
				{
					object o = e.Object;

					if (o is XmlSpawner)
					{
						((XmlSpawner)o).DoRespawn = true;
					}
				}
			}
		}

		private void SaveList(Mobile from, string filename)
		{
			if (m_SearchList == null) return;

			string dirname;
			if (System.IO.Directory.Exists(XmlSpawner.XmlSpawnDir) && filename != null && !filename.StartsWith("/") && !filename.StartsWith("\\"))
			{
				// put it in the defaults directory if it exists
				dirname = String.Format("{0}/{1}", XmlSpawner.XmlSpawnDir, filename);
			}
			else
			{
				// otherwise just put it in the main installation dir
				dirname = filename;
			}


			List<XmlSpawner> savelist = new List<XmlSpawner>();

			for (int i = 0; i < m_SearchList.Count; i++)
			{
				SearchEntry e = (SearchEntry)m_SearchList[i];

				if (e.Selected)
				{
					object o = e.Object;

					if (o is XmlSpawner)
					{
						// add it to the saves list
						savelist.Add((XmlSpawner)o);
					}
				}
			}

			// write out the spawners to a file
			XmlSpawner.SaveSpawnList(from, savelist, dirname, false, true);
		}

		private void ExecuteCommand(Mobile from, string command)
		{
			if (m_SearchList == null) return;

			ArrayList executelist = new ArrayList();

			for (int i = 0; i < m_SearchList.Count; i++)
			{
				SearchEntry e = (SearchEntry)m_SearchList[i];

				if (e.Selected)
				{
					object o = e.Object;

					// add it to the execute list
					executelist.Add(o);

				}
			}

			// lookup the command
			// and execute it
			if (command != null && command.Length > 0)
			{
				string[] args = command.Split(' ');

				if (args != null && args.Length > 1)
				{
					string[] cargs = new string[args.Length - 1];
					for (int i = 0; i < args.Length - 1; i++)
						cargs[i] = args[i + 1];

					CommandEventArgs e = new CommandEventArgs(from, args[0], command, cargs);

					foreach (BaseCommand c in TargetCommands.AllCommands)
					{
						// find the matching command
						if (c.Commands[0].ToLower() == args[0].ToLower())
						{
							bool flushToLog = false;

							// execute the command on the objects in the list

							if (executelist.Count > 20)
								CommandLogging.Enabled = false;

							c.ExecuteList(e, executelist);

							if (executelist.Count > 20)
							{
								flushToLog = true;
								CommandLogging.Enabled = true;
							}

							c.Flush(from, flushToLog);
							return;
						}
					}
					from.SendMessage("Invalid command: {0}", args[0]);
				}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			if (info == null || state == null || state.Mobile == null || m_SearchCriteria == null) return;

			int radiostate = -1;
			if (info.Switches.Length > 0)
			{
				radiostate = info.Switches[0];
			}

			// read the text entries for the search criteria
			TextRelay tr = info.GetTextEntry(105);        // range info
			m_SearchCriteria.Searchage = 0;
			if (tr != null && tr.Text != null && tr.Text.Length > 0)
			{
				try { m_SearchCriteria.Searchage = double.Parse(tr.Text); }
				catch { }
			}

			// read the text entries for the search criteria
			tr = info.GetTextEntry(100);        // range info
			m_SearchCriteria.Searchrange = -1;
			if (tr != null && tr.Text != null && tr.Text.Length > 0)
			{
				try { m_SearchCriteria.Searchrange = int.Parse(tr.Text); }
				catch { }
			}

			tr = info.GetTextEntry(101);        // type info
			if (tr != null)
				m_SearchCriteria.Searchtype = tr.Text;

			tr = info.GetTextEntry(102);        // name info
			if (tr != null)
				m_SearchCriteria.Searchname = tr.Text;

			tr = info.GetTextEntry(125);        // attachment type info
			if (tr != null)
				m_SearchCriteria.Searchattachtype = tr.Text;

			tr = info.GetTextEntry(103);        // entry info
			if (tr != null)
				m_SearchCriteria.Searchspawnentry = tr.Text;

			tr = info.GetTextEntry(104);        // condition info
			if (tr != null)
				m_SearchCriteria.Searchcondition = tr.Text;

			tr = info.GetTextEntry(106);        // region info
			if (tr != null)
				m_SearchCriteria.Searchregion = tr.Text;


			tr = info.GetTextEntry(400);        // displayfrom info
			try
			{
				DisplayFrom = int.Parse(tr.Text);
			}
			catch { }


			tr = info.GetTextEntry(300);        // savefilename info
			if (tr != null)
				SaveFilename = tr.Text;

			tr = info.GetTextEntry(301);        // commandstring info
			if (tr != null)
				CommandString = tr.Text;


			// check all of the check boxes
			m_SearchCriteria.Searchagedirection = info.IsSwitched(302);
			m_SearchCriteria.Dosearchage = info.IsSwitched(303);
			m_SearchCriteria.Dosearchrange = info.IsSwitched(304);
			m_SearchCriteria.Dosearchtype = info.IsSwitched(305);
			m_SearchCriteria.Dosearchname = info.IsSwitched(306);
			m_SearchCriteria.Dosearchspawnentry = info.IsSwitched(307);
			m_SearchCriteria.Dosearchspawntype = info.IsSwitched(326);
			m_SearchCriteria.Dosearcherr = info.IsSwitched(313);
			m_SearchCriteria.Dosearchcondition = info.IsSwitched(315);

			m_SearchCriteria.Dosearchint = info.IsSwitched(312);
			m_SearchCriteria.Dosearchfel = info.IsSwitched(308);
			m_SearchCriteria.Dosearchtram = info.IsSwitched(309);
			m_SearchCriteria.Dosearchmal = info.IsSwitched(310);
			m_SearchCriteria.Dosearchilsh = info.IsSwitched(311);
			m_SearchCriteria.Dosearchtok = info.IsSwitched(318);
            m_SearchCriteria.Dosearchter = info.IsSwitched(320);
			m_SearchCriteria.Dosearchnull = info.IsSwitched(314);

			m_SearchCriteria.Dohidevalidint = info.IsSwitched(316);
			m_SearchCriteria.Dosearchwithattach = info.IsSwitched(317);
			m_SearchCriteria.Dosearchattach = info.IsSwitched(325);
			m_SearchCriteria.Dosearchregion = info.IsSwitched(319);

			switch (info.ButtonID)
			{

				case 0: // Close
					{
						return;
					}
				case 3: // Search
					{
						// clear any selection
						Selected = -1;

						// reset displayfrom
						DisplayFrom = 0;

						// do the search
						m_SearchCriteria.Currentloc = state.Mobile.Location;
						m_SearchCriteria.Currentmap = state.Mobile.Map;

						//m_SearchList = Search(m_SearchCriteria, out status_str);
						XmlFindThread tobj = new XmlFindThread(state.Mobile, m_SearchCriteria, CommandString);
						Thread find = new Thread(new ThreadStart(tobj.XmlFindThreadMain));
						find.Name = "XmlFind Thread";
						find.Start();

						// turn on gump extension
						m_ShowExtension = true;
						return;
					}
				case 4: // SubSearch
					{
						// do the search
						string status_str;
						m_SearchList = Search(m_SearchCriteria, out status_str);
						break;
					}
				case 150: // Open the map gump
					{
						break;
					}
				case 154: // Bring all selected objects to the current location
					{
						Refresh(state);

						state.Mobile.SendGump(new XmlConfirmBringGump(state.Mobile, m_SearchList));
						return;
					}
				case 155: // Return the player to the starting loc
					{
						m_From.Location = StartingLoc;
						m_From.Map = StartingMap;
						break;
					}
				case 156: // Delete selected items
					{
						Refresh(state);

						state.Mobile.SendGump(new XmlConfirmDeleteGump(state.Mobile, m_SearchList));
						return;
					}
				case 157: // Reset selected items
					{
						ResetList();
						break;
					}
				case 158: // Respawn selected items
					{
						RespawnList();
						break;
					}
				case 159: // xmlsave selected spawners
					{
						SaveList(state.Mobile, SaveFilename);
						break;
					}
				case 160: // execute the command on the selected items
					{
						ExecuteCommand(state.Mobile, CommandString);
						break;
					}
				case 200: // gump extension
					{
						m_ShowExtension = !m_ShowExtension;
						break;
					}
				case 201: // forward block
					{
						if (m_SearchList != null && DisplayFrom + MaxEntries < m_SearchList.Count)
						{
							DisplayFrom += MaxEntries;
							// clear any selection
							Selected = -1;
						}
						break;
					}
				case 202: // backward block
					{

						DisplayFrom -= MaxEntries;
						if (DisplayFrom < 0) DisplayFrom = 0;
						// clear any selection
						Selected = -1;
						break;
					}

				case 700: // Sort
					{
						// clear any selection
						Selected = -1;

						Sorttype = false;
						Sortname = false;
						Sortrange = false;
						Sortmap = false;
						Sortselect = false;
						// read the toggle switches that determine the sort
						if (radiostate == 0) // sort by type
						{
							Sorttype = true;
						}
						else
							if (radiostate == 1) // sort by name
							{
								Sortname = true;
							}
							else
								if (radiostate == 2) // sort by range
								{
									Sortrange = true;
								}
								else
									if (radiostate == 4) // sort by entry
									{
										Sortmap = true;
									}
									else
										if (radiostate == 5) // sort by selected
										{
											Sortselect = true;
										}

						SortFindList();
						break;
					}
				case 701: // descending sort
					{
						Descendingsort = !Descendingsort;
						break;
					}
				case 9998:  // refresh the gump
					{
						// clear any selection
						Selected = -1;
						break;
					}
				default:
					{

						if (info.ButtonID >= 1000 && info.ButtonID < 1000 + MaxEntries)
						{
							// flag the entry selected
							Selected = info.ButtonID - 1000;
							// then go to it
							DoGoTo(info.ButtonID - 1000 + DisplayFrom);
						}
						if (info.ButtonID >= 2000 && info.ButtonID < 2000 + MaxEntries)
						{
							// flag the entry selected
							Selected = info.ButtonID - 2000;
							// then open the gump
							Refresh(state);
							DoShowGump(info.ButtonID - 2000 + DisplayFrom);
							return;
						}
						if (info.ButtonID >= 3000 && info.ButtonID < 3000 + MaxEntries)
						{
							Selected = info.ButtonID - 3000;
							// Show the props window
							Refresh(state);
							DoShowProps(info.ButtonID - 3000 + DisplayFrom);
							return;
						}
						else
							if (info.ButtonID == 3998)
							{
								SelectAll = !SelectAll;

								if (m_SearchList != null)
								{
									foreach (SearchEntry e in m_SearchList)
									{
										e.Selected = SelectAll;
									}
								}
							}
						if (info.ButtonID == 3999)
						{

							// toggle selection of everything currently displayed
							if (m_SearchList != null)
							{
								for (int i = 0; i < MaxEntries; i++)
								{
									if (i + DisplayFrom < m_SearchList.Count)
									{
										SearchEntry e = (SearchEntry)m_SearchList[i + DisplayFrom];

										e.Selected = !e.Selected;
									}
									else
									{
										break;
									}
								}
							}
						}
						if (info.ButtonID >= 4000 && info.ButtonID < 4000 + MaxEntries)
						{
							int i = info.ButtonID - 4000;

							if (m_SearchList != null && i >= 0 && m_SearchList.Count > i + DisplayFrom)
							{
								SearchEntry e = (SearchEntry)m_SearchList[i + DisplayFrom];

								e.Selected = !e.Selected;
							}
						}
						if (info.ButtonID >= 5000 && info.ButtonID < 5000 + MaxEntries)
						{
							int i = info.ButtonID - 5000;

							if (m_SearchList != null && i >= 0 && m_SearchList.Count > i + DisplayFrom)
							{
								SearchEntry e = (SearchEntry)m_SearchList[i + DisplayFrom];

								state.Mobile.CloseGump(typeof(XmlGetAttGump));
								state.Mobile.SendGump(new XmlGetAttGump(state.Mobile, e.Object, 10, 10));
							}
						}
						break;
					}
			}
			// Create a new gump
			//m_Spawner.OnDoubleClick( state.Mobile);
			Refresh(state);
		}

		public class XmlConfirmBringGump : Gump
		{
			private ArrayList SearchList;

			private Mobile From;

			public XmlConfirmBringGump(Mobile from, ArrayList searchlist)
				: base(0, 0)
			{
				SearchList = searchlist;

				From = from;
				Closable = false;
				Dragable = true;
				AddPage(0);
				AddBackground(10, 200, 200, 130, 5054);
				int count = 0;

				if (SearchList != null)
				{
					for (int i = 0; i < SearchList.Count; i++)
					{
						if (((SearchEntry)SearchList[i]).Selected) count++;
					}
				}

				AddLabel(20, 225, 33, String.Format("Bring {0} objects to you?", count));
				AddRadio(35, 255, 9721, 9724, false, 1); // accept/yes radio
				AddRadio(135, 255, 9721, 9724, true, 2); // decline/no radio
				AddHtmlLocalized(72, 255, 200, 30, 1049016, 0x7fff, false, false); // Yes
				AddHtmlLocalized(172, 255, 200, 30, 1049017, 0x7fff, false, false); // No
				AddButton(80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0); // Okay button

			}
			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (info == null || state == null || state.Mobile == null) return;

				int radiostate = -1;

				Point3D myloc = state.Mobile.Location;
				Map mymap = state.Mobile.Map;

				if (info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}
				switch (info.ButtonID)
				{

					default:
						{
							if (radiostate == 1 && SearchList != null)
							{    // accept
								for (int i = 0; i < SearchList.Count; i++)
								{
									SearchEntry e = (SearchEntry)SearchList[i];

									if (e.Selected)
									{
										object o = e.Object;

										if (o is Item)
										{

											((Item)o).MoveToWorld(myloc, mymap);

										}
										else
											if (o is Mobile)
											{

												((Mobile)o).MoveToWorld(myloc, mymap);

											}
									}
								}
							}
							break;
						}
				}
			}
		}

		public class XmlConfirmDeleteGump : Gump
		{
			private ArrayList SearchList;

			private Mobile From;

			public XmlConfirmDeleteGump(Mobile from, ArrayList searchlist)
				: base(0, 0)
			{
				SearchList = searchlist;

				From = from;
				Closable = false;
				Dragable = true;
				AddPage(0);
				AddBackground(10, 200, 200, 130, 5054);
				int count = 0;

				if (SearchList != null)
				{
					for (int i = 0; i < SearchList.Count; i++)
					{
						if (((SearchEntry)SearchList[i]).Selected) count++;
					}
				}

				AddLabel(20, 225, 33, String.Format("Delete {0} objects?", count));
				AddRadio(35, 255, 9721, 9724, false, 1); // accept/yes radio
				AddRadio(135, 255, 9721, 9724, true, 2); // decline/no radio
				AddHtmlLocalized(72, 255, 200, 30, 1049016, 0x7fff, false, false); // Yes
				AddHtmlLocalized(172, 255, 200, 30, 1049017, 0x7fff, false, false); // No
				AddButton(80, 289, 2130, 2129, 3, GumpButtonType.Reply, 0); // Okay button

			}
			public override void OnResponse(NetState state, RelayInfo info)
			{
				if (info == null || state == null || state.Mobile == null) return;

				int radiostate = -1;
				if (info.Switches.Length > 0)
				{
					radiostate = info.Switches[0];
				}
				switch (info.ButtonID)
				{

					default:
						{
							if (radiostate == 1 && SearchList != null)
							{    // accept
								for (int i = 0; i < SearchList.Count; i++)
								{
									SearchEntry e = (SearchEntry)SearchList[i];

									if (e.Selected)
									{
										object o = e.Object;

										if (o is Item)
										{
											// some objects may not delete gracefully (null map items are particularly error prone) so trap them
											try
											{
												((Item)o).Delete();
											}
											catch { }
										}
										else
											// block player deletion
											if ((o is Mobile) && !(((Mobile)o).Player))
											{
												try
												{
													((Mobile)o).Delete();
												}
												catch { }
											}
											else
												if (o is XmlAttachment)
												{
													try
													{
														((XmlAttachment)o).Delete();
													}
													catch { }
												}
									}

								}
							}
							break;
						}
				}
			}
		}
	}
}
