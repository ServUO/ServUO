#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using Server.ContextMenus;
using Server.Items;
using Server.Targeting;
#endregion

namespace Server
{
	public enum MusicName
	{
		Invalid = -1,
		OldUlt01 = 0,
		Create1,
		DragFlit,
		OldUlt02,
		OldUlt03,
		OldUlt04,
		OldUlt05,
		OldUlt06,
		Stones2,
		Britain1,
		Britain2,
		Bucsden,
		Jhelom,
		LBCastle,
		Linelle,
		Magincia,
		Minoc,
		Ocllo,
		Samlethe,
		Serpents,
		Skarabra,
		Trinsic,
		Vesper,
		Wind,
		Yew,
		Cave01,
		Dungeon9,
		Forest_a,
		InTown01,
		Jungle_a,
		Mountn_a,
		Plains_a,
		Sailing,
		Swamp_a,
		Tavern01,
		Tavern02,
		Tavern03,
		Tavern04,
		Combat1,
		Combat2,
		Combat3,
		Approach,
		Death,
		Victory,
		BTCastle,
		Nujelm,
		Dungeon2,
		Cove,
		Moonglow,
		Zento,
		TokunoDungeon,
		Taiko,
		DreadHornArea,
		ElfCity,
		GrizzleDungeon,
		MelisandesLair,
		ParoxysmusLair,
		GwennoConversation,
		GoodEndGame,
		GoodVsEvil,
		GreatEarthSerpents,
		Humanoids_U9,
		MinocNegative,
		Paws,
		SelimsBar,
		SerpentIsleCombat_U7,
		ValoriaShips,
		TheWanderer,
		Castle,
		Festival,
		Honor,
		Medieval,
		BattleOnStones,
		Docktown,
		GargoyleQueen,
		GenericCombat,
		Holycity,
		HumanLevel,
		LoginLoop,
		NorthernForestBattleonStones,
		PrimevalLich,
		QueenPalace,
		RoyalCity,
		SlasherVeil,
		StygianAbyss,
		StygianDragon,
		Void,
		CodexShrine,
		AnvilStrikeInMinoc,
		ASkaranLullaby,
		BlackthornsMarch,
		DupresNightInTrinsic,
		FayaxionAndTheSix,
		FlightOfTheNexus,
		GalehavenJaunt,
		JhelomToArms,
		MidnightInYew,
		MoonglowSonata,
		NewMaginciaMarch,
		NujelmWaltz,
		SherrysSong,
		StarlightInBritain,
		TheVesperMist,
		Pubtune,
		Overlordv2
	}

	[PropertyObject]
	public class Region : IComparable, IComparable<Region>
	{
		public static List<Region> Regions { get; } = new List<Region>(0x400);

		public static Region Find(Point3D p, Map map)
		{
			if (map == null)
			{
				return Map.Internal.DefaultRegion;
			}

			var sector = map.GetSector(p);

			if (sector == null)
			{
				return map.DefaultRegion;
			}

			var list = sector.RegionRects;

			var r = list.FirstOrDefault(regRect => regRect.Contains(p));

			return r?.Region ?? map.DefaultRegion;
		}

		public static IEnumerable<Region> FindRegions(Point3D p, Map map)
		{
			if (map == null)
			{
				yield return Map.Internal.DefaultRegion;
				yield break;
			}

			var sector = map.GetSector(p);

			if (sector == null)
			{
				yield return map.DefaultRegion;
				yield break;
			}

			var found = false;
			var list = sector.RegionRects;

			foreach (var regRect in list)
			{
				if (regRect.Contains(p))
				{
					if (!found)
					{
						found = true;
					}

					yield return regRect.Region;
				}
			}

			if (!found)
			{
				yield return map.DefaultRegion;
			}
		}

		public static event Action OnLoaded;

		public static event Action<Region> OnActivate;
		public static event Action<Region> OnDeactivate;

		public static event Action<Region, Mobile, Region> OnTransition;

		public static Type DefaultRegionType { get; set; } = typeof(Region);

		public static TimeSpan StaffLogoutDelay { get; set; } = TimeSpan.Zero;
		public static TimeSpan DefaultLogoutDelay { get; set; } = TimeSpan.FromMinutes(5.0);

		public static readonly int DefaultPriority = 50;

		public static readonly int MinZ = SByte.MinValue;
		public static readonly int MaxZ = SByte.MaxValue + 1;

		public static Rectangle3D ConvertTo3D(Rectangle2D rect)
		{
			return new Rectangle3D(new Point3D(rect.Start, MinZ), new Point3D(rect.End, MaxZ));
		}

		public static Rectangle3D[] ConvertTo3D(Rectangle2D[] rects)
		{
			var ret = new Rectangle3D[rects.Length];

			for (var i = 0; i < ret.Length; i++)
			{
				ret[i] = ConvertTo3D(rects[i]);
			}

			return ret;
		}

		public static IEnumerable<Rectangle3D> ConvertTo3D(IEnumerable<Rectangle2D> rects)
		{
			foreach (var r in rects)
			{
				yield return ConvertTo3D(r);
			}
		}

		public static bool CanSwitch(IEntity e, Point3D destLoc)
		{
			return CanSwitch(e, destLoc, e.Map);
		}

		public static bool CanSwitch(IEntity e, Point3D destLoc, Map destMap)
		{
			if (e == null || e.Deleted)
			{
				return false;
			}

			if (e.Location == destLoc && e.Map == destMap)
			{
				return true;
			}

			var source = Find(e.Location, e.Map);

			if (source == null || (source.Map == destMap && source.Contains(destLoc)))
			{
				return true;
			}

			var dest = Find(destLoc, destMap);

			return CanSwitch(e, source, dest);
		}

		public static bool CanSwitch(IEntity e, Region source, Region dest)
		{
			return e != null && !e.Deleted && (source == dest || ((source == null || source.CanExit(e)) && (dest == null || dest.CanEnter(e))));
		}

		private readonly string m_Name;
		private readonly int m_Priority;

		private Point3D m_GoLocation;

		[CommandProperty(AccessLevel.Counselor, true)]
		public string Name => m_Name;

		[CommandProperty(AccessLevel.Counselor, true)]
		public Map Map { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public Region Parent { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public List<Region> Children { get; } = new List<Region>();

		public Rectangle3D[] Area { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public Rectangle3D Bounds { get; private set; }

		public Sector[] Sectors { get; private set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool Dynamic { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public int Priority => m_Priority;

		[CommandProperty(AccessLevel.Counselor, true)]
		public int ChildLevel { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool Registered { get; private set; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Point3D GoLocation { get => m_GoLocation; set => m_GoLocation = value; }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public MusicName Music { get; set; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool IsDefault => Map.DefaultRegion == this;

		[CommandProperty(AccessLevel.Counselor, true)]
		public virtual MusicName DefaultMusic => Parent != null ? Parent.Music : MusicName.Invalid;

		[CommandProperty(AccessLevel.Counselor, true)]
		public virtual double InsuranceMultiplier => 1.0;

		private IEnumerable<Item> InternalItems
		{
			get
			{
				for (var i = 0; i < Area.Length; i++)
				{
					foreach (var o in Map.GetItemsInBounds(Area[i]))
						yield return o;
				}
			}
		}

		public IEnumerable<Item> AllItems => InternalItems.Distinct();

		[CommandProperty(AccessLevel.Counselor, true)]
		public int ItemCount => AllItems.Count();

		private IEnumerable<Mobile> InternalPlayers
		{
			get
			{
				for (var i = 0; i < Area.Length; i++)
				{
					foreach (var ns in Map.GetClientsInBounds(Area[i]))
						yield return ns.Mobile;
				}
			}
		}

		public IEnumerable<Mobile> AllPlayers => InternalPlayers.Distinct();

		[CommandProperty(AccessLevel.Counselor, true)]
		public int PlayerCount => AllPlayers.Count();

		private IEnumerable<Mobile> InternalMobiles
		{
			get
			{
				for (var i = 0; i < Area.Length; i++)
				{
					foreach (var m in Map.GetMobilesInBounds(Area[i]))
						yield return m;
				}
			}
		}

		public IEnumerable<Mobile> AllMobiles => InternalMobiles.Distinct();

		[CommandProperty(AccessLevel.Counselor, true)]
		public int MobileCount => AllMobiles.Count();

		private IEnumerable<BaseMulti> InternalMultis
		{
			get
			{
				for (var i = 0; i < Area.Length; i++)
				{
					foreach (var m in Map.GetMultisInBounds(Area[i]))
						yield return m;
				}
			}
		}

		public IEnumerable<BaseMulti> AllMultis => InternalMultis.Distinct();

		[CommandProperty(AccessLevel.Counselor, true)]
		public int MultiCount => AllMultis.Count();

		public Region(string name, Map map, int priority, params Rectangle2D[] area)
			: this(name, map, priority, ConvertTo3D(area))
		{ }

		public Region(string name, Map map, int priority, params Rectangle3D[] area)
			: this(name, map, null, area)
		{
			m_Priority = priority;
		}

		public Region(string name, Map map, Region parent, params Rectangle2D[] area)
			: this(name, map, parent, ConvertTo3D(area))
		{ }

		public Region(string name, Map map, Region parent, params Rectangle3D[] area)
		{
			m_Name = name;
			Map = map;
			Parent = parent;
			Area = area;
			Dynamic = true;

			Music = DefaultMusic;

			if (Parent == null)
			{
				ChildLevel = 0;
				m_Priority = DefaultPriority;
			}
			else
			{
				ChildLevel = Parent.ChildLevel + 1;
				m_Priority = Parent.Priority;
			}
		}

		public void Register()
		{
			if (Registered)
			{
				return;
			}

			OnRegister();

			Registered = true;

			if (Parent != null)
			{
				Parent.Children.Add(this);
				Parent.OnChildAdded(this);
			}

			Regions.Add(this);

			Map.RegisterRegion(this);

			var area = Area;

			if (area.Length > 0)
			{
				var rect = area[0];

				var p1 = rect.Start;
				var p2 = rect.End;

				var sectors = new HashSet<Sector>(area.Length * 8 * 8);

				for (var i = 0; i < area.Length; i++)
				{
					rect = Area[i];

					var start = Map.Bound(rect.Start);
					var end = Map.Bound(rect.End);

					Utility.FixPoints(ref start, ref end);

					if (i > 0)
					{
						p1.m_X = Math.Min(start.m_X, p1.m_X);
						p2.m_X = Math.Max(end.m_X, p2.m_X);

						p1.m_Y = Math.Min(start.m_Y, p1.m_Y);
						p2.m_Y = Math.Max(end.m_Y, p2.m_Y);

						p1.m_Z = Math.Min(rect.Start.m_Z, p1.m_Z);
						p2.m_Z = Math.Max(rect.End.m_Z, p2.m_Z);
					}

					var startSector = Map.GetSector(start);
					var endSector = Map.GetSector(end);

					for (var x = startSector.X; x <= endSector.X; x++)
					{
						for (var y = startSector.Y; y <= endSector.Y; y++)
						{
							var sector = Map.GetRealSector(x, y);

							if (sectors.Add(sector))
							{
								sector.OnEnter(this, rect);
							}
						}
					}
				}

				Bounds = new Rectangle3D(p1, p2);

				Sectors = sectors.ToArray();

				sectors.Clear();
				sectors.TrimExcess();
			}
			else
				Sectors = Array.Empty<Sector>();

			OnActivate?.Invoke(this);
		}

		public void Unregister()
		{
			if (!Registered)
			{
				return;
			}

			OnUnregister();

			Registered = false;

			OnDeactivate?.Invoke(this);

			if (Children.Count > 0)
			{
				Console.WriteLine("Warning: Unregistering region '{0}' with children", this);
			}

			if (Parent != null)
			{
				Parent.Children.Remove(this);
				Parent.Children.TrimExcess();
				Parent.OnChildRemoved(this);
			}

			Regions.Remove(this);
			Regions.TrimExcess();

			Map.UnregisterRegion(this);

			if (Sectors == null)
			{
				return;
			}

			foreach (var s in Sectors)
			{
				s.OnLeave(this);
			}

			Sectors = null;
		}

		public void PlayMusic(Mobile m)
		{
			var music = Music;

			if (OnPlayMusic(m, ref music))
			{
				Network.PlayMusic.Send(m.NetState, music);
			}
		}

		protected virtual bool OnPlayMusic(Mobile m, ref MusicName music)
		{
			if (Parent != null)
			{
				return Parent.OnPlayMusic(m, ref music);
			}

			return true;
		}

		public bool Contains(Point3D p)
		{
			for (var i = 0; i < Area.Length; i++)
			{
				if (Area[i].Contains(p))
					return true;
			}

			return false;
		}

		public bool IsChildOf(Region region)
		{
			if (region == null)
			{
				return false;
			}

			var p = Parent;

			while (p != null)
			{
				if (p == region)
				{
					return true;
				}

				p = p.Parent;
			}

			return false;
		}

		public Region GetRegion(Type regionType)
		{
			if (regionType == null)
			{
				return null;
			}

			var r = this;

			do
			{
				if (regionType.IsAssignableFrom(r.GetType()))
				{
					return r;
				}

				r = r.Parent;
			}
			while (r != null);

			return null;
		}

		public Region GetRegion(string regionName)
		{
			if (regionName == null)
			{
				return null;
			}

			var r = this;

			do
			{
				if (r.Name == regionName)
				{
					return r;
				}

				r = r.Parent;
			}
			while (r != null);

			return null;
		}

		public bool IsPartOf(Region region)
		{
			return this == region || IsChildOf(region);
		}

		public bool IsPartOf(Type regionType)
		{
			return GetRegion(regionType) != null;
		}

		public bool IsPartOf<T>() where T : Region
		{
			return IsPartOf(typeof(T));
		}

		public bool IsPartOf(string regionName)
		{
			return GetRegion(regionName) != null;
		}

		public virtual bool AcceptsSpawnsFrom(Region region)
		{
			if (!AllowSpawn())
			{
				return false;
			}

			if (region == this)
			{
				return true;
			}

			if (Parent != null)
			{
				return Parent.AcceptsSpawnsFrom(region);
			}

			return false;
		}

		public virtual int CompareTo(object obj)
		{
			if (obj is Region reg)
			{
				return CompareTo(reg);
			}

			return 1;
		}

		public virtual int CompareTo(Region reg)
		{
			if (reg == null)
			{
				return 1;
			}

			if (Dynamic)
			{
				if (!reg.Dynamic)
				{
					return -1;
				}
			}
			else if (reg.Dynamic)
			{
				return 1;
			}

			return ComparePriority(reg);
		}

		public int ComparePriority(Region reg)
		{
			var thisPriority = Priority;
			var regPriority = reg.Priority;

			if (thisPriority != regPriority)
			{
				return regPriority - thisPriority;
			}

			return reg.ChildLevel - ChildLevel;
		}

		public override string ToString()
		{
			return m_Name ?? GetType().Name;
		}

		public virtual void OnRegister()
		{ }

		public virtual void OnUnregister()
		{ }

		public virtual void OnChildAdded(Region child)
		{ }

		public virtual void OnChildRemoved(Region child)
		{ }

		public virtual bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
		{
			if (Contains(oldLocation))
			{
				return m.WalkRegion == null || (m.Spawner != null && AcceptsSpawnsFrom(m.WalkRegion));
			}

			var oldRegion = Find(oldLocation, m.Map);

			if (oldRegion != null && oldRegion != this && (!oldRegion.CanExit(m) || !CanEnter(m)))
			{
				return false;
			}

			return m.WalkRegion == null || (m.Spawner != null && AcceptsSpawnsFrom(m.WalkRegion));
		}

		public virtual bool CanEnter(IEntity e)
		{
			return true;
		}

		public virtual bool CanExit(IEntity e)
		{
			return true;
		}

		public virtual void OnEnter(Mobile m)
		{ }

		public virtual void OnExit(Mobile m)
		{ }

		public virtual Mobile MakeGuard(Mobile focus)
		{
			return Parent?.MakeGuard(focus);
		}

		public virtual Type GetResource(Type type)
		{
			return Parent?.GetResource(type) ?? type;
		}

		public virtual bool CanUseStuckMenu(Mobile m)
		{
			return Parent?.CanUseStuckMenu(m) != false;
		}

		public virtual void OnAggressed(Mobile aggressor, Mobile aggressed, bool criminal)
		{
			Parent?.OnAggressed(aggressor, aggressed, criminal);
		}

		public virtual void OnDidHarmful(Mobile harmer, IDamageable harmed)
		{
			Parent?.OnDidHarmful(harmer, harmed);
		}

		public virtual void OnGotHarmful(Mobile harmer, IDamageable harmed)
		{
			Parent?.OnGotHarmful(harmer, harmed);
		}

		public virtual void OnLocationChanged(Mobile m, Point3D oldLocation)
		{
			Parent?.OnLocationChanged(m, oldLocation);
		}

		public virtual bool OnTarget(Mobile m, Target t, object o)
		{
			return Parent?.OnTarget(m, t, o) != false;
		}

		public virtual bool OnCombatantChange(Mobile m, IDamageable Old, IDamageable New)
		{
			return Parent?.OnCombatantChange(m, Old, New) != false;
		}

		public virtual bool AllowAutoClaim(Mobile m)
		{
			return Parent?.AllowAutoClaim(m) != false;
		}

		public virtual bool AllowFlying(Mobile m)
		{
			return Parent?.AllowFlying(m) != false;
		}

		public virtual bool AllowHousing(Mobile m, Point3D p)
		{
			return Parent?.AllowHousing(m, p) != false;
		}

		public virtual bool SendInaccessibleMessage(Item item, Mobile m)
		{
			return Parent?.SendInaccessibleMessage(item, m) == true;
		}

		public virtual bool CheckAccessibility(Item item, Mobile user)
		{
			var args = new CheckAccessItemEventArgs(user, item, false);

			EventSink.InvokeCheckAccessItem(args);

			return !args.Block && Parent?.CheckAccessibility(item, user) != false;
		}

		public virtual bool CheckAccessibility(Mobile mobile, Mobile user)
		{
			return Parent?.CheckAccessibility(mobile, user) != false;
		}

		public virtual bool OnDecay(Item item)
		{
			return Parent?.OnDecay(item) != false;
		}

		public virtual bool AllowHarmful(Mobile m, IDamageable target)
		{
			return (Parent?.AllowHarmful(m, target) ?? Mobile.AllowHarmfulHandler?.Invoke(m, target)) != false;

		}

		public virtual void OnCriminalAction(Mobile m, bool message)
		{
			if (Parent != null)
			{
				Parent.OnCriminalAction(m, message);
			}
			else if (message)
			{
				m.SendLocalizedMessage(1005040); // You've committed a criminal act!!
			}
		}

		public virtual bool AllowBeneficial(Mobile m, Mobile target)
		{
			return (Parent?.AllowBeneficial(m, target) ?? Mobile.AllowBeneficialHandler?.Invoke(m, target)) != false;
		}

		public virtual void OnBeneficialAction(Mobile helper, Mobile target)
		{
			Parent?.OnBeneficialAction(helper, target);
		}

		public virtual void OnGotBeneficialAction(Mobile helper, Mobile target)
		{
			Parent?.OnGotBeneficialAction(helper, target);
		}

		public virtual void SpellDamageScalar(Mobile caster, Mobile target, ref double damage)
		{
			Parent?.SpellDamageScalar(caster, target, ref damage);
		}

		public virtual void OnSpeech(SpeechEventArgs args)
		{
			Parent?.OnSpeech(args);
		}

		public virtual bool OnSkillUse(Mobile m, int skill)
		{
			return Parent?.OnSkillUse(m, skill) != false;
		}

		public virtual bool OnSkillGain(Mobile m, ref int toGain)
		{
			return Parent?.OnSkillGain(m, ref toGain) != false;
		}

		public virtual bool OnBeginSpellCast(Mobile m, ISpell s)
		{
			return Parent?.OnBeginSpellCast(m, s) != false;
		}

		public virtual void OnSpellCast(Mobile m, ISpell s)
		{
			Parent?.OnSpellCast(m, s);
		}

		public virtual bool OnResurrect(Mobile m)
		{
			return Parent?.OnResurrect(m) != false;
		}

		public virtual bool OnBeforeDeath(Mobile m)
		{
			return Parent?.OnBeforeDeath(m) != false;
		}

		public virtual void OnDeath(Mobile m)
		{
			Parent?.OnDeath(m);
		}

		public virtual bool OnDamage(Mobile m, ref int damage)
		{
			return Parent?.OnDamage(m, ref damage) != false;
		}

		public virtual bool OnHeal(Mobile m, ref int heal)
		{
			return Parent?.OnHeal(m, ref heal) != false;
		}

		public virtual bool OnDoubleClick(Mobile m, object o)
		{
			return Parent?.OnDoubleClick(m, o) != false;
		}

		public virtual bool OnSingleClick(Mobile m, object o)
		{
			return Parent?.OnSingleClick(m, o) != false;
		}

		public virtual void OnDelete(Item item)
		{
			Parent?.OnDelete(item);
		}

		public virtual void GetContextMenuEntries(Mobile m, List<ContextMenuEntry> list, Item item)
		{
			Parent?.GetContextMenuEntries(m, list, item);
		}

		public virtual bool AllowSpawn()
		{
			return Parent?.AllowSpawn() != false;
		}

		public virtual void AlterLightLevel(Mobile m, ref int global, ref int personal)
		{
			Parent?.AlterLightLevel(m, ref global, ref personal);
		}

		public virtual TimeSpan GetLogoutDelay(Mobile m)
		{
			return Parent?.GetLogoutDelay(m) ?? (m.IsStaff() ? StaffLogoutDelay : DefaultLogoutDelay);
		}

		static internal bool CanMove(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation, Map map)
		{
			var oldRegion = m.Region;
			var newRegion = Find(newLocation, map);

			while (oldRegion != newRegion)
			{
				if (!newRegion.OnMoveInto(m, d, newLocation, oldLocation))
				{
					return false;
				}

				if (newRegion.Parent == null)
				{
					return true;
				}

				newRegion = newRegion.Parent;
			}

			return true;
		}

		static internal void OnRegionChange(Mobile m, Region oldRegion, Region newRegion)
		{
			var oldR = oldRegion;
			var newR = newRegion;

			while (oldR != newR)
			{
				var oldRChild = oldR?.ChildLevel ?? -1;
				var newRChild = newR?.ChildLevel ?? -1;

				if (oldR != null && oldRChild >= newRChild)
				{
					oldR.OnExit(m);

					EventSink.InvokeOnExitRegion(new OnExitRegionEventArgs(m, oldR, newR));

					oldR = oldR.Parent;
				}

				if (newR != null && newRChild >= oldRChild)
				{
					newR.OnEnter(m);

					EventSink.InvokeOnEnterRegion(new OnEnterRegionEventArgs(m, oldR, newR));

					newR = newR.Parent;
				}
			}

			if (newRegion != null && m.NetState != null)
			{
				m.CheckLightLevels(false);

				if (oldRegion == null || oldRegion.Music != newRegion.Music)
				{
					newRegion.PlayMusic(m);
				}
			}

			OnTransition?.Invoke(oldRegion, m, newRegion);
		}

		public static readonly string XmlDirectory = Path.Combine(Core.BaseDirectory, "Data", "Regions");

		static internal void Load()
		{
			if (!Directory.Exists(XmlDirectory))
			{
				Utility.WriteLine(ConsoleColor.Red, $"Regions: Directory not found:\n > {XmlDirectory}");
				return;
			}

			Utility.WriteLine(ConsoleColor.Yellow, "Regions: Loading...");

			var files = new HashSet<string>();

			var legacyPath = Path.Combine(Core.BaseDirectory, "Data", "Regions.xml");

			if (File.Exists(legacyPath))
			{
				files.Add(legacyPath);
			}

			files.UnionWith(Directory.EnumerateFiles(XmlDirectory, "*.xml", SearchOption.AllDirectories));

			var doc = new XmlDocument();

			foreach (var file in files)
			{
				doc.Load(file);

				var root = doc["ServerRegions"];

				if (root == null)
				{
					Utility.WriteLine(ConsoleColor.Red, $"Regions: 'ServerRegions' element not found:\n > {file}");
					continue;
				}

				var nodes = root.SelectNodes("Facet");

				if (nodes != null)
				{
					foreach (XmlElement facet in nodes)
					{
						Map map = null;

						if (ReadMap(facet, "name", ref map) && map != null && map != Map.Internal)
						{
							LoadRegions(facet, map, null);
						}
					}
				}
			}

			Utility.WriteLine(ConsoleColor.Green, "Regions: Loaded");

			OnLoaded?.Invoke();
		}

		private static void LoadRegions(XmlElement xml, Map map, Region parent)
		{
			var nodes = xml.SelectNodes("region");

			if (nodes == null)
			{
				return;
			}

			foreach (XmlElement xmlReg in nodes)
			{
				var expansion = Expansion.None;

				if (ReadEnum(xmlReg, "expansion", ref expansion, false) && expansion > Core.Expansion)
				{
					continue;
				}

				var type = DefaultRegionType;

				ReadType(xmlReg, "type", ref type, false);

				if (!typeof(Region).IsAssignableFrom(type))
				{
					Utility.WriteLine(ConsoleColor.Red, $"Invalid region type '{type}' in regions.xml");
					continue;
				}

				Region region;

				try
				{
					region = (Region)Activator.CreateInstance(type, xmlReg, map, parent);
				}
				catch (Exception ex)
				{
					Utility.WriteLine(ConsoleColor.Red, $"Error during the creation of region type '{type}':\n{ex}");
					continue;
				}

				region.Register();

				LoadRegions(xmlReg, map, region);
			}
		}

		public Region(XmlElement xml, Map map, Region parent)
		{
			Map = map;
			Parent = parent;
			Dynamic = false;

			if (Parent == null)
			{
				ChildLevel = 0;
				m_Priority = DefaultPriority;
			}
			else
			{
				ChildLevel = Parent.ChildLevel + 1;
				m_Priority = Parent.Priority;
			}

			ReadString(xml, "name", ref m_Name, false);

			if (parent == null)
			{
				ReadInt32(xml, "priority", ref m_Priority, false);
			}

			var minZ = MinZ;
			var maxZ = MaxZ;

			var zrange = xml["zrange"];
			ReadInt32(zrange, "min", ref minZ, false);
			ReadInt32(zrange, "max", ref maxZ, false);

			var area = new List<Rectangle3D>();

			var nodes = xml.SelectNodes("rect");

			if (nodes != null)
			{
				foreach (XmlElement xmlRect in nodes)
				{
					var expansion = Expansion.None;

					if (ReadEnum(xmlRect, "expansion", ref expansion, false) && expansion > Core.Expansion)
					{
						continue;
					}

					var rect = new Rectangle3D();

					if (ReadRectangle3D(xmlRect, minZ, maxZ, ref rect))
					{
						area.Add(rect);
					}
				}
			}

			Area = area.ToArray();

			area.Clear();
			area.TrimExcess();

			if (Area.Length == 0)
			{
				Utility.WriteLine(ConsoleColor.Red, $"Empty area for region '{this}'");
			}

			if (!ReadPoint3D(xml["go"], map, ref m_GoLocation, false) && Area.Length > 0)
			{
				var start = Area[0].Start;
				var end = Area[0].End;

				var x = start.X + (end.X - start.X) / 2;
				var y = start.Y + (end.Y - start.Y) / 2;

				m_GoLocation = new Point3D(x, y, Map.GetAverageZ(x, y));
			}

			var music = DefaultMusic;

			ReadEnum(xml["music"], "name", ref music, false);

			Music = music;
		}

		protected static string GetAttribute(XmlElement xml, string attribute, bool mandatory)
		{
			if (xml == null)
			{
				if (mandatory)
				{
					Utility.WriteLine(ConsoleColor.Red, $"Missing element for attribute '{attribute}'");
				}

				return null;
			}

			if (xml.HasAttribute(attribute))
			{
				return xml.GetAttribute(attribute);
			}

			if (mandatory)
			{
				Utility.WriteLine(ConsoleColor.Red, $"Missing attribute '{attribute}' in element '{xml.Name}'");
			}

			return null;
		}

		public static bool ReadString(XmlElement xml, string attribute, ref string value)
		{
			return ReadString(xml, attribute, ref value, true);
		}

		public static bool ReadString(XmlElement xml, string attribute, ref string value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			value = s;
			return true;
		}

		public static bool ReadInt32(XmlElement xml, string attribute, ref int value)
		{
			return ReadInt32(xml, attribute, ref value, true);
		}

		public static bool ReadInt32(XmlElement xml, string attribute, ref int value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			try
			{
				value = XmlConvert.ToInt32(s);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse integer attribute '{attribute}' in element '{xml.Name}'");
				return false;
			}

			return true;
		}

		public static bool ReadBoolean(XmlElement xml, string attribute, ref bool value)
		{
			return ReadBoolean(xml, attribute, ref value, true);
		}

		public static bool ReadBoolean(XmlElement xml, string attribute, ref bool value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			try
			{
				value = XmlConvert.ToBoolean(s);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse boolean '{attribute}' in element '{xml.Name}'");
				return false;
			}

			return true;
		}

		public static bool ReadDateTime(XmlElement xml, string attribute, ref DateTime value)
		{
			return ReadDateTime(xml, attribute, ref value, true);
		}

		public static bool ReadDateTime(XmlElement xml, string attribute, ref DateTime value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			try
			{
				value = XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.Utc);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse date attribute '{attribute}' in element '{xml.Name}'");
				return false;
			}

			return true;
		}

		public static bool ReadTimeSpan(XmlElement xml, string attribute, ref TimeSpan value)
		{
			return ReadTimeSpan(xml, attribute, ref value, true);
		}

		public static bool ReadTimeSpan(XmlElement xml, string attribute, ref TimeSpan value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			try
			{
				value = XmlConvert.ToTimeSpan(s);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse time attribute '{attribute}' in element '{xml.Name}'");
				return false;
			}

			return true;
		}

#if MONO
		public static bool ReadEnum<T>(XmlElement xml, string attribute, ref T value) where T : struct, IConvertible
#else
		public static bool ReadEnum<T>(XmlElement xml, string attribute, ref T value) where T : struct, Enum
#endif
		{
			return ReadEnum(xml, attribute, ref value, true);
		}

#if MONO
		public static bool ReadEnum<T>(XmlElement xml, string attribute, ref T value, bool mandatory) where T : struct, IConvertible
#else
		public static bool ReadEnum<T>(XmlElement xml, string attribute, ref T value, bool mandatory) where T : struct, Enum
#endif
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			var type = typeof(T);

			if (type.IsEnum && Enum.TryParse(s, true, out T tempVal))
			{
				value = tempVal;
				return true;
			}

			Utility.WriteLine(ConsoleColor.Red, $"Could not parse {type} enum attribute '{attribute}' in element '{xml.Name}'");
			return false;
		}

		public static bool ReadMap(XmlElement xml, string attribute, ref Map value)
		{
			return ReadMap(xml, attribute, ref value, true);
		}

		public static bool ReadMap(XmlElement xml, string attribute, ref Map value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			try
			{
				value = Map.Parse(s);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse map '{attribute}' in element '{xml.Name}'");
				return false;
			}

			return true;
		}

		public static bool ReadType(XmlElement xml, string attribute, ref Type value)
		{
			return ReadType(xml, attribute, ref value, true);
		}

		public static bool ReadType(XmlElement xml, string attribute, ref Type value, bool mandatory)
		{
			var s = GetAttribute(xml, attribute, mandatory);

			if (s == null)
			{
				return false;
			}

			Type type;

			try
			{
				type = ScriptCompiler.FindTypeByName(s, false);
			}
			catch
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not parse type attribute '{attribute}' in element '{xml.Name}'");
				return false;
			}

			if (type == null)
			{
				Utility.WriteLine(ConsoleColor.Red, $"Could not find type '{s}'");
				return false;
			}

			value = type;
			return true;
		}

		public static bool ReadPoint3D(XmlElement xml, Map map, ref Point3D value)
		{
			return ReadPoint3D(xml, map, ref value, true);
		}

		public static bool ReadPoint3D(XmlElement xml, Map map, ref Point3D value, bool mandatory)
		{
			int x = 0, y = 0, z = 0;

			var xyOk = ReadInt32(xml, "x", ref x, mandatory) & ReadInt32(xml, "y", ref y, mandatory);
			var zOk = ReadInt32(xml, "z", ref z, mandatory && map == null);

			if (xyOk && (zOk || map != null))
			{
				if (!zOk)
				{
					z = map.GetAverageZ(x, y);
				}

				value = new Point3D(x, y, z);
				return true;
			}

			return false;
		}

		public static bool ReadRectangle3D(XmlElement xml, int defaultMinZ, int defaultMaxZ, ref Rectangle3D value)
		{
			return ReadRectangle3D(xml, defaultMinZ, defaultMaxZ, ref value, true);
		}

		public static bool ReadRectangle3D(XmlElement xml, int defaultMinZ, int defaultMaxZ, ref Rectangle3D value, bool mandatory)
		{
			int x1 = 0, y1 = 0, x2 = 0, y2 = 0;

			if (xml.HasAttribute("x"))
			{
				if (ReadInt32(xml, "x", ref x1, mandatory) & ReadInt32(xml, "y", ref y1, mandatory) &
					ReadInt32(xml, "width", ref x2, mandatory) & ReadInt32(xml, "height", ref y2, mandatory))
				{
					x2 += x1;
					y2 += y1;
				}
				else
				{
					return false;
				}
			}
			else
			{
				if (!ReadInt32(xml, "x1", ref x1, mandatory) | !ReadInt32(xml, "y1", ref y1, mandatory) |
					!ReadInt32(xml, "x2", ref x2, mandatory) | !ReadInt32(xml, "y2", ref y2, mandatory))
				{
					return false;
				}
			}

			var z1 = defaultMinZ;
			var z2 = defaultMaxZ;

			ReadInt32(xml, "zmin", ref z1, false);
			ReadInt32(xml, "zmax", ref z2, false);

			value = new Rectangle3D(new Point3D(x1, y1, z1), new Point3D(x2, y2, z2));

			return true;
		}
	}
}
