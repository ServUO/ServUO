/* Created by Hammerhand*/
using System;
using System.Globalization;
using Server;

namespace Server.Items
{
    public class BlankBankCheck : BaseDecorationArtifact
    {
        public override int ArtifactRarity { get { return Utility.RandomMinMax(20, 1000); } }
        [Constructable]
        public BlankBankCheck()
            : base(0x14F0)
        {
            Name = "A Bank Check";
            Weight = 1.0;
            Hue = 0x34;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string worth;

            list.Add(1060738, "0"); // value: ~1_val~
        } 

        public BlankBankCheck(Serial serial)
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