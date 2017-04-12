using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ossein ram corpse")]
    public class OsseinRam : BaseCreature
    {
        [Constructable]
        public OsseinRam() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Ossein Ram";
            this.Body = 0x591;
            this.Female = true;

            this.SetStr(300, 400);
            this.SetDex(80, 100);
            this.SetInt(100, 120);

            this.SetHits(450, 550);

            this.SetDamage(18, 23);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Cold, 25);
            this.SetDamageType(ResistanceType.Energy, 25);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 40, 50);

            this.SetSkill(SkillName.MagicResist, 70.0, 80.0);
            this.SetSkill(SkillName.Tactics, 80.0, 90.0);
            this.SetSkill(SkillName.Wrestling, 90.0, 100.0);
            this.SetSkill(SkillName.DetectHidden, 40.0, 50.0);
            this.SetSkill(SkillName.Anatomy, 75.0, 85.0);
            this.SetSkill(SkillName.Necromancy, 20.0);
            this.SetSkill(SkillName.SpiritSpeak, 20.0);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 72.0;
        }

        public override int Meat { get { return 3; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomBool())
            {
                if (!Kappa.IsBeingDrained(defender) && this.Mana > 14)
                {
                    defender.SendLocalizedMessage(1070848); // You feel your life force being stolen away.
                    Kappa.BeginLifeDrain(defender, this);
                    this.Mana -= 15;
                }
            }
            else
            {
                Paralyze(defender);
            }
        }

        #region Paralyze
        private void Paralyze(Mobile defender)
        {
            defender.Paralyze(TimeSpan.FromSeconds(Utility.Random(3)));

            defender.FixedEffect(0x376A, 6, 1);
            defender.PlaySound(0x204);

            defender.SendLocalizedMessage(1060164); // The attack has temporarily paralyzed you!
        }
        #endregion

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.Disarm;
        }

        public override bool CanAngerOnTame { get { return true; } }
        public override bool StatLossAfterTame { get { return true; } }

        public OsseinRam(Serial serial) : base(serial)
        {
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
    }
}