using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a betrayer corpse")]
    public class Betrayer : BaseCreature
    {
        private bool m_Stunning;
        private DateTime m_NextAbilityTime;
        [Constructable]
        public Betrayer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a betrayer";
            this.Body = 767;

            this.SetStr(401, 500);
            this.SetDex(81, 100);
            this.SetInt(151, 200);

            this.SetHits(241, 300);

            this.SetDamage(16, 22);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 60, 70);
            this.SetResistance(ResistanceType.Fire, 60, 70);
            this.SetResistance(ResistanceType.Cold, 60, 70);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 20, 30);

            this.SetSkill(SkillName.Anatomy, 90.1, 100.0);
            this.SetSkill(SkillName.EvalInt, 90.1, 100.0);
            this.SetSkill(SkillName.Magery, 50.1, 100.0);
            this.SetSkill(SkillName.Meditation, 90.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 120.1, 130.0);
            this.SetSkill(SkillName.Tactics, 90.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 90.1, 100.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 65;
            this.SpeechHue = Utility.RandomDyedHue();

            this.PackItem(new PowerCrystal());

            if (0.02 > Utility.RandomDouble())
                this.PackItem(new BlackthornWelcomeBook());

            this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 30));
        }

        public Betrayer(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool BardImmune
        {
            get
            {
                return !Core.AOS;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (0.05 > Utility.RandomDouble())
            {
                if (!this.IsParagon)
                {
                    if (0.75 > Utility.RandomDouble())
                        c.DropItem(DawnsMusicGear.RandomCommon);
                    else
                        c.DropItem(DawnsMusicGear.RandomUncommon);
                }
                else
                    c.DropItem(DawnsMusicGear.RandomRare);
            }
        }

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

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Gems, 1);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (!this.m_Stunning && 0.3 > Utility.RandomDouble())
            {
                this.m_Stunning = true;

                defender.Animate(21, 6, 1, true, false, 0);
                this.PlaySound(0xEE);
                defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "You have been stunned by a colossal blow!");

                BaseWeapon weapon = this.Weapon as BaseWeapon;
                if (weapon != null)
                    weapon.OnHit(this, defender);

                if (defender.Alive)
                {
                    defender.Frozen = true;
                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerStateCallback(Recover_Callback), defender);
                }
            }
        }

        public override void OnActionCombat()
        {
            Mobile combatant = this.Combatant;

            if (DateTime.UtcNow < this.m_NextAbilityTime || combatant == null || combatant.Deleted || combatant.Map != this.Map || !this.InRange(combatant, 3) || !this.CanBeHarmful(combatant) || !this.InLOS(combatant))
                return;

            this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 30));

            if (Utility.RandomBool())
            {
                this.FixedParticles(0x376A, 9, 32, 0x2539, EffectLayer.LeftHand);
                this.PlaySound(0x1DE);

                foreach (Mobile m in this.GetMobilesInRange(2))
                {
                    if (m != this && this.IsEnemy(m))
                    {
                        m.ApplyPoison(this, Poison.Deadly);
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private void Recover_Callback(object state)
        {
            Mobile defender = state as Mobile;

            if (defender != null)
            {
                defender.Frozen = false;
                defender.Combatant = null;
                defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "You recover your senses.");
            }

            this.m_Stunning = false;
        }
    }
}