using System;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{
    public class SiegeRam : BaseSiegeWeapon
    {
        // facing 0
        public static int[] CatapultWest = new int[] { 925, 925, 925, 1, 1, 5823, 5822, 5821, 5820, 5819, 5818, 5817, 5826, 5831, 5841, 5836 };
        public static int[] CatapultWestXOffset = new int[] { 2, 1, 0, -1, -2, -2, 0, 0, 1, 2, 2, 2, 1, 1, -1, -1 };
        public static int[] CatapultWestYOffset = new int[] { 0, 0, 0, 0, 0, -1, -1, 1, 0, -1, 0, 1, 1, -1, -1, 1 };
        public static int[] CatapultWestZOffset = new int[] { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] CatapultWestLaunch = new int[] { 925, 925 };
        // facing 1
        public static int[] CatapultNorth = new int[] { 926, 926, 926, 1, 1, 5784, 5786, 5789, 5785, 5783, 5782, 5780, 5781, 5799, 5794, 5808 };
        public static int[] CatapultNorthXOffset = new int[] { 0, 0, 0, 0, 0, 1, -1, 1, -1, 0, -1, 1, 0, 1, -1, -1 };
        public static int[] CatapultNorthYOffset = new int[] { 1, 0, -1, -2, -3, -1, -3, 0, -1, 0, 1, 1, 1, -2, 0, -2 };
        public static int[] CatapultNorthZOffset = new int[] { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] CatapultNorthLaunch = new int[] { 926, 926 };
        // facing 2
        public static int[] CatapultEast = new int[] { 925, 925, 925, 1, 1, 5763, 5758, 5746, 5747, 5748, 5750, 5751, 5752, 5753, 5768, 5749 };
        public static int[] CatapultEastXOffset = new int[] { -1, 0, 1, 2, 3, -1, -1, 1, 0, 0, -2, -2, -1, 1, 1, -2 };
        public static int[] CatapultEastYOffset = new int[] { 0, 0, 0, 0, 0, -1, 1, 0, 1, -1, 0, 1, 0, 1, -1, -1 };
        public static int[] CatapultEastZOffset = new int[] { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] CatapultEastLaunch = new int[] { 925, 925 };
        // facing 3
        public static int[] CatapultSouth = new int[] { 926, 926, 926, 1, 1, 5704, 5716, 5721, 5710, 5709, 5708, 5707, 5714, 5706, 5730, 5705 };
        public static int[] CatapultSouthXOffset = new int[] { 0, 0, 0, 0, 0, 0, 1, -1, 1, 0, -1, -1, 1, 1, -1, 0 };
        public static int[] CatapultSouthYOffset = new int[] { -1, 0, 1, 2, 3, -1, -1, -1, 0, -2, -2, 0, 1, -2, 1, 1 };
        public static int[] CatapultSouthZOffset = new int[] { 6, 6, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        public static int[] CatapultSouthLaunch = new int[] { 926, 926 };
        private readonly Type[] m_allowedprojectiles = new Type[] { typeof(SiegeLog) };
        private InternalTimer m_Timer;
        [Constructable]
        public SiegeRam()
            : this(0)
        {
        }

        [Constructable]
        public SiegeRam(int facing)
        {
            // addon the components
            for (int i = 0; i < CatapultNorth.Length; i++)
            {
                this.AddComponent(new SiegeComponent(0, this.Name), 0, 0, 0);
            }

            // assign the facing
            if (facing < 0)
                facing = 3;
            if (facing > 3)
                facing = 0;
            this.Facing = facing;

            // set the default props
            this.Name = "Siege Ram";
            this.Weight = 50;

            // make them siegable by default
            // XmlSiege( hitsmax, resistfire, resistphysical, wood, iron, stone)
            XmlAttach.AttachTo(this, new XmlSiege(100, 10, 10, 20, 30, 0));

            // and draggable
            XmlAttach.AttachTo(this, new XmlDrag());

            // undo the temporary hue indicator that is set when the xmlsiege attachment is added
            this.Hue = 0;
        }

        public SiegeRam(Serial serial)
            : base(serial)
        {
        }

        public override double WeaponLoadingDelay
        {
            get
            {
                return 10;
            }
        }// base delay for loading this weapon
        public override double WeaponStorageDelay
        {
            get
            {
                return 15.0;
            }
        }// base delay for packing away this weapon
        public override double WeaponRangeFactor
        {
            get
            {
                return 1.0;
            }
        }//  range multiplier for the weapon
        public override int MinTargetRange
        {
            get
            {
                return 2;
            }
        }// target must be further away than this
        public override int MinStorageRange
        {
            get
            {
                return 2;
            }
        }// player must be at least this close to store the weapon
        public override int MinFiringRange
        {
            get
            {
                return 3;
            }
        }// player must be at least this close to fire the weapon
        public override bool CheckLOS
        {
            get
            {
                return false;
            }
        }// whether the weapon needs to consider line of sight when selecting a target
        public override Type[] AllowedProjectiles
        {
            get
            {
                return this.m_allowedprojectiles;
            }
        }
        public override Point3D ProjectileLaunchPoint
        {
            get
            {
                if (this.Components != null && this.Components.Count > 0)
                {
                    switch (this.Facing)
                    {
                        case 0:
                            return new Point3D(CatapultWestXOffset[0] + this.Location.X - 2, CatapultWestYOffset[0] + this.Location.Y - 1, this.Location.Z + 5);
                        case 1:
                            return new Point3D(CatapultNorthXOffset[0] + this.Location.X - 1, CatapultNorthYOffset[0] + this.Location.Y - 1, this.Location.Z + 5);
                        case 2:
                            return new Point3D(CatapultEastXOffset[0] + this.Location.X - 2, CatapultEastYOffset[0] + this.Location.Y - 1, this.Location.Z + 5);
                        case 3:
                            return new Point3D(CatapultSouthXOffset[0] + this.Location.X, CatapultSouthYOffset[0] + this.Location.Y - 1, this.Location.Z + 5);
                    }
                }

                return (this.Location);
            }
        }
        public void DoTimer(Mobile from, SiegeRam weapon, IEntity target, Point3D targetloc, Item projectile, TimeSpan damagedelay, int step)
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            if (step > 4 || step < 0)
                return;

            this.m_Timer = new InternalTimer(from, weapon, target, targetloc, projectile, damagedelay, step);
            this.m_Timer.Start();
        }

        public override void LoadWeapon(Mobile from, Item projectile)
        {
            if (projectile == null)
                return;

            // restrict allowed projectiles
            if (!this.CheckAllowedProjectile(projectile))
            {
                from.SendMessage("That cannot be loaded into this weapon");
                return;
            }

            if (this.Projectile != null && !this.Projectile.Deleted)
            {
                from.SendMessage("{0} unloaded", this.Projectile.Name);
                from.AddToBackpack(this.Projectile);
            }

            // allow stacked projectiles to be loaded
            this.Projectile = projectile;

            if (this.Projectile != null)
            {
                this.Projectile.Internalize();

                from.SendMessage("{0} loaded", this.Projectile.Name);
            }
        }

        public override void LaunchProjectile(Mobile from, Item projectile, IEntity target, Point3D targetloc, TimeSpan delay)
        {
            // launch animation and delayed projectile release
            this.DoTimer(from, this, target, targetloc, projectile, delay, 0);
            // play the launch sound
            Effects.PlaySound(this, this.Map, 0x531);
        }

        public override void UpdateDisplay()
        {
            if (this.Components != null && this.Components.Count > 2)
            {
                int[] itemid = null;
                int[] xoffset = null;
                int[] yoffset = null;
                int[] zoffset = null;

                switch (this.Facing)
                {
                    case 0: // West
                        itemid = CatapultWest;
                        xoffset = CatapultWestXOffset;
                        yoffset = CatapultWestYOffset;
                        zoffset = CatapultWestZOffset;
                        break;
                    case 1: // North
                        itemid = CatapultNorth;
                        xoffset = CatapultNorthXOffset;
                        yoffset = CatapultNorthYOffset;
                        zoffset = CatapultNorthZOffset;
                        break;
                    case 2: // East
                        itemid = CatapultEast;
                        xoffset = CatapultEastXOffset;
                        yoffset = CatapultEastYOffset;
                        zoffset = CatapultEastZOffset;
                        break;
                    case 3: // South
                        itemid = CatapultSouth;
                        xoffset = CatapultSouthXOffset;
                        yoffset = CatapultSouthYOffset;
                        zoffset = CatapultSouthZOffset;
                        break;
                }

                if (itemid != null && xoffset != null && yoffset != null && zoffset != null && this.Components.Count == itemid.Length)
                {
                    for (int i = 0; i < this.Components.Count; i++)
                    {
                        ((AddonComponent)this.Components[i]).ItemID = itemid[i];
                        Point3D newoffset = new Point3D(xoffset[i], yoffset[i], zoffset[i]);
                        ((AddonComponent)this.Components[i]).Offset = newoffset;
                        ((AddonComponent)this.Components[i]).Location = new Point3D(newoffset.X + this.X, newoffset.Y + this.Y, newoffset.Z + this.Z);
                    }
                }
            }
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

        private void DisplayLaunch(int frame)
        {
            int[] LaunchIDArray = null;

            switch (this.Facing)
            {
                case 0:
                    LaunchIDArray = CatapultWestLaunch;
                    break;
                case 1:
                    LaunchIDArray = CatapultNorthLaunch;
                    break;
                case 2:
                    LaunchIDArray = CatapultEastLaunch;
                    break;
                case 3:
                    LaunchIDArray = CatapultSouthLaunch;
                    break;
            }

            if (LaunchIDArray != null && this.Components != null && LaunchIDArray.Length > 0)
            {
                switch (frame)
                {
                    case 0:
                        ((AddonComponent)this.Components[0]).ItemID = LaunchIDArray[0];
                        ((AddonComponent)this.Components[1]).ItemID = LaunchIDArray[0];
                        ((AddonComponent)this.Components[3]).ItemID = 1;
                        ((AddonComponent)this.Components[4]).ItemID = 1;
                        break;
                    case 1:
                        ((AddonComponent)this.Components[0]).ItemID = 1;
                        ((AddonComponent)this.Components[1]).ItemID = LaunchIDArray[0];
                        ((AddonComponent)this.Components[3]).ItemID = LaunchIDArray[0];
                        ((AddonComponent)this.Components[4]).ItemID = 1;
                        break;
                    case 2:
                        ((AddonComponent)this.Components[0]).ItemID = 1;
                        ((AddonComponent)this.Components[1]).ItemID = 1;
                        ((AddonComponent)this.Components[3]).ItemID = LaunchIDArray[0];
                        ((AddonComponent)this.Components[4]).ItemID = LaunchIDArray[0];
                        break;
                }
            }
        }

        // animation timer that begins on firing
        private class InternalTimer : Timer
        {
            private readonly SiegeRam m_weapon;
            private readonly Item m_Projectile;
            private readonly Point3D m_targetloc;
            private readonly IEntity m_target;
            private readonly Mobile m_from;
            private readonly TimeSpan m_damagedelay;
            private int m_step;
            public InternalTimer(Mobile from, SiegeRam weapon, IEntity target, Point3D targetloc, Item projectile, TimeSpan damagedelay, int step)
                : base(TimeSpan.FromMilliseconds(150))
            {
                this.Priority = TimerPriority.FiftyMS;
                this.m_weapon = weapon;
                this.m_Projectile = projectile;
                this.m_target = target;
                this.m_targetloc = targetloc;
                this.m_from = from;
                this.m_step = step;
                this.m_damagedelay = damagedelay;
            }

            protected override void OnTick()
            {
                ISiegeProjectile pitem = this.m_Projectile as ISiegeProjectile;

                if (this.m_weapon != null && !this.m_weapon.Deleted && pitem != null)
                {
                    int animationid = pitem.AnimationID;
                    int animationhue = pitem.AnimationHue;

                    switch (this.m_step)
                    {
                        case 0:
                        case 4:
                            this.m_weapon.DisplayLaunch(0);
                            break;
                        case 1:
                        case 3:
                            this.m_weapon.DisplayLaunch(1);
                            break;
                        case 2:
                            this.m_weapon.DisplayLaunch(2);
                            // launch sounds
                            Effects.PlaySound(this.m_weapon, this.m_weapon.Map, 0x4C9);
                            Effects.PlaySound(this.m_weapon, this.m_weapon.Map, 0x2B2);

                            // show the projectile moving to the target
                            /*
                            if (m_target is Mobile)
                            {
                            XmlSiege.SendMovingProjectileEffect(m_weapon, null, animationid, m_weapon.ProjectileLaunchPoint, m_targetloc, 7, 0, false, true, animationhue);
                            }
                            else
                            {
                            XmlSiege.SendMovingProjectileEffect(m_weapon, m_target, animationid, m_weapon.ProjectileLaunchPoint, m_targetloc, 7, 0, false, true, animationhue);
                            }
                            * */
                            // delayed damage at the target to account for travel distance of the projectile
                            Timer.DelayCall(this.m_damagedelay, new TimerStateCallback(this.m_weapon.DamageTarget_Callback),
                                new object[] { this.m_from, this.m_weapon, this.m_target, this.m_targetloc, this.m_Projectile });
                            break;
                    }

                    // advance to the next step
                    this.m_weapon.DoTimer(this.m_from, this.m_weapon, this.m_target, this.m_targetloc, this.m_Projectile, this.m_damagedelay, ++this.m_step);
                }
            }
        }
    }
}