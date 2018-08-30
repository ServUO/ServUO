#define RUNUO_2 //Comment this out to enable RunUO 1.0 Mode

using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Touring
{
	public class TourGuide : Mobile
	{
		public override bool ClickTitle { get { return false; } }

		[Constructable]
		public TourGuide(Destination dest)
		{
			SpeechHue = 0x55;

			Hidden = true;
			Blessed = true;
			CantWalk = true;
			Frozen = true;

			if (Utility.RandomBool())
			{
				BodyValue = 401;
				Female = true;
				Name = NameList.RandomName("female");
			}
			else
			{
				BodyValue = 400;
				Female = false;
				Name = NameList.RandomName("male");
			}

			Title = "[Reisefuehrer/in]";

			Dress();

			bool esc = false;
			Point3D loc = new Point3D(0, 0, 0);

			for (int x = dest.Location.X - 2; x < dest.Location.X + 2; x++)
			{
				for (int y = dest.Location.Y - 2; y < dest.Location.Y + 2; y++)
				{
					Point3D test = new Point3D(x, y, dest.Location.Z);//dest.Map.GetAverageZ(x, y));

					if (test == dest.Location)
						continue;

					if (dest.Map.CanSpawnMobile(test))
					{
						loc = test;
						esc = true;
						break;
					}
				}

				if (esc)
					break;
			}

			if (!Server.Spells.SpellHelper.IsInvalid(dest.Map, loc) && loc != new Point3D(0, 0, 0))
				MoveToWorld(loc, dest.Map);
			else
				Delete();
		}

		public void ShowTo(Mobile m)
		{
			if (m == null || m.Deleted)
				return;

			if (m is PlayerMobile)
			{
				PlayerMobile pm = (PlayerMobile)m;

				if (pm.NetState == null)
					return;

				MobileUpdate p1 = new MobileUpdate(this);
				MobileIncoming p2 = new MobileIncoming(pm, this);

				pm.NetState.Send(p1);
				pm.NetState.Send(p2);
			}

			Direction = GetDirectionTo(m.Location);
		}

		private int GetRandomHue()
		{
			switch (Utility.Random(6))
			{
				default:
				case 0: return 0;
				case 1: return Utility.RandomBlueHue();
				case 2: return Utility.RandomGreenHue();
				case 3: return Utility.RandomRedHue();
				case 4: return Utility.RandomYellowHue();
				case 5: return Utility.RandomNeutralHue();
			}
		}

		private void Dress()
		{
			if (Female)
				AddItem(new PlainDress());
			else
				AddItem(new Shirt(GetRandomHue()));

			int lowHue = GetRandomHue();

			AddItem(new ShortPants(lowHue));

			if (Female)
				AddItem(new Boots(lowHue));
			else
				AddItem(new Shoes(lowHue));

#if(RUNUO_2)
			Utility.AssignRandomHair(this);
#else
			AddItem(new FloppyHat());
#endif
		}

		public TourGuide(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch (version)
			{
				case 0: { Delete(); } break;
			}
		}

		public override bool OnMoveOver(Mobile m)
		{
			return true;
		}
	}
}