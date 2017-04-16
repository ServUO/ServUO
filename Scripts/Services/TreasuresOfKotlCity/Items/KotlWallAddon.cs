using System;
using Server;
using Server.Items;

namespace Server.Engines.TreasuresOfKotlCity
{
    public class KotlWallAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return null; } }

        [Constructable]
        public KotlWallAddon()
        {
            AddComplexComponent(this, 6270, -1, 3, 0, 2591, -1, "", 1);// 7
            AddComplexComponent(this, 6270, -3, 4, 0, 2591, -1, "", 1);// 8
            AddComplexComponent(this, 6270, -3, 2, 0, 2591, -1, "", 1);// 9
            AddComplexComponent(this, 4943, -3, 1, 0, 2591, -1, "", 1);// 16
            AddComplexComponent(this, 4944, -2, 1, 0, 2591, -1, "", 1);// 17
            AddComplexComponent(this, 4954, 0, 1, 0, 2591, -1, "", 1);// 18
            AddComplexComponent(this, 3117, 1, 4, 0, 2591, -1, "", 1);// 19
            AddComplexComponent(this, 18129, 0, 5, 0, 2591, -1, "", 1);// 20
            AddComplexComponent(this, 4958, -1, 2, 0, 2591, -1, "", 1);// 21
            AddComplexComponent(this, 4957, -1, 3, 0, 2591, -1, "", 1);// 22
            AddComplexComponent(this, 4956, -2, 4, 1, 2591, -1, "", 1);// 23
            AddComplexComponent(this, 17864, 0, 6, 0, 2591, -1, "", 1);// 25
            AddComplexComponent(this, 18116, 0, 2, 0, 2591, -1, "", 1);// 26
            AddComplexComponent(this, 18129, 0, 3, 0, 2591, -1, "", 1);// 27
            AddComplexComponent(this, 4954, -1, 1, 0, 2591, -1, "", 1);// 28
            AddComplexComponent(this, 4954, -2, 6, 0, 2591, -1, "", 1);// 29
            AddComplexComponent(this, 4955, -2, 5, 0, 2591, -1, "", 1);// 30
            AddComplexComponent(this, 17850, 0, 7, 0, 2591, -1, "", 1);// 31
            AddComplexComponent(this, 6270, -3, -3, 0, 2591, -1, "", 1);// 32
            AddComplexComponent(this, 4945, -2, 0, 0, 2591, -1, "", 1);// 37
            AddComplexComponent(this, 4954, -2, -4, 0, 2591, -1, "", 1);// 38
            AddComplexComponent(this, 4955, -2, -5, 0, 2591, -1, "", 1);// 39
            AddComplexComponent(this, 4955, 0, 0, 0, 2591, -1, "", 1);// 40
            AddComplexComponent(this, 3117, 1, -7, 0, 2591, -1, "", 1);// 41
            AddComplexComponent(this, 3117, -1, -1, 0, 2591, -1, "", 1);// 42
            AddComplexComponent(this, 18129, 0, -5, 0, 2591, -1, "", 1);// 43
            AddComplexComponent(this, 4949, -1, -4, 0, 2591, -1, "", 1);// 44
            AddComplexComponent(this, 4955, -1, 0, 0, 2591, -1, "", 1);// 46
            AddComplexComponent(this, 17864, 0, -6, 0, 2591, -1, "", 1);// 47
            AddComplexComponent(this, 4952, -1, -2, 0, 2591, -1, "", 1);// 48
            AddComplexComponent(this, 17850, 0, -7, 0, 2591, -1, "", 1);// 49
            AddComplexComponent(this, 4953, -1, -3, 0, 2591, -1, "", 1);// 50
            AddComplexComponent(this, 4950, -1, -5, 0, 2591, -1, "", 1);// 51
            AddComplexComponent(this, 18111, 0, -4, 0, 2591, -1, "", 1);// 52
            AddComplexComponent(this, 4951, -2, -1, 0, 2591, -1, "", 1);// 54
            AddComplexComponent(this, 18116, 0, -2, 0, 2591, -1, "", 1);// 55
            AddComplexComponent(this, 4948, -1, -7, 0, 2591, -1, "", 1);// 58
            AddComplexComponent(this, 4947, -1, -6, 0, 2591, -1, "", 1);// 59
            AddComplexComponent(this, 18129, 0, -1, 0, 2591, -1, "", 1);// 60
            AddComplexComponent(this, 4962, -2, 7, 0, 2591, -1, "", 1);// 61
        }

        public KotlWallAddon(Serial serial)
            : base(serial)
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
            if (name != null && name.Length > 0)
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