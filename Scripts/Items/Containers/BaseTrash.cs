using Server.Commands;
using Server.ContextMenus;
using Server.Engines.Points;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
	public static class TrashHelper
	{
		public static void AddMenuEntry(Mobile from, List<ContextMenuEntry> list)
		{
			if (CleanUpBritanniaData.Enabled && from is PlayerMobile)
			{
				list.Add(new AppraiseforCleanup(from));
			}
		}

		public static int AddCleanupItem(HashSet<CleanupEntry> cleanup, Mobile from, Item item)
		{
			if (!CleanUpBritanniaData.Enabled)
			{
				return 0;
			}

			var added = 0;

			if (item is Container c)
			{
				foreach (var child in c.FindItems())
				{
					var points = CleanUpBritanniaData.GetPoints(child);

					if (points > 0)
					{
						var entry = new CleanupEntry(child, from, points);

						if (cleanup.Add(entry))
						{
							++added;
						}
					}
				}
			}
			else
			{
				var points = CleanUpBritanniaData.GetPoints(item);

				if (points > 0)
				{
					var entry = new CleanupEntry(item, from, points);

					if (cleanup.Add(entry))
					{
						++added;
					}
				}
			}

			return added;
		}

		public static bool Empty(List<Item> items, HashSet<CleanupEntry> cleanup)
		{
			return Empty(items, cleanup, null);
		}

		public static bool Empty(List<Item> items, HashSet<CleanupEntry> cleanup, Action<Item> handler)
		{
			if (items.Count <= 0)
			{
				return false;
			}

			var i = items.Count;

			while (--i >= 0)
			{
				if (i >= items.Count)
				{
					continue;
				}

				if (ConfirmCleanupItem(cleanup, items[i]))
				{
					if (handler != null)
					{
						handler(items[i]);
					}
					else
					{
						items[i].Delete();
					}
				}
			}

			foreach (var o in cleanup.GroupBy(x => x.Mobile))
			{
				var m = o.Key;

				if (m == null)
				{
					continue;
				}

				var points = 0.0;
				var count = 0;

				foreach (var e in o)
				{
					if (e.Item.Deleted && e.Confirm)
					{
						points += e.Points;

						++count;
					}
				}

				PointsSystem.CleanUpBritannia.AwardPoints(m, points);

				// You have received approximately ~1_VALUE~points for turning in ~2_COUNT~items for Clean Up Britannia.
				m.SendLocalizedMessage(1151280, $"{points}\t{count}");
			}

			cleanup.Clear();

			return true;
		}

		public static bool ConfirmCleanupItem(HashSet<CleanupEntry> cleanup, Item item)
		{
			foreach (var e in cleanup)
			{
				if (e.Item == item || (item is Container c && e.Item.IsChildOf(c)))
				{
					return e.Confirm = true;
				}
			}

			return false;
		}

		public static void DropToCavernOfDiscarded(Item item)
		{
			if (item?.Deleted != false)
			{
				return;
			}

			var rec = new Rectangle2D(901, 482, 40, 27);
			var map = Map.TerMur;

			for (var i = 0; i < 50; i++)
			{
				var x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
				var y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
				var z = map.GetAverageZ(x, y);

				var p = new Point3D(x, y, z);

				if (map.CanSpawnMobile(p))
				{
					item.MoveToWorld(p, map);
					return;
				}
			}

			item.Delete();
		}

		public static void WriteEntries(GenericWriter writer, HashSet<CleanupEntry> cleanup)
		{
			writer.WriteEncodedInt(0); // version

			writer.WriteEncodedInt(cleanup.Count);

			foreach (var entry in cleanup)
			{
				entry.Serialize(writer);
			}
		}

		public static void ReadEntries(GenericReader reader, HashSet<CleanupEntry> cleanup)
		{
			reader.ReadEncodedInt();

			var count = reader.ReadEncodedInt();

			while (--count >= 0)
			{
				var entry = new CleanupEntry(reader);

				if (entry.Item != null)
				{
					cleanup.Add(entry);
				}
			}
		}

		public class AppraiseforCleanup : ContextMenuEntry
		{
			private readonly Mobile m_Mobile;

			public AppraiseforCleanup(Mobile mobile)
				: base(1151298, 2) //Appraise for Cleanup
			{
				m_Mobile = mobile;
			}

			public override void OnClick()
			{
				m_Mobile.Target = new AppraiseforCleanupTarget(m_Mobile);

				// Target items to see how many Clean Up Britannia points you will receive for throwing them away.
				// Continue targeting items until done, then press the ESC key to cancel the targeting cursor.
				m_Mobile.SendLocalizedMessage(1151299);
			}
		}
	}

	public sealed class CleanupEntry : IEquatable<CleanupEntry>, IComparable<CleanupEntry>
	{
		public Item Item { get; private set; }
		public Mobile Mobile { get; private set; }
		public double Points { get; private set; }

		public bool Confirm { get; set; }

		public CleanupEntry(Item item, Mobile mobile, double points)
		{
			Item = item;
			Mobile = mobile;
			Points = points;
		}

		public CleanupEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		public override int GetHashCode()
		{
			return Item?.GetHashCode() ?? 0;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as CleanupEntry);
		}

		public bool Equals(CleanupEntry other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			if (ReferenceEquals(other, this))
			{
				return true;
			}

			return Item == other.Item;
		}

		public int CompareTo(CleanupEntry other)
		{
			if (ReferenceEquals(other, null))
			{
				return -1;
			}

			if (ReferenceEquals(other, this))
			{
				return 0;
			}

			return Points.CompareTo(other.Points);
		}

		public void Serialize(GenericWriter writer)
		{
			writer.WriteEncodedInt(0); // version

			writer.Write(Item);
			writer.Write(Mobile);
			writer.Write(Points);
			writer.Write(Confirm);
		}

		public void Deserialize(GenericReader reader)
		{
			reader.ReadEncodedInt();

			Mobile = reader.ReadMobile();
			Item = reader.ReadItem();
			Points = reader.ReadDouble();
			Confirm = reader.ReadBool();
		}
	}

	public abstract class BaseTrash : Container
	{
		private Timer m_Timer;

		private readonly HashSet<CleanupEntry> m_Cleanup = new HashSet<CleanupEntry>();

		public virtual double CavernOfDiscardedChance => 0.0;

		public virtual int EmptyTriggerCount => 50;

		public virtual TextDefinition EmptyBroadcast => 0;

		public virtual TextDefinition EmptyMessage => 501478; // The trash is full!  Emptying!
		public virtual TextDefinition EmptyWarning => 1010442; // The item will be deleted in three minutes

		public override int DefaultMaxItems => EmptyTriggerCount;
		public override int DefaultMaxWeight => 0;

		public override bool IsDecoContainer => false;

		public BaseTrash(int itemID)
			: base(itemID)
		{
		}

		public BaseTrash(Serial serial) : base(serial)
		{
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			TrashHelper.AddMenuEntry(from, list);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (!base.OnDragDrop(from, dropped))
			{
				return false;
			}

			AddCleanupItem(from, dropped);

			OnItemsAdded(from);

			return true;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if (!base.OnDragDropInto(from, item, p))
			{
				return false;
			}

			AddCleanupItem(from, item);

			OnItemsAdded(from);

			return true;
		}

		private void OnItemsAdded(Mobile from)
		{
			var max = EmptyTriggerCount;

			if (max > 0 && Items.Count >= max)
			{
				TextDefinition.SendMessageTo(from, EmptyMessage);

				EmptyCallback();
			}
			else
			{
				TextDefinition.SendMessageTo(from, EmptyWarning);

				m_Timer?.Stop();
				m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), EmptyCallback);
			}
		}

		public virtual int AddCleanupItem(Mobile from, Item item)
		{
			return TrashHelper.AddCleanupItem(m_Cleanup, from, item);
		}

		private void EmptyCallback()
		{
			m_Timer?.Stop();
			m_Timer = null;

			Empty();

			m_Cleanup.Clear();
		}

		public virtual void Empty()
		{
			var msg = EmptyBroadcast;

			if (msg.Number > 0)
			{
				if (!String.IsNullOrWhiteSpace(msg.String))
				{
					PublicOverheadMessage(MessageType.Regular, 0x3B2, msg.Number, msg.String);
				}
				else
				{
					PublicOverheadMessage(MessageType.Regular, 0x3B2, msg.Number);
				}
			}
			else if (!String.IsNullOrWhiteSpace(msg.String))
			{
				PublicOverheadMessage(MessageType.Regular, 0x3B2, false, msg.String);
			}

			if (TrashHelper.Empty(Items, m_Cleanup, OnEmpty))
			{
				OnEmpty();
			}
		}

		protected virtual void OnEmpty(Item item)
		{
			var cod = CavernOfDiscardedChance;

			if (cod > 0 && Utility.RandomDouble() <= cod)
			{
				TrashHelper.DropToCavernOfDiscarded(item);
			}
			else
			{
				item.Delete();
			}
		}

		protected virtual void OnEmpty()
		{
			ColUtility.SafeDelete(Items);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(1); // version

			TrashHelper.WriteEntries(writer, m_Cleanup);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.ReadInt();

			if (v >= 1)
			{
				TrashHelper.ReadEntries(reader, m_Cleanup);
			}

			if (Items.Count > 0)
			{
				m_Timer?.Stop();
				m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), EmptyCallback);
			}
			else
			{
				m_Cleanup.Clear();
			}
		}
	}

	public abstract class BaseTrashAddon : BaseAddonContainer
	{
		private Timer m_Timer;

		protected readonly HashSet<CleanupEntry> m_Cleanup = new HashSet<CleanupEntry>();

		public virtual double CavernOfDiscardedChance => 0.0;

		public virtual int EmptyTriggerCount => 50;

		public virtual TextDefinition EmptyBroadcast => 0;

		public virtual TextDefinition EmptyMessage => 501478; // The trash is full!  Emptying!
		public virtual TextDefinition EmptyWarning => 1010442; // The item will be deleted in three minutes

		public override int DefaultMaxItems => EmptyTriggerCount;
		public override int DefaultMaxWeight => 0;

		public override bool IsDecoContainer => false;

		public BaseTrashAddon(int itemID)
			: base(itemID)
		{
		}

		public BaseTrashAddon(Serial serial)
			: base(serial)
		{
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			base.GetContextMenuEntries(from, list);

			TrashHelper.AddMenuEntry(from, list);
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (!base.OnDragDrop(from, dropped))
			{
				return false;
			}

			AddCleanupItem(from, dropped);

			OnItemsAdded(from);

			return true;
		}

		public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
		{
			if (!base.OnDragDropInto(from, item, p))
			{
				return false;
			}

			AddCleanupItem(from, item);

			OnItemsAdded(from);

			return true;
		}

		private void OnItemsAdded(Mobile from)
		{
			var max = EmptyTriggerCount;

			if (max > 0 && Items.Count >= max)
			{
				TextDefinition.SendMessageTo(from, EmptyMessage);

				EmptyCallback();
			}
			else
			{
				TextDefinition.SendMessageTo(from, EmptyWarning);

				m_Timer?.Stop();
				m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), EmptyCallback);
			}
		}

		private void EmptyCallback()
		{
			m_Timer?.Stop();
			m_Timer = null;

			Empty();

			m_Cleanup.Clear();
		}

		public virtual void Empty()
		{
			var msg = EmptyBroadcast;

			if (msg.Number > 0)
			{
				if (!String.IsNullOrWhiteSpace(msg.String))
				{
					PublicOverheadMessage(MessageType.Regular, 0x3B2, msg.Number, msg.String);
				}
				else
				{
					PublicOverheadMessage(MessageType.Regular, 0x3B2, msg.Number);
				}
			}
			else if (!String.IsNullOrWhiteSpace(msg.String))
			{
				PublicOverheadMessage(MessageType.Regular, 0x3B2, false, msg.String);
			}

			if (TrashHelper.Empty(Items, m_Cleanup, OnEmpty))
			{
				OnEmpty();
			}
		}

		protected virtual void OnEmpty(Item item)
		{
			var cod = CavernOfDiscardedChance;

			if (cod > 0 && Utility.RandomDouble() <= cod)
			{
				TrashHelper.DropToCavernOfDiscarded(item);
			}
			else
			{
				item.Delete();
			}
		}

		protected virtual void OnEmpty()
		{
			ColUtility.SafeDelete(Items);
		}

		public virtual int AddCleanupItem(Mobile from, Item item)
		{
			return TrashHelper.AddCleanupItem(m_Cleanup, from, item);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(1); // version

			TrashHelper.WriteEntries(writer, m_Cleanup);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.ReadEncodedInt();

			if (v >= 1)
			{
				TrashHelper.ReadEntries(reader, m_Cleanup);
			}

			if (Items.Count > 0)
			{
				m_Timer?.Stop();
				m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), EmptyCallback);
			}
			else
			{
				m_Cleanup.Clear();
			}
		}
	}
}
