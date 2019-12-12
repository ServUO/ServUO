using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class PetBondingPotion : Item
    {
        public override int LabelNumber { get { return 1152921; } } // Pet Bonding Potion

        [Constructable]
        public PetBondingPotion() : base(0x0F04)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
            Hue = 2629;
        }

        public override void OnDoubleClick(Mobile from) // Override double click of the deed to call our target 
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it. 
            }
            else
            {
                from.SendLocalizedMessage(1152922); // Target the pet you wish to bond with. Press ESC to cancel. This item is consumed on successful use, so choose wisely!
                from.Target = new BondingTarget(this);
            }
        }

        public PetBondingPotion(Serial serial) : base(serial)
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
    }

    public class BondingTarget : Target
    {
        private PetBondingPotion m_Potion;

        public BondingTarget(PetBondingPotion potion) : base(1, false, TargetFlags.None)
        {
            m_Potion = potion;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (m_Potion == null || m_Potion.Deleted || !m_Potion.IsChildOf(from.Backpack))
                    return;
                    
            if (target is BaseCreature)
            {
                BaseCreature t = (BaseCreature)target;

                if (t.IsBonded == true)
                {
                    from.SendLocalizedMessage(1152925); // That pet is already bonded to you.
                }
                else if (t.ControlMaster != from)
                {
                    from.SendLocalizedMessage(1114368); // This is not your pet!
                }
                else if (t.Allured || t.Summoned)
                {
                    from.SendLocalizedMessage(1152924); // That is not a valid pet.
                }
			    else if (target is BaseTalismanSummon)
                {
                    from.SendLocalizedMessage(1152924); // That is not a valid pet.
                }
                else
                {
                    t.IsBonded = !t.IsBonded;
                    from.SendLocalizedMessage(1049666); // Your pet has bonded with you!
                    m_Potion.Delete();
                }
            }
            else
            {
                from.SendLocalizedMessage(1152924);  // That is not a valid pet.
            }
        }
    }
}
