using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a slith corpse")]
    public class StoneSlith : BaseCreature
    {
        [Constructable]
        public StoneSlith()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a stone slith";
            this.Body = 734; 

            this.SetStr(250, 300);
            this.SetDex(76, 90);
            this.SetInt(34, 69);

            this.SetHits(154, 166);
            this.SetStam(76, 90);
            this.SetMana(34, 69);

            this.SetDamage(6, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 55);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 10, 20);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.MagicResist, 86.8, 95.1);
            this.SetSkill(SkillName.Tactics, 82.6, 88.6);
            this.SetSkill(SkillName.Wrestling, 75.8, 87.4);
            this.SetSkill(SkillName.Anatomy, 0.0, 2.9);

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 65.1;
        }

        public StoneSlith(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }// fire breath enabled
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        //public ovverride int DragonBlood{ get{ return 6; } }
        public override int Hides
        {
            get
            {
                return 12;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Spined;
            }
        }
        public override void GenerateLoot()
        {
            //PackItem(Gold(UtilityRandom(100, 200);
            //PackItem(SlithTongue);
            //PackItem(PotteryFragment);
            this.AddLoot(LootPack.Average, 2);
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.BleedAttack;
            //return WeaponAbility.LowerPhysicalResist;
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