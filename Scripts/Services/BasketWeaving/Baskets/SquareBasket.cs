using System;
using Server;
using Server.Items;

public class SquareBasket : BaseContainer
{
    [Constructable]
    public SquareBasket()
        : base(0x24D5)
    {
        this.Weight = 1.0; 
    }

    public SquareBasket(Serial serial)
        : base(serial)
    {
    }

    public override int LabelNumber
    {
        get
        {
            return 1112295;
        }
    }// square basket

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