using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fire beetle corpse")]
    [Server.Engines.Craft.Forge]
    public class FireBeetle : BaseMount
    {
        [Constructable]
        public FireBeetle()
            : base("a fire beetle", 0xA9, 0x3E95, AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.SetStr(300);
            this.SetDex(100);
            this.SetInt(500);

            this.SetHits(200);

            this.SetDamage(7, 20);

            this.SetDamageType(ResistanceType.Physical, 0);
            this.SetDamageType(ResistanceType.Fire, 100);

            this.SetResistance(ResistanceType.Physical, 40);
            this.SetResistance(ResistanceType.Fire, 70, 75);
            this.SetResistance(ResistanceType.Cold, 10);
            this.SetResistance(ResistanceType.Poison, 30);
            this.SetResistance(ResistanceType.Energy, 30);

            this.SetSkill(SkillName.MagicResist, 90.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 4000;
            this.Karma = -4000;

            this.Tamable = true;
            this.ControlSlots = 3;
            this.MinTameSkill = 93.9;

            this.PackItem(new SulfurousAsh(Utility.RandomMinMax(16, 25)));
            this.PackItem(new IronIngot(2));

            this.Hue = 0x489;
        }

        public FireBeetle(Serial serial)
            : base(serial)
        {
        }

        public override bool SubdueBeforeTame
        {
            get
            {
                return true;
            }
        }// Must be beaten into submission
        public override bool StatLossAfterTame
        {
            get
            {
                return true;
            }
        }
        public virtual double BoostedSpeed
        {
            get
            {
                return 0.1;
            }
        }
        public override bool ReduceSpeedWithDamage
        {
            get
            {
                return false;
            }
        }
        public override int Meat
        {
            get
            {
                return 16;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }
        public override void OnHarmfulSpell(Mobile from)
        {
            if (!this.Controlled && this.ControlMaster == null)
                this.CurrentSpeed = this.BoostedSpeed;
        }

        public override void OnCombatantChange()
        {
            if (this.Combatant == null && !this.Controlled && this.ControlMaster == null)
                this.CurrentSpeed = this.PassiveSpeed;
        }

        public override bool OverrideBondingReqs()
        {
            return true;
        }

        public override int GetAngerSound()
        {
            return 0x21D;
        }

        public override int GetIdleSound()
        {
            return 0x21D;
        }

        public override int GetAttackSound()
        {
            return 0x162;
        }

        public override int GetHurtSound()
        {
            return 0x163;
        }

        public override int GetDeathSound()
        {
            return 0x21D;
        }

        public override double GetControlChance(Mobile m, bool useBaseSkill)
        {
            return 1.0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                this.Hue = 0x489;
        }
    }
}