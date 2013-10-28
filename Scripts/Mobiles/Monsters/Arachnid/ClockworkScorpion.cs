using System;

namespace Server.Mobiles
{
    [CorpseName("a clockwork scorpion corpse")]
    public class ClockworkScorpion : BaseCreature
    {
        [Constructable]
        public ClockworkScorpion()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            this.Name = "a clockwork scorpion";
            this.Body = 717;

            this.SetStr(225, 245);
            this.SetDex(80, 100);
            this.SetInt(30, 40);

            this.SetHits(151, 210);

            this.SetDamage(5, 10);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 80, 100);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 60, 80);
            this.SetResistance(ResistanceType.Poison, 100);
            this.SetResistance(ResistanceType.Energy, 10, 25);

            this.SetSkill(SkillName.MagicResist, 30.1, 50.0);
            this.SetSkill(SkillName.Poisoning, 95.1, 100.0);
            this.SetSkill(SkillName.Tactics, 70.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 80.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.ControlSlots = 1;
        }

        public ClockworkScorpion(Serial serial)
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
                return FoodType.Meat;
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
        public override bool DeleteOnRelease
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
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager, 2);
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

        /*
        public override void OnGaveMeleeAttack( Mobile defender )
        {
        base.OnGaveMeleeAttack( defender );

        if ( !m_Stunning && 0.3 > Utility.RandomDouble() )
        {
        m_Stunning = true;

        defender.Animate( 21, 6, 1, true, false, 0 );
        this.PlaySound( 0xEE );
        defender.LocalOverheadMessage( MessageType.Regular, 0x3B2, false, "You have been stunned by a colossal blow!" );

        BaseWeapon weapon = this.Weapon as BaseWeapon;
        if ( weapon != null )
        weapon.OnHit( this, defender );

        if ( defender.Alive )
        {
        defender.Frozen = true;
        Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( Recover_Callback ), defender );
        }
        }
        }
        */
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            Mobile master = this.GetMaster();

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
    }
}