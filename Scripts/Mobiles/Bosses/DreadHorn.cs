using Server.Items;
using Server.Spells;
using System;
using System.Linq;

namespace Server.Mobiles
{
    [CorpseName("a dread horns corpse")]
    public class DreadHorn : BasePeerless
    {
        public virtual int StrikingRange => 12;

        [Constructable]
        public DreadHorn() : base(AIType.AI_Spellweaving, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a Dread Horn";
            Body = 257;
            BaseSoundID = 0xA8;

            SetStr(878, 993);
            SetDex(581, 683);
            SetInt(1200, 1300);

            SetHits(50000);
            SetStam(507, 669);
            SetMana(1200, 1300);

            SetDamage(21, 28);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Poison, 60);

            SetResistance(ResistanceType.Physical, 40, 55);
            SetResistance(ResistanceType.Fire, 50, 65);
            SetResistance(ResistanceType.Cold, 50, 65);
            SetResistance(ResistanceType.Poison, 65, 75);
            SetResistance(ResistanceType.Energy, 60, 75);

            SetSkill(SkillName.Wrestling, 90.0);
            SetSkill(SkillName.Tactics, 90.0);
            SetSkill(SkillName.MagicResist, 110.0);
            SetSkill(SkillName.Poisoning, 120.0);
            SetSkill(SkillName.Magery, 110.0);
            SetSkill(SkillName.EvalInt, 110.0);
            SetSkill(SkillName.Meditation, 110.0);
            SetSkill(SkillName.Spellweaving, 120.0);

            Fame = 32000;
            Karma = -32000;

            m_Change = DateTime.UtcNow;
            m_Stomp = DateTime.UtcNow;
            m_Teleport = DateTime.UtcNow;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 8);
            AddLoot(LootPack.LowScrolls, 4);
            AddLoot(LootPack.MedScrolls, 4);
            AddLoot(LootPack.HighScrolls, 4);
            AddLoot(LootPack.ArcanistScrolls, Utility.RandomMinMax(1, 6));
            AddLoot(LootPack.PeerlessResource, 8);
            AddLoot(LootPack.Talisman, 5);
            AddLoot(LootPack.LootItem<DreadHornMane>());
            AddLoot(LootPack.LootItem<TaintedMushroom>(60.0));
            AddLoot(LootPack.LootItem<ParrotItem>(60.0));
            AddLoot(LootPack.LootItem<MangledHeadOfDreadhorn>(50.0));
            AddLoot(LootPack.LootItem<HornOfTheDreadhorn>(50.0));
            AddLoot(LootPack.LootItem<PristineDreadHorn>(5.0));
            AddLoot(LootPack.LootItem<DreadFlute>(5.0));
            AddLoot(LootPack.LootItem<DreadsRevenge>(5.0));
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Combatant != null)
            {
                if (m_Change < DateTime.UtcNow && Utility.RandomDouble() < 0.1)
                    ChangeOpponent();

                if (m_Stomp < DateTime.UtcNow && Utility.RandomDouble() < 0.1)
                    HoofStomp();

                if (m_Teleport < DateTime.UtcNow && Utility.RandomDouble() < 0.1)
                    Teleport();
            }
        }

        public override int Hides => 10;
        public override HideType HideType => HideType.Regular;

        public override int Meat => 5;
        public override MeatType MeatType => MeatType.Ribs;

        public override bool GivesMLMinorArtifact => true;
        public override bool Unprovokable => true;
        public override Poison PoisonImmune => Poison.Deadly;
        public override Poison HitPoison => Poison.Lethal;
        public override int TreasureMapLevel => 5;

        public DreadHorn(Serial serial) : base(serial)
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

            int version = reader.ReadInt();

            m_Change = DateTime.UtcNow;
            m_Stomp = DateTime.UtcNow;
            m_Teleport = DateTime.UtcNow;
        }

        private DateTime m_Change;
        private DateTime m_Stomp;
        private DateTime m_Teleport;

        private void Teleport()
        {
            System.Collections.Generic.List<PlayerMobile> toTele = SpellHelper.AcquireIndirectTargets(this, Location, Map, StrikingRange).OfType<PlayerMobile>().ToList();

            if (toTele.Count > 0)
            {
                PlayerMobile from = toTele[Utility.Random(toTele.Count)];

                if (from != null)
                {
                    Combatant = from;

                    from.MoveToWorld(GetSpawnPosition(1), Map);
                    from.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                    from.PlaySound(0x1FE);

                    from.ApplyPoison(this, HitPoison);
                }
            }

            ColUtility.Free(toTele);
            m_Teleport = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(40, 60));
        }

        public void ChangeOpponent()
        {
            Mobile agro, best = null;
            double distance, random = Utility.RandomDouble();

            if (random < 0.75)
            {
                // find random target relatively close
                for (int i = 0; i < Aggressors.Count && best == null; i++)
                {
                    agro = Validate(Aggressors[i].Attacker);

                    if (agro == null)
                        continue;

                    distance = StrikingRange - GetDistanceToSqrt(agro);

                    if (distance > 0 && distance < StrikingRange - 2 && InLOS(agro.Location))
                    {
                        distance /= StrikingRange;

                        if (random < distance)
                            best = agro;
                    }
                }
            }
            else
            {
                int damage = 0;

                // find a player who dealt most damage
                for (int i = 0; i < DamageEntries.Count; i++)
                {
                    agro = Validate(DamageEntries[i].Damager);

                    if (agro == null)
                        continue;

                    distance = GetDistanceToSqrt(agro);

                    if (distance < StrikingRange && DamageEntries[i].DamageGiven > damage && InLOS(agro.Location))
                    {
                        best = agro;
                        damage = DamageEntries[i].DamageGiven;
                    }
                }
            }

            if (best != null)
            {
                // teleport
                best.Location = GetSpawnPosition(Location, Map, 1);
                best.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                best.PlaySound(0x1FE);

                Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                 {
                     best.ApplyPoison(this, HitPoison);
                     best.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                     best.PlaySound(0x474);
                 });

                m_Change = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));
            }
        }

        public void HoofStomp()
        {
            if (Map == null)
                return;

            foreach (Mobile m in SpellHelper.AcquireIndirectTargets(this, Location, Map, StrikingRange).OfType<Mobile>())
            {
                if (m.GetStatMod("DreadHornStr") == null)
                {
                    double percent = m.Skills.MagicResist.Value / 100;
                    int malas = (int)(-20 + (percent * 5.2));

                    m.AddStatMod(new StatMod(StatType.Str, "DreadHornStr", m.Str < Math.Abs(malas) ? m.Str / 2 : malas, TimeSpan.FromSeconds(60)));
                    m.AddStatMod(new StatMod(StatType.Dex, "DreadHornDex", m.Dex < Math.Abs(malas) ? m.Dex / 2 : malas, TimeSpan.FromSeconds(60)));
                    m.AddStatMod(new StatMod(StatType.Int, "DreadHornInt", m.Int < Math.Abs(malas) ? m.Int / 2 : malas, TimeSpan.FromSeconds(60)));
                }

                m.SendLocalizedMessage(1075081); // *Dreadhorns eyes light up, his mouth almost a grin, as he slams one hoof to the ground!*
            }

            // earthquake
            PlaySound(0x2F3);
            m_Stomp = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 80));
        }

        public static bool IsUnderInfluence(Mobile m)
        {
            return m.GetStatMod("DreadHornStr") != null;
        }

        public Mobile Validate(Mobile m)
        {
            Mobile agro;

            if (m is BaseCreature)
                agro = ((BaseCreature)m).ControlMaster;
            else
                agro = m;

            if (!CanBeHarmful(agro, false) || !agro.Player /*|| Combatant == agro*/ )
                return null;

            return agro;
        }
    }
}
