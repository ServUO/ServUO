using System;

namespace Server.Items
{
    public class EarringBoxSet : RedVelvetGiftBox
    {
        [Constructable]
        public EarringBoxSet()
            : base()
        {
            this.DropItem(new EarringsOfProtection(AosElementAttribute.Physical));
            this.DropItem(new EarringsOfProtection(AosElementAttribute.Fire));
            this.DropItem(new EarringsOfProtection(AosElementAttribute.Cold));
            this.DropItem(new EarringsOfProtection(AosElementAttribute.Poison));
            this.DropItem(new EarringsOfProtection(AosElementAttribute.Energy));
        }

        public EarringBoxSet(Serial serial)
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

    public class EarringsOfProtection : BaseJewel
    {
        private AosElementAttribute m_Attribute;
        [Constructable]
        public EarringsOfProtection()
            : this(RandomType())
        {
        }

        [Constructable]
        public EarringsOfProtection(AosElementAttribute element)
            : base(0x1087, Layer.Earrings)
        {
            this.Resistances[((AosElementAttribute)element)] = 2;

            this.m_Attribute = element;
            this.LootType = LootType.Blessed;
        }

        public EarringsOfProtection(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual AosElementAttribute Attribute
        {
            get
            {
                return this.m_Attribute;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return GetItemData(this.m_Attribute, true);
            }
        }
        public override int Hue
        {
            get
            {
                return GetItemData(this.m_Attribute, false);
            }
        }
        public static AosElementAttribute RandomType()
        {
            return GetTypes(Utility.Random(5));
        }

        public static AosElementAttribute GetTypes(int value)
        {
            switch( value )
            {
                case 0:
                    return AosElementAttribute.Physical;
                case 1:
                    return AosElementAttribute.Fire;
                case 2:
                    return AosElementAttribute.Cold;
                case 3:
                    return AosElementAttribute.Poison;
                default:
                    return AosElementAttribute.Energy;
            }
        }

        public static int GetItemData(AosElementAttribute element, bool label)
        {
            switch( element )
            {
                case AosElementAttribute.Physical:
                    return (label) ? 1071091 : 0;         // Earring of Protection (Physical)  1071091
                case AosElementAttribute.Fire:
                    return (label) ? 1071092 : 0x4ec;     // Earring of Protection (Fire)      1071092
                case AosElementAttribute.Cold:
                    return (label) ? 1071093 : 0x4f2;     // Earring of Protection (Cold)      1071093
                case AosElementAttribute.Poison:
                    return (label) ? 1071094 : 0x4f8;     // Earring of Protection (Poison)    1071094
                case AosElementAttribute.Energy:
                    return (label) ? 1071095 : 0x4fe;     // Earring of Protection (Energy)    1071095

                default:
                    return -1;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)this.m_Attribute);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.m_Attribute = (AosElementAttribute)reader.ReadInt();
        }
    }
}