using System;

namespace Server.Mobiles
{
    [CorpseName("a mantra effervescence corpse")]
    public class MantraEffervescence : BaseCreature
    {
        [Constructable]
        public MantraEffervescence()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a mantra effervescence";
            this.Body = 0x111;
            this.BaseSoundID = 0x56E;

            this.SetStr(130, 150);
            this.SetDex(120, 130);
            this.SetInt(150, 230);

            this.SetHits(150, 250);

            this.SetDamage(21, 25);

            this.SetDamageType(ResistanceType.Physical, 30);
            this.SetDamageType(ResistanceType.Energy, 70);

            this.SetResistance(ResistanceType.Physical, 60, 65);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 40, 50);
            this.SetResistance(ResistanceType.Poison, 50, 60);
            this.SetResistance(ResistanceType.Energy, 100);

            this.SetSkill(SkillName.Wrestling, 80.0, 85.0);
            this.SetSkill(SkillName.Tactics, 80.0, 85.0);
            this.SetSkill(SkillName.MagicResist, 105.0, 115.0);
            this.SetSkill(SkillName.Magery, 90.0, 110.0);
            this.SetSkill(SkillName.EvalInt, 80.0, 90.0);
            this.SetSkill(SkillName.Meditation, 90.0, 100.0);
			
            this.Fame = 6500;
            this.Karma = -6500;
        }

        public MantraEffervescence(Serial serial)
            : base(serial)
        {
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
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