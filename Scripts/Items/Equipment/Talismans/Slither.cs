using System;

namespace Server.Items
{
    public class Slither : BaseTalisman
    {
		public override bool IsArtifact { get { return true; } }
		public override int LabelNumber { get{return 1114782;} }// Slither
		
        [Constructable]
        public Slither()
            : base(0x2F5B)
        {
            Hue = 0x587;
            Blessed = RandomTalisman.GetRandomBlessed();
            Attributes.BonusHits = 10;
            Attributes.RegenHits = 2;
            Attributes.DefendChance = 10;
            Attributes.BonusMana = 10;
            Attributes.AttackChance = 10;
            Attributes.BonusStam = 8;

            switch (Utility.Random(8))
            {
                case 0: SkillBonuses.SetValues(0, SkillName.MagicResist, 20.0); break;
                case 1: SkillBonuses.SetValues(0, SkillName.Tactics, 20.0); break;
                case 2: SkillBonuses.SetValues(0, SkillName.Anatomy, 20.0); break;
                case 3: SkillBonuses.SetValues(0, SkillName.Healing, 20.0); break;
                case 4: SkillBonuses.SetValues(0, SkillName.EvalInt, 20.0); break;
                case 5: SkillBonuses.SetValues(0, SkillName.Ninjitsu, 20.0); break;
                case 6: SkillBonuses.SetValues(0, SkillName.Bushido, 20.0); break;
                case 7: SkillBonuses.SetValues(0, SkillName.Chivalry, 20.0); break;
            }
        }

        public Slither(Serial serial)
            : base(serial)
        {
        }				

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }
    }
}