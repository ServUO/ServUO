using System;
using System.Linq;

using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a goat corpse")]
    public class Goat : BaseCreature
    {
        [Constructable]
        public Goat()
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a goat";
            Body = 0xD1;
            BaseSoundID = 0x99;

            SetStr(19);
            SetDex(15);
            SetInt(5);

            SetHits(12);
            SetMana(0);

            SetDamage(3, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 5, 15);

            SetSkill(SkillName.MagicResist, 5.0);
            SetSkill(SkillName.Tactics, 5.0);
            SetSkill(SkillName.Wrestling, 5.0);

            Fame = 150;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 11.1;
        }

        public Goat(Serial serial)
            : base(serial)
        {
        }

        public override int Meat => 2;
        public override int Hides => 8;
        public override FoodType FavoriteFood => FoodType.GrainsAndHay | FoodType.FruitsAndVegies;

        private static readonly Type[] _FeedTypes = new[]
        {
            typeof(Backpack), typeof(BaseShoes), typeof(Bag)
        };

        public override bool CheckFoodPreference(Item f)
        {
            if (!base.CheckFoodPreference(f))
            {
                if (f is BaseArmor && (((BaseArmor)f).MaterialType == ArmorMaterialType.Leather || ((BaseArmor)f).MaterialType == ArmorMaterialType.Studded))
                {
                    return true;
                }

                var type = f.GetType();

                return _FeedTypes.Any(t => t == type || type.IsSubclassOf(t));
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
