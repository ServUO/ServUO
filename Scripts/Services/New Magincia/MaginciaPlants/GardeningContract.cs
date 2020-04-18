using Server.Engines.Plants;
using Server.Targeting;
using System;

namespace Server.Items
{
    public class GardeningContract : Item
    {
        public override int LabelNumber => 1155764;  // Gardening Contract

        [Constructable]
        public GardeningContract()
            : base(0x14F0)
        {
            Weight = 1.0;
        }

        public GardeningContract(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1080058); // This must be in your backpack to use it.
            }
            else
            {
                from.SendLocalizedMessage(1155765); // Target the New Magincia plant you wish to have a gardener tend to...
                from.Target = new InternalTarget(this);
            }
        }

        private class InternalTarget : Target
        {
            private readonly Item m_Item;

            public InternalTarget(Item item)
                : base(2, true, TargetFlags.None)
            {
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is MaginciaPlantItem)
                {
                    MaginciaPlantItem plant = (MaginciaPlantItem)targeted;

                    if (!plant.IsContract)
                    {
                        if (plant.ContractTime.Month == DateTime.UtcNow.Month)
                        {
                            from.SendLocalizedMessage(1155760); // You may do this once every other month.
                            return;
                        }

                        plant.ContractTime = DateTime.UtcNow;
                        from.SendLocalizedMessage(1155762); // You have hired a gardener to tend to your plant.  The gardener will no longer tend to your plant when server maintenance occurs after the expiration date of your gardening contract.  While a gardener is tending to your plant you will not have to care for it.
                        m_Item.Delete();
                    }
                    else
                    {
                        from.SendLocalizedMessage(1155761); // A Gardener is already tending to this plant.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(1155759); // This item can only be used on New Magincia Gardening Plants.
                }
            }
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
