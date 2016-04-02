using System;

namespace Server.Items
{ 
    public class VesperCollectionRing : GoldRing
    {
        public VesperCollectionRing()
            : base()
        { 
        }

        public VesperCollectionRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073234;
            }
        }// A Souvenir from the Museum of Vesper
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

    public class VesperCollectionNecklace : GoldNecklace
    {
        public VesperCollectionNecklace()
            : base()
        { 
        }

        public VesperCollectionNecklace(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073234;
            }
        }// A Souvenir from the Museum of Vesper
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

    public class VesperCollectionBracelet : GoldBracelet
    {
        public VesperCollectionBracelet()
            : base()
        { 
        }

        public VesperCollectionBracelet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073234;
            }
        }// A Souvenir from the Museum of Vesper
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

    public class VesperCollectionEarrings : GoldEarrings
    {
        public VesperCollectionEarrings()
            : base()
        { 
        }

        public VesperCollectionEarrings(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073234;
            }
        }// A Souvenir from the Museum of Vesper
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