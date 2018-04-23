// 15JAN2008 written by RavonTUS
//
//   /\\           |\\  ||
//  /__\\  |\\ ||  | \\ ||  /\\  \ //
// /    \\ | \\||  |  \\||  \//  / \\ 
// Play at An Nox, the cure for the UO addiction
// http://annox.no-ip.com  // RavonTUS@Yahoo.com

//use [add CurrentServerTime
//use [set name "Your CurrentServerTime information goes here."

#define CurrentServerTime

using System;
using Server.Items;
using Server.Gumps;
using Server.Accounting;
using Server.Mobiles;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x104C, 0X104B)]
    public class CurrentServerTime : Item
    {
        [Constructable]
        public CurrentServerTime()
            : base(0x104C)
        {
            Movable = true;
            //Visible = false;

            Name = "Medival World’s Current Time";
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile from, Point3D oldLocation)
        {
            if (from.InRange(this, 2) && from is PlayerMobile)
            {
                if (!from.HasGump(typeof(CurrentServerTimeGump)))
                    from.SendGump(new CurrentServerTimeGump(Name));
            }
            if (!from.InRange(this, 3) && from is PlayerMobile)
            {
                if (from.HasGump(typeof(CurrentServerTimeGump)))
                    from.CloseGump(typeof(CurrentServerTimeGump));
            }
        }

        //If you want to be able to double click on the 'CurrentServerTime' then add the following lines.
        //public override void OnDoubleClick(Mobile from)
        //{
        //    if (!from.HasGump(typeof(CurrentServerTimeGump)))
        //        from.SendGump(new CurrentServerTimeGump(Name));
        //}

        public CurrentServerTime(Serial serial)
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
    }
}

namespace Server.Gumps
{
    public class CurrentServerTimeGump : Gump
    {
        DateTime now = DateTime.Now;

        public CurrentServerTimeGump(string Name)
            : base(500, 350)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            AddPage(0);
            AddBackground(15, 8, 275, 142, 9300);
            AddLabel(35, 17, 462, @"Current Server Time:");
            AddLabel(35, 35, 265, @"" + now);
            AddHtml(35, 65, 230, 68, @"" + Name, (bool)false, (bool)false);

        }
    }
}
