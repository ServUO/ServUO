using System;
using Server;
using Server.Mobiles;
using Server.Engines.XmlSpawner2;

namespace Server.Items
{

    // ---------------------------------------------------
    // Key augment to open doors and locked containers
    // ---------------------------------------------------

    public class KeyAugment : BaseSocketAugmentation
    {

        private SkillName m_RequiredSkill = SkillName.Lockpicking;     // default skill required to use this augment
        private int m_RequiredSkillLevel = 100;               // default skill level required to use this augment
        private int m_OpenSound = 0x1F4;                            // default opening sound
        private int m_KeyValue;                                     // matched against the keyvalue of the door or container it is intended to open
        private int m_UsesRemaining = 1;                            // can only use once by default


        [CommandProperty( AccessLevel.GameMaster )]
        public SkillName RequiredSkill { get{ return m_RequiredSkill; } set { m_RequiredSkill = value; } }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int RequiredSkillLevel { get{ return m_RequiredSkillLevel; } set { m_RequiredSkillLevel = value; } }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int OpenSound { get{ return m_OpenSound; } set { m_OpenSound = value; } }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int KeyValue { get{ return m_KeyValue; } set { m_KeyValue = value; } }
        
        [CommandProperty( AccessLevel.GameMaster )]
        public int UsesRemaining { get{ return m_UsesRemaining; } set { m_UsesRemaining = value; InvalidateProperties(); } }


        [Constructable]
        public KeyAugment(string name, int keyvalue, string skillname, int minlevel) : base(0x1f14)
        {
            Hue = 20;
            Name = name;
            KeyValue = keyvalue;
            try{
            RequiredSkill = (SkillName)Enum.Parse(typeof(SkillName),skillname);
            } catch{}
            RequiredSkillLevel = minlevel;
        }

        [Constructable]
        public KeyAugment(string name, int keyvalue) : base(0x1f14)
        {
            Hue = 20;
            Name = name;
            KeyValue = keyvalue;
        }

        [Constructable]
        public KeyAugment(string name) : base(0x1f14)
        {
            Hue = 20;
            Name = name;
        }
        
        [Constructable]
        public KeyAugment() : base(0x1f14)
        {
            Hue = 20;
        }

        public override bool DestroyAfterUse { get { return (UsesRemaining == 0); } }

        public override int IconXOffset { get { return 5;} }

        public override int IconYOffset { get { return 20;} }

        public KeyAugment( Serial serial ) : base( serial )
		{
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            if(m_UsesRemaining >= 0)
                list.Add( 1060584, m_UsesRemaining.ToString() ); // uses remaining: ~1_val~
		}

        public override string OnIdentify(Mobile from)
        {
            return String.Format("Key to {0}", Name);
        }

        public override bool OnAugment(Mobile from, object target)
        {
            if(from == null || target == null || UsesRemaining == 0) return false;

            if(target is BaseDoor)
            {
                BaseDoor d = target as BaseDoor;
                
                // open the door
                d.Open = true;
                
                if(OpenSound > 0)
                    from.PlaySound( OpenSound );
                
                // and add a fresh socket attachment to the door
                XmlAttach.AttachTo(d,new XmlSockets(1,false));
                
                // decrement the uses
                if(UsesRemaining > 0)
                    UsesRemaining--;

                return true;
            } else
            if(target is LockableContainer)
            {
                LockableContainer d = target as LockableContainer;
                
                // open the container
                d.Locked = false;
                
                if(OpenSound > 0)
                    from.PlaySound( OpenSound );
                
                // decrement the uses
                if(UsesRemaining > 0)
                    UsesRemaining--;

                return true;
            }


            return false;
        }


        public override bool CanAugment(Mobile from, object target)
        {
            if(from == null) return false;

            if(target is BaseDoor || target is LockableContainer)
            {
                Item d = target as Item;

                if(RequiredSkillLevel > 0 && from.Skills[RequiredSkill].Value < RequiredSkillLevel)
                {
                    from.SendMessage("You lack the {0} skill to use this", RequiredSkill.ToString());
                    return false;
                }
                
                if((d is BaseDoor && ((BaseDoor)d).KeyValue == KeyValue) || (d is LockableContainer && ((LockableContainer)d).KeyValue == KeyValue))
                    return true;
            }

            return false;
        }
        

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			
			writer.Write( (int)m_RequiredSkill);
			writer.Write( (int)m_RequiredSkillLevel);
			writer.Write( (int)m_OpenSound);
			writer.Write( (int)m_KeyValue);
			writer.Write( (int)m_UsesRemaining);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_RequiredSkill = (SkillName)reader.ReadInt();
			m_RequiredSkillLevel = reader.ReadInt();
			m_OpenSound = reader.ReadInt();
			m_KeyValue = reader.ReadInt();
			m_UsesRemaining = reader.ReadInt();
		}
    }
}
