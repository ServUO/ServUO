using System;

namespace Server.Items
{
    public enum WaterState
    {
        Dead,
        Dying,
        Unhealthy,
        Healthy,
        Strong
    }

    public enum FoodState
    {
        Dead,
        Starving,
        Hungry,
        Full,
        Overfed
    }

    [PropertyObject]
    public class AquariumState
    {
        private int m_State;
        private int m_Maintain;
        private int m_Improve;
        private int m_Added;
        public AquariumState()
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int State
        {
            get
            {
                return this.m_State;
            }
            set
            {
                this.m_State = value;

                if (this.m_State < 0)
                    this.m_State = 0;

                if (this.m_State > 4)
                    this.m_State = 4;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Maintain
        {
            get
            {
                return this.m_Maintain;
            }
            set
            {
                this.m_Maintain = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Improve
        {
            get
            {
                return this.m_Improve;
            }
            set
            {
                this.m_Improve = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Added
        {
            get
            {
                return this.m_Added;
            }
            set
            {
                this.m_Added = value;
            }
        }
        public override string ToString()
        {
            return "...";
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(this.m_State);
            writer.Write(this.m_Maintain);
            writer.Write(this.m_Improve);
            writer.Write(this.m_Added);
        }

        public virtual void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            this.m_State = reader.ReadInt();
            this.m_Maintain = reader.ReadInt();
            this.m_Improve = reader.ReadInt();
            this.m_Added = reader.ReadInt();
        }
    }
}