using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;

namespace Server.Items
{
    public class RewardSign : Sign, IEngravable
    {
        public string EngravedText { get; set; }

        [Constructable]
        public RewardSign() : base((SignType)Utility.RandomMinMax(31, 58), Utility.RandomBool() ? SignFacing.North : SignFacing.West)
        {
            Movable = true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!String.IsNullOrEmpty(EngravedText))
            {
                list.Add(1072305, EngravedText); // Engraved: ~1_INSCRIPTION~
            }
        }

        public RewardSign(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(EngravedText);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            EngravedText = reader.ReadString();
        }
    }
}