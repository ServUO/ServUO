using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a boura corpse")]
    public class HighPlainsBoura : BaseCreature
    {
        private bool m_Stunning;
        [Constructable]
        public HighPlainsBoura()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a high plains boura";
            this.Body = 715;

            this.SetStr(400, 435);
            this.SetDex(90, 96);
            this.SetInt(25, 30);

            this.SetHits(555, 618);

            this.SetDamage(20, 25);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 35, 40);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 95.2, 105.4);
            this.SetSkill(SkillName.MagicResist, 60.7, 70.0);
            this.SetSkill(SkillName.Tactics, 95.4, 105.7);
            this.SetSkill(SkillName.Wrestling, 105.1, 115.3);

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 47.1;
        }

        public HighPlainsBoura(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 10;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Horned;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }
        public override int GetIdleSound()
        {
            return 1507;
        }

        public override int GetAngerSound()
        {
            return 1504;
        }

        public override int GetHurtSound()
        {
            return 1506;
        }

        public override int GetDeathSound()
        {
            return 1505;
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