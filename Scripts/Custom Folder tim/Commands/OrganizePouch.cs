/*
 * Organize Me by Tresdni
 * www.uofreedom.com
 * Instantly organize your backpack with a simple command.
 */

namespace Server.Items
{
    public class OrganizePouch : BaseContainer
    {
        [Constructable]
        public OrganizePouch()
            : base(0xE79)
        {
            Weight = 1.0;
            Name = "Organization Pouch";
        }

        public OrganizePouch(Serial serial)
            : base(serial)
        {
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