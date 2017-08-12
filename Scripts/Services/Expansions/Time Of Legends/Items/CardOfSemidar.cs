using System;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    [Flipable(0x9C14, 0x9C15)]
    public class CardOfSemidar : Item
    {
        public enum CardType
        {
            Dupre,
            Nystul,
            Shamino,
            Juonar,
            ProfessorRafkin,
            Minax
        }

        public override int LabelNumber
        {
            get
            {
                return 1156395;
            }
        }

        private CardType _Type;

        [CommandProperty(AccessLevel.GameMaster)]
        public CardType Type { get { return _Type; } set { _Type = value; InvalidateProperties(); } }

        [Constructable]
        public CardOfSemidar()
            : this((CardType)Utility.RandomMinMax(0, 3))
        {
        }

        [Constructable]
        public CardOfSemidar(CardType type)
            : base(0x9C14)
        {
            _Type = type;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 3))
            {
                Gump g = new Gump(100, 100);
                g.AddImage(0, 0, 39904 + (int)_Type);

                from.SendGump(g);
            }
        }
        
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (_Type == CardType.ProfessorRafkin)
                list.Add(1156562); // Professor Ellie Rafkin
            else if (_Type == CardType.Minax)
                list.Add(1156981); // Minax the Enchantress
            else
                list.Add(1156396 + (int)_Type);
        }

        public CardOfSemidar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write((int)_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            _Type = (CardType)reader.ReadInt();
        }
    }
}