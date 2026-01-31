using System;
using System.Collections.Generic;
using System.Linq;

using Server.Items;
using Server.Mobiles;
using Server.Spells;

namespace Server.Multis
{
	public abstract class BaseCamp : BaseMulti
	{
		public static List<BaseCamp> Camps { get; } = new List<BaseCamp>();

		public static void Initialize()
		{
			Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), ProcessCamps);
		}

		public static void ProcessCamps()
		{
			var index = Camps.Count;

			while (--index >= 0)
			{
				if (index >= Camps.Count)
				{
					continue;
				}

				var c = Camps[index];

				if (c?.Deleted != false)
				{
					Camps.RemoveAt(index);
					continue;
				}

				c.CheckPrisoner();
				c.CheckDecay();
			}
		}

		private List<Item> m_Items;
		private List<Mobile> m_Mobiles;

		[CommandProperty(AccessLevel.GameMaster)]
		public DateTime TimeOfDecay { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseCreature Prisoner { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseContainer Treasure1 { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public BaseContainer Treasure2 { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual TimeSpan DecayDelay => TimeSpan.FromMinutes(30.0);

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Decaying => TimeOfDecay > DateTime.MinValue;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool ForceDecay { get => false; set => SetDecayTime(); }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool RestrictDecay { get; set; }

		public override bool HandlesOnMovement => true;

		public virtual int EventRange => 10;

		public BaseCamp(int multiID)
			: base(multiID)
		{
			m_Items = new List<Item>();
			m_Mobiles = new List<Mobile>();

			Visible = false;

			CheckAddComponents();

			Camps.Add(this);
		}

		public BaseCamp(Serial serial)
			: base(serial)
		{
			Camps.Add(this);
		}

		public void CheckAddComponents()
		{
			if (!Deleted)
			{
				AddComponents();
			}
		}

		public virtual void AddComponents()
		{
		}

		public virtual void CheckDecay()
		{
			if (RestrictDecay)
			{
				return;
			}

			if (!Decaying)
			{
				if (Treasure1?.Deleted == false && Treasure1.Items.Count > 0)
				{
					return;
				}

				if (Treasure2?.Deleted == false && Treasure2.Items.Count > 0)
				{
					return;
				}

				if (Prisoner?.Deleted == false && Prisoner.CantWalk)
				{
					return;
				}

				SetDecayTime();
			}
			else if (DateTime.UtcNow >= TimeOfDecay)
			{
				Delete();
			}
		}

		public virtual void SetDecayTime()
		{
			if (!Deleted && !RestrictDecay)
			{
				TimeOfDecay = DateTime.UtcNow + DecayDelay;
			}
		}

		public virtual void AddItem(Item item, int xOffset, int yOffset, int zOffset)
		{
			if (Map == null || item?.Deleted != false)
			{
				return;
			}

			if (!m_Items.Contains(item))
			{
				m_Items.Add(item);
			}

			var zavg = Map.GetAverageZ(X + xOffset, Y + yOffset);

			if (!Map.CanFit(X + xOffset, Y + yOffset, zavg, item.ItemData.Height))
			{
				for (var z = 1; z <= 39; z++)
				{
					if (Map.CanFit(X + xOffset, Y + yOffset, zavg + z, item.ItemData.Height))
					{
						zavg += z;
						break;
					}
				}
			}

			item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, zavg + zOffset), Map);
		}

		public virtual void AddMobile(Mobile m, int xOffset, int yOffset, int zOffset)
		{
			if (Map == null || m?.Deleted != false)
			{
				return;
			}

			if (!m_Mobiles.Contains(m))
			{
				m_Mobiles.Add(m);
			}

			var zavg = Map.GetAverageZ(X + xOffset, Y + yOffset);

			if (!Map.CanSpawnMobile(X + xOffset, Y + yOffset, zavg))
			{
				for (var z = 1; z <= 39; z++)
				{
					if (Map.CanSpawnMobile(X + xOffset, Y + yOffset, zavg + z))
					{
						zavg += z;
						break;
					}
				}
			}

			m.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, zavg + zOffset), Map);

			SetCreature(m as BaseCreature);
		}

		private void SetCreature(BaseCreature bc)
		{
			if (bc?.Deleted == false)
			{
				IPoint3D p = bc.Location;

				SpellHelper.GetSurfaceTop(ref p);

				var loc = new Point3D(p);

				bc.RangeHome = bc.IsPrisoner ? 0 : 6;
				bc.Home = loc;

				if (bc.Location != loc)
				{
					bc.Location = loc;
				}

				if (bc is BaseVendor || bc is Banker)
				{
					bc.Direction = Direction.South;
				}
			}
		}

		public virtual void OnEnter(Mobile m)
		{
		}

		public virtual void OnExit(Mobile m)
		{
		}

		public override void OnLocationChange(Point3D old)
		{
			base.OnLocationChange(old);

			for (var i = m_Items.Count - 1; i >= 0; i--)
			{
				if (i < m_Items.Count && m_Items[i]?.Deleted == false)
				{
					var item = m_Items[i];

					item.Location = new Point3D(X + (item.X - old.X), Y + (item.Y - old.Y), Z + (item.Z - old.Z));
				}
			}

			for (var i = m_Mobiles.Count - 1; i >= 0; i--)
			{
				if (i < m_Mobiles.Count && m_Mobiles[i]?.Deleted == false)
				{
					var m = m_Mobiles[i];

					m.Location = new Point3D(X + (m.X - old.X), Y + (m.Y - old.Y), Z + (m.Z - old.Z));

					SetCreature(m as BaseCreature);
				}
			}
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			for (var i = m_Items.Count - 1; i >= 0; i--)
			{
				if (i < m_Items.Count && m_Items[i]?.Deleted == false)
				{
					m_Items[i].Map = Map;
				}
			}

			for (var i = m_Mobiles.Count - 1; i >= 0; i--)
			{
				if (i < m_Mobiles.Count && m_Mobiles[i]?.Deleted == false)
				{
					m_Mobiles[i].Map = Map;
				}
			}
		}

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			var inOldRange = Utility.InRange(oldLocation, Location, EventRange);
			var inNewRange = Utility.InRange(m.Location, Location, EventRange);

			if (inNewRange && !inOldRange)
			{
				OnEnter(m);
			}
			else if (inOldRange && !inNewRange)
			{
				OnExit(m);
			}
		}

		public override void OnDelete()
		{
			for (var i = m_Items.Count - 1; i >= 0; i--)
			{
				if (i < m_Items.Count && m_Items[i]?.Deleted == false)
				{
					m_Items[i].Delete();
				}
			}

			for (var i = m_Mobiles.Count - 1; i >= 0; i--)
			{
				if (i < m_Mobiles.Count && m_Mobiles[i]?.Deleted == false)
				{
					if (m_Mobiles[i] != Prisoner || m_Mobiles[i].CantWalk)
					{
						m_Mobiles[i].Delete();
					}
				}
			}

			base.OnDelete();
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();

			m_Items.Clear();
			m_Mobiles.Clear();

			Camps.Remove(this);
		}

		protected virtual void AddCampChests()
		{
			AddItem(Treasure1 = new TreasureLevel1()
			{
				Locked = false
			}, 2, 2, 0);

			AddItem(Treasure2 = new TreasureLevel3(), -2, -2, 0);
		}

		public void CheckPrisoner()
		{
			if (Prisoner?.Deleted == false && Prisoner.CantWalk)
			{
				if (m_Mobiles.Count == 0 || m_Mobiles.All(m => m == null || m.Deleted || m == Prisoner || !m.Alive))
				{
					Prisoner.CantWalk = false;
					Prisoner.CanMove = true;
					Prisoner.Frozen = false;

					Prisoner.BeginDeleteTimer();
				}
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(2); // version

			writer.Write(Prisoner);
			writer.Write(Treasure1);
			writer.Write(Treasure2);

			writer.Write(m_Items, true);
			writer.Write(m_Mobiles, true);

			writer.WriteDeltaTime(TimeOfDecay);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 2:
				{
					Prisoner = reader.ReadMobile<BaseCreature>();
					Treasure1 = reader.ReadItem<BaseContainer>();
					Treasure2 = reader.ReadItem<BaseContainer>();

					goto case 1;
				}
				case 1:
				case 0:
				{
					m_Items = reader.ReadStrongItemList();
					m_Mobiles = reader.ReadStrongMobileList();

					TimeOfDecay = reader.ReadDeltaTime();

					break;
				}
			}

			if (version == 0 && ItemID == 0x10EE)
			{
				ItemID = 0x1F6D;
			}

			if (Prisoner?.Deleted == false)
			{
				Prisoner.IsPrisoner = true;
			}

			if (version < 2)
			{
				Delete();
			}
		}
	}
}
