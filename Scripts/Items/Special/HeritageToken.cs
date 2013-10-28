using System;
using Server.Gumps;

namespace Server.Items
{
    public interface ITokunoDyable
    {
    }

    public class HeritageToken : Item
    {
        [Constructable]
        public HeritageToken()
            : base(0x367A)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 5.0;
        }

        public HeritageToken(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076596;
            }
        }// A Heritage Token
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(HeritageTokenGump));
                from.SendGump(new HeritageTokenGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1070998, String.Format("#{0}", 1076595));  // Use this to redeem<br>Your Heritage Items
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}