using System;

namespace Server.Items
{ 
    public class SmithBag : Bag 
    { 
        [Constructable] 
        public SmithBag()
            : this(5000)
        { 
        }

        [Constructable] 
        public SmithBag(int amount) 
        { 
            this.DropItem(new DullCopperIngot(amount)); 
            this.DropItem(new ShadowIronIngot(amount)); 
            this.DropItem(new CopperIngot(amount)); 
            this.DropItem(new BronzeIngot(amount)); 
            this.DropItem(new GoldIngot(amount)); 
            this.DropItem(new AgapiteIngot(amount)); 
            this.DropItem(new VeriteIngot(amount)); 
            this.DropItem(new ValoriteIngot(amount)); 
            this.DropItem(new IronIngot(amount)); 
            this.DropItem(new Tongs(amount));
            this.DropItem(new TinkerTools(amount));
        }

        public SmithBag(Serial serial)
            : base(serial)
        { 
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