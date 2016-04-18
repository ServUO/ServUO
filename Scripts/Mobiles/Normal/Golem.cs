using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a golem corpse")]
    public class Golem : BaseCreature
    {
        private bool m_Stunning;
        [Constructable]
        public Golem()
            : this(false, 1.0)
        {
        }

        [Constructable]
        public Golem(bool summoned, double scalar)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            this.Name = "a golem";
            this.Body = 752;

            if (summoned)
                this.Hue = 2101;

            this.SetStr((int)(251 * scalar), (int)(350 * scalar));
            this.SetDex((int)(76 * scalar), (int)(100 * scalar));
            this.SetInt((int)(101 * scalar), (int)(150 * scalar));

            this.SetHits((int)(151 * scalar), (int)(210 * scalar));

            this.SetDamage((int)(13 * scalar), (int)(24 * scalar));

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, (int)(35 * scalar), (int)(55 * scalar));

            if (summoned)
                this.SetResistance(ResistanceType.Fire, (int)(50 * scalar), (int)(60 * scalar));
            else
                this.SetResistance(ResistanceType.Fire, (int)(100 * scalar));

            this.SetResistance(ResistanceType.Cold, (int)(10 * scalar), (int)(30 * scalar));
            this.SetResistance(ResistanceType.Poison, (int)(10 * scalar), (int)(25 * scalar));
            this.SetResistance(ResistanceType.Energy, (int)(30 * scalar), (int)(40 * scalar));

            this.SetSkill(SkillName.MagicResist, (150.1 * scalar), (190.0 * scalar));
            this.SetSkill(SkillName.Tactics, (60.1 * scalar), (100.0 * scalar));
            this.SetSkill(SkillName.Wrestling, (60.1 * scalar), (100.0 * scalar));

            if (summoned)
            {
                this.Fame = 10;
                this.Karma = 10;
            }
            else
            {
                this.Fame = 3500;
                this.Karma = -3500;
            }

            if (!summoned)
            {
                this.PackItem(new IronIngot(Utility.RandomMinMax(13, 21)));

                if (0.1 > Utility.RandomDouble())
                    this.PackItem(new PowerCrystal());

                if (0.15 > Utility.RandomDouble())
                    this.PackItem(new ClockworkAssembly());

                if (0.2 > Utility.RandomDouble())
                    this.PackItem(new ArcaneGem());

                if (0.25 > Utility.RandomDouble())
                    this.PackItem(new Gears());
            }

            this.ControlSlots = 3;
        }

        public Golem(Serial serial)
            : base(serial)
        {
        }

        public override bool IsScaredOfScaryThings
        {
            get
            {
                return false;
            }
        }
        public override bool IsScaryToPets
        {
            get
            {
                return true;
            }
        }
        public override bool IsBondable
        {
            get
            {
                return false;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.None;
            }
        }
        public override bool CanBeDistracted
        {
            get
            {
                return false;
            }
        }
        public override bool DeleteOnRelease
        {
            get
            {
                return true;
            }
        }
        public override bool AutoDispel
        {
            get
            {
                return !this.Controlled;
            }
        }
        public override bool BleedImmune
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
                return !Core.AOS || this.Controlled;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
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

        public override int GetAngerSound()
        {
            return 541;
        }

        public override int GetIdleSound()
        {
            if (!this.Controlled)
                return 542;

            return base.GetIdleSound();
        }

        public override int GetDeathSound()
        {
            if (!this.Controlled)
                return 545;

            return base.GetDeathSound();
        }

        public override int GetAttackSound()
        {
            return 562;
        }

        public override int GetHurtSound()
        {
            if (this.Controlled)
                return 320;

            return base.GetHurtSound();
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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (this.Controlled || this.Summoned)
            {
                Mobile master = (this.ControlMaster);

                if (master == null)
                    master = this.SummonMaster;

                if (master != null && master.Player && master.Map == this.Map && master.InRange(this.Location, 20))
                {
                    if (master.Mana >= amount)
                    {
                        master.Mana -= amount;
                    }
                    else
                    {
                        amount -= master.Mana;
                        master.Mana = 0;
                        master.Damage(amount);
                    }
                }
            }

            base.OnDamage(amount, from, willKill);
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