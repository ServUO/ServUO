using Server.Items;
using Server.Misc;
using System;

namespace Server.Mobiles
{
    [CorpseName("a glowing orc corpse")]
    public class PirateCrew : BaseCreature
    {
        public override WeaponAbility GetWeaponAbility()
        {
            Item weapon = FindItemOnLayer(Layer.TwoHanded);

            if (weapon == null)
                return null;

            if (weapon is BaseWeapon)
            {
                if (Utility.RandomBool())
                    return ((BaseWeapon)weapon).PrimaryAbility;
                else
                    return ((BaseWeapon)weapon).SecondaryAbility;
            }
            return null;
        }

        public override InhumanSpeech SpeechType => InhumanSpeech.Orc;

        private DateTime m_NextBomb;
        private int m_Thrown;

        [Constructable]
        public PirateCrew() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, .2, .4)
        {
            Name = "Orcish Crew";

            Body = 0.33 > Utility.RandomDouble() ? 0x8C : Utility.RandomList(0xB5, 0xB6);

            SetHits(2000);

            if (Body == 0x8C) // Mage
            {
                SetStr(140, 160);
                SetDex(130, 150);
                SetInt(170, 190);
                SetDamage(4, 14);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 30, 40);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 20, 30);
                SetResistance(ResistanceType.Poison, 30, 40);
                SetResistance(ResistanceType.Energy, 30, 40);

                ChangeAIType(AIType.AI_Mage);

                SetSkill(SkillName.Wrestling, 45.0, 55.0);
                SetSkill(SkillName.Tactics, 50.0, 60.0);
                SetSkill(SkillName.MagicResist, 65.0, 75.0);
                SetSkill(SkillName.Magery, 60.0, 70.0);
                SetSkill(SkillName.EvalInt, 60.0, 75.0);
                SetSkill(SkillName.Meditation, 70.0, 90.0);
                SetSkill(SkillName.Focus, 80.0, 100.0);
            }
            else if (Body == 0xB6) // Bomber
            {
                SetStr(150, 200);
                SetDex(90, 110);
                SetInt(70, 100);
                SetDamage(1, 8);

                SetDamageType(ResistanceType.Physical, 75);
                SetDamageType(ResistanceType.Fire, 25);

                SetResistance(ResistanceType.Physical, 20, 30);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 15, 25);
                SetResistance(ResistanceType.Poison, 15, 25);
                SetResistance(ResistanceType.Energy, 20, 30);

                SetSkill(SkillName.Wrestling, 60.0, 90.0);
                SetSkill(SkillName.Tactics, 70.0, 85.0);
                SetSkill(SkillName.MagicResist, 70.0, 85.0);
            }
            else // Archer 
            {
                SetStr(100, 130);
                SetDex(100, 130);
                SetInt(30, 70);
                SetDamage(5, 7);

                SetDamageType(ResistanceType.Physical, 100);

                SetResistance(ResistanceType.Physical, 20, 35);
                SetResistance(ResistanceType.Fire, 30, 40);
                SetResistance(ResistanceType.Cold, 15, 25);
                SetResistance(ResistanceType.Poison, 15, 25);
                SetResistance(ResistanceType.Energy, 20, 30);

                ChangeAIType(AIType.AI_Archer);

                SetSkill(SkillName.Tactics, 55.0, 70.0);
                SetSkill(SkillName.MagicResist, 50.0, 70.0);
                SetSkill(SkillName.Anatomy, 60.0, 85.0);
                SetSkill(SkillName.Healing, 60.0, 80.0);
                SetSkill(SkillName.Archery, 100.0, 120.0);

                Item bow;

                switch (Utility.Random(4))
                {
                    default:
                    case 0: bow = new CompositeBow(); break;
                    case 1: bow = new Crossbow(); break;
                    case 2: bow = new Bow(); break;
                    case 3: bow = new HeavyCrossbow(); break;
                }

                AddItem(bow);
            }

            SetSkill(SkillName.DetectHidden, 40.0, 45.0);

            Fame = 8000;
            Karma = -8000;
        }

        public override void OnActionCombat()
        {
            if (Body == 0xB6)
            {
                Mobile combatant = Combatant as Mobile;

                if (combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 12) || !CanBeHarmful(combatant) || !InLOS(combatant))
                    return;

                if (DateTime.Now >= m_NextBomb)
                {
                    ThrowBomb(combatant);

                    m_Thrown++;

                    if (0.75 >= Utility.RandomDouble() && (m_Thrown % 2) == 1) // 75% chance to quickly throw another bomb
                        m_NextBomb = DateTime.Now + TimeSpan.FromSeconds(3.0);
                    else
                        m_NextBomb = DateTime.Now + TimeSpan.FromSeconds(5.0 + (10.0 * Utility.RandomDouble())); // 5-15 seconds
                }
            }
        }

        public void ThrowBomb(Mobile m)
        {
            DoHarmful(m);

            MovingParticles(m, 0x1C19, 1, 0, false, true, 0, 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0);

            new InternalTimer(m, this).Start();
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;

            public InternalTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0))
            {
                m_Mobile = m;
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_Mobile.PlaySound(0x11D);
                AOS.Damage(m_Mobile, m_From, Utility.RandomMinMax(10, 20), 0, 100, 0, 0, 0);
            }
        }

        public override int TreasureMapLevel => 3;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich, 2);
            AddLoot(LootPack.LootItem<Arrow>(25, true));
            AddLoot(LootPack.LootItem<Bolt>(25, true));
        }

        public PirateCrew(Serial serial)
            : base(serial)
        {
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
    }

}
