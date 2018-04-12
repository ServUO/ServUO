using System;

namespace Server.Items
{
    public class OssianGrimoire : NecromancerSpellbook, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public OssianGrimoire()
        {
            LootType = LootType.Blessed;

            SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0);
            Attributes.RegenMana = 1;
            Attributes.CastSpeed = 1;
            Attributes.IncreasedKarmaLoss = 5;
        }

        public OssianGrimoire(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1078148;
            }
        }// Ossian Grimoire
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
                Attributes.IncreasedKarmaLoss = 5;
        }
    }
}
