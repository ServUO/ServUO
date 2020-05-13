using Server.Items;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class BasePeerless : BaseCreature
    {
        private PeerlessAltar m_Altar;

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar
        {
            get
            {
                return m_Altar;
            }
            set
            {
                m_Altar = value;
            }
        }

        public override bool CanBeParagon => false;
        public virtual bool DropPrimer => true;
        public virtual bool GiveMLSpecial => true;

        public override bool Unprovokable => true;
        public virtual double ChangeCombatant => 0.3;

        public BasePeerless(Serial serial)
            : base(serial)
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            if (HasFireRing && Combatant != null && Alive && Hits > 0.8 * HitsMax && m_NextFireRing > Core.TickCount && Utility.RandomDouble() < FireRingChance)
                FireRing();

            if (CanSpawnHelpers && Combatant != null && Alive && CanSpawnWave())
                SpawnHelpers();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (DropPrimer)
            {
                SkillMasteryPrimer primer = SkillMasteryPrimer.GetRandom();
                List<DamageStore> rights = GetLootingRights();

                if (rights.Count > 0)
                {
                    Mobile m = rights[Utility.Random(rights.Count)].m_Mobile;

                    m.SendLocalizedMessage(1156209); // You have received a mastery primer!

                    if (m.Backpack == null || !m.Backpack.TryDropItem(m, primer, false))
                        m.BankBox.DropItem(primer);
                }
                else
                {
                    c.DropItem(primer);
                }
            }

            if (GivesMLMinorArtifact && 0.5 > Utility.RandomDouble())
            {
                MondainsLegacy.DropPeerlessMinor(c);
            }

            if (GiveMLSpecial)
            {
                if (Utility.RandomDouble() < 0.10)
                    c.DropItem(new HumanFeyLeggings());

                if (Utility.RandomDouble() < 0.025)
                    c.DropItem(new CrimsonCincture());

                if (0.05 > Utility.RandomDouble())
                {
                    switch (Utility.Random(32))
                    {
                        case 0: c.DropItem(new AssassinChest()); break;
                        case 1: c.DropItem(new AssassinArms()); break;
                        case 2: c.DropItem(new AssassinLegs()); break;
                        case 3: c.DropItem(new AssassinGloves()); break;
                        case 4: c.DropItem(new DeathChest()); break;
                        case 5: c.DropItem(new DeathArms()); break;
                        case 6: c.DropItem(new DeathLegs()); break;
                        case 7: c.DropItem(new DeathBoneHelm()); break;
                        case 8: c.DropItem(new DeathGloves()); break;
                        case 9: c.DropItem(new MyrmidonArms()); break;
                        case 10: c.DropItem(new MyrmidonLegs()); break;
                        case 11: c.DropItem(new MyrmidonGorget()); break;
                        case 12: c.DropItem(new MyrmidonChest()); break;
                        case 13: c.DropItem(new LeafweaveGloves()); break;
                        case 14: c.DropItem(new LeafweaveLegs()); break;
                        case 15: c.DropItem(new LeafweavePauldrons()); break;
                        case 16: c.DropItem(new PaladinGloves()); break;
                        case 17: c.DropItem(new PaladinGorget()); break;
                        case 18: c.DropItem(new PaladinArms()); break;
                        case 19: c.DropItem(new PaladinLegs()); break;
                        case 20: c.DropItem(new PaladinHelm()); break;
                        case 21: c.DropItem(new PaladinChest()); break;
                        case 22: c.DropItem(new HunterArms()); break;
                        case 23: c.DropItem(new HunterGloves()); break;
                        case 24: c.DropItem(new HunterLegs()); break;
                        case 25: c.DropItem(new HunterChest()); break;
                        case 26: c.DropItem(new GreymistArms()); break;
                        case 27: c.DropItem(new GreymistGloves()); break;
                        case 28: c.DropItem(new GreymistLegs()); break;
                        case 29: c.DropItem(new MalekisHonor()); break;
                        case 30: c.DropItem(new Feathernock()); break;
                        case 31: c.DropItem(new Swiftflight()); break;
                    }
                }
            }

            if (m_Altar != null)
                m_Altar.OnPeerlessDeath();
        }

        public BasePeerless(AIType aiType, FightMode fightMode, int rangePerception, int rangeFight, double activeSpeed, double passiveSpeed)
            : base(aiType, fightMode, rangePerception, rangeFight, activeSpeed, passiveSpeed)
        {
            m_NextFireRing = Core.TickCount + 10000;
            m_CurrentWave = MaxHelpersWaves;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(m_Altar);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Altar = reader.ReadItem() as PeerlessAltar;
        }

        #region Helpers		
        public virtual bool CanSpawnHelpers => false;
        public virtual int MaxHelpersWaves => 0;
        public virtual double SpawnHelpersChance => 0.05;

        private int m_CurrentWave;

        public int CurrentWave
        {
            get
            {
                return m_CurrentWave;
            }
            set
            {
                m_CurrentWave = value;
            }
        }

        public bool AllHelpersDead
        {
            get
            {
                if (m_Altar != null)
                    return m_Altar.AllHelpersDead();

                return true;
            }
        }

        public virtual bool CanSpawnWave()
        {
            if (MaxHelpersWaves > 0 && m_CurrentWave > 0)
            {
                double hits = (Hits / (double)HitsMax);
                double waves = (m_CurrentWave / (double)(MaxHelpersWaves + 1));

                if (hits < waves && Utility.RandomDouble() < SpawnHelpersChance)
                {
                    m_CurrentWave -= 1;
                    return true;
                }
            }

            return false;
        }

        public virtual void SpawnHelpers()
        {
        }

        public void SpawnHelper(BaseCreature helper, int range)
        {
            SpawnHelper(helper, GetSpawnPosition(range));
        }

        public void SpawnHelper(BaseCreature helper, int x, int y, int z)
        {
            SpawnHelper(helper, new Point3D(x, y, z));
        }

        public void SpawnHelper(BaseCreature helper, Point3D location)
        {
            if (helper == null)
                return;

            helper.Home = location;
            helper.RangeHome = 4;

            if (m_Altar != null)
                m_Altar.AddHelper(helper);

            helper.MoveToWorld(location, Map);
        }

        #endregion

        #region Fire Ring
        private static readonly int[] m_North = new int[]
        {
            -1, -1,
            1, -1,
            -1, 2,
            1, 2
        };

        private static readonly int[] m_East = new int[]
        {
            -1, 0,
            2, 0
        };

        public virtual bool HasFireRing => false;
        public virtual double FireRingChance => 1.0;

        private long m_NextFireRing = Core.TickCount;

        public virtual void FireRing()
        {
            for (int i = 0; i < m_North.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_North[i];
                p.Y += m_North[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E27, 50);
            }

            for (int i = 0; i < m_East.Length; i += 2)
            {
                Point3D p = Location;

                p.X += m_East[i];
                p.Y += m_East[i + 1];

                IPoint3D po = p as IPoint3D;

                SpellHelper.GetSurfaceTop(ref po);

                Effects.SendLocationEffect(po, Map, 0x3E31, 50);
            }

            m_NextFireRing = Core.TickCount + 10000;
        }
        #endregion
    }
}
