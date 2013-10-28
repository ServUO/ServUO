using System;
using Server.Engines.CannedEvil;

namespace Server.Items
{
    public class ChampionSkull : Item
    {
        private ChampionSkullType m_Type;
        [Constructable]
        public ChampionSkull(ChampionSkullType type)
            : base(0x1AE1)
        {
            this.m_Type = type;
            this.LootType = LootType.Cursed;

            // TODO: All hue values
            switch ( type )
            {
                case ChampionSkullType.Power:
                    this.Hue = 0x159;
                    break;
                case ChampionSkullType.Venom:
                    this.Hue = 0x172;
                    break;
                case ChampionSkullType.Greed:
                    this.Hue = 0x1EE;
                    break;
                case ChampionSkullType.Death:
                    this.Hue = 0x025;
                    break;
                case ChampionSkullType.Pain:
                    this.Hue = 0x035;
                    break;
                case ChampionSkullType.Terror:
                    this.Hue = 0x4F2;
                    break;
                case ChampionSkullType.Infuse:
                    this.Hue = 0x550;
                    break;
            }
        }

        public ChampionSkull(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1049479 + (int)this.m_Type;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        this.m_Type = (ChampionSkullType)reader.ReadInt();

                        break;
                    }
            }
			
            if (version == 0)
            {
                if (this.LootType != LootType.Cursed)
                    this.LootType = LootType.Cursed;
	
                if (this.Insured)
                    this.Insured = false;
            }
        }
    }
}