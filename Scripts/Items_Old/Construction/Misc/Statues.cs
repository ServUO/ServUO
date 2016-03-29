using System;

namespace Server.Items 
{ 
    public class StatueSouth : Item 
    { 
        [Constructable] 
        public StatueSouth()
            : base(0x139A)
        { 
            this.Weight = 10; 
        }

        public StatueSouth(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueSouth2 : Item 
    { 
        [Constructable] 
        public StatueSouth2()
            : base(0x1227)
        { 
            this.Weight = 10; 
        }

        public StatueSouth2(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueNorth : Item 
    { 
        [Constructable] 
        public StatueNorth()
            : base(0x139B)
        { 
            this.Weight = 10; 
        }

        public StatueNorth(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueWest : Item 
    { 
        [Constructable] 
        public StatueWest()
            : base(0x1226)
        { 
            this.Weight = 10; 
        }

        public StatueWest(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueEast : Item 
    { 
        [Constructable] 
        public StatueEast()
            : base(0x139C)
        { 
            this.Weight = 10; 
        }

        public StatueEast(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueEast2 : Item 
    { 
        [Constructable] 
        public StatueEast2()
            : base(0x1224)
        { 
            this.Weight = 10; 
        }

        public StatueEast2(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatueSouthEast : Item 
    { 
        [Constructable] 
        public StatueSouthEast()
            : base(0x1225)
        { 
            this.Weight = 10; 
        }

        public StatueSouthEast(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class BustSouth : Item 
    { 
        [Constructable] 
        public BustSouth()
            : base(0x12CB)
        { 
            this.Weight = 10; 
        }

        public BustSouth(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class BustEast : Item 
    { 
        [Constructable] 
        public BustEast()
            : base(0x12CA)
        { 
            this.Weight = 10; 
        }

        public BustEast(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatuePegasus : Item 
    { 
        [Constructable] 
        public StatuePegasus()
            : base(0x139D)
        { 
            this.Weight = 10; 
        }

        public StatuePegasus(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class StatuePegasus2 : Item 
    { 
        [Constructable] 
        public StatuePegasus2()
            : base(0x1228)
        { 
            this.Weight = 10; 
        }

        public StatuePegasus2(Serial serial)
            : base(serial)
        { 
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public override void Serialize(GenericWriter writer) 
        { 
            base.Serialize(writer); 

            writer.Write((int)0); 
        }

        public override void Deserialize(GenericReader reader) 
        { 
            base.Deserialize(reader); 

            int version = reader.ReadInt(); 
        }
    }

    public class SmallTowerSculpture : Item
    {
        [Constructable]
        public SmallTowerSculpture()
            : base(0x241A)
        {
            this.Weight = 20.0;
        }

        public SmallTowerSculpture(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}