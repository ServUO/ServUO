using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a deathwatchbeetle hatchling corpse")]
    [TypeAlias("Server.Mobiles.DeathWatchBeetleHatchling")]
    public class DeathwatchBeetleHatchling : BaseCreature
    {
        [Constructable]
        public DeathwatchBeetleHatchling()
            : base(AIType.AI_Melee, Core.ML ? FightMode.Aggressor : FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a deathwatch beetle hatchling";
            this.Body = 242;

            this.SetStr(26, 50);
            this.SetDex(41, 52);
            this.SetInt(21, 30);

            this.SetHits(51, 60);
            this.SetMana(20);

            this.SetDamage(2, 8);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 40);
            this.SetResistance(ResistanceType.Fire, 15, 30);
            this.SetResistance(ResistanceType.Cold, 15, 30);
            this.SetResistance(ResistanceType.Poison, 20, 40);
            this.SetResistance(ResistanceType.Energy, 20, 35);

            this.SetSkill(SkillName.Wrestling, 30.1, 40.0);
            this.SetSkill(SkillName.Tactics, 47.1, 57.0);
            this.SetSkill(SkillName.MagicResist, 30.1, 38.0);
            this.SetSkill(SkillName.Anatomy, 20.1, 24.0);

            this.Fame = 700;
            this.Karma = -700;

            if (Utility.RandomBool())
            {
                Item i = Loot.RandomReagent();
                i.Amount = 3;
                this.PackItem(i);
            }
			
            switch ( Utility.Random(12) )
            {
                case 0:
                    this.PackItem(new LeatherGorget());
                    break;
                case 1:
                    this.PackItem(new LeatherGloves());
                    break;
                case 2:
                    this.PackItem(new LeatherArms());
                    break;
                case 3:
                    this.PackItem(new LeatherLegs());
                    break;
                case 4:
                    this.PackItem(new LeatherCap());
                    break;
                case 5:
                    this.PackItem(new LeatherChest());
                    break;
            }
        }

        public DeathwatchBeetleHatchling(Serial serial)
            : base(serial)
        {
        }

        public override int Hides
        {
            get
            {
                return 8;
            }
        }
        public override int GetAngerSound()
        {
            return 0x4F3;
        }

        public override int GetIdleSound()
        {
            return 0x4F2;
        }

        public override int GetAttackSound()
        {
            return 0x4F1;
        }

        public override int GetHurtSound()
        {
            return 0x4F4;
        }

        public override int GetDeathSound()
        {
            return 0x4F0;
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.LowScrolls, 1);
            this.AddLoot(LootPack.Potions, 1);
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