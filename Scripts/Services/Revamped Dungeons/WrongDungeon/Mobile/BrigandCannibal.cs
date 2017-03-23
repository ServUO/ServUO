using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("Brigand Cannibal corpse")] 
    public class BrigandCannibal : HumanBrigand
    { 
        [Constructable] 
        public BrigandCannibal()
            : base()
        {
            this.Title = "the brigand cannibal";
            this.Hue = 33782;

            this.SetStr(68, 95);
            this.SetDex(81, 95);
            this.SetInt(110, 115);

            this.SetHits(2058, 2126);
            this.SetMana(552, 553);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 65, 68);
            this.SetResistance(ResistanceType.Fire, 65, 66);
            this.SetResistance(ResistanceType.Cold, 62, 69);
            this.SetResistance(ResistanceType.Poison, 62, 67);
            this.SetResistance(ResistanceType.Energy, 64, 68);

            this.SetSkill(SkillName.MagicResist, 96.9, 96.9);
            this.SetSkill(SkillName.Tactics, 94.0, 94.0);
            this.SetSkill(SkillName.Swords, 54.3, 54.3);

            this.Fame = 14500;
            this.Karma = -14500;

            this.VirtualArmor = 16;
        }

        public BrigandCannibal(Serial serial)
            : base(serial)
        { 
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Average);
            AddLoot(LootPack.Meager);
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