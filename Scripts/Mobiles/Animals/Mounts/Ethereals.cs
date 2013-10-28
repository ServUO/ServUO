using System;
using Server.Engines.VeteranRewards;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    public class EtherealMount : Item, IMount, IMountItem, Engines.VeteranRewards.IRewardItem
    {
        private int m_MountedID;
        private int m_RegularID;
        private Mobile m_Rider;
        private bool m_IsRewardItem;
        private bool m_IsDonationItem;
        [Constructable]
        public EtherealMount(int itemID, int mountID)
            : base(itemID)
        {
            this.m_MountedID = mountID;
            this.m_RegularID = itemID;
            this.m_Rider = null;

            this.Layer = Layer.Invalid;

            this.LootType = LootType.Blessed;
        }

        public EtherealMount(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
            }
        }
        public override double DefaultWeight
        {
            get
            {
                return 1.0;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public bool IsDonationItem
        {
            get
            {
                return this.m_IsDonationItem;
            }
            set
            {
                this.m_IsDonationItem = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MountedID
        {
            get
            {
                return this.m_MountedID;
            }
            set
            {
                if (this.m_MountedID != value)
                {
                    this.m_MountedID = value;

                    if (this.m_Rider != null)
                        this.ItemID = value;
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RegularID
        {
            get
            {
                return this.m_RegularID;
            }
            set
            {
                if (this.m_RegularID != value)
                {
                    this.m_RegularID = value;

                    if (this.m_Rider == null)
                        this.ItemID = value;
                }
            }
        }
        public override bool DisplayLootType
        {
            get
            {
                return false;
            }
        }
        public virtual int FollowerSlots
        {
            get
            {
                return 1;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Rider
        {
            get
            {
                return this.m_Rider;
            }
            set
            {
                if (value != this.m_Rider)
                {
                    if (value == null)
                    {
                        this.Internalize();
                        this.UnmountMe();

                        this.RemoveFollowers();
                        this.m_Rider = value;
                    }
                    else
                    {
                        if (this.m_Rider != null)
                            Dismount(this.m_Rider);

                        Dismount(value);

                        this.RemoveFollowers();
                        this.m_Rider = value;
                        this.AddFollowers();

                        this.MountMe();
                    }
                }
            }
        }
        public virtual int EtherealHue
        {
            get
            {
                return 0x4001;
            }
        }
        public IMount Mount
        {
            get
            {
                return this;
            }
        }
        public static void Dismount(Mobile m)
        {
            IMount mount = m.Mount;

            if (mount != null)
                mount.Rider = null;
        }

        public static void StopMounting(Mobile mob)
        {
            if (mob.Spell is EtherealSpell)
                ((EtherealSpell)mob.Spell).Stop();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_IsDonationItem)
            {
                list.Add("Donation Ethereal");
                list.Add("7.5 sec slower cast time if not a 9mo. Veteran");
            }
            if (Core.ML && this.m_IsRewardItem)
                list.Add(RewardSystem.GetRewardYearLabel(this, new object[] { })); // X Year Veteran Reward
        }

        public void RemoveFollowers()
        {
            if (this.m_Rider != null)
                this.m_Rider.Followers -= this.FollowerSlots;

            if (this.m_Rider != null && this.m_Rider.Followers < 0)
                this.m_Rider.Followers = 0;
        }

        public void AddFollowers()
        {
            if (this.m_Rider != null)
                this.m_Rider.Followers += this.FollowerSlots;
        }

        public virtual bool Validate(Mobile from)
        {
            if (this.Parent == null)
            {
                from.SayTo(from, 1010095); // This must be on your person to use.
                return false;
            }
            else if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
            {
                // CheckIsUsableBy sends the message
                return false;
            }
            else if (!BaseMount.CheckMountAllowed(from, true))
            {
                // CheckMountAllowed sends the message
                return false;
            }
            else if (from.Mounted)
            {
                from.SendLocalizedMessage(1005583); // Please dismount first.
                return false;
            }
            else if (from.IsBodyMod && !from.Body.IsHuman)
            {
                from.SendLocalizedMessage(1061628); // You can't do that while polymorphed.
                return false;
            }
            else if (from.HasTrade)
            {
                from.SendLocalizedMessage(1042317, "", 0x41); // You may not ride at this time
                return false;
            }
            else if ((from.Followers + this.FollowerSlots) > from.FollowersMax)
            {
                from.SendLocalizedMessage(1049679); // You have too many followers to summon your mount.
                return false;
            }
            else if (!Multis.DesignContext.Check(from))
            {
                // Check sends the message
                return false;
            }

            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.Validate(from))
                new EtherealSpell(this, from).Cast();
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.m_IsDonationItem)
                this.LabelTo(from, "Donation Ethereal");
            else
                this.LabelTo(from, "Veteran Reward");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)3); // version

            writer.Write(this.m_IsDonationItem);
            writer.Write(this.m_IsRewardItem);

            writer.Write((int)this.m_MountedID);
            writer.Write((int)this.m_RegularID);
            writer.Write(this.m_Rider);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            this.LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch( version )
            {
                case 3:
                    {
                        this.m_IsDonationItem = reader.ReadBool();
                        goto case 2;
                    }
                case 2:
                    {
                        this.m_IsRewardItem = reader.ReadBool();
                        goto case 0;
                    }
                case 1:
                    reader.ReadInt();
                    goto case 0;
                case 0:
                    {
                        this.m_MountedID = reader.ReadInt();
                        this.m_RegularID = reader.ReadInt();
                        this.m_Rider = reader.ReadMobile();

                        if (this.m_MountedID == 0x3EA2)
                            this.m_MountedID = 0x3EAA;

                        break;
                    }
            }

            this.AddFollowers();

            if (version < 3 && this.Weight == 0)
                this.Weight = -1;
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            this.Rider = null;//get off, move to pack

            return DeathMoveResult.RemainEquiped;
        }

        public void UnmountMe()
        {
            Container bp = this.m_Rider.Backpack;

            this.ItemID = this.m_RegularID;
            this.Layer = Layer.Invalid;
            this.Movable = true;

            if (this.Hue == this.EtherealHue)
                this.Hue = 0;

            if (bp != null)
            {
                bp.DropItem(this);
            }
            else
            {
                Point3D loc = this.m_Rider.Location;
                Map map = this.m_Rider.Map;

                if (map == null || map == Map.Internal)
                {
                    loc = this.m_Rider.LogoutLocation;
                    map = this.m_Rider.LogoutMap;
                }

                this.MoveToWorld(loc, map);
            }
        }

        public void MountMe()
        {
            this.ItemID = this.m_MountedID;
            this.Layer = Layer.Mount;
            this.Movable = false;

            if (this.Hue == 0)
                this.Hue = this.EtherealHue;

            this.ProcessDelta();
            this.m_Rider.ProcessDelta();
            this.m_Rider.EquipItem(this);
            this.m_Rider.ProcessDelta();
            this.ProcessDelta();
        }

        public void OnRiderDamaged(int amount, Mobile from, bool willKill)
        {
        }

        private class EtherealSpell : Spell
        {
            private static readonly SpellInfo m_Info = new SpellInfo("Ethereal Mount", "", 230);
            private readonly EtherealMount m_Mount;
            private readonly Mobile m_Rider;
            private bool m_Stop;
            public EtherealSpell(EtherealMount mount, Mobile rider)
                : base(rider, null, m_Info)
            {
                this.m_Rider = rider;
                this.m_Mount = mount;
            }

            public override bool ClearHandsOnCast
            {
                get
                {
                    return false;
                }
            }
            public override bool RevealOnCast
            {
                get
                {
                    return false;
                }
            }
            public override double CastDelayFastScalar
            {
                get
                {
                    return 0;
                }
            }
            public override TimeSpan CastDelayBase
            {
                get
                {
                    return TimeSpan.FromSeconds(((this.m_Mount.IsDonationItem && RewardSystem.GetRewardLevel(this.m_Rider) < 3) ? (7.5 + (Core.AOS ? 3.0 : 2.0)) : (Core.AOS ? 3.0 : 2.0)));
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
                this.m_Stop = true;
                this.Disturb(DisturbType.Hurt, false, false);
            }

            public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
            {
                if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/)
                    return false;

                return true;
            }

            public override void DoHurtFizzle()
            {
                if (!this.m_Stop)
                    base.DoHurtFizzle();
            }

            public override void DoFizzle()
            {
                if (!this.m_Stop)
                    base.DoFizzle();
            }

            public override void OnDisturb(DisturbType type, bool message)
            {
                if (message && !this.m_Stop)
                    this.Caster.SendLocalizedMessage(1049455); // You have been disrupted while attempting to summon your ethereal mount!
                //m_Mount.UnmountMe();
            }

            public override void OnCast()
            {
                if (!this.m_Mount.Deleted && this.m_Mount.Rider == null && this.m_Mount.Validate(this.m_Rider))
                    this.m_Mount.Rider = this.m_Rider;

                this.FinishSequence();
            }
        }
    }

    public class EtherealHorse : EtherealMount
    {
        [Constructable]
        public EtherealHorse()
            : base(0x20DD, 0x3EAA)
        {
        }

        public EtherealHorse(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041298;
            }
        }// Ethereal Horse Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal horse")
                this.Name = null;

            if (this.ItemID == 0x2124)
                this.ItemID = 0x20DD;
        }
    }

    public class EtherealLlama : EtherealMount
    {
        [Constructable]
        public EtherealLlama()
            : base(0x20F6, 0x3EAB)
        {
        }

        public EtherealLlama(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041300;
            }
        }// Ethereal Llama Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal llama")
                this.Name = null;
        }
    }

    public class EtherealOstard : EtherealMount
    {
        [Constructable]
        public EtherealOstard()
            : base(0x2135, 0x3EAC)
        {
        }

        public EtherealOstard(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041299;
            }
        }// Ethereal Ostard Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal ostard")
                this.Name = null;
        }
    }

    public class EtherealRidgeback : EtherealMount
    {
        [Constructable]
        public EtherealRidgeback()
            : base(0x2615, 0x3E9A)
        {
        }

        public EtherealRidgeback(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049747;
            }
        }// Ethereal Ridgeback Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal ridgeback")
                this.Name = null;
        }
    }

    public class EtherealUnicorn : EtherealMount
    {
        [Constructable]
        public EtherealUnicorn()
            : base(0x25CE, 0x3E9B)
        {
        }

        public EtherealUnicorn(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049745;
            }
        }// Ethereal Unicorn Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal unicorn")
                this.Name = null;
        }
    }

    public class EtherealBeetle : EtherealMount
    {
        [Constructable]
        public EtherealBeetle()
            : base(0x260F, 0x3E97)
        {
        }

        public EtherealBeetle(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049748;
            }
        }// Ethereal Beetle Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal beetle")
                this.Name = null;
        }
    }

    public class EtherealKirin : EtherealMount
    {
        [Constructable]
        public EtherealKirin()
            : base(0x25A0, 0x3E9C)
        {
        }

        public EtherealKirin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049746;
            }
        }// Ethereal Ki-Rin Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal kirin")
                this.Name = null;
        }
    }

    public class EtherealSwampDragon : EtherealMount
    {
        [Constructable]
        public EtherealSwampDragon()
            : base(0x2619, 0x3E98)
        {
        }

        public EtherealSwampDragon(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049749;
            }
        }// Ethereal Swamp Dragon Statuette
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Name == "an ethereal swamp dragon")
                this.Name = null;
        }
    }

    public class RideablePolarBear : EtherealMount
    {
        [Constructable]
        public RideablePolarBear()
            : base(0x20E1, 0x3EC5)
        {
        }

        public RideablePolarBear(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1076159;
            }
        }// Rideable Polar Bear 
        public override int EtherealHue
        {
            get
            {
                return 0;
            }
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

    public class EtherealCuSidhe : EtherealMount
    {
        [Constructable]
        public EtherealCuSidhe()
            : base(0x2D96, 0x3E91)
        {
        }

        public EtherealCuSidhe(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1080386;
            }
        }// Ethereal Cu Sidhe Statuette
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

    public class EtherealHiryu : EtherealMount
    {
        [Constructable]
        public EtherealHiryu()
            : base(0x276A, 0x3E94)
        {
        }

        public EtherealHiryu(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113813;
            }
        }// Ethereal Hiryu Statuette
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

    public class EtherealReptalon : EtherealMount
    {
        [Constructable]
        public EtherealReptalon()
            : base(0x2d95, 0x3e90)
        {
        }

        public EtherealReptalon(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113812;
            }
        }// Ethereal Reptalon Statuette
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

    public class ChargerOfTheFallen : EtherealMount
    {
        [Constructable]
        public ChargerOfTheFallen()
            : base(0x2D9C, 0x3E92)
        {
        }

        public ChargerOfTheFallen(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074816;
            }
        }// Charger of the Fallen Statuette
        public override int EtherealHue
        {
            get
            {
                return 0;
            }
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

            if (version <= 1 && this.Hue != 0)
            {
                this.Hue = 0;
            }
        }
    }
}