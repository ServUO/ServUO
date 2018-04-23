using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Commands;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Scripts.ColorPicker
{
    public static class ColorPicker
    {
        #region HUES
        public const int STANDART_HUE_FIRST = 0x2;
        public const int STANDART_HUE_LAST = 0x3E9;

        public const int SKIN_HUE_FIRST = 0x3EA;
        public const int SKIN_HUE_LAST = 0x422;

        public const int HAIR_HUE_FIRST = 0x44E;
        public const int HAIR_HUE_LAST = 0x47D;
        #endregion

        public static void Initialize()
	    {
		    CommandSystem.Register( "ColorPicker", AccessLevel.GameMaster, new CommandEventHandler( ColorPicker_OnCommand ) );
	    }

        [Usage( "ColorPicker" )]
		[Description( "Shows a color listing" )]
        public static void ColorPicker_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendGump( new ColorPickerGump( e.Mobile ) );
        }

        public class ColorPickerGump : Gump
        {
            public delegate void OnSelectedColor( ColorPickerGump colorPicker, Mobile mobile, int hue );

            public const int DEFAULT_FIRST = 0x1;
            public const int DEFAULT_LAST = 0xB9A;

            private const int DEFAULT_IMAGE = 0x1EFD; //Fancy Shirt
            private const int DEFAULT_SPACE = 20;
            private const int DEFAULT_LENGTH = 15;

            private const int BUTTON_NEXT = -1;
            private const int BUTTON_PREV = -2;
            private const int BUTTON_PREV5 = -3;
            private const int BUTTON_NEXT5 = -4;
            private const int BUTTON_MODEL = -5;

            private Mobile m_Selector;
            private int m_First;
            private int m_Last;
            private int m_Current;
            private int m_ItemImage;
            private int m_Space;
            private int m_Length;
            private bool m_EnableSetModel;
            private OnSelectedColor m_Action;

            public ColorPickerGump( Mobile m, ColorPickerGump cpg ) : this( m, cpg, cpg.m_ItemImage )
            { }

            public ColorPickerGump( Mobile m, ColorPickerGump cpg, int i ) : this( m, cpg.m_Action, cpg.m_EnableSetModel, cpg.m_First, cpg.m_Last, cpg.m_Current, i, cpg.m_Space, cpg.m_Length )
            { }

            public ColorPickerGump( Mobile m ) 
                : this( m, delegate( ColorPickerGump cp, Mobile mob, int h ) { mob.Target = new ColorTarget( cp, h ); }, true, DEFAULT_FIRST, DEFAULT_LAST )
            { }

            public ColorPickerGump( Mobile m, OnSelectedColor osc, int i, int cs, int ce )
                : this( m, osc, false, cs, ce, cs, i, DEFAULT_SPACE, DEFAULT_LENGTH )
            { }

            public ColorPickerGump( Mobile m, OnSelectedColor osc, bool esm, int cs, int ce )
                : this( m, osc, esm, cs, ce, cs, DEFAULT_IMAGE, DEFAULT_SPACE, DEFAULT_LENGTH )
            { }

            public ColorPickerGump( Mobile m, OnSelectedColor osc, bool esm, int cs, int ce, int cc, int i, int s, int l ) : base( 100, 100 )
            {
                m_Selector = m;
                m_Action = osc;
                m_First = cs;
                m_Last = ce;
                m_Current = cc;
                m_ItemImage = i;
                m_Space = s;
                m_Length = l;
                m_EnableSetModel = esm;

                //###### Initialization
                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;
                
                int width = 140;
                int height = m_Length * ( m_Space + 2 ) + 70;

                AddBackground( 0, 0, width, height, 0xE10 );
                AddAlphaRegion( 15, 15, width - 30, height - 30 );
                AddLabel( 35, 15, 168, "Pick a Color" );

                if (m_EnableSetModel)
                    AddButton( ( width / 2 ) - 7, height - 40, 0x2C89, 0x2C8A, BUTTON_MODEL, GumpButtonType.Reply, 0 ); //Select new model

                //###### Show Colors
                int total = Math.Min( m_Length, m_Last - m_Current );
                int ey = 40;

                for ( int index = 0; index < total; index++ )
                {
                    int cy = ey + ( index * m_Space );
                    int hue = m_Current + index;

                    AddItem( 10, cy, m_ItemImage, hue );
                    AddLabel( 65, cy, 371, hue.ToString() );
                    AddButton( width - 35, cy, 0x2622, 0x2623, hue, GumpButtonType.Reply, 0 );  //Set new Color
                }

                //###### Buttons next and previous
                if ( m_Current - m_Length >= m_First )  //Previous m_Length hues
                    AddButton( 20, height - 40,  0x26B6, 0x26B7, BUTTON_PREV, GumpButtonType.Reply, 0 );
                if ( m_Current - ( m_Length * 5 ) >= m_First) //Previous m_Length * 5 hues
                    AddButton( 40, height - 40, 0x26B5, 0x26B5, BUTTON_PREV5, GumpButtonType.Reply, 0);
                if ( m_Current + m_Length <= m_Last )   //Next m_Length hues
                    AddButton( width - 40, height - 40, 0x26B0, 0x26B1, BUTTON_NEXT, GumpButtonType.Reply, 0 );
                if ( m_Current + ( m_Length * 5 ) <= m_Last )   //Next m_Length * 5 hues
                    AddButton( width - 60, height - 40, 0x26AF, 0x26BAF, BUTTON_NEXT5, GumpButtonType.Reply, 0 ); 
            }

            public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
            {
                int b = info.ButtonID;

                if ( b == 0 )  //Disposed
                    return;
                else if ( b == BUTTON_MODEL )
                {
                    m_Selector.Target = new ColorModelTarget( m_Selector, this );
                }
                else if ( b == BUTTON_NEXT || b == BUTTON_PREV || b == BUTTON_NEXT5 || b == BUTTON_PREV5 )
                {
                    int nextCurrent = m_Current;

                    switch ( b )
                    {
                        case BUTTON_NEXT: nextCurrent += m_Length; break;
                        case BUTTON_NEXT5: nextCurrent += m_Length * 5; break;
                        case BUTTON_PREV: nextCurrent -= m_Length; break;
                        case BUTTON_PREV5: nextCurrent -= m_Length * 5; break;
                    }

                    m_Selector.SendGump( new ColorPickerGump( m_Selector, m_Action, m_EnableSetModel, m_First, m_Last, nextCurrent, m_ItemImage, m_Space, m_Length ) );
                }
                else
                {
                    m_Action( this, sender.Mobile, b );
                }
            }
        }

        public class ColorTarget : Target
        {
            private int m_Hue;
            private ColorPickerGump m_Gump;

            public ColorTarget( ColorPickerGump colorPicker, int hue ) : base( 20, false, TargetFlags.None )
            {
                m_Hue = hue;
                m_Gump = colorPicker;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is Mobile )
                {
                    Mobile m = targeted as Mobile;
                    m.Hue = m_Hue;
                }
                else if ( targeted is Item )
                {
                    Item i = targeted as Item;
                    i.Hue = m_Hue;
                }
                else
                {
                    from.SendMessage( "Invalid Target" );
                }

                from.SendGump( new ColorPickerGump( from, m_Gump ) );
            }
        }

        public class ColorModelTarget : Target
        {
            private Mobile m_Selector;
            private ColorPickerGump m_Prev;

            public ColorModelTarget( Mobile m, ColorPickerGump cpg ) : base( 20, false, TargetFlags.None )
            {
                m_Selector = m;
                m_Prev = cpg;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is Item )
                {
                    Item i = targeted as Item;
                    from.SendGump( new ColorPickerGump( from, m_Prev, i.ItemID ) );
                }
                else
                {
                    from.SendMessage( "Target is not an item" );
                    from.SendGump( new ColorPickerGump( from, m_Prev ) );
                }
            }
        }
    }
}
