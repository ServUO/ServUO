using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class EnragedEarthElemental : BaseCreature
    {
        [Constructable]
        public EnragedEarthElemental()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Enraged Earth Elemental";
            this.Body = 14;
            this.BaseSoundID = 268;

            this.SetStr(147, 155);
            this.SetDex(78, 89);
            this.SetInt(94, 110);

            this.SetHits(500, 505);
			this.SetMana(94, 110);
			this.SetStam(78, 89);

            this.SetDamage(9, 16);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 59, 65);
            this.SetResistance(ResistanceType.Fire, 20, 30);
            this.SetResistance(ResistanceType.Cold, 21, 28);
            this.SetResistance(ResistanceType.Poison, 47, 51);
            this.SetResistance(ResistanceType.Energy, 30, 33);

            this.SetSkill(SkillName.MagicResist, 100.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 120.0);

            this.Fame = 3500;
            this.Karma = -3500;

            this.VirtualArmor = 34;
            this.ControlSlots = 2;

            this.PackItem(new FertileDirt(Utility.RandomMinMax(1, 4)));
            this.PackItem(new MandrakeRoot());
			
            Item ore = new IronOre(5);
            ore.ItemID = 0x19B7;
            this.PackItem(ore);
        }

        public EnragedEarthElemental(Serial serial)
            : base(serial)
        {
        }

        public override double DispelDifficulty
        {
            get
            {
                return 117.5;
            }
        }
        public override double DispelFocus
        {
            get
            {
                return 45.0;
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
            this.AddLoot(LootPack.Average);
            this.AddLoot(LootPack.Meager);
            this.AddLoot(LootPack.Gems);
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