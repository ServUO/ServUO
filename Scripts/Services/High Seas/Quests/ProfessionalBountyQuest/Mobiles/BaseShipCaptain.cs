using Server;
using System;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Engines.Quests;
using System.Linq;

namespace Server.Mobiles
{
    public class BaseShipCaptain : BaseCreature
    {
        public static readonly TimeSpan MoveFrequency = TimeSpan.FromSeconds(2);
        public static readonly TimeSpan DeleteAfterDeath = TimeSpan.FromMinutes(15);
        public static readonly TimeSpan ResumeTime = TimeSpan.FromSeconds(Utility.RandomMinMax(30, 45));
        public static readonly TimeSpan DecayRetry = TimeSpan.FromSeconds(30);

        private BaseGalleon m_Galleon;
        private bool m_OnCourse;
        private DateTime m_NextCannonShot;
        private DateTime m_NextMoveCheck;
        private DateTime m_ActionTime;
        private SpawnZone m_Zone;
        private bool m_Blockade;
        private List<Mobile> m_Crew = new List<Mobile>();

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return m_Galleon; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OnCourse { get { return m_OnCourse; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextCannonShot { get { return m_NextCannonShot; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextMoveCheck { get { return m_NextMoveCheck; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpawnZone Zone { get { return m_Zone; } set { m_Zone = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Blockade { get { return m_Blockade; } set { m_Blockade = value; } }

        public List<Mobile> Crew { get { return m_Crew; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool Aggressive { get { return true; } }

        public override bool PlayerRangeSensitive { get { return false; } }

        public override double TreasureMapChance { get { return 0.05; } }
        public override int TreasureMapLevel { get { return 7; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual TimeSpan ShootFrequency
        {
            get
            {
                return TimeSpan.FromSeconds(Math.Min(20, 20.0 - ((double)m_Crew.Count * 2.5)));
            }
        }

        [Constructable]
        public BaseShipCaptain() : this(null) { }

        public BaseShipCaptain(BaseGalleon galleon)
            : this(galleon, AIType.AI_Melee, FightMode.Weakest, 10, 1, .2, .4)
        {
        }

        public BaseShipCaptain(BaseGalleon galleon, AIType ai, FightMode fm, int per, int range, double passive, double active)
            : base(ai, fm, per, range, passive, active)
        {
            m_Galleon = galleon;
            m_OnCourse = true;
            m_StopTime = DateTime.MinValue;

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new Skirt(Utility.RandomNeutralHue()));
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomNeutralHue()));
            }

            SetStr(500, 750);
            SetDex(125, 175);
            SetInt(61, 75);

            SetHits(4500, 5000);

            SetDamage(23, 35);

            SetSkill(SkillName.Fencing, 115.0, 120.0);
            SetSkill(SkillName.Macing, 115.0, 120.0);
            SetSkill(SkillName.MagicResist, 115.0, 120.0);
            SetSkill(SkillName.Swords, 115.0, 120.0);
            SetSkill(SkillName.Tactics, 115.0, 120.0);
            SetSkill(SkillName.Wrestling, 115.0, 120.0);
            SetSkill(SkillName.Anatomy, 115.0, 120.0);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 45, 55);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 45, 55);
            SetResistance(ResistanceType.Energy, 45, 55);

            if (galleon == null)
                Timer.DelayCall(TimeSpan.FromSeconds(.5), new TimerCallback(SpawnShip));
        }

        public void SpawnShip()
        {
            BaseGalleon gal;

            if (this is PirateCaptain)
                gal = new OrcishGalleon(Direction.North);
            else if (this.Map == Map.Tokuno)
                gal = new TokunoGalleon(Direction.North);
            else
                gal = new GargishGalleon(Direction.North);

            if (gal.CanFit(this.Location, this.Map, gal.ItemID))
            {
                gal.Owner = this;
                gal.MoveToWorld(this.Location, this.Map);
                m_Galleon = gal;

                Server.Engines.Quests.BountyQuestSpawner.FillHold(m_Galleon);

                this.MoveToWorld(new Point3D(gal.X, gal.Y - 1, gal.ZSurface), this.Map);

                int crewCount = Utility.RandomMinMax(3, 5);

                for (int j = 0; j < crewCount; j++)
                {
                    Mobile crew = new PirateCrew();

                    if (j == 0 && this is PirateCaptain)
                        crew.Title = "the orc captain";

                    AddToCrew(crew);
                    crew.MoveToWorld(new Point3D(gal.X + Utility.RandomList(-1, 1), gal.Y + Utility.RandomList(-1, 0, 1), gal.ZSurface), this.Map);
                }

                gal.AutoAddCannons(this);

                return;
            }
            else
            {
                gal.Delete();
                Delete();
            }
        }

        public void OnShipDelete()
        {
            if (this.Alive && !this.Deleted)
                Kill();

            for (int i = 0; i < m_Crew.Count; i++)
            {
                Mobile mob = m_Crew[i];

                if (mob != null && !mob.Deleted)
                    mob.Kill();
            }
        }

        public override void Delete()
        {
            if(BountyQuestSpawner.Instance != null)
                BountyQuestSpawner.Instance.HandleDeath(this);

            if (m_Galleon != null && !m_Galleon.Deleted)
                Timer.DelayCall(DeleteAfterDeath, new TimerStateCallback(TryDecayGalleon), m_Galleon);

            base.Delete();
        }

        public override void OnCombatantChange()
        {
            if (Combatant == null)
                CantWalk = true;
            else
                CantWalk = false;

            base.OnCombatantChange();
        }

        public void TryDecayGalleon(object obj)
        {
            BaseGalleon gal = obj as BaseGalleon;

            if (gal == null)
                return;

            List<ISpawnable> list = gal.GetObjectsOnBoard();

            foreach (ISpawnable i in list)
            {
                if (i is PlayerMobile)
                {
                    Timer.DelayCall(DecayRetry, new TimerStateCallback(TryDecayGalleon), gal);
                    return;
                }
            }

            if (gal != null && !gal.Deleted)
                gal.ForceDecay();
        }

        public void AddToCrew(Mobile mob)
        {
            if (!m_Crew.Contains(mob))
                m_Crew.Add(mob);
        }

        private DateTime m_StopTime;

        public void ResumeCourseTimed(TimeSpan ts, bool check)
        {
            Timer.DelayCall(ts, new TimerCallback(ResumeCourse));
        }

        public void ResumeCourse()
        {
            if (m_Galleon != null)
            {
                m_Galleon.StartCourse(false, false);
                m_OnCourse = true;
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            if (m_Galleon == null)
                return;

            if (m_Galleon.Deleted)
                OnShipDelete();

            // Ship is fucked without his captain!!!
            if (!m_Galleon.Contains(this))
                return;

            if (!IsInBattle())
            {
                if (!m_OnCourse)
                    ResumeCourse();
                else if (m_OnCourse && !m_Galleon.IsMoving && m_ActionTime < DateTime.UtcNow)
                {
                    ResumeCourseTimed(ResumeTime, true);
                    m_ActionTime = DateTime.UtcNow + ResumeTime;
                }
                return;
            }

            m_OnCourse = false;

            Mobile focusMob = GetFocusMob();

            if(m_TargetBoat == null || !InRange(m_TargetBoat.Location, 25))
                m_TargetBoat = GetFocusBoat(focusMob);

            if (focusMob == null && m_TargetBoat == null)
                return;

            if (m_NextMoveCheck < DateTime.UtcNow && !m_Galleon.Scuttled && !m_Blockade)
            {
                Point3D pnt = m_TargetBoat != null ? m_TargetBoat.Location : focusMob.Location;

                int dist = (int)GetDistanceToSqrt(pnt);

                if (!Aggressive && dist < 25)
                    MoveBoat(pnt);
                else if (Aggressive && dist >= 10 && dist <= 35)
                    MoveBoat(pnt);
                else
                {
                    m_Galleon.StopMove(false);
                    ResumeCourseTimed(TimeSpan.FromMinutes(2), false); //Loiter
                }
            }

            if (m_TargetBoat != null && !m_TargetBoat.Scuttled)
                ShootCannons(focusMob, true);
            else
                ShootCannons(focusMob, false);
        }

        private BaseGalleon m_TargetBoat;

        public Mobile GetFocusMob()
        {
            Mobile focus = Combatant as Mobile;

            if (focus == null || focus.Deleted || !focus.Alive)
            {
                int closest = 25;

                foreach (Mobile mob in m_Crew)
                {
                    if (mob.Alive && mob.Combatant is Mobile)
                    {
                        if (focus == null || (int)focus.GetDistanceToSqrt(mob) < closest)
                        {
                            focus = mob.Combatant as Mobile;
                            closest = (int)focus.GetDistanceToSqrt(mob);
                        }
                    }
                }
            }

            return focus;
        }

        public BaseGalleon GetFocusBoat(Mobile focusMob)
        {
            if (focusMob == null || focusMob.Deleted || focusMob.Map == null || focusMob.Map == Map.Internal)
                return null;

            BaseGalleon g = BaseGalleon.FindGalleonAt(focusMob, focusMob.Map);

            return g != m_Galleon ? g : null;
        }

        public void MoveBoat(Point3D p)
        {
            if (m_Galleon == null || m_Galleon.Contains(p))
                return;

            int x = p.X;
            int y = p.Y;
            int speed;
            int flee = Aggressive ? 1 : -1;

            //Direction d = Utility.GetDirectionTo(p.X, p.Y);
            Direction dir = m_Galleon.GetMovementFor(x, y, out speed);
            Direction f = m_Galleon.Facing;

            if (!Aggressive)
                dir = (Direction)(((int)dir + -4) & 0x7);

            if (dir == Direction.West || dir == Direction.Left || dir == Direction.South)
            {
                m_Galleon.Turn(-2 * flee, true);
                m_NextMoveCheck = DateTime.UtcNow + TimeSpan.FromSeconds(m_Galleon.TurnDelay);
                return;
            }
            else if (dir == Direction.East || dir == Direction.Down)
            {
                m_Galleon.Turn(2 * flee, true);
                m_NextMoveCheck = DateTime.UtcNow + TimeSpan.FromSeconds(m_Galleon.TurnDelay);
                return;
            }

            m_Galleon.StartMove(dir, true);
        }

        private Dictionary<BaseCannon, DateTime> m_ShootTable = new Dictionary<BaseCannon, DateTime>();

        public void ShootCannons(Mobile focus, bool shootAtBoat)
        {
            List<Item> cannons = new List<Item>(m_Galleon.Cannons.Where(i => !i.Deleted));

            foreach (BaseCannon cannon in cannons.OfType<BaseCannon>())
            {
                if (cannon != null)
                {
                    if (m_ShootTable.ContainsKey(cannon) && m_ShootTable[cannon] > DateTime.UtcNow)
                        continue;

                    Direction facing = cannon.GetFacing();

                    if (shootAtBoat && HasTarget(focus, cannon, true))
                    {
                        cannon.AmmoType = AmmoType.Cannonball;
                        cannon.LoadedAmmo = cannon.LoadTypes[0];
                    }
                    else if (!shootAtBoat && HasTarget(focus, cannon, false))
                    {
                        cannon.AmmoType = AmmoType.Grapeshot;
                        cannon.LoadedAmmo = cannon.LoadTypes[1];
                    }
                    else
                        continue;

                    cannon.Shoot(this);
                    m_ShootTable[cannon] = DateTime.UtcNow + ShootFrequency + TimeSpan.FromSeconds(Utility.RandomMinMax(0, 3));
                }
            }
        }

        private bool HasTarget(Mobile focus, BaseCannon cannon, bool shootatboat)
        {
            if (cannon == null || cannon.Deleted || cannon.Map == null || cannon.Map == Map.Internal || m_Galleon == null || m_Galleon.Deleted)
                return false;

            Direction d = cannon.GetFacing();
            int xOffset = 0; int yOffset = 0;
            int cannonrange = cannon.Range;
            int currentRange = 0;
            Point3D pnt = cannon.Location;

            switch (d)
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

            while (currentRange++ <= cannonrange)
            {
                xOffset = xo;
                yOffset = yo;

                for (int i = -1; i <= 1; i++)
                {
                    Point3D newPoint;

                    if (xOffset == 0)
                        newPoint = new Point3D(pnt.X + (xOffset + i), pnt.Y + (yOffset * currentRange), pnt.Z);
                    else
                        newPoint = new Point3D(pnt.X + (xOffset * currentRange), pnt.Y + (yOffset + i), pnt.Z);

                    if (shootatboat)
                    {
                        BaseGalleon g = BaseGalleon.FindGalleonAt(newPoint, this.Map);

                        if (g != null && g == m_TargetBoat && g != Galleon)
                            return true;
                    }
                    else
                    {
                        if (focus == null)
                            return false;

                        if (newPoint.X == focus.X && newPoint.Y == focus.Y)
                        {
                            Console.WriteLine("Shooting: {0} at {1} / {2}", focus.Name, newPoint.X, newPoint.Y);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsInBattle()
        {
            if (Combatant != null)
                return true;

            foreach (Mobile mob in m_Crew)
            {
                if (mob.Alive && mob.Combatant != null)
                    return true;
            }
            return false;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AddHoldItem
        {
            get { return null; }
            set
            {
                string str = value;

                Type type = ScriptCompiler.FindTypeByName(str);

                if (type != null && (type == typeof(Item) || type.IsSubclassOf(typeof(Item))))
                {
                    Item item = Loot.Construct(type);

                    if (item != null)
                        HoldItem = item;
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item HoldItem
        {
            get { return null; }
            set
            {
                Item item = value;

                if (item != null)
                    AddItemToHold(item);
            }
        }

        public void AddItemToHold(Item item)
        {
            if (item == null)
                return;

            if (m_Galleon != null && m_Galleon.GalleonHold != null)
                m_Galleon.GalleonHold.DropItem(item);
            else
                item.Delete();
        }

        public BaseShipCaptain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Blockade);

            writer.Write((int)m_Zone);
            writer.Write(m_StopTime);
            writer.Write(m_Galleon);
            writer.Write(m_OnCourse);

            writer.Write(m_Crew.Count);
            foreach (Mobile mob in m_Crew)
                    writer.Write(mob);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Blockade = reader.ReadBool();
            m_Zone = (SpawnZone)reader.ReadInt();
            m_StopTime = reader.ReadDateTime();
            m_Galleon = reader.ReadItem() as BaseGalleon;
            m_OnCourse = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile mob = reader.ReadMobile() as Mobile;
                if (mob != null && !mob.Deleted && mob.Alive)
                    m_Crew.Add(mob);
            }

            if (!m_Blockade)
                ResumeCourseTimed(TimeSpan.FromSeconds(15), true);

            m_NextMoveCheck = DateTime.UtcNow;
            m_NextCannonShot = DateTime.UtcNow;
        }
    }
}