namespace Server.Items
{
    public class MechanicalComponent : BaseDecayingItem
    {
        [Constructable]
        public MechanicalComponent()
            : base(0x2DD7)
        {
            Hue = 2500;
            Weight = 1;
        }

        public override int Lifespan => 259200;
        public override bool UseSeconds => false;

        public override int LabelNumber => 1153865;  // Mechanical Component

        public MechanicalComponent(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Skills[SkillName.Tinkering].Value >= 80.0)
            {
                from.AddToBackpack(new ExoticToolkit());
                Delete();
                from.SendLocalizedMessage(1152369); // You successfully convert the component into an exotic tool kit.
            }
            else
            {
                from.SendLocalizedMessage(1152370); // Only an Adept Tinker would know what to do with this. 
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