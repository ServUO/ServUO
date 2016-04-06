using Server;
using System;

namespace Server.Items
{
    public class CoffinPiece : Item
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1116783; } }

        [Constructable]
        public CoffinPiece() : base(Utility.RandomList(7481, 7480, 7479, 7452, 7451, 7450))
        {
        }

        public CoffinPiece(Serial serial)
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