using System;

namespace Server.Mobiles
{
    public class AnimalBuyInfo : GenericBuyInfo
    {
        private readonly int m_ControlSlots;
        public AnimalBuyInfo(int controlSlots, Type type, int price, int amount, int itemID, int hue)
            : this(controlSlots, null, type, price, amount, itemID, hue)
        {
        }

        public AnimalBuyInfo(int controlSlots, string name, Type type, int price, int amount, int itemID, int hue)
            : base(name, type, price, amount, itemID, hue)
        {
            this.m_ControlSlots = controlSlots;
        }

        public override int ControlSlots
        {
            get
            {
                return this.m_ControlSlots;
            }
        }
    }
}