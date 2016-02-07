using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an ore elemental corpse")]
    public class VeriteElemental : BaseCreature
    {
        [Constructable]
        public VeriteElemental()
            : this(2)
        {
        }

        [Constructable]
        public VeriteElemental(int oreAmount)
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a verite elemental";
            this.Body = 113;
            this.BaseSoundID = 268;

            this.SetStr(226, 255);
            this.SetDex(126, 145);
            this.SetInt(71, 92);

            this.SetHits(136, 153);

            this.SetDamage(9, 16);

            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 30, 40);
            this.SetResistance(ResistanceType.Fire, 10, 20);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.MagicResist, 50.1, 95.0);
            this.SetSkill(SkillName.Tactics, 60.1, 100.0);
            this.SetSkill(SkillName.Wrestling, 60.1, 100.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 35;

            Item ore = new VeriteOre(oreAmount);
            ore.ItemID = 0x19B9;
            this.PackItem(ore);
        }

        public VeriteElemental(Serial serial)
            : base(serial)
        {
        }

        public override bool AutoDispel
        {
            get
            {
                return true;
            }
        }
        public override bool BleedImmune
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
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.Gems, 2);
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