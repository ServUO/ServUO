using Server.Engines.Quests;
using Server.Mobiles;
using System;

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

        public override int LabelNumber => 1074161;// Access to Bedlam by invitation only

        public override bool ForceShowProperties => true;

        public virtual Type Quest => typeof(MistakenIdentityQuest);
        public override void OnDoubleClick(Mobile from)
        {
            if (!MondainsLegacy.Bedlam && (int)from.AccessLevel < (int)AccessLevel.GameMaster)
            {
                from.SendLocalizedMessage(1042753, "Bedlam"); // ~1_SOMETHING~ has been temporarily disabled.
                return;
            }

            if (from is PlayerMobile)
            {
                PlayerMobile player = (PlayerMobile)from;

                if (player.Bedlam)
                {
                    BaseCreature.TeleportPets(player, new Point3D(121, 1682, 0), Map);
                    player.MoveToWorld(new Point3D(121, 1682, 0), Map);
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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
