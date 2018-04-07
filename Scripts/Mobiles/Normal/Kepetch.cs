using System;
using Server.Items;

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

        public override int Meat { get { return 5; } }
        public override int Hides { get { return 14; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override FoodType FavoriteFood { get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }
        public override int DragonBlood { get { return 8; } }

        public bool Carve(Mobile from, Item item)
        {
            if (!GatheredFur)
            {
                var fur = new KepetchFur(30);

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
                from.SendLocalizedMessage(1112358); // The Kepetch nimbly escapes your attempts to shear its mane.

            return false;
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            base.OnCarve(from, corpse, with);

            if (!GatheredFur)
            {
                from.SendLocalizedMessage(1112765); // You shear it, and the fur is now on the corpse.
                corpse.AddCarvedItem(new KepetchFur(15), from);
                GatheredFur = true;
            }
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
            var version = reader.ReadInt();

            if (version == 1)
                reader.ReadDeltaTime();
            else
                GatheredFur = reader.ReadBool();
        }
    }
}
