using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a kepetch corpse")]
    public class KepetchAmbusher : BaseCreature, ICarvable
    {
        public bool GatheredFur { get; set; }

        [Constructable]
        public KepetchAmbusher() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a kepetch ambusher";
            Body = 726;

            SetStr(440, 446);
            SetDex(229, 254);
            SetInt(46, 46);

            SetHits(533, 544);

            SetDamage(7, 17);

            SetDamageType(ResistanceType.Physical, 80);
            SetDamageType(ResistanceType.Poison, 20);

            SetResistance(ResistanceType.Physical, 73, 95);
            SetResistance(ResistanceType.Fire, 57, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 55, 65);
            SetResistance(ResistanceType.Energy, 70, 95);

            SetSkill(SkillName.Anatomy, 104.3, 114.1);
            SetSkill(SkillName.MagicResist, 94.6, 97.4);
            SetSkill(SkillName.Tactics, 110.4, 123.5);
            SetSkill(SkillName.Wrestling, 107.3, 113.9);
            SetSkill(SkillName.Stealth, 125.0);
            SetSkill(SkillName.Hiding, 125.0);

            Fame = 2500;
            Karma = -2500;

            QLPoints = 25;

            PackItem(new RawRibs(5));
            //	VirtualArmor = 16;
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

        public KepetchAmbusher(Serial serial) : base(serial)
        {
        }

        public override int Meat
        {
            get { return 7; }
        }

        public override int Hides
        {
            get { return 12; }
        }

        public override HideType HideType
        {
            get { return HideType.Horned; }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; }
        }

        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.ShadowStrike;
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

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new DragonBlood(6));

            if (Utility.RandomDouble() < 0.1)
            {
                c.DropItem(new KepetchWax());
            }
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