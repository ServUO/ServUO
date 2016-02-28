using Server;
using System;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.Despise
{
    public class DespiseController : Item
    {
        public static void Initialize()
        {
            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.OnEnterRegion += new OnEnterRegionEventHandler(OnEnterRegion);
        }

        private static DespiseController m_Instance;
        public static DespiseController Instance { get { return m_Instance; } set { m_Instance = value; } }

        private bool m_Enabled;
        private bool m_Sequencing;
        private DateTime m_NextBossEncounter;
        private DespiseBoss m_Boss;
        private DateTime m_DeadLine;
        private Alignment m_SequenceAlignment;

        private Timer m_Timer;
        private Timer m_SequenceTimer;
        private Timer m_CleanupTimer;

        private DespiseRegion m_GoodRegion;
        private DespiseRegion m_EvilRegion;
        private DespiseRegion m_LowerRegion;
        private DespiseRegion m_StartRegion;

        public Region GoodRegion { get { return m_GoodRegion; } }
        public Region EvilRegion { get { return m_EvilRegion; } }
        public Region LowerRegion { get { return m_LowerRegion; } }
        public Region StartRegion { get { return m_StartRegion; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get { return m_Enabled; }
            set
            {
                if (m_Enabled != value)
                {
                    m_Enabled = value;

                    if (m_Enabled)
                        BeginTimer();
                    else
                        EndTimer();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Sequencing { get { return m_Sequencing; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextBossEncounter
        {
            get { return m_NextBossEncounter; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DespiseBoss Boss
        {
            get { return m_Boss; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeadLine
        {
            get { return m_DeadLine; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Alignment SequenceAlignment
        {
            get { return m_SequenceAlignment; }
        }

        private List<DespiseCreature> m_EvilArmy = new List<DespiseCreature>();
        private List<DespiseCreature> m_GoodArmy = new List<DespiseCreature>();

        public List<DespiseCreature> EvilArmy { get { return m_EvilArmy; } }
        public List<DespiseCreature> GoodArmy { get { return m_GoodArmy; } }

        private List<Mobile> m_ToTransport = new List<Mobile>();

        private readonly TimeSpan EncounterCheckDuration = TimeSpan.FromMinutes(10);
        private readonly TimeSpan DeadLineDuration = TimeSpan.FromMinutes(90);

        public bool IsInSequence { get { return m_SequenceTimer != null || m_CleanupTimer != null; } }

        public DespiseController()
            : base(3806)
        {
            Movable = false;
            Visible = false;

            m_Enabled = true;
            m_Instance = this;

            m_NextBossEncounter = DateTime.Now;
            m_Boss = null;

            if(m_Enabled)
                BeginTimer();

            CreateSpawners();
        }

        public static WispOrb GetWispOrb(Mobile from)
        {
            foreach (WispOrb orb in WispOrb.Orbs)
            {
                if (orb != null && !orb.Deleted && orb.Owner == from)
                    return orb;
            }

            return null;
        }

        private void BeginTimer()
        {
            EndTimer();

            m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(OnTick));
            m_Timer.Priority = TimerPriority.OneSecond;

            m_LowerRegion = new DespiseRegion("Despise Lower", m_LowerLevelBounds, true);
            m_EvilRegion = new DespiseRegion("Despise Evil", m_EvilBounds);
            m_GoodRegion = new DespiseRegion("Despise Good", m_GoodBounds);
            m_StartRegion = new DespiseRegion("Despise Start", new Rectangle2D[] { new Rectangle2D(5568, 623, 22, 20) } );
        }

        private void EndTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_LowerRegion != null)
                m_LowerRegion.Unregister();

            if (m_EvilRegion != null)
                m_EvilRegion.Unregister();

            if (m_GoodRegion != null)
                m_GoodRegion.Unregister();

            if (m_StartRegion != null)
                m_StartRegion.Unregister();

            m_LowerRegion = null;
            m_EvilRegion = null;
            m_GoodRegion = null;
            m_StartRegion = null;
        }

        /*private void BuildSpawners()
        {
            if(m_Spawners == null)
                m_Spawners = new List<XmlSpawner>();
            else
                m_Spawners.Clear();

            foreach (Sector sector in m_LowerRegion.Sectors)
            {
                if (sector == null || sector.Items == null)
                    continue;

                List<Item> list = new List<Item>(sector.Items);

                foreach (Item item in list)
                {
                    if(item is XmlSpawner && item.Name != null && item.Name.ToLower().IndexOf("despiserevamp") >= 0)
                    {
                        m_Spawners.Add((XmlSpawner)item);
                    }
                }
            }

            Console.WriteLine("Loaded {0} spawners for despise lower level", m_Spawners.Count);
        }*/

        private void OnTick()
        {
            if (m_NextBossEncounter == DateTime.MinValue || m_NextBossEncounter > DateTime.Now)
                return;

            int good = GetArmyPower(Alignment.Good);
            int evil = GetArmyPower(Alignment.Evil);
            Alignment strongest;

            if (good == 0 && evil == 0)
            {
                m_NextBossEncounter = DateTime.Now + EncounterCheckDuration;
                return;
            }

            if (good > evil) strongest = Alignment.Good;
            else if (good < evil) strongest = Alignment.Evil;
            else strongest = 0.5 > Utility.RandomDouble() ? Alignment.Good : Alignment.Evil;

            List<Mobile> players = new List<Mobile>();
            players.AddRange(m_GoodRegion.GetPlayers());
            players.AddRange(m_EvilRegion.GetPlayers());
            players.AddRange(m_StartRegion.GetPlayers());

            foreach (Mobile m in players)
            {
                if (!m.Player)
                    continue;

                WispOrb orb = GetWispOrb(m);
                m.PlaySound(0x66C);

                if (orb == null || orb.Alignment != strongest)
                {
                    m.SendLocalizedMessage(strongest != Alignment.Neutral ? 1153334 : 1153333);
                    // The Call to Arms has sounded, but your forces are not yet strong enough to heed it.
                    // Your enemy forces are stronger, and they have been called to battle.
                }
                else if (orb != null && orb.Alignment == strongest)
                {
                    m.SendLocalizedMessage(1153332); // The Call to Arms has sounded. The forces of your alignment are strong, and you have been called to battle!

                    if (orb.Conscripted)
                    {
                        m.SendLocalizedMessage(1153337); // You will be teleported into the depths of the dungeon within 60 seconds to heed the Call to Arms, unless you release your conscripted creature or it dies.
                        m_ToTransport.Add(m);
                    }
                    else
                        m.SendLocalizedMessage(1153338); // You have under 60 seconds to conscript a creature to answer the Call to Arms, or you will not be summoned for the battle.
                }
            }

            m_SequenceAlignment = strongest;

            Timer.DelayCall(TimeSpan.FromSeconds(60), new TimerCallback(BeginSequence));
            m_NextBossEncounter = DateTime.MinValue;
            m_Sequencing = true;
        }

        public int GetArmyPower(Alignment alignment)
        {
            int power = 0;
            foreach (WispOrb orb in WispOrb.Orbs)
            {
                if (orb.Conscripted && orb.Alignment == alignment)
                    power += orb.GetArmyPower();
            }
            return power;
        }

        public void TryAddToArmy(WispOrb orb)
        {
            if (orb != null && orb.Owner != null && m_Sequencing && orb.Alignment == m_SequenceAlignment && !m_ToTransport.Contains(orb.Owner))
                m_ToTransport.Add(orb.Owner);
        }

        #region Spawner Stuff
        private List<XmlSpawner> m_GoodSpawners;
        private List<XmlSpawner> m_EvilSpawners;

        private void CreateSpawners()
        {
            Console.Write("Creating Despise Revamp Spawners...");

            m_GoodSpawners = new List<XmlSpawner>();
            m_EvilSpawners = new List<XmlSpawner>();

            foreach (Sector sector in m_LowerRegion.Sectors)
            {
                foreach (Item item in sector.Items)
                {
                    if (item is XmlSpawner && item.Name != null && item.Name.ToLower().IndexOf("despiserevamped") >= 0)
                    {
                        if (item.Name.ToLower().IndexOf("despiserevamped good") >= 0)
                            m_GoodSpawners.Add((XmlSpawner)item);
                        if (item.Name.ToLower().IndexOf("despiserevamped evil") >= 0)
                            m_EvilSpawners.Add((XmlSpawner)item); 
                    }
                }
            }
            
            Console.Write("Done.");
            Console.WriteLine("Added {0} Evil spawners, and {1} Good Spawners", m_EvilSpawners.Count, m_GoodSpawners.Count);
        }

        private void ResetSpawners(bool reset)
        {
            if (reset)
            {
                foreach (XmlSpawner spawner in m_EvilSpawners)
                    if (spawner.Running)
                        spawner.DoReset = true;

                foreach (XmlSpawner spawner in m_GoodSpawners)
                    if (spawner.Running)
                        spawner.DoReset = true;
            }
            else
            {
                List<XmlSpawner> useList;

                if (m_SequenceAlignment == Alignment.Good)
                    useList = m_EvilSpawners;
                else
                    useList = m_GoodSpawners;

                if (useList == null)
                    return;

                foreach (XmlSpawner spawner in useList)
                    spawner.DoRespawn = true;
            }
            /*string lookfor = "good";

            if(m_SequenceAlignment == Alignment.Good)
                lookfor = "evil";

            foreach (XmlSpawner spawner in m_Spawners)
            {
                if (reset && spawner.Running)
                    spawner.DoReset = true;
                else if (spawner.Name != null && spawner.Name.ToLower().IndexOf(lookfor) >= 0)
                    spawner.DoRespawn = true;
            }*/
        }
        #endregion

        #region Instance Sequence

        private void BeginSequence()
        {
            m_Sequencing = false;

            if (m_ToTransport.Count == 0)
            {
                m_NextBossEncounter = DateTime.Now + EncounterCheckDuration;
                m_SequenceAlignment = Alignment.Neutral;
                return;
            }

            if (m_SequenceAlignment == Alignment.Good)
                m_Boss = new AndrosTheDreadLord();
            else
                m_Boss = new AdrianTheGloriousLord();

            ResetSpawners(false);

            m_Boss.MoveToWorld(BossLocation, Map.Trammel);
            m_DeadLine = DateTime.Now + DeadLineDuration;
            //m_NextBossEncounter = DateTime.MinValue;

            BeginSequenceTimer();
            KickFromBossRegion(false);

            Timer.DelayCall(TimeSpan.FromSeconds(60), new TimerCallback(TransportPlayers));

            Timer.DelayCall(TimeSpan.FromSeconds(12), new TimerStateCallback(SendReadyMessage_Callback), 1153339); // You have been called to assist in a fight of good versus evil. Fight your way to the Lake, and defeat the enemy overlord and its lieutenants!
            Timer.DelayCall(TimeSpan.FromSeconds(24), new TimerStateCallback(SendReadyMessage_Callback), 1153340); // The Overlord is shielded from all attacks by players, but not by creatures possessed by Wisp Orbs. You must protect your controlled creature as it fights.
            Timer.DelayCall(TimeSpan.FromSeconds(36), new TimerStateCallback(SendReadyMessage_Callback), 1153341); // The Lieutenants are vulnerable to your attacks. If you die during this battle, your possessed creature will fall. Furthermore, your ghost and your corpse will be teleported back to your home base.
        }

        private void EndSequence()
        {
            if (m_Boss != null && !m_Boss.Deleted)
                m_Boss.Delete();

            m_Boss = null;
            EndCleanupTimer();
            KickFromBossRegion(false);
            m_SequenceAlignment = Alignment.Neutral;

            m_DeadLine = DateTime.MinValue;
            m_ToTransport.Clear();

            ResetSpawners(true);

            m_NextBossEncounter = DateTime.Now + EncounterCheckDuration;
        }

        private void OnSequenceTick()
        {
            if (m_SequenceTimer != null && m_DeadLine < DateTime.Now && m_LowerRegion != null)
            {
                EndSequenceTimer();
                SendRegionMessage(m_LowerRegion, 1153348); // You were unable to defeat the enemy overlord in the time allotted. He has activated a Doom Spell!

                /*foreach (Mobile m in m_LowerRegion.GetMobiles())
                {
                    if (m is DespiseCreature && ((DespiseCreature)m).Orb != null && ((DespiseCreature)m).Controlled && m.Alive)
                        m.Kill();
                }*/

                Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(EndSequence));
            }
        }

        public void OnBossSlain()
        {
            EndSequenceTimer();
            SendRegionMessage(m_LowerRegion, 1153343); // The battle has ended. The battlefield will be cleared in five minutes and you will be returned to your home base at that time.

            BeginCleanupTimer();
        }

        private void SendRegionMessage(DespiseRegion region, int cliloc)
        {
            if (region != null)
            {
                foreach (Mobile m in region.GetPlayers())
                    m.SendLocalizedMessage(cliloc);
            }
        }

        private void KickFromBossRegion(bool deletepet)
        {
            if (m_LowerRegion == null)
                return;

            List<Mobile> mobiles = m_LowerRegion.GetPlayers();
            Rectangle2D bounds = m_SequenceAlignment == Alignment.Evil ? EvilKickBounds : GoodKickBounds;

            foreach (Mobile m in mobiles)
            {
                WispOrb orb = GetWispOrb(m);
                Point3D p = GetRandomLoc(bounds);

                m.MoveToWorld(p, Map.Trammel);

                if (orb != null && deletepet)
                {
                    if (orb.Pet != null)
                    {
                        orb.Pet.Delete();
                        orb.Pet = null;
                    }

                    orb.Delete();
                    m.SendLocalizedMessage(1153312); // The Wisp Orb dissolves into aether.
                }
                else  if (orb != null && orb.Pet != null && orb.Pet.Alive)
                    orb.Pet.MoveToWorld(p, Map.Trammel);

                m.SendLocalizedMessage(1153346); // You are summoned back to your stronghold.
            }
        }

        private void TransportPlayers()
        {
            List<Mobile> list = new List<Mobile>(m_ToTransport);

            foreach (Mobile m in list)
            {
                WispOrb orb = GetWispOrb(m);
                if (orb == null || orb.Deleted || !orb.Conscripted || m.Region == null || !m.Region.IsPartOf(typeof(DespiseRegion)))
                    m_ToTransport.Remove(m);
            }

            if (m_ToTransport.Count == 0)
            {
                EndSequenceTimer();
                EndSequence();
            }
            else
            {
                foreach (Mobile m in m_ToTransport)
                {
                    if (m != null && m.Region != null && m.Region.IsPartOf(typeof(DespiseRegion)))
                    {
                        WispOrb orb = GetWispOrb(m);

                        if (orb != null && orb.Pet != null && orb.Pet.Alive)
                        {
                            Point3D p = GetRandomLoc(BossEntranceLocation);
                            m.MoveToWorld(p, Map.Trammel);
                            orb.Pet.MoveToWorld(p, Map.Trammel);

                            m.SendLocalizedMessage(1153280, "You!");
                            orb.Anchor = m;
                            orb.Pet.ControlTarget = m;
                            orb.Pet.ControlOrder = OrderType.Follow;
                        }
                    }
                }
            }
        }

        private Point3D GetRandomLoc(Rectangle2D rec)
        {
            Point3D p = new Point3D(rec.X, rec.Y, this.Map.GetAverageZ(rec.X, rec.Y));

            for (int i = 0; i < 50; i++)
            {
                int x = Utility.RandomMinMax(rec.X, rec.X + rec.Width);
                int y = Utility.RandomMinMax(rec.Y, rec.Y + rec.Height);
                int z = Map.Trammel.GetAverageZ(x, y);

                if (Map.Trammel.CanSpawnMobile(x, y, z))
                {
                    p = new Point3D(x, y, z);
                    break;
                }
            }

            return p;
        }

        public void BeginSequenceTimer()
        {
            EndSequenceTimer();

            m_SequenceTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), new TimerCallback(OnSequenceTick));
            m_SequenceTimer.Start();
        }

        public void EndSequenceTimer()
        {
            if (m_SequenceTimer != null)
            {
                m_SequenceTimer.Stop();
                m_SequenceTimer = null;
            }
        }

        public void BeginCleanupTimer()
        {
            EndCleanupTimer();
            m_CleanupTimer = Timer.DelayCall(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5), new TimerCallback(EndSequence));
            m_CleanupTimer.Start();

            foreach (Mobile m in m_LowerRegion.GetMobiles())
            {
                if (m is DespiseCreature && ((DespiseCreature)m).Orb != null)
                {
                    m.Delete();
                }
            }
        }

        public void EndCleanupTimer()
        {
            if (m_CleanupTimer != null)
            {
                m_CleanupTimer.Stop();
                m_CleanupTimer = null;
            }
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;

            DespiseController controller = m_Instance;

            if (controller != null && controller.LowerRegion != null)
            {
                if (from.Region != null && from.Region.IsPartOf(controller.LowerRegion) && !controller.IsInSequence)
                {
                    WispOrb orb = GetWispOrb(from);
                    Rectangle2D bounds = EvilKickBounds;

                    if (orb != null && orb.Alignment == Alignment.Good)
                        bounds = GoodKickBounds;

                    while (true)
                    {
                        int x = Utility.RandomMinMax(bounds.X, bounds.X + bounds.Width);
                        int y = Utility.RandomMinMax(bounds.Y, bounds.Y + bounds.Height);
                        int z = Map.Trammel.GetAverageZ(x, y);

                        if (Map.Trammel.CanSpawnMobile(x, y, z))
                        {
                            from.MoveToWorld(new Point3D(x, y, z), Map.Trammel);
                            if (orb != null && orb.Pet != null && orb.Pet.Alive)
                                orb.Pet.MoveToWorld(new Point3D(x, y, z), Map.Trammel);
                            break;
                        }
                    }
                }
            }
        }

        public static void OnEnterRegion(OnEnterRegionEventArgs e)
        {
            WispOrb orb = GetWispOrb(e.From);

            if (orb != null && !Region.Find(e.From.Location, e.From.Map).IsPartOf(typeof(DespiseRegion)))
            {
                Timer.DelayCall(orb.Delete);
            }
        }

        private void SendReadyMessage_Callback(object o)
        {
            int cliloc = (int)o;
            foreach (Mobile m in m_ToTransport)
                m.SendLocalizedMessage(cliloc);
        }

        #endregion

        #region Location Defs

        public static Rectangle2D[] EvilBounds { get { return m_EvilBounds; } }
        private static Rectangle2D[] m_EvilBounds = new Rectangle2D[]
		{
			new Rectangle2D(5381, 644, 149, 120)
		};

        public static Rectangle2D[] GoodBounds { get { return m_GoodBounds; } }
        private static Rectangle2D[] m_GoodBounds = new Rectangle2D[]
		{
			new Rectangle2D(5380, 515, 134, 121)
		};

        public static Rectangle2D[] LowerLevelBounds { get { return m_LowerLevelBounds; } }
        private static Rectangle2D[] m_LowerLevelBounds = new Rectangle2D[]
		{
			new Rectangle2D(5379, 771, 247, 250)
		};

        private static Rectangle2D EvilKickBounds = new Rectangle2D(5500, 571, 20, 5);
        private static Rectangle2D GoodKickBounds = new Rectangle2D(5484, 567, 15, 8);
        private static Rectangle2D BossEntranceLocation = new Rectangle2D(5391, 855, 13, 15);

        private static Point3D BossLocation = new Point3D(5556, 823, 45);

        #endregion

        #region Despise Points
        private Dictionary<Mobile, int> m_PointsTable = new Dictionary<Mobile, int>();
        public Dictionary<Mobile, int> PointsTable { get { return m_PointsTable; } }

        public void AddDespisePoints(Mobile from, PutridHeart heart)
        {
            if (!m_PointsTable.ContainsKey(from))
                m_PointsTable[from] = 0;

            int amount = heart.Amount;

            m_PointsTable[from] += amount;
            from.SendLocalizedMessage(1153423, amount.ToString()); // You have gained ~1_AMT~ Dungeon Crystal Points of Despise.
            heart.Delete();
        }

        public int GetDespisePoints(Mobile from)
        {
            if (!m_PointsTable.ContainsKey(from))
                return 0;

            return m_PointsTable[from];
        }

        public void DeductDespisePoints(Mobile from, int points)
        {
            if (m_PointsTable.ContainsKey(from))
                m_PointsTable[from] -= points;
        }
        #endregion

        public DespiseController(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Enabled);
            writer.Write(m_NextBossEncounter);
            writer.Write(m_Boss);
            writer.Write(m_DeadLine);
            writer.Write((int)m_SequenceAlignment);

            writer.Write(m_GoodSpawners.Count);
            foreach (XmlSpawner spawner in m_GoodSpawners)
                writer.Write(spawner);

            writer.Write(m_EvilSpawners.Count);
            foreach (XmlSpawner spawner in m_EvilSpawners)
                writer.Write(spawner);

            writer.Write(m_PointsTable.Count);
            foreach (KeyValuePair<Mobile, int> kvp in m_PointsTable)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

            m_EvilSpawners = new List<XmlSpawner>();
            m_GoodSpawners = new List<XmlSpawner>();

            m_Instance = this;
			m_Enabled = reader.ReadBool();
			m_NextBossEncounter = reader.ReadDateTime();
			m_Boss = reader.ReadMobile() as DespiseBoss;
			m_DeadLine = reader.ReadDateTime();
			m_SequenceAlignment = (Alignment)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                XmlSpawner spawner = reader.ReadItem() as XmlSpawner;
                if (spawner != null)
                    m_GoodSpawners.Add(spawner);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                XmlSpawner spawner = reader.ReadItem() as XmlSpawner;
                if (spawner != null)
                    m_EvilSpawners.Add(spawner);
            }

            count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                Mobile m = reader.ReadMobile();
                int points = reader.ReadInt();

                if (m != null && points > 0)
                    m_PointsTable[m] = points;
            }

			if(!m_Enabled)
				return;
				
			BeginTimer();
			
			if(m_DeadLine > DateTime.Now)
			{
				if(m_Boss != null && m_Boss.Alive)
				{
					BeginSequenceTimer();
					return;
				}
			}
			else if (m_DeadLine != DateTime.MinValue)
			{
				BeginCleanupTimer();
				return;
			}
			
			EndSequence();
		}
    }
}