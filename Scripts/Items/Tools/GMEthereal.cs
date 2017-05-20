/*
GMEthereal.cs
Version 1.2 [RunUO 2.0]
snicker7
Released: 03/26/06
Updated: 11/08/06
Description:
Single item that can function as any type of mount
currently in the game. Only usable by counselors and
above. The item self-deletes should a player try to use
it.

Use the [props command to change the type of
EtherealMount you would like to use. Also functions
as a functional ethereal seahorse if that option is
selected.

The GMEthereal has no mount time and you will mount
immediately. This does not affect any other Ethereals
in the game.

To install, drop in your custom folder and do:
[add GMEthereal [EtherealType]
Where "EtherealType" is optional and can be any
of the types listed below.
*/

using System;
using CustomsFramework;

namespace Server.Mobiles
{
    public class GMEthereal : EtherealMount
    {
        private static readonly EtherealInfo[] EthyItemTypes = new EtherealInfo[]
        {
            new EtherealInfo(0x20DD, 0x3EAA), //Horse
            new EtherealInfo(0x20F6, 0x3EAB), //Llama
            new EtherealInfo(0x2135, 0x3EAC), //Ostard
            new EtherealInfo(8501, 16035), //DesertOstard
            new EtherealInfo(8502, 16036), //FrenziedOstard
            new EtherealInfo(0x2615, 0x3E9A), //Ridgeback
            new EtherealInfo(0x25CE, 0x3E9B), //Unicorn
            new EtherealInfo(0x260F, 0x3E97), //Beetle
            new EtherealInfo(0x25A0, 0x3E9C), //Kirin
            new EtherealInfo(0x2619, 0x3E98), //SwampDragon
            new EtherealInfo(9751, 16059), //SkeletalMount
            new EtherealInfo(10090, 16020), //Hiryu
            new EtherealInfo(11676, 16018), //ChargerOfTheFallen
            new EtherealInfo(9658, 16051), //SeaHorse
            new EtherealInfo(11669, 16016), //Chimera
            new EtherealInfo(11670, 16017), //CuSidhe
            new EtherealInfo(8417, 16069), //PolarBear
            new EtherealInfo(8403, 16239), //Daemon
            new EtherealInfo(0x46f8, 0x3EC6)
        };
        private EtherealTypes m_EthyType;
        [Constructable]
        public GMEthereal()
            : this(EtherealTypes.Horse)
        {
        }

        [Constructable]
        public GMEthereal(EtherealTypes type)
            : base(0,0,0)
        {
            EthyType = type;
            LootType = LootType.Blessed;
            Hue = 2406;
        }

        public GMEthereal(Serial serial)
            : base(serial)
        {
        }

        public enum EtherealTypes
        {
            Horse,
            Llama,
            Ostard,
            OstardDesert,
            OstardFrenzied,
            Ridgeback,
            Unicorn,
            Beetle,
            Kirin,
            SwampDragon,
            SkeletalHorse,
            Hiryu,
            ChargerOfTheFallen,
            SeaHorse,
            Chimera,
            CuSidhe,
            PolarBear,
            Daemon,
            Boura
        }
        [CommandProperty(AccessLevel.Counselor)]
        public EtherealTypes EthyType
        {
            get
            {
                return m_EthyType;
            }
            set
            {
                if ((int)value > EthyItemTypes.Length)
                    return;
                m_EthyType = value;

                TransparentMountedID = EthyItemTypes[(int)value].MountedID;
                NonTransparentMountedID = TransparentMountedID;
                StatueID = EthyItemTypes[(int)value].RegularID;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "A GM's Ethereal Mount";
            }
        }
        public override int FollowerSlots
        {
            get
            {
                return 0;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (Utilities.IsStaff(from))
            {
                if (from.Mounted)
                    from.SendLocalizedMessage(1005583); // Please dismount first.
                else if (from.HasTrade)
                    from.SendLocalizedMessage(1042317, "", 0x41); // You may not ride at this time
                else if (Multis.DesignContext.Check(from))
                {
                    if (!Deleted && Rider == null && IsChildOf(from.Backpack))
                    {
                        Rider = from;
                        if (MountedID == 16051)
                            Rider.CanSwim = true;
                    }
                }
            }
            else
            {
                from.SendMessage("Players cannot ride  Sorry, BALEETED!");
                Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
            writer.Write((int)m_EthyType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            EthyType = (EtherealTypes)reader.ReadInt();

            if (version == 0)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(1), () => Transparent = false);

                StatueHue = 2406;
            }
        }

        public struct EtherealInfo
        {
            public int RegularID;
            public int MountedID;
            public EtherealInfo(int id, int mid)
            {
                RegularID = id;
                MountedID = mid;
            }
        }
    }

    public class GMEthVirtual : EtherealMount
    {
        public GMEthVirtual(int id, int mid)
            : base(id, mid, 0)
        {
        }

        public GMEthVirtual(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            Delete();
        }
    }
}