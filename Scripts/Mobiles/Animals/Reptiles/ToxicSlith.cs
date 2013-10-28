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

            this.SetStr(223, 306);
            this.SetDex(231, 258);
            this.SetInt(30, 35);

            this.SetHits(197, 215);
            this.SetStam(231, 258);

            this.SetDamage(6, 24);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 0, 9);
            this.SetResistance(ResistanceType.Cold, 5, 10);
            this.SetResistance(ResistanceType.Poison, 100, 100);
            this.SetResistance(ResistanceType.Energy, 5, 7);

            this.SetSkill(SkillName.MagicResist, 95.4, 98.3);
            this.SetSkill(SkillName.Tactics, 85.5, 90.9);
            this.SetSkill(SkillName.Wrestling, 90.4, 95.1);

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
        //public ovverride int DragonBlood{ get{ return 6; } }
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