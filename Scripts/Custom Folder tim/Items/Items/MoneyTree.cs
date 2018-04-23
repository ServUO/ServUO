using System;
using Server;

namespace Server.Items
{
    public class MoneyTree : BaseAddon
    {
        private DateTime NextUseTime = DateTime.UtcNow;

        private static int[,] m_AddOnSimpleComponents = new int[,] {
			  {3293, 0, 0, 0}, {3294, 0, 0, 0}// 1	2	
		};

        [Constructable]
        public MoneyTree()
        {
            Name = "MoneyTree";

            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public override void OnComponentUsed( AddonComponent c, Mobile from )
        {
            if (DateTime.UtcNow < NextUseTime)
            {
                TimeSpan TimeRemaining = NextUseTime - DateTime.UtcNow;
                from.SendMessage("You have {0} hours and {1} seconds left before you can use this next", TimeRemaining.Hours, TimeRemaining.Seconds);
                return;
            }

            NextUseTime = DateTime.UtcNow + TimeSpan.FromHours(Utility.Random(25));

            if (from.AccessLevel == AccessLevel.Player)
            {
                switch (Utility.Random(5))
                {
                    case 4: from.AddToBackpack(new BankCheck(5000)); break;
                    case 3: from.AddToBackpack(new BankCheck(4000)); break;
                    case 2: from.AddToBackpack(new BankCheck(3000)); break;
                    case 1: from.AddToBackpack(new BankCheck(2000)); break;
                    case 0: from.AddToBackpack(new BankCheck(1000)); break;
                }

                from.SendMessage("*an item has magically appeared in your backpack*");
            }
        }

        public MoneyTree(Serial serial): base(serial)
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
        }
    }
}