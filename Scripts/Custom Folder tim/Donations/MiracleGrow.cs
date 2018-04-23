using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Engines.Plants;
using Server;
 
namespace Server.Items
{
    public class MiracleGrowTarget : Target
    {
        private MiracleGrow m_MiracleGrow;
 
        public MiracleGrowTarget(MiracleGrow item)
            : base(1, false,
                TargetFlags.None)
        {
            m_MiracleGrow = item;
        }
 
        protected override void OnTarget(Mobile from, object target)
        {
 
            if (m_MiracleGrow.IsChildOf(from.Backpack))  //make sure it's still in their backpack when they target the plant
            {
                if (target is PlantItem)
                {
                    Item item = (Item)target;
 
                    if (((PlantItem)item).PlantStatus == PlantStatus.DecorativePlant)
                    {
                        from.SendMessage("That plant is already fully grown!");
                    }
                    else if (((PlantItem)item).PlantStatus == PlantStatus.BowlOfDirt)
                    {
                        from.SendMessage("You must plant a seed in the bowl first.");
                    }
                    else if (((PlantItem)item).PlantStatus == PlantStatus.DeadTwigs)
                    {
                        from.SendMessage("This plant is beyond the effects of a miracle.");
                    }
                    else
                    {
                        ((PlantItem)item).PlantStatus = PlantStatus.DecorativePlant;
                        from.SendMessage("The plant grows before your eyes...");
			from.PlaySound( 0x4E );
                        m_MiracleGrow.Delete();
                    }
                }
                else
                    from.SendMessage("Invalid target!");
            }
            else
                from.SendMessage("You must still have the Miracle Grow in your backpack when you use it.");
        }
    }
 
    public class MiracleGrow : Item
    {
        [Constructable]
        public MiracleGrow()
            : base(0xE26)
        {
            Weight = 1.0;
            Name = "Miracle Grow Potion";
            Hue = 668;
        }
 
        public MiracleGrow(Serial serial)
            : base(serial)
        {
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
        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
 
            }
            else
            {
                from.SendMessage("What plant would you like to use this on?");
                from.Target = new MiracleGrowTarget(this); // Call our target
            }
        }
    }
}