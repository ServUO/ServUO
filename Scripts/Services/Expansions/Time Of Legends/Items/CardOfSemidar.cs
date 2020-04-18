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
            Minax,
            Krampus
        }

        public override int LabelNumber => 1156395;

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
            if (from.InRange(GetWorldLocation(), 3))
            {
                Gump g = new Gump(100, 100);

                if (_Type == CardType.Krampus)
                {
                    g.AddImage(0, 0, 39914);
                }
                else
                {
                    g.AddImage(0, 0, 39904 + (int)_Type);
                }

                from.SendGump(g);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            switch (_Type)
            {
                case CardType.ProfessorRafkin: list.Add(1156562); break;
                case CardType.Minax: list.Add(1156981); break;
                case CardType.Krampus: list.Add(1158799); break;
                default: list.Add(1156396 + (int)_Type); break;
            }
        }

        public CardOfSemidar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
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
