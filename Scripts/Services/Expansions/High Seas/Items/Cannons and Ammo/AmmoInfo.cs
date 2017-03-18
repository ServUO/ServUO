using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public interface ICannonAmmo
    {
        AmmoType AmmoType { get; }
    }

    public class AmmoInfo
    {
        private static Dictionary<Type, AmmoInfo> m_Infos;

        public static void Initialize()
        {
            m_Infos = new Dictionary<Type, AmmoInfo>();

            m_Infos[typeof(LightCannonball)] = new AmmoInfo(typeof(LightCannonball), "cannonball", 20, 30, 3);
            m_Infos[typeof(HeavyCannonball)] = new AmmoInfo(typeof(HeavyCannonball), "cannonball", 30, 50, 3);
            m_Infos[typeof(LightGrapeshot)] = new AmmoInfo(typeof(LightGrapeshot), "grapeshot", 40, 50, 3);
            m_Infos[typeof(HeavyGrapeshot)] = new AmmoInfo(typeof(HeavyGrapeshot), "grapeshot", 50, 75, 3);

            m_Infos[typeof(HeavyFlameCannonball)] = new AmmoInfo(typeof(HeavyFlameCannonball), "cannonball", 30, 50, 3, true, 50, 50, 0, 0, 0, false);
            m_Infos[typeof(LightFlameCannonball)] = new AmmoInfo(typeof(LightFlameCannonball), "cannonball", 20, 30, 3, true, 50, 50, 0, 0, 0, false);
            m_Infos[typeof(HeavyFrostCannonball)] = new AmmoInfo(typeof(HeavyFrostCannonball), "cannonball", 30, 50, 3, true, 50, 0, 50, 0, 0, false);
            m_Infos[typeof(LightFrostCannonball)] = new AmmoInfo(typeof(LightFrostCannonball), "cannonball", 20, 30, 3, true, 50, 0, 50, 0, 0, false);

            // Custom Ammo Types, you can un-comment out to use!
            /*m_Infos[typeof(LightScatterShot)] = new AmmoInfo(typeof(LightScatterShot), "scatter shot", 20, 30, 1, false);
            m_Infos[typeof(HeavyScatterShot)] = new AmmoInfo(typeof(HeavyScatterShot), "scatter shot", 30, 40, 1, false);

            //These require special action with onhit
            m_Infos[typeof(LightFragShot)] = new AmmoInfo(typeof(LightFragShot), "frag shot", 20, 30, 3, true, 25, 75, 0, 0, 0, false);
            m_Infos[typeof(HeavyFragShot)] = new AmmoInfo(typeof(HeavyFragShot), "frag shot", 35, 45, 3, true, 25, 75, 0, 0, 0, false);

            m_Infos[typeof(LightHotShot)] = new AmmoInfo(typeof(LightHotShot), "hot shot", 15, 25, 6, true, 0, 100, 0, 0, 0, true);
            m_Infos[typeof(HeavyHotShot)] = new AmmoInfo(typeof(HeavyHotShot), "hot shot", 25, 35, 6, true, 0, 100, 0, 0, 0, true);*/
        }

        private Type m_AmmoType;
        private string m_Name;
        private int m_MinDamage;
        private int m_MaxDamage;
        private int m_LateralOffset;
        private int m_PhysicalDamage;
        private int m_FireDamage;
        private int m_ColdDamage;
        private int m_PoisonDamage;
        private int m_EnergyDamage;
        private bool m_SingleTarget;
        private bool m_RequiresSurface;

        public Type AmmoType { get { return m_AmmoType; } }
        public string Name { get { return m_Name; } }
        public int MinDamage { get { return m_MinDamage; } }
        public int MaxDamage { get { return m_MaxDamage; } }
        public int LateralOffset { get { return m_LateralOffset; } }
        public int PhysicalDamage { get { return m_PhysicalDamage; } }
        public int FireDamage { get { return m_FireDamage; } }
        public int ColdDamage { get { return m_ColdDamage; } }
        public int PoisonDamage { get { return m_PoisonDamage; } }
        public int EnergyDamage { get { return m_EnergyDamage; } }
        public bool SingleTarget { get { return m_SingleTarget; } }
        public bool RequiresSurface { get { return m_RequiresSurface; } }

        public AmmoInfo(Type type, string name, int minDamage, int maxDamage, int lateralOffset) 
            : this(type, name, minDamage, maxDamage, lateralOffset, true, 100, 0, 0, 0, 0, false)
        {
        }

        public AmmoInfo(Type type, string name, int minDamage, int maxDamage, int lateralOffset, bool singleOnly)
            : this(type, name, minDamage, maxDamage, lateralOffset, singleOnly, 100, 0, 0, 0, 0, false)
        {
        }

        public AmmoInfo(Type type, string name, int minDamage, int maxDamage, int lateralOffset, bool singleOnly, int phys, int fire, int cold, int poison, int energy, bool requiresSurface)
        {
            m_AmmoType = type;
            m_Name = name;
            m_MinDamage = minDamage;
            m_MaxDamage = maxDamage;
            m_LateralOffset = lateralOffset;
            m_PhysicalDamage = phys;
            m_FireDamage = fire;
            m_ColdDamage = cold;
            m_PoisonDamage = poison;
            m_EnergyDamage = energy;
            m_SingleTarget = singleOnly;
            m_RequiresSurface = requiresSurface;
        }

        public static AmmoInfo GetAmmoInfo(Type ammoType)
        {
            if (ammoType != null && m_Infos.ContainsKey(ammoType))
                return m_Infos[ammoType];

            return null;
        }

        public static void OnHit(Mobile shooter, IPoint3D hit, Map map, AmmoInfo info)
        {
            /*if (info == null || map == null || map == Map.Internal || shooter == null || hit == null)
                return;

            if (info.AmmoType == typeof(LightHotShot) || info.AmmoType == typeof(HeavyHotShot))
            {
                BaseGalleon g = BaseGalleon.FindGalleonAt(hit, map);

                if (g != null)
                {
                    int x = hit.X;
                    int y = hit.Y;
                    int d = g is BritannianShip ? 3 : 2;

                    switch (g.Facing)
                    {
                        case Direction.North:
                        case Direction.South:
                            do  
                            {  
                                x = g.X + Utility.RandomMinMax(-d, d);
                                y = g.Y + Utility.RandomMinMax(-1, 1);
                            }
                            while (x == g.X);
                            break;
                        case Direction.East:
                        case Direction.West:
                            do 
                            {
                                x = g.X + Utility.RandomMinMax(-1, 1);
                                y = g.Y + Utility.RandomMinMax(-d, d);
                            }
                            while (y == g.Y);
                                
                                break;
                    }

                    Point3D point = new Point3D(x, y, g.ZSurface);
                    GetSurfaceTop(ref point, map);

                    new HotShotItem(point, shooter, map, TimeSpan.FromSeconds(45), 15);
                }
            }
            else if (info.AmmoType == typeof(LightFragShot) || info.AmmoType == typeof(HeavyFragShot))
            {
                Mobile hitMobile = hit as Mobile;
                Point3D p = new Point3D(hit);
                List<Mobile> list = new List<Mobile>();

                IPooledEnumerable eable = map.GetMobilesInRange(p, 2);
                foreach (Mobile m in eable)
                {
                    if (m != hitMobile && Server.Spells.SpellHelper.ValidIndirectTarget(shooter, m) && shooter.CanBeHarmful(m, false))
                        list.Add(m);
                }

                new FragShotTimer(hitMobile, shooter, p, map, Utility.RandomMinMax(info.MinDamage, info.MaxDamage), list);
            }*/
        }

        public static void GetSurfaceTop(ref Point3D p, Map map)
        {
            StaticTile[] tiles = map.Tiles.GetStaticTiles(p.X, p.Y, true);
            int z = p.Z;

            foreach (StaticTile tile in tiles)
            {
                ItemData id = TileData.ItemTable[tile.ID & TileData.MaxItemValue];

                if (id.Surface && (z == p.Z || tile.Z + id.CalcHeight > z))
                {
                    z = tile.Z + id.CalcHeight;
                }
            }

            if (z != p.Z)
                p.Z = z;
        }

        public static string GetAmmoName(BaseCannon cannon)
        {
            AmmoInfo info = AmmoInfo.GetAmmoInfo(cannon.LoadedAmmo);

            if (info != null)
                return info.Name;

            return "unloaded";
        }

        /*private class FragShotTimer : Timer
        {
            private Point3D m_Center;
            private Map m_Map;
            private Mobile m_HitMobile;
            private Mobile m_Shooter;
            private List<Mobile> m_Affected;
            private int m_Damage;
            private int m_Tick;

            public FragShotTimer(Mobile hitMobile, Mobile shooter, Point3D center, Map map, int damage, List<Mobile> affected)
                : base(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(100))
            {
                m_HitMobile = hitMobile;
                m_Shooter = shooter;
                m_Center = center;
                m_Map = map;
                m_Affected = affected;
                m_Damage = damage;
                m_Tick = 0;

                this.Start();
            }

            protected override void OnTick()
            {
                m_Tick++;

                if (m_Tick > 3)
                {
                    BaseGalleon g = null;
                    for (int i = 0; i < m_Affected.Count; i++)
                    {
                        Mobile toFrag = m_Affected[i];
                        int dist = (int)toFrag.GetDistanceToSqrt(m_Center);

                        int damage = dist > 0 ? m_Damage / (dist + 1) : m_Damage;
                        AOS.Damage(toFrag, m_Shooter, damage, 100, 0, 0, 0, 0);

                        if(0.55 > Utility.RandomDouble())
                            BleedAttack.BeginBleed(toFrag, m_Shooter);

                        toFrag.FixedParticles(0x37C4, 1, 8, 9916, 39, 3, EffectLayer.Head);
                        toFrag.FixedParticles(0x37C4, 1, 8, 9502, 39, 4, EffectLayer.Head);

                        if (g == null)
                            g = BaseGalleon.FindGalleonAt(toFrag.Location, toFrag.Map);
                    }

                    if(g != null)
                        g.OnTakenDamage(m_Shooter, Utility.RandomMinMax(1, 3));

                    this.Stop();
                    return;
                }

                Server.Misc.Geometry.Circle2D(m_Center, m_Map, m_Tick, new Server.Misc.DoEffect_Callback(DoEffect));
            }

            public void DoEffect(Point3D p, Map map)
            {
                int[] effect = new int[] { 14000, 14013, 14026, 14120 };
                Effects.PlaySound(p, map, 0x307);
                Effects.SendLocationEffect(p, map, effect[Utility.Random(effect.Length)], 20);
            }
        }
    }

    public class HotShotItem : Item
    {
        private Timer m_Timer;
        private DateTime m_End;
        private Mobile m_Shooter;
        private int m_Damage;

        public override bool BlocksFit { get { return true; } }

        public Mobile Shooter { get { return m_Shooter; } }

        public HotShotItem(Point3D loc, Mobile shooter, Map map, TimeSpan duration, int damage)
            : base(Utility.RandomDouble() > 0.5 ? 0x398C : 0x3996)
        {
            Movable = false;
            Light = LightType.Circle300;

            MoveToWorld(loc, map);

            m_Shooter = shooter;

            m_Damage = damage;

            m_End = DateTime.UtcNow + duration;

            m_Timer = new InternalTimer(this, duration);
            m_Timer.Start();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public HotShotItem(Serial serial)
            : base(serial)
        {
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

            Delete();
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (Visible && m_Shooter != null && m != m_Shooter && Server.Spells.SpellHelper.ValidIndirectTarget(m_Shooter, m) && m_Shooter.CanBeHarmful(m, false))
            {
                m_Shooter.DoHarmful(m);
                int damage = m_Damage;
                AOS.Damage(m, m_Shooter, damage, 0, 100, 0, 0, 0);
                m.PlaySound(0x208);

                if (m is BaseCreature)
                    ((BaseCreature)m).OnHarmfulSpell(m_Shooter);
            }

            return true;
        }

        private class InternalTimer : Timer
        {
            private HotShotItem m_Item;

            private static Queue m_Queue = new Queue();

            public InternalTimer(HotShotItem item, TimeSpan delay)
                : base(delay, TimeSpan.FromSeconds(1.0))
            {
                m_Item = item;
                Priority = TimerPriority.FiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Item.Deleted)
                    return;

                if (!m_Item.Visible)
                {
                    m_Item.Visible = true;

                    if (!m_Item.Deleted)
                    {
                        m_Item.ProcessDelta();
                        Effects.SendLocationParticles(EffectItem.Create(m_Item.Location, m_Item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5029);
                    }
                }
                else if (DateTime.UtcNow > m_Item.m_End)
                {
                    m_Item.Delete();
                    Stop();
                }
                else
                {
                    Map map = m_Item.Map;
                    Mobile shooter = m_Item.m_Shooter;

                    if (map != null && shooter != null)
                    {
                        IPooledEnumerable eable = m_Item.GetMobilesInRange(0);
                        foreach (Mobile m in eable)
                        {
                            if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && m != shooter && Server.Spells.SpellHelper.ValidIndirectTarget(shooter, m) && shooter.CanBeHarmful(m, false))
                                m_Queue.Enqueue(m);
                        }
                        eable.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile m = (Mobile)m_Queue.Dequeue();
                            shooter.DoHarmful(m);
                            int damage = m_Item.m_Damage;

                            AOS.Damage(m, shooter, damage, 0, 100, 0, 0, 0);
                            m.PlaySound(0x208);

                            if (m is BaseCreature)
                                ((BaseCreature)m).OnHarmfulSpell(shooter);
                        }
                    }
                }
            }
        }*/
    }
}