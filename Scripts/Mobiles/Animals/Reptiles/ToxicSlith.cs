using System;

namespace Server.Mobiles
{
    [CorpseName("a slith corpse")]
    public class ToxicSlith : BaseCreature
    {
        [Constructable]
        public ToxicSlith()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a toxic slith";
            this.Body = 734; 

            this.SetStr(219, 330);
            this.SetDex(46, 65);
            this.SetInt(25, 38);

            this.SetHits(182, 209);
            this.SetStam(230, 279);
			this.SetMana(0, 3);

            this.SetDamage(6, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 36, 44);
            this.SetResistance(ResistanceType.Fire, 6, 10);
            this.SetResistance(ResistanceType.Cold, 6, 10);
            this.SetResistance(ResistanceType.Poison, 100, 100);
            this.SetResistance(ResistanceType.Energy, 6, 10);

            this.SetSkill(SkillName.MagicResist, 95.4, 98.9);
            this.SetSkill(SkillName.Tactics, 84.3, 91.5);
            this.SetSkill(SkillName.Wrestling, 89.3, 97.9);

            this.Tamable = false;
            this.ControlSlots = 1;
            this.MinTameSkill = 80.7;
        }

        public ToxicSlith(Serial serial)
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
                return 6;
            }
        }
        public override int DragonBlood{ get{ return 6; } }
        public override int Hides
        {
            get
            {
                return 11;
            }
        }
        public override void GenerateLoot()
        {
            //PackItem(Gold(UtilityRandom(400, 500);
            //PackItem(ToxicVenomSac);
            //PackItem(SlithTongue);
            //PackItem(PotteryFragment);
            //PackItem(TatteredScroll);
            this.AddLoot(LootPack.Average, 2);
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