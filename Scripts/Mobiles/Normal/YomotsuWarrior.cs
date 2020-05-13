using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a yomotsu corpse")]
    public class YomotsuWarrior : BaseCreature
    {
        [Constructable]
        public YomotsuWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a yomotsu warrior";
            Body = 245;
            BaseSoundID = 0x452;

            SetStr(486, 530);
            SetDex(151, 165);
            SetInt(17, 31);

            SetHits(486, 530);
            SetMana(17, 31);

            SetDamage(8, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 65, 85);
            SetResistance(ResistanceType.Fire, 30, 50);
            SetResistance(ResistanceType.Cold, 45, 65);
            SetResistance(ResistanceType.Poison, 35, 55);
            SetResistance(ResistanceType.Energy, 25, 50);

            SetSkill(SkillName.Anatomy, 85.1, 95.0);
            SetSkill(SkillName.MagicResist, 82.6, 90.5);
            SetSkill(SkillName.Tactics, 95.1, 105.0);
            SetSkill(SkillName.Wrestling, 97.6, 107.5);

            Fame = 4200;
            Karma = -4200;

            SetWeaponAbility(WeaponAbility.DoubleStrike);
        }

        public YomotsuWarrior(Serial serial)
            : base(serial)
        {
        }

        public override FoodType FavoriteFood => FoodType.Fish;
        public override int Meat => 1;
        public override bool CanRummageCorpses => true;
        public override int TreasureMapLevel => 3;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Rich, 2);
            AddLoot(LootPack.Gems, 2);
            AddLoot(LootPack.LootItem<GreenGourd>(1.0));
            AddLoot(LootPack.BonsaiSeed);
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
            writer.Write(0);
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
