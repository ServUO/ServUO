using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;

namespace Server.Commands
{
        /// <summary>
        /// David M. O'Hara
        /// 08-11-04
        /// Version 2.1
        /// Gives item (targeted or given type) into bank box. Distribution can be 1 per account, 1 per character, or
        /// based on AccessLevel (good for staff items).
        /// </summary>

        // Update by X-SirSly-X
        // 12/15/2005
        // www.LandofObsidian.com
        // The update fixes a issue when a item is given only once per account. The problem happens when a player deletes their first char which is char slot 0. So if char slot 0 is empty it just skips over that player, and they end up not getting a item in their bank.


        public class AddToBank
        {
                public static void Initialize()
                {
                        // alter AccessLevel to be AccessLevel.Admin if you only want admins to use.
                        CommandSystem.Register( "AddToBank", AccessLevel.Administrator, new CommandEventHandler( AddToBank_OnCommand ) );
                }

                private static void AddToBank_OnCommand( CommandEventArgs e )
                {
                        e.Mobile.SendGump( new AddToBankGump() );
                }

                private static void PlaceItemIn( Container parent, int x, int y, Item item )
                {
                        parent.AddItem( item );
                        item.Location = new Point3D( x, y, 0 );
                }

                #region " Targeting/Dupe System "

                public class DupeTarget : Target
                {
                        private bool m_InBag;
                        private int m_Amount;
                        private int m_GiveRule;
                        private int m_Access;

                        public DupeTarget( bool inbag, int amount, int give, int access ) : base( 15, false, TargetFlags.None )
                        {
                                m_InBag = inbag;
                                m_Amount = amount;
                                m_GiveRule = give;
                                m_Access = access;
                        }

                        protected override void OnTarget( Mobile from, object targ )
                        {
                                if ( !(targ is Item) )
                                {
                                        from.SendMessage( "You can only dupe items." );
                                        return;
                                }

                                from.SendMessage( "Placing {0} into bank boxes...", ((Item)targ).Name == null ? "an item" : ((Item)targ).Name.ToString() );
                                CommandLogging.WriteLine( from, "{0} {1} adding {2} to bank boxes )", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( targ ) );

                                GiveItem( from, (Item)targ, m_Amount, m_GiveRule, m_Access );
                        }
                }

                public static void GiveItem( Mobile from, Item item, int amount, int give, int access )
                {
                        bool done = true;
                        if ( give == (int)AddToBankGump.Switches.GiveToAccount )
                        {
                                done = AddToBank.GiveItemToAccounts( item, amount );
                        }
                        else if ( give == (int)AddToBankGump.Switches.GiveToCharacter )
                        {
                                done = AddToBank.GiveItemToCharacters( item, amount );
                        }
                        else if ( give == (int)AddToBankGump.Switches.GiveToAccessLevel )
                        {
                                done = AddToBank.GiveItemToAccessLevel( item, amount, access );
                        }
                       
                        if ( !done )
                        {
                                from.SendMessage( "Unable to give out to 1 or more players." );
                        }
                        else
                        {
                                from.SendMessage( "Completed." );
                        }

                }

                private static bool GiveItemToAccounts( Item item, int amount )
                {
                        bool success = true;
               
                        foreach ( Account acct in Accounts.GetAccounts() )
                        {
                                if ( acct[0] != null )
                                {
                                        if ( !CopyItem( item, amount, acct[0].BankBox ) )
                                        {
                                                Console.WriteLine( "Could not give item to {0}", acct[0].Name );
                                                success = false;
                                        }
                                }
                                else if ( acct[0] == null )
                                {
                                        if ( acct[1] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[1].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[1].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[2] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[2].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[2].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[3] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[3].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[3].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[4] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[4].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[4].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[5] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[5].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[5].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[6] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[6].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[6].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[7] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[7].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[7].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[8] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[8].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[8].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[9] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[9].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[9].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[10] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[10].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[10].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[11] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[11].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[11].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[12] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[12].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[12].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[13] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[13].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[13].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[14] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[14].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[14].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[15] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[15].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[15].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                                else if ( acct[0] == null )
                                {
                                        if ( acct[16] != null )
                                        {

                                                if ( !CopyItem( item, amount, acct[16].BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", acct[16].Name );
                                                        success = false;
                                                }
                                        }
                                }      
                               
                       
                        }
                        return success;
                }

                private static bool GiveItemToCharacters( Item item, int amount )
                {
                        bool success = true;
                        List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
                        foreach ( Mobile m in mobs )
                        {
                                if ( m is PlayerMobile )
                                {
                                        if ( !CopyItem( item, amount, m.BankBox ) )
                                        {
                                                Console.WriteLine( "Could not give item to {0}", m.Name );
                                                success = false;
                                        }
                                }
                        }
                        return success;
                }

                private static bool GiveItemToAccessLevel( Item item, int amount, int access )
                {
                        bool success = true;
                        List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
                        foreach ( Mobile m in mobs )
                        {
                                if ( m is PlayerMobile )
                                {
                                        bool give = false;
                                        if ( ( access & (int)AddToBankGump.Switches.Administrator ) != 0 && m.AccessLevel == AccessLevel.Administrator )
                                        {
                                                give = true;
                                        }
                                        else if ( ( access & (int)AddToBankGump.Switches.GameMaster ) != 0 && m.AccessLevel == AccessLevel.GameMaster )
                                        {
                                                give = true;
                                        }
                                        else if ( ( access & (int)AddToBankGump.Switches.Seer ) != 0 && m.AccessLevel == AccessLevel.Seer )
                                        {
                                                give = true;
                                        }
                                        else if ( ( access & (int)AddToBankGump.Switches.Counselor ) != 0 && m.AccessLevel == AccessLevel.Counselor )
                                        {
                                                give = true;
                                        }

                                        if ( give )
                                        {
                                                if ( !CopyItem( item, amount, m.BankBox ) )
                                                {
                                                        Console.WriteLine( "Could not give item to {0}", m.Name );
                                                        success = false;
                                                }
                                        }
                                }
                        }
                        return success;
                }

                private static bool CopyItem( Item item, int count, Container container)
                {
                        bool m_Success = false;
                        Type t = item.GetType();

                        ConstructorInfo[] info = t.GetConstructors();

                        foreach ( ConstructorInfo c in info )
                        {
                                ParameterInfo[] paramInfo = c.GetParameters();

                                if ( paramInfo.Length == 0 )
                                {
                                        object[] objParams = new object[0];

                                        try
                                        {
                                                for (int i=0;i<count;i++)
                                                {
                                                        object o = c.Invoke( objParams );

                                                        if ( o != null && o is Item )
                                                        {
                                                                Item newItem = (Item)o;
                                                                CopyProperties( newItem, item );
                                                                newItem.Parent = null;

                                                                // recurse if container
                                                                if ( item is Container && newItem.Items.Count == 0 )
                                                                {
                                                                        for ( int x=0;x<item.Items.Count;x++ )
                                                                        {
                                                                                m_Success = CopyItem( (Item)item.Items[x], 1, (Container)newItem );
                                                                        }
                                                                }

                                                                if ( container != null )
                                                                        PlaceItemIn( container, 20 + (i*10),10, newItem );
                                                        }
                                                }
                                                m_Success = true;
                                        }
                                        catch
                                        {
                                                m_Success = false;
                                        }
                                }

                        } // end foreach
                        return m_Success;

                } // end function

                private static void CopyProperties ( Item dest, Item src )
                {
                        PropertyInfo[] props = src.GetType().GetProperties();

                        for ( int i = 0; i < props.Length; i++ )
                        {
                                try
                                {
                                        if ( props[i].CanRead && props[i].CanWrite )
                                        {
                                                //Console.WriteLine( "Setting {0} = {1}", props[i].Name, props[i].GetValue( src, null ) );
                                                if ( src is Container && ( props[i].Name == "TotalWeight" || props[i].Name == "TotalItems" ) )
                                                {
                                                        // don't set these props
                                                }
                                                else
                                                {
                                                        props[i].SetValue( dest, props[i].GetValue( src, null ), null );
                                                }
                                        }
                                }
                                catch
                                {
                                        //Console.WriteLine( "Denied" );
                                }
                        }
                }
                #endregion

        } // end class

        #region " Gump "

        public class AddToBankGump : Gump
        {
                private int m_Amount;

                public void RenderGump()
                {
                        m_Amount = 1;
                        RenderGump( 100, 0, string.Empty );
                }

                public void RenderGump( int rule, int access, string type)
                {
                        AddPage( 0 );
                        AddBackground( 0, 0, 400, 270, 9260 );
                        AddLabel( 125, 20, 52, @"Distribute Items to Shard" );
                        AddLabel( 25, 40, 52, @"Rules:" );
                        AddLabel( 260, 60, 2100, @"Amount:" );
                        AddLabel( 315, 60, 2100, m_Amount.ToString() );
                        AddButton( 330, 62, 9700, 9701, (int)Buttons.IncAmount, GumpButtonType.Reply, 1 );
                        AddButton( 345, 62, 9704, 9705, (int)Buttons.DecAmount, GumpButtonType.Reply, -1 );
                        AddRadio( 35, 60, 209, 208, rule == (int)Switches.GiveToAccount, (int)Switches.GiveToAccount );
                        AddLabel( 65, 60, 2100, @"Per Account" );
                        AddRadio( 35, 80, 209, 208, rule == (int)Switches.GiveToCharacter, (int)Switches.GiveToCharacter );
                        AddLabel( 65, 80, 2100, @"Per Character (Mobile)" );
                        AddRadio( 35, 100, 209, 208, rule == (int)Switches.GiveToAccessLevel, (int)Switches.GiveToAccessLevel );
                        AddLabel( 65, 100, 2100, @"Per AccessLevel" );
                        AddCheck( 80, 125, 210, 211, ( access & (int)Switches.Administrator ) != 0, (int)Switches.Administrator );
                        AddLabel( 105, 125, 2100, @"Administrator" );
                        AddCheck( 215, 125, 210, 211, ( access & (int)Switches.GameMaster ) != 0, (int)Switches.GameMaster  );
                        AddLabel( 240, 125, 2100, @"GameMaster" );
                        AddCheck( 80, 150, 210, 211, ( access & (int)Switches.Seer ) != 0, (int)Switches.Seer );
                        AddLabel( 105, 150, 2100, @"Seer" );
                        AddCheck( 215, 150, 210, 211, ( access & (int)Switches.Counselor ) != 0, (int)Switches.Counselor );
                        AddLabel( 240, 150, 2100, @"Counselor" );

                        AddLabel( 80, 185, 52, @"Give By Type" );
                        AddLabel( 280, 185, 52, @"Give By Target" );
                        AddImageTiled( 40, 210, 160, 20, 9274 );
                        AddTextEntry( 45, 210, 150, 20, 2100, 100, type );
                        AddButton( 200, 210, 4014, 4016, (int)Buttons.GiveByType, GumpButtonType.Reply, 0 );
                        AddButton( 310, 210, 4005, 4007, (int)Buttons.GiveByTarget, GumpButtonType.Reply, 1 );
                }

                public AddToBankGump() : base( 50, 50 )
                {
                        RenderGump();
                }

                public AddToBankGump( int GiveRule, int Access, string TypeName, int Amount ) : base( 50, 50 )
                {
                        m_Amount = Amount;
                        RenderGump( GiveRule, Access, TypeName );
                }

                public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
                {
                        Mobile from = sender.Mobile;

                        string TypeName = string.Empty;
                        int GiveRule = 0;
                        int Access = 0;

                        foreach( int sw in info.Switches )
                        {
                                switch ( sw )
                                {
                                        case (int)Switches.GiveToCharacter:
                                        {
                                                GiveRule = (int)Switches.GiveToCharacter;
                                                break;
                                        }
                                        case (int)Switches.GiveToAccount:
                                        {
                                                GiveRule = (int)Switches.GiveToAccount;
                                                break;
                                        }
                                        case (int)Switches.GiveToAccessLevel:
                                        {
                                                GiveRule = (int)Switches.GiveToAccessLevel;
                                                break;
                                        }
                                        case (int)Switches.Administrator:
                                        case (int)Switches.GameMaster:
                                        case (int)Switches.Seer:
                                        case (int)Switches.Counselor:
                                        {
                                                Access += sw;
                                                break;
                                        }
                                }
                        }
                        if ( GiveRule == 0 )
                        {
                                from.SendMessage( "You must select the audience rule to receive the item." );
                                from.SendGump( new AddToBankGump( GiveRule, Access, TypeName, m_Amount ) );
                                return;
                        }
                        else if ( GiveRule == (int)Switches.GiveToAccessLevel && Access == 0 )
                        {
                                from.SendMessage( "You must select the AccessLevel to receive the item." );
                                from.SendGump( new AddToBankGump( GiveRule, Access, TypeName, m_Amount ) );
                                return;
                        }

                        switch( info.ButtonID )
                        {
                                case (int)Buttons.GiveByTarget:
                                {
                                        from.Target = new AddToBank.DupeTarget( false, m_Amount, GiveRule, Access );
                                        from.SendMessage( "What do you wish to give out?" );
                                        break;
                                }
                                case (int)Buttons.GiveByType:
                                {
                                        if ( info.TextEntries.Length > 0 )
                                        {
                                                TypeName = info.TextEntries[0].Text;
                                        }

                                        if ( TypeName == string.Empty )
                                        {
                                                from.SendMessage( "You must specify a type" );
                                                from.SendGump( new AddToBankGump( GiveRule, Access, TypeName, m_Amount ) );
                                        }
                                        else
                                        {
                                                Type type = ScriptCompiler.FindTypeByName( TypeName, true );
                                                if ( type == null )
                                                {
                                                        from.SendMessage( "{0} is not a valid type", type );
                                                        from.SendGump( new AddToBankGump( GiveRule, Access, string.Empty, m_Amount ) );
                                                        return;
                                                }
                                                else
                                                {
                                                        object obj = Activator.CreateInstance( type );
                                                        if ( obj is Item )
                                                                AddToBank.GiveItem( from, (Item)obj, m_Amount, GiveRule, Access );
                                                        else
                                                        {
                                                                from.SendMessage( "You may only duplicate items." );
                                                        }
                                                }
                                        }
                                        break;
                                }
                                case (int)Buttons.IncAmount:
                                {
                                        from.SendGump( new AddToBankGump( GiveRule, Access, TypeName, ++m_Amount ) );
                                        break;
                                }
                                case (int)Buttons.DecAmount:
                                {
                                        if ( m_Amount > 1 )
                                                m_Amount -= 1;
                                        else
                                                from.SendMessage( "You cannot give less than 1 item." );
                                        from.SendGump( new AddToBankGump( GiveRule, Access, TypeName, m_Amount ) );
                                        break;
                                }
                        }

                }

                public enum Buttons
                {
                        Cancel,
                        GiveByTarget,
                        GiveByType,
                        IncAmount,
                        DecAmount
                }

                public enum Switches
                {
                        Administrator = 1,
                        GameMaster = 2,
                        Seer = 4,
                        Counselor = 8,
                        GiveToAccount = 100,
                        GiveToCharacter = 200,
                        GiveToAccessLevel = 300
                }

        } // end class AddToBankGump

        #endregion

} // end namespace

