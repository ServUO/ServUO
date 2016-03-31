using System;

namespace Server.Items
{
    public class OssianGrimoire : NecromancerSpellbook, ITokunoDyable
    {
        [Constructable]
        public OssianGrimoire()
        {
            this.LootType = LootType.Blessed;

            this.SkillBonuses.SetValues(0, SkillName.Necromancy, 10.0);
            this.Attributes.RegenMana = 1;
            this.Attributes.CastSpeed = 1;
            this.Attributes.IncreasedKarmaLoss = 5;
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
                this.Attributes.IncreasedKarmaLoss = 5;
        }
    }
}