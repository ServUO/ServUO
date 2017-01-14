using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;

namespace Server.Engines.VvV
{
    public class CannonTurret : BaseAddon
    {
        public const int ScanRange = 8;
        public const int ReloadDelay = 10;

        private bool _NoShoot;
        private int _ShotsRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ShotsRemaining
        {
            get { return _ShotsRemaining; }
            set
            {
                _ShotsRemaining = value;

                if (_ShotsRemaining <= 0)
                    Delete();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CannonBase Base { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item Turret { get; set; }

        private DateTime _NextShot;

        public override BaseAddonDeed Deed { get { return null; } }

        [Constructable]
        public CannonTurret()
            : this(null)
        {
        }

        public CannonTurret(Mobile m)
        {
            Owner = m;

            ShotsRemaining = 20;

            Base = new CannonBase(this);
            Base.MoveToWorld(this.Location, this.Map);

            AddonComponent c = new LocalizedAddonComponent(16918, 1155505);
            AddComponent(c, 0, 0, 3);
            Turret = c;

            _NextShot = DateTime.UtcNow;
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            if (Base != null && !Base.Deleted)
                Base.Location = this.Location;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Base != null && !Base.Deleted)
                Base.Map = this.Map;
        }

        public override void Delete()
        {
            base.Delete();

            if (Base != null && !Base.Deleted)
                Base.Delete();
        }

        public void Scan()
        {
            if (Deleted || this.Map == null || _ShotsRemaining <= 0 || _NoShoot)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, ScanRange);
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (Owner == null || (ViceVsVirtueSystem.IsEnemy(Owner, m) && m.InLOS(this.Location)
                                                                           && m is PlayerMobile 
                                                                           && m.AccessLevel == AccessLevel.Player))
                {
                    list.Add(m);
                }
            }

            eable.Free();
            Mobile target = null;
            double closest = ScanRange;

            if (list.Count > 0)
            {
                foreach (Mobile m in list.Where(mob => target == null || mob.GetDistanceToSqrt(target) < closest))
                {
                    target = m;
                    closest = m.GetDistanceToSqrt(target);
                }
            }

            if (target != null)
            {
                AimAndShoot(target, (int)Math.Ceiling(closest));
            }

            list.Clear();
            list.TrimExcess();
        }

        public void AimAndShoot(Mobile target, int range)
        {
            Direction d = Utility.GetDirection(this, target);

            switch (d)
            {
                default:
                case Direction.North:
                case Direction.Right: Turret.ItemID = 16920; break;
                case Direction.East:
                case Direction.Down: Turret.ItemID = 16921; break;
                case Direction.South:
                case Direction.Left: Turret.ItemID = 16918; break;
                case Direction.West:
                case Direction.Up: Turret.ItemID = 16919; break;
            }

            if (_NextShot > DateTime.UtcNow)
                return;

            Timer.DelayCall(TimeSpan.FromMilliseconds(250), () =>
            {
                Point3D p = new Point3D(this.X, this.Y, this.Z + 2);
                Map map = this.Map;

                switch (Turret.ItemID)
                {
                    case 16920: p.Y--; break;
                    case 16921: p.X++; break;
                    case 16918: p.Y++; break;
                    case 16919: p.X--; break;
                }

                Effects.SendLocationEffect(p, map, 14120, 15, 10);
                Effects.PlaySound(p, map, 0x664);
            });

            Timer.DelayCall(TimeSpan.FromMilliseconds(250 + (150 * (range))), () =>
            {
                if (Owner != null)
                    Owner.DoHarmful(target);

                AOS.Damage(target, Owner, Utility.RandomMinMax(75, 100), 100, 0, 0, 0, 0);

                Effects.SendLocationEffect(target.Location, target.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
                Effects.PlaySound(target.Location, target.Map, 0x207);

                ShotsRemaining--;
            });

            _NextShot = DateTime.UtcNow + TimeSpan.FromSeconds(ReloadDelay);
        }

        public class CannonBase : DamageableItem
        {
            public override int LabelNumber { get { return 1155505; } }

            public CannonTurret Turret { get; set; }

            public CannonBase(CannonTurret turret)
                : base(1822, 1822)
            {
                Level = ItemLevel.Hard;
                Turret = turret;

                Name = "a cannon turret";
            }

            public override void Delete()
            {
                base.Delete();

                if (Turret != null)
                    Turret.Delete();
            }

            public CannonBase(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }

        public CannonTurret(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Owner);
            writer.Write(Base);
            writer.Write(Turret);
            writer.Write(ShotsRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Owner = reader.ReadMobile();
            Base = reader.ReadItem() as CannonBase;
            Turret = reader.ReadItem();
            ShotsRemaining = reader.ReadInt();

            _NoShoot = true;
            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
            {
                if (Base != null)
                {
                    if (ViceVsVirtueSystem.Instance.Battle.OnGoing)
                    {
                        Base.Turret = this;
                        ViceVsVirtueSystem.Instance.Battle.Turrets.Add(this);
                        _NoShoot = false;
                        return;
                    }
                }

                Delete();
            });
        }
    }

    public class CannonTurretPlans : Item
    {
        public override int LabelNumber { get { return 1155503; } } // Plans for a Cannon Turret

        [Constructable]
        public CannonTurretPlans() : base(5630)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                VvVBattle battle = ViceVsVirtueSystem.Instance.Battle;

                if (!ViceVsVirtueSystem.IsVvV(m))
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                }
                else if (battle == null || !battle.OnGoing || !battle.IsInActiveBattle(m))
                {
                    m.SendLocalizedMessage(1155406); // This item can only be used in an active VvV battle region!
                }
                else if (battle.TurretCount > VvVBattle.MaxTurrets)
                {
                    m.SendLocalizedMessage(1155502); // The turret limit for this battle has been reached!
                }
                else
                {
                    CannonTurret t = new CannonTurret(m);
                    t.MoveToWorld(m.Location, m.Map);

                    battle.Turrets.Add(t);

                    Delete();
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public CannonTurretPlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}