using System;
using Server.Items;

namespace Server.Mobiles 
{ 
    [CorpseName("an archmage corpse")] 
    public class BrigandCannibalMage : EvilMage
    { 
        [Constructable] 
        public BrigandCannibalMage()
            : base()
        {
            this.Title = "the brigand cannibal mage";

            this.SetStr(68, 95);
            this.SetDex(81, 95);
            this.SetInt(110, 115);

            this.SetHits(2058, 2126);
            this.SetMana(552, 553);

            this.SetDamage(10, 23);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 50, 60);
            this.SetResistance(ResistanceType.Fire, 50, 60);
            this.SetResistance(ResistanceType.Cold, 50, 60);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 50, 60);

            this.SetSkill(SkillName.MagicResist, 96.9, 96.9);
            this.SetSkill(SkillName.Tactics, 94.0, 94.0);
            this.SetSkill(SkillName.Wrestling, 54.3, 54.3);

            this.Fame = 14500;
            this.Karma = -14500;
            
            if (Utility.RandomDouble() < 0.75)
            {
                PackItem(new SeveredHumanEars());
            }
        }

        public BrigandCannibalMage(Serial serial)
            : base(serial)
        { 
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