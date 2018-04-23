     //Scripted By Thor//
//For Neshobas Gorean World//

using System;

namespace Server.Items
{
    public class picnicAddon : BaseAddon
    {
	[Constructable]
	public picnicAddon()

         {


            this.AddComponent(new AddonComponent(0x53), 1, 0, 0);
            this.AddComponent(new AddonComponent(0xB6E), 1, 0, 0);//Table South end// 
            this.AddComponent(new AddonComponent(0x991), 1, 0, 4);//Tray South End// 
            this.AddComponent(new AddonComponent(0x53), 2, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 3, 0, 0);//Sand Enter//
            this.AddComponent(new AddonComponent(0x246B), 4, 0, 0);//Sand Enter//
            this.AddComponent(new AddonComponent(0x246B), 5, 0, 0);//Sand Enter//  
            this.AddComponent(new AddonComponent(0x53), 6, 0, 0);
            this.AddComponent(new AddonComponent(0xB65), 7, 3, 0);//Picnic Bench East End//
            this.AddComponent(new AddonComponent(0xB67), 6, 3, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 5, 3, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 4, 3, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 3, 3, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 2, 3, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB66), 1, 3, 0);//Picnic Bench West End//
            this.AddComponent(new AddonComponent(0xB8A), 7, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 6, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 5, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 4, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 3, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 2, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 1, 4, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 7, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 6, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 5, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 4, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 3, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 2, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 1, 5, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8B), 7, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 6, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 5, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 4, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 3, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8D), 2, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB8C), 1, 6, 0);//Picnic Table//
            this.AddComponent(new AddonComponent(0xB65), 7, 7, 0);//Picnic Bench East End//
            this.AddComponent(new AddonComponent(0xB67), 6, 7, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 5, 7, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 4, 7, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 3, 7, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB67), 2, 7, 0);//Picnic Bench Ceanter//
            this.AddComponent(new AddonComponent(0xB66), 1, 7, 0);//Picnic Bench West End//
            this.AddComponent(new AddonComponent(0x52), 7, 0, 0);
            this.AddComponent(new AddonComponent(0xB6E), 7, 0, 0);//Table South end//
            this.AddComponent(new AddonComponent(0x9D7), 7, 0, 7);//Plate//
            this.AddComponent(new AddonComponent(0x1609), 7, 0, 8);//Lamb Legs//
            this.AddComponent(new AddonComponent(0x54), 7, -1, 0);
            this.AddComponent(new AddonComponent(0xB73), 7, -1, 0);//Table Middle//
            this.AddComponent(new AddonComponent(0x99D), 7, -1, 7);//Wine//
            this.AddComponent(new AddonComponent(0x54), 7, -2, 0);
            this.AddComponent(new AddonComponent(0xB73), 7, -2, 0);//Table Middle//
            this.AddComponent(new AddonComponent(0x99D), 7, -2, 7);//Wine//
            this.AddComponent(new AddonComponent(0x54), 7, -3, 0);
            this.AddComponent(new AddonComponent(0xB6F), 7, -3, 0);//Table North End//
            this.AddComponent(new AddonComponent(0x98C), 7, -3, 7);//French Bread//
            this.AddComponent(new AddonComponent(0x53), 7, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 8);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 7);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 6);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 5);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 4);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 3);
            this.AddComponent(new AddonComponent(0x53), 1, -4, 2);
            this.AddComponent(new AddonComponent(0x53), 2, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 3, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 4, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 5, -4, 0);
            this.AddComponent(new AddonComponent(0x53), 6, -4, 0);
            this.AddComponent(new AddonComponent(0x54), 0, -0, 0);  
            this.AddComponent(new AddonComponent(0x54), 0, -1, 0);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 0);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 1);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 2);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 4);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 5);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 6);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 7);
            this.AddComponent(new AddonComponent(0x54), 0, -2, 8);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 0);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 8);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 7);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 6);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 5);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 4);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 3);
            this.AddComponent(new AddonComponent(0x54), 0, -3, 2);
            this.AddComponent(new AddonComponent(0x40), 0, -4, 0);
            this.AddComponent(new AddonComponent(0x246B), 7, -3, 0);
            this.AddComponent(new AddonComponent(0x246B), 6, -3, 0);
            this.AddComponent(new AddonComponent(0x246B), 5, -3, 0);
            this.AddComponent(new AddonComponent(0x246B), 4, -3, 0);
            this.AddComponent(new AddonComponent(0x343B), 4, -3, 1);
            this.AddComponent(new AddonComponent(0x52), 4, -3, 1);
            this.AddComponent(new AddonComponent(0x246B), 3, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 3, -3, 0);
            this.AddComponent(new AddonComponent(0x9D99), 3, -3, 1);
            this.AddComponent(new AddonComponent(0x246B), 2, -3, 0);
            this.AddComponent(new AddonComponent(0x9D9D), 2, -3, 1);
            this.AddComponent(new AddonComponent(0x3709), 2, -3, 16);
            this.AddComponent(new AddonComponent(0x246B), 1, -3, 0);
            this.AddComponent(new AddonComponent(0x246B), 7, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 6, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 5, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 4, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 4, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 2, -2, 0);
            this.AddComponent(new AddonComponent(0x246B), 1, -2, 0);
            this.AddComponent(new AddonComponent(0x9994), 1, -2, 1);
            this.AddComponent(new AddonComponent(0x246B), 7, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 6, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 5, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 4, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 3, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 2, -1, 0);
            this.AddComponent(new AddonComponent(0x246B), 1, -1, 0);
            this.AddComponent(new AddonComponent(0xB6F),  1, -1, 0);//Table North End//
            this.AddComponent(new AddonComponent(0xFFF),  1, -1, 4);//Mug//
            this.AddComponent(new AddonComponent(0x246B), 7, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 6, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 5, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 4, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 3, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 2, 0, 0);
            this.AddComponent(new AddonComponent(0x246B), 1, 0, 0);
            this.AddComponent(new AddonComponent(0x9994), 1, -3, 1);
            
          Hue = 0;

        }

        public picnicAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new picnicAddonDeed();
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

    public class picnicAddonDeed : BaseAddonDeed
    {
        [Constructable]
        public picnicAddonDeed()
        {
           Name = "Picnic Area";
        }

        public picnicAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new picnicAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044345;
            }
        }// picnicAddon (east)
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