using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a dragon turtle hatchling corpse")]
    public class DragonTurtleHatchling : BaseCreature
    {
        [Constructable]
        public DragonTurtleHatchling()
            : base(AIType.AI_Mage, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Body = 1294;
            BaseSoundID = 362;
            Name = "a dragon turtle hatchling";

            SetStr(550, 650);
            SetDex(55, 65);
            SetInt(550, 650);

            SetHits(550, 850);

            SetDamage(24, 33);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 50, 55);
            SetResistance(ResistanceType.Poison, 60);
            SetResistance(ResistanceType.Energy, 70, 75);

            SetSkill(SkillName.MagicResist, 130, 140);
            SetSkill(SkillName.Tactics, 110, 120);
            SetSkill(SkillName.Wrestling, 140, 150);
            SetSkill(SkillName.Magery, 130, 140);
            SetSkill(SkillName.EvalInt, 100, 119);

            Fame = 16000;
            Karma = -16000;

            Tamable = true;
            ControlSlots = 5;
            MinTameSkill = 104.7;

            SetWeaponAbility(WeaponAbility.BleedAttack);
            SetSpecialAbility(SpecialAbility.DragonBreath);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average, 3);
        }

        public override void OnAfterTame(Mobile tamer)
        {
            if (Owners.Count == 0)
            {
                SkillsCap = Skills.Total;

                foreach (Skill sk in Skills)
                {
                    if (sk.Base > 0)
                    {
                        sk.Cap = Math.Max(100, sk.Base - (sk.Base * 10));
                        sk.Base = sk.Base - (sk.Base * .55);
                    }
                }
            }
        }

        public override int Meat => 4;
        public override FoodType FavoriteFood => FoodType.Fish;

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!Controlled && corpse != null && !corpse.Carved)
            {
                from.SendLocalizedMessage(1156198); // You cut away some pelts, but they remain on the corpse.
                corpse.DropItem(new DragonTurtleScute(4));
            }

            base.OnCarve(from, corpse, with);
        }

        public DragonTurtleHatchling(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                SetWeaponAbility(WeaponAbility.BleedAttack);
            }
        }
    }
}
