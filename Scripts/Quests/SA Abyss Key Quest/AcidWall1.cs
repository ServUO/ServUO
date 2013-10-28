/****************************************
* NAME    : Thick Gray Stone Wall      *
* SCRIPT  : ThickGrayStoneWall.cs      *
* VERSION : v1.00                      *
* CREATOR : Mans Sjoberg (Allmight)    *
* CREATED : 10-07.2002                 *
* **************************************/

using System;

namespace Server.Items
{
    public class AcidWall1 : BaseWall
    {
        [Constructable]
        public AcidWall1()
            : base(578)
        {
            //Hue = 1828;
        }

        public AcidWall1(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("You try to examine the strange wall but the vines get in your way.");
            }
            else if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("I can't reach that.");
            }
            base.OnDoubleClick(from);
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
}