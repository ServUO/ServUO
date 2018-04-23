// Lootable Treasure Piles
// a RunUO ver 2.0 Script
// Written by David 
// last edited 6/17/06

/* Version 1.1; added MaxUses, fixed Serialize */
/* Version 1.2; Verified 2.0 compatibility, added Sound */

using System;
using Server;
using System.Collections;

namespace Server.Items
{
    #region TreasureLooter struct
    public struct TreasureLooter
    {
        public TreasureLooter( Mobile from )
        {
            Looter = from;
            Time = DateTime.Now;
        }

        public Mobile Looter;
        public DateTime Time;
    }
    #endregion

    #region BaseTreasure class
    public class BaseTreasure : BaseAddon
    {
        private ArrayList Looters = new ArrayList();
        private TreasureLooter m_Looter;
        private int m_usesCount;

        private TimeSpan m_LootWait = TimeSpan.FromHours( 18 );
        private int m_MinAmount = 100;
        private int m_MaxAmount = 250;
        private int m_MaxUses = 0; // use zero for never decays
        private double m_GemChance = .15;
        private bool m_Sound = true;

        public virtual TimeSpan LootWait { get { return m_LootWait; } set { m_LootWait = value; } }
        public virtual int MinAmount { get { return m_MinAmount; } set { m_MinAmount = value; } }
        public virtual int MaxAmount { get { return m_MaxAmount; } set { m_MaxAmount = value; } }
        public virtual int MaxUses { get { return m_MaxUses; } set { m_MaxUses = value; } }
        public virtual int UsesCount { get { return m_usesCount; } }
        public virtual double GemChance { get { return m_GemChance; } set { m_GemChance = value % 1; } }
        public virtual bool PlaySound { get { return m_Sound; } set { m_Sound = value; } }

        public BaseTreasure()
            : base()
        {
        }

        public virtual void OnLoot( Mobile from )
        {
            DefragLooters();
            if( !FindLooter( from ) )
            {
                int g = Utility.RandomMinMax( m_MinAmount, m_MaxAmount );
                from.AddToBackpack( new Gold( g ) );

                if (Utility.RandomDouble() < GemChance)
                {
                    if (Utility.RandomBool())
                        from.AddToBackpack(Loot.RandomGem());
                    else
                        from.AddToBackpack(Loot.RandomJewelry());

                    from.SendMessage("You retrieve {0} gold and a shiny bauble", g);
                    if( m_Sound )
                        Effects.PlaySound(from.Location, from.Map, 0x2E2);
                }
                else
                {
                    from.SendMessage("You retrieve {0} gold", g);
                    if (m_Sound)
                        Effects.PlaySound(from.Location, from.Map, 0x2E5);
                }

                Looters.Add( new TreasureLooter( from ) );
                m_usesCount++;

                if( m_MaxUses > 0 && m_usesCount >= m_MaxUses )
                {
                    from.SendMessage( "You have recovered the last of the gold!" );
                    if (m_Sound)
                        Effects.PlaySound(from.Location, from.Map, 0x2E3);
                    this.Delete();
                }
            }
            else
                from.SendMessage( "You find nothing of value" );
        }

        public bool FindLooter( Mobile from )
        {
            bool rtn = false;

            if( Looters.Count == 0 )
                return rtn;

            foreach( Object obj in Looters )
            {
                if( obj is TreasureLooter )
                {
                    m_Looter = (TreasureLooter)obj;

                    if( m_Looter.Looter == from )
                    {
                        rtn = true;
                        break;
                    }
                }
            }
            return rtn;
        }

        public void DefragLooters()
        {
            if( Looters.Count == 0 )
                return;

            for( int i = 0 ; i < Looters.Count ; i++ )
            {
                try
                {
                    if( Looters[i] is TreasureLooter )
                    {
                        m_Looter = (TreasureLooter)Looters[i];

                        if( m_Looter.Time + m_LootWait < DateTime.Now || m_Looter.Looter == null )
                        {
                            Looters.RemoveAt( i-- );
                        }
                    }
                    else
                    {
                        Looters.RemoveAt( i-- );
                    }
                }
                catch( Exception e )
                {
                    Console.Write( "BaseTreasure: " );
                    Console.WriteLine( e );
                }
            }
        }

        public BaseTreasure( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)2 );

            writer.Write( m_Sound );
            writer.Write( m_LootWait );
            writer.Write( m_GemChance );
            writer.Write( m_MinAmount );
            writer.Write( m_MaxAmount );
            writer.Write( m_MaxUses );
            writer.Write( m_usesCount );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();

            switch( version )
            {
                case 2:
                    {
                        m_Sound = reader.ReadBool();
                        goto case 1;
                    }
                case 1:
                    {
                        m_LootWait = reader.ReadTimeSpan();
                        m_GemChance = reader.ReadDouble();
                        m_MinAmount = reader.ReadInt();
                        m_MaxAmount = reader.ReadInt();
                        m_MaxUses = reader.ReadInt();
                        m_usesCount = reader.ReadInt();
                        break;
                    }
            }
        }
    }
    #endregion

    #region TreasureComponent class
    public class TreasureComponent : AddonComponent
    {
        // Note: will crash if used in Addon other than BaseTreasure.
        [CommandProperty( AccessLevel.GameMaster )]
        public virtual TimeSpan LootWait
        {
            get { return ( (BaseTreasure)base.Addon ).LootWait; }
            set { ( (BaseTreasure)base.Addon ).LootWait = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int MinAmount
        {
            get { return ( (BaseTreasure)base.Addon ).MinAmount; }
            set { ( (BaseTreasure)base.Addon ).MinAmount = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int MaxAmount
        {
            get { return ( (BaseTreasure)base.Addon ).MaxAmount; }
            set { ( (BaseTreasure)base.Addon ).MaxAmount = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int MaxUses
        {
            get { return ( (BaseTreasure)base.Addon ).MaxUses; }
            set { ( (BaseTreasure)base.Addon ).MaxUses = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public virtual int UsesCount
        {
            get { return ( (BaseTreasure)base.Addon ).UsesCount; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double GemChance
        {
            get { return ((BaseTreasure)base.Addon).GemChance; }
            set { ((BaseTreasure)base.Addon).GemChance = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool PlaySound
        {
            get { return ((BaseTreasure)base.Addon).PlaySound; }
            set { ((BaseTreasure)base.Addon).PlaySound = value; }
        }

        public TreasureComponent(int itemID)
            : base( itemID )
        {
            Name = "A Pile of Treasure";
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( this, 1 ) )
            {
                if( base.Addon is BaseTreasure )
                    ( (BaseTreasure)base.Addon ).OnLoot( from );
            }
        }

        public TreasureComponent( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
    #endregion

    #region TreasureEast class
    public class TreasureEast : BaseTreasure
    {
        [Constructable]
        public TreasureEast()
        {
            AddComponent( new TreasureComponent( 0x1b59 ), 0, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b5a ), 1, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b60 ), 0, 1, 0 );
        }

        public TreasureEast( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
    #endregion

    #region TreasureSouth class
    public class TreasureSouth : BaseTreasure
    {
        [Constructable]
        public TreasureSouth()
        {
            AddComponent( new TreasureComponent( 0x1b54 ), 0, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b42 ), 0, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b43 ), 1, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b44 ), 2, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b41 ), 0, 1, 0 );
            AddComponent( new TreasureComponent( 0x1b40 ), 1, 1, 0 );
            AddComponent( new TreasureComponent( 0x1b3f ), 2, 1, 0 );
        }

        public TreasureSouth( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
    #endregion

    #region TreasurePile class
    public class TreasurePile : BaseTreasure
    {
        [Constructable]
        public TreasurePile()
        {
            AddComponent( new TreasureComponent( 0x1b57 ), 0, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b58 ), 1, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b45 ), 2, 0, 0 );
            AddComponent( new TreasureComponent( 0x1b5b ), 0, 1, 0 );
            AddComponent( new TreasureComponent( 0x1b55 ), 1, 1, 0 );
        }

        public TreasurePile( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
        }
    }
    #endregion
}
