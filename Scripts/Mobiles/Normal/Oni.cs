using System;

namespace Server.Mobiles
{
    [CorpseName("an oni corpse")]
    public class Oni : BaseCreature
    {
        private DateTime m_NextAbilityTime;

        [Constructable]
        public Oni()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "an oni";
            this.Body = 241;

            this.SetStr(801, 910);
            this.SetDex(151, 300);
            this.SetInt(171, 195);

            this.SetHits(401, 530);

            this.SetDamage(14, 20);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Energy, 20);

            this.SetResistance(ResistanceType.Physical, 65, 80);
            this.SetResistance(ResistanceType.Fire, 50, 70);
            this.SetResistance(ResistanceType.Cold, 35, 50);
            this.SetResistance(ResistanceType.Poison, 45, 70);
            this.SetResistance(ResistanceType.Energy, 45, 65);

            this.SetSkill(SkillName.EvalInt, 100.1, 125.0);
            this.SetSkill(SkillName.Magery, 96.1, 106.0);
            this.SetSkill(SkillName.Anatomy, 85.1, 95.0);
            this.SetSkill(SkillName.MagicResist, 85.1, 100.0);
            this.SetSkill(SkillName.Tactics, 86.1, 101.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = -15000;

            if (Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public override void OnThink()
        {
            if (Combatant == null)
                return;

            if (!BardPacified)
            {
                if (DateTime.UtcNow >= m_NextAbilityTime)
                {
                    Mobile target = this.Combatant as Mobile;

                    if (target != null && target.InRange(this, 10))
                    {
                        AngryFire(target);
                        m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(10, 30));
                    }
                }
            }

            base.OnThink();
        }

        #region AngryFire
        private void AngryFire(Mobile defender)
        {
            int damage = defender.Hits / 2;

            AOS.Damage(defender, this, damage, 0, 100, 0, 0, 0);

            defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            defender.PlaySound(0x208);

            defender.SendLocalizedMessage(1070823); // The creature hits you with its Angry Fire.            
        }
        #endregion

        public Oni(Serial serial)
            : base(serial)
        {
        }

        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int GetAngerSound()
        {
            return 0x4E3;
        }

        public override int GetIdleSound()
        {
            return 0x4E2;
        }

        public override int GetAttackSound()
        {
            return 0x4E1;
        }

        public override int GetHurtSound()
        {
            return 0x4E4;
        }

        public override int GetDeathSound()
        {
            return 0x4E0;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich, 3);
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
    }
}