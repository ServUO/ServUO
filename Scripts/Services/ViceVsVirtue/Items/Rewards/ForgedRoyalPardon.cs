using System;
using Server;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.VvV
{
	public class ForgedRoyalPardon : Item
	{
        public override int LabelNumber { get { return 1155524; } } // Forged Royal Pardon

        [Constructable]
        public ForgedRoyalPardon()
            : base(18098)
        {
            Hue = 0x21;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                if (m is PlayerMobile && ViceVsVirtueSystem.IsVvV(m))
                {
                    if (m.Kills <= 0)
                    {
                        m.SendMessage("You have no use for this item.");
                    }
                    else if (Server.Spells.SpellHelper.CheckCombat(m))
                    {
                        m.SendLocalizedMessage(1116588); //You cannot use a forged pardon while in combat.
                    }
                    else
                    {
                        m.SendGump(new ConfirmCallbackGump((PlayerMobile)m, 1155524, 1155525, null, null, (mobile, obj) =>
                            {
                                mobile.Kills = 0;

                                mobile.Delta(MobileDelta.Noto);

                                mobile.SendMessage("You have been pardoned from all murder counts.");
                                Delete();

                                //TODO: Effects? Message?
                            }));
                    }
                }
                else
                {
                    m.SendLocalizedMessage(1155496); // This item can only be used by VvV participants!
                }
            }
            else
            {
                m.SendLocalizedMessage(1042004); //That must be in your pack for you to use it.
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1154937); // vvv item
        }

        public ForgedRoyalPardon(Serial serial)
            : base(serial)
		{
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