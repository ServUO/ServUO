namespace Server.Items
{
    public class BlightedGroveTele : Teleporter
    {
        [Constructable]
        public BlightedGroveTele()
        {
        }

        public BlightedGroveTele(Serial serial)
            : base(serial)
        {
        }

        public static BoneMachete GetBoneMachete(Mobile m)
        {
            for (int i = 0; i < m.Items.Count; i++)
            {
                if (m.Items[i] is BoneMachete)
                    return (BoneMachete)m.Items[i];
            }

            if (m.Backpack != null)
                return m.Backpack.FindItemByType(typeof(BoneMachete), true) as BoneMachete;

            return null;
        }

        public override bool OnMoveOver(Mobile m)
        {
            BoneMachete machete = GetBoneMachete(m);

            if (machete != null)
            {
                if (0.6 > Utility.RandomDouble())
                {
                    m.SendLocalizedMessage(1075008); // Your bone handled machete has grown dull but you still manage to force your way past the venomous branches.
                }
                else
                {
                    machete.Delete();
                    m.SendLocalizedMessage(1075007); // Your bone handled machete snaps in half as you force your way through the poisonous undergrowth.
                }

                return base.OnMoveOver(m);
            }

            m.SendLocalizedMessage(1074275); // You are unable to push your way through the tangling roots of the mighty tree.

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class BlightedGroveTreeInTele : Teleporter
    {
        [Constructable]
        public BlightedGroveTreeInTele()
        {
        }

        public BlightedGroveTreeInTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            m.SendLocalizedMessage(1074162); // You notice a hole in the tree and climb down
            return base.OnMoveOver(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class BlightedGroveTreeOutTele : Teleporter
    {
        [Constructable]
        public BlightedGroveTreeOutTele()
        {
        }

        public BlightedGroveTreeOutTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            m.SendLocalizedMessage(1074163); // You find a way to climb back outside the tree
            return base.OnMoveOver(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
