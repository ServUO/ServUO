using System.Linq;

using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public static class GMBody
	{
		[ConfigProperty("Staff.Body")]
		public static int Body { get => Config.Get("Staff.Body", 987); set => Config.Set("Staff.Body", value); }

		[ConfigProperty("Staff.CutHair")]
		public static bool CutHair { get => Config.Get("Staff.CutHair", false); set => Config.Set("Staff.CutHair", value); }

		[ConfigProperty("Staff.CutFacialHair")]
		public static bool CutFacialHair { get => Config.Get("Staff.CutFacialHair", false); set => Config.Set("Staff.CutFacialHair", value); }

		[ConfigProperty("Staff.GiveRing")]
		public static bool GiveRing { get => Config.Get("Staff.GiveRing", true); set => Config.Set("Staff.GiveRing", value); }

		[ConfigProperty("Staff.GiveOrb")]
		public static bool GiveOrb { get => Config.Get("Staff.GiveOrb", true); set => Config.Set("Staff.GiveOrb", value); }

		[ConfigProperty("Staff.GiveStone")]
		public static bool GiveStone { get => Config.Get("Staff.GiveStone", true); set => Config.Set("Staff.GiveStone", value); }

		[ConfigProperty("Staff.GiveMount")]
		public static bool GiveMount { get => Config.Get("Staff.GiveMount", true); set => Config.Set("Staff.GiveMount", value); }

		[ConfigProperty("Staff.GiveBoots")]
		public static bool GiveBoots { get => Config.Get("Staff.GiveBoots", true); set => Config.Set("Staff.GiveBoots", value); }

		[ConfigProperty("Staff.UseColoring")]
		public static bool UseColoring { get => Config.Get("Staff.UseColoring", true); set => Config.Set("Staff.UseColoring", value); }

		// Counselor, Decorator, Spawner, GameMaster, Seer, Administrator, Developer, CoOwner, Owner
		private static readonly int[] m_DefaultColors = { 3, 85, 85, 39, 467, 1001, 1001, 1001, 1001 };

		[ConfigProperty("Staff.Colors")]
		public static int[] Colors { get => Config.GetArray("Staff.Colors", m_DefaultColors); set => Config.SetArray("Staff.Colors", value); }

		public static void Configure()
		{
			CommandSystem.Register("GMBody", AccessLevel.Counselor, GM_OnCommand);
		}

		[Usage("GMBody")]
		[Description("Helps staff members get going.")]
		public static void GM_OnCommand(CommandEventArgs e)
		{
			Process(e.Mobile);
		}

		public static void Process(Mobile m)
		{
			if (m?.Deleted != false || m.AccessLevel < AccessLevel.Counselor)
			{
				return;
			}

			CommandLogging.WriteLine(m, "{0} {1} is assuming a GM body", m.AccessLevel, CommandLogging.Format(m));

			var hue = 0;

			if (UseColoring)
			{
				var colors = Colors;
				var access = (int)m.AccessLevel - 2;

				if (access >= 0 && access < colors.Length)
				{
					hue = colors[access];
				}
			}

			var pack = m.Backpack;

			if (pack?.Deleted != false)
			{
				m.AddItem(pack = new Backpack
				{
					Hue = hue,
					Movable = false
				});
			}
			else if (hue > 0)
			{
				pack.Hue = hue;
			}

			var items = m.Items;
			var count = items.Count;

			while (--count >= 0)
			{
				if (count >= items.Count)
				{
					continue;
				}

				var item = items[count];

				if (item != pack && item.Layer != Layer.Bank && item.Layer != Layer.Backpack && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.Layer != Layer.Face && item.Layer != Layer.Mount)
				{
					pack.DropItem(item);
				}
			}

			m.Hidden = true;
			m.Blessed = true;

			if (Body > 0)
			{
				m.BodyValue = Body;
			}

			if (UseColoring)
			{
				m.BodyHue = m.NameHue = hue;
			}

			if (CutHair)
			{
				m.HairItemID = 0;
			}

			if (m.Female || CutFacialHair)
			{
				m.FacialHairItemID = 0;
			}

			m.Hunger = m.Thirst = 20;

			m.Fame = m.Karma = m.Kills = 0;

			m.RawStr = m.RawDex = m.RawInt = 100;

			m.Hits = m.HitsMax;
			m.Mana = m.ManaMax;
			m.Stam = m.StamMax;

			for (var i = 0; i < m.Skills.Length; ++i)
			{
				m.Skills[i].Base = m.Skills[i].Cap = 120.0;
			}

			if (GiveOrb && !Exists<StaffOrb>(m))
			{
				PackItem(m, new StaffOrb());
			}

			if (GiveStone && !Exists<GMHidingStone>(m))
			{
				PackItem(m, new GMHidingStone());
			}

			if (GiveMount)
			{
				if (!Exists<GMEthereal>(m, out var mount))
				{
					mount = new GMEthereal
					{
						Hue = hue
					};
				}

				if (mount.Rider != m)
				{
					PackItem(m, mount);
				}
			}

			if (GiveRing)
			{
				if (!Exists<StaffRing>(m, out var ring))
				{
					ring = new StaffRing();
				}
				
				EquipItem(m, ring);
			}

			if (GiveBoots)
			{
				EquipItem(m, new FurBoots(hue));
			}
		}

		private static bool Exists<T>(Mobile m) where T : Item
		{
			return Exists<T>(m, out _);
		}

		private static bool Exists<T>(Mobile m, out T found) where T : Item
		{
			foreach (var o in m.Items)
			{
				if (o is T t)
				{
					found = t;
					return true;
				}
			}

			return (found = m?.Backpack?.FindItemByType<T>()) != null || (found = m?.BankBox?.FindItemByType<T>()) != null;
		}

		private static void EquipItem(Mobile m, Item item)
		{
			EquipItem(m, item, false);
		}

		private static void EquipItem(Mobile m, Item item, bool mustEquip)
		{
			if (item?.Deleted != false || item.Parent == m)
			{
				return;
			}

			if (m?.Deleted == false)
			{
				if (m.EquipItem(item))
				{
					return;
				}

				if (!mustEquip)
				{
					PackItem(m, item);
					return;
				}
			}

			item.Delete();
		}

		private static void PackItem(Mobile m, Item item)
		{
			if (item?.Deleted != false)
			{
				return;
			}

			if (m?.Deleted == false)
			{
				var pack = m.Backpack;

				if (pack?.Deleted == false)
				{
					pack.DropItem(item);
					return;
				}
			}

			item.Delete();
		}
	}
}
