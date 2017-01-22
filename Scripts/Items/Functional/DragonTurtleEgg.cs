using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class DragonTurtleEgg : ChickenLizardEgg
    {
        public override bool CanMutate { get { return false; } }

        [Constructable]
        public DragonTurtleEgg()
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1156238); // Dragon Turtle
        }

        public override void OnHatch(Mobile from)
        {
            BaseCreature bc = new DragonTurtleHatchling();

            from.SendLocalizedMessage(1156239); // You hatch a dragon turtle!

            bc.MoveToWorld(from.Location, from.Map);
            Delete();
        }

        public DragonTurtleEgg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}