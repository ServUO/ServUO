//Iron Beetle Beta Release 
using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a Iron Beetle corpse")]
    public class IronBeetle : BaseCreature
    {
        [Constructable]
        public IronBeetle()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)// AI Type??
        {
            Name = "Iron Beetle";
            Body = 714;

            SetStr(809, 866);
            SetDex(66, 73);
            SetInt(39, 50);

            SetHits(806, 878);
            SetStam(66, 73);
            SetMana(39, 50);

            SetDamage(9, 20);// Check Damage

            SetDamageType(ResistanceType.Physical, 100);//Check Damage Type

            SetResistance(ResistanceType.Physical, 50, 64);
            SetResistance(ResistanceType.Fire, 22, 27);
            SetResistance(ResistanceType.Cold, 21, 27);
            SetResistance(ResistanceType.Poison, 21, 30);
            SetResistance(ResistanceType.Energy, 47, 53);

            SetSkill(SkillName.Anatomy, 80.6, 89.5);
            SetSkill(SkillName.MagicResist, 123.2, 129.4);
            SetSkill(SkillName.Tactics, 83.4, 96.4);
            SetSkill(SkillName.Wrestling, 94.0, 107.1);

            Tamable = true;
            ControlSlots = 4;
            MinTameSkill = 39.1;//Check Required Skills (Any Skill controlable?)

            QLPoints = 20;
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
            AddLoot(LootPack.Meager, 2);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomDouble() < 0.03)
                c.DropItem(new LuckyCoin());
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