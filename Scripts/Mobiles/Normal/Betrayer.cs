using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a betrayer corpse")]
    public class Betrayer : BaseCreature
    {
        private DateTime m_NextAbilityTime;
        [Constructable]
        public Betrayer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a betrayer";
            Body = 767;

            SetStr(401, 500);
            SetDex(81, 100);
            SetInt(151, 200);

            SetHits(241, 300);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 60, 70);
            SetResistance(ResistanceType.Fire, 60, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 30, 40);
            SetResistance(ResistanceType.Energy, 20, 30);

            SetSkill(SkillName.Anatomy, 90.1, 100.0);
            SetSkill(SkillName.EvalInt, 90.1, 100.0);
            SetSkill(SkillName.Magery, 50.1, 100.0);
            SetSkill(SkillName.Meditation, 90.1, 100.0);
            SetSkill(SkillName.MagicResist, 120.1, 130.0);
            SetSkill(SkillName.Tactics, 90.1, 100.0);
            SetSkill(SkillName.Wrestling, 90.1, 100.0);

            Fame = 15000;
            Karma = -15000;

            SpeechHue = Utility.RandomDyedHue();

            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 30));
            SetSpecialAbility(SpecialAbility.ColossalBlow);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems);
            AddLoot(LootPack.LootItem<BlackthornWelcomeBook>(2.0));
            AddLoot(LootPack.LootItem<PowerCrystal>());
            AddLoot(LootPack.LootItemCallback(Golem.SpawnGears, 5.0, 1, false, false));
        }

        public Betrayer(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override Poison PoisonImmune => Poison.Lethal;
        public override int Meat => 1;
        public override int TreasureMapLevel => 5;

        public override int GetDeathSound()
        {
            return 0x423;
        }

        public override int GetAttackSound()
        {
            return 0x23B;
        }

        public override int GetHurtSound()
        {
            return 0x140;
        }

        public override void OnActionCombat()
        {
            Mobile combatant = Combatant as Mobile;

            if (DateTime.UtcNow < m_NextAbilityTime || combatant == null || combatant.Deleted || combatant.Map != Map || !InRange(combatant, 3) || !CanBeHarmful(combatant) || !InLOS(combatant))
                return;

            m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 30));

            if (Utility.RandomBool())
            {
                FixedParticles(0x376A, 9, 32, 0x2539, EffectLayer.LeftHand);
                PlaySound(0x1DE);

                IPooledEnumerable eable = GetMobilesInRange(2);

                foreach (Mobile m in eable)
                {
                    if (m != this && IsEnemy(m))
                    {
                        m.ApplyPoison(this, Poison.Deadly);
                    }
                }

                eable.Free();
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
        }
    }
}
