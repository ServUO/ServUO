using System;
using Server;
using Server.Items;

public class SmallBushel : BaseContainer
{
    [Constructable]
    public SmallBushel()
        : base(0x09B1)
    {
        this.Weight = 1.0; 
    }

    public SmallBushel(Serial serial)
        : base(serial)
    {
    }

    public override int LabelNumber
    {
        get
        {
            return 1112337;
        }
    }// small bushel

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