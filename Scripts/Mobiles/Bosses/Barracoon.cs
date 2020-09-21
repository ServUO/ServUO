using Server.Engines.CannedEvil;
using Server.Items;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using System;

namespace Server.Mobiles
{
    public class Barracoon : BaseChampion
    {
        [Constructable]
        public Barracoon()
            : base(AIType.AI_Melee)
        {
            Name = "Barracoon";
            Title = "the piper";
            Body = 0x190;
            Hue = 0x83EC;

            SetStr(283, 425);
            SetDex(72, 150);
            SetInt(505, 750);

            SetHits(12000);
            SetStam(102, 300);
            SetMana(505, 750);

            SetDamage(29, 38);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 70, 80);
            SetResistance(ResistanceType.Cold, 65, 80);
            SetResistance(ResistanceType.Poison, 70, 75);
            SetResistance(ResistanceType.Energy, 70, 80);

            SetSkill(SkillName.MagicResist, 100.0);
            SetSkill(SkillName.Tactics, 118.3, 120.2);
            SetSkill(SkillName.Wrestling, 118.4, 122.7);

            Fame = 22500;
            Karma = -22500;

            AddItem(new FancyShirt(Utility.RandomGreenHue()));
            AddItem(new LongPants(Utility.RandomYellowHue()));
            AddItem(new JesterHat(Utility.RandomPinkHue()));
            AddItem(new Cloak(Utility.RandomPinkHue()));
            AddItem(new Sandals());

            HairItemID = 0x203B; // Short Hair
            HairHue = 0x94;

            m_SpecialSlayerMechanics = true;
        }

        public Barracoon(Serial serial)
            : base(serial)
        {
        }

        public override ChampionSkullType SkullType => ChampionSkullType.Greed;
        public override Type[] UniqueList => new[] { typeof(FangOfRactus) };
        public override Type[] SharedList => new[]
                {
                    typeof(EmbroideredOakLeafCloak),
                    typeof(DjinnisRing),
                    typeof(DetectiveBoots),
                    typeof(GauntletsOfAnger)
                };
        public override Type[] DecorativeList => new[] { typeof(SwampTile), typeof(MonsterStatuette) };
        public override MonsterStatuetteType[] StatueTypes => new[] { MonsterStatuetteType.Slime };
        public override bool AlwaysMurderer => true;
        public override bool AutoDispel => true;
        public override double AutoDispelChance => 1.0;
        public override bool AllureImmune => true;
        public override bool Unprovokable => true;
        public override bool Uncalmable => true;
        public override Poison PoisonImmune => Poison.Deadly;
        public override bool ShowFameTitle => false;
        public override bool ClickTitle => false;

        public override bool ForceStayHome => true;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
        }

        public void Polymorph(Mobile m)
        {
            if (!m.CanBeginAction(typeof(PolymorphSpell)) || !m.CanBeginAction(typeof(IncognitoSpell)) || m.IsBodyMod)
                return;

            IMount mount = m.Mount;

            if (mount != null)
                mount.Rider = null;

            if (m.Flying)
                m.ToggleFlying();

            if (m.Mounted)
                return;

            if (m.BeginAction(typeof(PolymorphSpell)))
            {
                m.BodyMod = 42;
                m.HueMod = 0;
                if (m == this)
                {
                    m_SlayerVulnerabilities.Add("Vermin");
                    m_SlayerVulnerabilities.Add("Repond");
                }

                new ExpirePolymorphTimer(m).Start();
            }
        }

        public void SpawnRatmen(Mobile target)
        {
            Map map = Map;

            if (map == null)
                return;

            int rats = 0;

            IPooledEnumerable eable = GetMobilesInRange(10);

            foreach (Mobile m in eable)
            {
                if (m is Ratman || m is RatmanArcher || m is RatmanMage)
                    ++rats;
            }

            eable.Free();

            if (rats < 16)
            {
                PlaySound(0x3D);

                int newRats = Utility.RandomMinMax(3, 6);

                for (int i = 0; i < newRats; ++i)
                {
                    BaseCreature rat;

                    switch (Utility.Random(5))
                    {
                        default:
                        case 0:
                        case 1:
                            rat = new Ratman();
                            break;
                        case 2:
                        case 3:
                            rat = new RatmanArcher();
                            break;
                        case 4:
                            rat = new RatmanMage();
                            break;
                    }

                    rat.Team = Team;

                    bool validLocation = false;
                    Point3D loc = Location;

                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = X + Utility.Random(3) - 1;
                        int y = Y + Utility.Random(3) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, Z, 16, false, false))
                            loc = new Point3D(x, y, Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }

                    rat.IsChampionSpawn = true;
                    rat.MoveToWorld(loc, map);
                    rat.Combatant = target;
                }
            }
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target == null || target.Deleted) //sanity
                return;

            if (target.Player && 0.6 >= Utility.RandomDouble()) // 60% chance to polymorph attacker into a ratman
                Polymorph(target);

            if (0.1 >= Utility.RandomDouble()) // 10% chance to more ratmen
                SpawnRatmen(target);

            if (0.05 >= Utility.RandomDouble() && !IsBodyMod) // 5% chance to polymorph into a ratman
                Polymorph(this);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            DoSpecialAbility(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            DoSpecialAbility(defender);
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            base.OnDamagedBySpell(from);

            DoSpecialAbility(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_SlayerVulnerabilities.Clear();
        }

        private class ExpirePolymorphTimer : Timer
        {
            private readonly Mobile m_Owner;
            public ExpirePolymorphTimer(Mobile owner)
                : base(TimeSpan.FromMinutes(3.0)) 
            {
                m_Owner = owner;

                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (!m_Owner.CanBeginAction(typeof(PolymorphSpell)))
                {
                    m_Owner.BodyMod = 0;
                    m_Owner.HueMod = -1;
                    m_Owner.EndAction(typeof(PolymorphSpell));
                    if (m_Owner.SlayerVulnerabilities != null)
                    {
                        m_Owner.SlayerVulnerabilities.Remove("Vermin");
                        m_Owner.SlayerVulnerabilities.Remove("Repond");
                    }
                }
            }
        }
    }
}
