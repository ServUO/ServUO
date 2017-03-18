using Server;
using System;
using Server.Multis;

namespace Server.Items
{
    [FlipableAttribute(0x1f14, 0x1f15, 0x1f16, 0x1f17)]
    public class ShipRune : Item
    {
        private BaseGalleon m_Galleon;

        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGalleon Galleon { get { return m_Galleon; } set { m_Galleon = value; } }

        public BaseDockedBoat DockedBoat 
        { 
            get 
            { 
                if(m_Galleon != null)
                    return m_Galleon.BoatItem;

                return null;  
            } 
        }

        public override int LabelNumber { get { return 1149570; } } //A Ship Rune

        [Constructable]
        public ShipRune() : this(null)
        {
        }

        [Constructable]
        public ShipRune(BaseGalleon galleon) : base(0x1F14)
        {
            m_Galleon = galleon;
            Hue = 0x47F;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Galleon != null)
            {
                list.Add(1149571, m_Galleon.Owner != null ? m_Galleon.Owner.Name : "Nobody"); //Owner: ~1_NAME~

                if (m_Galleon.ShipName != null)
                    list.Add(1149572, m_Galleon.ShipName); //Name: the ~1_NAME~
                else list.Add(1149573);                    //Name: the Unnamed Ship

                if (m_Galleon.Map != Map.Internal)
                    list.Add(1149574, m_Galleon.Map != null ? m_Galleon.Map.ToString() : "Unknown"); //Location: ~1_FACET~
                else
                {
                    string name = m_Galleon.ShipName != null && m_Galleon.ShipName != "" ? m_Galleon.ShipName : "Unnamed Ship";
                    list.Add(1041644, name); //The ~1_VAL~ (Dry Docked)
                }
                
            }
        }

        public ShipRune(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(m_Galleon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Galleon = reader.ReadItem() as BaseGalleon;
            if(version == 0)
                reader.ReadItem();
        }
    }
}