using System;
using Server.Mobiles;

namespace Server.Items
{
    public class MechanicalLifeManual : Item
    {
        [Constructable]
        public MechanicalLifeManual()
            : base(0xFBE)
        {
            this.Weight = 2.0;
        }

        public MechanicalLifeManual(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112874;
            }// Mechanical Life Manual
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
            PlayerMobile pm = from as PlayerMobile;

            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (pm == null || from.Skills[SkillName.Tinkering].Base < 100.0)
            {
                pm.SendLocalizedMessage(1112255); // Only a Grandmaster Tinker can learn from this Manual.
            }
            else if (pm.MechanicalLife)
            {
                pm.SendLocalizedMessage(1080066); // You have already learned this information.
            }
            else
            {
                pm.MechanicalLife = true;
                pm.SendLocalizedMessage(1112942); // You have learned how to build mechanical companions.
                this.Delete();
            }
        }
    }
}