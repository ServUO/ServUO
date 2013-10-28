using System;

namespace Server.Factions
{
    public class TownMonolith : BaseMonolith
    {
        public TownMonolith()
            : this(null)
        {
        }

        public TownMonolith(Town town)
            : base(town, null)
        {
        }

        public TownMonolith(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultLabelNumber
        {
            get
            {
                return 1041403;
            }
        }// A Faction Town Sigil Monolith
        public override void OnTownChanged()
        {
            this.AssignName(this.Town == null ? null : this.Town.Definition.TownMonolithName);
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