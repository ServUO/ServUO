using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class SorcerersScroll : Item
    {
        public override int LabelNumber => 1023637;  // scroll

        [Constructable]
        public SorcerersScroll()
            : base(0x46B2)
        {
            Hue = 33;
            Movable = false;
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1154330); // A Lesson Plan
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.HasGump(typeof(SorcerersScrollGump)))
            {
                from.SendGump(new SorcerersScrollGump(from));
            }
        }

        public SorcerersScroll(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }
    }

    public class SorcerersScrollGump : Gump
    {
        public SorcerersScrollGump(Mobile owner) : base(50, 50)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;

            AddPage(0);
            AddBackground(6, 11, 390, 324, 9380);
            AddHtmlLocalized(60, 60, 350, 24, 1154328, 1062086, false, false); // Magical Reagents: Identification & Applications
            AddHtmlLocalized(147, 90, 250, 24, 1154415, 1062086, false, false); // Cousteau Perron
            AddHtmlLocalized(42, 120, 323, 174, 1154329, 1, false, true); // <I>*The scroll appears to be a lesson plan for some sort of magical study*</I><BR><BR>The identification and application of magical reagents is an important skill for any studying apprentice of the Arcane Arts.  Reagents can easily be identified occurring naturally within the world, and as such, being able to identify them as useful is an important skill.  Just as a house of cards relies on each individual to support the greater structure, so do reagents support the wider application of magics.  Each reagent has well known properties and documented uses that when combined with one another can produce hugely varying results.   A keen understanding of those properties as well as the result of any combination thereof is essential to any apprentice wishing to advance beyond their current level.  Test your knowledge of reagent identification by placing the correctly identified reagent on the plates.  When each plate has the correct reagent placed on it, the Master Instructor will check your work. 

        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;
            from.Frozen = false;
            switch (info.ButtonID)
            {
                case 0:
                    {
                        //Cancel
                        break;
                    }
            }
        }
    }
}
