using Server.Targeting;

namespace Server.Items
{
    public class ExoticToolkit : BaseDecayingItem
    {
        [Constructable]
        public ExoticToolkit()
            : base(0x1EB9)
        {
            Hue = 2500;
            Weight = 1;
        }

        public override int Lifespan => 604800;
        public override bool UseSeconds => false;

        public override int LabelNumber => 1153866;  // Exotic Toolkit

        public ExoticToolkit(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(500325); // I am too far away to do that.
            else
            {
                from.Target = new InternalTarget(this);
                from.SendLocalizedMessage(1152378); // Use this on a broken Nexus.
            }
        }

        public class InternalTarget : Target
        {
            public ExoticToolkit m_Toolkit;
            public InternalTarget(ExoticToolkit toolkit) : base(-1, true, TargetFlags.None)
            {
                m_Toolkit = toolkit;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is NexusComponent)
                {
                    NexusComponent addon = ((NexusComponent)targeted) as NexusComponent;

                    if (addon.Addon is ExodusNexus)
                    {
                        if (!((ExodusNexus)addon.Addon).Active)
                        {
                            ((ExodusNexus)addon.Addon).OpenGump(from);
                        }
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}