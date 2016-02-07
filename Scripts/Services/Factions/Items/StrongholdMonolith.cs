using System;

namespace Server.Factions
{
    public class StrongholdMonolith : BaseMonolith
    {
        public StrongholdMonolith()
            : this(null, null)
        {
        }

        public StrongholdMonolith(Town town, Faction faction)
            : base(town, faction)
        {
        }

        public StrongholdMonolith(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultLabelNumber
        {
            get
            {
                return 1041042;
            }
        }// A Faction Sigil Monolith
        public override void OnTownChanged()
        {
            this.AssignName(this.Town == null ? null : this.Town.Definition.StrongholdMonolithName);
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