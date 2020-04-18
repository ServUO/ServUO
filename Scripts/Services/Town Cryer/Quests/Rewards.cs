namespace Server.Items
{
    public class HuntmastersQuestRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158140);  // Hunter

        [Constructable]
        public HuntmastersQuestRewardTitleDeed()
        {
        }

        public HuntmastersQuestRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class PaladinOfTrinsicRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158090);  // Paladin of Trinsic

        [Constructable]
        public PaladinOfTrinsicRewardTitleDeed()
        {
        }

        public PaladinOfTrinsicRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class RightingWrongRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158161);  // Warden of Wrong

        [Constructable]
        public RightingWrongRewardTitleDeed()
        {
        }

        public RightingWrongRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class TreasureHunterRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158389);  // Treasure Hunter

        [Constructable]
        public TreasureHunterRewardTitleDeed()
        {
        }

        public TreasureHunterRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class HeroOfMincRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158278);  // Hero of Minoc

        [Constructable]
        public HeroOfMincRewardTitleDeed()
        {
        }

        public HeroOfMincRewardTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class DespiseTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1158303);  // The Battle of Wisps TODO: Correct cliloc

        [Constructable]
        public DespiseTitleDeed()
        {
        }

        public DespiseTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class ExploringTheDeedTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1154505);  // Salvager of the Deep

        [Constructable]
        public ExploringTheDeedTitleDeed()
        {
        }

        public ExploringTheDeedTitleDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}