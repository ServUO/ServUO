using System;

namespace Server.Items
{
    public class BaseHighseasFish : Item, ICarvable, ICommodity
    {
        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public virtual Item GetCarved => new RawFishSteak();
        public virtual int GetCarvedAmount => 4;

        public override double DefaultWeight => 10.0;

        public BaseHighseasFish(int itemID) : base(itemID)
        {
            Stackable = true;
        }

        public bool Carve(Mobile from, Item item)
        {
            if (Parent is ShippingCrate && !((ShippingCrate)Parent).CheckCarve(this))
                return false;

            Item newItem = GetCarved;

            if (newItem == null)
            {
                newItem = new RawFishSteak();
            }

            if (newItem != null && HasSocket<Caddellite>())
            {
                newItem.AttachSocket(new Caddellite());
            }

            if (newItem != null)
                base.ScissorHelper(from, newItem, GetCarvedAmount);

            return true;
        }

        public static int GetCrabID()
        {
            return Utility.RandomBool() ? 17617 : 17618;
        }

        public static int GetLobsterID()
        {
            return Utility.RandomBool() ? 17619 : 17620;
        }

        public static Type[] DeepWaterFish => m_DeepWaterFish;
        private static readonly Type[] m_DeepWaterFish = new Type[]
        {
            typeof(Haddock),            typeof(CapeCod),            typeof(BlackSeabass),
            typeof(Tarpon),             typeof(RedSnook),           typeof(GraySnapper),
            typeof(Cobia),              typeof(MahiMahi),           typeof(Amberjack),
            typeof(Shad),               typeof(YellowfinTuna),      typeof(Bonito),
            typeof(BlueFish),           typeof(RedGrouper),         typeof(CaptainSnook),
            typeof(Bonefish),           typeof(RedDrum),            typeof(BlueGrouper),
        };


        public static Type[] ShoreFish => m_ShoreFish;
        private static readonly Type[] m_ShoreFish = new Type[]
        {
            typeof(PumpkinSeedSunfish),     typeof(YellowPerch),        typeof(PikeFish),
            typeof(BrookTrout),             typeof(RainbowTrout),       typeof(BluegillSunfish),
            typeof(RedbellyBream),          typeof(SmallmouthBass),     typeof(UncommonShiner),
            typeof(GreenCatfish),           typeof(Walleye),            typeof(KokaneeSalmon),
        };

        public static Type[] DungeonFish => m_DungeonFish;
        private static readonly Type[] m_DungeonFish = new Type[]
        {
            typeof(DungeonChub),        typeof(DemonTrout),         typeof(SnaggletoothBass),
            typeof(CutThroatTrout),     typeof(GrimCisco),          typeof(DrakeFish),
            typeof(OrcBass),            typeof(DarkFish),           typeof(CragSnapper),
            typeof(InfernalTuna),       typeof(TormentedPike),      typeof(LurkerFish),
        };

        public static Type[] LobstersAndCrabs => m_LobstersAndCrabs;
        private static readonly Type[] m_LobstersAndCrabs = new Type[]
        {
            typeof(DungeonessCrab),     typeof(BlueCrab),           typeof(KingCrab),
            typeof(RockCrab),           typeof(SnowCrab),           typeof(AppleCrab),
            typeof(SpineyLobster),      typeof(RockLobster),        typeof(HummerLobster),
            typeof(FredLobster),        typeof(CrustyLobster),      typeof(ShovelNoseLobster),
        };

        public static Type[] Lobsters => m_Lobsters;
        private static readonly Type[] m_Lobsters = new Type[]
        {
            typeof(Lobster),
            typeof(SpineyLobster),      typeof(RockLobster),        typeof(HummerLobster),
            typeof(FredLobster),        typeof(CrustyLobster),      typeof(ShovelNoseLobster),
            typeof(BlueLobster),        typeof(BloodLobster),       typeof(DreadLobster),
            typeof(VoidLobster),
        };

        public static Type[] Crabs => m_Crabs;
        private static readonly Type[] m_Crabs = new Type[]
        {
            typeof(Crab),
            typeof(DungeonessCrab),     typeof(BlueCrab),           typeof(KingCrab),
            typeof(RockCrab),           typeof(SnowCrab),           typeof(AppleCrab),
            typeof(VoidCrab),           typeof(TunnelCrab),         typeof(SpiderCrab),
            typeof(StoneCrab)
        };

        public BaseHighseasFish(Serial serial) : base(serial) { }

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

    public class Amberjack : BaseHighseasFish
    {
        public override int LabelNumber => 1116402;

        [Constructable]
        public Amberjack() : base(17606)
        {
        }

        public Amberjack(Serial serial) : base(serial) { }

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

    public class BlackSeabass : BaseHighseasFish
    {
        public override int LabelNumber => 1116396;

        [Constructable]
        public BlackSeabass()
            : base(2510)
        {
        }

        public BlackSeabass(Serial serial) : base(serial) { }

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

    public class BlueGrouper : BaseHighseasFish
    {
        public override int LabelNumber => 1116411;

        [Constructable]
        public BlueGrouper()
            : base(17158)
        {
        }

        public BlueGrouper(Serial serial) : base(serial) { }

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

    public class BlueFish : BaseHighseasFish
    {
        public override int LabelNumber => 1116406;

        [Constructable]
        public BlueFish()
            : base(2508)
        {
        }

        public BlueFish(Serial serial) : base(serial) { }

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

    public class BluegillSunfish : BaseHighseasFish
    {
        public override int LabelNumber => 1116417;

        [Constructable]
        public BluegillSunfish()
            : base(17158)
        {
        }

        public BluegillSunfish(Serial serial) : base(serial) { }

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

    public class Bonefish : BaseHighseasFish
    {
        public override int LabelNumber => 1116409;

        [Constructable]
        public Bonefish()
            : base(17603)
        {
        }

        public Bonefish(Serial serial) : base(serial) { }

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

    public class Bonito : BaseHighseasFish
    {
        public override int LabelNumber => 1116405;

        [Constructable]
        public Bonito()
            : base(17155)
        {
        }

        public Bonito(Serial serial) : base(serial) { }

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

    public class BrookTrout : BaseHighseasFish
    {
        public override int LabelNumber => 1116415;

        [Constructable]
        public BrookTrout()
            : base(2508)
        {
        }

        public BrookTrout(Serial serial) : base(serial) { }

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

    public class CapeCod : BaseHighseasFish
    {
        public override int LabelNumber => 1116395;

        [Constructable]
        public CapeCod()
            : base(17158)
        {
        }

        public CapeCod(Serial serial) : base(serial) { }

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

    public class CaptainSnook : BaseHighseasFish
    {
        public override int LabelNumber => 1116408;

        [Constructable]
        public CaptainSnook()
            : base(17605)
        {
        }

        public CaptainSnook(Serial serial) : base(serial) { }

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

    public class Cobia : BaseHighseasFish
    {
        public override int LabelNumber => 1116400;

        [Constructable]
        public Cobia()
            : base(17155)
        {
        }

        public Cobia(Serial serial) : base(serial) { }

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

    public class CragSnapper : BaseHighseasFish
    {
        public override int LabelNumber => 1116432;

        [Constructable]
        public CragSnapper()
            : base(17604)
        {
        }

        public CragSnapper(Serial serial) : base(serial) { }

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

    public class CutThroatTrout : BaseHighseasFish
    {
        public override int LabelNumber => 1116427;

        [Constructable]
        public CutThroatTrout()
            : base(17155)
        {
        }

        public CutThroatTrout(Serial serial) : base(serial) { }

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

    public class DarkFish : BaseHighseasFish
    {
        public override int LabelNumber => 1116431;

        [Constructable]
        public DarkFish()
            : base(17159)
        {
        }

        public DarkFish(Serial serial) : base(serial) { }

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

    public class DemonTrout : BaseHighseasFish
    {
        public override int LabelNumber => 1116425;

        [Constructable]
        public DemonTrout()
            : base(17154)
        {
        }

        public DemonTrout(Serial serial) : base(serial) { }

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

    public class DrakeFish : BaseHighseasFish
    {
        public override int LabelNumber => 1116429;

        [Constructable]
        public DrakeFish()
            : base(17605)
        {
        }

        public DrakeFish(Serial serial) : base(serial) { }

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

    public class DungeonChub : BaseHighseasFish
    {
        public override int LabelNumber => 1116424;

        [Constructable]
        public DungeonChub()
            : base(17158)
        {
        }

        public DungeonChub(Serial serial) : base(serial) { }

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

    public class GraySnapper : BaseHighseasFish
    {
        public override int LabelNumber => 1116399;

        [Constructable]
        public GraySnapper()
            : base(17159)
        {
        }

        public GraySnapper(Serial serial) : base(serial) { }

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

    public class GreenCatfish : BaseHighseasFish
    {
        public override int LabelNumber => 1116421;

        [Constructable]
        public GreenCatfish()
            : base(17606)
        {
        }

        public GreenCatfish(Serial serial) : base(serial) { }

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

    public class GrimCisco : BaseHighseasFish
    {
        public override int LabelNumber => 1116428;

        [Constructable]
        public GrimCisco()
            : base(17603)
        {
        }

        public GrimCisco(Serial serial) : base(serial) { }

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

    public class Haddock : BaseHighseasFish
    {
        public override int LabelNumber => 1116394;

        [Constructable]
        public Haddock()
            : base(2508)
        {
        }

        public Haddock(Serial serial) : base(serial) { }

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

    public class InfernalTuna : BaseHighseasFish
    {
        public override int LabelNumber => 1116433;

        [Constructable]
        public InfernalTuna()
            : base(17159)
        {
        }

        public InfernalTuna(Serial serial) : base(serial) { }

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

    public class KokaneeSalmon : BaseHighseasFish
    {
        public override int LabelNumber => 1116423;

        [Constructable]
        public KokaneeSalmon()
            : base(17155)
        {
        }

        public KokaneeSalmon(Serial serial) : base(serial) { }

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

    public class LurkerFish : BaseHighseasFish
    {
        public override int LabelNumber => 1116435;

        [Constructable]
        public LurkerFish()
            : base(2510)
        {
        }

        public LurkerFish(Serial serial) : base(serial) { }

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

    public class MahiMahi : BaseHighseasFish
    {
        public override int LabelNumber => 1116401;

        [Constructable]
        public MahiMahi()
            : base(17606)
        {
        }

        public MahiMahi(Serial serial) : base(serial) { }

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

    public class OrcBass : BaseHighseasFish
    {
        public override int LabelNumber => 1116430;

        [Constructable]
        public OrcBass()
            : base(2508)
        {
        }

        public OrcBass(Serial serial) : base(serial) { }

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

    public class PikeFish : BaseHighseasFish
    {
        public override int LabelNumber => 1116414;

        [Constructable]
        public PikeFish()
            : base(17604)
        {
        }

        public PikeFish(Serial serial) : base(serial) { }

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

    public class PumpkinSeedSunfish : BaseHighseasFish
    {
        public override int LabelNumber => 1116412;

        [Constructable]
        public PumpkinSeedSunfish()
            : base(17159)
        {
        }

        public PumpkinSeedSunfish(Serial serial) : base(serial) { }

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

    public class RainbowTrout : BaseHighseasFish
    {
        public override int LabelNumber => 1116416;

        [Constructable]
        public RainbowTrout()
            : base(17159)
        {
        }

        public RainbowTrout(Serial serial) : base(serial) { }

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

    public class RedDrum : BaseHighseasFish
    {
        public override int LabelNumber => 1116410;

        [Constructable]
        public RedDrum()
            : base(17159)
        {
        }

        public RedDrum(Serial serial) : base(serial) { }

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

    public class RedGrouper : BaseHighseasFish
    {
        public override int LabelNumber => 1116407;

        [Constructable]
        public RedGrouper()
            : base(17159)
        {
        }

        public RedGrouper(Serial serial) : base(serial) { }

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

    public class RedSnook : BaseHighseasFish
    {
        public override int LabelNumber => 1116398;

        [Constructable]
        public RedSnook()
            : base(2509)
        {
        }

        public RedSnook(Serial serial) : base(serial) { }

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

    public class RedbellyBream : BaseHighseasFish
    {
        public override int LabelNumber => 1116418;

        [Constructable]
        public RedbellyBream()
            : base(17159)
        {
        }

        public RedbellyBream(Serial serial) : base(serial) { }

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

    public class Shad : BaseHighseasFish
    {
        public override int LabelNumber => 1116403;

        [Constructable]
        public Shad()
            : base(17159)
        {
        }

        public Shad(Serial serial) : base(serial) { }

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

    public class SmallmouthBass : BaseHighseasFish
    {
        public override int LabelNumber => 1116419;

        [Constructable]
        public SmallmouthBass()
            : base(2509)
        {
        }

        public SmallmouthBass(Serial serial) : base(serial) { }

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

    public class SnaggletoothBass : BaseHighseasFish
    {
        public override int LabelNumber => 1116426;

        [Constructable]
        public SnaggletoothBass()
            : base(2511)
        {
        }

        public SnaggletoothBass(Serial serial) : base(serial) { }

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

    public class Tarpon : BaseHighseasFish
    {
        public override int LabelNumber => 1116397;

        [Constructable]
        public Tarpon()
            : base(17603)
        {
        }

        public Tarpon(Serial serial) : base(serial) { }

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

    public class TormentedPike : BaseHighseasFish
    {
        public override int LabelNumber => 1116434;

        [Constructable]
        public TormentedPike()
            : base(17603)
        {
        }

        public TormentedPike(Serial serial) : base(serial) { }

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

    public class UncommonShiner : BaseHighseasFish
    {
        public override int LabelNumber => 1116420;

        [Constructable]
        public UncommonShiner()
            : base(2510)
        {
        }

        public UncommonShiner(Serial serial) : base(serial) { }

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

    public class Walleye : BaseHighseasFish
    {
        public override int LabelNumber => 1116422;

        [Constructable]
        public Walleye()
            : base(2511)
        {
        }

        public Walleye(Serial serial) : base(serial) { }

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

    public class YellowPerch : BaseHighseasFish
    {
        public override int LabelNumber => 1116413;

        [Constructable]
        public YellowPerch()
            : base(17155)
        {
        }

        public YellowPerch(Serial serial) : base(serial) { }

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

    public class YellowfinTuna : BaseHighseasFish
    {
        public override int LabelNumber => 1116404;

        [Constructable]
        public YellowfinTuna()
            : base(17604)
        {
        }

        public YellowfinTuna(Serial serial) : base(serial) { }

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
