namespace Server.Items
{
    public class BlackthornStep5 : BlackthornBaseAddon
    {
        public static BlackthornStep5 InstanceTram { get; set; }
        public static BlackthornStep5 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {4306, 7, 7, 0}, {4317, 5, 7, 0}, {4314, 5, 7, 0}// 2	3	4	
			, {4308, 3, 7, 0}, {4280, -1, 7, 0}, {4286, -7, -3, 0}// 5	6	7	
			, {4287, -6, -4, 0}, {4288, -5, -5, 0}, {9750, -7, -6, 3}// 8	9	10	
			, {4335, 7, 3, 0}, {4335, 4, -3, 0}, {4335, 3, -4, 0}// 12	13	14	
			, {4335, 2, -4, 0}, {4335, -1, -4, 0}, {4335, -5, -3, 0}// 15	16	17	
			, {4335, -7, -2, 0}, {4335, -6, 3, 0}, {4335, -3, 3, 0}// 18	19	20	
			, {4335, 2, 3, 0}, {7391, 7, -2, 0}, {7390, 6, -2, 0}// 21	23	24	
			, {3118, 6, -4, 0}, {3118, 8, -3, 0}, {4338, -7, 1, 0}// 25	26	27	
			, {4339, -3, -4, 0}, {3814, -4, 1, 0}, {3814, -3, 1, 0}// 28	29	30	
			, {3811, 8, -5, 0}, {3811, 0, 1, 0}, {3811, 2, 0, 0}// 41	42	43	
			, {3811, 0, 0, 6}// 44	
		};

        [Constructable]
        public BlackthornStep5()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 3309, 9, -6, 0, 63, -1, "", 1);// 1
            AddComplexComponent(this, 8445, -6, -4, 17, 2500, -1, "", 1);// 11
            AddComplexComponent(this, 8424, 4, -1, 0, 2548, -1, "", 1);// 22
            AddComplexComponent(this, 3308, 3, -6, 5, 63, -1, "", 1);// 31
            AddComplexComponent(this, 3308, -4, -6, 5, 63, -1, "", 1);// 32
            AddComplexComponent(this, 3310, -5, -6, 5, 63, -1, "", 1);// 33
            AddComplexComponent(this, 3310, 2, -6, 10, 63, -1, "", 1);// 34
            AddComplexComponent(this, 3309, -7, -6, 0, 63, -1, "", 1);// 35
            AddComplexComponent(this, 3309, -6, -6, 0, 63, -1, "", 1);// 36
            AddComplexComponent(this, 3309, -2, -6, 0, 63, -1, "", 1);// 37
            AddComplexComponent(this, 3309, 0, -6, 0, 63, -1, "", 1);// 38
            AddComplexComponent(this, 3309, 5, -6, 0, 63, -1, "", 1);// 39
            AddComplexComponent(this, 3309, 6, -6, 0, 63, -1, "", 1);// 40
            AddComplexComponent(this, 8424, -8, -4, 0, 2548, -1, "", 1);// 45
            AddComplexComponent(this, 3314, -9, -5, 0, 63, -1, "", 1);// 46
            AddComplexComponent(this, 3314, -9, -2, 5, 63, -1, "", 1);// 47
            AddComplexComponent(this, 3314, -9, 2, 5, 63, -1, "", 1);// 48
            AddComplexComponent(this, 3313, -9, 0, 5, 63, -1, "", 1);// 49
            AddComplexComponent(this, 3313, -9, 5, 0, 63, -1, "", 1);// 50
            AddComplexComponent(this, 3313, -9, 4, 0, 63, -1, "", 1);// 51
        }

        public BlackthornStep5(Serial serial)
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
