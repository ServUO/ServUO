using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Engines.ResortAndCasino
{
	public class CasinoCashier : Banker
	{
        [Constructable]
        public CasinoCashier()
        {
            Title = "the casino cashier";
            CantWalk = true;
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 2498);
            SetWearable(new Shoes(), 2413);

            Item pants = new LongPants();
            pants.ItemID = 0x2FC3;
            pants.Name = "Elven Pants";
            SetWearable(pants, 1910);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(this.Location, 3))
            {
                m.SendGump(new PurchaseCasinoChipGump(m as PlayerMobile));
            }
        }

        public CasinoCashier(Serial serial)
            : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int v = reader.ReadInt();
		}
	}

    public class CasinoDealer : BaseVendor
    {
        public Dictionary<PlayerMobile, BaseDiceGame> Players { get; set; }

        public override bool IsInvulnerable { get { return true; } }
        public override bool IsActiveVendor { get { return false; } }

        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public override void InitSBInfo()
        {
        }

        public CasinoDealer(string title)
            : base(title)
        {
            CantWalk = true;
        }

        public override void InitBody()
        {
            SetStr(100);
            SetInt(125);
            SetDex(100);

            if (Utility.RandomDouble() > 0.5)
            {
                Female = true;
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Female = false;
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            HairItemID = Race.RandomHair(Female);
            FacialHairItemID = Race.RandomFacialHair(Female);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 1169);
            SetWearable(new Shoes(), 1169);

            Item pants = new LongPants();
            pants.ItemID = 0x2FC3;
            pants.Name = "Elven Pants";
            SetWearable(pants, 1910);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && InRange(m.Location, 3))
            {
                SendGump((PlayerMobile)m);
            }
        }

        public void AddGame(PlayerMobile pm, BaseDiceGame game)
        {
            if (Players == null)
                Players = new Dictionary<PlayerMobile, BaseDiceGame>();

            Players[pm] = game;
        }

        public void RemoveGame(PlayerMobile pm, BaseDiceGame game)
        {
            if (Players == null || !Players.ContainsKey(pm))
                return;

            Players.Remove(pm);
        }

        public bool HasGame(PlayerMobile pm)
        {
            return GetGame(pm) != null;
        }

        public BaseDiceGame GetGame(PlayerMobile pm)
        {
            if (Players == null || !Players.ContainsKey(pm) || Players[pm] == null)
                return null;

            return Players[pm]; 
        }

        public virtual void SendGump(PlayerMobile pm)
        {
        }

        public override bool CanBeDamaged()
        {
            return false;
        }

        public CasinoDealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class ChucklesLuckDealer : CasinoDealer
    {
        [Constructable]
        public ChucklesLuckDealer()
            : base("The Chuckles' Luck Dealer")
        {
        }

        public override void SendGump(PlayerMobile pm)
        {
            ChucklesLuck game = GetGame(pm) as ChucklesLuck;

            if (game == null)
            {
                game = new ChucklesLuck(pm, this);
                AddGame(pm, game);
            }

            ChucklesLuckGump g = pm.FindGump(typeof(ChucklesLuckGump)) as ChucklesLuckGump;

            if(g != null)
                g.Refresh();
            else
            {
                pm.SendGump(new ChucklesLuckGump(pm, game));
            }
        }

        public ChucklesLuckDealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class HiMiddleLowDealer : CasinoDealer
    {
        [Constructable]
        public HiMiddleLowDealer()
            : base("The Hi-Middle-Low Dealer")
        {
        }

        public override void SendGump(PlayerMobile pm)
        {
            HiMiddleLow game = GetGame(pm) as HiMiddleLow;

            if (game == null)
            {
                game = new HiMiddleLow(pm, this);
                AddGame(pm, game);
            }

            HiMiddleLowGump g = pm.FindGump(typeof(HiMiddleLowGump)) as HiMiddleLowGump;

            if (g != null)
                g.Refresh();
            else
            {
                pm.SendGump(new HiMiddleLowGump(pm, game));
            }
        }

        public HiMiddleLowDealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class DiceRiderDealer : CasinoDealer
    {
        [Constructable]
        public DiceRiderDealer()
            : base("The Dice Rider Dealer")
        {
        }

        public override void SendGump(PlayerMobile pm)
        {
            DiceRider game = GetGame(pm) as DiceRider;

            if (game == null)
            {
                game = new DiceRider(pm, this);
                AddGame(pm, game);
            }

            DiceRiderGump g = pm.FindGump(typeof(DiceRiderGump)) as DiceRiderGump;

            if (g != null)
                g.Refresh();
            else
            {
                pm.SendGump(new DiceRiderGump(pm, game));
            }
        }

        public DiceRiderDealer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class CasinoWaitress : BaseVendor
    {
        public override bool IsActiveVendor { get { return false; } }

        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        public Dictionary<Mobile, int> _Drinks { get; set; }

        private DateTime _NextAdvertise;

        public override void InitSBInfo()
        {
        }

        [Constructable]
        public CasinoWaitress()
            : base("The drinks girl")
        {
        }

        public override void InitBody()
        {
            InitStats(125, 100, 25);

            SpeechHue = 1276;
            Hue = Utility.RandomSkinHue();

            if (IsInvulnerable && !Core.AOS)
                NameHue = 0x35;

            Female = true;
            Body = 0x191;
            HairItemID = Race.RandomHair(true);
            HairHue = Race.RandomHairHue();
            Name = NameList.RandomName("female");
        }

        public override void InitOutfit()
        {
            SetWearable(new StuddedBustierArms(), 1927);
            SetWearable(new LeatherSkirt(), 1930);
            SetWearable(new Sandals(), 1927);
            SetWearable(new GoldBracelet(), 1931);
            SetWearable(new GoldRing(), 1931);
            SetWearable(new Necklace(), 1931);
            SetWearable(new GoldEarrings(), 1931);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if(!m.InRange(this.Location, 3))
                return;

            if (_Drinks == null)
                _Drinks = new Dictionary<Mobile, int>();

            if (!_Drinks.ContainsKey(m) || _Drinks[m] < 2)
            {
                GiveDrink(m);

                if (_Drinks.ContainsKey(m))
                    _Drinks[m]++;
                else
                    _Drinks[m] = 1;
            }
        }

        public void GiveDrink(Mobile m)
        {
            FortunesFireGrog grog = new FortunesFireGrog();
            m.AddToBackpack(grog);

            int cliloc = 1153416 + Utility.RandomMinMax(0, 2);

            SayTo(m, cliloc); // Here you are, hun. - Drink up! - Enjoy the drink. Tips are appreciated!
        }

        public override void OnThink()
        {
            base.OnThink();

            IPooledEnumerable eable = this.Map.GetMobilesInRange(this.Location, 8);
            bool canspeak = _NextAdvertise < DateTime.UtcNow;

            if (!canspeak)
                return;

            canspeak = false;
            foreach (Mobile m in eable)
            {
                if (m is PlayerMobile)
                {
                    canspeak = true;
                    break;
                }
            }

            if (canspeak)
                Say(1153419);

            _NextAdvertise = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(60, 120));
        }

        public override bool OnGoldGiven(Mobile from, Gold dropped)
        {
            Direction = GetDirectionTo(from);
            SayTo(from, 1153420); // Oh, thank you dearie!
            dropped.Delete();
            return true;
        }

        public CasinoWaitress(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            if(_Drinks != null)
                _Drinks.Clear();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}