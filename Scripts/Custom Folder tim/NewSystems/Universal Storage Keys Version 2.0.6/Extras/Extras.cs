//To enable the keyguard functionality, uncomment the following line
#define XML_ATTACHMENT_PRESENT
//#define FS_ATS_PETS_PRESENT

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;

#if XML_ATTACHMENT_PRESENT
using Server.Engines.XmlSpawner2;
#endif

namespace Solaris.Extras
{
    //this class contains configuration-specific checks
    public class CheckExtras
    {
        public static bool Add(Item item)
        {
            //this code requires XML attachment tool - preprocessor symbol needs to be set to allow this
#if (XML_ATTACHMENT_PRESENT)
            {
                //don't add with 'Guard' Attachment
                Keyguard GuardAtt = new Keyguard();
                if (XmlAttach.FindAttachment(item,typeof(Keyguard)) != null)
                {
                    GuardAtt = (Keyguard)XmlAttach.FindAttachment(item,typeof(Keyguard));

                    if (GuardAtt.Guard)
                    {
                        return false;
                    }
                }
            }
#endif

            return true;
        }
    }

    public class ExecuteExtras
    {
        public static void ToggleKeyGuard(Mobile from,Item item)
        {
#if (XML_ATTACHMENT_PRESENT)
            {
                //Get guardatt attachment
                Keyguard guardatt = (Keyguard)XmlAttach.FindAttachment(item,typeof(Keyguard));

                // does item already have an attachment?
                if (guardatt == null)
                {
                    // create a new attachment and add it
                    XmlAttach.AttachTo(item,new Keyguard());

                    guardatt = (Keyguard)XmlAttach.FindAttachment(item,typeof(Keyguard));
                    guardatt.Guard = true;
                    guardatt.SetAttachedBy(from.Name);
                    guardatt.Name = "Keyguard";
                    from.SendMessage(264,"Item is now Guarded");
                }
                else
                {
                    //delete the attachment to reduce soap film buildup!
                    guardatt.Delete();
                    from.SendMessage(33,"Item is now Un-Guarded");
                }
            }
#else
	    	{
		    	from.SendMessage( "This command only works when XML attachments are enabled." );
	    	}
#endif
        }
    }
}

// Code originally from Keyguard.cs
// Author: datguy (Morpheus)
// Version: 1.0
// Requirements: Runuo 2.0, XmlSpawner2
// Server Tested with: 2.0 build 2702
// Revision Date: 8/23/2008
// Purpose: Things marked will not auto go into storage keys

#if( XML_ATTACHMENT_PRESENT )
namespace Server.Engines.XmlSpawner2
{
    public class Keyguard : XmlAttachment
    {
        //private int m_location;
        private bool m_guard;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Guard { get { return m_guard; } set { m_guard = value; } }

        [Attachable]
        public Keyguard()
        {
        }

        [Attachable]
        public Keyguard(bool value)
        {
            m_guard = value;
        }
		
	/*************************************************/
	/** BEGIN EDITS FOR DISPLAY ON ITEM PROPERTIES **/
	/*************************************************/
	
		public override void AddProperties(ObjectPropertyList list)
		{
			base.AddProperties(list);
			if (AttachedTo is Item)
            {
				list.Add("<BASEFONT COLOR=#00FF00>[Key Guard]<BASEFONT COLOR=#FFFFFF>"); //FFFFFF
			}
		}
		
		/***
		
		IMPORTANT NOTE:
		
			InvalidateProperties needs to be called within the OnAttach/OnDelete methods.
			Calling it in any other way seems to cause a stack overflow error and the entire shard will crash.
			
		***/
		
		public void InvalidateParentProperties()
		{
			if (AttachedTo is Item)
				((Item)AttachedTo).InvalidateProperties();
		}
		
		public override void OnAttach()
		{
			base.OnAttach();
			
			InvalidateParentProperties();
		}
		
		/// This is when the Attachment is "Deleted" as in Removed or Detached 
		public override void OnDelete()
		{
			InvalidateParentProperties();
		}
	
	/***********************************************/
	/** END EDITS FOR DISPLAY ON ITEM PROPERTIES **/
	/*********************************************/
	
	
        //serial constructor
        public Keyguard(ASerial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(m_guard);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            // version 0
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_guard = reader.ReadBool();
                        break;
                    }
            }
        }
    }
}
#endif

#if (FS_ATS_PETS_PRESENT)
namespace Solaris.ItemStore
{
	//stores power scrolls
	public class PetPowerScrollListEntry : ItemListEntry 
	{
		public override int GumpWidth{ get{ return 400; } }

		protected SkillName _Skill;
		protected double _Value;

		public SkillName Skill{ get{ return _Skill; } }
		public double Value{ get{ return _Value; } }

		public override List<ItemListEntryColumn> Columns
		{
			get
			{
				if( _Columns == null )
				{
					List<ItemListEntryColumn> columns = base.Columns;

					_Columns.Add( new ItemListEntryColumn( 250, "Value", _Value.ToString() ) );

					return columns;
				}

				return _Columns;
			}
		}

		//master constructor, use the addon deed class name, and hue
		public PetPowerScrollListEntry( Item item ) : base ( item, ((PetPowerScroll)item).Skill.ToString(), item.Hue - 1 )
		{
			PetPowerScroll scroll = (PetPowerScroll)item;

			_Skill = scroll.Skill;
			_Value = scroll.Value;
		}

		//world load constructor
		public PetPowerScrollListEntry( GenericReader reader ) : base( reader )
		{
		}

		//clone constructor
		public PetPowerScrollListEntry( PetPowerScrollListEntry entry ) : base( entry )
		{
			_Skill = entry.Skill;
			_Value = entry.Value;
		}

		//this generates an item from what is stored in the entry.  Note no exception handling
		public override Item GenerateItem()
		{
			//this allows for inherited classes of addon deeds to fit into this entry.
			PetPowerScroll scroll = (PetPowerScroll)Activator.CreateInstance( _Type, new object[]{ _Skill, _Value } );

			return scroll;
		}

		//this checks if the item you're attempting to create with is proper.  The child classes define specifics for this
		public override bool AllGood( Item item )
		{
			if( !base.AllGood( item ) )
			{
				return false;
			}

			//TODO: move this to base class, since the _Type is specified in ListEntry?
			if( !( item is PetPowerScroll ) )
			{
				return false;
			}

			return true;
		}

		//this is used to drive the cloning process - derived classes fire their associated clone constructor
		public override ItemListEntry Clone()
		{
			return new PetPowerScrollListEntry( this );
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 );
			writer.Write( (int)_Skill );
			writer.Write( _Value );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			_Skill = (SkillName)reader.ReadInt();
			_Value = reader.ReadDouble();
		}
	}
}
#endif