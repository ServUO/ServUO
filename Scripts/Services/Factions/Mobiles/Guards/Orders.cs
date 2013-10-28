using System;
using System.Collections.Generic;

namespace Server.Factions.AI
{
    public enum ReactionType
    {
        Ignore,
        Warn,
        Attack
    }

    public enum MovementType
    {
        Stand,
        Patrol,
        Follow
    }

    public class Reaction
    {
        private readonly Faction m_Faction;
        private ReactionType m_Type;
        public Reaction(Faction faction, ReactionType type)
        {
            this.m_Faction = faction;
            this.m_Type = type;
        }

        public Reaction(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Faction = Faction.ReadReference(reader);
                        this.m_Type = (ReactionType)reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
        }
        public ReactionType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
            }
        }
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);
            writer.WriteEncodedInt((int)this.m_Type);
        }
    }

    public class Orders
    {
        private readonly BaseFactionGuard m_Guard;
        private readonly List<Reaction> m_Reactions;
        private MovementType m_Movement;
        private Mobile m_Follow;
        public Orders(BaseFactionGuard guard)
        {
            this.m_Guard = guard;
            this.m_Reactions = new List<Reaction>();
            this.m_Movement = MovementType.Patrol;
        }

        public Orders(BaseFactionGuard guard, GenericReader reader)
        {
            this.m_Guard = guard;

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Follow = reader.ReadMobile();
                        goto case 0;
                    }
                case 0:
                    {
                        int count = reader.ReadEncodedInt();
                        this.m_Reactions = new List<Reaction>(count);

                        for (int i = 0; i < count; ++i)
                            this.m_Reactions.Add(new Reaction(reader));

                        this.m_Movement = (MovementType)reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        public BaseFactionGuard Guard
        {
            get
            {
                return this.m_Guard;
            }
        }
        public MovementType Movement
        {
            get
            {
                return this.m_Movement;
            }
            set
            {
                this.m_Movement = value;
            }
        }
        public Mobile Follow
        {
            get
            {
                return this.m_Follow;
            }
            set
            {
                this.m_Follow = value;
            }
        }
        public Reaction GetReaction(Faction faction)
        {
            Reaction reaction;

            for (int i = 0; i < this.m_Reactions.Count; ++i)
            {
                reaction = this.m_Reactions[i];

                if (reaction.Faction == faction)
                    return reaction;
            }

            reaction = new Reaction(faction, (faction == null || faction == this.m_Guard.Faction) ? ReactionType.Ignore : ReactionType.Attack);
            this.m_Reactions.Add(reaction);

            return reaction;
        }

        public void SetReaction(Faction faction, ReactionType type)
        {
            Reaction reaction = this.GetReaction(faction);

            reaction.Type = type;
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)1); // version

            writer.Write((Mobile)this.m_Follow);

            writer.WriteEncodedInt((int)this.m_Reactions.Count);

            for (int i = 0; i < this.m_Reactions.Count; ++i)
                this.m_Reactions[i].Serialize(writer);

            writer.WriteEncodedInt((int)this.m_Movement);
        }
    }
}