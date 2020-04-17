using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class Kepetch : BaseCreature, ICarvable
    {
        public bool GatheredFur { get; set; }

        [Constructable]
        public Kepetch()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a kepetch";
            Body = 726;

            SetStr(337, 380);
            SetDex(184, 194);
            SetInt(30, 50);

            SetHits(300, 400);

            SetDamage(7, 17);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 55, 75);
            SetResistance(ResistanceType.Fire, 40, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.Anatomy, 119.7, 124.1);
            SetSkill(SkillName.MagicResist, 89.9, 97.4);
            SetSkill(SkillName.Tactics, 117.4, 123.5);
            SetSkill(SkillName.Wrestling, 107.7, 113.9);
            SetSkill(SkillName.DetectHidden, 25.0);
            SetSkill(SkillName.Parry, 60.0, 70.0);

            Fame = 6000;
            Karma = -6000;

            SetSpecialAbility(SpecialAbility.ViciousBite);
        }

        public Kepetch(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 5;
        public override int Hides => 14;
        public override HideType HideType => HideType.Spined;
        public override FoodType FavoriteFood => FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
        public override int DragonBlood => 8;
        public override int Fur => GatheredFur ? 0 : 15;
        public override FurType FurType => FurType.Brown;

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                Fur fur = new Fur(FurType, Fur);

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, fur, false))
                {
                    from.SendLocalizedMessage(1112359); // You would not be able to place the gathered kepetch fur in your backpack!
                    fur.Delete();
                }
                else
                {
                    from.SendLocalizedMessage(1112360); // You place the gathered kepetch fur into your backpack.
                    GatheredFur = true;
                    return true;
                }
            }
            else
            {
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, 1112358, from.NetState); // The Kepetch nimbly escapes your attempts to shear its mane.
            }

            return false;
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 2);
        }

        public override int GetIdleSound()
        {
            return 1545;
        }

        public override int GetAngerSound()
        {
            return 1542;
        }

        public override int GetHurtSound()
        {
            return 1544;
        }

        public override int GetDeathSound()
        {
            return 1543;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(2);

            writer.Write(GatheredFur);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 1)
                reader.ReadDeltaTime();
            else
                GatheredFur = reader.ReadBool();
        }
    }
}
