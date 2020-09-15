using Server.Engines.Points;
using Server.Mobiles;
using System;

namespace Server.Engines.CityLoyalty
{
    [PropertyObject]
    public class CityLoyaltyEntry : PointsEntry
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public int Love { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Hate { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Neutrality { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public CityTitle Titles { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCitizen { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public City City { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string CustomTitle { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowGainMessage { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public double ArtisanBodPoints { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public LoyaltyRating LoyaltyRating => CityLoyaltySystem.GetCityInstance(City).GetLoyaltyRating(Player, this);

        private bool _Utilizing;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UtilizingTradeDeal
        {
            get
            {
                return _Utilizing;
            }
            set
            {
                if (Player == null)
                    return;

                if (!_Utilizing && value)
                    TradeDealExpires = DateTime.UtcNow + TimeSpan.FromHours(CityLoyaltySystem.TradeDealUtilizationPeriod);

                if (_Utilizing && !value)
                {
                    BuffInfo.RemoveBuff(Player, BuffIcon.CityTradeDeal);

                    CityLoyaltySystem.RemoveTradeDeal(Player);

                    if (Player.NetState != null)
                    {
                        Player.SendLocalizedMessage(1154074); // The benefit from your City's Trade Deal has expired! Visit the City Stone to reclaim it.
                    }

                    Player.Delta(MobileDelta.WeaponDamage);
                }

                _Utilizing = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TradeDealExpires { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TradeDealExpired => TradeDealExpires < DateTime.UtcNow;

        public CityLoyaltyEntry(PlayerMobile pm, City city) : base(pm)
        {
            City = city;
            ShowGainMessage = true;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", City.ToString(), IsCitizen ? "[Citizen]" : string.Empty);
        }

        public void DeclareCitizenship()
        {
            IsCitizen = true;
            AddTitle(CityTitle.Citizen);
        }

        public void RenounceCitizenship()
        {
            foreach (int i in Enum.GetValues(typeof(CityTitle)))
            {
                Player.RemoveRewardTitle(CityLoyaltySystem.GetTitleLocalization(Player, (CityTitle)i, City), true);
            }

            CustomTitle = null;
            Player.RemoveRewardTitle(1154017, true);

            IsCitizen = false;
            Titles = CityTitle.None;
        }

        public virtual void AddTitle(CityTitle title)
        {
            if ((Titles & title) == 0)
            {
                int loc = CityLoyaltySystem.GetTitleLocalization(Player, title, City);
                Player.AddRewardTitle(loc);

                Titles |= title;
                Player.SendLocalizedMessage(1073625, string.Format("#{0}", loc.ToString())); // The title "~1_TITLE~" has been bestowed upon you.
            }
        }

        public void CheckTradeDeal()
        {
            if (TradeDealExpired)
            {
                UtilizingTradeDeal = false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(ShowGainMessage);

            writer.Write(Love);
            writer.Write(Hate);
            writer.Write(Neutrality);

            writer.Write((int)Titles);
            writer.Write(TradeDealExpires);
            writer.Write(_Utilizing);
            writer.Write(CustomTitle);

            writer.Write(IsCitizen);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    ShowGainMessage = reader.ReadBool();
                    goto case 0;
                case 0:
                    {
                        if (version == 0)
                        {
                            ShowGainMessage = true;
                        }

                        Love = reader.ReadInt();
                        Hate = reader.ReadInt();
                        Neutrality = reader.ReadInt();

                        Titles = (CityTitle)reader.ReadInt();
                        TradeDealExpires = reader.ReadDateTime();
                        _Utilizing = reader.ReadBool();
                        CustomTitle = reader.ReadString();

                        CheckTradeDeal();
                        IsCitizen = reader.ReadBool();
                    }
                    break;
            }
        }
    }
}
