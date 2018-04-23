//Created by DelBoy aka Fury on 18/02/14

using System;
using Server;

namespace Server.Items
{
    public class TigerFur : Item
    {
        [Constructable]
        public TigerFur()
            : this(1)
        {
        }
        [Constructable]
        public TigerFur(int amount)
            : base(0x11F4)
        {
            Name = "Tiger Fur";
            Stackable = false;
            Hue = 1359;
            Weight = 0.1;
            Amount = amount;
        }

        public TigerFur(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {


            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001);
            }
            else
            {
                if (from.Skills[SkillName.Tailoring].Base < 120.00)
                {
                    from.SendMessage("Only a Legendary Tailor can use such an item.");
                }
                else
                {
                    from.SendMessage("You create a piece of clothing from the Tiger fur.");

                    switch (Utility.Random(3))
                    {
                        case 0:
                            {
                                from.AddToBackpack(new TigerFurBoots());
                                break;
                            }

                        case 1:
                            {
                                from.AddToBackpack(new TigerFurCape());
                                break;
                            }

                        //If you dont want the gargish sash comment out this and decrease the random by one    
                        case 2:
                            {
                                from.AddToBackpack(new GargishTigerFurSash());
                                break;
                            }
                    }
                    this.Delete();
                }

            }

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}