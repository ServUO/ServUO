using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Engines.CannedEvil
{
    public class ChampionSkullBrazier : AddonComponent
    {
        private ChampionSkullPlatform m_Platform;
        private ChampionSkullType m_Type;
        private Item m_Skull;
        public ChampionSkullBrazier(ChampionSkullPlatform platform, ChampionSkullType type)
            : base(0x19BB)
        {
            this.Hue = 0x455;
            this.Light = LightType.Circle300;

            this.m_Platform = platform;
            this.m_Type = type;
        }

        public ChampionSkullBrazier(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSkullPlatform Platform
        {
            get
            {
                return this.m_Platform;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSkullType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Item Skull
        {
            get
            {
                return this.m_Skull;
            }
            set
            {
                this.m_Skull = value;
                if (this.m_Platform != null)
                    this.m_Platform.Validate();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1049489 + (int)this.m_Type;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Platform != null)
                this.m_Platform.Validate();

            this.BeginSacrifice(from);
        }

        public void BeginSacrifice(Mobile from)
        {
            if (this.Deleted)
                return;

            if (this.m_Skull != null && this.m_Skull.Deleted)
                this.Skull = null;

            if (from.Map != this.Map || !from.InRange(this.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (!Harrower.CanSpawn)
            {
                from.SendMessage("The harrower has already been spawned.");
            }
            else if (this.m_Skull == null)
            {
                from.SendLocalizedMessage(1049485); // What would you like to sacrifice?
                from.Target = new SacrificeTarget(this);
            }
            else
            {
                this.SendLocalizedMessageTo(from, 1049487, ""); // I already have my champions awakening skull!
            }
        }

        public void EndSacrifice(Mobile from, ChampionSkull skull)
        {
            if (this.Deleted)
                return;

            if (this.m_Skull != null && this.m_Skull.Deleted)
                this.Skull = null;

            if (from.Map != this.Map || !from.InRange(this.GetWorldLocation(), 3))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (!Harrower.CanSpawn)
            {
                from.SendMessage("The harrower has already been spawned.");
            }
            else if (skull == null)
            {
                this.SendLocalizedMessageTo(from, 1049488, ""); // That is not my champions awakening skull!
            }
            else if (this.m_Skull != null)
            {
                this.SendLocalizedMessageTo(from, 1049487, ""); // I already have my champions awakening skull!
            }
            else if (!skull.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1049486); // You can only sacrifice items that are in your backpack!
            }
            else
            {
                if (skull.Type == this.Type)
                {
                    skull.Movable = false;
                    skull.MoveToWorld(this.GetWorldTop(), this.Map);

                    this.Skull = skull;
                }
                else
                {
                    this.SendLocalizedMessageTo(from, 1049488, ""); // That is not my champions awakening skull!
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)this.m_Type);
            writer.Write(this.m_Platform);
            writer.Write(this.m_Skull);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Type = (ChampionSkullType)reader.ReadInt();
                        this.m_Platform = reader.ReadItem() as ChampionSkullPlatform;
                        this.m_Skull = reader.ReadItem();

                        if (this.m_Platform == null)
                            this.Delete();

                        break;
                    }
            }

            if (this.Hue == 0x497)
                this.Hue = 0x455;

            if (this.Light != LightType.Circle300)
                this.Light = LightType.Circle300;
        }

        private class SacrificeTarget : Target
        {
            private readonly ChampionSkullBrazier m_Brazier;
            public SacrificeTarget(ChampionSkullBrazier brazier)
                : base(12, false, TargetFlags.None)
            {
                this.m_Brazier = brazier;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                this.m_Brazier.EndSacrifice(from, targeted as ChampionSkull);
            }
        }
    }
}