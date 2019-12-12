using System;
using Server.Engines.Quests;
using Server.Mobiles;

namespace Server.Items
{
    public class BedlamTele : Item
    {
        [Constructable]
        public BedlamTele()
            : base(0x124D)
        {
            Movable = false;
        }

        public BedlamTele(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074161;
            }
        }// Access to Bedlam by invitation only
        
        public override bool ForceShowProperties { get { return ObjectPropertyList.Enabled; } }
        
        public virtual Type Quest
        {
            get
            {
                return typeof(MistakenIdentityQuest);
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.NetState == null || !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.					
                return;
            }
            else if (!MondainsLegacy.Bedlam && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(1042753, "Bedlam"); // ~1_SOMETHING~ has been temporarily disabled.
                return;
            }
		
            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;

                if (player.Bedlam)
                {
                    BaseCreature.TeleportPets(player, new Point3D(121, 1682, 0), this.Map);
                    player.MoveToWorld(new Point3D(121, 1682, 0), this.Map);
                }
                else
                {
                    player.SendLocalizedMessage(1074276); // You press and push on the iron maiden, but nothing happens.
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
