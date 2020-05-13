using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a juka corpse")]
    public class JukaWarrior : BaseCreature
    {
        [Constructable]
        public JukaWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a juka warrior";
            Body = 764;

            SetStr(251, 350);
            SetDex(61, 80);
            SetInt(101, 150);

            SetHits(151, 210);

            SetDamage(7, 9);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 40, 50);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 80.1, 90.0);
            SetSkill(SkillName.Fencing, 80.1, 90.0);
            SetSkill(SkillName.Macing, 80.1, 90.0);
            SetSkill(SkillName.MagicResist, 120.1, 130.0);
            SetSkill(SkillName.Swords, 80.1, 90.0);
            SetSkill(SkillName.Tactics, 80.1, 90.0);
            SetSkill(SkillName.Wrestling, 80.1, 90.0);

            Fame = 10000;
            Karma = -10000;
        }

        public JukaWarrior(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer => true;
        public override bool CanRummageCorpses => true;
        public override int Meat => 1;
        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
            AddLoot(LootPack.Gems, 1);
            AddLoot(LootPack.LootItem<ArcaneGem>(10.0));
        }

        public override int GetIdleSound()
        {
            return 0x1AC;
        }

        public override int GetAngerSound()
        {
            return 0x1CD;
        }

        public override int GetHurtSound()
        {
            return 0x1D0;
        }

        public override int GetDeathSound()
        {
            return 0x28D;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.2 < Utility.RandomDouble())
                return;

            switch (Utility.Random(3))
            {
                case 0:
                    {
                        defender.SendLocalizedMessage(1004014); // You have been stunned!
                        defender.Freeze(TimeSpan.FromSeconds(4.0));
                        break;
                    }
                case 1:
                    {
                        defender.SendAsciiMessage("You have been hit by a paralyzing blow!");
                        defender.Freeze(TimeSpan.FromSeconds(3.0));
                        break;
                    }
                case 2:
                    {
                        AOS.Damage(defender, this, Utility.Random(10, 5), 100, 0, 0, 0, 0);
                        defender.SendAsciiMessage("You have been hit by a critical strike!");
                        break;
                    }
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
    }
}
