/*using System;

namespace Server.Items
{
    public class RewardBook : BaseLocalizedBook
    {
        public enum Edition
        {
            Normal,
            Collectors,
            First,
            Limited
        }

        private Edition _Edition;

        public Edition BookEdition { get { return _Edition; } set { _Edition = value; InvalidateHue(); InvalidateProperties(); } }

        public override int[] Contents { get { return new int[] { }; } }

        [Constructable]
        public RewardBook()
            : this(RewardBookData[Utility.Random(RewardBookData.Length)], Edition.Normal)
        {
        }

        [Constructable]
        public RewardBook(int[] data)
            : this(data, Edition.Normal)
        {
        }

        [Constructable]
        public RewardBook(int[] data, Edition edition)
        {
            BookEdition = edition;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Edition > Edition.Normal)
            {
                list.Add(1113206 = (int)_Edition);
            }
        }

        public RewardBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
            writer.Write((int)BookEdition);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
            Edition = (BookEdition)reader.ReadInt();
        }

        public static int[][] RewardBookData =
        {
            new int[] { 1113304, 1113305, 1113306, 1113307 }, // Alice in Wonderland
            new int[] { 1113155, 
        };
    }
}*/
