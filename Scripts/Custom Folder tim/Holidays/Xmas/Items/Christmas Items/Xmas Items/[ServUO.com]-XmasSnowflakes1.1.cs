///xmas 2017 Neshoba and Lagatha 
//modifyed snowflake with random hues  change hues to Yours
// add player names to namesxml for random names on snowflakes
///</namelist>
	//<namelist type="player"> Your Players Names Go here

 using System;

namespace Server.Items
{
    public class XmasSnowflake : Item
    {
        [Constructable]
        public XmasSnowflake()
            : base(0x232E)
        {
            this.Weight = 1.0;
        Hue = Utility.RandomList( 1209,2153,2039,2278,1070,2265,1081,2259,2280,2268,1154,1366, 2879,2246,1747,1951,1959,1070,1063,1162,
		1152,1247,1086,2152,1174,1287,1289,1098,2197,2794,2082,1979 ); 

        Name = NameList.RandomName( "player" );
            this.LootType = LootType.Blessed;
        }

        public XmasSnowflake(Serial serial)
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

    public class XmasSnowFlake : Item
    {
        [Constructable]
        public XmasSnowFlake()
            : base(0x232F)
        {
            this.Weight = 1.0;
         Hue = Utility.RandomList( 1209,2153,2039,2278,1070,2265,1081,2259,2280,2268,1154,1366,2879,2246,1747,1951,1959,1070,1063,1162,1152,
		 1247,1086,2152,1174,1287,1289,1098,2197,2794,2082,1979 ); 

        Name = NameList.RandomName( "player" );
            this.LootType = LootType.Blessed;
        }

        public XmasSnowFlake(Serial serial)
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