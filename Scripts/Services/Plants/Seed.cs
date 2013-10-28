using System;
using System.Text;
using Server.Targeting;

namespace Server.Engines.Plants
{
    public class Seed : Item
    {
        private PlantType m_PlantType;
        private PlantHue m_PlantHue;
        private bool m_ShowType;
        [Constructable]
        public Seed()
            : this(PlantTypeInfo.RandomFirstGeneration(), PlantHueInfo.RandomFirstGeneration(), false)
        {
        }

        [Constructable]
        public Seed(PlantType plantType, PlantHue plantHue, bool showType)
            : base(0xDCF)
        {
            this.Weight = 1.0;
            this.Stackable = Core.SA;

            this.m_PlantType = plantType;
            this.m_PlantHue = plantHue;
            this.m_ShowType = showType;

            this.Hue = PlantHueInfo.GetInfo(plantHue).Hue;
        }

        public Seed(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantType PlantType
        {
            get
            {
                return this.m_PlantType;
            }
            set
            {
                this.m_PlantType = value;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue
        {
            get
            {
                return this.m_PlantHue;
            }
            set
            {
                this.m_PlantHue = value;
                this.Hue = PlantHueInfo.GetInfo(value).Hue;
                this.InvalidateProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowType
        {
            get
            {
                return this.m_ShowType;
            }
            set
            {
                this.m_ShowType = value;
                this.InvalidateProperties();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1060810;
            }
        }// seed
        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public static Seed RandomBonsaiSeed()
        {
            return RandomBonsaiSeed(0.5);
        }

        public static Seed RandomBonsaiSeed(double increaseRatio)
        {
            return new Seed(PlantTypeInfo.RandomBonsai(increaseRatio), PlantHue.Plain, false);
        }

        public static Seed RandomPeculiarSeed(int group)
        {
            switch ( group )
            {
                case 1:
                    return new Seed(PlantTypeInfo.RandomPeculiarGroupOne(), PlantHue.Plain, false);
                case 2:
                    return new Seed(PlantTypeInfo.RandomPeculiarGroupTwo(), PlantHue.Plain, false);
                case 3:
                    return new Seed(PlantTypeInfo.RandomPeculiarGroupThree(), PlantHue.Plain, false);
                default:
                    return new Seed(PlantTypeInfo.RandomPeculiarGroupFour(), PlantHue.Plain, false);
            }
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            string args;
            list.Add(this.GetLabel(out args), args);
        }

        public override void OnSingleClick(Mobile from)
        {
            string args;
            this.LabelTo(from, this.GetLabel(out args), args);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            from.Target = new InternalTarget(this);
            this.LabelTo(from, 1061916); // Choose a bowl of dirt to plant this seed in.
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is Seed)
            {
                Seed other = (Seed)dropped;

                if (other.PlantType == this.m_PlantType && other.PlantHue == this.m_PlantHue && other.ShowType == this.m_ShowType)
                    return base.StackWith(from, dropped, playSound);
            }

            return false;
        }

        public override void OnAfterDuped(Item newItem)
        {
            Seed newSeed = newItem as Seed;

            if (newSeed == null)
                return;

            newSeed.PlantType = this.m_PlantType;
            newSeed.PlantHue = this.m_PlantHue;
            newSeed.ShowType = this.m_ShowType;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_PlantType);
            writer.Write((int)this.m_PlantHue);
            writer.Write((bool)this.m_ShowType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_PlantType = (PlantType)reader.ReadInt();
            this.m_PlantHue = (PlantHue)reader.ReadInt();
            this.m_ShowType = reader.ReadBool();

            if (this.Weight != 1.0)
                this.Weight = 1.0;

            if (version < 1)
                this.Stackable = Core.SA;
        }

        private int GetLabel(out string args)
        {
            PlantHueInfo hueInfo = PlantHueInfo.GetInfo(this.m_PlantHue);

            int title = PlantTypeInfo.GetBonsaiTitle(this.m_PlantType);
            if (title == 0) // Not a bonsai
                title = hueInfo.Name;

            int label;

            if (this.Amount == 1)
                label = this.m_ShowType ? 1061917 : 1060838; // ~1_COLOR~ ~2_TYPE~ seed : ~1_val~ seed
            else
                label = this.m_ShowType ? 1113492 : 1113490; // ~1_amount~ ~2_color~ ~3_type~ seeds : ~1_amount~ ~2_val~ seeds

            if (hueInfo.IsBright())
                ++label;

            StringBuilder ab = new StringBuilder();

            if (this.Amount != 1)
            {
                ab.Append(this.Amount);
                ab.Append('\t');
            }

            ab.Append('#');
            ab.Append(title);

            if (this.m_ShowType)
            {
                PlantTypeInfo typeInfo = PlantTypeInfo.GetInfo(this.m_PlantType);

                ab.Append("\t#");
                ab.Append(typeInfo.Name);
            }

            args = ab.ToString();

            return label;
        }

        private class InternalTarget : Target
        {
            private readonly Seed m_Seed;
            public InternalTarget(Seed seed)
                : base(-1, false, TargetFlags.None)
            {
                this.m_Seed = seed;
                this.CheckLOS = false;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Seed.Deleted)
                    return;

                if (!this.m_Seed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }

                if (targeted is PlantItem)
                {
                    PlantItem plant = (PlantItem)targeted;

                    plant.PlantSeed(from, this.m_Seed);
                }
                else if (targeted is Item)
                {
                    ((Item)targeted).LabelTo(from, 1061919); // You must use a seed on a bowl of dirt!
                }
                else
                {
                    from.SendLocalizedMessage(1061919); // You must use a seed on a bowl of dirt!
                }
            }
        }
    }
}