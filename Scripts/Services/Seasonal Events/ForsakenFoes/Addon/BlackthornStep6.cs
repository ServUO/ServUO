namespace Server.Items
{
    public class BlackthornStep6 : BlackthornBaseAddon
    {
        public static BlackthornStep6 InstanceTram { get; set; }
        public static BlackthornStep6 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {18324, 11, 1, 4}, {18324, 9, 1, 4}, {3793, -9, 1, 4}// 10	11	19	
			, {4306, -3, -2, 4}, {18325, 3, -4, 4}, {40248, 2, -4, 4}// 20	21	22	
			, {40248, -4, 2, 4}, {3793, -4, 3, 4}, {3792, -12, 6, 4}// 30	31	53	
			, {3792, -12, 5, 4}, {4306, -17, 5, 14}, {4967, -11, 3, 4}// 54	55	58	
			, {4954, -13, 4, 4}, {4962, -15, 1, 4}, {4954, -10, 0, 4}// 59	60	61	
					};

        [Constructable]
        public BlackthornStep6()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 17856, 17, 2, 4, 1109, -1, "", 1);// 1
            AddComplexComponent(this, 17855, 17, 3, 4, 1109, -1, "", 1);// 2
            AddComplexComponent(this, 17858, 16, 3, 4, 1109, -1, "", 1);// 3
            AddComplexComponent(this, 18001, 14, -5, 4, 1109, -1, "", 1);// 4
            AddComplexComponent(this, 18001, 13, -5, 4, 1109, -1, "", 1);// 5
            AddComplexComponent(this, 18012, 11, -5, 4, 1109, -1, "", 1);// 6
            AddComplexComponent(this, 18003, 12, -5, 4, 1109, -1, "", 1);// 7
            AddComplexComponent(this, 18012, 10, -5, 4, 1109, -1, "", 1);// 8
            AddComplexComponent(this, 18020, 10, -4, 4, 1109, -1, "", 1);// 9
            AddComplexComponent(this, 17992, 9, 1, 4, 1109, -1, "", 1);// 12
            AddComplexComponent(this, 18017, 10, 1, 4, 1109, -1, "", 1);// 13
            AddComplexComponent(this, 18006, 10, 2, 2, 1109, -1, "", 1);// 14
            AddComplexComponent(this, 18006, 10, 3, 4, 1109, -1, "", 1);// 15
            AddComplexComponent(this, 18018, 10, 3, 3, 1109, -1, "", 1);// 16
            AddComplexComponent(this, 18012, 10, -6, 4, 1109, -1, "", 1);// 17
            AddComplexComponent(this, 18012, 9, -6, 4, 1109, -1, "", 1);// 18
            AddComplexComponent(this, 40734, 5, -3, 7, 2500, -1, "", 1);// 23
            AddComplexComponent(this, 6049, 5, -3, 7, 2548, -1, "", 1);// 24
            AddComplexComponent(this, 40735, 2, -2, 5, 2500, -1, "", 1);// 25
            AddComplexComponent(this, 40737, -2, -2, 5, 2500, -1, "", 1);// 26
            AddComplexComponent(this, 40442, 6, 7, 4, 2075, -1, "", 1);// 27
            AddComplexComponent(this, 40738, 2, 6, 5, 2500, -1, "", 1);// 28
            AddComplexComponent(this, 40737, -2, 6, 5, 2500, -1, "", 1);// 29
            AddComplexComponent(this, 22000, -9, 1, 4, 2548, -1, "", 1);// 32
            AddComplexComponent(this, 22000, -9, 2, 4, 2548, -1, "", 1);// 33
            AddComplexComponent(this, 22000, -6, 3, 4, 2548, -1, "", 1);// 34
            AddComplexComponent(this, 22000, -7, 3, 4, 2548, -1, "", 1);// 35
            AddComplexComponent(this, 22000, -7, 1, 4, 2548, -1, "", 1);// 36
            AddComplexComponent(this, 22000, -8, 1, 4, 2548, -1, "", 1);// 37
            AddComplexComponent(this, 22000, -8, 2, 4, 2548, -1, "", 1);// 38
            AddComplexComponent(this, 22000, -7, 2, 4, 2548, -1, "", 1);// 39
            AddComplexComponent(this, 22000, -6, 2, 4, 2548, -1, "", 1);// 40
            AddComplexComponent(this, 22000, -6, 1, 4, 2548, -1, "", 1);// 41
            AddComplexComponent(this, 22000, -5, 3, 4, 2548, -1, "", 1);// 42
            AddComplexComponent(this, 22000, -5, 2, 4, 2548, -1, "", 1);// 43
            AddComplexComponent(this, 22000, -5, 1, 4, 2548, -1, "", 1);// 44
            AddComplexComponent(this, 22000, -4, 4, 4, 2548, -1, "", 1);// 45
            AddComplexComponent(this, 22000, -4, 3, 4, 2548, -1, "", 1);// 46
            AddComplexComponent(this, 22000, -4, 2, 4, 2548, -1, "", 1);// 47
            AddComplexComponent(this, 22000, -4, 1, 4, 2548, -1, "", 1);// 48
            AddComplexComponent(this, 22111, -3, 4, 4, 2548, -1, "", 1);// 49
            AddComplexComponent(this, 22111, -3, 3, 4, 2548, -1, "", 1);// 50
            AddComplexComponent(this, 22111, -3, 2, 4, 2548, -1, "", 1);// 51
            AddComplexComponent(this, 22112, -3, 1, 4, 2548, -1, "", 1);// 52
            AddComplexComponent(this, 22000, -10, 1, 4, 2548, -1, "", 1);// 56
            AddComplexComponent(this, 22000, -10, 2, 4, 2548, -1, "", 1);// 57
            AddComplexComponent(this, 6046, -11, -3, 4, 2548, -1, "", 1);// 62
            AddComplexComponent(this, 6046, -11, -2, 4, 2548, -1, "", 1);// 63
            AddComplexComponent(this, 6046, -11, -1, 4, 2548, -1, "", 1);// 64
            AddComplexComponent(this, 6051, -14, -3, 4, 2548, -1, "", 1);// 65
            AddComplexComponent(this, 6051, -14, -2, 4, 2548, -1, "", 1);// 66
            AddComplexComponent(this, 6051, -14, 0, 4, 2548, -1, "", 1);// 67
            AddComplexComponent(this, 6051, -14, -1, 4, 2548, -1, "", 1);// 68
            AddComplexComponent(this, 22000, -13, 0, 4, 2548, -1, "", 1);// 69
            AddComplexComponent(this, 41041, -11, 3, 0, 2548, -1, "", 1);// 70
            AddComplexComponent(this, 22000, -12, -1, 4, 2548, -1, "", 1);// 71
            AddComplexComponent(this, 22000, -13, -1, 4, 2548, -1, "", 1);// 72
            AddComplexComponent(this, 22000, -12, -3, 4, 2548, -1, "", 1);// 73
            AddComplexComponent(this, 22000, -12, -2, 4, 2548, -1, "", 1);// 74
            AddComplexComponent(this, 22000, -13, -2, 4, 2548, -1, "", 1);// 75
            AddComplexComponent(this, 22000, -13, -3, 4, 2548, -1, "", 1);// 76
        }

        public BlackthornStep6(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                InstanceTram = this;
            }

            if (Map == Map.Felucca)
            {
                InstanceFel = this;
            }
        }
    }
}
