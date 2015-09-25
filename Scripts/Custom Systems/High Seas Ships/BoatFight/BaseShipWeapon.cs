using System;
using Server.Engines.XmlSpawner2;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public abstract class BaseShipWeapon : BaseGalleonItem, IShipWeapon
    {
        public bool Storing = false;
        private int _facing;
        private Item _projectile;
        private DateTime _nextFiringTime;
        private XmlBoatFight _siegeAttachment = null;
        public BaseShipWeapon(BaseGalleon galleon, int northItemID, Point3D initOffset)
			:base(galleon, northItemID, initOffset)
        {
			IsDraggable = true;
			IsPackable = true;
			FixedFacing = false;
        }

        public BaseShipWeapon(Serial serial)
            : base(serial)
        {
        }
		
		[CommandProperty(AccessLevel.GameMaster, true)]
        public override bool ShareHue
        {
            get { return false; }
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
        /*public override BaseAddonDeed Deed
        *{
        *    get
        *    {
        *        return null;
        *    }
        *}
		*/
        public abstract Type[] AllowedProjectiles { get; }
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
            get
            {
                return _projectile;
            }
            set
            {
                _projectile = value;
                // invalidate component properties
                /*if (this.Components != null)
                *{
                *    foreach (AddonComponent c in this.Components)
                *    {
                *        c.InvalidateProperties();
                *    }
                *}
				*/
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsDraggable { get; set; }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool IsPackable { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool FixedFacing { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Facing
        {
            get
            {
                return _facing;
            }
            set
            {
                _facing = value;
                if (_facing < 0)
                    _facing = 3;
                if (_facing > 3)
                    _facing = 0;
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
            get
            {
                return _nextFiringTime - DateTime.Now;
            }
            set
            {
                _nextFiringTime = DateTime.Now + value;
            }
        }
		
        public virtual Point3D ProjectileLaunchPoint
        {
            get
            {
                return (Location);
            }
        }
		
        private XmlBoatFight SiegeAttachment
        {
            get
            {
                if (_siegeAttachment == null)
                {
                    _siegeAttachment = (XmlBoatFight)XmlAttach.FindAttachment(this, typeof(XmlBoatFight));
                }
                return _siegeAttachment;
            }
        }
		
        public abstract void UpdateDisplay();

        public override void OnDelete()
        {
            base.OnDelete();

            if (_projectile != null)
            {
                _projectile.Delete();
            }
        }

        public virtual void StoreWeapon_Callback(object state)
        {
            object[] args = (object[])state;

            Mobile from = (Mobile)args[0];
            BaseShipWeapon weapon = (BaseShipWeapon)args[1];

            if (weapon == null || weapon.Deleted || from == null)
                return;

            // make sure that there is only one person nearby
            IPooledEnumerable moblist = from.Map.GetMobilesInRange(weapon.Location, MinStorageRange);
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
            /*foreach (AddonComponent i in weapon.Components)
            *{
            *    if (i != null)
            *    {
            *        i.Internalize();
            *    }
            *}
			*/
			
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
            if (from == null)
                return;

            if (!from.InRange(Location, 2) || from.Map != Map)
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

        public bool CheckAllowedProjectile(Item projectile)
        {
            if (projectile == null || AllowedProjectiles == null)
                return false;

            for (int i = 0; i < AllowedProjectiles.Length; i++)
            {
                Type t = AllowedProjectiles[i];
                Type pt = projectile.GetType();

                if (t == null || pt == null)
                    continue;

                if (pt.IsSubclassOf(t) || pt.Equals(t) || (t.IsInterface && ContainsInterface(pt.GetInterfaces(), t)))
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
            if (Hits == 0)
                return;

            // restrict allowed projectiles
            if (!CheckAllowedProjectile(projectile))
            {
                from.SendMessage("That cannot be loaded into this weapon");
                return;
            }

            if (_projectile != null && !_projectile.Deleted)
            {
                from.SendMessage("{0} unloaded", _projectile.Name);
                from.AddToBackpack(_projectile);
            }

            if (projectile.Amount > 1)
            {
                //projectile.Amount--;
                //Projectile = projectile.Dupe(1);
                Projectile = Mobile.LiftItemDupe(projectile, projectile.Amount - 1);
            }
            else
            {
                Projectile = projectile;
            }

            if (_projectile != null)
            {
                _projectile.Internalize();

                from.SendMessage("{0} loaded", _projectile.Name);
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

        public virtual bool AttackTarget(Mobile from, IEntity target, Point3D targetloc, bool checkLOS)
        {
            IShipProjectile projectile = _projectile as IShipProjectile;

            if (from == null || from.Map == null || projectile == null)
                return false;

            if (!HasFiringAngle(targetloc))
            {
                from.SendMessage("No firing angle");
                return false;
            }

            // check the target range
            int distance = (int)XmlBoatFight.GetDistance(targetloc, Location);

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

            _nextFiringTime = DateTime.Now + TimeSpan.FromSeconds(loadingdelay);

            // calculate the accuracy based on distance and weapon skill
            int accuracy = distance * 10 - weaponskill + accuracybonus;

            if (Utility.Random(100) < accuracy)
            {
                from.SendMessage("Target missed");
                // consume the ammunition
                _projectile.Consume(1);
                // update the properties display
                Projectile = _projectile;
                return true;
            }

            LaunchProjectile(from, _projectile, target, targetloc, TimeSpan.FromSeconds((double)distance * 0.08));

            return true;
        }

        public virtual void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            IShipProjectile pitem = projectile as IShipProjectile;

            if (pitem == null)
                return;

            int animationid = pitem.AnimationID;
            int animationhue = pitem.AnimationHue;
			
			Effects.PlaySound(target, from.Map, Utility.RandomList(0x11B, 0x11C, 0x11D));

            // show the projectile moving to the target
            XmlBoatFight.SendMovingProjectileEffect(this, target, animationid, ProjectileLaunchPoint, targetloc, 7, 0, false, true, animationhue);

            // delayed damage at the target to account for travel distance of the projectile
            Timer.DelayCall(delay, new TimerStateCallback(DamageTarget_Callback),
                new object[] { from, this, target, targetloc, projectile });

            return;
        }

        public virtual void DamageTarget_Callback(object state)
        {
            object[] args = (object[])state;

            Mobile from = (Mobile)args[0];
            BaseShipWeapon weapon = (BaseShipWeapon)args[1];
            IEntity target = (IEntity)args[2];
            Point3D targetloc = (Point3D)args[3];
            Item pitem = (Item)args[4];

            IShipProjectile projectile = pitem as IShipProjectile;

            if (projectile != null)
            {
                projectile.OnHit(from, weapon, target, targetloc);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent != null)
                return;

            // can't use destroyed weapons
            if (Hits == 0)
                return;

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

            if (_projectile == null || _projectile.Deleted)
            {
                from.SendMessage("{0} empty", Name);
                return;
            }

            // check if the cannon is cool enough to fire
            if (_nextFiringTime > DateTime.Now)
            {
                from.SendMessage("Not ready yet.");
                return;
            }

            from.Target = new ShipTarget(this, from, CheckLOS);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
            // version 1
            writer.Write(FixedFacing);
            writer.Write(IsDraggable);
            writer.Write(IsPackable);
            // version 0
            writer.Write(_facing);
            writer.Write(_projectile);
            writer.Write(_nextFiringTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    FixedFacing = reader.ReadBool();
                    IsDraggable = reader.ReadBool();
                    IsPackable = reader.ReadBool();
                    goto case 0;
                case 0:
                    _facing = reader.ReadInt();
                    _projectile = reader.ReadItem();
                    _nextFiringTime = reader.ReadDateTime();
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

        private class ShipTarget : Target
        {
            private readonly BaseShipWeapon _weapon;
            private readonly Mobile _from;
            private readonly bool _checklos;
            public ShipTarget(BaseShipWeapon weapon, Mobile from, bool checklos)
                : base(30, true, TargetFlags.None)
            {
                _weapon = weapon;
                _from = from;
                _checklos = checklos;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (from == null || _weapon == null || from.Map == null)
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
                        _weapon.AttackTarget(from, multiitem, multiitem.Map.GetPoint(targeted, true), _checklos);
                    }
                }
                else if (targeted is IEntity)
                {
                    // attack the target
                    _weapon.AttackTarget(from, (IEntity)targeted, ((IEntity)targeted).Location, _checklos);
                }
                else if (targeted is LandTarget)
                {
                    // attack the target
                    _weapon.AttackTarget(from, null, ((LandTarget)targeted).Location, _checklos);
                }
            }
        }		
    }
}