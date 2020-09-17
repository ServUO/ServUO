using Server.Mobiles;
using Server.Multis;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    public abstract class MannedCannon : Item
    {
        public virtual TimeSpan ScanDelay => TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Operator { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing => GetFacing();

        public DateTime NextScan { get; set; }
        public bool CanFireUnmanned { get; set; }

        public abstract CannonPower Power { get; }
        public abstract int Range { get; }

        public virtual AmmunitionType AmmoType => AmmunitionType.Cannonball;
        public virtual int LateralOffset => 1;

        public MannedCannon(Mobile opera, Direction facing)
            : base(0)
        {
            ItemID = GetID(facing, Power);
            Operator = opera;

            Movable = false;
        }

        private static int GetID(Direction facing, CannonPower power)
        {
            switch (facing)
            {
                default:
                case Direction.South:
                    return BaseGalleon.CannonIDs[0][(int)power];
                case Direction.West:
                    return BaseGalleon.CannonIDs[1][(int)power];
                case Direction.North:
                    return BaseGalleon.CannonIDs[2][(int)power];
                case Direction.East:
                    return BaseGalleon.CannonIDs[3][(int)power];
            }
        }

        public Direction GetFacing()
        {
            if (BaseGalleon.CannonIDs[0].Any(id => id == ItemID))
            {
                return Direction.South;
            }
            if (BaseGalleon.CannonIDs[1].Any(id => id == ItemID))
            {
                return Direction.West;
            }
            if (BaseGalleon.CannonIDs[2].Any(id => id == ItemID))
            {
                return Direction.North;
            }

            return Direction.East;
        }

        public bool Scan(bool shoot)
        {
            Target[] targets = AcquireTarget();
            bool acquiredTarget = false;

            if (targets != null && targets.Length > 0)
            {
                foreach (Target t in targets)
                {
                    if (t.Entity is BaseGalleon && AmmoType != AmmunitionType.Grapeshot)
                    {
                        if (shoot)
                        {
                            DoShootEffects();
                            TimeSpan delay = TimeSpan.FromSeconds(t.Range / 10.0);

                            Timer.DelayCall(delay, new TimerStateCallback(OnShipHit), new object[] { (BaseGalleon)t.Entity, t.Location, AmmoType });
                        }

                        acquiredTarget = true;
                    }
                    else if (t.Entity is Mobile && AmmoType == AmmunitionType.Grapeshot)
                    {
                        Mobile m = t.Entity as Mobile;

                        if (shoot)
                        {
                            DoShootEffects();
                            TimeSpan delay = TimeSpan.FromSeconds(t.Range / 10.0);

                            Timer.DelayCall(delay, new TimerStateCallback(OnMobileHit), new object[] { m, t.Location, AmmoType });
                        }

                        acquiredTarget = true;
                    }
                }
            }

            return acquiredTarget;
        }

        public Target[] AcquireTarget()
        {
            AmmoInfo ammo = AmmoInfo.GetAmmoInfo(AmmoType);

            int xOffset = 0; int yOffset = 0;
            int currentRange = 0;
            Point3D pnt = Location;
            Map map = Map;

            switch (GetFacing())
            {
                case Direction.North:
                    xOffset = 0; yOffset = -1; break;
                case Direction.South:
                    xOffset = 0; yOffset = 1; break;
                case Direction.West:
                    xOffset = -1; yOffset = 0; break;
                case Direction.East:
                    xOffset = 1; yOffset = 0; break;
            }

            int xo = xOffset;
            int yo = yOffset;
            int lateralOffset = 1;

            while (currentRange++ <= Range)
            {
                xOffset = xo;
                yOffset = yo;

                if (LateralOffset > 1 && currentRange % LateralOffset == 0)
                    lateralOffset++;

                TimeSpan delay = TimeSpan.FromSeconds(currentRange / 10.0);

                switch (AmmoType)
                {
                    case AmmunitionType.Empty: break;
                    case AmmunitionType.Cannonball:
                    case AmmunitionType.FrostCannonball:
                    case AmmunitionType.FlameCannonball:
                        {
                            Point3D newPoint = pnt;
                            //List<IEntity> list = new List<IEntity>();

                            for (int i = -lateralOffset; i <= lateralOffset; i++)
                            {
                                if (xOffset == 0)
                                    newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                                else
                                    newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                                BaseGalleon g = FindValidBoatTarget(newPoint, map, ammo);

                                if (g != null && g.DamageTaken < DamageLevel.Severely && g.Owner is PlayerMobile)
                                {
                                    Target target = new Target
                                    {
                                        Entity = g,
                                        Location = newPoint,
                                        Range = currentRange
                                    };

                                    return new Target[] { target };
                                }
                            }
                        }
                        break;
                    case AmmunitionType.Grapeshot:
                        {
                            Point3D newPoint = pnt;
                            List<Target> mobiles = new List<Target>();

                            for (int i = -lateralOffset; i <= lateralOffset; i++)
                            {
                                if (xOffset == 0)
                                    newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                                else
                                    newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                                foreach (Mobile m in GetTargets(newPoint, map))
                                {
                                    Target target = new Target
                                    {
                                        Entity = m,
                                        Location = newPoint,
                                        Range = currentRange
                                    };

                                    mobiles.Add(target);
                                }

                                if (mobiles.Count > 0 && ammo.SingleTarget)
                                {
                                    Target toHit = mobiles[Utility.Random(mobiles.Count)];
                                    ColUtility.Free(mobiles);
                                    return new Target[] { toHit };
                                }
                            }

                            if (mobiles.Count > 0)
                            {
                                return mobiles.ToArray();
                            }
                        }
                        break;
                }
            }

            return null;
        }

        private IEnumerable<Mobile> GetTargets(Point3D newPoint, Map map)
        {
            if (Operator != null)
            {
                foreach (Mobile m in SpellHelper.AcquireIndirectTargets(Operator, newPoint, map, 0).OfType<Mobile>())
                {
                    yield return m;
                }

                yield break;
            }

            IPooledEnumerable eable = map.GetMobilesInRange(newPoint, 0);

            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile || (m is BaseCreature && ((BaseCreature)m).GetMaster() is PlayerMobile))
                {
                    yield return m;
                }
            }
        }

        public struct Target
        {
            public IEntity Entity { get; set; }
            public Point3D Location { get; set; }
            public int Range { get; set; }
        }

        private BaseGalleon FindValidBoatTarget(Point3D newPoint, Map map, AmmoInfo info)
        {
            BaseGalleon galleon = BaseGalleon.FindGalleonAt(newPoint, map);

            if (galleon != null && info.RequiresSurface)
            {
                int d = galleon is BritannianShip ? 3 : 2;
                switch (galleon.Facing)
                {
                    case Direction.North:
                    case Direction.South:
                        if (newPoint.X <= galleon.X - d || newPoint.X >= galleon.X + d)
                            return null;
                        break;
                    case Direction.East:
                    case Direction.West:
                        if (newPoint.Y <= galleon.Y - d || newPoint.Y >= galleon.Y + d)
                            return null;
                        break;
                }

                StaticTile[] tiles = map.Tiles.GetStaticTiles(newPoint.X, newPoint.Y, true);

                foreach (StaticTile tile in tiles)
                {
                    ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];
                    bool isWater = (tile.ID >= 0x1796 && tile.ID <= 0x17B2);

                    if (!isWater && id.Surface && !id.Impassable)
                    {
                        return galleon;
                    }
                }

                return null;
            }

            return galleon;
        }

        public virtual void OnShipHit(object obj)
        {
            object[] list = (object[])obj;
            BaseBoat target = list[0] as BaseBoat;
            Point3D pnt = (Point3D)list[1];

            AmmoInfo ammoInfo = AmmoInfo.GetAmmoInfo((AmmunitionType)list[2]);

            if (ammoInfo != null && target != null)
            {
                int damage = (Utility.RandomMinMax(ammoInfo.MinDamage, ammoInfo.MaxDamage));
                damage /= 7;
                target.OnTakenDamage(Operator, damage);

                int z = target.ZSurface;

                if (target.TillerMan != null && target.TillerMan is IEntity)
                {
                    z = ((IEntity)target.TillerMan).Z;
                }

                Direction d = Utility.GetDirection(this, pnt);
                int xOffset = 0;
                int yOffset = 0;
                Point3D hit = pnt;

                if (!ammoInfo.RequiresSurface)
                {
                    switch (d)
                    {
                        default:
                        case Direction.North:
                            xOffset = Utility.RandomMinMax(-1, 1);
                            yOffset = Utility.RandomMinMax(-2, 0);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.South:
                            xOffset = Utility.RandomMinMax(-1, 1);
                            yOffset = Utility.RandomMinMax(0, 2);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.East:
                            xOffset = Utility.RandomMinMax(0, 2);
                            yOffset = Utility.RandomMinMax(-1, 1);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                        case Direction.West:
                            xOffset = Utility.RandomMinMax(-2, 0);
                            yOffset = Utility.RandomMinMax(-1, 1);
                            hit = new Point3D(pnt.X + xOffset, pnt.Y + yOffset, z);
                            break;
                    }
                }

                Effects.SendLocationEffect(hit, target.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
                Effects.PlaySound(hit, target.Map, 0x207);

                if (Operator != null && (!Operator.Deleted || CanFireUnmanned))
                {
                    Mobile victim = target.Owner;

                    if (victim != null && target.Contains(victim) && Operator.CanBeHarmful(victim, false))
                    {
                        Operator.DoHarmful(victim);
                    }
                    else
                    {
                        List<Mobile> candidates = new List<Mobile>();
                        SecurityLevel highest = SecurityLevel.Passenger;

                        foreach (PlayerMobile mob in target.MobilesOnBoard.OfType<PlayerMobile>().Where(pm => Operator.CanBeHarmful(pm, false)))
                        {
                            if (target is BaseGalleon && ((BaseGalleon)target).GetSecurityLevel(mob) > highest)
                            {
                                candidates.Insert(0, mob);
                            }
                            else
                            {
                                candidates.Add(mob);
                            }
                        }

                        if (candidates.Count > 0)
                        {
                            Operator.DoHarmful(candidates[0]);
                        }
                        else if (victim != null && Operator.IsHarmfulCriminal(victim))
                        {
                            Operator.CriminalAction(false);
                        }

                        ColUtility.Free(candidates);
                    }
                }
            }
        }

        public virtual void OnMobileHit(object obj)
        {
            object[] objects = (object[])obj;
            Mobile toHit = objects[0] as Mobile;
            Point3D pnt = (Point3D)objects[1];

            AmmoInfo ammoInfo = AmmoInfo.GetAmmoInfo((AmmunitionType)objects[2]);

            if (ammoInfo != null)
            {
                int damage = Utility.RandomMinMax(ammoInfo.MinDamage, ammoInfo.MaxDamage);

                if (Operator != null)
                {
                    Operator.DoHarmful(toHit);
                }

                AOS.Damage(toHit, Operator, damage, ammoInfo.PhysicalDamage, ammoInfo.FireDamage, ammoInfo.ColdDamage, ammoInfo.PoisonDamage, ammoInfo.EnergyDamage);
                Effects.SendLocationEffect(toHit.Location, toHit.Map, Utility.RandomBool() ? 14000 : 14013, 15, 10);
                Effects.PlaySound(toHit.Location, toHit.Map, 0x207);
            }
        }

        public void DoShootEffects()
        {
            Point3D p = Location;
            Map map = Map;

            p.Z -= 3;

            switch (Facing)
            {
                case Direction.North: p.Y--; break;
                case Direction.East: p.X++; break;
                case Direction.South: p.Y++; break;
                case Direction.West: p.X--; break;
            }

            Effects.SendLocationEffect(p, map, 14120, 15, 10);
            Effects.PlaySound(p, map, 0x664);
        }

        public MannedCannon(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(Operator);
            writer.Write(CanFireUnmanned);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Operator = reader.ReadMobile();
                    CanFireUnmanned = reader.ReadBool();
                    break;
            }
        }
    }

    public class MannedCulverin : MannedCannon
    {
        public override int Range => 10;
        public override CannonPower Power => CannonPower.Light;

        public MannedCulverin(Mobile oper, Direction facing)
            : base(oper, facing)
        {
        }

        public MannedCulverin(Serial serial) : base(serial) { }

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

    public class MannedCarronade : MannedCannon
    {
        public override int Range => 10;
        public override CannonPower Power => CannonPower.Heavy;

        public MannedCarronade(Mobile oper, Direction facing)
            : base(oper, facing)
        {
        }

        public MannedCarronade(Serial serial) : base(serial) { }

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

    public class MannedBlundercannon : MannedCannon
    {
        public override int LabelNumber => 1158942;  // Blundercannon

        public override int Range => 12;
        public override CannonPower Power => CannonPower.Massive;

        public MannedBlundercannon(Mobile oper, Direction facing)
            : base(oper, facing)
        {
        }

        public MannedBlundercannon(Serial serial) : base(serial) { }

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
