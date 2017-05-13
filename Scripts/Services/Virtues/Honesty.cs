#region Header
// **********
// ServUO - Honesty.cs
// **********
#endregion

#region References
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Server.Items;
using Server.Mobiles;
#endregion

namespace Server.Services.Virtues
{
	public static class Honesty
	{
        public static bool Enabled = Config.Get("Honesty.Enabled", true);
        public static int MaxGeneration = Config.Get("Honesty.MaxGeneration", 1000);
        public static bool TrammelGeneration = Config.Get("Honesty.TrammelGeneration", true);

		private static readonly string[] _Regions =
		{
			"Britain", "Minoc", "Magincia", "Trinsic", "Jhelom", "Moonglow",
			"Skara Brae", "Yew"
		};

		private static readonly List<Item> _Items;
		private static readonly object _ItemsLock;

		private static readonly List<int>[] _Invalid;
		private static readonly object _InvalidLock;

		private static readonly Rectangle2D _Bounds = new Rectangle2D(0, 0, 5119, 4095);

		static Honesty()
		{
			_Items = new List<Item>(0x400);
			_ItemsLock = ((ICollection)_Items).SyncRoot;

			_Invalid = new[] {new List<int>(0x1000), new List<int>(0x1000)};
			_InvalidLock = ((ICollection)_Invalid).SyncRoot;
		}

		public static void Initialize()
		{
			EventSink.ItemDeleted += OnItemDeleted;
			EventSink.AfterWorldSave += OnAfterSave;

			VirtueGump.Register(106, OnVirtueUsed);

            if (Enabled)
            {
                foreach (var i in World.Items.Values.Where(item => item.HonestyItem))
                {
                    lock (_ItemsLock)
                    {
                        if (!_Items.Contains(i))
                        {
                            _Items.Add(i);
                        }
                    }
                }

                GenerateHonestyItems();
            }
		}

		private static void OnItemCreated(ItemCreatedEventArgs e)
		{
			if (!e.Item.HonestyItem)
			{
				return;
			}

			lock (_ItemsLock)
			{
				if (!_Items.Contains(e.Item))
				{
					_Items.Add(e.Item);
				}
			}
		}

		private static void OnItemDeleted(ItemDeletedEventArgs e)
		{
			if (!e.Item.HonestyItem)
			{
				return;
			}

			lock (_ItemsLock)
			{
				_Items.Remove(e.Item);
			}
		}

		private static void OnAfterSave(AfterWorldSaveEventArgs e)
		{
			World.WaitForWriteCompletion();

			PruneTaken();

			if (Enabled)
			{
				GenerateHonestyItems();
			}
		}

		public static void OnVirtueUsed(Mobile from)
		{
			if (Enabled)
			{
				from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
			}
		}

		private static void PruneTaken()
		{
			lock (_ItemsLock)
			{
				_Items.RemoveAll(ItemFlags.GetTaken);
			}
		}

		private static void GenerateHonestyItems()
		{
            bool initial = _Items.Count == 0;
            long ticks = Core.TickCount;

            if (initial)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.Write("Honesty Items generating:");
                Utility.PopColor();
            }

			try
			{
				var count = MaxGeneration - _Items.Count;
				var spawned = new Item[count];

				for (var i = 0; i < spawned.Length; i++)
				{
					var item = spawned[i] = Loot.RandomArmorOrShieldOrWeapon();

					if (item == null || item.Deleted)
					{
						--i;

						continue;
					}

					item.HonestyItem = true;

					lock (_ItemsLock)
					{
						if (!_Items.Contains(item))
						{
							_Items.Add(item);
						}
					}
				}

				var locs = new Dictionary<Map, Point3D?[]>();

				if (TrammelGeneration)
				{
					locs[Map.Trammel] = new Point3D?[spawned.Length];
				}

				locs[Map.Felucca] = new Point3D?[spawned.Length];

				Parallel.For(
					0,
					spawned.Length,
					i =>
					{
						var map = TrammelGeneration && Utility.RandomBool() ? Map.Trammel : Map.Felucca;

						locs[map][i] = GetValidLocation(map);
					});

				foreach (var kv in locs)
				{
					var map = kv.Key;
					var points = kv.Value;

					for (var i = 0; i < spawned.Length; i++)
					{
						var loc = points[i];

						if (loc == null || loc == Point3D.Zero)
						{
							continue;
						}

						var item = spawned[i];

						if (item == null || item.Deleted)
						{
							continue;
						}

						ItemFlags.SetTaken(item, false);

						item.MoveToWorld(loc.Value, map);

						spawned[i] = null;
					}
				}

				foreach (var item in spawned.Where(item => item != null && !item.Deleted))
				{
					item.Delete();
				}
			}
			catch (Exception e)
			{
				Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine(" Failed!");
				Console.WriteLine(e);
				Utility.PopColor();
			}

            if (initial)
            {
                Utility.PushColor(ConsoleColor.Green);
                Console.WriteLine(" Done, took {0} milliseconds!", Core.TickCount - ticks);
                Utility.PopColor();
            }
		}

		private static Point3D? GetValidLocation(Map map)
		{
			if (map == null)
			{
				return null;
			}

			var index = map == Map.Trammel ? 0 : 1;

			var attempts = (int)(Math.Sqrt(_Bounds.Width * _Bounds.Height) / 10);

			int x, y, h;
			bool valid;

			do
			{
				x = Utility.RandomMinMax(_Bounds.Start.X, _Bounds.End.X);
				y = Utility.RandomMinMax(_Bounds.Start.Y, _Bounds.End.Y);

				unchecked
				{
					h = (x + map.Width) * y;
					h = (h * 397) ^ x;
					h = (h * 397) ^ y;
				}

				lock (_InvalidLock)
				{
					if (_Invalid[index].Contains(h))
					{
						valid = false;
						continue;
					}
				}

				valid = TreasureMap.ValidateLocation(x, y, map);

				if (!valid)
				{
					lock (_InvalidLock)
					{
						_Invalid[index].Add(h);
					}
				}
			}
			while (!valid && --attempts >= 0);

			if (valid)
			{
				return new Point3D(x, y, map.GetAverageZ(x, y));
			}

			return null;
		}

        public static void AssignOwner(Item item)
        {
            item.HonestyRegion = _Regions[Utility.Random(_Regions.Length)];

            if (!String.IsNullOrWhiteSpace(item.HonestyRegion) && BaseVendor.AllVendors.Count >= 10)
            {
                var attempts = BaseVendor.AllVendors.Count / 10;

                BaseVendor m;

                do
                {
                    m = BaseVendor.AllVendors[Utility.Random(BaseVendor.AllVendors.Count)];
                }
                while ((m == null || m.Map != item.Map || !m.Region.IsPartOf(item.HonestyRegion)) && --attempts >= 0);

                item.HonestyOwner = m;
            }
        }
	}

	public class HonestyChest : Container
	{
        public override int LabelNumber { get { return 1151529; } } // lost and found box

		[Constructable]
		public HonestyChest()
			: base(0x9A9)
		{
		}

		public HonestyChest(Serial serial)
			: base(serial)
		{ }

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			return CheckGain(from, dropped);
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			return CheckGain(from, item);
		}

		public bool CheckGain(Mobile from, Item item)
		{
			if (from == null || from.Deleted || item == null || !item.HonestyItem)
			{
                from.SendLocalizedMessage(1151530); // This is not a lost item.
				return false;
			}

			var reg = Region.Find(Location, Map);

			var gainedPath = false;

			if (item.HonestyRegion == reg.Name)
			{
				VirtueHelper.Award(from, VirtueName.Honesty, 60, ref gainedPath);
			}
			else
			{
				VirtueHelper.Award(from, VirtueName.Honesty, 30, ref gainedPath);
			}

			from.SendMessage(gainedPath ? "You have gained a path in Honesty!" : "You have gained in Honesty.");

			item.Delete();

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}