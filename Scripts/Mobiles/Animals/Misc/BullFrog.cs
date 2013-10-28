using System;

namespace Server.Mobiles
{
    [CorpseName("a bull frog corpse")]
    [TypeAlias("Server.Mobiles.Bullfrog")]
    public class BullFrog : BaseCreature
    {
        [Constructable]
        public BullFrog()
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bull frog";
            this.Body = 81;
            this.Hue = Utility.RandomList(0x5AC, 0x5A3, 0x59A, 0x591, 0x588, 0x57F);
            this.BaseSoundID = 0x266;

            this.SetStr(46, 70);
            this.SetDex(6, 25);
            this.SetInt(11, 20);

            this.SetHits(28, 42);
            this.SetMana(0);

            this.SetDamage(1, 2);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 5, 10);

            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 40.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 350;
            this.Karma = 0;

            this.VirtualArmor = 6;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 23.1;
        }

        public BullFrog(Serial serial)
            : base(serial)
        {
        }

        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override int Hides
        {
            get
            {
                return 4;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish | FoodType.Meat;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Poor);
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