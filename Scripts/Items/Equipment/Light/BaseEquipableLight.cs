namespace Server.Items
{
    public abstract class BaseEquipableLight : BaseLight
    {
        [Constructable]
        public BaseEquipableLight(int itemID)
            : base(itemID)
        {
            Layer = Layer.TwoHanded;
        }

        public BaseEquipableLight(Serial serial)
            : base(serial)
        {
        }

        public override void Ignite()
        {
            if (!(Parent is Mobile) && RootParent is Mobile)
            {
                Mobile holder = (Mobile)RootParent;

                if (holder.EquipItem(this))
                {
                    if (this is Candle)
                        holder.SendLocalizedMessage(502969); // You put the candle in your left hand.
                    else if (this is Torch)
                        holder.SendLocalizedMessage(502971); // You put the torch in your left hand.

                    base.Ignite();
                }
                else
                {
                    holder.SendLocalizedMessage(502449); // You cannot hold this item.
                }
            }
            else
            {
                base.Ignite();
            }
        }

        public override void OnAdded(object parent)
        {
            if (Burning && parent is Container)
                Douse();

            base.OnAdded(parent);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
