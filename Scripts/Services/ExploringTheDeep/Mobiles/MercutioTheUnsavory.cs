using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.Quests;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a mercutio corpse")]
    public class MercutioTheUnsavory : BaseCreature
    {
        private static readonly ArrayList m_Instances = new ArrayList();
        public static ArrayList Instances { get { return m_Instances; } }

        [Constructable]
        public MercutioTheUnsavory()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Instances.Add(this);

            this.Body = 0x190;
            this.Hue = Utility.RandomSkinHue();
            this.Name = "Mercutio";
            this.Title = "The Unsavory";
            this.Female = false;

            this.SetStr(1000, 1300);
            this.SetDex(101, 125);
            this.SetInt(61, 75);

            this.SetDamage(11, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 10, 15);
            this.SetResistance(ResistanceType.Fire, 10, 15);
            this.SetResistance(ResistanceType.Poison, 10, 15);
            this.SetResistance(ResistanceType.Energy, 10, 15);

            this.SetSkill(SkillName.Fencing, 106.0, 117.5);
            this.SetSkill(SkillName.Macing, 105.0, 117.5);
            this.SetSkill(SkillName.MagicResist, 50.0, 90.5);
            this.SetSkill(SkillName.Swords, 105.0, 117.5);
            this.SetSkill(SkillName.Parry, 105.0, 117.5);
            this.SetSkill(SkillName.Tactics, 105.0, 117.5);
            this.SetSkill(SkillName.Wrestling, 55.0, 87.5);

            this.Fame = 3000;
            this.Karma = -3000;

            this.AddImmovableItem(new MercutiosCutlass());
            this.AddItem(new ChainChest());
            this.AddItem(Loot.RandomShield());
            this.AddItem(new ShortPants(Utility.RandomNeutralHue()));
            this.AddItem(new Boots(Utility.RandomNeutralHue()));

            Utility.AssignRandomHair(this);

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }

        private void AddImmovableItem(Item item)
        {
            item.LootType = LootType.Blessed;
            AddItem(item);
        }

        public override bool ClickTitle { get { return false; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDeath(Container c)
        {
            List<DamageStore> rights = GetLootingRights();            

            foreach (Mobile m in rights.Select(x => x.m_Mobile).Distinct())
            {
                if (m is PlayerMobile)
                {
                    PlayerMobile pm = m as PlayerMobile;

                    if (pm.ExploringTheDeepQuest == ExploringTheDeepQuestChain.CollectTheComponent)
                    {
						Item item = new MercutiosCutlass();
						
                        if (m.Backpack == null || !m.Backpack.TryDropItem(m, item, false))
                        {
                            m.BankBox.DropItem(item);
                        }

                        m.SendLocalizedMessage(1154489); // You received a Quest Item!
                    }
                }
            }

            base.OnDeath(c);
        }

        public static MercutioTheUnsavory Spawn(Point3D platLoc, Map platMap)
        {
            if (m_Instances.Count > 0)
                return null;

            MercutioTheUnsavory creature = new MercutioTheUnsavory();
            creature.Home = platLoc;
            creature.RangeHome = 4;
            creature.MoveToWorld(platLoc, platMap);

            return creature;
        }

        public MercutioTheUnsavory(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
        }

        public class InternalSelfDeleteTimer : Timer
        {
            private MercutioTheUnsavory Mare;

            public InternalSelfDeleteTimer(Mobile p) : base(TimeSpan.FromMinutes(60))
            {
                Priority = TimerPriority.FiveSeconds;
                Mare = ((MercutioTheUnsavory)p);
            }
            protected override void OnTick()
            {
                if (Mare.Map != Map.Internal)
                {
                    Mare.Delete();
                    this.Stop();
                }
            }
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble())
                this.SpawnBrigand(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 >= Utility.RandomDouble())
                this.SpawnBrigand(defender);
        }

        #region Helpers
        public void SpawnBrigand(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            int brigands = 0;

            foreach (Mobile m in this.GetMobilesInRange(10))
            {
                if (m is Brigand)
                    ++brigands;
            }

            if (brigands < 16)
            {
                this.PlaySound(0x3D);

                int newBrigands = Utility.RandomMinMax(3, 6);

                for (int i = 0; i < newBrigands; ++i)
                {
                    BaseCreature brigand = new HumanBrigand();

                    brigand.Team = this.Team;

                    bool validLocation = false;
                    Point3D loc = this.Location;

                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = this.X + Utility.Random(3) - 1;
                        int y = this.Y + Utility.Random(3) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                            loc = new Point3D(x, y, this.Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    m_Instances.Add(brigand);
                    brigand.MoveToWorld(loc, map);
                    brigand.Combatant = target;
                }
            }
        }
        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Timer SelfDeleteTimer = new InternalSelfDeleteTimer(this);
            SelfDeleteTimer.Start();
        }
    }
}