using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a glowing yomotsu corpse")]
    public class YomotsuPriest : BaseCreature
    {
        [Constructable]
        public YomotsuPriest()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a yomotsu priest";
            Body = 253;
            BaseSoundID = 0x452;

            SetStr(486, 530);
            SetDex(101, 115);
            SetInt(601, 670);

            SetHits(486, 530);

            SetDamage(8, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 45, 65);
            SetResistance(ResistanceType.Poison, 35, 55);
            SetResistance(ResistanceType.Energy, 25, 50);

            SetSkill(SkillName.EvalInt, 92.6, 107.5);
            SetSkill(SkillName.Magery, 105.1, 115.0);
            SetSkill(SkillName.Meditation, 100.1, 110.0);
            SetSkill(SkillName.MagicResist, 112.6, 122.5);
            SetSkill(SkillName.Tactics, 55.1, 105.0);
            SetSkill(SkillName.Wrestling, 47.6, 57.5);

            Fame = 9000;
            Karma = -9000;

            PackItem(new GreenGourd());
            PackItem(new ExecutionersAxe());

            switch ( Utility.Random(3) )
            {
                case 0:
                    PackItem(new LongPants());
                    break;
                case 1:
                    PackItem(new ShortPants());
                    break;
            }

            switch ( Utility.Random(6) )
            {
                case 0:
                    PackItem(new Shoes());
                    break;
                case 1:
                    PackItem(new Sandals());
                    break;
                case 2:
                    PackItem(new Boots());
                    break;
                case 3:
                    PackItem(new ThighBoots());
                    break;
            }

            if (Utility.RandomDouble() < .25)
                PackItem(Engines.Plants.Seed.RandomBonsaiSeed());

            SetWeaponAbility(WeaponAbility.DoubleStrike);
        }

        public YomotsuPriest(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Rich);
            AddLoot(LootPack.Gems, 4);
        }

        // TODO: Body Transformation
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Maniacal laugh
                * Cliloc: 1070840
                * Effect: Type: "3" From: "0x57D4F5B" To: "0x0" ItemId: "0x37B9" ItemIdName: "glow" FromLocation: "(884 715, 10)" ToLocation: "(884 715, 10)" Speed: "10" Duration: "5" FixedDirection: "True" Explode: "False"
                * Paralyzes for 4 seconds, or until hit
                */
                defender.FixedEffect(0x37B9, 10, 5);
                defender.SendLocalizedMessage(1070840); // You are frozen as the creature laughs maniacally.

                defender.Paralyze(TimeSpan.FromSeconds(4.0));
            }
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

        public override int GetIdleSound()
        {
            return 0x42A;
        }

        public override int GetAttackSound()
        {
            return 0x435;
        }

        public override int GetHurtSound()
        {
            return 0x436;
        }

        public override int GetDeathSound()
        {
            return 0x43A;
        }
    }
}