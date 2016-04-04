using System;
using Server;
using Server.Items;

public class RoundBasketHandles : BaseContainer
{
    [Constructable]
    public RoundBasketHandles()
        : this(1)
    {
        this.Weight = 1.0; 
    }

    [Constructable]
    public RoundBasketHandles(int amount)
        : base(0x9AC)
    {
        this.Weight = 1.0;

        this.Name = "Round basket w/Handles";  			
        this.Stackable = true;
        this.Amount = amount; 
    }

    public RoundBasketHandles(Serial serial)
        : base(serial)
    {
    }

    //public override int LabelNumber { get { return 1112293; } } // Basket
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