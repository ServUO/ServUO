using Server.Network;

namespace Server.Misc
{
    public class Paperdoll
    {
        public static void Initialize()
        {
            EventSink.PaperdollRequest += EventSink_PaperdollRequest;
        }

        public static void EventSink_PaperdollRequest(PaperdollRequestEventArgs e)
        {
            Mobile beholder = e.Beholder;
            Mobile beheld = e.Beheld;

            beholder.Send(new DisplayPaperdoll(beheld, Titles.ComputeTitle(beholder, beheld), beheld.AllowEquipFrom(beholder)));

            foreach (Item item in beheld.Items)
                beholder.Send(item.OPLPacket);

            // NOTE: OSI sends MobileUpdate when opening your own paperdoll.
            // It has a very bad rubber-banding affect. What positive affects does it have?
        }
    }
}
