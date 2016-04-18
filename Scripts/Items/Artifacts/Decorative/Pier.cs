using System;

namespace Server.Items
{
    public class Pier : Item
    {
        /*
        * This does not make a lot of sense, being a "Pier"
        * and having possible itemids that have nothing
        * to do with piers. The three items here are basically
        * permutations of the same "drop", or item that
        * will be randomly selected when the item drops.
        * 
        * It was either this, or make 2
        * new classes named to reflect that they are rocks
        * in water, or put them all in one class. Either
        * is kind of senseless, so it is what it is.
        * 
        */
		public override bool IsArtifact { get { return true; } }
        private static readonly int[] m_itemids = new int[]
        {
            0x3486, 0x348b, 0x3ae
        };
        [Constructable]
        public Pier()
            : base(m_itemids[Utility.Random(3)])
        {
        }

        public Pier(int itemid)
            : base(itemid)
        {
        }

        public Pier(Serial serial)
            : base(serial)
        {
        }

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