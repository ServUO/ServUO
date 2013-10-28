using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class GorgonLens : Item
    {
        private int m_Uses;
        [Constructable]
        public GorgonLens()
            : base(9908)
        {
            this.Name = "Gorgon Lens";
            this.Weight = 1.0;
            this.Hue = 1364;
            this.Movable = true;  
            this.LootType = LootType.Blessed; 
			
            this.Uses = 10;
        }

        public GorgonLens(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Uses
        {
            get
            {
                return this.m_Uses;
            }
            set
            {
                this.m_Uses = value;
                this.InvalidateProperties();
            }
        }
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1060658, "Uses remaining\t{0}", this.m_Uses.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(1112596); //Which item do you wish to enhance with Gorgon Lenses?
  
            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((int)this.m_Uses);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            switch ( version )
            {
                case 0:
                    {
                        this.m_Uses = (int)reader.ReadInt();

                        break;
                    }
            }
        }

        public bool ConsumeUse(Mobile from)
        {
            --this.Uses;
			
            if (this.Uses == 0)
            {
                from.SendLocalizedMessage(1112600); //Your lenses crumble. You are no longer protected from Medusa's gaze!
                this.Delete();

                return false;
            }
			
            return true;
        }

        private class InternalTarget : Target
        {
            private readonly GorgonLens m_Gorg;
            public InternalTarget(GorgonLens gorg)
                : base(10, false, TargetFlags.None)
            {
                this.m_Gorg = gorg;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = from as PlayerMobile;

                if (this.m_Gorg.Deleted)
                    return;

                if (targeted is BaseArmor)
                {
                    this.m_Gorg.Visible = false;
                    this.m_Gorg.Weight = 0;

                    from.SendLocalizedMessage(1112595); //You enhance the item with Gorgon Lenses!
                }
                else
                {
                    from.SendLocalizedMessage(1112594); //You cannot place gorgon lenses on this.  
                }
            }
        }
    }
}