#region References
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Spells;
#endregion

namespace Server
{
	public static class Siege
	{
		public static readonly string FilePath = Path.Combine("Saves", "Siege.bin");

		public static bool SiegeShard { get => Core.IsSiege; set => Core.IsSiege = value; }

		[ConfigProperty("Siege.CharacterSlots")]
		public static int CharacterSlots { get => Config.Get("Siege.CharacterSlots", 1); set => Config.Set("Siege.CharacterSlots", value); }

		[ConfigProperty("Siege.StatsPerDay")]
		public static int StatsPerDay { get => Config.Get("Siege.StatsPerDay", 15); set => Config.Set("Siege.StatsPerDay", value); }

		public static Dictionary<PlayerMobile, Dictionary<SkillName, DateTime>> ROTTable { get; } = new Dictionary<PlayerMobile, Dictionary<SkillName, DateTime>>();
		public static Dictionary<PlayerMobile, int> StatsTable { get; } = new Dictionary<PlayerMobile, int>();

		public static DateTime LastReset { get; private set; }

		public static void Configure()
		{
			Core.OnSiegeStateChanged += Invalidate;

			EventSink.Login += OnLogin;

			EventSink.WorldSave += OnSave;
			EventSink.WorldLoad += OnLoad;

			EventSink.AfterWorldSave += OnAfterSave;

			EventSink.ContainerDroppedTo += OnDropped;

			CommandSystem.Register("ResetROT", AccessLevel.GameMaster, e =>
			{
				if (!SiegeShard)
				{
					return;
				}

				LastReset = DateTime.Now;

				e.Mobile.SendMessage("Rate over Time reset!");
			});

			CommandSystem.Register("GetROTInfo", AccessLevel.GameMaster, e =>
			{
				if (!SiegeShard)
				{
					return;
				}

				foreach (var kvp in ROTTable)
				{
					Console.WriteLine($"Player: {kvp.Key.Name}");

					var stats = 0;

					if (StatsTable.ContainsKey(kvp.Key))
					{
						stats = StatsTable[kvp.Key];
					}

					Console.WriteLine($"Stats gained today: {stats} of {StatsPerDay}");

					Utility.PushColor(ConsoleColor.Magenta);

					foreach (var kvp2 in kvp.Value)
					{
						var pergain = MinutesPerGain(kvp.Key, kvp.Key.Skills[kvp2.Key]);
						var last = kvp2.Value;
						var next = last.AddMinutes(pergain);

						var nextg = next < DateTime.Now ? "now" : "in " + ((int)(next - DateTime.Now).TotalMinutes).ToString() + " minutes";

						Console.WriteLine($"   {kvp2.Key}: last gained {last.ToShortTimeString()}, can gain {nextg} (every {pergain} minutes)");
					}

					Utility.PopColor();
				}

				Console.WriteLine("---");
				Console.WriteLine($"Next Reset: {(LastReset + TimeSpan.FromHours(24) - DateTime.Now).TotalMinutes} minutes");
			});
		}

		[CallPriority(Int32.MaxValue - 1)]
		public static void Initialize()
		{
			Invalidate();
		}

		public static void Invalidate()
		{
			if (!SiegeShard)
			{
				return;
			}

			Utility.WriteLine(ConsoleColor.Red, "Initializing Siege Perilous Shard...");

			var tick = Core.TickCount;

			var toReset = new List<XmlSpawner>();

			foreach (var item in World.Items.Values.OfType<XmlSpawner>().Where(sp => sp.Map == Map.Trammel && sp.Running))
			{
				toReset.Add(item);
			}

			foreach (var item in toReset)
			{
				item.DoReset = true;
			}

			ColUtility.Free(toReset);

			Utility.WriteLine(ConsoleColor.Red, $"Reset {toReset.Count} trammel spawners in {Core.TickCount - tick} milliseconds!");
		}

		private static void OnSave(WorldSaveEventArgs e)
		{
			Persistence.Serialize(FilePath, OnSerialize);
		}

		private static void OnLoad()
		{
			Persistence.Deserialize(FilePath, OnDeserialize);
		}

		private static void OnAfterSave(AfterWorldSaveEventArgs e)
		{
			CheckTime();
		}

		private static void OnSerialize(GenericWriter writer)
		{
			writer.Write(0);

			writer.Write(LastReset);

			writer.Write(ROTTable.Count);

			foreach (var kvp in ROTTable)
			{
				writer.Write(kvp.Key);
				writer.Write(kvp.Value.Count);

				foreach (var kvp2 in kvp.Value)
				{
					writer.Write((int)kvp2.Key);
					writer.Write(kvp2.Value);
				}
			}

			writer.Write(StatsTable.Count);

			foreach (var kvp in StatsTable)
			{
				writer.Write(kvp.Key);
				writer.Write(kvp.Value);
			}
		}

		private static void OnDeserialize(GenericReader reader)
		{
			reader.ReadInt();

			LastReset = reader.ReadDateTime();

			var count = reader.ReadInt();

			for (var i = 0; i < count; i++)
			{
				var pm = reader.ReadMobile<PlayerMobile>();
				var dict = new Dictionary<SkillName, DateTime>();

				var c = reader.ReadInt();

				for (var j = 0; j < c; j++)
				{
					var sk = (SkillName)reader.ReadInt();
					var next = reader.ReadDateTime();

					dict[sk] = next;
				}

				if (pm != null)
				{
					ROTTable[pm] = dict;
				}
			}

			count = reader.ReadInt();

			for (var i = 0; i < count; i++)
			{
				var pm = reader.ReadMobile<PlayerMobile>();
				var total = reader.ReadInt();

				if (pm != null)
				{
					StatsTable[pm] = total;
				}
			}

			CheckTime();
		}

		private static void OnLogin(LoginEventArgs e)
		{
			if (!SiegeShard)
			{
				return;
			}

			if (e.Mobile is PlayerMobile pm && pm.Map == Map.Trammel && pm.AccessLevel < AccessLevel.Counselor)
			{
				pm.MoveToWorld(new Point3D(989, 519, -50), Map.Malas);
				pm.SendMessage("You have been removed from Trammel.");
			}
		}

		private static void OnDropped(ContainerDroppedToEventArgs e)
		{
			if (!SiegeShard)
			{
				return;
			}

			var item = e.Dropped;
			var from = e.Mobile;
			var cont = e.Container;

			if (item != null && cont != from.Backpack && from is PlayerMobile pm && pm.BlessedItem != null && pm.BlessedItem == item)
			{
				pm.BlessedItem = null;

				item.LootType = LootType.Regular;

				pm.SendLocalizedMessage(1075292, item.Name ?? "#" + item.LabelNumber); // ~1_NAME~ has been unblessed.
			}
		}

		public static bool CheckTravel(Mobile m, Point3D p, Map map, TravelCheckType type)
		{
			if (!SiegeShard)
			{
				return true;
			}

			if (m.AccessLevel > AccessLevel.Player)
			{
				return true;
			}

			switch (type)
			{
				case TravelCheckType.RecallFrom:
				case TravelCheckType.RecallTo: return false;
				case TravelCheckType.GateFrom:
				case TravelCheckType.GateTo:
				case TravelCheckType.Mark: return CanTravelTo(m, p, map);
				case TravelCheckType.TeleportFrom:
				case TravelCheckType.TeleportTo: return true;
			}

			return true;
		}

		public static bool CanTravelTo(Mobile m, Point3D p, Map map)
		{
			return !(Region.Find(p, map) is DungeonRegion) && !SpellHelper.IsAnyT2A(map, p) && !SpellHelper.IsIlshenar(map, p);
		}

		public static void CheckTime()
		{
			if (!SiegeShard)
			{
				return;
			}

			var now = DateTime.Now;

			if (LastReset.AddHours(24) < now)
			{
				var reset = new DateTime(now.Year, now.Month, now.Day, 20, 0, 0);

				if (now < reset)
				{
					LastReset = reset - TimeSpan.FromHours(24);
				}
				else
				{
					ROTTable.Clear();
					StatsTable.Clear();

					LastReset = reset;
				}
			}
		}

		public static bool CheckSkillGain(PlayerMobile pm, int minutesPerSkill, Skill skill)
		{
			if (!SiegeShard || minutesPerSkill <= 0)
			{
				return true;
			}

			var sk = skill.SkillName;

			if (ROTTable.ContainsKey(pm))
			{
				if (ROTTable[pm].ContainsKey(sk))
				{
					var lastGain = ROTTable[pm][sk];

					if (lastGain + TimeSpan.FromMinutes(minutesPerSkill) < DateTime.Now)
					{
						ROTTable[pm][sk] = DateTime.Now;

						return true;
					}

					return false;
				}

				ROTTable[pm][sk] = DateTime.Now;

				return true;
			}

			ROTTable[pm] = new Dictionary<SkillName, DateTime>
			{
				[sk] = DateTime.Now
			};

			return true;
		}

		public static int MinutesPerGain(Mobile m, Skill skill)
		{
			if (!SiegeShard)
			{
				return -1;
			}

			var value = skill.Base;

			if (value < 70.0)
			{
				return 0;
			}

			if (value <= 79.9)
			{
				return 5;
			}

			if (value <= 89.9)
			{
				return 8;
			}

			if (value <= 99.9)
			{
				return 12;
			}

			return 15;
		}

		public static bool CanGainStat(PlayerMobile m)
		{
			if (!StatsTable.ContainsKey(m))
			{
				return true;
			}

			return StatsTable[m] < StatsPerDay;
		}

		public static void IncreaseStat(PlayerMobile m)
		{
			if (!StatsTable.ContainsKey(m))
			{
				StatsTable[m] = 1;
			}
			else
			{
				StatsTable[m]++;
			}
		}

		public static bool VendorCanSell(Type t)
		{
			if (t == null)
			{
				return false;
			}

			foreach (var type in _NoSellList)
			{
				if (t == type || t.IsSubclassOf(type))
				{
					return false;
				}
			}

			return true;
		}

		private static readonly Type[] _NoSellList =
		{
			typeof(BaseIngot), typeof(BaseWoodBoard), typeof(BaseLog), typeof(BaseLeather), typeof(BaseHides), typeof(Cloth),
			typeof(BoltOfCloth), typeof(UncutCloth), typeof(Wool), typeof(Cotton), typeof(Flax), typeof(SpoolOfThread),
			typeof(Feather), typeof(Shaft), typeof(Arrow), typeof(Bolt)
		};

		public static void TryBlessItem(PlayerMobile pm, object targeted)
		{
			if (targeted is Item item)
			{
				if (CanBlessItem(pm, item))
				{
					if (pm.BlessedItem != null && pm.BlessedItem == item)
					{
						pm.BlessedItem.LootType = LootType.Regular;

						pm.SendLocalizedMessage(1075292, pm.BlessedItem.Name ?? "#" + pm.BlessedItem.LabelNumber); // ~1_NAME~ has been unblessed.

						pm.BlessedItem = null;
					}
					else if (item.LootType == LootType.Regular && !(item is Container))
					{
						var old = pm.BlessedItem;

						pm.BlessedItem = item;
						pm.BlessedItem.LootType = LootType.Blessed;

						pm.SendLocalizedMessage(1075293, pm.BlessedItem.Name ?? "#" + pm.BlessedItem.LabelNumber); // ~1_NAME~ has been blessed.

						if (old != null)
						{
							old.LootType = LootType.Regular;

							pm.SendLocalizedMessage(1075292, old.Name ?? "#" + old.LabelNumber); // ~1_NAME~ has been unblessed.
						}
					}
				}
				else
				{
					pm.SendLocalizedMessage(1045114); // You cannot bless that item
				}
			}
		}

		public static bool CanBlessItem(PlayerMobile pm, Item item)
		{
			return item.RootParent == pm && !item.Stackable && (item is BaseArmor || item is BaseJewel || item is BaseClothing || item is BaseWeapon);
		}

		public static void CheckUsesRemaining(Mobile from, Item item)
		{
			if (item is IUsesRemaining uses)
			{
				uses.ShowUsesRemaining = true;

				if (--uses.UsesRemaining <= 0)
				{
					item.Delete();

					from.SendLocalizedMessage(1044038); // You have worn out your tool!
				}
			}
		}
	}
}
