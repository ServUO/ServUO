using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class IntenseTealPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public IntenseTealPigment()
            : base(CompassionPigmentType.IntenseTeal)
        {
        }

        public IntenseTealPigment(Serial serial)
            : base(serial)
        {
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

    public class TyrianPurplePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public TyrianPurplePigment()
            : base(CompassionPigmentType.TyrianPurple)
        {
        }

        public TyrianPurplePigment(Serial serial)
            : base(serial)
        {
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

    public class MottledSunsetBluePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MottledSunsetBluePigment()
            : base(CompassionPigmentType.MottledSunsetBlue)
        {
        }

        public MottledSunsetBluePigment(Serial serial)
            : base(serial)
        {
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

    public class MossyGreenPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MossyGreenPigment()
            : base(CompassionPigmentType.MossyGreen)
        {
        }

        public MossyGreenPigment(Serial serial)
            : base(serial)
        {
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

    public class VibrantOcherPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VibrantOcherPigment()
            : base(CompassionPigmentType.VibrantOcher)
        {
        }

        public VibrantOcherPigment(Serial serial)
            : base(serial)
        {
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

    public class OliveGreenPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public OliveGreenPigment()
            : base(CompassionPigmentType.OliveGreen)
        {
        }

        public OliveGreenPigment(Serial serial)
            : base(serial)
        {
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

    public class PolishedBronzePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public PolishedBronzePigment()
            : base(CompassionPigmentType.PolishedBronze)
        {
        }

        public PolishedBronzePigment(Serial serial)
            : base(serial)
        {
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

    public class GlossyBluePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GlossyBluePigment()
            : base(CompassionPigmentType.GlossyBlue)
        {
        }

        public GlossyBluePigment(Serial serial)
            : base(serial)
        {
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

    public class BlackAndGreenPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public BlackAndGreenPigment()
            : base(CompassionPigmentType.BlackAndGreen)
        {
        }

        public BlackAndGreenPigment(Serial serial)
            : base(serial)
        {
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

    public class DeepVioletPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DeepVioletPigment()
            : base(CompassionPigmentType.DeepViolet)
        {
        }

        public DeepVioletPigment(Serial serial)
            : base(serial)
        {
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

    public class AuraOfAmberPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public AuraOfAmberPigment()
            : base(CompassionPigmentType.AuraOfAmber)
        {
        }

        public AuraOfAmberPigment(Serial serial)
            : base(serial)
        {
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

    public class MurkySeagreenPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MurkySeagreenPigment()
            : base(CompassionPigmentType.MurkySeagreen)
        {
        }

        public MurkySeagreenPigment(Serial serial)
            : base(serial)
        {
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

    public class ShadowyBluePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ShadowyBluePigment()
            : base(CompassionPigmentType.ShadowyBlue)
        {
        }

        public ShadowyBluePigment(Serial serial)
            : base(serial)
        {
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

    public class GleamingFuchsiaPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GleamingFuchsiaPigment()
            : base(CompassionPigmentType.GleamingFuchsia)
        {
        }

        public GleamingFuchsiaPigment(Serial serial)
            : base(serial)
        {
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

    public class GlossyFuchsiaPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public GlossyFuchsiaPigment()
            : base(CompassionPigmentType.GlossyFuchsia)
        {
        }

        public GlossyFuchsiaPigment(Serial serial)
            : base(serial)
        {
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

    public class DeepBluePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DeepBluePigment()
            : base(CompassionPigmentType.DeepBlue)
        {
        }

        public DeepBluePigment(Serial serial)
            : base(serial)
        {
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

    public class VibranSeagreenPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VibranSeagreenPigment()
            : base(CompassionPigmentType.VibranSeagreen)
        {
        }

        public VibranSeagreenPigment(Serial serial)
            : base(serial)
        {
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

    public class MurkyAmberPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MurkyAmberPigment()
            : base(CompassionPigmentType.MurkyAmber)
        {
        }

        public MurkyAmberPigment(Serial serial)
            : base(serial)
        {
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

    public class VibrantCrimsonPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public VibrantCrimsonPigment()
            : base(CompassionPigmentType.VibrantCrimson)
        {
        }

        public VibrantCrimsonPigment(Serial serial)
            : base(serial)
        {
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

    public class ReflectiveShadowPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public ReflectiveShadowPigment()
            : base(CompassionPigmentType.ReflectiveShadow)
        {
        }

        public ReflectiveShadowPigment(Serial serial)
            : base(serial)
        {
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

    public class StarBluePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public StarBluePigment()
            : base(CompassionPigmentType.StarBlue)
        {
        }

        public StarBluePigment(Serial serial)
            : base(serial)
        {
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

    public class MotherOfPearlPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public MotherOfPearlPigment()
            : base(CompassionPigmentType.MotherOfPearl)
        {
        }

        public MotherOfPearlPigment(Serial serial)
            : base(serial)
        {
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

    public class LiquidSunshinePigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public LiquidSunshinePigment()
            : base(CompassionPigmentType.LiquidSunshine)
        {
        }

        public LiquidSunshinePigment(Serial serial)
            : base(serial)
        {
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

    public class DarkVoidPigment : CompassionPigment
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DarkVoidPigment()
            : base(CompassionPigmentType.DarkVoid)
        {
        }

        public DarkVoidPigment(Serial serial)
            : base(serial)
        {
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
