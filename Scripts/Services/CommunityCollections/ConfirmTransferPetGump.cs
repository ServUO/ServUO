using Server.Mobiles;

namespace Server.Gumps
{
    public class ConfirmTransferPetGump : Gump
    {
        private readonly IComunityCollection m_Collection;
        private readonly Point3D m_Location;
        private readonly BaseCreature m_Pet;
        public ConfirmTransferPetGump(IComunityCollection collection, Point3D location, BaseCreature pet)
            : base(50, 50)
        {
            m_Collection = collection;
            m_Location = location;
            m_Pet = pet;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 270, 120, 0x13BE);

            AddHtmlLocalized(10, 10, 250, 75, 1073105, 0x0, true, false); // <div align=center>Are you sure you wish to transfer this pet away, with no possibility of recovery?</div>
            AddHtmlLocalized(55, 90, 75, 20, 1011011, 0x0, false, false); // CONTINUE
            AddHtmlLocalized(170, 90, 75, 20, 1011012, 0x0, false, false); // CANCEL

            AddButton(20, 90, 0xFA5, 0xFA7, (int)Buttons.Continue, GumpButtonType.Reply, 0);
            AddButton(135, 90, 0xFA5, 0xFA7, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Cancel,
            Continue,
        }
        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            if (m_Collection == null || m_Pet == null || m_Pet.Deleted || m_Pet.ControlMaster != state.Mobile || !state.Mobile.InRange(m_Location, 2))
                return;

            if (info.ButtonID == (int)Buttons.Continue && state.Mobile is PlayerMobile)
                m_Collection.DonatePet((PlayerMobile)state.Mobile, m_Pet);
        }
    }
}