using System;

namespace Server.Items
{ 
    public class BlightedGroveTele : Teleporter
    { 
        [Constructable]
        public BlightedGroveTele()
            : base()
        {
        }

        public BlightedGroveTele(Serial serial)
            : base(serial)
        {
        }

        public static BoneMachete GetBoneMachete(Mobile m)
        {
            for (int i = 0; i < m.Items.Count; i ++)
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
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (!MondainsLegacy.BlightedGrove && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Blighted Grove"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }
			
            BoneMachete machete = GetBoneMachete(m);
			
            if (machete != null)
            {
                if (Utility.RandomDouble() < 0.75 || machete.Insured || machete.LootType == LootType.Blessed)
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
            else
                m.SendLocalizedMessage(1074275); // You are unable to push your way through the tangling roots of the mighty tree.
				
            return true;
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

    public class BlightedGroveTreeInTele : Teleporter
    { 
        [Constructable]
        public BlightedGroveTreeInTele()
            : base()
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }

    public class BlightedGroveTreeOutTele : Teleporter
    { 
        [Constructable]
        public BlightedGroveTreeOutTele()
            : base()
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
			
            int version = reader.ReadInt();
        }
    }
}