using System;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class ChickenLizardItem : Item
    { 

        [Constructable]
        public ChickenLizardItem()
            : base(0x428A)
        {
            Name = "Pet ChickenLizard";
            this.Hue = 0;								
        }

        public ChickenLizardItem(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendMessage("Target the ChickenLizard Perch you wish to place this ChickenLizard upon."); // Target the ChickenLizard Perch you wish to place this ChickenLizard upon.
            }
            else
                from.SendLocalizedMessage(1042004); // That must be in your pack for you to use it.
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

        public class InternalTarget : Target
        {
            private readonly ChickenLizardItem m_ChickenLizard;
            public InternalTarget(ChickenLizardItem chickenlizard)
                : base(2, false, TargetFlags.None)
            {
                this.m_ChickenLizard = chickenlizard;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is AddonComponent)
                {
                    AddonComponent component = (AddonComponent)targeted;		
					
                    if (component.Addon is ChickenLizardPerchAddon)
                    {
                        ChickenLizardPerchAddon perch = (ChickenLizardPerchAddon)component.Addon;
												
                        BaseHouse house = BaseHouse.FindHouseAt(perch);
						
                        if (house != null && house.IsCoOwner(from))
                        {
                            if (perch.ChickenLizard == null || perch.ChickenLizard.Deleted)
                            { 
                                PetChickenLizard chickenlizard = new PetChickenLizard();				
                                chickenlizard.Hue = this.m_ChickenLizard.Hue;					
                                chickenlizard.MoveToWorld(perch.Location, perch.Map);
                                chickenlizard.Z += 12;
								
                                perch.ChickenLizard = chickenlizard;
                                this.m_ChickenLizard.Delete();
                            }
                            else
                                from.SendMessage("That ChickenLizard Perch already has a ChickenLizard."); //That ChickenLizard Perch already has a ChickenLizard.
                        }
                        else
                            from.SendMessage("ChickenLizards can only be placed on ChickenLizard Perches in houses where you are an owner or co-owner."); //ChickenLizards can only be placed on ChickenLizard Perches in houses where you are an owner or co-owner.	
                    }
                    else
                        from.SendMessage("You must place the ChickenLizard on a ChickenLizard Perch."); //You must place the ChickenLizard on a ChickenLizard Perch.
                }
                else
                    from.SendMessage("You must place the ChickenLizard on a ChickenLizard Perch."); //You must place the ChickenLizard on a ChickenLizard Perch.
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                base.OnTargetOutOfRange(from, targeted);

                from.SendMessage("You must be closer to the ChickenLizard Perch to place the ChickenLizard upon it."); //You must be closer to the ChickenLizard Perch to place the ChickenLizard upon it.
            }
        }
    }
}