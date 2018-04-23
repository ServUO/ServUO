//////////////////////////////////////////////////////////////////////////////////////////////////////////
//////////////                      Scented Candle by Abracadabra                           //////////////
//////////////                      Special Thanks to Ravenwolfe                            //////////////
//////////////          Ultima Online Phoenix: www.ultimaonlinephoenix.com                  //////////////
//////////////Lock down in your home and burn to receive mana regeneration for a short time.//////////////
//////////////Feel free to make any modifications you wish but please leave credit in place.//////////////
//////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using Server;
using System.Collections;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class ScentedCandle : BaseLight
    {
        public override int LitItemID { get { return 0x1430; } }
	    public override int UnlitItemID { get { return 0x1433; } }

        private static bool _activated;
        private int m_MaxCharges;
        private int m_Charges;
        private int m_MaxChargeTime;
        private int m_ChargeTime;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Activated
		{
			get { return _activated; }
			set { _activated = value; InvalidateProperties(); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set { m_MaxCharges = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                m_Charges = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxChargeTime
        {
            get { return m_MaxChargeTime; }
            set { m_MaxChargeTime = value; InvalidateProperties(); }
        }

        public int ChargeTime
        {
            get { return m_ChargeTime; }
            set { m_ChargeTime = value; InvalidateProperties(); }
        }

        [Constructable]
        public ScentedCandle() : base(0x1433)
        {
            MaxChargeTime = 300;
            Charges = 50;
            MaxCharges = Charges;
            Hue = 1165;
            Name = "a scented candle";
            Burning = false;
            Light = LightType.Circle150;
            Weight = 2.0;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "Charges: {0}", Charges);

            if (_activated)
            {
                LabelTo(from, "Giving off a lovely scent!");
            }
        }

        public override void OnDoubleClick(Mobile from)
		{
			if (_activated)
			{
				from.SendMessage("This candle is already burning!");
				return;
			}

			if (IsLockedDown)
			{
				BaseHouse house = BaseHouse.FindHouseAt(from);

				if (house != null && house.IsCoOwner(from))
				{
					if (ChargeTime <= 0)
					{
						if (Charges > 0)
						{
							Timer t = new InternalTimer(this);

							t.Start();

							Charges--;
							_activated = true;
							from.SendMessage("A wonderful aroma fills the air!");
							ChargeTime = MaxChargeTime;
							Ignite();
							return;
						}
					}
				}
				else
					from.SendLocalizedMessage(502436); // That is not accessible.
			}
			else
				from.SendLocalizedMessage(502692); // This must be in a house and be locked down to work.

            base.OnDoubleClick(from);
        }

        public static void DoSpecial(Mobile from)
        {
            if (from.Alive)
            {
                if (from.Mana < from.ManaMax)
                {
                    from.Mana += 10;
                    from.FixedEffect(0x37B9, 10, 16, 1165, 3);
                }
            }
        }

        public ScentedCandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private class InternalTimer : Timer
        {
            private int _mCount;
	        private ScentedCandle _candle;

            public InternalTimer(ScentedCandle candle) : base(TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(2.0))
            {
                Priority = TimerPriority.TwoFiftyMS;
	            _candle = candle;
            }

	        protected override void OnTick()
	        {
		        if (_candle == null || _candle.Deleted)
		        {
			        Stop();
			        return;
		        }

		        if (!_candle.IsLockedDown)
		        {
			        Stop();
			        _candle.Activated = false;
			        _candle.Douse();
			        return;
		        }

		        if (_mCount >= _candle.ChargeTime)
		        {
			        Stop();
			        _candle.Activated = false;
			        _candle.Douse();
		        }

		        else
		        {
			        foreach (Mobile mob in _candle.GetMobilesInRange(8))
				        if (mob is PlayerMobile)
				        {
					        DoSpecial(mob);
				        }

			        _mCount++;
		        }
	        }
        }
    }
}