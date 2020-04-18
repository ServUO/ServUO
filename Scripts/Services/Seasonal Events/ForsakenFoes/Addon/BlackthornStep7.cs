namespace Server.Items
{
    public class BlackthornStep7 : BlackthornBaseAddon
    {
        public static BlackthornStep7 InstanceTram { get; set; }
        public static BlackthornStep7 InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {6009, -9, 1, 0}, {6009, -9, 2, 0}, {6012, -9, 2, 0}// 12	13	14	
			, {6012, -8, 2, 0}, {7099, -8, 1, 0}, {7087, -8, 0, 0}// 15	16	17	
			, {7087, -8, -1, 0}, {6004, -9, -1, 0}, {6011, -9, -1, 0}// 18	19	20	
			, {6009, -9, 0, 0}, {6012, -9, 2, 0}, {6011, -8, 1, 0}// 21	22	23	
			, {7403, -1, -3, 0}, {7816, -3, 9, 0}, {7390, -1, -8, 0}// 24	25	77	
			, {9779, -3, -8, 0}// 78	
		};

        [Constructable]
        public BlackthornStep7()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 7100, 9, 1, 0, 2500, -1, "", 1);// 1
            AddComplexComponent(this, 7087, 9, 0, 0, 2500, -1, "", 1);// 2
            AddComplexComponent(this, 7088, 9, -1, 0, 2500, -1, "", 1);// 3
            AddComplexComponent(this, 7054, 8, -1, 0, 1175, -1, "", 1);// 4
            AddComplexComponent(this, 7056, 7, -1, 0, 1175, -1, "", 1);// 5
            AddComplexComponent(this, 7059, 8, 1, 0, 1175, -1, "", 1);// 6
            AddComplexComponent(this, 7076, 6, 1, 0, 2500, -1, "", 1);// 7
            AddComplexComponent(this, 7049, 6, 0, 6, 2500, -1, "", 1);// 8
            AddComplexComponent(this, 12320, 6, 0, 0, 1175, -1, "", 1);// 9
            AddComplexComponent(this, 7075, 6, -1, 0, 2500, -1, "", 1);// 10
            AddComplexComponent(this, 4962, 3, 2, 0, 1109, -1, "", 1);// 11
            AddComplexComponent(this, 4545, -6, 2, 0, 1109, -1, "", 1);// 26
            AddComplexComponent(this, 4545, -5, 2, 0, 1109, -1, "", 1);// 27
            AddComplexComponent(this, 4545, -4, 2, 0, 1109, -1, "", 1);// 28
            AddComplexComponent(this, 4545, -3, 2, 0, 1109, -1, "", 1);// 29
            AddComplexComponent(this, 4545, -2, 2, 0, 1109, -1, "", 1);// 30
            AddComplexComponent(this, 4545, -1, 2, 0, 1109, -1, "", 1);// 31
            AddComplexComponent(this, 4545, 0, 2, 0, 1109, -1, "", 1);// 32
            AddComplexComponent(this, 4545, 1, 2, 0, 1109, -1, "", 1);// 33
            AddComplexComponent(this, 4545, 2, 2, 0, 1109, -1, "", 1);// 34
            AddComplexComponent(this, 22000, -4, 4, 0, 2548, -1, "", 1);// 35
            AddComplexComponent(this, 22000, -5, 4, 0, 2548, -1, "", 1);// 36
            AddComplexComponent(this, 22000, -5, 5, 0, 2548, -1, "", 1);// 37
            AddComplexComponent(this, 22000, -4, 5, 0, 2548, -1, "", 1);// 38
            AddComplexComponent(this, 22000, -4, 6, 0, 2548, -1, "", 1);// 39
            AddComplexComponent(this, 22000, -5, 6, 0, 2548, -1, "", 1);// 40
            AddComplexComponent(this, 22000, -5, 7, 0, 2548, -1, "", 1);// 41
            AddComplexComponent(this, 22000, -4, 7, 0, 2548, -1, "", 1);// 42
            AddComplexComponent(this, 22000, -4, 8, 0, 2548, -1, "", 1);// 43
            AddComplexComponent(this, 22000, -5, 8, 0, 2548, -1, "", 1);// 44
            AddComplexComponent(this, 22000, -5, 9, 0, 2548, -1, "", 1);// 45
            AddComplexComponent(this, 22000, -4, 9, 0, 2548, -1, "", 1);// 46
            AddComplexComponent(this, 22000, -3, 9, 0, 2548, -1, "", 1);// 47
            AddComplexComponent(this, 22000, -2, 9, 0, 2548, -1, "", 1);// 48
            AddComplexComponent(this, 22000, -1, 9, 0, 2548, -1, "", 1);// 49
            AddComplexComponent(this, 22000, 0, 9, 0, 2548, -1, "", 1);// 50
            AddComplexComponent(this, 22000, 1, 9, 0, 2548, -1, "", 1);// 51
            AddComplexComponent(this, 22000, 2, 8, 0, 2548, -1, "", 1);// 52
            AddComplexComponent(this, 22000, 2, 7, 0, 2548, -1, "", 1);// 53
            AddComplexComponent(this, 22000, 2, 6, 0, 2548, -1, "", 1);// 54
            AddComplexComponent(this, 22000, 2, 5, 0, 2548, -1, "", 1);// 55
            AddComplexComponent(this, 22000, 2, 4, 0, 2548, -1, "", 1);// 56
            AddComplexComponent(this, 22000, 2, 3, 0, 2548, -1, "", 1);// 57
            AddComplexComponent(this, 22000, 1, 3, 0, 2548, -1, "", 1);// 58
            AddComplexComponent(this, 22000, 0, 3, 0, 2548, -1, "", 1);// 59
            AddComplexComponent(this, 22000, -1, 3, 0, 2548, -1, "", 1);// 60
            AddComplexComponent(this, 22000, -2, 3, 0, 2548, -1, "", 1);// 61
            AddComplexComponent(this, 22000, -3, 3, 0, 2548, -1, "", 1);// 62
            AddComplexComponent(this, 22000, -4, 3, 0, 2548, -1, "", 1);// 63
            AddComplexComponent(this, 22000, -6, 3, 0, 2548, -1, "", 1);// 64
            AddComplexComponent(this, 22000, -5, 3, 0, 2548, -1, "", 1);// 65
            AddComplexComponent(this, 4967, 2, 2, 0, 1109, -1, "", 1);// 66
            AddComplexComponent(this, 4964, 1, 2, 2, 1109, -1, "", 1);// 67
            AddComplexComponent(this, 4970, 1, 2, 0, 1109, -1, "", 1);// 68
            AddComplexComponent(this, 4963, 0, 2, 0, 1109, -1, "", 1);// 69
            AddComplexComponent(this, 4967, -1, 2, 0, 1109, -1, "", 1);// 70
            AddComplexComponent(this, 4963, -2, 2, 0, 1109, -1, "", 1);// 71
            AddComplexComponent(this, 4964, -3, 2, 2, 1109, -1, "", 1);// 72
            AddComplexComponent(this, 4970, -3, 2, 0, 1109, -1, "", 1);// 73
            AddComplexComponent(this, 4967, -4, 2, 0, 1109, -1, "", 1);// 74
            AddComplexComponent(this, 4963, -5, 2, 0, 1109, -1, "", 1);// 75
            AddComplexComponent(this, 4962, -6, 2, 0, 1109, -1, "", 1);// 76
            AddComplexComponent(this, 22000, -1, -7, 0, 2548, -1, "", 1);// 79
            AddComplexComponent(this, 22000, -1, -8, 0, 2548, -1, "", 1);// 80
            AddComplexComponent(this, 22000, -1, -9, 0, 2548, -1, "", 1);// 81
            AddComplexComponent(this, 22000, -2, -9, 0, 2548, -1, "", 1);// 82
            AddComplexComponent(this, 22000, -2, -8, 0, 2548, -1, "", 1);// 83
            AddComplexComponent(this, 22000, -2, -7, 0, 2548, -1, "", 1);// 84
            AddComplexComponent(this, 22000, -3, -7, 0, 2548, -1, "", 1);// 85
            AddComplexComponent(this, 22000, -3, -8, 0, 2548, -1, "", 1);// 86
            AddComplexComponent(this, 22000, -3, -9, 0, 2548, -1, "", 1);// 87
            AddComplexComponent(this, 22000, -4, -9, 0, 2548, -1, "", 1);// 88
            AddComplexComponent(this, 22000, -4, -8, 0, 2548, -1, "", 1);// 89
            AddComplexComponent(this, 22000, -5, -8, 0, 2548, -1, "", 1);// 90
            AddComplexComponent(this, 22000, -4, -7, 0, 2548, -1, "", 1);// 91
            AddComplexComponent(this, 22000, -5, -7, 0, 2548, -1, "", 1);// 92
            AddComplexComponent(this, 3220, 0, -8, 0, 1109, -1, "", 1);// 93
            AddComplexComponent(this, 3220, 0, -7, 0, 1109, -1, "", 1);// 94
            AddComplexComponent(this, 3255, -5, -6, 0, 1109, -1, "", 1);// 95
            AddComplexComponent(this, 3220, -4, -6, 0, 1109, -1, "", 1);// 96
        }

        public BlackthornStep7(Serial serial)
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
