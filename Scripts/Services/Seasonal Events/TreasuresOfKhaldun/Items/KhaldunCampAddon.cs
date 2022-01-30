namespace Server.Items
{
    public class KhaldunCampAddon : BaseAddon
    {
        private static readonly int[,] m_AddOnSimpleComponents = new int[,] {
              {2879, -2, 6, 4}, {2883, 1, 10, 4}, {2879, -2, 7, 4}// 1	2	3	
			, {2880, 0, 6, 4}, {2880, -1, 6, 4}, {875, 5, 5, 4}// 4	5	6	
			, {875, -3, 5, 5}, {3014, 6, 6, 12}, {2421, 5, 8, 3}// 7	8	20	
			, {2884, 2, 10, 4}, {2879, -2, 8, 4}, {875, 5, 10, 5}// 21	22	23	
			, {2880, 0, -2, 4}, {2988, 0, -5, 20}, {2880, -2, -9, 3}// 38	41	42	
			, {2880, -3, -9, 3}, {2990, 2, 2, 23}, {2880, -2, -2, 4}// 45	46	47	
			, {2880, -3, -2, 4}, {2880, -1, -2, 4}, {2879, -5, -8, 3}// 51	53	58	
			, {2879, -5, -6, 3}, {2879, -5, -7, 3}, {2880, -5, -9, 3}// 59	60	65	
			, {880, -6, 1, 1}, {875, -6, -10, 1}, {2880, -4, -9, 3}// 68	70	71	
			, {2880, -4, -2, 4}, {2879, -5, 0, 4}, {2879, -5, -2, 4}// 72	73	77	
			, {2879, -5, -1, 4}, {875, -6, -3, 5}// 81	83	
            , {2880, 1, 6, 4}, {2880, 2, 6, 4}, {2980, 6, 11, 13} // 84, 85, 86
            , {542, 7, 5, -1}, {542, 7, 6, -1}, {542, 7, 7, -1} // 87, 88, 89
            , {542, 7, 8, -1}, {542, 7, 9, -1}, {542, 7, 10, -1},
        };

        public override BaseAddonDeed Deed => null;

        [Constructable]
        public KhaldunCampAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            AddComplexComponent(this, 880, -3, 9, 5, 2500, -1, "", 1);// 9
            AddComplexComponent(this, 880, -3, 8, 5, 1835, -1, "", 1);// 10
            AddComplexComponent(this, 881, 0, 5, 5, 2500, -1, "", 1);// 11
            AddComplexComponent(this, 873, -2, 10, 5, 2500, -1, "", 1);// 12
            AddComplexComponent(this, 876, -3, 10, 5, 1835, -1, "", 1);// 13
            AddComplexComponent(this, 881, -1, 5, 5, 1835, -1, "", 1);// 14
            AddComplexComponent(this, 880, -3, 6, 5, 1835, -1, "", 1);// 15
            AddComplexComponent(this, 873, 0, 10, 5, 2500, -1, "", 1);// 16
            AddComplexComponent(this, 873, -1, 10, 5, 1835, -1, "", 1);// 17
            AddComplexComponent(this, 880, -3, 7, 5, 2500, -1, "", 1);// 18
            AddComplexComponent(this, 881, 1, 5, 5, 1835, -1, "", 1);// 19
            AddComplexComponent(this, 4012, 5, 8, 3, 0, 2, "", 1);// 24
            AddComplexComponent(this, 881, -2, 5, 5, 2500, -1, "", 1);// 25
            AddComplexComponent(this, 881, 0, -3, 5, 1159, -1, "", 1);// 26
            AddComplexComponent(this, 877, -1, -10, 0, 1911, -1, "", 1);// 27
            AddComplexComponent(this, 874, -1, -9, 1, 1912, -1, "", 1);// 28
            AddComplexComponent(this, 874, -1, -6, 3, 1912, -1, "", 1);// 29
            AddComplexComponent(this, 881, -2, -3, 5, 1159, -1, "", 1);// 30
            AddComplexComponent(this, 881, -3, -3, 5, 1917, -1, "", 1);// 31
            AddComplexComponent(this, 873, -2, -5, 3, 1912, -1, "", 1);// 32
            AddComplexComponent(this, 877, 1, -3, 5, 1917, -1, "", 1);// 33
            AddComplexComponent(this, 872, -1, -5, 3, 1911, -1, "", 1);// 34
            AddComplexComponent(this, 881, -1, -3, 5, 1917, -1, "", 1);// 35
            AddComplexComponent(this, 873, -3, 2, 5, 1917, -1, "", 1);// 36
            AddComplexComponent(this, 873, -2, 2, 5, 1159, -1, "", 1);// 37
            AddComplexComponent(this, 873, -1, 2, 5, 1917, -1, "", 1);// 39
            AddComplexComponent(this, 873, 0, 2, 5, 1159, -1, "", 1);// 40
            AddComplexComponent(this, 873, -3, -5, 3, 1911, -1, "", 1);// 43
            AddComplexComponent(this, 872, 1, 2, 4, 1917, -1, "", 1);// 44
            AddComplexComponent(this, 874, 1, 1, 5, 1159, -1, "", 1);// 48
            AddComplexComponent(this, 874, 1, -2, 5, 1159, -1, "", 1);// 49
            AddComplexComponent(this, 878, -3, -10, 3, 1911, -1, "", 1);// 50
            AddComplexComponent(this, 878, -2, -10, 2, 1912, -1, "", 1);// 52
            AddComplexComponent(this, 876, -6, -5, 4, 1912, -1, "", 1);// 54
            AddComplexComponent(this, 876, -6, 2, 5, 1159, -1, "", 1);// 55
            AddComplexComponent(this, 873, -5, 2, 5, 1917, -1, "", 1);// 56
            AddComplexComponent(this, 873, -4, 2, 5, 1159, -1, "", 1);// 57
            AddComplexComponent(this, 873, -5, -5, 3, 1911, -1, "", 1);// 61
            AddComplexComponent(this, 873, -4, -5, 3, 1912, -1, "", 1);// 62
            AddComplexComponent(this, 880, -6, -1, 5, 1917, -1, "", 1);// 63
            AddComplexComponent(this, 880, -6, 0, 5, 1159, -1, "", 1);// 64
            AddComplexComponent(this, 881, -4, -3, 5, 1159, -1, "", 1);// 66
            AddComplexComponent(this, 881, -5, -3, 4, 1917, -1, "", 1);// 67
            AddComplexComponent(this, 880, -6, 1, 5, 1917, -1, "", 1);// 69
            AddComplexComponent(this, 880, -6, -2, 5, 1159, -1, "", 1);// 74
            AddComplexComponent(this, 880, -6, -8, 3, 1911, -1, "", 1);// 75
            AddComplexComponent(this, 880, -6, -9, 3, 1912, -1, "", 1);// 76
            AddComplexComponent(this, 880, -6, -7, 3, 1912, -1, "", 1);// 78
            AddComplexComponent(this, 878, -4, -10, 3, 1912, -1, "", 1);// 79
            AddComplexComponent(this, 878, -5, -10, 3, 1911, -1, "", 1);// 80
            AddComplexComponent(this, 880, -6, -6, 4, 1911, -1, "", 1);// 82

        }

        public KhaldunCampAddon(Serial serial) : base(serial)
        {
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource)
        {
            AddComplexComponent(addon, item, xoffset, yoffset, zoffset, hue, lightsource, null, 1);
        }

        private static void AddComplexComponent(BaseAddon addon, int item, int xoffset, int yoffset, int zoffset, int hue, int lightsource, string name, int amount)
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
}