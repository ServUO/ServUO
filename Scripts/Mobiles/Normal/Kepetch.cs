using System;
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
            : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.Name = "a kepetch";
            this.Body = 726;

            this.SetStr(337, 354);
            this.SetDex(184, 194);
            this.SetInt(32, 37);

            this.SetHits(308, 366);

            this.SetDamage(7, 17);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 55, 65);
            this.SetResistance(ResistanceType.Fire, 40, 45);
            this.SetResistance(ResistanceType.Cold, 45, 55);
            this.SetResistance(ResistanceType.Poison, 55, 65);
            this.SetResistance(ResistanceType.Energy, 65, 75);

            this.SetSkill(SkillName.Anatomy, 119.7, 124.1);
            this.SetSkill(SkillName.MagicResist, 89.9, 97.4);
            this.SetSkill(SkillName.Tactics, 117.4, 123.5);
            this.SetSkill(SkillName.Wrestling, 107.7, 113.9);

            this.QLPoints = 10;
        }

        public Kepetch(Serial serial)
            : base(serial)
        {
        }
        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 14;
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
                return FoodType.FruitsAndVegies | FoodType.GrainsAndHay;
            }
        }

        public void Carve(Mobile from, Item item)
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
                }
            }
            else
                from.SendLocalizedMessage(1112358); // The Kepetch nimbly escapes your attempts to shear its mane.
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
            this.AddLoot(LootPack.Average, 2);
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