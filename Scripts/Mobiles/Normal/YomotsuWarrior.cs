using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a yomotsu corpse")]
    public class YomotsuWarrior : BaseCreature
    {
        [Constructable]
        public YomotsuWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a yomotsu warrior";
            this.Body = 245;
            this.BaseSoundID = 0x452;

            this.SetStr(486, 530);
            this.SetDex(151, 165);
            this.SetInt(17, 31);

            this.SetHits(486, 530);
            this.SetMana(17, 31);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 85);
            this.SetResistance(ResistanceType.Fire, 30, 50);
            this.SetResistance(ResistanceType.Cold, 45, 65);
            this.SetResistance(ResistanceType.Poison, 35, 55);
            this.SetResistance(ResistanceType.Energy, 25, 50);

            this.SetSkill(SkillName.Anatomy, 85.1, 95.0);
            this.SetSkill(SkillName.MagicResist, 82.6, 90.5);
            this.SetSkill(SkillName.Tactics, 95.1, 105.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 107.5);

            this.Fame = 4200;	
            this.Karma = -4200;

            this.PackItem(new GreenGourd());
            this.PackItem(new ExecutionersAxe());

            if (Utility.RandomBool())
                this.PackItem(new LongPants());
            else
                this.PackItem(new ShortPants());

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.PackItem(new Shoes());
                    break;
                case 1:
                    this.PackItem(new Sandals());
                    break;
                case 2:
                    this.PackItem(new Boots());
                    break;
                case 3:
                    this.PackItem(new ThighBoots());
                    break;
            }

            if (Utility.RandomDouble() < .25)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public YomotsuWarrior(Serial serial)
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
                return 3;
            }
        }
        public override WeaponAbility GetWeaponAbility()
        {
            return WeaponAbility.DoubleStrike;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich, 2);
            this.AddLoot(LootPack.Gems, 2);
        }

        // TODO: Throwing Dagger
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