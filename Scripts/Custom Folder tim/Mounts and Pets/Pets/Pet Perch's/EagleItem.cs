using System;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;

namespace Server.Items
{
    public class EagleItem : Item
    { 
        [Constructable]
        public EagleItem()
            : base(0x211D)
        {
            Name = "Pet Eagle";
            this.Weight = 1;								

        }

        public EagleItem(Serial serial)
            : base(serial)
        {
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.IsChildOf(from.Backpack))
            {
                from.Target = new InternalTarget(this);
                from.SendMessage("Target the Eagle Perch you wish to place this Eagle upon."); // Target the Eagle Perch you wish to place this Eagle upon.
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
            private readonly EagleItem m_Eagle;
            public InternalTarget(EagleItem eagle)
                : base(2, false, TargetFlags.None)
            {
                this.m_Eagle = eagle;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is AddonComponent)
                {
                    AddonComponent component = (AddonComponent)targeted;		
					
                    if (component.Addon is EaglePerchAddon)
                    {
                        EaglePerchAddon perch = (EaglePerchAddon)component.Addon;
												
                        BaseHouse house = BaseHouse.FindHouseAt(perch);
						
                        if (house != null && house.IsCoOwner(from))
                        {
                            if (perch.Eagle == null || perch.Eagle.Deleted)
                            { 
                                PetEagle eagle = new PetEagle();				
                                eagle.Hue = this.m_Eagle.Hue;					
                                eagle.MoveToWorld(perch.Location, perch.Map);
                                eagle.Z += 12;
								
                                perch.Eagle = eagle;
                                this.m_Eagle.Delete();
                            }
                            else
                                from.SendMessage("That Eagle Perch already has an Eagle."); //That Eagle Perch already has a Eagle.
                        }
                        else
                            from.SendMessage("Eagles can only be placed on Eagle Perches in houses where you are an owner or co-owner."); //Eagles can only be placed on Eagle Perches in houses where you are an owner or co-owner.	
                    }
                    else
                        from.SendMessage("You must place the Eagle on a Eagle Perch."); //You must place the Eagle on a Eagle Perch.
                }
                else
                    from.SendMessage("You must place the Eagle on a Eagle Perch"); //You must place the Eagle on a Eagle Perch.
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                base.OnTargetOutOfRange(from, targeted);

                from.SendMessage("You must be closer to the Eagle Perch to place the Eagle upon it."); //You must be closer to the Eagle Perch to place the Eagle upon it.
            }
        }
    }
}