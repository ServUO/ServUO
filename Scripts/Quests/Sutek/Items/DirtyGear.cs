using System;
using Server.Items.MusicBox;

namespace Server.Items
{ 
    public class DirtyGear : Item 
    { 
        [Constructable]
        public DirtyGear()
            : this(1)
        {
            this.ItemID = 0x1053;
            this.Movable = true;
            this.Hue = 962;
            this.Name = "Sutek's Dirty Gear";	
        }

        [Constructable]
        public DirtyGear(int amount) 
        {
        }

        public DirtyGear(Serial serial)
            : base(serial)
        { 
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.RandomDouble() < 0.05)
            {
                from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.Rare));
            }
            else
            {
                if (Utility.RandomBool())
                    from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.Common));
                else
                    from.AddToBackpack(MusicBoxGears.RandomMusixBoxGears(TrackRarity.UnCommon));
            }
            this.Delete();
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