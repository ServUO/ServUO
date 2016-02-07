using System;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class TwistedWealdTele : Teleporter
    { 
        [Constructable]
        public TwistedWealdTele()
            : base(new Point3D(2189, 1253, 0), Map.Ilshenar)
        {
        }

        public TwistedWealdTele(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (!MondainsLegacy.TwistedWeald && (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Twisted Weald"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }
			
            if (m is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)m;
				
                if (QuestHelper.GetQuest(player, typeof(DreadhornQuest)) != null)
                    return base.OnMoveOver(m);
				
                player.SendLocalizedMessage(1074274); // You dance in the fairy ring, but nothing happens.
            }
			
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
}