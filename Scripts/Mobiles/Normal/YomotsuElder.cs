using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wrinkly yomotsu corpse")]
    public class YomotsuElder : BaseCreature
    {
        [Constructable]
        public YomotsuElder()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a yomotsu elder";
            Body = 255;
            BaseSoundID = 0x452;

            SetStr(686, 830);
            SetDex(251, 365);
            SetInt(17, 31);

            SetHits(801, 900);

            SetDamage(19, 27);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 45, 65);
            SetResistance(ResistanceType.Poison, 35, 55);
            SetResistance(ResistanceType.Energy, 25, 50);

            SetSkill(SkillName.Anatomy, 115.1, 130.0);
            SetSkill(SkillName.MagicResist, 100.1, 120.0);
            SetSkill(SkillName.Tactics, 115.1, 130.0);
            SetSkill(SkillName.Wrestling, 110.1, 130.0);

            Fame = 12000;
            Karma = -12000;

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

        public YomotsuElder(Serial serial)
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
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich, 3);
            AddLoot(LootPack.Gems, 2);
        }

        // TODO: Axe Throw
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