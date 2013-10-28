using System;
using Server;
using Server.Items;

public class SquareBasket : BaseContainer
{
    [Constructable]
    public SquareBasket()
        : this(1)
    {
        this.Weight = 1.0; 
    }

    [Constructable]
    public SquareBasket(int amount)
        : base(0x24D5)
    {
        this.Weight = 1.0;

        //Hue = 0;  			
        this.Stackable = true;
        this.Amount = amount; 
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
    }// Basket
    public bool RetainsColorFrom
    {
        get
        {
            return true;
        }
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