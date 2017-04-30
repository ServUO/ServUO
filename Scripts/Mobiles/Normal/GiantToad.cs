using System;

namespace Server.Mobiles
{
    [CorpseName("a giant toad corpse")]
    [TypeAlias("Server.Mobiles.Gianttoad")]
    public class GiantToad : BaseCreature
    {
        [Constructable]
        public GiantToad()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a giant toad";
            this.Body = 80;
            this.BaseSoundID = 0x26B;

            this.SetStr(76, 100);
            this.SetDex(6, 25);
            this.SetInt(11, 20);

            this.SetHits(46, 60);
            this.SetMana(0);

            this.SetDamage(5, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 25);
            this.SetResistance(ResistanceType.Fire, 5, 10);
            this.SetResistance(ResistanceType.Energy, 5, 10);

            this.SetSkill(SkillName.MagicResist, 25.1, 40.0);
            this.SetSkill(SkillName.Tactics, 40.1, 60.0);
            this.SetSkill(SkillName.Wrestling, 40.1, 60.0);

            this.Fame = 750;
            this.Karma = -750;

            this.VirtualArmor = 24;

            this.Tamable = true;
            this.ControlSlots = 1;
            this.MinTameSkill = 77.1;
            
            if (Utility.RandomDouble() < 0.2)
			{
				switch (Utility.Random(2))
				{
					case 0:
						{
							Hue = 191;
							break;
						}
					case 1:
						{
							Hue = 1166;
							break;
						}
				}
			}
        }

        public GiantToad(Serial serial)
            : base(serial)
        {
        }

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

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            if (version < 1)
            {
                this.AI = AIType.AI_Melee;
                this.FightMode = FightMode.Closest;
            }
        }
    }
}
