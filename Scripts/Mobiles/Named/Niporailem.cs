using Server.Items;
using Server.Mobiles;
using Server.Network;
using System;

namespace Server.Mobiles
{
    [CorpseName("a Niporailem corpse")]
    public class Niporailem : BaseSABoss
    {
        public override Type[] UniqueSAList => new Type[]
        {
          typeof(HelmOfVillainousEpiphany),
          typeof(GorgetOfVillainousEpiphany),
          typeof(BreastplateOfVillainousEpiphany),
          typeof(ArmsOfVillainousEpiphany),
          typeof(GauntletsOfVillainousEpiphany),
          typeof(LegsOfVillainousEpiphany),
          typeof(KiltOfVillainousEpiphany),
          typeof(EarringsOfVillainousEpiphany),
          typeof(GargishBreastplateOfVillainousEpiphany),
          typeof(GargishArmsOfVillainousEpiphany),
          typeof(NecklaceOfVillainousEpiphany),
          typeof(GargishLegsOfVillainousEpiphany),
          typeof(HelmOfVirtuousEpiphany),
          typeof(GorgetOfVirtuousEpiphany),
          typeof(BreastplateOfVirtuousEpiphany),
          typeof(ArmsOfVirtuousEpiphany),
          typeof(GauntletsOfVirtuousEpiphany),
          typeof(LegsOfVirtuousEpiphany),
          typeof(KiltOfVirtuousEpiphany),
          typeof(EarringsOfVirtuousEpiphany),
          typeof(GargishBreastplateOfVirtuousEpiphany),
          typeof(GargishArmsOfVirtuousEpiphany),
          typeof(NecklaceOfVirtuousEpiphany),
          typeof(GargishLegsOfVirtuousEpiphany)
        };

        public override Type[] SharedSAList => new Type[]
        {
            typeof(BladeOfBattle),
            typeof(DemonBridleRing),
            typeof(GiantSteps),
            typeof(SwordOfShatteredHopes)
        };

        private DateTime m_NextAbilityTime;
        private Mobile m_SpectralArmor;

        private const int MinAbilityTime = 4;
        private const int MaxAbilityTime = 8;

        [Constructable]
        public Niporailem()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Niporailem";
            Title = "the Thief";
            Body = 0x2D2;

            SetStr(1000);
            SetDex(1200);
            SetInt(1200);

            SetHits(10000);

            SetDamage(15, 27);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Cold, 40);
            SetDamageType(ResistanceType.Energy, 40);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 100);
            SetResistance(ResistanceType.Energy, 40, 50);

            SetSkill(SkillName.MagicResist, 78.3, 87.7);
            SetSkill(SkillName.Tactics, 60.8, 88.7);
            SetSkill(SkillName.Anatomy, 13.5, 13.5);
            SetSkill(SkillName.Wrestling, 59.8, 69.4);
            SetSkill(SkillName.Necromancy, 91.2, 98.3);
            SetSkill(SkillName.SpiritSpeak, 97.5, 105.2);

            Fame = 18000;
            Karma = -18000;
        }

        public override int GetAngerSound() { return 0x175; }
        public override int GetIdleSound() { return 0x19D; }
        public override int GetAttackSound() { return 0xE2; }
        public override int GetHurtSound() { return 0x28B; }
        public override int GetDeathSound() { return 0x108; }
        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);//Vengeful Curse

            if (0.2 > Utility.RandomDouble() && (m_SpectralArmor == null || m_SpectralArmor.Deleted))
            {
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                m_SpectralArmor = new SpectralArmour();
                m_SpectralArmor.MoveToWorld(Location, Map);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);//encounter during fight

            if (0.2 > Utility.RandomDouble() && defender.GetStatMod("Niporailem Str Curse") == null)
            {
                defender.AddStatMod(new StatMod(StatType.Str, "Niporailem Str Curse", -30, TimeSpan.FromSeconds(15.0)));
            }
        }
        /// <summary>
        /// https://uo.com/wiki/ultima-online-wiki/world/dungeons/tomb-of-kings/
        /// Niporailem the thief, a formidable foe who will summon spectral armor and throw overweight,
        /// damaging gold piles which dismount an attacker. These piles turn into treasure sand after a period of time or when dropped. Dropping
        /// the gold piles, or being hidden when they are thrown, can result in a spawn of cursed metallic knights and mages.
        /// </summary>
        public override void OnThink()
        {
            base.OnThink(); // Fool’s Gold

            if (0.1 > Utility.RandomDouble() && DateTime.UtcNow > m_NextAbilityTime && Combatant != null && InRange(Combatant, RangePerception)) // as per OSI, no check for LOS
            {
                Mobile to = (Mobile)Combatant;

                switch (Utility.Random(1))
                {
                    case 0: // Niporailem's Treasure
                        {
                            Effects.SendPacket(Location, Map, new HuedEffect(EffectType.Moving, Serial, to.Serial, 0xEEF, Location, to.Location, 10, 0, false, false, 0, 0));
                            Effects.PlaySound(to.Location, to.Map, 0x37);

                            int amount = Utility.RandomMinMax(2, 4);

                            for (int i = 0; i < amount; i++)
                            {
                                Item treasure = new NiporailemsTreasure();

                                if (!to.Player || !to.PlaceInBackpack(treasure))
                                {
                                    treasure.MoveToWorld(to.Location, to.Map);
                                    treasure.OnDroppedToWorld(this, to.Location);
                                }
                            }
                            to.SendLocalizedMessage(1112112); // To carry the burden of greed!

                            BaseMount.Dismount(to);
                            to.Damage(Utility.Random(18, 27), this);

                            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(MinAbilityTime, MaxAbilityTime));

                            break;
                        }
                }
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new UndyingFlesh());          
        }

        public Niporailem(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_SpectralArmor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            m_SpectralArmor = reader.ReadMobile();
        }
    }
}

namespace Server.Items
{
    public class NiporailemsTreasure : Item
    {
        public override int LabelNumber => ItemID == 0xEEF
                    ? 1112113  // Niporailem's Treasure
                    : 1112115; // Treasure Sand

        private readonly NiporailemsTreasureTimer m_Timer;
        private bool m_CanSpawn;

        public NiporailemsTreasure()
            : base(0xEEF)
        {
            Weight = 25.0;

            m_Timer = new NiporailemsTreasureTimer(this);
            m_Timer.Start();

            m_CanSpawn = true;
        }

        public void TurnToSand()
        {
            ItemID = 0x11EA + Utility.Random(1);
            m_CanSpawn = false;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (!base.OnDroppedToWorld(from, p))
            {
                return false;
            }

            if (m_CanSpawn)
            {
                int amount = Utility.Random(3); // 0-2

                for (int i = 0; i < amount; i++)
                {
                    Mobile summon;

                    if (Utility.RandomBool())
                    {
                        summon = new CursedMetallicKnight();
                    }
                    else
                    {
                        summon = new CursedMetallicMage();
                    }

                    summon.MoveToWorld(p, from.Map);
                }
            }
            from.SendLocalizedMessage(1112111); // To steal my gold? To give it freely!

            TurnToSand();

            return true;
        }

        public NiporailemsTreasure(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            /*int version = */
            reader.ReadInt();
        }

        private class NiporailemsTreasureTimer : Timer
        {
            private readonly NiporailemsTreasure m_Owner;

            public NiporailemsTreasureTimer(NiporailemsTreasure owner)
                : base(TimeSpan.FromSeconds(60.0))
            {

                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if (!m_Owner.Deleted)
                {
                    m_Owner.TurnToSand();
                }
            }
        }
    }

    public class TreasureSand : Item
    {
        public override int LabelNumber => 1112115;  // Treasure Sand

        public TreasureSand()
            : base(0x11EA + Utility.Random(1))
        {
            Weight = 1.0;
        }

        public TreasureSand(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
