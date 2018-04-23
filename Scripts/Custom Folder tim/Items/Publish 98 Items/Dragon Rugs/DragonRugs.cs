using System;

namespace Server.Items
{
    public class DragonRugEastAddon : BaseAddon
    {
        [Constructable]
        public DragonRugEastAddon()
        {
			this.AddComponent(new AddonComponent(40934), -2, -2, 0);
			this.AddComponent(new AddonComponent(40933), -2, -1, 0);
			this.AddComponent(new AddonComponent(40932), -2, 0, 0);
			this.AddComponent(new AddonComponent(40924), -1, -2, 0);
			this.AddComponent(new AddonComponent(40923), -1, -1, 0);
			this.AddComponent(new AddonComponent(40922), -1, 0, 0);
			
			this.AddComponent(new AddonComponent(40921), -1, 1, 0);
			this.AddComponent(new AddonComponent(40931), -2, 1, 0);
			this.AddComponent(new AddonComponent(40930), -2, 2, 0);
			this.AddComponent(new AddonComponent(40920), -1, 2, 0);
			this.AddComponent(new AddonComponent(40925), 2, 2, 0);
			this.AddComponent(new AddonComponent(40919), 1, 2, 0);
			this.AddComponent(new AddonComponent(40918), 0, 2, 0);
			
			this.AddComponent(new AddonComponent(40913), 0, 0, 0);
			
			this.AddComponent(new AddonComponent(40911), 0, 1, 0);
			this.AddComponent(new AddonComponent(40910), 1, 1, 0);
			this.AddComponent(new AddonComponent(40926), 2, 1, 0);
			this.AddComponent(new AddonComponent(40915), 0, -1, 0);
			this.AddComponent(new AddonComponent(40916), 0, -2, 0);
			
			this.AddComponent(new AddonComponent(40912), 1, 0, 0);
			this.AddComponent(new AddonComponent(40914), 1, -1, 0);
			this.AddComponent(new AddonComponent(40917), 1, -2, 0);
			this.AddComponent(new AddonComponent(40927), 2, 0, 0);
			this.AddComponent(new AddonComponent(40928), 2, -1, 0);
			this.AddComponent(new AddonComponent(40929), 2, -2, 0);
        }

        public DragonRugEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DragonRugEastDeed();
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

    public class DragonRugEastDeed : BaseAddonDeed
    {
        [Constructable]
        public DragonRugEastDeed()
        {
        }

        public DragonRugEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DragonRugEastAddon();
            }
        }
		
        public override int LabelNumber
        {
            get
            {
                return 1049397;
            }
        }// a brown bear rug deed facing east
		
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

    public class DragonRugSouthAddon : BaseAddon
    {
        [Constructable]
        public DragonRugSouthAddon()
        {
            this.AddComponent(new AddonComponent(40904), 2, -2, 0);
			this.AddComponent(new AddonComponent(40903), 2, -1, 0);
			this.AddComponent(new AddonComponent(40909), -2, -2, 0);
			this.AddComponent(new AddonComponent(40908), -2, -1, 0);
			this.AddComponent(new AddonComponent(40907), -2, 0, 0);
			this.AddComponent(new AddonComponent(40892), 0, -2, 0);
			
			this.AddComponent(new AddonComponent(40891), 1, -2, 0);
			this.AddComponent(new AddonComponent(40889), 1, -1, 0);
			this.AddComponent(new AddonComponent(40899), -1, -2, 0);
			this.AddComponent(new AddonComponent(40898), -1, -1, 0);
			this.AddComponent(new AddonComponent(40897), -1, 0, 0);
			this.AddComponent(new AddonComponent(40890), 0, -1, 0);
			
			this.AddComponent(new AddonComponent(40888), 0, 0, 0);
			
			this.AddComponent(new AddonComponent(40886), 0, 1, 0);
			this.AddComponent(new AddonComponent(40887), 1, 0, 0);
			this.AddComponent(new AddonComponent(40885), 1, 1, 0);
			this.AddComponent(new AddonComponent(40894), 1, 2, 0);
			this.AddComponent(new AddonComponent(40896), -1, 1, 0);
			this.AddComponent(new AddonComponent(40906), -2, 1, 0);
			
			this.AddComponent(new AddonComponent(40893), 0, 2, 0);
			this.AddComponent(new AddonComponent(40902), 2, 0, 0);
			this.AddComponent(new AddonComponent(40901), 2, 1, 0);
			this.AddComponent(new AddonComponent(40900), 2, 2, 0);
			this.AddComponent(new AddonComponent(40895), -1, 2, 0);
			this.AddComponent(new AddonComponent(40905), -2, 2, 0);

        }

        public DragonRugSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DragonRugSouthDeed();
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

    public class DragonRugSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public DragonRugSouthDeed()
        {
        }

        public DragonRugSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DragonRugSouthAddon();
            }
        }
		
        public override int LabelNumber
        {
            get
            {
                return 1049398;
            }
        }// a brown bear rug deed facing south
		
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