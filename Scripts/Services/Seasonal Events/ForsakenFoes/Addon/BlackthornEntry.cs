namespace Server.Items
{
    public class BlackthornBaseAddon : BaseAddon
    {
        [Constructable]
        public BlackthornBaseAddon()
        {
        }

        public BlackthornBaseAddon(Serial serial)
            : base(serial)
        {
        }

        public static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        public static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
        {
            AddonComponent ac;
            ac = new AddonComponent(item);
            if (!string.IsNullOrEmpty(name))
                ac.Name = name;
            if (hue != 0)
                ac.Hue = hue;
            if (amount > 1)
            {
                ac.Stackable = true;
                ac.Amount = amount;
            }
            if (lightsource != -1)
                ac.Light = (LightType)lightsource;
            addon.AddComponent(ac, xoffset, yoffset, zoffset);
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
        }
    }

    public class BlackthornEntry : BlackthornBaseAddon
    {
        public static BlackthornEntry InstanceTram { get; set; }
        public static BlackthornEntry InstanceFel { get; set; }

        private static readonly int[,] m_AddOnSimpleComponents = new int[,]
        {
              {6923, 7, 3, 0}, {12906, -5, 12, 0}, {12906, -4, 12, 0}// 10	18	19	
			, {12906, -4, 11, 0}, {12906, -5, 11, 0}, {12906, -6, 11, 0}// 20	21	22	
			, {12906, -7, 11, 0}, {12906, -8, 11, 0}, {12906, -7, 12, 0}// 23	24	25	
			, {12906, -6, 12, 0}, {12906, -6, 13, 0}, {12906, -5, 13, 0}// 26	27	28	
			, {12906, -4, 13, 0}, {12906, -3, 12, 0}, {12906, -3, 13, 0}// 29	30	31	
			, {12906, -3, 14, 0}, {12906, -2, 14, 0}, {12906, -2, 13, 0}// 32	33	34	
			, {12906, -2, 12, 0}, {12906, -2, 11, 0}, {12906, -3, 11, 0}// 35	36	37	
			, {4952, -1, 14, 0}, {4956, -1, 13, 0}, {4967, -1, 12, 5}// 38	39	40	
			, {4962, -1, 12, 0}, {4963, -1, 11, 5}, {4962, -1, 11, 0}// 41	42	43	
			, {4288, -9, 4, 10}, {6875, -8, -5, 0}, {6873, -8, -4, 0}// 44	46	47	
			, {3545, 6, -2, 0}, {3545, -5, -1, 0}, {3545, 1, -5, 0}// 49	50	51	
			, {3545, 2, 2, 0}, {3545, 6, 2, 0}, {12906, 3, 0, 0}// 52	53	63	
			, {12906, 4, 0, 0}, {12906, 4, -1, 0}, {12906, 3, -1, 0}// 64	65	66	
			, {12906, 2, -2, 0}, {12906, 3, -2, 0}, {12906, 3, -4, 0}// 67	68	69	
			, {12906, -1, -4, 0}, {12906, 1, -4, 0}, {12906, 3, -3, 0}// 70	71	72	
			, {12906, 2, -3, 0}, {12906, 1, -3, 0}, {12906, 1, -2, 0}// 73	74	75	
			, {12906, 0, -2, 0}, {12906, 0, -3, 0}, {12906, -1, -3, 0}// 76	77	78	
			, {12906, -2, -4, 0}, {12906, -2, -3, 0}, {12906, -3, -2, 0}// 79	80	81	
			, {12906, -2, -2, 0}, {12906, -3, -3, 0}, {12906, -3, -5, 0}// 82	83	84	
			, {12906, -3, -4, 0}, {12906, -4, -4, 0}, {12906, -4, -5, 0}// 85	86	87	
			, {4960, -1, -1, 0}, {4952, 0, -1, 0}, {4945, 2, -1, 0}// 88	89	90	
			, {4954, 3, 0, 0}, {4973, 2, 1, 0}, {4967, 3, 1, 0}// 91	92	93	
			, {4970, 4, 1, 0}, {4963, 5, 1, 0}, {4967, 5, 0, 0}// 94	95	96	
			, {4963, 5, -1, 0}, {4970, 4, -2, 0}, {4963, 4, -2, 0}// 97	98	99	
			, {4963, 4, -3, 0}, {4970, 4, -4, 0}, {4960, 3, -4, 0}// 100	101	102	
			, {4967, 2, -4, 0}, {4967, 1, -4, 0}, {4959, 0, -4, 0}// 103	104	105	
			, {4962, 0, -4, 0}, {4967, -1, -4, 0}, {4957, -1, -4, 0}// 106	107	108	
			, {4943, -1, -5, 0}, {6011, -4, -5, 2}, {6008, -5, -5, 0}// 109	110	111	
			, {4955, -2, -1, 0}, {4955, -3, -2, 0}, {4955, -4, -3, 0}// 112	113	114	
			, {4961, -5, -4, 0}, {6008, -4, -4, 0}, {12906, -4, 10, 0}// 115	116	117	
			, {12906, -5, 10, 0}, {12906, -6, 10, 0}, {12906, -7, 10, 0}// 118	119	120	
			, {12906, -8, 10, 0}, {12906, -9, 10, 0}, {12906, -9, 9, 0}// 121	122	123	
			, {12906, -8, 9, 0}, {12906, -7, 9, 0}, {12906, -6, 9, 0}// 124	125	126	
			, {12906, -5, 9, 0}, {12906, -4, 9, 0}, {12906, -5, 8, 0}// 127	128	129	
			, {12906, -6, 8, 0}, {12906, -7, 8, 0}, {12906, -8, 8, 0}// 130	131	132	
			, {12906, -9, 8, 0}, {4962, -1, 10, 0}, {4962, -2, 10, 0}// 133	134	135	
			, {4967, -3, 10, 0}, {4967, -3, 9, 0}, {4962, -3, 8, 0}// 136	137	138	
			, {4962, -4, 8, 0}, {4967, -5, 7, 0}, {4963, -7, 7, 0}// 139	140	141	
			, {4962, -7, 7, 0}, {4962, -8, 7, 0}, {4967, -9, 7, 0}// 142	143	144	
			, {4962, -9, 7, 0}, {12906, 0, -13, 0}, {12906, -1, -13, 0}// 145	146	147	
			, {12906, 0, -14, 0}, {12906, 0, -15, 0}, {12906, -1, -15, 0}// 148	149	150	
			, {12906, -1, -14, 0}, {4968, 0, -16, 2}, {4970, 1, -16, 0}// 151	152	153	
			, {4970, -1, -16, 5}, {4970, 0, -16, 2}, {4968, -1, -16, 5}// 154	155	156	
			, {4965, 0, -16, 5}, {4965, -1, -16, 5}, {4967, 0, -16, 0}// 157	158	159	
			, {4964, 0, -16, 0}, {4960, -1, -16, 0}, {12906, -4, -6, 0}// 160	161	162	
			, {12906, -3, -6, 0}, {12906, -2, -6, 0}, {12906, -1, -7, 0}// 163	164	165	
			, {12906, -1, -8, 0}, {12906, -2, -8, 0}, {12906, -2, -7, 0}// 166	167	168	
			, {12906, -3, -7, 0}, {12906, -4, -7, 0}, {12906, -4, -8, 0}// 169	170	171	
			, {12906, -3, -9, 0}, {12906, -3, -8, 0}, {12906, -2, -9, 0}// 172	173	174	
			, {12906, -1, -9, 0}, {12906, -1, -10, 0}, {12906, 0, -9, 0}// 175	176	177	
			, {12906, 0, -10, 0}, {12906, 1, -9, 0}, {12906, 2, -9, 0}// 178	179	180	
			, {12906, 3, -8, 0}, {12906, 3, -9, 0}, {12906, 1, -10, 0}// 181	182	183	
			, {12906, 3, -10, 0}, {12906, 2, -10, 0}, {12906, 2, -11, 0}// 184	185	186	
			, {12906, 1, -11, 0}, {12906, 0, -11, 0}, {12906, -1, -11, 0}// 187	188	189	
			, {12906, 1, -12, 0}, {12906, 0, -12, 0}, {12906, -1, -12, 0}// 190	191	192	
			, {4959, -1, -6, 0}, {4967, 0, -7, 0}, {4967, 0, -8, 0}// 193	194	195	
			, {4967, 1, -8, 0}, {4967, 2, -8, 0}, {4960, 4, -8, 0}// 196	197	198	
			, {4956, 4, -9, 0}, {4956, 4, -10, 0}, {4955, -4, -6, 0}// 199	200	201	
			, {4961, -5, -7, 0}, {4961, -4, -7, 4}, {4967, -5, -8, 0}// 202	203	204	
			, {4963, -4, -8, 0}, {4961, -4, -9, 0}, {4967, -3, -9, 0}// 205	206	207	
			, {4286, -11, 6, 10}, {4287, -10, 5, 10}, {4967, -11, 5, 0}// 208	209	210	
			, {12906, -10, 9, 0}, {12906, -10, 8, 0}, {12906, -10, 7, 0}// 211	212	213	
			, {12906, -11, 7, 0}, {12906, -11, 8, 0}, {4962, -10, 6, 0}// 214	215	216	
			, {4954, -11, 7, 0}// 217	
		};

        [Constructable]
        public BlackthornEntry()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 954, 7, -2, 0, 1175, -1, "", 1);// 1
            AddComplexComponent(this, 964, 7, -1, 0, 1175, -1, "", 1);// 2
            AddComplexComponent(this, 956, 7, -1, 0, 1175, -1, "", 1);// 3
            AddComplexComponent(this, 954, 8, -1, 0, 1175, -1, "", 1);// 4
            AddComplexComponent(this, 963, 9, -1, 0, 1175, -1, "", 1);// 5
            AddComplexComponent(this, 960, 10, -1, 0, 1175, -1, "", 1);// 6
            AddComplexComponent(this, 965, 12, -1, 0, 1175, -1, "", 1);// 7
            AddComplexComponent(this, 963, 11, -1, 0, 1175, -1, "", 1);// 8
            AddComplexComponent(this, 3387, 9, -3, 0, 1109, -1, "", 1);// 9
            AddComplexComponent(this, 7392, 9, 6, 0, 2500, -1, "", 1);// 11
            AddComplexComponent(this, 3388, 10, 2, 0, 1109, -1, "", 1);// 12
            AddComplexComponent(this, 3388, 3, 12, 0, 1109, -1, "", 1);// 13
            AddComplexComponent(this, 3392, 0, 16, 10, 1920, -1, "", 1);// 14
            AddComplexComponent(this, 3392, 0, 16, 5, 1920, -1, "", 1);// 15
            AddComplexComponent(this, 3392, 0, 16, 0, 1920, -1, "", 1);// 16
            AddComplexComponent(this, 3392, 0, 16, 0, 1920, -1, "", 1);// 17
            AddComplexComponent(this, 3387, -9, -1, 0, 1109, -1, "", 1);// 45
            AddComplexComponent(this, 3566, -7, -3, 0, 2500, -1, "", 1);// 48
            AddComplexComponent(this, 3389, 4, 2, 0, 1109, -1, "", 1);// 54
            AddComplexComponent(this, 3566, 6, -4, 3, 2500, -1, "", 1);// 55
            AddComplexComponent(this, 953, 6, -4, 0, 1175, -1, "", 1);// 56
            AddComplexComponent(this, 953, 6, -3, 0, 1175, -1, "", 1);// 57
            AddComplexComponent(this, 951, 6, -2, 0, 1175, -1, "", 1);// 58
            AddComplexComponent(this, 7396, -8, 3, 0, 2500, -1, "", 1);// 59
            AddComplexComponent(this, 3388, -8, 6, 0, 2500, -1, "", 1);// 60
            AddComplexComponent(this, 7390, 1, 8, 0, 2500, -1, "", 1);// 61
            AddComplexComponent(this, 6053, 5, -2, 0, 2548, -1, "", 1);// 62
        }

        public BlackthornEntry(Serial serial) : base(serial)
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
