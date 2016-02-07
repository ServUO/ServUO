using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    [FlipableAttribute( 0x13E4, 0x13E3 )]
    public class ExceptionalSocketHammer : Item
    {
        private int m_UsesRemaining;                // if set to less than zero, becomes unlimited uses
		private int m_Level;
        
        [CommandProperty( AccessLevel.GameMaster )]
		public int UsesRemaining
		{
			get { return m_UsesRemaining; }
			set { m_UsesRemaining = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level
		{
			get { return m_Level; }
			set { m_Level = value; InvalidateProperties(); }
		}

        [Constructable]
        public ExceptionalSocketHammer() : this(50)
        {
        }
        
        [Constructable]
        public ExceptionalSocketHammer(int nuses) : base(0x13E4)
        {
            Name = "An Exceptional Socket Hammer";
            Hue = 5;
            UsesRemaining = nuses;
			int rand = Utility.Random(100);
			if(rand < 5)
			{
				m_Level = Utility.Random(4) + 1;
			}
			else
				if(rand < 10)
			{
				m_Level = Utility.Random(3) + 1;
			} 
			else
				if(rand < 20)
			{
				m_Level = Utility.Random(2) + 1;
			} 
			else
				if(rand < 40)
			{
				m_Level = Utility.Random(1) + 1;
			} 
			else
			{
				m_Level = 1;
			}
        }

        public ExceptionalSocketHammer( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            if(m_UsesRemaining >= 0)
                list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~

			list.Add( 1060658, "Level\t{0}", m_Level ); // ~1_val~: ~2_val~
		}
		
		public override void OnDoubleClick( Mobile from)
        {
            if(UsesRemaining == 0)
            {
                from.SendMessage("This hammer is now useless");
                return;
            }
            if ( IsChildOf( from.Backpack ) || Parent == from )
			{
                from.Target = new XmlSockets.AddSocketToTarget(m_Level);
                if(UsesRemaining > 0)
                    UsesRemaining--;
            } 
            else
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
			// version 1
			writer.Write(m_Level);
			// version 0
			writer.Write(m_UsesRemaining);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			switch(version)
			{
				case 1:
					m_Level = reader.ReadInt();
					goto case 0;
				case 0:
					m_UsesRemaining = reader.ReadInt();
					break;
			}
		}
    }
}
