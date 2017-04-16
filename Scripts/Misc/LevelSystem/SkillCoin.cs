using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class SkillCoin : Item
    {
        

        [CommandProperty(AccessLevel.GameMaster)]
        public int char_SKV
        {
            get { return char_SKV; }
            set { char_SKV = value; InvalidateProperties(); }
        }
        
        [Constructable]
        public SkillCoin()
            : base(0x1869)
        {
            Name = "A Skill Coin";
            Weight = 1.0;
            LootType = LootType.Blessed;

            char_SKV = Utility.RandomMinMax(1, 5);


        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("+{0}", char_SKV.ToString(), "Skill Points"); // value: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;

            if (IsChildOf(pm.Backpack))
            {
                if (pm.SkillsTotal >= 700)  //Edit this value based on your servers skill cap
		            pm.SendMessage("You have reached the skill cap, what do you need more skill points for");
                else
                    pm.charSKPoints += char_SKV;
                    pm.SendMessage("You have been awarded {0} skill points", char_SKV);
                    this.Delete();               
            }
            else
                pm.SendMessage("This must be in your pack!");

        }

        
        public SkillCoin(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        char_SKV = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)char_SKV);
        }
    }
}