#region Header
// **********
// ServUO - Ethereals.cs
// **********
#endregion

#region References
using System;

using Server.Engines.VeteranRewards;
using Server.Items;
using Server.Multis;
using Server.Spells;
#endregion

namespace Server.Mobiles
{
    public class EtherealMount : Item, IMount, IMountItem, IRewardItem
    {
        public static readonly int DefaultEtherealHue = 0x4001;

        private Mobile m_Rider;
        private bool m_IsRewardItem;

        private bool m_Transparent;

        private int m_TransparentMountedID;
        private int m_NonTransparentMountedID;
        private int m_TransparentMountedHue;
        private int m_NonTransparentMountedHue;

        private int m_StatueID;
        private int m_StatueHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Transparent
        {
            get { return m_Transparent; }
            set
            {
                if (Rider != null)
                {
                    if (value && !m_Transparent)
                    {
                        ItemID = m_TransparentMountedID;
                        Hue = m_TransparentMountedHue;
                    }
                    else if (!value && m_Transparent)
                    {
                        ItemID = m_NonTransparentMountedID;
                        Hue = m_NonTransparentMountedHue;
                    }
                }

                m_Transparent = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TransparentMountedID
        {
            get { return m_TransparentMountedID; }
            set
            {
                if (m_TransparentMountedID != value)
                {
                    m_TransparentMountedID = value;

                    if (m_Rider != null && Transparent)
                    {
                        ItemID = value;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NonTransparentMountedID
        {
            get { return m_NonTransparentMountedID; }
            set
            {
                if (m_NonTransparentMountedID != value)
                {
                    m_NonTransparentMountedID = value;

                    if (m_Rider != null && !Transparent)
                    {
                        ItemID = value;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int TransparentMountedHue
        {
            get { return m_TransparentMountedHue; }
            set
            {
                if (m_TransparentMountedHue != value)
                {
                    m_TransparentMountedHue = value;

                    if (m_Rider != null && Transparent)
                    {
                        Hue = value;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int NonTransparentMountedHue
        {
            get { return m_NonTransparentMountedHue; }
            set
            {
                if (m_NonTransparentMountedHue != value)
                {
                    m_NonTransparentMountedHue = value;

                    if (m_Rider != null && !Transparent)
                    {
                        Hue = value;
                    }

                    InvalidateProperties();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StatueID
        {
            get { return m_StatueID; }
            set
            {
                if (m_StatueID != value)
                {
                    m_StatueID = value;

                    if (m_Rider == null)
                    {
                        ItemID = value;
                    }
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int StatueHue
        {
            get { return m_StatueHue; }
            set
            {
                if (m_StatueHue != value)
                {
                    m_StatueHue = value;

                    if (Rider == null)
                    {
                        Hue = value;
                    }
                }
            }
        }

        public int MountedID
        {
            get
            {
                return Transparent ? TransparentMountedID : NonTransparentMountedID;
            }
        }

        public int MountedHue
        {
            get
            {
                return Transparent ? TransparentMountedHue : NonTransparentMountedHue;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem { get { return m_IsRewardItem; } set { m_IsRewardItem = value; } }

        public override double DefaultWeight { get { return 1.0; } }

        public override bool DisplayLootType { get { return Core.AOS; } }
        public virtual int FollowerSlots { get { return 1; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Rider
        {
            get { return m_Rider; }
            set
            {
                if (value == m_Rider)
                {
                    return;
                }

                if (value == null)
                {
                    Internalize();
                    UnmountMe();

                    RemoveFollowers();
                    m_Rider = null;
                }
                else
                {
                    if (m_Rider != null)
                    {
                        Dismount(m_Rider);
                    }

                    Dismount(value);

                    RemoveFollowers();
                    m_Rider = value;
                    AddFollowers();

                    MountMe();
                }
            }
        }

        public IMount Mount { get { return this; } }

        [Constructable]
        public EtherealMount(int itemID, int transMountedID, int nonTransMountedID, int transHue = 0, int nonTransHue = 0)
            : base(itemID)
        {
            m_Rider = null;
            Layer = Layer.Invalid;
            LootType = LootType.Blessed;

            m_Transparent = true;
            m_TransparentMountedID = transMountedID;
            m_NonTransparentMountedID = nonTransMountedID;
            m_TransparentMountedHue = transHue;
            m_NonTransparentMountedHue = nonTransHue;

            StatueID = itemID;
            StatueHue = 0;
        }

        public EtherealMount(Serial serial)
            : base(serial)
        {
        }

        public static void Dismount(Mobile m)
        {
            IMount mount = m.Mount;

            if (mount != null)
            {
                mount.Rider = null;
            }
        }

        public static void StopMounting(Mobile mob)
        {
            if (mob.Spell is EtherealSpell)
            {
                ((EtherealSpell)mob.Spell).Stop();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Core.ML && m_IsRewardItem)
            {
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { })); // X Year Veteran Reward
            }

            EtherealRetouchingTool.AddProperty(this, list);
        }

        public void RemoveFollowers()
        {
            if (m_Rider != null)
            {
                m_Rider.Followers -= FollowerSlots;
            }

            if (m_Rider != null && m_Rider.Followers < 0)
            {
                m_Rider.Followers = 0;
            }
        }

        public void AddFollowers()
        {
            if (m_Rider != null)
            {
                m_Rider.Followers += FollowerSlots;
            }
        }

        public virtual bool Validate(Mobile from)
        {
            #region SA
            if (from.Race == Race.Gargoyle)
            {
                from.SendLocalizedMessage(1112281); // gargs can't mount
                return false;
            }
            #endregion

            if (Parent == null)
            {
                from.SayTo(from, 1010095); // This must be on your person to use.
                return false;
            }

            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                // CheckIsUsableBy sends the message
                return false;
            }

            if (!BaseMount.CheckMountAllowed(from, true))
            {
                // CheckMountAllowed sends the message
                return false;
            }

            if (from.Mounted)
            {
                from.SendLocalizedMessage(1005583); // Please dismount first.
                return false;
            }

            if (from.IsBodyMod && !from.Body.IsHuman)
            {
                from.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
                return false;
            }

            if (from.HasTrade)
            {
                from.SendLocalizedMessage(1042317, "", 0x41); // You may not ride at this time
                return false;
            }

            if ((from.Followers + FollowerSlots) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049679); // You have too many followers to summon your mount.
                return false;
            }

            if (!DesignContext.Check(from))
            {
                // Check sends the message
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Validate(from))
            {
                new EtherealSpell(this, from).Cast();
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Veteran Reward");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(5); // version

            writer.Write(m_Transparent);

            writer.Write(m_TransparentMountedID);
            writer.Write(m_NonTransparentMountedID);
            writer.Write(m_TransparentMountedHue);
            writer.Write(m_NonTransparentMountedHue);
            writer.Write(m_StatueID);
            writer.Write(m_StatueHue);

            writer.Write(m_IsRewardItem);

            writer.Write(m_Rider);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                    m_Transparent = reader.ReadBool();
                    m_TransparentMountedID = reader.ReadInt();
                    m_NonTransparentMountedID = reader.ReadInt();
                    m_TransparentMountedHue = reader.ReadInt();
                    m_NonTransparentMountedHue = reader.ReadInt();
                    m_StatueID = reader.ReadInt();
                    m_StatueHue = reader.ReadInt();

                    m_IsRewardItem = reader.ReadBool();
                    m_Rider = reader.ReadMobile();
                    break;
                case 4:
                    m_NonTransparentMountedID = reader.ReadInt(); // m_DefaultMountedID = reader.ReadInt();
                    m_NonTransparentMountedHue = reader.ReadInt(); // m_OriginalHue = reader.ReadInt();
                    m_TransparentMountedHue = reader.ReadInt(); // m_EtherealHue = reader.ReadInt();
                    goto case 3;
                case 3:
                    reader.ReadBool();
                    goto case 2;
                case 2:
                    m_IsRewardItem = reader.ReadBool();
                    goto case 0;
                case 1:
                    reader.ReadInt();
                    goto case 0;
                case 0:
                    {
                        m_TransparentMountedID = reader.ReadInt(); // m_MountedID = reader.ReadInt();
                        m_StatueID = reader.ReadInt(); // m_RegularID = reader.ReadInt();
                        m_Rider = reader.ReadMobile();

                        if (m_TransparentMountedID == 0x3EA2)
                        {
                            m_TransparentMountedID = 0x3EAA;
                        }
                    }
                    break;
            }

            AddFollowers();
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            Rider = null; //get off, move to pack

            return DeathMoveResult.RemainEquiped;
        }

        public void UnmountMe()
        {
            Container bp = m_Rider.Backpack;

            ItemID = m_StatueID;
            Layer = Layer.Invalid;

            Movable = true;

            if (Hue != m_StatueHue)
            {
                Hue = m_StatueHue;
            }

            if (bp != null)
            {
                bp.DropItem(this);
                return;
            }

            Point3D loc = m_Rider.Location;
            Map map = m_Rider.Map;

            if (map == null || map == Map.Internal)
            {
                loc = m_Rider.LogoutLocation;
                map = m_Rider.LogoutMap;
            }

            MoveToWorld(loc, map);
        }

        public void MountMe()
        {
            StatueHue = Hue;

            ItemID = MountedID;
            Hue = MountedHue;

            Layer = Layer.Mount;
            Movable = false;

            ProcessDelta();
            m_Rider.ProcessDelta();
            m_Rider.EquipItem(this);
            m_Rider.ProcessDelta();
            ProcessDelta();
        }

        public void OnRiderDamaged(int amount, Mobile from, bool willKill)
        { }

        private class EtherealSpell : Spell
        {
            private static readonly SpellInfo m_Info = new SpellInfo("Ethereal Mount", "", 230);
            private readonly EtherealMount m_Mount;
            private readonly Mobile m_Rider;
            private bool m_Stop;

            public EtherealSpell(EtherealMount mount, Mobile rider)
                : base(rider, null, m_Info)
            {
                m_Rider = rider;
                m_Mount = mount;
            }

            public override bool ClearHandsOnCast { get { return false; } }
            public override bool RevealOnCast { get { return false; } }
            public override double CastDelayFastScalar { get { return 0; } }

            public override TimeSpan CastDelayBase
            {
                get
                {
                    return
                            TimeSpan.FromSeconds(Core.AOS ? 3.0 : 2.0);
                }
            }

            public override TimeSpan GetCastRecovery()
            {
                return TimeSpan.Zero;
            }

            public override int GetMana()
            {
                return 0;
            }

            public override bool ConsumeReagents()
            {
                return true;
            }

            public override bool CheckFizzle()
            {
                return true;
            }

            public void Stop()
            {
                m_Stop = true;
                Disturb(DisturbType.Hurt, false, false);
            }

            public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
            {
                if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest /* || type == DisturbType.Hurt*/)
                {
                    return false;
                }

                return true;
            }

            public override void DoHurtFizzle()
            {
                if (!m_Stop)
                {
                    base.DoHurtFizzle();
                }
            }

            public override void DoFizzle()
            {
                if (!m_Stop)
                {
                    base.DoFizzle();
                }
            }

            public override void OnDisturb(DisturbType type, bool message)
            {
                if (message && !m_Stop)
                {
                    Caster.SendLocalizedMessage(1049455); // You have been disrupted while attempting to summon your ethereal mount!
                }

                //m_Mount.UnmountMe();
            }

            public override void OnCast()
            {
                if (!m_Mount.Deleted && m_Mount.Rider == null && m_Mount.Validate(m_Rider))
                {
                    m_Mount.Rider = m_Rider;
                }

                FinishSequence();
            }
        }
    }

    public class EtherealHorse : EtherealMount
    {
        [Constructable]
        public EtherealHorse()
            : base(0x20DD, 0x3EAA, 0x3EA0)
        { }

        public EtherealHorse(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1041298; } } // Ethereal Horse Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EA0;
                Transparent = true;
            }
        }
    }

    public class EtherealLlama : EtherealMount
    {
        [Constructable]
        public EtherealLlama()
            : base(0x20F6, 0x3EAB, 0x3EA6)
        { }

        public EtherealLlama(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1041300; } } // Ethereal Llama Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EA6;
                Transparent = true;
            }
        }
    }

    public class EtherealOstard : EtherealMount
    {
        [Constructable]
        public EtherealOstard()
            : base(0x2135, 0x3EAC, 0x3EA5)
        { }

        public EtherealOstard(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1041299; } } // Ethereal Ostard Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EA5;
                Transparent = true;
            }
        }
    }

    public class EtherealRidgeback : EtherealMount
    {
        [Constructable]
        public EtherealRidgeback()
            : base(0x2615, 0x3E9A, 0x3EBA, DefaultEtherealHue)
        { }

        public EtherealRidgeback(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1049747; } } // Ethereal Ridgeback Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EBA;
                Transparent = true;
            }

            if (version == 1)
            {
                TransparentMountedHue = DefaultEtherealHue;
            }
        }
    }

    public class EtherealUnicorn : EtherealMount
    {
        [Constructable]
        public EtherealUnicorn()
            : base(0x25CE, 0x3E9B, 0x3EB4, DefaultEtherealHue)
        { }

        public EtherealUnicorn(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1049745; } } // Ethereal Unicorn Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EB4;
                Transparent = true;
            }

            if (version == 1)
            {
                TransparentMountedHue = DefaultEtherealHue;
            }
        }
    }

    public class EtherealBeetle : EtherealMount
    {
        [Constructable]
        public EtherealBeetle()
            : base(0x260F, 0x3E97, 0x3EBC, DefaultEtherealHue)
        { }

        public EtherealBeetle(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1049748; } } // Ethereal Beetle Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EBC;
                Transparent = true;
            }

            if (version == 1)
            {
                TransparentMountedHue = DefaultEtherealHue;
            }
        }
    }

    public class EtherealKirin : EtherealMount
    {
        [Constructable]
        public EtherealKirin()
            : base(0x25A0, 0x3E9C, 0x3EAD, DefaultEtherealHue)
        { }

        public EtherealKirin(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1049746; } } // Ethereal Ki-Rin Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EAD;
                Transparent = true;
            }

            if (version == 1)
            {
                TransparentMountedHue = DefaultEtherealHue;
            }
        }
    }

    public class EtherealSwampDragon : EtherealMount
    {
        [Constructable]
        public EtherealSwampDragon()
            : base(0x2619, 0x3E98, 0x3EBD, DefaultEtherealHue, 0x851)
        { }

        public EtherealSwampDragon(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1049749; } } // Ethereal Swamp Dragon Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                NonTransparentMountedID = 0x3EBD;
                NonTransparentMountedHue = 0x851;
                Transparent = true;
            }
        }
    }

    public class RideablePolarBear : EtherealMount
    {
        [Constructable]
        public RideablePolarBear()
            : base(0x20E1, 0x3EC5, 0x3EC5, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public RideablePolarBear(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1076159; } } // Rideable Polar Bear

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 1)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealCuSidhe : EtherealMount
    {
        [Constructable]
        public EtherealCuSidhe()
            : base(0x2D96, 0x3E91, 0x3E91, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealCuSidhe(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1080386; } } // Ethereal Cu Sidhe Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealHiryu : EtherealMount
    {
        [Constructable]
        public EtherealHiryu()
            : base(0x276A, 0x3E94, 0x3E94, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealHiryu(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1113813; } } // Ethereal Hiryu Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealReptalon : EtherealMount
    {
        [Constructable]
        public EtherealReptalon()
            : base(0x2d95, 0x3e90, 0x3e90, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealReptalon(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1113812; } } // Ethereal Reptalon Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class ChargerOfTheFallen : EtherealMount
    {
        [Constructable]
        public ChargerOfTheFallen()
            : base(0x2D9C, 0x3E92, 0x3E92, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public ChargerOfTheFallen(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1074816; } } // Charger of the Fallen Statuette

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealBoura : EtherealMount
    {
        public override int LabelNumber { get { return 1150006; } } // Rideable Boura Statuette

        [Constructable]
        public EtherealBoura()
            : base(0x46F8, 0x3EC6, 0x3EC6, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealBoura(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 1)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealAncientHellHound : EtherealMount
    {
        public override int LabelNumber { get { return 1155723; } } // Ancient Hell Hound Statuette

        [Constructable]
        public EtherealAncientHellHound()
            : base(0x3FFD, 0x3EC9, 0x3EC9, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealAncientHellHound(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 1)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealTiger : EtherealMount
    {
        [Constructable]
        public EtherealTiger()
            : this(false)
        {
        }

        [Constructable]
        public EtherealTiger(bool light = false)
            : base(0x9844, light ? 0x3EC7 : 0x3EC8, light ? 0x3EC7 : 0x3EC8, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealTiger(Serial serial)
            : base(serial)
        { }

        public override int LabelNumber { get { return 1154589; } } // Ethereal Tiger Statuette

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealTarantula : EtherealMount
    {
        public override int LabelNumber { get { return 1157081; } } // Tarantula Statuette

        [Constructable]
        public EtherealTarantula()
            : base(0x9DD6, 0x3ECA, 0x3ECA, DefaultEtherealHue)
        {
            Transparent = false;
        }


        public EtherealTarantula(Serial serial)
            : base(serial)
        {
        }

        public override bool Validate(Mobile from)
        {
            #region TOL
            if (from.NetState != null && !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1156139); // * You must upgrade to the Time of Legends in order to use this. *                               
                return false;
            }
            #endregion

            return base.Validate(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealLasher : EtherealMount
    {
        public override int LabelNumber { get { return 1157214; } } // Lasher

        [Constructable]
        public EtherealLasher()
            : base(0x9E35, 0x3ECB, 0x3ECB, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealLasher(Serial serial)
            : base(serial)
        {
        }

        public override bool Validate(Mobile from)
        {
            #region TOL
            if (from.NetState != null && !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1156139); // * You must upgrade to the Time of Legends in order to use this. *                               
                return false;
            }
            #endregion

            return base.Validate(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }

    public class EtherealSerpentineDragon : EtherealMount
    {
        public override int LabelNumber { get { return 1157995; } } // Ethereal Dragon Statuette

        [Constructable]
        public EtherealSerpentineDragon()
            : base(0xA010, 0x3ECE, 0x3ECE, DefaultEtherealHue)
        {
            Transparent = false;
        }

        public EtherealSerpentineDragon(Serial serial)
            : base(serial)
        {
        }

        public override bool Validate(Mobile from)
        {
            #region TOL
            if (from.NetState != null && !from.NetState.SupportsExpansion(Expansion.ML))
            {
                from.SendLocalizedMessage(1156139); // * You must upgrade to the Time of Legends in order to use this. *                               
                return false;
            }
            #endregion

            return base.Validate(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                Transparent = false;
            }
        }
    }
}
