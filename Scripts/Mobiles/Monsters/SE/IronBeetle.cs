//Iron Beetle Beta Release 
using System;

namespace Server.Mobiles
{
    [CorpseName("a Iron Beetle corpse")]
    public class IronBeetle : BaseCreature
    {
        [Constructable]
        public IronBeetle()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)// AI Type??
        {
            this.Name = "Iron Beetle";
            this.Body = 714;

            this.SetStr(809, 866);
            this.SetDex(66, 73);
            this.SetInt(39, 50);

            this.SetHits(806, 878);
            this.SetStam(66, 73);
            this.SetMana(39, 50);

            this.SetDamage(9, 20);// Check Damage

            this.SetDamageType(ResistanceType.Physical, 100);//Check Damage Type

            this.SetResistance(ResistanceType.Physical, 50, 64);
            this.SetResistance(ResistanceType.Fire, 22, 27);
            this.SetResistance(ResistanceType.Cold, 21, 27);
            this.SetResistance(ResistanceType.Poison, 21, 30);
            this.SetResistance(ResistanceType.Energy, 47, 53);

            this.SetSkill(SkillName.Anatomy, 80.6, 89.5);
            this.SetSkill(SkillName.MagicResist, 123.2, 129.4);
            this.SetSkill(SkillName.Tactics, 83.4, 96.4);
            this.SetSkill(SkillName.Wrestling, 94.0, 107.1);

            this.Tamable = true;
            this.ControlSlots = 4;
            this.MinTameSkill = 39.1;//Check Required Skills (Any Skill controlable?)
        }

        public IronBeetle(Serial serial)
            : base(serial)
        {
        }

        public override bool SubdueBeforeTame
        {
            get
            {
                return true;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Meat;
            }
        }//Check Food Type

        //ToDo/Check mining ability???
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Meager, 2);
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