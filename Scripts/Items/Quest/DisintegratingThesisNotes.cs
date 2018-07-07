using System;

namespace Server.Items
{
    public class DisintegratingThesisNotes : PeerlessKey
    {
        [Constructable]
        public DisintegratingThesisNotes()
            : base(0xE36)
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public DisintegratingThesisNotes(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber { get {return 1074440;} } // Disintegrating Thesis Notes
        
        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            if (!parent.Player && !parent.IsDeadBondedPet)
               return DeathMoveResult.MoveToCorpse;

            return base.OnInventoryDeath(parent);
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
