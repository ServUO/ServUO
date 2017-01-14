using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a hydra corpse")]
    public class Hydra : BaseCreature
    {
        [Constructable]
        public Hydra()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a hydra";
            this.Body = 0x109;
            this.BaseSoundID = 0x16A;

            this.SetStr(801, 828);
            this.SetDex(105, 118);
            this.SetInt(102, 120);

            this.SetHits(1484, 1500);

            this.SetDamage(21, 26);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Poison, 10);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 70, 81);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 35, 43);
            this.SetResistance(ResistanceType.Energy, 36, 45);

            this.SetSkill(SkillName.Wrestling, 103.5, 117.4);
            this.SetSkill(SkillName.Tactics, 100.1, 109.8);
            this.SetSkill(SkillName.MagicResist, 85.5, 96.4);
            this.SetSkill(SkillName.Anatomy, 75.4, 79.8);

            this.Fame = 22000;
            this.Karma = -22000;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Hydra(Serial serial)
            : base(serial)
        {
        }

        public override bool HasBreath
        {
            get
            {
                return true;
            }
        }
        public override int Hides
        {
            get
            {
                return 40;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
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
            this.AddLoot(LootPack.AosUltraRich, 3);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new HydraScale());				
			
            if (Utility.RandomDouble() < 0.2)				
                c.DropItem(new ParrotItem());
				
            if (Utility.RandomDouble() < 0.05)				
                c.DropItem(new ThorvaldsMedallion());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}