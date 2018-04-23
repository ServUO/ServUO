/*
Script Name: ChocolateHeart.cs
Author: CEO
Version: 1.0
Public Release: 02/13/08
*/ 
using System;
using Server;
using Server.Network;


namespace Server.Items
{
	public class ChocolateHeart : Food
	{
		private InternalTimer toothache;

        public override int LabelNumber { get { return (Hue == 1120 ? 1079995 : (Hue == 1125 ? 1079994 : 1079996)); } }

            [Constructable]
		public ChocolateHeart()
			: base(0x24B)
		{
			Stackable = false;
            Hue = Utility.RandomList(1120, 1125, 1150);
			this.Weight = 1.0;
			this.FillFactor = 0;
		}

		public ChocolateHeart(Serial serial)
			: base(serial)
		{
		}

		private int GetPhrase()
		{
			return 0;
		}

		public override bool Eat(Mobile from)
		{
			from.PlaySound(Utility.Random(0x3A, 3));

			if (from.Body.IsHuman && !from.Mounted)
				from.Animate(34, 5, 1, true, false, 0);

			if (Utility.RandomDouble() < 0.05)
				GiveToothAche(from, 0);
			else
				from.SendLocalizedMessage(1077387);

			Consume();
			return true;
		}


		public void GiveToothAche(Mobile from, int seq)
		{
			if (toothache != null)
				toothache.Stop();
			from.SendLocalizedMessage(1077388 + seq);
			if (seq < 5)
			{
				toothache = new InternalTimer(this, from, seq, TimeSpan.FromSeconds(15));
				toothache.Start();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		private class InternalTimer : Timer
		{
			private ChocolateHeart i_ChocolateHeart;
			private int i_sequencer;
			private Mobile i_mobile;

			public InternalTimer(ChocolateHeart ChocolateHeart, Mobile m, int sequencer, TimeSpan delay)
				: base(delay)
			{
				Priority = TimerPriority.OneSecond;
				i_ChocolateHeart = ChocolateHeart;
				i_mobile = m;
				i_sequencer = sequencer;
			}

			protected override void OnTick()
			{
				if (i_mobile != null)
				{
					i_ChocolateHeart.GiveToothAche(i_mobile, i_sequencer + 1);
				}
			}
		}
	}
}