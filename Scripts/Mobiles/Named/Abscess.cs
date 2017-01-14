using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an abscess's corpse")]
    public class Abscess : BaseCreature
    {
        [Constructable]
        public Abscess()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Abscess";
            this.Body = 0x109;
            this.Hue = 0x8FD;
            this.BaseSoundID = 0x16A;

            this.SetStr(845, 871);
            this.SetDex(121, 134);
            this.SetInt(128, 142);

            this.SetHits(7470, 7540);

            this.SetDamage(26, 31);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Fire, 10);
            this.SetDamageType(ResistanceType.Cold, 10);
            this.SetDamageType(ResistanceType.Poison, 10);
            this.SetDamageType(ResistanceType.Energy, 10);

            this.SetResistance(ResistanceType.Physical, 65, 75);
            this.SetResistance(ResistanceType.Fire, 70, 80);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 35, 45);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Wrestling, 132.3, 143.8);
            this.SetSkill(SkillName.Tactics, 121.0, 130.5);
            this.SetSkill(SkillName.MagicResist, 102.9, 119.0);
            this.SetSkill(SkillName.Anatomy, 91.8, 94.3);

            for (int i = 0; i < Utility.RandomMinMax(1, 2); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Abscess(Serial serial)
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
        public override bool GivesMLMinorArtifact
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
            this.AddLoot(LootPack.AosUltraRich, 4);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);		
			
            c.DropItem(new AbscessTail());			
			
            if (Paragon.ChestChance > Utility.RandomDouble())
                c.DropItem(new ParagonChest(this.Name, this.TreasureMapLevel));
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