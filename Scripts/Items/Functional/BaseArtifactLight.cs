using System;

namespace Server.Items
{
    public abstract class BaseArtifactLight : BaseLight
    {
        public abstract int ArtifactRarity { get; }
        public virtual bool ShowArtifactRarity { get { return true; } }

        public BaseArtifactLight(int itemID)
            : base(itemID)
        {
        }

        public BaseArtifactLight(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (ShowArtifactRarity)
                list.Add(1061078, this.ArtifactRarity.ToString()); // artifact rarity ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }        

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}