using System;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Engines.Points;
using System.Collections.Generic;
using System.Linq;

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
        public LoyaltyRating LoyaltyRating
        {
            get
            {
                return CityLoyaltySystem.GetCityInstance(City).GetLoyaltyRating(Player, this);
            }
        }

        private bool _Utilizing;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UtilizingTradeDeal 
        { 
            get 
            {
                if (_Utilizing && TradeDealExpires < DateTime.UtcNow)
                    UtilizingTradeDeal = false;

                return _Utilizing;
            } 
            set 
            {
                if (Player == null)
                    return;

                if (!_Utilizing && value)
                    TradeDealExpires = DateTime.UtcNow + TimeSpan.FromHours(CityLoyaltySystem.TradeDealUtilizationPeriod);

                if (_Utilizing && !value && Player.NetState != null)
                {
                    BuffInfo.RemoveBuff(Player, BuffIcon.CityTradeDeal);
                    Player.SendLocalizedMessage(1154074); // The benefit from your City's Trade Deal has expired! Visit the City Stone to reclaim it.
                }

                _Utilizing = value;
            } 
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime TradeDealExpires { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool TradeDealExpired { get { return TradeDealExpires < DateTime.UtcNow; } }
		
		public CityLoyaltyEntry(PlayerMobile pm, City city) : base(pm)
		{
            City = city;
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
                Player.RemoveCollectionTitle(CityLoyaltySystem.GetTitleLocalization(Player, (CityTitle)i, City), true);
            }

            Player.RemoveCollectionTitle(1154017, true);

			IsCitizen = false;
            Titles = CityTitle.None;
		}
		
		public virtual void AddTitle(CityTitle title)
		{
            if ((Titles & title) == 0)
            {
                int loc = CityLoyaltySystem.GetTitleLocalization(Player, title, City);
                Player.AddCollectionTitle(loc);

                Titles |= title;
                Player.SendLocalizedMessage(1073625, String.Format("#{0}", loc.ToString())); // The title "~1_TITLE~" has been bestowed upon you.
            }
		}

        public bool CheckTradeDeal()
        {
            return UtilizingTradeDeal;
        }
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
			
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
	}
}