using System;

namespace Server.Items
{
    public class EodonTribeRewardTitleToken : BaseRewardTitleToken
    {
        [Constructable]
        public EodonTribeRewardTitleToken() : base(18098)
        {
            Hue = 467;
        }

        public override void InitializeTitles()
        {
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156691), typeof(DefenderOfEodonTitleDeed)));         // Defender of Eodon
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156692), typeof(DefenderOfTheMyrmidexTitleDeed)));   // Defender of the Myrmidex
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156693), typeof(FlameOfTheJukariTitleDeed)));        // Flame of the Jukari
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156694), typeof(AmbusherOfTheKurakTitleDeed)));      // Ambusher of the Kurak
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156695), typeof(TrooperOfTheBarakoTitleDeed)));      // Trooper of the Barako
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156696), typeof(ThunderOfTheUraliTitleDeed)));       // Thunder of the Urali
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156697), typeof(HerderOfTheSakkhraTitleDeed))); 	    // Herder of the Sakkhra
            Titles.Add(new Tuple<TextDefinition, Type>(new TextDefinition(1156698), typeof(ColonizerOfTheBarrabTitleDeed)));    // Colonizer of the Barrab
        }

        public EodonTribeRewardTitleToken(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class DefenderOfEodonTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Defender of Eodon");

        [Constructable]
        public DefenderOfEodonTitleDeed()
        {
        }

        public DefenderOfEodonTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class DefenderOfTheMyrmidexTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Defender of the Myrmidex");

        [Constructable]
        public DefenderOfTheMyrmidexTitleDeed()
        {
        }

        public DefenderOfTheMyrmidexTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class FlameOfTheJukariTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Flame of the Jukari");

        [Constructable]
        public FlameOfTheJukariTitleDeed()
        {
        }

        public FlameOfTheJukariTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class AmbusherOfTheKurakTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Ambusher of the Kurak");

        [Constructable]
        public AmbusherOfTheKurakTitleDeed()
        {
        }

        public AmbusherOfTheKurakTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class TrooperOfTheBarakoTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Trooper of the Barako");

        [Constructable]
        public TrooperOfTheBarakoTitleDeed()
        {
        }

        public TrooperOfTheBarakoTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class ThunderOfTheUraliTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Thunder of the Urali");

        [Constructable]
        public ThunderOfTheUraliTitleDeed()
        {
        }

        public ThunderOfTheUraliTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class HerderOfTheSakkhraTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Herder of the Sakkhra");

        [Constructable]
        public HerderOfTheSakkhraTitleDeed()
        {
        }

        public HerderOfTheSakkhraTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }

    public class ColonizerOfTheBarrabTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition("Colonizer of the Barrab");

        [Constructable]
        public ColonizerOfTheBarrabTitleDeed()
        {
        }

        public ColonizerOfTheBarrabTitleDeed(Serial serial) : base(serial)
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
            int version = reader.ReadInt();
        }
    }
}