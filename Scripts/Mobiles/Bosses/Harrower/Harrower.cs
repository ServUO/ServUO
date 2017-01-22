using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
    public class Harrower : BaseCreature
    {
        private int m_StatCap = Config.Get("PlayerCaps.TotalStatCap", 225);
        private static readonly SpawnEntry[] m_Entries = new SpawnEntry[]
        {
            new SpawnEntry(new Point3D(5242, 945, -40), new Point3D(1176, 2638, 0)), // Destard
            new SpawnEntry(new Point3D(5225, 798, 0), new Point3D(1176, 2638, 0)), // Destard
            new SpawnEntry(new Point3D(5556, 886, 30), new Point3D(1298, 1080, 0)), // Despise
            new SpawnEntry(new Point3D(5187, 615, 0), new Point3D(4111, 432, 5)), // Deceit
            new SpawnEntry(new Point3D(5319, 583, 0), new Point3D(4111, 432, 5)), // Deceit
            new SpawnEntry(new Point3D(5713, 1334, -1), new Point3D(2923, 3407, 8)), // Fire
            new SpawnEntry(new Point3D(5860, 1460, -2), new Point3D(2923, 3407, 8)), // Fire
            new SpawnEntry(new Point3D(5328, 1620, 0), new Point3D(5451, 3143, -60)), // Terathan Keep
            new SpawnEntry(new Point3D(5690, 538, 0), new Point3D(2042, 224, 14)), // Wrong
            new SpawnEntry(new Point3D(5609, 195, 0), new Point3D(514, 1561, 0)), // Shame
            new SpawnEntry(new Point3D(5475, 187, 0), new Point3D(514, 1561, 0)), // Shame
            new SpawnEntry(new Point3D(6085, 179, 0), new Point3D(4721, 3822, 0)), // Hythloth
            new SpawnEntry(new Point3D(6084, 66, 0), new Point3D(4721, 3822, 0)), // Hythloth
            /*new SpawnEntry(new Point3D(5499, 2003, 0), new Point3D(2499, 919, 0)), // Covetous*/
            new SpawnEntry(new Point3D(5579, 1858, 0), new Point3D(2499, 919, 0))// Covetous
        };
        private static readonly ArrayList m_Instances = new ArrayList();
        private static readonly double[] m_Offsets = new double[]
        {
            Math.Cos(000.0 / 180.0 * Math.PI), Math.Sin(000.0 / 180.0 * Math.PI),
            Math.Cos(040.0 / 180.0 * Math.PI), Math.Sin(040.0 / 180.0 * Math.PI),
            Math.Cos(080.0 / 180.0 * Math.PI), Math.Sin(080.0 / 180.0 * Math.PI),
            Math.Cos(120.0 / 180.0 * Math.PI), Math.Sin(120.0 / 180.0 * Math.PI),
            Math.Cos(160.0 / 180.0 * Math.PI), Math.Sin(160.0 / 180.0 * Math.PI),
            Math.Cos(200.0 / 180.0 * Math.PI), Math.Sin(200.0 / 180.0 * Math.PI),
            Math.Cos(240.0 / 180.0 * Math.PI), Math.Sin(240.0 / 180.0 * Math.PI),
            Math.Cos(280.0 / 180.0 * Math.PI), Math.Sin(280.0 / 180.0 * Math.PI),
            Math.Cos(320.0 / 180.0 * Math.PI), Math.Sin(320.0 / 180.0 * Math.PI),
        };
        private bool m_TrueForm;
        private Item m_GateItem;
        private List<HarrowerTentacles> m_Tentacles;

        Dictionary<Mobile, int> m_DamageEntries;
        [Constructable]
        public Harrower()
            : base(AIType.AI_Mage, FightMode.Closest, 18, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            this.Name = "the harrower";
            this.BodyValue = 146;

            this.SetStr(900, 1000);
            this.SetDex(125, 135);
            this.SetInt(1000, 1200);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 60;

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 60, 80);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 60, 80);
            this.SetResistance(ResistanceType.Energy, 60, 80);

            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);
            this.SetSkill(SkillName.Tactics, 90.2, 110.0);
            this.SetSkill(SkillName.MagicResist, 120.2, 160.0);
            this.SetSkill(SkillName.Magery, 120.0);
            this.SetSkill(SkillName.EvalInt, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0);

            this.m_Tentacles = new List<HarrowerTentacles>();
        }

        public Harrower(Serial serial)
            : base(serial)
        {
            m_Instances.Add(this);
        }

        public static ArrayList Instances
        {
            get
            {
                return m_Instances;
            }
        }
        public static bool CanSpawn
        {
            get
            {
                return (m_Instances.Count == 0);
            }
        }
        public Type[] UniqueList
        {
            get
            {
                return new Type[] { typeof(AcidProofRobe) };
            }
        }
        public Type[] SharedList
        {
            get
            {
                return new Type[] { typeof(TheRobeOfBritanniaAri) };
            }
        }
        public Type[] DecorativeList
        {
            get
            {
                return new Type[] { typeof(EvilIdolSkull), typeof(SkullPole) };
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool Unprovokable
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                return this.m_TrueForm ? 65000 : 30000;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get
            {
                return 5000;
            }
        }
        public override bool DisallowAllMoves
        {
            get
            {
                return this.m_TrueForm;
            }
        }

        public override bool TeleportsTo { get { return true; } }

        public static Harrower Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            SpawnEntry entry = m_Entries[Utility.Random(m_Entries.Length)];

            Harrower harrower = new Harrower();

            harrower.MoveToWorld(entry.m_Location, Map.Felucca);

            harrower.m_GateItem = new HarrowerGate(harrower, platLoc, platMap, entry.m_Entrance, Map.Felucca);

            return harrower;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss, 2);
            this.AddLoot(LootPack.Meager);
        }

        public void Morph()
        {
            if (this.m_TrueForm)
                return;

            this.m_TrueForm = true;

            this.Name = "the true harrower";
            this.BodyValue = 780;
            this.Hue = 0x497;

            this.Hits = this.HitsMax;
            this.Stam = this.StamMax;
            this.Mana = this.ManaMax;

            this.ProcessDelta();

            this.Say(1049499); // Behold my true form!

            Map map = this.Map;

            if (map != null)
            {
                for (int i = 0; i < m_Offsets.Length; i += 2)
                {
                    double rx = m_Offsets[i];
                    double ry = m_Offsets[i + 1];

                    int dist = 0;
                    bool ok = false;
                    int x = 0, y = 0, z = 0;

                    while (!ok && dist < 10)
                    {
                        int rdist = 10 + dist;

                        x = this.X + (int)(rx * rdist);
                        y = this.Y + (int)(ry * rdist);
                        z = map.GetAverageZ(x, y);

                        if (!(ok = map.CanFit(x, y, this.Z, 16, false, false)))
                            ok = map.CanFit(x, y, z, 16, false, false);

                        if (dist >= 0)
                            dist = -(dist + 1);
                        else
                            dist = -(dist - 1);
                    }

                    if (!ok)
                        continue;

                    HarrowerTentacles spawn = new HarrowerTentacles(this);

                    spawn.Team = this.Team;

                    spawn.MoveToWorld(new Point3D(x, y, z), map);

                    this.m_Tentacles.Add(spawn);
                }
            }
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_TrueForm);
            writer.Write(this.m_GateItem);
            writer.WriteMobileList<HarrowerTentacles>(this.m_Tentacles);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_TrueForm = reader.ReadBool();
                        this.m_GateItem = reader.ReadItem();
                        this.m_Tentacles = reader.ReadStrongMobileList<HarrowerTentacles>();

                        break;
                    }
            }
        }

        public void GivePowerScrolls()
        {
            List<Mobile> toGive = new List<Mobile>();
            List<DamageStore> rights = GetLootingRights();

            for (int i = rights.Count - 1; i >= 0; --i)
            {
                DamageStore ds = rights[i];

                if (ds.m_HasRight)
                    toGive.Add(ds.m_Mobile);
            }

            if (toGive.Count == 0)
                return;

            // Randomize
            for (int i = 0; i < toGive.Count; ++i)
            {
                int rand = Utility.Random(toGive.Count);
                Mobile hold = toGive[i];
                toGive[i] = toGive[rand];
                toGive[rand] = hold;
            }

            for (int i = 0; i < ChampionSystem.StatScrollAmount; ++i)
            {
                Mobile m = toGive[i % toGive.Count];

                m.SendLocalizedMessage(1049524); // You have received a scroll of power!
                m.AddToBackpack(new StatCapScroll(m_StatCap + RandomStatScrollLevel()));

                if (m is PlayerMobile)
                {
                    PlayerMobile pm = (PlayerMobile)m;

                    for (int j = 0; j < pm.JusticeProtectors.Count; ++j)
                    {
                        Mobile prot = (Mobile)pm.JusticeProtectors[j];

                        if (prot.Map != m.Map || prot.Kills >= 5 || prot.Criminal || !JusticeVirtue.CheckMapRegion(m, prot))
                            continue;

                        int chance = 0;

                        switch ( VirtueHelper.GetLevel(prot, VirtueName.Justice) )
                        {
                            case VirtueLevel.Seeker:
                                chance = 60;
                                break;
                            case VirtueLevel.Follower:
                                chance = 80;
                                break;
                            case VirtueLevel.Knight:
                                chance = 100;
                                break;
                        }

                        if (chance > Utility.Random(100))
                        {
                            prot.SendLocalizedMessage(1049368); // You have been rewarded for your dedication to Justice!
                            prot.AddToBackpack(new StatCapScroll(m_StatCap + RandomStatScrollLevel()));
                        }
                    }
                }
            }
        }

		private static int RandomStatScrollLevel()
		{
			double random = Utility.RandomDouble();

			if (0.1 >= random)
				return 25;
			else if (0.25 >= random)
				return 20;
			else if (0.45 >= random)
				return 15;
			else if (0.70 >= random)
				return 10;
			return 5;
		}

		public override bool OnBeforeDeath()
        {
            if (this.m_TrueForm)
            {
                List<DamageStore> rights = GetLootingRights();

                for (int i = rights.Count - 1; i >= 0; --i)
                {
                    DamageStore ds = rights[i];

                    if (ds.m_HasRight && ds.m_Mobile is PlayerMobile)
                        PlayerMobile.ChampionTitleInfo.AwardHarrowerTitle((PlayerMobile)ds.m_Mobile);
                }

                if (!this.NoKillAwards)
                {
                    this.GivePowerScrolls();

                    Map map = this.Map;

					GoldShower.DoForHarrower(Location, Map);

                    this.m_DamageEntries = new Dictionary<Mobile, int>();

                    for (int i = 0; i < this.m_Tentacles.Count; ++i)
                    {
                        Mobile m = this.m_Tentacles[i];

                        if (!m.Deleted)
                            m.Kill();

                        this.RegisterDamageTo(m);
                    }

                    this.m_Tentacles.Clear();

                    this.RegisterDamageTo(this);
                    this.AwardArtifact(this.GetArtifact());

                    if (this.m_GateItem != null)
                        this.m_GateItem.Delete();
                }

                return base.OnBeforeDeath();
            }
            else
            {
                this.Morph();
                return false;
            }
        }

        public virtual void RegisterDamageTo(Mobile m)
        {
            if (m == null)
                return;

            foreach (DamageEntry de in m.DamageEntries)
            {
                Mobile damager = de.Damager;

                Mobile master = damager.GetDamageMaster(m);

                if (master != null)
                    damager = master;

                this.RegisterDamage(damager, de.DamageGiven);
            }
        }

        public void RegisterDamage(Mobile from, int amount)
        {
            if (from == null || !from.Player)
                return;

            if (this.m_DamageEntries.ContainsKey(from))
                this.m_DamageEntries[from] += amount;
            else
                this.m_DamageEntries.Add(from, amount);

            from.SendMessage(String.Format("Total Damage: {0}", this.m_DamageEntries[from]));
        }

        public void AwardArtifact(Item artifact)
        {
            if (artifact == null)
                return;

            int totalDamage = 0;

            Dictionary<Mobile, int> validEntries = new Dictionary<Mobile, int>();

            foreach (KeyValuePair<Mobile, int> kvp in this.m_DamageEntries)
            {
                if (this.IsEligible(kvp.Key, artifact))
                {
                    validEntries.Add(kvp.Key, kvp.Value);
                    totalDamage += kvp.Value;
                }
            }

            int randomDamage = Utility.RandomMinMax(1, totalDamage);

            totalDamage = 0;

            foreach (KeyValuePair<Mobile, int> kvp in validEntries)
            {
                totalDamage += kvp.Value;

                if (totalDamage >= randomDamage)
                {
                    this.GiveArtifact(kvp.Key, artifact);
                    return;
                }
            }

            artifact.Delete();
        }

        public void GiveArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

			to.PlaySound(0x5B4);

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                artifact.Delete();
            else
                to.SendLocalizedMessage(1062317); // For your valor in combating the fallen beast, a special artifact has been bestowed on you.
        }

        public bool IsEligible(Mobile m, Item Artifact)
        {
            return m.Player && m.Alive && m.InRange(this.Location, 32) && m.Backpack != null && m.Backpack.CheckHold(m, Artifact, false);
        }

        public Item GetArtifact()
        {
            double random = Utility.RandomDouble();
            if (0.05 >= random)
                return this.CreateArtifact(this.UniqueList);
            else if (0.15 >= random)
                return this.CreateArtifact(this.SharedList);
            else if (0.30 >= random)
                return this.CreateArtifact(this.DecorativeList);
            return null;
        }

        public Item CreateArtifact(Type[] list)
        {
            if (list.Length == 0)
                return null;

            int random = Utility.Random(list.Length);
			
            Type type = list[random];

            return Loot.Construct(type);
        }

        private class SpawnEntry
        {
            public readonly Point3D m_Location;
            public readonly Point3D m_Entrance;
            public SpawnEntry(Point3D loc, Point3D ent)
            {
                this.m_Location = loc;
                this.m_Entrance = ent;
            }
        }
    }
}