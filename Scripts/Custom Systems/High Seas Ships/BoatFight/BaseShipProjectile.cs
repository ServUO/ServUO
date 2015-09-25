using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public class BaseShipProjectile : Item, IShipProjectile
    {
        private int _range;// max number of tiles it can travel
        private int _accuracyBonus;// adjustment to accuracy
        private int _firingSpeed;// adjustment to time until next shot in seconds*10
        private int _area;// radius of area damage
        private int _fireDamage;// amount of fire damage to the target
        private int _physicalDamage;// amount of physical damage to the target
        public BaseShipProjectile()
            : this(1, 0xE74)
        {
        }

        public BaseShipProjectile(int amount)
            : this(amount, 0xE74)
        {
        }

        public BaseShipProjectile(int amount, int itemid)
            : base(itemid)
        {
            Weight = 5;
            Stackable = true;
            Amount = amount;
        }

        public BaseShipProjectile(Serial serial)
            : base(serial)
        {
        }

        public virtual int AnimationID
        {
            get
            {
                return 0xE73;
            }
        }
        public virtual int AnimationHue
        {
            get
            {
                return 0;
            }
        }
        public virtual double MobDamageMultiplier
        {
            get
            {
                return 1.0;
            }
        }// default damage multiplier for creatures
        public virtual double StructureDamageMultiplier
        {
            get
            {
                return 1.0;
            }
        }// default damage multiplier for structures
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Range
        {
            get
            {
                return _range;
            }
            set
            {
                _range = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int FiringSpeed
        {
            get
            {
                return _firingSpeed;
            }
            set
            {
                _firingSpeed = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int AccuracyBonus
        {
            get
            {
                return _accuracyBonus;
            }
            set
            {
                _accuracyBonus = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int FireDamage
        {
            get
            {
                return _fireDamage;
            }
            set
            {
                _fireDamage = value;
                InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int PhysicalDamage
        {
            get
            {
                return _physicalDamage;
            }
            set
            {
                _physicalDamage = value;
                InvalidateProperties();
            }
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

            BaseShipProjectile s = newItem as BaseShipProjectile;
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

            from.Target = new ShipWeaponTarget(this);
        }

        public void OnHit(Mobile from, IShipWeapon weapon, IEntity target, Point3D targetloc)
        {
            if (weapon == null || from == null)
                return;

            // play explosion sound at target

            Effects.PlaySound(targetloc, weapon.Map, 0x11D);

            ArrayList damagelist = new ArrayList();

            // deal with the fact that for multis, the targetloc and the actual multi location may differ
            // so deal the multi damage first
            if (target is BaseMulti)
            {
                XmlBoatFight a = (XmlBoatFight)XmlAttach.FindAttachment(target, typeof(XmlBoatFight));

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
                    if (item == null || item.Deleted)
                        continue;

                    XmlBoatFight a = (XmlBoatFight)XmlAttach.FindAttachment(item, typeof(XmlBoatFight));

                    if (a != null && !damagelist.Contains(a))
                    {
                        damagelist.Add(a);
                    }
                    else if (item is AddonComponent)
                    {
                        a = (XmlBoatFight)XmlAttach.FindAttachment(((AddonComponent)item).Addon, typeof(XmlBoatFight));

                        if (a != null && !damagelist.Contains(a))
                        {
                            damagelist.Add(a);
                        }
                    }
                }
            }

            int scaledfiredamage = (int)(FireDamage * StructureDamageMultiplier * weapon.WeaponDamageFactor);
            int scaledphysicaldamage = (int)(PhysicalDamage * StructureDamageMultiplier * weapon.WeaponDamageFactor);

            foreach (XmlBoatFight a in damagelist)
            {				
                // apply siege damage
                a.ApplyScaledDamage(from, scaledfiredamage, scaledphysicaldamage);
				
				
				#region HS Ships
				int HitsAfterDamage = a.Hits;
				int HitsMax = a.HitsMax;
				
				if (target is BaseGalleon)
				{
					BaseGalleon targetedGalleon = (BaseGalleon)target;
					targetedGalleon.Durability = (ushort)(100 * HitsAfterDamage / HitsMax);
				}
				#endregion
            }

            // apply splash damage to mobiles
            ArrayList mobdamage = new ArrayList();

            IPooledEnumerable moblist = from.Map.GetMobilesInRange(targetloc, Area);
            if (moblist != null)
            {
                foreach (Mobile m in moblist)
                {
                    if (m == null || m.Deleted || !from.CanBeHarmful(m, false))
                        continue;

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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            // version 0
            writer.Write(_range);
            writer.Write(_accuracyBonus);
            writer.Write(_area);
            writer.Write(_fireDamage);
            writer.Write(_physicalDamage);
            writer.Write(_firingSpeed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    _range = reader.ReadInt();
                    _accuracyBonus = reader.ReadInt();
                    _area = reader.ReadInt();
                    _fireDamage = reader.ReadInt();
                    _physicalDamage = reader.ReadInt();
                    _firingSpeed = reader.ReadInt();
                    break;
            }
        }

        private class ShipWeaponTarget : Target
        {
            private readonly BaseShipProjectile m_projectile;
            public ShipWeaponTarget(BaseShipProjectile projectile)
                : base(2, true, TargetFlags.None)
            {
                m_projectile = projectile;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || m_projectile == null || from.Map == null)
                    return;

                IShipWeapon weapon = null;

                if (targeted is IShipWeapon)
                {
                    // load the cannon
                    weapon = (IShipWeapon)targeted;
                }
                else if (targeted is ShipComponent)
                {
                    weapon = ((ShipComponent)targeted).Addon as IShipWeapon;
                }

                if (weapon == null || weapon.Map == null)
                {
                    from.SendMessage("Invalid target");
                    return;
                }

                // load the cannon
                weapon.LoadWeapon(from, m_projectile); 
            }
        }
    }
}