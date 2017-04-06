using System;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAttribute(0x9DB1, 0x9DB2)]
    public class BlackrockAutomatonHead : KotlAutomatonHead
    {
        public override int LabelNumber { get { return 1157220; } } // Blackrock Automaton Head

        [Constructable]
        public BlackrockAutomatonHead()
        {
            Hue = 1175;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1157022, "#1157178"); // Rebuilt ~1_MATTYPE~ Automaton Head
        }

        public override KotlAutomaton GetAutomaton(Mobile master)
        {
            return new BlackrockAutomaton();
        }

        public BlackrockAutomatonHead(Serial serial)
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
