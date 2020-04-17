using Server.Network;

namespace Server.Items
{
    public class VesperCollectionStatuette : BaseStatuette
    {
        public override bool IsArtifact => true;
        public VesperCollectionStatuette(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public VesperCollectionStatuette(Serial serial)
            : base(serial)
        {
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (TurnedOn && IsLockedDown && (!m.Hidden || m.IsPlayer()) && Utility.InRange(m.Location, Location, 2) && !Utility.InRange(oldLocation, Location, 2))
            {
                int cliloc = Utility.RandomMinMax(1073266, 1073286);

                if (cliloc == 1073282)
                    cliloc -= 1;

                PublicOverheadMessage(MessageType.Regular, 0x3B2, cliloc);
                Effects.PlaySound(Location, Map, Utility.Random(0x17));
            }

            base.OnMovement(m, oldLocation);
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

    public class TrollStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public TrollStatuette()
            : base(0x20E9)
        {
        }

        public TrollStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073242;// G'Thunk the Troll - Museum of Vesper Replica
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

    public class CrystalBallStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public CrystalBallStatuette()
            : base(0xE2E)
        {
        }

        public CrystalBallStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073244;// Nystul's Crystal Ball - Museum of Vesper Replica
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

    public class DevourerStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public DevourerStatuette()
            : base(0x2623)
        {
        }

        public DevourerStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073245;// Dangerous Creatures Replica: Devourer of Souls - Museum of Vesper
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

    public class SnowLadyStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public SnowLadyStatuette()
            : base(0x276C)
        {
        }

        public SnowLadyStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075016;// Dangerous Creatures Replica: Lady of the Snow - Museum of Vesper
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

    public class GolemStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public GolemStatuette()
            : base(0x2610)
        {
        }

        public GolemStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075017;// Dangerous Creatures Replica: Golem - Museum of Vesper
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

    public class ExodusOverseerStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public ExodusOverseerStatuette()
            : base(0x260C)
        {
        }

        public ExodusOverseerStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075018;// Dangerous Creatures Replica: Exodus Overseer - Museum of Vesper
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

    public class JukaLordStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public JukaLordStatuette()
            : base(0x25FC)
        {
        }

        public JukaLordStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075019;// Dangerous Creatures Replica: Juka Lord- Museum of Vesper
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

    public class MeerCaptainStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public MeerCaptainStatuette()
            : base(0x25FA)
        {
        }

        public MeerCaptainStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075020;// Dangerous Creatures Replica: Meer Captain - Museum of Vesper
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

    public class MeerEternalStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public MeerEternalStatuette()
            : base(0x25F8)
        {
        }

        public MeerEternalStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075021;// Dangerous Creatures Replica: Meer Eternal - Museum of Vesper
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

    public class SolenQueenStatuette : VesperCollectionStatuette
    {
        [Constructable]
        public SolenQueenStatuette()
            : base(0x2602)
        {
        }

        public SolenQueenStatuette(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075022;// Dangerous Creatures Replica: Solen Queen - Museum of Vesper
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

    public class VesperSpecialAchievementReplica : VesperCollectionStatuette
    {
        [Constructable]
        public VesperSpecialAchievementReplica()
            : base(0x2D4E)
        {
        }

        public VesperSpecialAchievementReplica(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073265;// Museum of Vesper Special Achievement Replica
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