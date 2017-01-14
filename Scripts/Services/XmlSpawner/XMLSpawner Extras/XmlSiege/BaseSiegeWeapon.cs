using System;
using Server;
using Server.Targeting;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

	public abstract class BaseSiegeWeapon : BaseAddon, ISiegeWeapon
	{

		public virtual double WeaponLoadingDelay { get { return 15.0; } } // base delay for loading this weapon
		public virtual double WeaponStorageDelay { get { return 15.0; } } // base delay for packing away this weapon

		public virtual double DamageReductionWhenDamaged { get { return 0.4; } } // scale damage from 40-100% depending on the damage it has taken 
		public virtual double RangeReductionWhenDamaged { get { return 0.7; } } // scale range from 70-100% depending on the damage it has taken 

		public virtual int MinTargetRange { get { return 1; } } // target must be further away than this
		public virtual int MinStorageRange { get { return 2; } } // player must be at least this close to store the weapon
		public virtual int MinFiringRange { get { return 3; } } // player must be at least this close to fire the weapon

		public virtual bool CheckLOS { get { return true; } } // whether the weapon needs to consider line of sight when selecting a target

		public virtual int StoredWeaponID { get { return 3644; } } // itemid used when the weapon is packed up (crate by default)

		public override BaseAddonDeed Deed { get { return null; } }

		public abstract void UpdateDisplay();

		public abstract Type[] AllowedProjectiles { get;}

		private int m_Facing;
		private Item m_Projectile;
		private DateTime m_NextFiringTime;
		private bool m_FixedFacing = false;
		private bool m_Draggable = true;
		private bool m_Packable = true;
		public bool Storing = false;

		private XmlSiege m_SiegeAttachment = null;

		private XmlSiege SiegeAttachment
		{
			get
			{
				if (m_SiegeAttachment == null)
				{
					m_SiegeAttachment = (XmlSiege)XmlAttach.FindAttachment(this, typeof(XmlSiege));
				}
				return m_SiegeAttachment;
			}

		}


		public int Hits
		{
			get
			{
				return (SiegeAttachment != null) ? SiegeAttachment.Hits : 0;
			}
		}

		public int HitsMax
		{
			get
			{
				return (SiegeAttachment != null) ? SiegeAttachment.HitsMax : 0;
			}
		}

		// default weapon performance factors.
		// taking damage reduces the multiplier

		// default damage multiplier for the weapon
		public virtual double WeaponDamageFactor
		{
			get
			{
				if (HitsMax > 0)
				{
					return ((1 - DamageReductionWhenDamaged) * Hits / HitsMax) + DamageReductionWhenDamaged;
				}
				return 1;
			}
		}

		// default range multiplier for the weapon
		public virtual double WeaponRangeFactor
		{
			get
			{
				if (HitsMax > 0)
				{
					return ((1 - RangeReductionWhenDamaged) * Hits / HitsMax) + RangeReductionWhenDamaged;
				}
				return 1;
			}
		}

		public virtual Item Projectile
		{
			get { return m_Projectile; }
			set
			{
				m_Projectile = value;
				// invalidate component properties
				if (Components != null)
				{
					foreach (AddonComponent c in Components)
					{
						c.InvalidateProperties();
					}
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool IsDraggable { get { return m_Draggable; } set { m_Draggable = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool IsPackable { get { return m_Packable; } set { m_Packable = value; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual bool FixedFacing
		{
			get { return m_FixedFacing; }
			set { m_FixedFacing = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Facing
		{
			get { return m_Facing; }
			set
			{
				m_Facing = value;
				if (m_Facing < 0) m_Facing = 3;
				if (m_Facing > 3) m_Facing = 0;
				UpdateDisplay();
				// save the current state of the itemids
				if (SiegeAttachment != null)
				{
					SiegeAttachment.StoreOriginalItemID(this);
				}
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan NextFiring
		{
			get { return m_NextFiringTime - DateTime.UtcNow; }
			set
			{
				m_NextFiringTime = DateTime.UtcNow + value;
			}
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (m_Projectile != null)
			{
				m_Projectile.Delete();
			}
		}

		public virtual void StoreWeapon_Callback(object state)
		{
			object[] args = (object[])state;

			Mobile from = (Mobile)args[0];
			BaseSiegeWeapon weapon = (BaseSiegeWeapon)args[1];

			if (weapon == null || weapon.Deleted || from == null) return;

			// make sure that there is only one person nearby
			IPooledEnumerable moblist = from.Map.GetMobilesInRange(weapon.Location, MinStorageRange);
			int count = 0;
			if (moblist != null)
			{
				foreach (Mobile m in moblist)
				{
					if (m.Player) count++;
				}
			}
			if (count > 1)
			{
				from.SendMessage("Too many players nearby. Storage failed.");
				return;
			}

			// make sure that the player is still next to the weapon
			if (!from.InRange(weapon.Location, MinStorageRange) || from.Map != weapon.Map)
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				from.SendMessage("{0} not stored.", weapon.Name);
				return;
			}

			// use the crate itemid while stored
			weapon.ItemID = StoredWeaponID;
			weapon.Visible = true;
			weapon.Movable = true;
			from.AddToBackpack(weapon);

			// hide the components
			foreach (AddonComponent i in weapon.Components)
			{
				if (i != null)
				{
					i.Internalize();
				}
			}

			from.SendMessage("{0} stored.", weapon.Name);
			weapon.Storing = false;
		}

		public virtual void PlaceWeapon(Mobile from, Point3D location, Map map)
		{
			MoveToWorld(location, map);
			UpdateDisplay();
		}

		public virtual void StoreWeapon(Mobile from)
		{
			if (from == null) return;

			if (!from.InRange(this.Location, 2) || from.Map != this.Map)
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			// 15 second delay to pack up the cannon
			Timer.DelayCall(TimeSpan.FromSeconds(WeaponStorageDelay), new TimerStateCallback(StoreWeapon_Callback),
			new object[] { from, this });

			from.SendMessage("Packing up the {0}...", Name);

			Storing = true;
		}

		private bool ContainsInterface(Type[] typearray, Type type)
		{
			if (typearray == null || type == null) return false;

			foreach (Type t in typearray)
			{
				if (t == type) return true;
			}

			return false;
		}

		public bool CheckAllowedProjectile(Item projectile)
		{
			if (projectile == null || AllowedProjectiles == null) return false;

			for (int i = 0; i < AllowedProjectiles.Length; i++)
			{
				Type t = AllowedProjectiles[i];
				Type pt = projectile.GetType();

				if (t == null || pt == null) continue;

				if (pt.IsSubclassOf(t) || pt.Equals(t) || (t.IsInterface && ContainsInterface(pt.GetInterfaces(), t)))
				{
					return true;
				}
			}

			return false;
		}

		public virtual void LoadWeapon(Mobile from, Item projectile)
		{
			if (projectile == null) return;

			// can't load destroyed weapons
			if (Hits == 0) return;

			// restrict allowed projectiles
			if (!CheckAllowedProjectile(projectile))
			{
				from.SendMessage("That cannot be loaded into this weapon");
				return;
			}

			if (m_Projectile != null && !m_Projectile.Deleted)
			{

				from.SendMessage("{0} unloaded", m_Projectile.Name);
				from.AddToBackpack(m_Projectile);
			}

			if (projectile.Amount > 1)
			{
				//projectile.Amount--;
				//Projectile = projectile.Dupe(1);
				Projectile = Mobile.LiftItemDupe(projectile, projectile.Amount-1);

			}
			else
			{
				Projectile = projectile;
			}

			if (m_Projectile != null)
			{

				m_Projectile.Internalize();

				from.SendMessage("{0} loaded", m_Projectile.Name);
			}
		}

		public override bool OnDroppedToWorld(Mobile from, Point3D point)
		{
			bool dropped = base.OnDroppedToWorld(from, point);

			if (dropped)
			{
				ItemID = 1;
				Visible = false;
				Movable = false;
				UpdateDisplay();
			}
			return dropped;
		}

		public virtual bool HasFiringAngle(IPoint3D t)
		{
			int dy = t.Y - Y;
			int dx = t.X - X;

			switch (Facing)
			{
				case 0:
					return t.X < X && ((dy <= 0 && -dy <= -dx) || (dy > 0 && dy <= -dx));
				case 1:
					return t.Y < Y && ((dx <= 0 && -dx <= -dy) || (dx > 0 && dx <= -dy));
				case 2:
					return t.X > X && ((dy <= 0 && -dy <= dx) || (dy > 0 && dy <= dx));
				case 3:
					return t.Y > Y && ((dx <= 0 && -dx <= dy) || (dx > 0 && dx <= dy));
			}

			return false;
		}

		public BaseSiegeWeapon()
		{
		}

		public BaseSiegeWeapon(Serial serial)
			: base(serial)
		{
		}



		public virtual Point3D ProjectileLaunchPoint
		{
			get
			{
				return (this.Location);
			}
		}

		public virtual bool AttackTarget(Mobile from, IEntity target, Point3D targetloc, bool checkLOS)
		{
			ISiegeProjectile projectile = m_Projectile as ISiegeProjectile;

			if (from == null || from.Map == null || projectile == null) return false;

			if (!HasFiringAngle(targetloc))
			{
				from.SendMessage("No firing angle");
				return false;
			}

			// check the target range
			int distance = (int)XmlSiege.GetDistance(targetloc, Location);

			int projectilerange = (int)(projectile.Range * WeaponRangeFactor);

			if (projectilerange < distance)
			{
				from.SendMessage("Out of range");
				return false;
			}

			if (distance <= MinTargetRange)
			{
				from.SendMessage("Target is too close");
				return false;
			}

			// check the target line of sight
			int height = 1;
			if (target is Item)
			{
				height = ((Item)target).ItemData.Height;
			}
			else if (target is Mobile)
			{
				height = 14;
			}

			Point3D adjustedloc = new Point3D(targetloc.X, targetloc.Y, targetloc.Z + height);

			if (checkLOS && !Map.LineOfSight(this, adjustedloc))
			{
				from.SendMessage("Cannot see target");
				return false;
			}

			// ok, the projectile is being fired
			// calculate attack parameters
			double firingspeedbonus = projectile.FiringSpeed / 10.0;
			double dexbonus = (double)from.Dex / 30.0;
			int weaponskill = (int)from.Skills[SkillName.ArmsLore].Value;


			int accuracybonus = projectile.AccuracyBonus;


			// calculate the cooldown time with dexterity bonus and firing speed bonus on top of the base delay
			double loadingdelay = WeaponLoadingDelay - dexbonus - firingspeedbonus;

			m_NextFiringTime = DateTime.UtcNow + TimeSpan.FromSeconds(loadingdelay);

			// calculate the accuracy based on distance and weapon skill
			int accuracy = distance * 10 - weaponskill + accuracybonus;

			if (Utility.Random(100) < accuracy)
			{
				from.SendMessage("Target missed");
				// consume the ammunition
				m_Projectile.Consume(1);
				// update the properties display
				Projectile = m_Projectile;
				return true;
			}

			LaunchProjectile(from, m_Projectile, target, targetloc, TimeSpan.FromSeconds((double)distance * 0.08));

			return true;
		}


		public virtual void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
		{
			ISiegeProjectile pitem = projectile as ISiegeProjectile;

			if (pitem == null) return;

			int animationid = pitem.AnimationID;
			int animationhue = pitem.AnimationHue;

			// show the projectile moving to the target
			XmlSiege.SendMovingProjectileEffect(this, target, animationid, ProjectileLaunchPoint, targetloc, 7, 0, false, true, animationhue);

			// delayed damage at the target to account for travel distance of the projectile
			Timer.DelayCall(delay, new TimerStateCallback(DamageTarget_Callback),
			new object[] { from, this, target, targetloc, projectile });

			return;

		}

		public virtual void DamageTarget_Callback(object state)
		{
			object[] args = (object[])state;

			Mobile from = (Mobile)args[0];
			BaseSiegeWeapon weapon = (BaseSiegeWeapon)args[1];
			IEntity target = (IEntity)args[2];
			Point3D targetloc = (Point3D)args[3];
			Item pitem = (Item)args[4];

			ISiegeProjectile projectile = pitem as ISiegeProjectile;

			if (projectile != null)
			{
				projectile.OnHit(from, weapon, target, targetloc);
			}
		}


		public override void OnDoubleClick(Mobile from)
		{
			if (Parent != null) return;

			// can't use destroyed weapons
			if (Hits == 0) return;

			// check the range between the player and weapon
			if (!from.InRange(Location, MinFiringRange) || from.Map != Map)
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			if (Storing)
			{
				from.SendMessage("{0} being stored", Name);
				return;
			}

			if (m_Projectile == null || m_Projectile.Deleted)
			{
				from.SendMessage("{0} empty", Name);
				return;
			}

			// check if the cannon is cool enough to fire
			if (m_NextFiringTime > DateTime.UtcNow)
			{
				from.SendMessage("Not ready yet.");
				return;
			}

			from.Target = new SiegeTarget(this, from, CheckLOS);

		}


		private class SiegeTarget : Target
		{
			private BaseSiegeWeapon m_weapon;
			private Mobile m_from;
			private bool m_checklos;

			public SiegeTarget(BaseSiegeWeapon weapon, Mobile from, bool checklos)
				: base(30, true, TargetFlags.None)
			{
				m_weapon = weapon;
				m_from = from;
				m_checklos = checklos;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || m_weapon == null || from.Map == null) return;

				if (targeted is StaticTarget)
				{
					int staticid = ((StaticTarget)targeted).ItemID;
					int staticx = ((StaticTarget)targeted).Location.X;
					int staticy = ((StaticTarget)targeted).Location.Y;

					Item multiitem = null;
					Point3D tileloc = Point3D.Zero;

					// find the possible multi owner of the static tile
					foreach (Item item in from.Map.GetItemsInRange(((StaticTarget)targeted).Location, 50))
					{

						if (item is BaseMulti)
						{
							// search the component list for a match
							MultiComponentList mcl = ((BaseMulti)item).Components;
							bool found = false;
							if (mcl != null && mcl.List != null)
							{
								for (int i = 0; i < mcl.List.Length; i++)
								{
									MultiTileEntry t = mcl.List[i];

									int x = t.m_OffsetX + item.X;
									int y = t.m_OffsetY + item.Y;
									int z = t.m_OffsetZ + item.Z;
									int itemID = t.m_ItemID & 0x3FFF;

									if (itemID == staticid && x == staticx && y == staticy)
									{
										found = true;
										tileloc = new Point3D(x, y, z);
										break;
									}

								}
							}

							if (found)
							{
								multiitem = item;
								break;
							}
						}
					}
					if (multiitem != null)
					{
						//Console.WriteLine("attacking {0} at {1}:{2}", multiitem, tileloc, ((StaticTarget)targeted).Location);
						// may have to reconsider the use tileloc vs target loc
						//m_cannon.AttackTarget(from, multiitem, ((StaticTarget)targeted).Location);

						m_weapon.AttackTarget(from, multiitem, multiitem.Map.GetPoint(targeted, true), m_checklos);
					}
				}
				else
					if (targeted is IEntity)
					{
						// attack the target
						m_weapon.AttackTarget(from, (IEntity)targeted, ((IEntity)targeted).Location, m_checklos);
					}
					else
						if (targeted is LandTarget)
						{
							// attack the target
							m_weapon.AttackTarget(from, null, ((LandTarget)targeted).Location, m_checklos);
						}
			}
		}



		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)1); // version
			// version 1
			writer.Write(m_FixedFacing);
			writer.Write(m_Draggable);
			writer.Write(m_Packable);
			// version 0
			writer.Write(m_Facing);
			writer.Write(m_Projectile);
			writer.Write(m_NextFiringTime);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					m_FixedFacing = reader.ReadBool();
					m_Draggable = reader.ReadBool();
					m_Packable = reader.ReadBool();
					goto case 0;
				case 0:
					m_Facing = reader.ReadInt();
					m_Projectile = reader.ReadItem();
					m_NextFiringTime = reader.ReadDateTime();
					break;
			}
		}
	}
}
