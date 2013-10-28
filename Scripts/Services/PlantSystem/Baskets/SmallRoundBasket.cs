using System;
using Server;
using Server.Items;

public class SmallRoundBasket : BaseContainer
{
    [Constructable]
    public SmallRoundBasket()
        : this(1)
    {
        this.Weight = 1.0; 
    }

    [Constructable]
    public SmallRoundBasket(int amount)
        : base(0x24DD)
    {
        this.Weight = 1.0;

        //Hue = 0;  			
        this.Stackable = true;
        this.Amount = amount; 
    }

    public SmallRoundBasket(Serial serial)
        : base(serial)
    {
    }

    public override int LabelNumber
    {
        get
        {
            return 1112298;
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