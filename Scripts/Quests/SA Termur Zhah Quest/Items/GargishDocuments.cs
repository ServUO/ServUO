using Server;
using System;
using Server.Multis;
using Server.Gumps;
using Server.ContextMenus;
using System.Collections.Generic;

namespace Server.Items
{
    public class GargishDocumentBook : BaseLocalizedBook, ISecurable
    {
        private SecureLevel m_Level;

        public override int[] Contents
        {
            get { return new int[] { }; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        public GargishDocumentBook()
        {
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Title is int)
                list.Add(1150928, String.Format("#{0}", (int)Title));  // Gargish Document - ~1_NAME~
            else if (Title is string)
                list.Add(1150928, (string)Title);
            else
                base.AddNameProperty(list);
        }

        public GargishDocumentBook( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
            writer.Write((int)m_Level);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
            m_Level = (SecureLevel)reader.ReadInt();
		}
    }
	
	public class GargishDocumentNote : Note
	{
        public virtual int Title { get { return 0; } }

        public GargishDocumentNote()
            : base()
        {
        }

		public GargishDocumentNote(int content) : base(content)
		{
		}

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1150928, String.Format("#{0}", Title));  // Gargish Document - ~1_NAME~
        }
		
		public GargishDocumentNote( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class ChallengeRite : GargishDocumentBook
	{
        public override object Title { get { return 1150904; } } // The Challenge Rite
		public override object Author { get { return "unknown"; } }
        public override int[] Contents
        {
            get
            {
                return new int[] { 1150915, 1150916, 1150917, 1150918, 1150919, 1150920, 1150921, 1150922 };
            }
        }
		
        [Constructable]
		public ChallengeRite()
		{
			Hue = 1007;
		}
		
		public ChallengeRite( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class OnTheVoid : GargishDocumentBook
	{
        public override object Title { get { return 1150907; } } // On the Void
        public override object Author { get { return "Prugyilonus"; } }
        public override int[] Contents
        {
            get
            {
                return new int[] { 1150894, 1150895, 1150896 };
            }
        }

        [Constructable]
		public OnTheVoid()
		{
			Hue = 404;
		}
		
		public OnTheVoid( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class InMemory : GargishDocumentBook
	{
        public override object Title { get { return 1150913; } } // In Memory
        public override object Author { get { return "Queen Zhah"; } }
        public override int[] Contents
        {
            get
            {
                return new int[] { 1151071, 1151072, 1151073 };
            }
        }

        [Constructable]
		public InMemory()
		{
			Hue = 375;
		}
		
		public InMemory( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

    public class ChronicleOfTheGargoyleQueen1 : GargishDocumentBook
    {
        public static void Initialize()
        {
            for(int i = 0; i < 34; i++)
            {
                if(i == 0)
                    m_Contents[i] = 1150901;
                else
                {
                    m_Contents[i] = 1150943 + (i - 1);
                }
            }
        }

        private static int[] m_Contents = new int[34];

        public override object Title { get { return 1150914; } } // Chronicle of the Gargoyle Queen Vol. 1
        public override object Author { get { return "Queen Zhah"; } }
        public override int[] Contents { get { return m_Contents; } }

        private int m_Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return m_Charges; } set { m_Charges = value; InvalidateProperties(); } }

        [Constructable]
        public ChronicleOfTheGargoyleQueen1()
        {
            Hue = 567;
            m_Charges = 500;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153098, m_Charges.ToString());
        }

        public ChronicleOfTheGargoyleQueen1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
            m_Charges = reader.ReadInt();
        }
    }
	
	public class AnthenaeumDecree : GargishDocumentNote
	{
        public override int Title { get { return 1150905; } } // Athenaeum Decree

        [Constructable]
		public AnthenaeumDecree() : base(1150891)
		{
		}
		
		public AnthenaeumDecree( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class LetterFromTheKing : GargishDocumentNote
	{
        public override int Title { get { return 1150906; } } // A Letter from the King

        private string m_Content = "To Her Honor the High Broodmother, Lady Zhah from his majesty, King Trajalem:<br><br>	High Broodmother, I have received your latest petition regarding your desires and I once again must remind you that I have absolutely no interest in altering tradition or granting you the freedom from the slavery you have deluded yourself into believing makes up your life.<br><br>Please remember that your office may be stripped by me if you are deemed unfit to lead the other Broodmothers. Be happy with your place and do not forget it; this is the last time I will lower myself to respond to these ridiculous accusations and requests.";

        [Constructable]
		public LetterFromTheKing() : base()
		{
            NoteString = m_Content;
		}
		
		public LetterFromTheKing( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class ShilaxrinarsMemorial : GargishDocumentNote
	{
        public override int Title { get { return 1150908; } } // Shilaxrinar's Memorial

        [Constructable]
		public ShilaxrinarsMemorial() : base(1150899)
		{
		}
		
		public ShilaxrinarsMemorial( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
	
	public class ToTheHighScholar : GargishDocumentNote
	{
        public override int Title { get { return 1150909; } } // To the High Scholar

        [Constructable]
		public ToTheHighScholar() : base(1151062)
		{
		}
		
		public ToTheHighScholar( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class ToTheHighBroodmother : GargishDocumentNote
	{
        public override int Title { get { return 1150910; } } // To the High Broodmother

        [Constructable]
		public ToTheHighBroodmother() : base(1151064)
		{
		}
		
		public ToTheHighBroodmother( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class ReplyToTheHighScholar : GargishDocumentNote
	{
        public override int Title { get { return 1150911; } } // Reply to the High Scholar

        [Constructable]
		public ReplyToTheHighScholar() : base(1151066)
		{
		}
		
		public ReplyToTheHighScholar( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

	public class AccessToTheIsle : GargishDocumentNote
	{
        public override int Title { get { return 1150912; } } // Access to the Isle

        [Constructable]
		public AccessToTheIsle() : base(1151069)
		{
		}
		
		public AccessToTheIsle( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}
			
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}
}