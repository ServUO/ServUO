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
            Title = "the brigand cannibal mage";

            SetStr(68, 95);
            SetDex(81, 95);
            SetInt(110, 115);

            SetHits(2058, 2126);
            SetMana(552, 553);

            SetDamage(10, 23);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 96.9, 96.9);
            SetSkill(SkillName.Tactics, 94.0, 94.0);
            SetSkill(SkillName.Wrestling, 54.3, 54.3);
            SetSkill(SkillName.Necromancy, 94.0, 94.0);
            SetSkill(SkillName.SpiritSpeak, 54.3, 54.3);

            Fame = 14500;
            Karma = -14500;
            
            if (Utility.RandomDouble() < 0.75)
            {
                PackItem(new SeveredHumanEars());
            }

            AI = AIType.AI_NecroMage;
            SetSpecialAbility(SpecialAbility.LifeLeech);
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