using Server;
using System;

namespace Server.Items
{
    public class SmugglersLiquor : BaseBeverage
    {
        private int m_Label;
        private SmugglersLiquorType m_Type;

        public override int LabelNumber { get { return m_Label; } }
        public override int MaxQuantity { get { return 5; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public SmugglersLiquorType LiquorType { get { return m_Type; } set { m_Type = value; ItemID = ComputeItemID(); } }

        public enum SmugglersLiquorType
        {
            AppleIsleWhiskey,
            ArabellasGargishStout,
            DiablosBlazePort,
            EldadorElvenWine,
            EquinoxWine,
            FitchsWhiteAle,
            GatheredSpiritsWhisky,
            GoldenBrewRum,
            JoesSpicyBrew,
            KazolasTreeTopWine,
            Moonshine,
            NapaValleysChardonnay,
            OtisHomemadeBrew,
            PandorasPinotNoir,
            RedEyeRum,
            RoyalGuardAle,
            SaltyDogMead,
            SapewinWine,
            SerpentsCrossLager,
            SummerSolsticeBrew,
            VesperAle,
            WaxingDarkBrew,
            WhiteRoseZinfandel,
        }

        [Constructable]
        public SmugglersLiquor() : this((SmugglersLiquorType)Utility.Random(26))
        {
        }

        public SmugglersLiquor(SmugglersLiquorType type) : base(GetContents(type))
        {
            this.LiquorType = type;
        }

        public static BeverageType GetContents(SmugglersLiquorType type)
        {
            switch (type)
            {
                default:
                case SmugglersLiquorType.AppleIsleWhiskey: return BeverageType.Liquor;
                case SmugglersLiquorType.ArabellasGargishStout: return BeverageType.Ale;
                case SmugglersLiquorType.DiablosBlazePort: return BeverageType.Ale;
                case SmugglersLiquorType.EldadorElvenWine: return BeverageType.Wine;
                case SmugglersLiquorType.EquinoxWine: return BeverageType.Wine;                    
                case SmugglersLiquorType.FitchsWhiteAle: return BeverageType.Ale;                   
                case SmugglersLiquorType.GatheredSpiritsWhisky: return BeverageType.Liquor;
                case SmugglersLiquorType.GoldenBrewRum: return BeverageType.Liquor;  
                case SmugglersLiquorType.JoesSpicyBrew: return BeverageType.Ale;                   
                case SmugglersLiquorType.KazolasTreeTopWine: return BeverageType.Wine;                   
                case SmugglersLiquorType.Moonshine: return BeverageType.Liquor;                  
                case SmugglersLiquorType.NapaValleysChardonnay: return BeverageType.Wine;                    
                case SmugglersLiquorType.OtisHomemadeBrew: return BeverageType.Ale;                   
                case SmugglersLiquorType.PandorasPinotNoir: return BeverageType.Wine;                    
                case SmugglersLiquorType.RedEyeRum: return BeverageType.Liquor;                    
                case SmugglersLiquorType.RoyalGuardAle: return BeverageType.Ale;                    
                case SmugglersLiquorType.SaltyDogMead: return BeverageType.Ale;                   
                case SmugglersLiquorType.SapewinWine: return BeverageType.Wine;                    
                case SmugglersLiquorType.SerpentsCrossLager: return BeverageType.Ale;                   
                case SmugglersLiquorType.SummerSolsticeBrew: return BeverageType.Ale;                    
                case SmugglersLiquorType.VesperAle: return BeverageType.Ale;                   
                case SmugglersLiquorType.WaxingDarkBrew: return BeverageType.Ale;                    
                case SmugglersLiquorType.WhiteRoseZinfandel: return BeverageType.Wine;                   
            }
        }

        public override int ComputeItemID()
        {
            int id = 0;
            switch (m_Type)
            {
                case SmugglersLiquorType.AppleIsleWhiskey:
                    id = 2504;
                    Hue = 692; //Confirmed
                    m_Label = 1150020;
                    break;
                case SmugglersLiquorType.ArabellasGargishStout:
                    id = 2459;
                    Hue = 507;  //Confirmed
                    m_Label = 1150033;
                    break;
                case SmugglersLiquorType.DiablosBlazePort:
                    id = 2459;
                    Hue = 675; //Confirmed
                    m_Label = 1150034;
                    break;
                case SmugglersLiquorType.EldadorElvenWine:
                    id = 2459;
                    Hue = 540; //Confirmed
                    m_Label = 1150029;
                    break;
                case SmugglersLiquorType.EquinoxWine:
                    id = 2459;
                    Hue = 634; //Confirmed
                    m_Label = 1150037;
                    break;
                case SmugglersLiquorType.FitchsWhiteAle:
                    id = 2459;
                    Hue = 687; //Confirmed
                    m_Label = 1150035;
                    break;
                case SmugglersLiquorType.GatheredSpiritsWhisky:
                    id = 2459;
                    Hue = 654; //Confirmed
                    m_Label = 1150026;
                    break;
                case SmugglersLiquorType.GoldenBrewRum:
                    id = 2459;
                    Hue = 584;  //Confirmed
                    m_Label = 1150039;
                    break;
                case SmugglersLiquorType.JoesSpicyBrew:
                    id = 2459;
                    Hue = 535; //Confirmed
                    m_Label = 1150027;
                    break;
                case SmugglersLiquorType.KazolasTreeTopWine:
                    id = 2504;
                    Hue = 512; //Confirmed
                    m_Label = 123456;
                    break;
                case SmugglersLiquorType.Moonshine:
                    id = 2504;
                    Hue = 555; //Confirmed
                    m_Label = 1150041;
                    break;
                case SmugglersLiquorType.NapaValleysChardonnay:
                    id = 2459;
                    Hue = 600;  //Confirmed
                    m_Label = 1150025;
                    break;
                case SmugglersLiquorType.OtisHomemadeBrew:
                    id = 2459;
                    Hue = 680; //Confirmed
                    m_Label = 1150019;
                    break;
                case SmugglersLiquorType.PandorasPinotNoir:
                    id = 2459;
                    Hue = 621; //Confirmed
                    m_Label = 1150023;
                    break;
                case SmugglersLiquorType.RedEyeRum:
                    id = 2459;
                    Hue = 687; //Confirmed
                    m_Label = 1150022;
                    break;
                case SmugglersLiquorType.RoyalGuardAle:
                    id = 2504;
                    Hue = 651; //Confirmed
                    m_Label = 1150024;
                    break;
                case SmugglersLiquorType.SaltyDogMead:
                    id = 2504;
                    Hue = 566; //Confirmed
                    m_Label = 1150028;
                    break;
                case SmugglersLiquorType.SapewinWine:
                    id = 2459;
                    Hue = 557;  //Confirmed
                    m_Label = 1150040;
                    break;
                case SmugglersLiquorType.SerpentsCrossLager:
                    id = 2459;
                    Hue = 593; //Confirmed
                    m_Label = 1150031;
                    break;
                case SmugglersLiquorType.SummerSolsticeBrew:
                    id = 2459;
                    Hue = 663;  //Confirmed
                    m_Label = 1150038;
                    break;
                case SmugglersLiquorType.VesperAle:
                    id = 2504;
                    Hue = 519; //Confirmed
                    m_Label = 1150032;
                    break;
                case SmugglersLiquorType.WaxingDarkBrew:
                    id = 2459;
                    Hue = 567; //Confirmed
                    m_Label = 1150030;
                    break;
                case SmugglersLiquorType.WhiteRoseZinfandel:
                    id = 2459;
                    Hue = 516; //Confirmed
                    m_Label = 1150021;
                    break;
            }
            return id;
        }

        public static Item GetRandom()
        {
            int pick = Utility.Random(22);

            return new SmugglersLiquor((SmugglersLiquorType)pick) as Item;
        }

        public SmugglersLiquor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Type = (SmugglersLiquorType)reader.ReadInt();
                    break;
                case 0:
                    int type = reader.ReadInt();

                    if (type > (int)SmugglersLiquorType.WhiteRoseZinfandel)
                        m_Type = SmugglersLiquorType.WhiteRoseZinfandel;
                    else
                        m_Type = (SmugglersLiquorType)type;
                    break;
            }
        }
    }
}