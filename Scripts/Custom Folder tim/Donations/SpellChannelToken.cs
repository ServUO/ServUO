using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
    public class SpellChannelingTarget : Target
    {
        private SpellChannelingToken m_Deed;

        public SpellChannelingTarget(SpellChannelingToken deed)
            : base(1, false, TargetFlags.None)
        {
            m_Deed = deed;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is BaseWeapon)
            {
                Item item = (Item)target;

                if (((BaseWeapon)item).Attributes.SpellChanneling == 1)
                {
                    from.SendMessage("That already has spell channeling!");
                }
                else
                {
                    if (item.RootParent != from) // Make sure its in their pack or they are wearing it
                    {
                        from.SendMessage("You cannot put spell channeling on that!");
                    }
                    else
                    {
                        ((BaseWeapon)item).Attributes.SpellChanneling = 1;
                        from.SendMessage("You magically add spell channeling to your weapon....");

                        m_Deed.Delete(); // Delete the deed
                    }
                }
            }
            if (target is BaseShield)
            {
                Item item = (Item)target;

                if (((BaseShield)item).Attributes.SpellChanneling == 1)
                {
                    from.SendMessage("That already has spell channeling!");
                }
                else
                {
                    if (item.RootParent != from)
                    {
                        from.SendMessage("You cannot put spell channeling on that!");
                    }
                    else
                    {
                        ((BaseShield)item).Attributes.SpellChanneling = 1;
                        from.SendMessage("You magically add spell channeling to your shield....");

                        m_Deed.Delete();
                    }
                }
            }
            else
            {
                //from.SendMessage("You cannot put spell channeling on that");
            }
        }
    }

    public class SpellChannelingToken : Item
    {
        [Constructable]
        public SpellChannelingToken() : base(0x2AAA)
        {
            Weight = 5.0;
            Name = "a Spell Channeling Token";
            LootType = LootType.Blessed;
        }

        public SpellChannelingToken(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();
        }

        public override bool DisplayLootType { get { return false; } }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) // Make sure its in their pack
            {
                //from.SendLocalizedMessage(1042001);
            }
            else
            {
                from.SendMessage("What item would you like to add spell channeling to?");
                from.Target = new SpellChannelingTarget(this);
            }
        }
    }
}