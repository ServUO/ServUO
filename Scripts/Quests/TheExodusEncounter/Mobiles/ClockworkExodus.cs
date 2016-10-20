using System;
using System.Collections;
using Server.Items;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;
using Server;

namespace Server.Mobiles
{
    [CorpseName("a clockwork exodus corpse")]
    public class ClockworkExodus : BaseExodusPeerless
    {
        private static readonly Type[] m_Artifact = new Type[]
        {
            typeof(ScrollofValiantCommendation),
            typeof(BracersofAlchemicalDevastation),
            typeof(HygieiasAmulet),
            typeof(Asclepius),
            typeof(ClockworkLeggings),
            typeof(DupresSword),
            typeof(GargishDupresSword),
            typeof(GargishClockworkLeggings),
            typeof(GargishBracersofAlchemicalDevastation),
            typeof(GargishAsclepius)
        };

        private Point3D m_LastTarget;
        private static Point3D m_Location;        

        [Constructable]
        public ClockworkExodus() : base(AIType.AI_Mystic/*AI_Melee*/, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Clockwork Exodus";
            this.Body = 1248;
            this.BaseSoundID = 639;

            this.SetStr(851, 950);
            this.SetDex(581, 683);
            this.SetInt(601, 750);

            this.SetHits(50000);
            this.SetStam(507, 669);

            this.SetDamage(20, 30);

            this.SetDamageType(ResistanceType.Physical, 75);
            this.SetDamageType(ResistanceType.Fire, 25);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 75, 90);
            this.SetResistance(ResistanceType.Fire, 50, 65);
            this.SetResistance(ResistanceType.Cold, 45, 60);
            this.SetResistance(ResistanceType.Poison, 45, 60);
            this.SetResistance(ResistanceType.Energy, 45, 60);

            this.SetSkill(SkillName.EvalInt, 130.0);
            this.SetSkill(SkillName.Mysticism, 120);
            this.SetSkill(SkillName.MagicResist, 140.0);
            this.SetSkill(SkillName.Tactics, 90.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 105.0);
            this.SetSkill(SkillName.SpiritSpeak, 120.0);
            this.SetSkill(SkillName.Necromancy, 120.0);
            this.SetSkill(SkillName.Poisoning, 120.0);
            this.SetSkill(SkillName.Meditation, 120.0);
            this.SetSkill(SkillName.Anatomy, 120.0);
            this.SetSkill(SkillName.Healing, 120.0);

            this.RespawnTime = TimeSpan.FromMinutes(10.0);
            new InternalTimer(this).Start();

            this.Fame = 24000;
            this.Karma = -24000;

            this.VirtualArmor = 20;

            PackGold(10000, 60000);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Unprovokable { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override int TreasureMapLevel { get { return 5; } }

        public static void DistributeRandomArtifact(BaseCreature bc, Type[] typelist)
        {
            int random = Utility.Random(typelist.Length);
            Item item = Loot.Construct(typelist[random]);
            DistributeArtifact(DemonKnight.FindRandomPlayer(bc), item);
        }

        public static void DistributeArtifact(Mobile to, Item artifact)
        {
            if (to == null || artifact == null)
                return;

            Container pack = to.Backpack;

            if (pack == null || !pack.TryDropItem(to, artifact, false))
                to.BankBox.DropItem(artifact);

			//CommandLogging.WriteLineNoLuckArtifact(to, artifact, String.Format("Peerless Artifact"));

            to.SendLocalizedMessage(502088); // A special gift has been placed in your backpack.
		}

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomDouble() < 0.2)
                DistributeRandomArtifact(this, m_Artifact);            

            Map map = this.Map;

            if (map != null)
            {
                for (int x = -8; x <= 8; ++x)
                {
                    for (int y = -8; y <= 8; ++y)
                    {
                        double dist = Math.Sqrt(x * x + y * y);

                        if (dist <= 8)
                            new GoldTimer(map, X + x, Y + y).Start();
                    }
                }
            }
            return true;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosSuperBoss, 8);
        }

        public void SpawnVortices(Mobile target)
        {
            Map map = this.Map;

            if (map == null)
                return;

            DeathVortexTrap dv = new DeathVortexTrap();

            this.MovingParticles(target, 0x1AF6, 5, 0, false, false, 0x816, 0, 3006, 0, 0, 0);

            dv.MoveToWorld(target.Location, map);

            m_LastTarget = target.Location;
        }

        public void DoSpecialAbility(Mobile target)
        {
            if (target != null)
            {
                if (m_LastTarget != target.Location)
                {
                    target.SendMessage("Clockwork Exodus casts a deadly vortex at you!");
                    SpawnVortices(target);
                }

                if (0.5 >= Utility.RandomDouble() && this != null)
                    Aura(this, this.Map, target, 25, 5, "Clockwork Exodus : The creature's aura of energy is damaging you!");
            }
        }

        public static void Aura(Mobile dv, Map map, Mobile from, int damage, int range, string text)
        {
            if (from == null)
                return;

            if (dv != null)
                m_Location = dv.Location;
            else
                return;

            List<Mobile> targets = new List<Mobile>();

            foreach (Mobile m in Map.AllMaps[map.MapID].GetMobilesInRange(m_Location, range))
            {
                if (m.Alive && !m.IsStaff() && m != dv)
                    targets.Add(m);
            }

            for (int i = 0; i < targets.Count; i++)
            {
                Mobile m = (Mobile)targets[i];
                m.RevealingAction();

                int auradamage = damage;

                from.DoHarmful(m);
                AOS.Damage(m, (from == null) ? m : from, auradamage, false, 0, 0, 0, 0, 100, 0, 0, false, false, false);

                if (text != "")
                    m.SendMessage(text);
            }

            targets.Clear();
        }

        public override void OnDamagedBySpell(Mobile attacker)
        {
            base.OnDamagedBySpell(attacker);

            DoSpecialAbility(attacker);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);  //if it hits you it spawns vortices

            DoSpecialAbility(defender);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker); //if you hit creature it spawns vortices

            DoSpecialAbility(attacker);
        }

        public ClockworkExodus(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class GoldTimer : Timer
        {
            private Map m_Map;
            private int m_X, m_Y;

            public GoldTimer(Map map, int x, int y) : base(TimeSpan.FromSeconds(Utility.RandomDouble() * 10.0))
            {
                m_Map = map;
                m_X = x;
                m_Y = y;
            }

            protected override void OnTick()
            {
                int z = m_Map.GetAverageZ(m_X, m_Y);
                bool canFit = m_Map.CanFit(m_X, m_Y, z, 6, false, false);

                for (int i = -3; !canFit && i <= 3; ++i)
                {
                    canFit = m_Map.CanFit(m_X, m_Y, z + i, 6, false, false);

                    if (canFit)
                        z += i;
                }

                if (!canFit)
                    return;

                Gold g = new Gold(500, 1000);

                g.MoveToWorld(new Point3D(m_X, m_Y, z), m_Map);

                if (0.5 >= Utility.RandomDouble())
                {
                    switch (Utility.Random(3))
                    {
                        case 0: // Fire column
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x3709, 10, 30, 5052);
                                Effects.PlaySound(g, g.Map, 0x208);

                                break;
                            }
                        case 1: // Explosion
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36BD, 20, 10, 5044);
                                Effects.PlaySound(g, g.Map, 0x307);

                                break;
                            }
                        case 2: // Ball of fire
                            {
                                Effects.SendLocationParticles(EffectItem.Create(g.Location, g.Map, EffectItem.DefaultDuration), 0x36FE, 10, 10, 5052);

                                break;
                            }
                    }
                }
            }
        }
    }
}