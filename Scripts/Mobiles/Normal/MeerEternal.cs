using Server.Items;
using System;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("a meer's corpse")]
    public class MeerEternal : BaseCreature
    {
        private DateTime m_NextAbilityTime;
        [Constructable]
        public MeerEternal()
            : base(AIType.AI_Spellweaving, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a meer eternal";
            Body = 772;

            SetStr(416, 505);
            SetDex(146, 165);
            SetInt(566, 655);

            SetHits(250, 303);

            SetDamage(11, 13);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 45, 55);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 45, 55);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 90.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 150.5, 200.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);
            SetSkill(SkillName.Spellweaving, 90.1, 100.0);

            Fame = 18000;
            Karma = 18000;

            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
        }

        public MeerEternal(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel => true;
        public override bool CanRummageCorpses => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int TreasureMapLevel => 5;
        public override bool InitialInnocent => true;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 2);
            AddLoot(LootPack.MedScrolls, 2);
            AddLoot(LootPack.HighScrolls, 2);
            AddLoot(LootPack.RandomLootItem(new[] { typeof(StrangleScroll), typeof(WitherScroll), typeof(VampiricEmbraceScroll) }, 25.0, 1, false, true));
        }

        public override int GetHurtSound()
        {
            return 0x167;
        }

        public override int GetDeathSound()
        {
            return 0xBC;
        }

        public override int GetAttackSound()
        {
            return 0x28B;
        }

        public override void OnThink()
        {
            if (DateTime.UtcNow >= m_NextAbilityTime)
            {
                Mobile combatant = Combatant as Mobile;

                if (combatant != null && combatant.Map == Map && combatant.InRange(this, 12))
                {
                    m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

                    int ability = Utility.Random(4);

                    switch (ability)
                    {
                        case 0:
                            DoFocusedLeech(combatant, "Thine essence will fill my withering body with strength!");
                            break;
                        case 1:
                            DoFocusedLeech(combatant, "I rebuke thee, worm, and cleanse thy vile spirit of its tainted blood!");
                            break;
                        case 2:
                            DoFocusedLeech(combatant, "I devour your life's essence to strengthen my resolve!");
                            break;
                        case 3:
                            DoAreaLeech();
                            break;
                            // TODO: Resurrect ability
                    }
                }
            }

            base.OnThink();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private void DoAreaLeech()
        {
            m_NextAbilityTime += TimeSpan.FromSeconds(2.5);

            Say(true, "Beware, mortals!  You have provoked my wrath!");
            FixedParticles(0x376A, 10, 10, 9537, 33, 0, EffectLayer.Waist);

            Timer.DelayCall(TimeSpan.FromSeconds(5.0), DoAreaLeech_Finish);
        }

        private void DoAreaLeech_Finish()
        {
            ArrayList list = new ArrayList();
            IPooledEnumerable eable = GetMobilesInRange(6);

            foreach (Mobile m in eable)
            {
                if (CanBeHarmful(m) && IsEnemy(m))
                    list.Add(m);
            }
            eable.Free();

            if (list.Count == 0)
            {
                Say(true, "Bah! You have escaped my grasp this time, mortal!");
            }
            else
            {
                double scalar;

                if (list.Count == 1)
                    scalar = 0.75;
                else if (list.Count == 2)
                    scalar = 0.50;
                else
                    scalar = 0.25;

                for (int i = 0; i < list.Count; ++i)
                {
                    Mobile m = (Mobile)list[i];

                    int damage = (int)(m.Hits * scalar);

                    damage += Utility.RandomMinMax(-5, 5);

                    if (damage < 1)
                        damage = 1;

                    m.MovingParticles(this, 0x36F4, 1, 0, false, false, 32, 0, 9535, 1, 0, (EffectLayer)255, 0x100);
                    m.MovingParticles(this, 0x0001, 1, 0, false, true, 32, 0, 9535, 9536, 0, (EffectLayer)255, 0);

                    DoHarmful(m);
                    Hits += AOS.Damage(m, this, damage, 100, 0, 0, 0, 0);
                }

                Say(true, "If I cannot cleanse thy soul, I will destroy it!");
            }
        }

        private void DoFocusedLeech(Mobile combatant, string message)
        {
            Say(true, message);

            Timer.DelayCall(TimeSpan.FromSeconds(0.5), new TimerStateCallback(DoFocusedLeech_Stage1), combatant);
        }

        private void DoFocusedLeech_Stage1(object state)
        {
            Mobile combatant = (Mobile)state;

            if (CanBeHarmful(combatant))
            {
                MovingParticles(combatant, 0x36FA, 1, 0, false, false, 1108, 0, 9533, 1, 0, (EffectLayer)255, 0x100);
                MovingParticles(combatant, 0x0001, 1, 0, false, true, 1108, 0, 9533, 9534, 0, (EffectLayer)255, 0);
                PlaySound(0x1FB);

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(DoFocusedLeech_Stage2), combatant);
            }
        }

        private void DoFocusedLeech_Stage2(object state)
        {
            Mobile combatant = (Mobile)state;

            if (CanBeHarmful(combatant))
            {
                combatant.MovingParticles(this, 0x36F4, 1, 0, false, false, 32, 0, 9535, 1, 0, (EffectLayer)255, 0x100);
                combatant.MovingParticles(this, 0x0001, 1, 0, false, true, 32, 0, 9535, 9536, 0, (EffectLayer)255, 0);

                PlaySound(0x209);
                DoHarmful(combatant);
                Hits += AOS.Damage(combatant, this, Utility.RandomMinMax(30, 40), 100, 0, 0, 0, 0);
            }
        }
    }
}
