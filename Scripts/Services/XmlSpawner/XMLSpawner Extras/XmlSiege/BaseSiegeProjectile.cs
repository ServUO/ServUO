using System;
using Server;
using Server.Targeting;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

	public class BaseSiegeProjectile : Item, ISiegeProjectile
	{

		private int m_Range;				// max number of tiles it can travel
		private int m_AccuracyBonus;		// adjustment to accuracy
		private int m_FiringSpeed;			// adjustment to time until next shot in seconds*10
		private int m_Area;					// radius of area damage
		private int m_FireDamage;			// amount of fire damage to the target
		private int m_PhysicalDamage;		// amount of physical damage to the target

		public virtual int AnimationID { get { return 0xE73; } }
		public virtual int AnimationHue { get { return 0; } }

		public virtual double MobDamageMultiplier { get { return 1.0; } } // default damage multiplier for creatures
		public virtual double StructureDamageMultiplier { get { return 1.0; } } // default damage multiplier for structures

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Range
		{
			get { return m_Range; }
			set { m_Range = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int FiringSpeed
		{
			get { return m_FiringSpeed; }
			set { m_FiringSpeed = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int AccuracyBonus
		{
			get { return m_AccuracyBonus; }
			set { m_AccuracyBonus = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int Area
		{
			get { return m_Area; }
			set { m_Area = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int FireDamage
		{
			get { return m_FireDamage; }
			set { m_FireDamage = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public virtual int PhysicalDamage
		{
			get { return m_PhysicalDamage; }
			set { m_PhysicalDamage = value; InvalidateProperties(); }
		}

		public BaseSiegeProjectile()
			: this(1, 0xE74)
		{
		}

		public BaseSiegeProjectile(int amount)
			: this(amount, 0xE74)
		{
		}

		public BaseSiegeProjectile(int amount, int itemid)
			: base(itemid)
		{

			Weight = 5;
			Stackable = true;
			Amount = amount;
		}

		public BaseSiegeProjectile(Serial serial)
			: base(serial)
		{
		}
/*
		public override Item Dupe(int amount)
		{
			BaseSiegeProjectile s = new BaseSiegeProjectile(amount);

			return this.Dupe(s, amount);
		}
*/
		public override void OnAfterDuped(Item newItem)
		{

			base.OnAfterDuped(newItem);

			BaseSiegeProjectile s = newItem as BaseSiegeProjectile;
			// dupe the siege projectile props
			if (s != null)
			{
				s.FiringSpeed = FiringSpeed;
				s.AccuracyBonus = AccuracyBonus;
				s.Area = Area;
				s.Range = Range;
				s.FireDamage = FireDamage;
				s.PhysicalDamage = PhysicalDamage;
			}

		}
		/*
		public override Item Dupe(Item item, int amount)
		{
			BaseSiegeProjectile s = item as BaseSiegeProjectile;
			// dupe the siege projectile props
			if (s != null)
			{
				s.FiringSpeed = FiringSpeed;
				s.AccuracyBonus = AccuracyBonus;
				s.Area = Area;
				s.Range = Range;
				s.FireDamage = FireDamage;
				s.PhysicalDamage = PhysicalDamage;
			}
			// dupe the regular item props
			return base.Dupe(item, amount);
		}
		*/
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1061169, Range.ToString()); // range ~1_val~
			list.Add(1060658, "Speed\t{0}", FiringSpeed.ToString()); // ~1_val~: ~2_val~
			list.Add(1060659, "Accuracy bonus\t{0}", AccuracyBonus.ToString()); // ~1_val~: ~2_val~
			list.Add(1060660, "Area\t{0}", Area.ToString()); // ~1_val~: ~2_val~
			list.Add(1060661, "Physical damage\t{0}", PhysicalDamage.ToString()); // ~1_val~: ~2_val~
			list.Add(1060662, "Fire damage\t{0}", FireDamage.ToString()); // ~1_val~: ~2_val~

		}

		public override void OnDoubleClick(Mobile from)
		{

			// check the range between the player and projectiles
			if ((Parent == null && !from.InRange(Location, 2)) ||
				(RootParent is Mobile && !from.InRange(((Mobile)RootParent).Location, 2)) ||
				(RootParent is Container && !from.InRange(((Container)RootParent).Location, 2))
				)
			{
				from.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			from.Target = new SiegeWeaponTarget(this);

		}

		public void OnHit(Mobile from, ISiegeWeapon weapon, IEntity target, Point3D targetloc)
		{
			if (weapon == null || from == null) return;

			// play explosion sound at target

			Effects.PlaySound(targetloc, weapon.Map, 0x11D);

			ArrayList damagelist = new ArrayList();

			// deal with the fact that for multis, the targetloc and the actual multi location may differ
			// so deal the multi damage first
			if (target is BaseMulti)
			{
				XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

				if (a != null)
				{
					damagelist.Add(a);
				}
			}

			// apply splash damage to objects with a siege attachment
			IPooledEnumerable itemlist = from.Map.GetItemsInRange(targetloc, Area);

			if (itemlist != null)
			{
				foreach (Item item in itemlist)
				{
					if (item == null || item.Deleted) continue;

					XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(item, typeof(XmlSiege));

					if (a != null && !damagelist.Contains(a))
					{
						damagelist.Add(a);
					}
					else
						// if it had no siege attachment and the item is an addoncomponent, then check the parent addon
						if (item is AddonComponent)
						{
							a = (XmlSiege)XmlAttach.FindAttachment(((AddonComponent)item).Addon, typeof(XmlSiege));

							if (a != null && !damagelist.Contains(a))
							{
								damagelist.Add(a);
							}
						}
				}
			}

			int scaledfiredamage = (int)(FireDamage * StructureDamageMultiplier * weapon.WeaponDamageFactor);
			int scaledphysicaldamage = (int)(PhysicalDamage * StructureDamageMultiplier * weapon.WeaponDamageFactor);

			foreach (XmlSiege a in damagelist)
			{
				// apply siege damage
				a.ApplyScaledDamage(from, scaledfiredamage, scaledphysicaldamage);
			}

			// apply splash damage to mobiles
			ArrayList mobdamage = new ArrayList();

			IPooledEnumerable moblist = from.Map.GetMobilesInRange(targetloc, Area);
			if (moblist != null)
			{
				foreach (Mobile m in moblist)
				{
					if (m == null || m.Deleted || !from.CanBeHarmful(m, false)) continue;

					mobdamage.Add(m);
				}
			}

			int totaldamage = FireDamage + PhysicalDamage;
			if (totaldamage > 0)
			{
				int scaledmobdamage = (int)(totaldamage * MobDamageMultiplier * weapon.WeaponDamageFactor);
				int phys = 100 * PhysicalDamage / totaldamage;
				int fire = 100 * FireDamage / totaldamage;
				foreach (Mobile m in mobdamage)
				{
					// AOS.Damage( Mobile m, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy )
					AOS.Damage(m, from, scaledmobdamage, phys, fire, 0, 0, 0);
				}
			}

			// consume the ammunition
			Consume(1);
			weapon.Projectile = this;
		}

		private class SiegeWeaponTarget : Target
		{
			private BaseSiegeProjectile m_projectile;

			public SiegeWeaponTarget(BaseSiegeProjectile projectile)
				: base(2, true, TargetFlags.None)
			{
				m_projectile = projectile;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (from == null || m_projectile == null || from.Map == null) return;

				ISiegeWeapon weapon = null;

				if (targeted is ISiegeWeapon)
				{
					// load the cannon
					weapon = (ISiegeWeapon)targeted;
				}
				else
					if (targeted is SiegeComponent)
					{
						weapon = ((SiegeComponent)targeted).Addon as ISiegeWeapon;
					}

				if (weapon == null || weapon.Map == null)
				{
					from.SendMessage("Invalid target");
					return;
				}

				// load the cannon
				weapon.LoadWeapon(from, m_projectile); ;


			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			// version 0
			writer.Write(m_Range);
			writer.Write(m_AccuracyBonus);
			writer.Write(m_Area);
			writer.Write(m_FireDamage);
			writer.Write(m_PhysicalDamage);
			writer.Write(m_FiringSpeed);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					m_Range = reader.ReadInt();
					m_AccuracyBonus = reader.ReadInt();
					m_Area = reader.ReadInt();
					m_FireDamage = reader.ReadInt();
					m_PhysicalDamage = reader.ReadInt();
					m_FiringSpeed = reader.ReadInt();
					break;
			}
		}
	}
}
