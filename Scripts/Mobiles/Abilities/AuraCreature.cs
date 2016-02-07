// Created by Peoharen
using System;

//using Server.Spells.BlueMagic;
namespace Server.Mobiles
{
    public class AuraCreature : BaseCreature
    {
        public DateTime m_AuraDelay;
        private int m_MinAuraDelay;
        private int m_MaxAuraDelay;
        private int m_MinAuraDamage;
        private int m_MaxAuraDamage;
        private int m_AuraRange;
        private ResistanceType m_AuraType = ResistanceType.Physical;
        private Poison m_AuraPoison = null;
        private string m_AuraMessage = "";

        #region publicprops
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinAuraDelay
        {
            get
            {
                return this.m_MinAuraDelay;
            }
            set
            {
                this.m_MinAuraDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxAuraDelay
        {
            get
            {
                return this.m_MaxAuraDelay;
            }
            set
            {
                this.m_MaxAuraDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinAuraDamage
        {
            get
            {
                return this.m_MinAuraDamage;
            }
            set
            {
                this.m_MinAuraDamage = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxAuraDamage
        {
            get
            {
                return this.m_MaxAuraDamage;
            }
            set
            {
                this.m_MaxAuraDamage = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public new int AuraRange
        {
            get
            {
                return this.m_AuraRange;
            }
            set
            {
                this.m_AuraRange = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ResistanceType AuraType
        {
            get
            {
                return this.m_AuraType;
            }
            set
            {
                this.m_AuraType = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Poison AuraPoison
        {
            get
            {
                return this.m_AuraPoison;
            }
            set
            {
                this.m_AuraPoison = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string AuraMessage
        {
            get
            {
                return this.m_AuraMessage;
            }
            set
            {
                this.m_AuraMessage = value;
            }
        }
        #endregion

        public AuraCreature(AIType aitype, FightMode fightmode, int spot, int meleerange, double passivespeed, double activespeed)
            : base(aitype, fightmode, spot, meleerange, passivespeed, activespeed)
        {
            this.m_AuraDelay = DateTime.UtcNow;
            /*
            Default is ?
            AuraMessage = "The intense cold is damaging you!";
            AuraType = ResistanceType.Fire;
            MinAuraDelay = 5;
            MaxAuraDelay = 15;
            MinAuraDamage = 15;
            MaxAuraDamage = 25;
            AuraRange = 3;
            */
        }

        public override void OnThink()
        {
            if (DateTime.UtcNow > this.m_AuraDelay)
            {
                this.DebugSay("Auraing");
                Ability.Aura(this, this.m_MinAuraDamage, this.m_MaxAuraDamage, this.m_AuraType, this.m_AuraRange, this.m_AuraPoison, this.m_AuraMessage);

                this.m_AuraDelay = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(this.m_MinAuraDelay, this.m_MaxAuraDelay));
            }

            base.OnThink();
        }

        public AuraCreature(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(this.m_MinAuraDelay);
            writer.Write(this.m_MaxAuraDelay);
            writer.Write(this.m_MinAuraDamage);
            writer.Write(this.m_MaxAuraDamage);
            writer.Write(this.m_AuraRange);
            writer.Write((int)this.m_AuraType);
            Poison.Serialize(this.m_AuraPoison, writer);
            writer.Write(this.m_AuraMessage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            this.m_MinAuraDelay = reader.ReadInt();
            this.m_MaxAuraDelay = reader.ReadInt();
            this.m_MinAuraDamage = reader.ReadInt();
            this.m_MaxAuraDamage = reader.ReadInt();
            this.m_AuraRange = reader.ReadInt();
            this.m_AuraType = (ResistanceType)reader.ReadInt();
            this.m_AuraPoison = Poison.Deserialize(reader);
            this.m_AuraMessage = reader.ReadString();
            this.m_AuraDelay = DateTime.UtcNow;
        }
    }
}