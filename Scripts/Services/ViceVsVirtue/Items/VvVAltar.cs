using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Network;
using Server.Guilds;
using System.Linq;
using Server.Engines.Points;

namespace Server.Engines.VvV
{
    public class VvVAltar : BaseAddon
    {
        public VvVBattle Battle { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsActive { get; set; }

        public List<Item> Braziers { get; set; }
        public List<Item> Torches { get; set; }

        public override bool HandlesOnMovement { get { return IsActive; } }

        public OccupyTimer OccupationTimer { get; set; }
        public Timer CheckTimer { get; set; }

        public VvVAltar(VvVBattle battle)
        {
            Battle = battle;

            Braziers = new List<Item>();
            Torches = new List<Item>();

            int virtue = Utility.Random(8);

            AddComponent(new AddonComponent(1822), -2, -2, 0);
            AddComponent(new AddonComponent(1822), -1, -2, 0);
            AddComponent(new AddonComponent(1822), 0, -2, 0);
            AddComponent(new AddonComponent(1822), 1, -2, 0);
            AddComponent(new AddonComponent(1822), 2, -2, 0);

            AddComponent(new AddonComponent(1822), -2, -1, 0);
            AddComponent(new AddonComponent(1822), -1, -1, 0);
            AddComponent(new AddonComponent(1822), 0, -1, 0);
            AddComponent(new AddonComponent(1822), 1, -1, 0);
            AddComponent(new AddonComponent(1822), 2, -1, 0);

            AddComponent(new AddonComponent(1822), -2, 0, 0);
            AddComponent(new AddonComponent(1822), -1, 0, 0);
            AddComponent(new AddonComponent(1822), 0, 0, 0);
            AddComponent(new AddonComponent(1822), 1, 0, 0);
            AddComponent(new AddonComponent(1822), 2, 0, 0);

            AddComponent(new AddonComponent(1822), -2, 1, 0);
            AddComponent(new AddonComponent(1822), -1, 1, 0);
            AddComponent(new AddonComponent(1822), 0, 1, 0);
            AddComponent(new AddonComponent(1822), 1, 1, 0);
            AddComponent(new AddonComponent(1822), 2, 1, 0);

            AddComponent(new AddonComponent(1822), -2, 2, 0);
            AddComponent(new AddonComponent(1822), -1, 2, 0);
            AddComponent(new AddonComponent(1822), 0, 2, 0);
            AddComponent(new AddonComponent(1822), 1, 2, 0);
            AddComponent(new AddonComponent(1822), 2, 2, 0);

            //NorthWest
            AddComponent(new AddonComponent(_Tiles[0][virtue]), -2, -2, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 1), -2, -1, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 2), -1, -1, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 3), -1, -2, 5);

            // SouthEast
            AddComponent(new AddonComponent(_Tiles[0][virtue]), 1, 1, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 1), 1, 2, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 2), 2, 2, 5);
            AddComponent(new AddonComponent(_Tiles[0][virtue] + 3), 2, 1, 5);

            //SouthWest
            AddComponent(new AddonComponent(_Tiles[1][virtue]), -2, 1, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 1), -1, 1, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 2), -2, 2, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 3), -1, 2, 5);

            //NorthEast
            AddComponent(new AddonComponent(_Tiles[1][virtue]), 1, -2, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 1), 2, -2, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 2), 1, -1, 5);
            AddComponent(new AddonComponent(_Tiles[1][virtue] + 3), 2, -1, 5);

            AddComponent(new AddonComponent(1866), -1, -3, 0);
            AddComponent(new AddonComponent(1847), 0, -3, 0);
            AddComponent(new AddonComponent(1868), 1, -3, 0);

            AddComponent(new AddonComponent(1868), 3, -1, 0);
            AddComponent(new AddonComponent(1846), 3, 0, 0);
            AddComponent(new AddonComponent(1867), 3, 1, 0);

            AddComponent(new AddonComponent(1869), -1, 3, 0);
            AddComponent(new AddonComponent(1823), 0, 3, 0);
            AddComponent(new AddonComponent(1867), 1, 3, 0);

            AddComponent(new AddonComponent(1866), -3, -1, 0);
            AddComponent(new AddonComponent(1865), -3, 0, 0);
            AddComponent(new AddonComponent(1869), -3, 1, 0);

            AddonComponent c = new AddonComponent(6570);
            AddComponent(c, 3, -3, 0);
            Braziers.Add(c);

            c = new AddonComponent(6570);
            AddComponent(c, 3, 3, 0);
            Braziers.Add(c);

            c = new AddonComponent(6570);
            AddComponent(c, -3, 3, 0);
            Braziers.Add(c);

            c = new AddonComponent(6570);
            AddComponent(c, -3, -3, 0);
            Braziers.Add(c);
        }

        public bool Contains(IPoint3D p)
        {
            if (p is IEntity && ((IEntity)p).Map != this.Map)
                return false;

            return p.X >= this.X - 2 && p.X <= this.X + 2 && p.Y >= this.Y - 2 && p.Y <= this.Y + 2;
        }

        public void Activate()
        {
            IsActive = true;

            CheckTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500), CheckOccupy);
            CheckTimer.Start();
        }

        public void Complete(Guild g)
        {
            Timer.DelayCall<Guild>(TimeSpan.FromSeconds(5), Battle.OccupyAltar, g);
            Timer.DelayCall(TimeSpan.FromSeconds(2), DoFireworks);
            IsActive = false;

            if (OccupationTimer != null)
            {
                OccupationTimer = null;
            }

            if (CheckTimer != null)
            {
                CheckTimer.Stop();
                CheckTimer = null;
            }

            Timer.DelayCall(TimeSpan.FromMinutes(2), () =>
                {
                    Torches.ForEach(t => t.Delete());
                    Torches.Clear();
                });
        }

        public void DoFireworks()
        {
            if (Deleted)
                return;

            for (int i = 2; i <= 8; i += 2)
            {
                Server.Timer.DelayCall(TimeSpan.FromMilliseconds((i - 2) * 600), o =>
                {
                    Server.Misc.Geometry.Circle2D(this.Location, this.Map, (int)o, (pnt, map) =>
                    {
                        LaunchFireworks(pnt, map);
                    });
                }, i);
            }
        }

        public static void LaunchFireworks(Point3D p, Map map)
        {
            if (map == null || map == Map.Internal)
                return;

            Point3D startLoc = new Point3D(p.X, p.Y, p.Z + 10);
            Point3D endLoc = new Point3D(p.X + Utility.RandomMinMax(-1, 1), p.Y + Utility.RandomMinMax(-1, 1), p.Z + 32);

            Effects.SendMovingEffect(new Entity(Serial.Zero, startLoc, map), new Entity(Serial.Zero, endLoc, map),
                0x36E4, 5, 0, false, false);

            Server.Timer.DelayCall(TimeSpan.FromSeconds(1.0), () =>
                {
                    int hue = Utility.Random(40);

                    if (hue < 8)
                        hue = 0x66D;
                    else if (hue < 10)
                        hue = 0x482;
                    else if (hue < 12)
                        hue = 0x47E;
                    else if (hue < 16)
                        hue = 0x480;
                    else if (hue < 20)
                        hue = 0x47F;
                    else
                        hue = 0;

                    if (Utility.RandomBool())
                        hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D);

                    int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);

                    Effects.PlaySound(endLoc, map, Utility.Random(0x11B, 4));
                    Effects.SendLocationEffect(endLoc, map, 0x373A + (0x10 * Utility.Random(4)), 16, 10, hue, renderMode);
                });
        }

        public void CheckOccupy()
        {
            if (!IsActive || this.Map == null || this.Map == Map.Internal)
                return;

            IPooledEnumerable eable = this.Map.GetMobilesInBounds(new Rectangle2D(this.X - 2, this.Y - 2, 5, 5));
            int count = 0;

            foreach (Mobile m in eable)
            {
                VvVPlayerEntry entry;

                if (ViceVsVirtueSystem.IsVvV(m, out entry))
                {
                    count++;

                    if (OccupationTimer != null)
                    {
                        Guild g = OccupationTimer.Occupier;

                        if (entry.Guild != g && !entry.Guild.IsAlly(g))
                        {
                            Clear();
                            break;
                        }
                    }
                    else
                    {
                        this.OccupationTimer = new OccupyTimer(this, entry.Guild);
                        this.OccupationTimer.Start();
                    }
                }
            }

            if (OccupationTimer != null && count == 0)
                Clear();

            eable.Free();
        }

        private void Clear()
        {
            OccupationTimer.Stop();
            OccupationTimer = null;

            Torches.ForEach(t => t.Delete());
            Torches.Clear();
        }

        public class OccupyTimer : Timer
        {
            public Guild Occupier { get; set; }
            public VvVAltar Altar { get; set; }

            private int _Tick;

            public OccupyTimer(VvVAltar altar, Guild occupier)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                Occupier = occupier;
                Altar = altar;
                _Tick = 0;
            }

            protected override void OnTick()
            {
                Point3D p = _Locs[_Tick];

                Effects.SendLocationEffect(new Point3D(Altar.X + p.X, Altar.Y + p.Y, Altar.Z + p.Z), Altar.Map, 0x3709, 30, 10);
                Effects.PlaySound(Altar.Location, Altar.Map, 0x208);

                int index = _Tick / 4;

                if (_Tick > 0 && index < Altar.Braziers.Count && (_Tick + 1) % 4 == 0)
                {
                    Server.Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            AddonComponent torch = new AddonComponent(6571);
                            Altar.Torches.Add(torch);

                            Point3D to = Altar.Braziers[index].Location;

                            torch.MoveToWorld(new Point3D(to.X, to.Y, to.Z + 17), Altar.Map);
                            Effects.PlaySound(to, Altar.Map, 0x47);
                        });
                }

                _Tick++;

                if (_Tick >= _Locs.Length)
                {
                    Altar.Complete(Occupier);
                    Stop();
                }
            }

            private Point3D[] _Locs =
            {
                new Point3D(-1, -2, 7), new Point3D(0, -2, 7), new Point3D(1, -2, 7), new Point3D(2, -2, 7),
                new Point3D(2, -1, 7), new Point3D(2, 0, 7), new Point3D(2, 1, 7), new Point3D(2, 2, 7),
                new Point3D(1, 2, 7), new Point3D(0, 2, 7), new Point3D(-1, 2, 7), new Point3D(-2, 2, 7),
                new Point3D(-2, 1, 7), new Point3D(-2, 0, 7), new Point3D(-2, -1, 7), new Point3D(-2, -2, 7),
            };
        }

        public override void Delete()
        {
            base.Delete();

            Torches.ForEach(t => t.Delete());
        }

        public VvVAltar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(IsActive);

            writer.Write(Braziers.Count);
            Braziers.ForEach(b => writer.Write(b));

            writer.Write(Torches.Count);
            Torches.ForEach(t => writer.Write(t));
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Braziers = new List<Item>();
            Torches = new List<Item>();

            IsActive = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadItem();
                if (item != null)
                    Braziers.Add(item);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadItem();
                if (item != null)
                    Torches.Add(item);
            }

            if (IsActive)
            {
                CheckTimer = Timer.DelayCall(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500), CheckOccupy);
                CheckTimer.Start();
            }
        }

        private int[][] _Tiles =
        {
            new int[] { 5283,  5291,  5299,  5307,  5315,  5323,  5331,  5390 },
            new int[] { 39372, 39380, 39388, 39396, 39404, 39412, 39420, 39428 }
        };
    }

    public class AltarArrow : QuestArrow
    {
        public Timer Timer { get; private set; }
        public VvVAltar Altar { get; private set; }

        public int LastX { get; private set; }
        public int LastY { get; private set; }

        public AltarArrow(Mobile from, IEntity target)
            : base(from, target)
        {
            Altar = target as VvVAltar;

            Timer = Timer.DelayCall(TimeSpan.FromSeconds(0.25), TimeSpan.FromSeconds(2.5), OnTick); //new TrackTimer(from, target, range, this);
            Timer.Start();
        }

        public void OnTick()
        {
            if (!Running)
            {
                Timer.Stop();
                return;
            }
            else if (this.Mobile.NetState == null || this.Mobile.Deleted || Altar.Deleted || this.Mobile.Map != Altar.Map
                || ViceVsVirtueSystem.Instance.Battle == null || !ViceVsVirtueSystem.Instance.Battle.OnGoing || !this.Mobile.Region.IsPartOf(ViceVsVirtueSystem.Instance.Battle.Region)
                || Altar.Contains(this.Mobile))
            {
                Stop();
                return;
            }

            // this should never happen!
            if (LastX != Target.X || LastY != Target.Y)
            {
                LastX = Target.X;
                LastY = Target.Y;

                Update();
            }
        }

        public override void OnStop()
        {
            Timer.Stop();
        }
    }
}