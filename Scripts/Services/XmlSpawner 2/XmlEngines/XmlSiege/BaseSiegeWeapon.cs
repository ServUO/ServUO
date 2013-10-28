using System;
using Server.Engines.XmlSpawner2;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseSiegeWeapon : BaseAddon, ISiegeWeapon
    {
        public bool Storing = false;
        private int m_Facing;
        private Item m_Projectile;
        private DateTime m_NextFiringTime;
        private bool m_FixedFacing = false;
        private bool m_Draggable = true;
        private bool m_Packable = true;
        private XmlSiege m_SiegeAttachment = null;
        public BaseSiegeWeapon()
        {
        }

        public BaseSiegeWeapon(Serial serial)
            : base(serial)
        {
        }

        public virtual double WeaponLoadingDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for loading this weapon
        public virtual double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public virtual double DamageReductionWhenDamaged
        {
            get
            {
                return 0.4;
            }
        }// scale damage from 40-100% depending on the damage it has taken 
        public virtual double RangeReductionWhenDamaged
        {
            get
            {
                return 0.7;
            }
        }// scale range from 70-100% depending on the damage it has taken 
        public virtual int MinTargetRange
        {
            get
            {
                return 1;
            }
        }// target must be further away than this
        public virtual int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public virtual int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public virtual bool CheckLOS
        {
            get
            {
                return true;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public virtual int StoredWeaponID
        {
            get
            {
                return 3644;
            }
        }// itemid used when the weapon is packed up (crate by default)
        public override BaseAddonDeed Deed
        {
            get
            {
                return null;
            }
        }
        public abstract Type[] AllowedProjectiles { get; }
        public int Hits
        {
            get
            {
                return (this.SiegeAttachment != null) ? this.SiegeAttachment.Hits : 0;
            }
        }
        public int HitsMax
        {
            get
            {
                return (this.SiegeAttachment != null) ? this.SiegeAttachment.HitsMax : 0;
            }
        }
        // default weapon performance factors.
        // taking damage reduces the multiplier

        // default damage multiplier for the weapon
        public virtual double WeaponDamageFactor
        {
            get
            {
                if (this.HitsMax > 0)
                {
                    return ((1 - this.DamageReductionWhenDamaged) * this.Hits / this.HitsMax) + this.DamageReductionWhenDamaged;
                }
                return 1;
            }
        }
        // default range multiplier for the weapon
        public virtual double WeaponRangeFactor
        {
            get
            {
                if (this.HitsMax > 0)
                {
                    return ((1 - this.RangeReductionWhenDamaged) * this.Hits / this.HitsMax) + this.RangeReductionWhenDamaged;
                }
                return 1;
            }
        }
        public virtual Item Projectile
        {
            get
            {
                return this.m_Projectile;
            }
            set
            {
                this.m_Projectile = value;
                // invalidate component properties
                if (this.Components != null)
                {
                    foreach (AddonComponent c in this.Components)
                    {
                        c.InvalidateProperties();
                    }
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsDraggable
        {
            get
            {
                return this.m_Draggable;
            }
            set
            {
                this.m_Draggable = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsPackable
        {
            get
            {
                return this.m_Packable;
            }
            set
            {
                this.m_Packable = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool FixedFacing
        {
            get
            {
                return this.m_FixedFacing;
            }
            set
            {
                this.m_FixedFacing = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Facing
        {
            get
            {
                return this.m_Facing;
            }
            set
            {
                this.m_Facing = value;
                if (this.m_Facing < 0)
                    this.m_Facing = 3;
                if (this.m_Facing > 3)
                    this.m_Facing = 0;
                this.UpdateDisplay();
                // save the current state of the itemids
                if (this.SiegeAttachment != null)
                {
                    this.SiegeAttachment.StoreOriginalItemID(this);
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextFiring
        {
            get
            {
                return this.m_NextFiringTime - DateTime.UtcNow;
            }
            set
            {
                this.m_NextFiringTime = DateTime.UtcNow + value;
            }
        }
        public virtual Point3D ProjectileLaunchPoint
        {
            get
            {
                return (this.Location);
            }
        }
        private XmlSiege SiegeAttachment
        {
            get
            {
                if (this.m_SiegeAttachment == null)
                {
                    this.m_SiegeAttachment = (XmlSiege)XmlAttach.FindAttachment(this, typeof(XmlSiege));
                }
                return this.m_SiegeAttachment;
            }
        }
        public abstract void UpdateDisplay();

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Projectile != null)
            {
                this.m_Projectile.Delete();
            }
        }

        public virtual void StoreWeapon_Callback(object state)
        {
            object[] args = (object[])state;

            Mobile from = (Mobile)args[0];
            BaseSiegeWeapon weapon = (BaseSiegeWeapon)args[1];

            if (weapon == null || weapon.Deleted || from == null)
                return;

            // make sure that there is only one person nearby
            IPooledEnumerable moblist = from.Map.GetMobilesInRange(weapon.Location, this.MinStorageRange);
            int count = 0;
            if (moblist != null)
            {
                foreach (Mobile m in moblist)
                {
                    if (m.Player)
                        count++;
                }
            }
            if (count > 1)
            {
                from.SendMessage("Too many players nearby. Storage failed.");
                return;
            }

            // make sure that the player is still next to the weapon
            if (!from.InRange(weapon.Location, this.MinStorageRange) || from.Map != weapon.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                from.SendMessage("{0} not stored.", weapon.Name);
                return;
            }

            // use the crate itemid while stored
            weapon.ItemID = this.StoredWeaponID;
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
            this.MoveToWorld(location, map);
            this.UpdateDisplay();
        }

        public virtual void StoreWeapon(Mobile from)
        {
            if (from == null)
                return;

            if (!from.InRange(this.Location, 2) || from.Map != this.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            // 15 second delay to pack up the cannon
            Timer.DelayCall(TimeSpan.FromSeconds(this.WeaponStorageDelay), new TimerStateCallback(StoreWeapon_Callback),
                new object[] { from, this });

            from.SendMessage("Packing up the {0}...", this.Name);

            this.Storing = true;
        }

        public bool CheckAllowedProjectile(Item projectile)
        {
            if (projectile == null || this.AllowedProjectiles == null)
                return false;

            for (int i = 0; i < this.AllowedProjectiles.Length; i++)
            {
                Type t = this.AllowedProjectiles[i];
                Type pt = projectile.GetType();

                if (t == null || pt == null)
                    continue;

                if (pt.IsSubclassOf(t) || pt.Equals(t) || (t.IsInterface && this.ContainsInterface(pt.GetInterfaces(), t)))
                {
                    return true;
                }
            }

            return false;
        }

        public virtual void LoadWeapon(Mobile from, Item projectile)
        {
            if (projectile == null)
                return;

            // can't load destroyed weapons
            if (this.Hits == 0)
                return;

            // restrict allowed projectiles
            if (!this.CheckAllowedProjectile(projectile))
            {
                from.SendMessage("That cannot be loaded into this weapon");
                return;
            }

            if (this.m_Projectile != null && !this.m_Projectile.Deleted)
            {
                from.SendMessage("{0} unloaded", this.m_Projectile.Name);
                from.AddToBackpack(this.m_Projectile);
            }

            if (projectile.Amount > 1)
            {
                //projectile.Amount--;
                //Projectile = projectile.Dupe(1);
                this.Projectile = Mobile.LiftItemDupe(projectile, projectile.Amount - 1);
            }
            else
            {
                this.Projectile = projectile;
            }

            if (this.m_Projectile != null)
            {
                this.m_Projectile.Internalize();

                from.SendMessage("{0} loaded", this.m_Projectile.Name);
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D point)
        {
            bool dropped = base.OnDroppedToWorld(from, point);

            if (dropped)
            {
                this.ItemID = 1;
                this.Visible = false;
                this.Movable = false;
                this.UpdateDisplay();
            }
            return dropped;
        }

        public virtual bool HasFiringAngle(IPoint3D t)
        {
            int dy = t.Y - this.Y;
            int dx = t.X - this.X;

            switch (this.Facing)
            {
                case 0:
                    return t.X < this.X && ((dy <= 0 && -dy <= -dx) || (dy > 0 && dy <= -dx));
                case 1:
                    return t.Y < this.Y && ((dx <= 0 && -dx <= -dy) || (dx > 0 && dx <= -dy));
                case 2:
                    return t.X > this.X && ((dy <= 0 && -dy <= dx) || (dy > 0 && dy <= dx));
                case 3:
                    return t.Y > this.Y && ((dx <= 0 && -dx <= dy) || (dx > 0 && dx <= dy));
            }

            return false;
        }

        public virtual bool AttackTarget(Mobile from, IEntity target, Point3D targetloc, bool checkLOS)
        {
            ISiegeProjectile projectile = this.m_Projectile as ISiegeProjectile;

            if (from == null || from.Map == null || projectile == null)
                return false;

            if (!this.HasFiringAngle(targetloc))
            {
                from.SendMessage("No firing angle");
                return false;
            }

            // check the target range
            int distance = (int)XmlSiege.GetDistance(targetloc, this.Location);

            int projectilerange = (int)(projectile.Range * this.WeaponRangeFactor);

            if (projectilerange < distance)
            {
                from.SendMessage("Out of range");
                return false;
            }

            if (distance <= this.MinTargetRange)
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

            if (checkLOS && !this.Map.LineOfSight(this, adjustedloc))
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
            double loadingdelay = this.WeaponLoadingDelay - dexbonus - firingspeedbonus;

            this.m_NextFiringTime = DateTime.UtcNow + TimeSpan.FromSeconds(loadingdelay);

            // calculate the accuracy based on distance and weapon skill
            int accuracy = distance * 10 - weaponskill + accuracybonus;

            if (Utility.Random(100) < accuracy)
            {
                from.SendMessage("Target missed");
                // consume the ammunition
                this.m_Projectile.Consume(1);
                // update the properties display
                this.Projectile = this.m_Projectile;
                return true;
            }

            this.LaunchProjectile(from, this.m_Projectile, target, targetloc, TimeSpan.FromSeconds((double)distance * 0.08));

            return true;
        }

        public virtual void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            ISiegeProjectile pitem = projectile as ISiegeProjectile;

            if (pitem == null)
                return;

            int animationid = pitem.AnimationID;
            int animationhue = pitem.AnimationHue;

            // show the projectile moving to the target
            XmlSiege.SendMovingProjectileEffect(this, target, animationid, this.ProjectileLaunchPoint, targetloc, 7, 0, false, true, animationhue);

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
            if (this.Parent != null)
                return;

            // can't use destroyed weapons
            if (this.Hits == 0)
                return;

            // check the range between the player and weapon
            if (!from.InRange(this.Location, this.MinFiringRange) || from.Map != this.Map)
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (this.Storing)
            {
                from.SendMessage("{0} being stored", this.Name);
                return;
            }

            if (this.m_Projectile == null || this.m_Projectile.Deleted)
            {
                from.SendMessage("{0} empty", this.Name);
                return;
            }

            // check if the cannon is cool enough to fire
            if (this.m_NextFiringTime > DateTime.UtcNow)
            {
                from.SendMessage("Not ready yet.");
                return;
            }

            from.Target = new SiegeTarget(this, from, this.CheckLOS);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            // version 1
            writer.Write(this.m_FixedFacing);
            writer.Write(this.m_Draggable);
            writer.Write(this.m_Packable);
            // version 0
            writer.Write(this.m_Facing);
            writer.Write(this.m_Projectile);
            writer.Write(this.m_NextFiringTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    this.m_FixedFacing = reader.ReadBool();
                    this.m_Draggable = reader.ReadBool();
                    this.m_Packable = reader.ReadBool();
                    goto case 0;
                case 0:
                    this.m_Facing = reader.ReadInt();
                    this.m_Projectile = reader.ReadItem();
                    this.m_NextFiringTime = reader.ReadDateTime();
                    break;
            }
        }

        private bool ContainsInterface(Type[] typearray, Type type)
        {
            if (typearray == null || type == null)
                return false;

            foreach (Type t in typearray)
            {
                if (t == type)
                    return true;
            }

            return false;
        }

        private class SiegeTarget : Target
        {
            private readonly BaseSiegeWeapon m_weapon;
            private readonly Mobile m_from;
            private readonly bool m_checklos;
            public SiegeTarget(BaseSiegeWeapon weapon, Mobile from, bool checklos)
                : base(30, true, TargetFlags.None)
            {
                this.m_weapon = weapon;
                this.m_from = from;
                this.m_checklos = checklos;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || this.m_weapon == null || from.Map == null)
                    return;

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
                        this.m_weapon.AttackTarget(from, multiitem, multiitem.Map.GetPoint(targeted, true), this.m_checklos);
                    }
                }
                else if (targeted is IEntity)
                {
                    // attack the target
                    this.m_weapon.AttackTarget(from, (IEntity)targeted, ((IEntity)targeted).Location, this.m_checklos);
                }
                else if (targeted is LandTarget)
                {
                    // attack the target
                    this.m_weapon.AttackTarget(from, null, ((LandTarget)targeted).Location, this.m_checklos);
                }
            }
        }
    }
}