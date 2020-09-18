using System;

namespace Server.Items
{
    public class SilverPlatedLamppost : BaseLight
    {
        public override int LabelNumber => 1159507;  // Silver Plated Lamppost

        [CommandProperty(AccessLevel.GameMaster)]
        public int LitID { get; set; }


        [Constructable]
        public SilverPlatedLamppost()
            : base(0xA5BA)
        {
            LitID = _LitItemID[Utility.Random(_LitItemID.Length)];
            Duration = TimeSpan.Zero;
            Burning = false;
            Weight = 1.0;
        }

        public SilverPlatedLamppost(Serial serial)
            : base(serial)
        {
        }

        private static readonly int[] _LitItemID =
        {
            0xA5BB, 0xA5C0, 0xA5C5, 0xA5CA
        };

        public override int LitItemID => LitID;
        public override int UnlitItemID => 0xA5BA;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(LitID);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            LitID = reader.ReadInt();
        }
    }
}
