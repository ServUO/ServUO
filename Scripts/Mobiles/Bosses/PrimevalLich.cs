using Server.Engines.CannedEvil;
using Server.Items;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a Primeval Lich corpse")]
    public class PrimevalLich : BaseChampion
    {
        private DateTime m_NextDiscordTime;
        private DateTime m_NextAbilityTime;
        private Timer m_Timer;

        [Constructable]
        public PrimevalLich()
            : base(AIType.AI_NecroMage)
        {
            Name = "Primeval Lich";
            Body = 830;

            SetStr(500);
            SetDex(100);
            SetInt(1000);

            SetHits(30000);
            SetMana(5000);

            SetDamage(17, 21);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Fire, 20);
            SetDamageType(ResistanceType.Cold, 20);
            SetDamageType(ResistanceType.Energy, 20);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 30);
            SetResistance(ResistanceType.Fire, 30);
            SetResistance(ResistanceType.Cold, 30);
            SetResistance(ResistanceType.Poison, 30);
            SetResistance(ResistanceType.Energy, 20);

            SetSkill(SkillName.EvalInt, 90, 120.0);
            SetSkill(SkillName.Magery, 90, 120.0);
            SetSkill(SkillName.Meditation, 100, 120.0);
            SetSkill(SkillName.Necromancy, 120.0);
            SetSkill(SkillName.SpiritSpeak, 120.0);
            SetSkill(SkillName.MagicResist, 120, 140.0);
            SetSkill(SkillName.Tactics, 90, 120);
            SetSkill(SkillName.Wrestling, 100, 120);

            Fame = 28000;
            Karma = -28000;

            m_Timer = new TeleportTimer(this);
            m_Timer.Start();
        }

        public PrimevalLich(Serial serial)
            : base(serial)
        {
        }

        public override int GetAttackSound() { return 0x61E; }
        public override int GetDeathSound() { return 0x61F; }
        public override int GetHurtSound() { return 0x620; }
        public override int GetIdleSound() { return 0x621; }

        public override bool CanRummageCorpses => true;
        public override bool BleedImmune => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override bool ShowFameTitle => false;
        public override bool ClickTitle => false;

        public override ChampionSkullType SkullType => ChampionSkullType.None;

        public override Type[] UniqueList => new[] { typeof(BansheesCall), typeof(CastOffZombieSkin), typeof(ChannelersDefender), typeof(LightsRampart) };
        public override Type[] SharedList => new[] { typeof(TokenOfHolyFavor), typeof(TheMostKnowledgePerson), typeof(LieutenantOfTheBritannianRoyalGuard), typeof(ProtectoroftheBattleMage) };
        public override Type[] DecorativeList => new[] { typeof(MummifiedCorpse) };
        public override MonsterStatuetteType[] StatueTypes => new MonsterStatuetteType[] { };

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 3);
            AddLoot(LootPack.Meager);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new PrimalLichDust());
            c.DropItem(new RisingColossusScroll());
        }

        public void ChangeCombatant()
        {
            ForceReacquire();
            ForceFleeUntil = DateTime.UtcNow + TimeSpan.FromSeconds(2.5);
        }

        public override void OnThink()
        {
            if (m_NextDiscordTime <= DateTime.UtcNow)
            {
                Mobile target = Combatant as Mobile;

                if (target != null && target.InRange(this, 8) && CanBeHarmful(target))
                    Discord(target);
            }
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.05 >= Utility.RandomDouble())
                SpawnShadowDwellers(attacker);
        }

        public override void AlterDamageScalarFrom(Mobile caster, ref double scalar)
        {
            if (0.05 >= Utility.RandomDouble())
                SpawnShadowDwellers(caster);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (DateTime.UtcNow > m_NextAbilityTime && 0.2 > Utility.RandomDouble())
            {
                switch (Utility.Random(2))
                {
                    case 0: BlastRadius(); break;
                    case 1: Lightning(); break;
                }

                m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(25, 35));
            }
        }

        #region Blast Radius
        private static readonly int BlastRange = 16;

        private static readonly double[] BlastChance = new[]
            {
                0.0, 0.0, 0.05, 0.95, 0.95, 0.95, 0.05, 0.95, 0.95,
                0.95, 0.05, 0.95, 0.95, 0.95, 0.05, 0.95, 0.95
            };

        private void BlastRadius()
        {
            // TODO: Based on OSI taken videos, not accurate, but an aproximation

            Point3D loc = Location;

            for (int x = -BlastRange; x <= BlastRange; x++)
            {
                for (int y = -BlastRange; y <= BlastRange; y++)
                {
                    Point3D p = new Point3D(loc.X + x, loc.Y + y, loc.Z);
                    int dist = (int)Math.Round(Utility.GetDistanceToSqrt(loc, p));

                    if (dist <= BlastRange && BlastChance[dist] > Utility.RandomDouble())
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(0.1 * dist), delegate
                        {
                            int hue = Utility.RandomList(90, 95);

                            Effects.SendPacket(loc, Map, new HuedEffect(EffectType.FixedXYZ, Serial.Zero, Serial.Zero, 0x3709, p, p, 20, 30, true, false, hue, 4));
                        });
                    }
                }
            }

            PlaySound(0x64C);

            IPooledEnumerable eable = GetMobilesInRange(BlastRange);
            foreach (Mobile m in eable)
            {
                if (this != m && GetDistanceToSqrt(m) <= BlastRange && CanBeHarmful(m))
                {
                    if (m is ShadowDweller)
                        continue;

                    DoHarmful(m);

                    double damage = m.Hits * 0.6;

                    if (damage < 100.0)
                        damage = 100.0;
                    else if (damage > 200.0)
                        damage = 200.0;

                    DoHarmful(m);

                    AOS.Damage(m, this, (int)damage, 0, 0, 0, 0, 100);
                }
            }

            eable.Free();
        }
        #endregion

        #region Lightning
        private void Lightning()
        {
            int count = 0;

            IPooledEnumerable eable = GetMobilesInRange(BlastRange);
            foreach (Mobile m in eable)
            {
                if (m is ShadowDweller)
                    continue;

                if (m.IsPlayer() && GetDistanceToSqrt(m) <= BlastRange && CanBeHarmful(m))
                {
                    DoHarmful(m);

                    Effects.SendBoltEffect(m, false, 0);
                    Effects.PlaySound(m, m.Map, 0x51D);

                    double damage = m.Hits * 0.6;

                    if (damage < 100.0)
                        damage = 100.0;
                    else if (damage > 200.0)
                        damage = 200.0;

                    AOS.Damage(m, this, (int)damage, 0, 0, 0, 0, 100);

                    count++;

                    if (count >= 6)
                        break;
                }
            }

            eable.Free();
        }
        #endregion

        #region Teleport
        private class TeleportTimer : Timer
        {
            private readonly Mobile m_Owner;

            private static readonly int[] m_Offsets = new[]
            {
                -1, -1,
                -1,  0,
                -1,  1,
                0, -1,
                0,  1,
                1, -1,
                1,  0,
                1,  1
            };

            public TeleportTimer(Mobile owner)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if (m_Owner.Deleted)
                {
                    Stop();
                    return;
                }

                Map map = m_Owner.Map;

                if (map == null)
                    return;

                if (0.25 < Utility.RandomDouble())
                    return;

                Mobile toTeleport = null;

                foreach (Mobile m in m_Owner.GetMobilesInRange(BlastRange))
                {
                    if (m != m_Owner && m.IsPlayer() && m_Owner.CanBeHarmful(m) && m_Owner.CanSee(m))
                    {
                        if (m is ShadowDweller)
                            continue;

                        toTeleport = m;
                        break;
                    }
                }

                if (toTeleport != null)
                {
                    int offset = Utility.Random(8) * 2;

                    Point3D to = m_Owner.Location;

                    for (int i = 0; i < m_Offsets.Length; i += 2)
                    {
                        int x = m_Owner.X + m_Offsets[(offset + i) % m_Offsets.Length];
                        int y = m_Owner.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                        if (map.CanSpawnMobile(x, y, m_Owner.Z))
                        {
                            to = new Point3D(x, y, m_Owner.Z);
                            break;
                        }
                        else
                        {
                            int z = map.GetAverageZ(x, y);

                            if (map.CanSpawnMobile(x, y, z))
                            {
                                to = new Point3D(x, y, z);
                                break;
                            }
                        }
                    }

                    Mobile m = toTeleport;

                    Point3D from = m.Location;

                    m.Location = to;

                    Spells.SpellHelper.Turn(m_Owner, toTeleport);
                    Spells.SpellHelper.Turn(toTeleport, m_Owner);

                    m.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                    m.PlaySound(0x1FE);

                    m_Owner.Combatant = toTeleport;
                }
            }
        }
        #endregion

        #region Unholy Touch
        private static readonly Dictionary<Mobile, Timer> m_UnholyTouched = new Dictionary<Mobile, Timer>();

        public void Discord(Mobile target)
        {
            if (Utility.RandomDouble() < 0.9 && !m_UnholyTouched.ContainsKey(target))
            {
                double scalar = -((20 - (target.Skills[SkillName.MagicResist].Value / 10)) / 100);

                ArrayList mods = new ArrayList();

                if (target.PhysicalResistance > 0)
                {
                    mods.Add(new ResistanceMod(ResistanceType.Physical, (int)(target.PhysicalResistance * scalar)));
                }

                if (target.FireResistance > 0)
                {
                    mods.Add(new ResistanceMod(ResistanceType.Fire, (int)(target.FireResistance * scalar)));
                }

                if (target.ColdResistance > 0)
                {
                    mods.Add(new ResistanceMod(ResistanceType.Cold, (int)(target.ColdResistance * scalar)));
                }

                if (target.PoisonResistance > 0)
                {
                    mods.Add(new ResistanceMod(ResistanceType.Poison, (int)(target.PoisonResistance * scalar)));
                }

                if (target.EnergyResistance > 0)
                {
                    mods.Add(new ResistanceMod(ResistanceType.Energy, (int)(target.EnergyResistance * scalar)));
                }

                for (int i = 0; i < target.Skills.Length; ++i)
                {
                    if (target.Skills[i].Value > 0)
                    {
                        mods.Add(new DefaultSkillMod((SkillName)i, true, target.Skills[i].Value * scalar));
                    }
                }

                target.PlaySound(0x458);

                ApplyMods(target, mods);

                m_UnholyTouched[target] = Timer.DelayCall(TimeSpan.FromSeconds(30), delegate
                {
                    ClearMods(target, mods);

                    m_UnholyTouched.Remove(target);
                });
            }

            m_NextDiscordTime = DateTime.UtcNow + TimeSpan.FromSeconds(5 + Utility.RandomDouble() * 22);
        }

        private static void ApplyMods(Mobile from, ArrayList mods)
        {
            for (int i = 0; i < mods.Count; ++i)
            {
                object mod = mods[i];

                if (mod is ResistanceMod)
                    from.AddResistanceMod((ResistanceMod)mod);
                else if (mod is StatMod)
                    from.AddStatMod((StatMod)mod);
                else if (mod is SkillMod)
                    from.AddSkillMod((SkillMod)mod);
            }
        }

        private static void ClearMods(Mobile from, ArrayList mods)
        {
            for (int i = 0; i < mods.Count; ++i)
            {
                object mod = mods[i];

                if (mod is ResistanceMod)
                    from.RemoveResistanceMod((ResistanceMod)mod);
                else if (mod is StatMod)
                    from.RemoveStatMod(((StatMod)mod).Name);
                else if (mod is SkillMod)
                    from.RemoveSkillMod((SkillMod)mod);
            }
        }
        #endregion

        public void SpawnShadowDwellers(Mobile target)
        {
            Map map = Map;

            if (map == null)
                return;

            int newShadowDwellers = Utility.RandomMinMax(2, 3);

            for (int i = 0; i < newShadowDwellers; ++i)
            {
                ShadowDweller shadowdweller = new ShadowDweller
                {
                    Team = Team,
                    FightMode = FightMode.Closest
                };

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

                shadowdweller.MoveToWorld(loc, map);
                shadowdweller.Combatant = target;
            }
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

            m_Timer = new TeleportTimer(this);
            m_Timer.Start();
        }
    }
}
