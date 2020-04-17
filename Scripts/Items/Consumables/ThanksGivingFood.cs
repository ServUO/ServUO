namespace Server.Items
{
    public abstract class BaseComplexFood : Food
    {
        private int m_Pieces;

        [CommandProperty(AccessLevel.GameMaster)]
        public int PiecesLeft
        {
            get { return m_Pieces; }
            set
            {
                int oldAmt = m_Pieces;
                m_Pieces = value;

                if (m_Pieces == 0)
                    Delete();
                else if (oldAmt != m_Pieces)
                {
                    OnChanged();
                    InvalidateProperties();
                }
            }
        }

        public abstract Food Piece { get; }
        public abstract int Pieces { get; }

        public BaseComplexFood(int id)
            : base(id)
        {
            Stackable = false;
            Weight = 1.0;

            m_Pieces = Pieces;
            FillFactor = m_Pieces;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                Item piece = Piece;

                if (piece == null)
                    return;

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, piece, false))
                    piece.MoveToWorld(from.Location, from.Map);

                PiecesLeft--;
            }
        }

        public virtual void OnChanged()
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153515, m_Pieces.ToString()); // ~1_COUNT~ pieces
        }

        public BaseComplexFood(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
            writer.Write(m_Pieces);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_Pieces = reader.ReadInt();
        }
    }

    public class SweetPotatoPie : BaseComplexFood
    {
        public override int LabelNumber => 1153514;  // sweet potato pie

        public override Food Piece => new SliceOfPie();
        public override int Pieces => 6;

        [Constructable]
        public SweetPotatoPie()
            : base(19469)
        {
            Stackable = false;
            Weight = 1.0;
        }

        public override void OnChanged()
        {
            if (PiecesLeft < Pieces && ItemID == 19469)
                ItemID = 19458;
        }

        public SweetPotatoPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(19459, 19560)]
    public class SliceOfPie : Food
    {
        public override int LabelNumber => 1153519;  // slice of pie

        [Constructable]
        public SliceOfPie()
            : base(19459)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 1;
        }

        public SliceOfPie(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MashedSweetPotatoes : Food
    {
        public override int LabelNumber => 1153516;  // mashed sweet potatoes

        [Constructable]
        public MashedSweetPotatoes()
            : base(19461)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 1;
        }

        public MashedSweetPotatoes(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(19371, 19372)]
    public class BasketOfRolls : BaseComplexFood
    {
        public override int LabelNumber => 1153518;  // basket of rolls

        public override Food Piece => new DinnerRoll();
        public override int Pieces => 13;

        [Constructable]
        public BasketOfRolls()
            : base(19371)
        {
            Stackable = false;
            Weight = 1.0;
        }

        public BasketOfRolls(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DinnerRoll : Food
    {
        public override int LabelNumber => 1153520;  // dinner roll

        [Constructable]
        public DinnerRoll()
            : base(2538)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public DinnerRoll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(18824, 18831)]
    public class TurkeyPlatter : BaseComplexFood
    {
        public override int LabelNumber => 1153517;  // turkey platter

        public override Food Piece
        {
            get
            {
                switch (Utility.Random(6))
                {
                    default:
                    case 0: return new TurkeyDinner();
                    case 1: return new RoastDuck();
                    case 2: return new RoastTurkey();
                    case 3: return new RoastChicken();
                    case 4: return new TurkeyLeg();
                    case 5: return new GibletGravey();
                }
            }
        }

        public override int Pieces => 8;

        [Constructable]
        public TurkeyPlatter()
            : base(18824)
        {
            Stackable = false;
            Weight = 1.0;
        }

        public TurkeyPlatter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TurkeyDinner : Food
    {
        public override int LabelNumber => 1153532;  // Turkey Dinner

        [Constructable]
        public TurkeyDinner()
            : base(2479)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public TurkeyDinner(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(2487, 2488)]
    public class RoastDuck : Food
    {
        public override int LabelNumber => 1153505;  // Roast Duck

        [Constructable]
        public RoastDuck()
            : base(2487)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public RoastDuck(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(2487, 2488)]
    public class RoastTurkey : Food
    {
        public override int LabelNumber => 1153507;  // Roast Turkey

        [Constructable]
        public RoastTurkey()
            : base(2487)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public RoastTurkey(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Flipable(2487, 2488)]
    public class RoastChicken : Food
    {
        public override int LabelNumber => 1153506;  // Roast Turkey

        [Constructable]
        public RoastChicken()
            : base(2487)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public RoastChicken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TurkeyLeg : Food
    {
        public override int LabelNumber => 1153508;  // turkey leg

        [Constructable]
        public TurkeyLeg()
            : base(5640)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public TurkeyLeg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GibletGravey : Food
    {
        public override int LabelNumber => 1153509;  // Giblet Gravey

        [Constructable]
        public GibletGravey()
            : base(5634)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public GibletGravey(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}