using Server.Items;
using Server.Network;
using System;

namespace Server.Gumps
{
    public class ConfirmPartyGump : Gump
    {
        private readonly MasterKey _Key;

        public ConfirmPartyGump(MasterKey key)
            : base(50, 50)
        {
            _Key = key;
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 245, 145, 9250);
            AddHtmlLocalized(21, 20, 203, 70, 1072525, false, false); // <CENTER>Are you sure you want to teleport <BR>your party to an unknown area?</CENTER>
            AddButton(157, 101, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(81, 100, 241, 248, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile as Mobile;

            switch (info.ButtonID)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        if (_Key == null || _Key.Altar == null)
                            return;

                        _Key.Altar.SendConfirmations(from);
                        _Key.Delete();

                        break;
                    }
            }
        }
    }

    public class ConfirmEntranceGump : Gump
    {
        private readonly Timer _Accept;
        private readonly PeerlessAltar _Altar;

        public ConfirmEntranceGump(PeerlessAltar altar, Mobile from)
            : base(50, 50)
        {
            from.CloseGump(typeof(ConfirmEntranceGump));

            _Altar = altar;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 245, 145, 9250);
            AddHtmlLocalized(21, 20, 203, 70, 1072526, false, false); // <CENTER>Your party is teleporting to an unknown area.<BR>Do you wish to go?</CENTER>
            AddButton(157, 101, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(81, 100, 241, 248, 0, GumpButtonType.Reply, 0);

            _Accept = new AcceptConfirmPeerlessPartyTimer(from);
            _Accept.Start();
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (_Altar == null)
                return;

            int button = info.ButtonID;

            switch (button)
            {
                case 0:
                    {
                        break;
                    }
                case 1:
                    {
                        _Altar.Enter(from);

                        break;
                    }
            }
        }
    }

    public class AcceptConfirmPeerlessPartyTimer : Timer
    {
        private readonly Mobile _From;

        public AcceptConfirmPeerlessPartyTimer(Mobile from)
            : base(TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(60.0), 1)
        {
            _From = from;
        }

        protected override void OnTick()
        {
            _From.CloseGump(typeof(ConfirmEntranceGump));
            Stop();
        }
    }

    public class ConfirmExitGump : BaseConfirmGump
    {
        public override int LabelNumber => 1075026;  // Are you sure you wish to teleport?

        private readonly object _Altar;

        public ConfirmExitGump(object altar) : base()
        {
            _Altar = altar;
        }

        public override void Confirm(Mobile from)
        {
            if (_Altar == null)
                return;

            if (_Altar is PeerlessAltar)
                ((PeerlessAltar)_Altar).Exit(from);
        }
    }
}
