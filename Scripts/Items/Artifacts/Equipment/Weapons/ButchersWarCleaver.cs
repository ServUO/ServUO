using System;

namespace Server.Items
{
    public class ButchersWarCleaver : WarCleaver
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ButchersWarCleaver()
            : base()
        {
            this.Slayer3 = TalismanSlayerName.Bovine;
        }

        public ButchersWarCleaver(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073526;
            }
        }// butcher's war cleaver
        public override void AppendChildNameProperties(ObjectPropertyList list)
        {
            base.AppendChildNameProperties(list);
			
            list.Add(1072512); // Bovine Slayer
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (this.Slayer3 != TalismanSlayerName.Bovine)
                this.Slayer3 = TalismanSlayerName.Bovine;
        }
    }
}