using System;

namespace Server.Items
{
    public class FaeryDust : Item
    {

        [Constructable]
        public FaeryDust() : this( 1 )
        {
        }

        [Constructable]
        public FaeryDust( int amountFrom, int amountTo ) : this( Utility.RandomMinMax( amountFrom, amountTo ) )
        {
        }   

        [Constructable]
        public FaeryDust( int amount ): base(0x5745)
        {
            Stackable = true;
            Amount = amount;           
        }

        public FaeryDust(Serial serial): base(serial)
        {
        }

        public override int LabelNumber{get { return 1113358; } }// faery dust

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