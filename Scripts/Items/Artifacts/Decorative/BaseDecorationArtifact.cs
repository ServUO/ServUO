namespace Server.Items
{
    public abstract class BaseDecorationArtifact : Item, IArtifact
    {
        public override bool IsArtifact => true;
        public virtual bool ShowArtifactRarity => true;

        public BaseDecorationArtifact(int itemID)
            : base(itemID)
        {
            Weight = 10.0;
        }

        public BaseDecorationArtifact(Serial serial)
            : base(serial)
        {
        }

        public abstract int ArtifactRarity { get; }
        public override bool ForceShowProperties => true;
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (ShowArtifactRarity)
                list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public abstract class BaseDecorationContainerArtifact : BaseContainer
    {
        public BaseDecorationContainerArtifact(int itemID)
            : base(itemID)
        {
            Weight = 10.0;
        }

        public BaseDecorationContainerArtifact(Serial serial)
            : base(serial)
        {
        }

        public abstract int ArtifactRarity { get; }
        public override bool ForceShowProperties => true;
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add(1061078, ArtifactRarity.ToString()); // artifact rarity ~1_val~
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}