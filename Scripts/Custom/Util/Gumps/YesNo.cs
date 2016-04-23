using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;


namespace Server.Gumps
{
    public delegate void YesNoCallback( Mobile from, bool yesNo );

    public delegate void YesNoCallbackState( Mobile from, bool yesNo, object[] state );

    public class YesNo
    {
        public static void SimpleConfirm( YesNoCallback callback, Mobile callingPlayer, bool dragable )
        {
            SimpleConfirmMsg( callback, callingPlayer, "Are you sure?", dragable);
        }

        public static void SimpleConfirm( YesNoCallbackState callback, Mobile callingPlayer, bool dragable, object[] state )
        {
            SimpleConfirmMsg( callback, callingPlayer, "Are you sure?", dragable, state );
        }

        public static void SimpleConfirmMsg( YesNoCallback callback, Mobile callingPlayer, String msg, bool dragable )
        {
            callingPlayer.SendGump( new SimpleConfirmGump( callback, callingPlayer, msg, dragable) );
        }

        public static void SimpleConfirmMsg( YesNoCallbackState callback, Mobile callingPlayer, String msg, bool dragable, object[] state )
        {
            callingPlayer.SendGump( new SimpleConfirmStateGump( callback, callingPlayer, msg, dragable, state ) );
        }

        public static void AskYesNo( YesNoCallback callback, Mobile callingPlayer, string theQuestion, bool dragable )
        {
            AskYesNo( callback, callingPlayer, "Query:", theQuestion, "Yes", "No", dragable );
        }

        public static void AskYesNo( YesNoCallback callback, Mobile callingPlayer, string queryTitle, string theQuestion, bool dragable )
        {
            AskYesNo( callback, callingPlayer, queryTitle, theQuestion, "Yes", "No", dragable );
        }


        public static void AskYesNo( YesNoCallback callback, Mobile callingPlayer, string queryTitle, string theQuestion, string yesString, string noString, bool dragable )
        {
            callingPlayer.SendGump( new YesNoGump( callback, callingPlayer, queryTitle, theQuestion, yesString, noString, dragable ) );
        }

        private class YesNoGump: Gump
        {
            private YesNoCallback _CallBack;
            private Mobile _CallingPlayer;

            public YesNoGump( YesNoCallback callback, Mobile callingPlayer, string queryTitle, string theQuestion, string yesString, string noString, bool dragable )
                : base( 100, 100 )
            {
                _CallBack = callback;
                _CallingPlayer = callingPlayer;

                Disposable = false;
                Dragable = dragable;

                AddPage( 0 );
                AddBackground( 0, 0, 400, 360, GumpUtil.Background_LightGrey );
                AddHtml( 10, 10, 380, 20, "<basefont color=#FFFFFF size=5><center>" + queryTitle + "</center></basefont>", false, false );
                AddHtml( 10, 60, 380, 170, theQuestion, true, true );

                AddButton( 20, 245, GumpUtil.ButtonSmallBlueUp, GumpUtil.ButtonSmallBlueDown, 0xffff, GumpButtonType.Reply, 0 );
                AddLabel( 40, 240, 1150, yesString );
                AddButton( 20, 265, GumpUtil.ButtonSmallBlueUp, GumpUtil.ButtonSmallBlueDown, 0, GumpButtonType.Reply, 0 );
                AddLabel( 40, 260, 1150, noString );
            }

            //Handles button presses
            public override void OnResponse( NetState state, RelayInfo info )
            {
                bool theAnswer = ( info.ButtonID != 0 );

                _CallBack( _CallingPlayer, theAnswer );
            }
        }

        private class SimpleConfirmGump: Gump
        {
            private YesNoCallback _CallBack;
            private Mobile _CallingPlayer;

            public SimpleConfirmGump( YesNoCallback callback, Mobile callingPlayer, String msg, bool dragable )
                : base( 300, 300 )
            {
                _CallBack = callback;
                _CallingPlayer = callingPlayer;

                Disposable = false;
                Dragable = dragable;

                AddPage( 0 );

                AddImage( 0, 0, 0x816 );
                AddButton( 34, 74, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0 ); // OK
                AddButton( 88, 74, 0x995, 0x996, 0, GumpButtonType.Reply, 0 ); // Cancel

                //string msg = "Are you sure?";
                AddLabel( 42, 25, 63, msg );
            }

            //Handles button presses
            public override void OnResponse( NetState state, RelayInfo info )
            {
                bool theAnswer = ( info.ButtonID == 1 );

                _CallBack( _CallingPlayer, theAnswer );

            }
        }

        private class SimpleConfirmStateGump: Gump
        {
            private YesNoCallbackState _CallBack;
            private Mobile _CallingPlayer;
            private object[] _State;

            public SimpleConfirmStateGump( YesNoCallbackState callback, Mobile callingPlayer, String msg, bool dragable, object[] state )
                : base( 300, 300 )
            {
                _CallBack = callback;
                _CallingPlayer = callingPlayer;
                _State = state;

                Disposable = false;
                Dragable = dragable;

                AddPage( 0 );

                AddImage( 0, 0, 0x816 );
                AddButton( 34, 74, 0x81A, 0x81B, 1, GumpButtonType.Reply, 0 ); // OK
                AddButton( 88, 74, 0x995, 0x996, 0, GumpButtonType.Reply, 0 ); // Cancel

                //string msg = "Are you sure?";
                AddLabel( 42, 25, 63, msg );
            }

            //Handles button presses
            public override void OnResponse( NetState state, RelayInfo info )
            {
                bool theAnswer = ( info.ButtonID == 1 );

                _CallBack( _CallingPlayer, theAnswer, _State );
            }
        }
    }
}


