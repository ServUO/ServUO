using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a jukan corpse")]
    public class JukaWarrior : BaseCreature
    {
        [Constructable]
        public JukaWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a juka warrior";
            this.Body = 764;

            this.SetStr(251, 350);
            this.SetDex(61, 80);
            this.SetInt(101, 150);

            this.SetHits(151, 210);

            this.SetDamage(7, 9);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 40, 50);
            this.SetResistance(ResistanceType.Fire, 30, 40);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 80.1, 90.0);
            this.SetSkill(SkillName.Fencing, 80.1, 90.0);
            this.SetSkill(SkillName.Macing, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 120.1, 130.0);
            this.SetSkill(SkillName.Swords, 80.1, 90.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);

            this.Fame = 10000;
            this.Karma = -10000;

            this.VirtualArmor = 22;

            if (Utility.RandomDouble() < 0.1)
                this.PackItem(new ArcaneGem());
        }

        public JukaWarrior(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Gems, 1);
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

            switch ( Utility.Random(3) )
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
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}