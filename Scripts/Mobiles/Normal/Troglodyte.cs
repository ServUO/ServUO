using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a troglodyte corpse")]
    public class Troglodyte : BaseCreature
    {
        public override double HealChance { get { return 1.0; } }

        [Constructable]
        public Troglodyte()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)// NEED TO CHECK
        {
            this.Name = "a troglodyte";
            this.Body = 267;
            this.BaseSoundID = 0x59F; 

            this.SetStr(148, 217);
            this.SetDex(91, 120);
            this.SetInt(51, 70);

            this.SetHits(302, 340);

            this.SetDamage(11, 14);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 30, 35);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 35, 40);
            this.SetResistance(ResistanceType.Poison, 30, 40);
            this.SetResistance(ResistanceType.Energy, 30, 40);

            this.SetSkill(SkillName.Anatomy, 70.5, 94.8);
            this.SetSkill(SkillName.MagicResist, 51.8, 65.0);
            this.SetSkill(SkillName.Tactics, 80.4, 94.7);
            this.SetSkill(SkillName.Wrestling, 70.2, 93.5);
            this.SetSkill(SkillName.Healing, 70.0, 95.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 28; // Don't know what it should be

            this.PackItem(new Bandage(5));  // How many?
            this.PackItem(new Ribs());

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Troglodyte(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);  // Need to verify
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