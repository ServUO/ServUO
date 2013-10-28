using System;
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
            this.m_Collection = collection;
            this.m_Location = location;
            this.m_Pet = pet;
		
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.AddPage(0);
            this.AddBackground(0, 0, 270, 120, 0x13BE);
			
            this.AddHtmlLocalized(10, 10, 250, 75, 1073105, 0x0, true, false); // <div align=center>Are you sure you wish to transfer this pet away, with no possibility of recovery?</div>
            this.AddHtmlLocalized(55, 90, 75, 20, 1011011, 0x0, false, false); // CONTINUE
            this.AddHtmlLocalized(170, 90, 75, 20, 1011012, 0x0, false, false); // CANCEL
			
            this.AddButton(20, 90, 0xFA5, 0xFA7, (int)Buttons.Continue, GumpButtonType.Reply, 0);
            this.AddButton(135, 90, 0xFA5, 0xFA7, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Cancel,
            Continue,
        }
        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        {
            if (this.m_Collection == null || this.m_Pet == null || this.m_Pet.Deleted || this.m_Pet.ControlMaster != state.Mobile || !state.Mobile.InRange(this.m_Location, 2))
                return;
				
            if (info.ButtonID == (int)Buttons.Continue && state.Mobile is PlayerMobile)
                this.m_Collection.DonatePet((PlayerMobile)state.Mobile, this.m_Pet);
        }
    }
}