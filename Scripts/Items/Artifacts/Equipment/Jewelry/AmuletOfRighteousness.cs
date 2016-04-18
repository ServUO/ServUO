using System;
using Server.Targeting;

namespace Server.Items
{
    public class AmuletOfRighteousness : SilverNecklace, IUsesRemaining
	{
		public override bool IsArtifact { get { return true; } }
        private int m_UsesRemaining;
        [Constructable]
        public AmuletOfRighteousness()
            : this(100)
        {
        }

        [Constructable]
        public AmuletOfRighteousness(int uses)
            : base()
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;	
			
            this.m_UsesRemaining = uses;
        }

        public AmuletOfRighteousness(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075313;
            }
        }// Amulet of Righteousness
        public virtual bool ShowUsesRemaining
        {
            get
            {
                return true;
            }
            set
            {
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get
            {
                return this.m_UsesRemaining;
            }
            set
            {
                this.m_UsesRemaining = value;
                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);			
			
            list.Add(1060584, this.m_UsesRemaining.ToString()); // uses remaining: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
			
            if (this.IsChildOf(from.Backpack))
                from.Target = new InternalTarget(this);
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
			
            writer.Write((int)0); // version
			
            writer.Write((int)this.m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
			
            this.m_UsesRemaining = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            private readonly AmuletOfRighteousness m_Amulet;
            public InternalTarget(AmuletOfRighteousness amulet)
                : base(12, false, TargetFlags.None)
            {
                this.m_Amulet = amulet;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Amulet == null || this.m_Amulet.Deleted)
                    return;
					
                if (targeted is Mobile)
                {
                    Mobile target = (Mobile)targeted;
					
                    if (this.m_Amulet.UsesRemaining <= 0)
                    {
                        from.SendLocalizedMessage(1042544); // This item is out of charges.
                        return;
                    }
						
                    target.BoltEffect(0);				
                    this.m_Amulet.UsesRemaining -= 1;
                    this.m_Amulet.InvalidateProperties();
                }
            }
        }
    }
}