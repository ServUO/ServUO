using System;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class CitadelTele : Item
    { 
        [Constructable]
        public CitadelTele()
            : base(0xE3F)
        {
            Movable = false;
        }

        public CitadelTele(Serial serial)
            : base(serial)
        {
        }
        
        public override void OnDoubleClick(Mobile from)
        {
            if (from.NetState == null || !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return;
            }
            else if (!MondainsLegacy.Citadel && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(1042753, "The Citadel"); // ~1_SOMETHING~ has been temporarily disabled.
                return;
            }
		
            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;
				
                if (QuestHelper.GetQuest(player, typeof(BlackOrderBadgesQuest)) != null || QuestHelper.GetQuest(player, typeof(EvidenceQuest)) != null)
                {
                    BaseCreature.TeleportPets(player, new Point3D(107, 1883, 0), Map.Malas);
                    player.MoveToWorld(new Point3D(107, 1883, 0), Map.Malas);
                }
                else
                {
                    player.SendLocalizedMessage(1074278); // You realize that your eyes are playing tricks on you. That crate isn't really shimmering.
                }
            }
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
