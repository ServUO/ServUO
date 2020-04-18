namespace Server.Items
{
    public class BlackthornStep4 : BlackthornBaseAddon
    {
        public static BlackthornStep4 InstanceTram { get; set; }
        public static BlackthornStep4 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {7036, 7, 8, 0}, {6773, 12, 2, 0}, {7822, 12, 0, 0}// 1	8	10	
			, {6773, 6, -6, 0}, {7059, 6, -7, 0}, {12906, 3, -3, 0}// 20	21	30	
			, {12906, 1, -1, 0}, {12906, 1, -2, 0}, {12906, 1, -3, 0}// 31	32	33	
			, {12906, 0, -3, 0}, {12906, 0, -2, 0}, {12906, 0, -1, 0}// 34	35	36	
			, {12906, 0, 0, 0}, {12906, -1, -2, 0}, {12906, -1, -1, 0}// 37	38	39	
			, {12906, -1, 0, 0}, {12906, -2, 0, 0}, {12906, -2, -1, 0}// 40	41	42	
			, {12906, -2, -2, 0}, {12906, -1, -3, 0}, {12906, 2, -3, 0}// 43	44	45	
			, {12906, 3, -2, 0}, {12906, 2, -2, 0}, {12906, 2, -1, 0}// 46	47	48	
			, {12906, 2, 0, 0}, {12906, 1, 0, 0}, {12906, 0, 2, 0}// 49	50	51	
			, {12906, 0, 1, 0}, {12906, -1, 1, 0}, {12906, -2, 1, 0}// 52	53	54	
			, {12906, -3, 0, 0}, {12906, -4, -1, 0}, {12906, -3, -1, 0}// 55	56	57	
			, {12906, -3, -2, 0}, {12906, -3, -3, 0}, {12906, -2, -3, 0}// 58	59	60	
			, {15723, -2, -7, 0}, {15721, -11, -7, 0}, {12906, 3, -5, 0}// 79	80	83	
			, {12906, 3, -4, 0}, {12906, -1, -4, 0}, {12906, 0, -4, 0}// 84	85	86	
			, {12906, 1, -4, 0}, {12906, 2, -4, 0}, {12906, -2, -4, 0}// 87	88	89	
			, {12906, -2, -5, 0}, {12906, -1, -5, 0}, {12906, 0, -5, 0}// 90	91	92	
			, {12906, 1, -5, 0}, {12906, 2, -5, 0}, {12906, 2, -6, 0}// 93	94	95	
			, {12906, 3, -6, 0}, {12906, 3, -7, 0}, {12906, 1, -8, 0}// 96	97	98	
			, {12906, 0, -6, 0}, {12906, 0, -7, 0}, {12906, 2, -7, 0}// 99	100	101	
			, {12906, 1, -6, 0}, {12906, 1, -7, 0}, {12906, 3, -8, 0}// 102	103	104	
			, {12906, 2, -8, 0}// 105	
		};

        [Constructable]
        public BlackthornStep4()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 3307, 12, 7, 0, 667, -1, "", 1);// 2
            AddComplexComponent(this, 3307, 11, 7, 0, 667, -1, "", 1);// 3
            AddComplexComponent(this, 3307, 10, 7, 0, 667, -1, "", 1);// 4
            AddComplexComponent(this, 3307, 9, 7, 0, 667, -1, "", 1);// 5
            AddComplexComponent(this, 3307, 8, 7, 0, 667, -1, "", 1);// 6
            AddComplexComponent(this, 3307, 7, 7, 0, 667, -1, "", 1);// 7
            AddComplexComponent(this, 6571, 9, 8, 0, 0, 1, "", 1);// 9
            AddComplexComponent(this, 7074, 12, 0, 0, 2500, -1, "", 1);// 11
            AddComplexComponent(this, 7075, 11, 0, 0, 2500, -1, "", 1);// 12
            AddComplexComponent(this, 7077, 11, -3, 0, 2500, -1, "", 1);// 13
            AddComplexComponent(this, 7090, 12, -1, 0, 2500, -1, "", 1);// 14
            AddComplexComponent(this, 7082, 12, -3, 0, 2500, -1, "", 1);// 15
            AddComplexComponent(this, 4944, 5, -2, 0, 1175, -1, "", 1);// 16
            AddComplexComponent(this, 6571, 10, -7, 0, 0, 1, "", 1);// 17
            AddComplexComponent(this, 4784, 12, -8, 0, 2548, -1, "", 1);// 18
            AddComplexComponent(this, 6942, 9, -8, 0, 2500, -1, "", 1);// 19
            AddComplexComponent(this, 4947, 5, -4, 0, 1175, -1, "", 1);// 22
            AddComplexComponent(this, 3119, -11, 1, 1, 2075, -1, "", 1);// 23
            AddComplexComponent(this, 3120, -11, 1, 0, 2075, -1, "", 1);// 24
            AddComplexComponent(this, 3120, -11, 0, 1, 2075, -1, "", 1);// 25
            AddComplexComponent(this, 3119, -11, 0, 0, 2075, -1, "", 1);// 26
            AddComplexComponent(this, 3119, -11, -1, 0, 2075, -1, "", 1);// 27
            AddComplexComponent(this, 6773, 0, -2, 0, 872, -1, "", 1);// 28
            AddComplexComponent(this, 6571, -10, 2, 0, 0, 1, "", 1);// 29
            AddComplexComponent(this, 4955, -4, 0, 0, 1175, -1, "", 1);// 61
            AddComplexComponent(this, 4957, -3, 1, 0, 1175, -1, "", 1);// 62
            AddComplexComponent(this, 4955, -4, -1, 0, 1175, -1, "", 1);// 63
            AddComplexComponent(this, 4946, -4, -2, 0, 1175, -1, "", 1);// 64
            AddComplexComponent(this, 4947, -3, -3, 0, 1175, -1, "", 1);// 65
            AddComplexComponent(this, 4947, -2, 2, 0, 1175, -1, "", 1);// 66
            AddComplexComponent(this, 4962, -1, 2, 0, 1175, -1, "", 1);// 67
            AddComplexComponent(this, 4963, -1, 3, 0, 1175, -1, "", 1);// 68
            AddComplexComponent(this, 4963, 0, 3, 5, 1175, -1, "", 1);// 69
            AddComplexComponent(this, 4962, 0, 3, 0, 1175, -1, "", 1);// 70
            AddComplexComponent(this, 4952, 1, 3, 0, 1175, -1, "", 1);// 71
            AddComplexComponent(this, 4947, 2, 2, 0, 1175, -1, "", 1);// 72
            AddComplexComponent(this, 4944, 3, 2, 0, 1175, -1, "", 1);// 73
            AddComplexComponent(this, 4943, 3, 1, 0, 1175, -1, "", 1);// 74
            AddComplexComponent(this, 4947, 4, 0, 0, 1175, -1, "", 1);// 75
            AddComplexComponent(this, 4956, 4, -1, 0, 1175, -1, "", 1);// 76
            AddComplexComponent(this, 4956, 3, -2, 0, 1175, -1, "", 1);// 77
            AddComplexComponent(this, 4954, 4, -3, 0, 1175, -1, "", 1);// 78
            AddComplexComponent(this, 8708, -10, -8, 0, 2075, -1, "", 1);// 81
            AddComplexComponent(this, 6571, -3, -8, 0, 0, 1, "", 1);// 82
            AddComplexComponent(this, 4944, -2, -4, 0, 1175, -1, "", 1);// 106
            AddComplexComponent(this, 4957, -2, -5, 0, 1175, -1, "", 1);// 107
            AddComplexComponent(this, 4958, -1, -6, 0, 1175, -1, "", 1);// 108
            AddComplexComponent(this, 4947, 0, -6, 0, 1175, -1, "", 1);// 109
            AddComplexComponent(this, 4949, 0, -7, 0, 1175, -1, "", 1);// 110
            AddComplexComponent(this, 4952, 1, -8, 0, 1175, -1, "", 1);// 111
            AddComplexComponent(this, 4954, 4, -4, 0, 1175, -1, "", 1);// 112
            AddComplexComponent(this, 4954, 4, -5, 0, 1175, -1, "", 1);// 113
            AddComplexComponent(this, 4963, 4, -7, 5, 1175, -1, "", 1);// 114
            AddComplexComponent(this, 4963, 4, -6, 0, 1175, -1, "", 1);// 115
            AddComplexComponent(this, 4962, 4, -7, 0, 1175, -1, "", 1);// 116
            AddComplexComponent(this, 4952, 4, -8, 0, 1175, -1, "", 1);// 117
            AddComplexComponent(this, 3097, -12, 2, 0, 2075, -1, "", 1);// 118
            AddComplexComponent(this, 3118, -12, 1, 0, 2075, -1, "", 1);// 119
            AddComplexComponent(this, 3119, -12, -1, 0, 2075, -1, "", 1);// 120
            AddComplexComponent(this, 3109, -12, 0, 0, 2075, -1, "", 1);// 121
            AddComplexComponent(this, 8700, -12, -8, 0, 2075, -1, "", 1);// 122
            AddComplexComponent(this, 8708, -12, -6, 0, 2075, -1, "", 1);// 123
        }

        public BlackthornStep4(Serial serial)
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
