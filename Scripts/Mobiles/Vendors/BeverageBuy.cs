using System;
using Server.Items;

namespace Server.Mobiles
{
    public class BeverageBuyInfo : GenericBuyInfo
    {
        private readonly BeverageType m_Content;
        public BeverageBuyInfo(Type type, BeverageType content, int price, int amount, int itemID, int hue)
            : this(null, type, content, price, amount, itemID, hue)
        {
        }

        public BeverageBuyInfo(string name, Type type, BeverageType content, int price, int amount, int itemID, int hue)
            : base(name, type, price, amount, itemID, hue)
        {
            this.m_Content = content;

            if (type == typeof(Pitcher))
                this.Name = (1048128 + (int)content).ToString();
            else if (type == typeof(BeverageBottle))
                this.Name = (1042959 + (int)content).ToString();
            else if (type == typeof(Jug))
                this.Name = (1042965 + (int)content).ToString();
        }

        public override bool CanCacheDisplay
        {
            get
            {
                return false;
            }
        }
        public override IEntity GetEntity()
        {
            return (IEntity)Activator.CreateInstance(this.Type, new object[] { this.m_Content });
        }
    }
}