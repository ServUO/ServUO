using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
	public class SiegeComponent : AddonComponent
	{
		public override bool ForceShowProperties { get { return true; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsDraggable
		{
			get
			{
				if (Addon is ISiegeWeapon)
				{
					return ((ISiegeWeapon)Addon).IsDraggable;
				}
				return false;
			}
			set
			{
				if (Addon is ISiegeWeapon)
				{
					((ISiegeWeapon)Addon).IsDraggable = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool IsPackable
		{
			get
			{
				if (Addon is ISiegeWeapon)
				{
					return ((ISiegeWeapon)Addon).IsPackable;
				}
				return false;
			}
			set
			{
				if (Addon is ISiegeWeapon)
				{
					((ISiegeWeapon)Addon).IsPackable = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool FixedFacing
		{
			get
			{
				if (Addon is ISiegeWeapon)
				{
					return ((ISiegeWeapon)Addon).FixedFacing;
				}
				return false;
			}
			set
			{
				if (Addon is ISiegeWeapon)
				{
					((ISiegeWeapon)Addon).FixedFacing = value;
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int Facing
		{
			get
			{
				if (Addon is ISiegeWeapon)
				{
					return ((ISiegeWeapon)Addon).Facing;
				}
				return 0;
			}
			set
			{
				if (Addon is ISiegeWeapon)
				{
					((ISiegeWeapon)Addon).Facing = value;
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (Addon != null)
			{
				Addon.OnDoubleClick(from);
			}
		}

		public SiegeComponent(int itemID)
			: base(itemID)
		{
		}

		public SiegeComponent(int itemID, string name)
			: base(itemID)
		{
			Name = name;
		}

		public SiegeComponent(Serial serial)
			: base(serial)
		{
		}

		private class RotateNextEntry : ContextMenuEntry
		{
			private ISiegeWeapon m_weapon;

			public RotateNextEntry(ISiegeWeapon weapon)
				: base(406)
			{
				m_weapon = weapon;
			}

			public override void OnClick()
			{
				if (m_weapon != null)
					m_weapon.Facing++;
			}
		}

		private class RotatePreviousEntry : ContextMenuEntry
		{
			private ISiegeWeapon m_weapon;

			public RotatePreviousEntry(ISiegeWeapon weapon)
				: base(405)
			{
				m_weapon = weapon;
			}

			public override void OnClick()
			{
				if (m_weapon != null)
					m_weapon.Facing--;
			}
		}

		private class BackpackEntry : ContextMenuEntry
		{
			private ISiegeWeapon m_weapon;
			private Mobile m_from;

			public BackpackEntry(Mobile from, ISiegeWeapon weapon)
				: base(2139)
			{
				m_weapon = weapon;
				m_from = from;
			}

			public override void OnClick()
			{
				if (m_weapon != null)
				{

					m_weapon.StoreWeapon(m_from);
				}
			}
		}

		private class ReleaseEntry : ContextMenuEntry
		{
			private Mobile m_from;
			private XmlDrag m_drag;

			public ReleaseEntry(Mobile from, XmlDrag drag)
				: base(6118)
			{
				m_from = from;
				m_drag = drag;
			}

			public override void OnClick()
			{
				if (m_drag == null) return;

				BaseCreature pet = m_drag.DraggedBy as BaseCreature;

				// only allow the person dragging it or their pet to release
				if (m_drag.DraggedBy == m_from || (pet != null && (pet.ControlMaster == m_from || pet.ControlMaster == null)))
				{
					m_drag.DraggedBy = null;
				}
			}
		}

		private class ConnectEntry : ContextMenuEntry
		{
			private Mobile m_from;
			private XmlDrag m_drag;

			public ConnectEntry(Mobile from, XmlDrag drag)
				: base(5119)
			{
				m_from = from;
				m_drag = drag;
			}

			public override void OnClick()
			{
				if (m_drag != null && m_from != null)
				{
					m_from.SendMessage("Select a mobile to drag the weapon");
					m_from.Target = new DragTarget(m_drag);
				}
			}
		}

		private class SetupEntry : ContextMenuEntry
		{
			private ISiegeWeapon m_weapon;
			private Mobile m_from;

			public SetupEntry(Mobile from, ISiegeWeapon weapon)
				: base(97)
			{
				m_weapon = weapon;
				m_from = from;
			}

			public override void OnClick()
			{
				if (m_weapon != null && m_from != null)
				{
					m_weapon.PlaceWeapon(m_from, m_from.Location, m_from.Map);
				}
			}
		}

		public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
		{
			if (Addon is ISiegeWeapon)
			{
				ISiegeWeapon weapon = (ISiegeWeapon)Addon;

				if (!weapon.FixedFacing)
				{
					list.Add(new RotateNextEntry(weapon));
					list.Add(new RotatePreviousEntry(weapon));
				}

				if (weapon.IsPackable)
				{
					list.Add(new BackpackEntry(from, weapon));
				}

				if (weapon.IsDraggable)
				{
					// does it support dragging?
					XmlDrag a = (XmlDrag)XmlAttach.FindAttachment(weapon, typeof(XmlDrag));
					if (a != null)
					{
						// is it currently being dragged?
						if (a.DraggedBy != null && !a.DraggedBy.Deleted)
						{
							list.Add(new ReleaseEntry(from, a));
						}
						else
						{
							list.Add(new ConnectEntry(from, a));
						}
					}
				}

			}
			base.GetContextMenuEntries(from, list);
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			ISiegeWeapon weapon = Addon as ISiegeWeapon;

			if (weapon == null) return;

			if (weapon.Projectile == null || weapon.Projectile.Deleted)
			{
				//list.Add(1061169, "empty"); // range ~1_val~
				list.Add(1042975); // It's empty
			}
			else
			{
				list.Add(500767); // Reloaded
				list.Add(1060658, "Type\t{0}", weapon.Projectile.Name); // ~1_val~: ~2_val~

				ISiegeProjectile projectile = weapon.Projectile as ISiegeProjectile;
				if (projectile != null)
				{
					list.Add(1061169, projectile.Range.ToString()); // range ~1_val~
				}
			}
		}

		private class DragTarget : Target
		{
			private XmlDrag m_attachment;

			public DragTarget(XmlDrag attachment)
				: base(30, false, TargetFlags.None)
			{
				m_attachment = attachment;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_attachment == null || from == null) return;

				if (!(targeted is Mobile))
				{
					from.SendMessage("Must target a mobile");
					return;
				}

				Mobile m = (Mobile)targeted;

				if (m == from || (m is BaseCreature && (((BaseCreature)m).Controlled && ((BaseCreature)m).ControlMaster == from)))
				{
					m_attachment.DraggedBy = m;
				}
				else
				{
					from.SendMessage("You dont control that.");
				}

			}
		}


		public override void OnMapChange()
		{
			if (Addon != null && Map != Map.Internal)
				Addon.Map = Map;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
