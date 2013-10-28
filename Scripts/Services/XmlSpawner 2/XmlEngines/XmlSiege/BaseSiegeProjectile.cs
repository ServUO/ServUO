using System;
using System.Collections;
using Server.Engines.XmlSpawner2;
using Server.Targeting;

namespace Server.Items
{
    public class BaseSiegeProjectile : Item, ISiegeProjectile
    {
        private int m_Range;// max number of tiles it can travel
        private int m_AccuracyBonus;// adjustment to accuracy
        private int m_FiringSpeed;// adjustment to time until next shot in seconds*10
        private int m_Area;// radius of area damage
        private int m_FireDamage;// amount of fire damage to the target
        private int m_PhysicalDamage;// amount of physical damage to the target
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
            this.Weight = 5;
            this.Stackable = true;
            this.Amount = amount;
        }

        public BaseSiegeProjectile(Serial serial)
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
                return this.m_Range;
            }
            set
            {
                this.m_Range = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int FiringSpeed
        {
            get
            {
                return this.m_FiringSpeed;
            }
            set
            {
                this.m_FiringSpeed = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int AccuracyBonus
        {
            get
            {
                return this.m_AccuracyBonus;
            }
            set
            {
                this.m_AccuracyBonus = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Area
        {
            get
            {
                return this.m_Area;
            }
            set
            {
                this.m_Area = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int FireDamage
        {
            get
            {
                return this.m_FireDamage;
            }
            set
            {
                this.m_FireDamage = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int PhysicalDamage
        {
            get
            {
                return this.m_PhysicalDamage;
            }
            set
            {
                this.m_PhysicalDamage = value;
                this.InvalidateProperties();
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

            BaseSiegeProjectile s = newItem as BaseSiegeProjectile;
            // dupe the siege projectile props
            if (s != null)
            {
                s.FiringSpeed = this.FiringSpeed;
                s.AccuracyBonus = this.AccuracyBonus;
                s.Area = this.Area;
                s.Range = this.Range;
                s.FireDamage = this.FireDamage;
                s.PhysicalDamage = this.PhysicalDamage;
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

            list.Add(1061169, this.Range.ToString()); // range ~1_val~
            list.Add(1060658, "Speed\t{0}", this.FiringSpeed.ToString()); // ~1_val~: ~2_val~
            list.Add(1060659, "Accuracy bonus\t{0}", this.AccuracyBonus.ToString()); // ~1_val~: ~2_val~
            list.Add(1060660, "Area\t{0}", this.Area.ToString()); // ~1_val~: ~2_val~
            list.Add(1060661, "Physical damage\t{0}", this.PhysicalDamage.ToString()); // ~1_val~: ~2_val~
            list.Add(1060662, "Fire damage\t{0}", this.FireDamage.ToString()); // ~1_val~: ~2_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            // check the range between the player and projectiles
            if ((this.Parent == null && !from.InRange(this.Location, 2)) ||
                (this.RootParent is Mobile && !from.InRange(((Mobile)this.RootParent).Location, 2)) ||
                (this.RootParent is Container && !from.InRange(((Container)this.RootParent).Location, 2))
            )
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            from.Target = new SiegeWeaponTarget(this);
        }

        public void OnHit(Mobile from, ISiegeWeapon weapon, IEntity target, Point3D targetloc)
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
                XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(target, typeof(XmlSiege));

                if (a != null)
                {
                    damagelist.Add(a);
                }
            }

            // apply splash damage to objects with a siege attachment
            IPooledEnumerable itemlist = from.Map.GetItemsInRange(targetloc, this.Area);

            if (itemlist != null)
            {
                foreach (Item item in itemlist)
                {
                    if (item == null || item.Deleted)
                        continue;

                    XmlSiege a = (XmlSiege)XmlAttach.FindAttachment(item, typeof(XmlSiege));

                    if (a != null && !damagelist.Contains(a))
                    {
                        damagelist.Add(a);
                    }
                    else if (item is AddonComponent)
                    {
                        a = (XmlSiege)XmlAttach.FindAttachment(((AddonComponent)item).Addon, typeof(XmlSiege));

                        if (a != null && !damagelist.Contains(a))
                        {
                            damagelist.Add(a);
                        }
                    }
                }
            }

            int scaledfiredamage = (int)(this.FireDamage * this.StructureDamageMultiplier * weapon.WeaponDamageFactor);
            int scaledphysicaldamage = (int)(this.PhysicalDamage * this.StructureDamageMultiplier * weapon.WeaponDamageFactor);

            foreach (XmlSiege a in damagelist)
            {
                // apply siege damage
                a.ApplyScaledDamage(from, scaledfiredamage, scaledphysicaldamage);
            }

            // apply splash damage to mobiles
            ArrayList mobdamage = new ArrayList();

            IPooledEnumerable moblist = from.Map.GetMobilesInRange(targetloc, this.Area);
            if (moblist != null)
            {
                foreach (Mobile m in moblist)
                {
                    if (m == null || m.Deleted || !from.CanBeHarmful(m, false))
                        continue;

                    mobdamage.Add(m);
                }
            }

            int totaldamage = this.FireDamage + this.PhysicalDamage;
            if (totaldamage > 0)
            {
                int scaledmobdamage = (int)(totaldamage * this.MobDamageMultiplier * weapon.WeaponDamageFactor);
                int phys = 100 * this.PhysicalDamage / totaldamage;
                int fire = 100 * this.FireDamage / totaldamage;
                foreach (Mobile m in mobdamage)
                {
                    // AOS.Damage( Mobile m, Mobile from, int damage, int phys, int fire, int cold, int pois, int nrgy )
                    AOS.Damage(m, from, scaledmobdamage, phys, fire, 0, 0, 0);
                }
            }

            // consume the ammunition
            this.Consume(1);
            weapon.Projectile = this;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            // version 0
            writer.Write(this.m_Range);
            writer.Write(this.m_AccuracyBonus);
            writer.Write(this.m_Area);
            writer.Write(this.m_FireDamage);
            writer.Write(this.m_PhysicalDamage);
            writer.Write(this.m_FiringSpeed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    this.m_Range = reader.ReadInt();
                    this.m_AccuracyBonus = reader.ReadInt();
                    this.m_Area = reader.ReadInt();
                    this.m_FireDamage = reader.ReadInt();
                    this.m_PhysicalDamage = reader.ReadInt();
                    this.m_FiringSpeed = reader.ReadInt();
                    break;
            }
        }

        private class SiegeWeaponTarget : Target
        {
            private readonly BaseSiegeProjectile m_projectile;
            public SiegeWeaponTarget(BaseSiegeProjectile projectile)
                : base(2, true, TargetFlags.None)
            {
                this.m_projectile = projectile;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || this.m_projectile == null || from.Map == null)
                    return;

                ISiegeWeapon weapon = null;

                if (targeted is ISiegeWeapon)
                {
                    // load the cannon
                    weapon = (ISiegeWeapon)targeted;
                }
                else if (targeted is SiegeComponent)
                {
                    weapon = ((SiegeComponent)targeted).Addon as ISiegeWeapon;
                }

                if (weapon == null || weapon.Map == null)
                {
                    from.SendMessage("Invalid target");
                    return;
                }

                // load the cannon
                weapon.LoadWeapon(from, this.m_projectile); 
            }
        }
    }
}