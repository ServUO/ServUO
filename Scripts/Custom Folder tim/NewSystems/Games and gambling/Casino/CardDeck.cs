using System;
using Server;
using System.Collections;
using Server.Items;


namespace Server.Items
{
	public class CardDeck
	{

		private int m_DeckSize;
		private short[] m_cards;
		private int m_DeckPointer = 0;
		private int m_BackDesign;
		private int m_shufflecount = 0;
		private int[] BackDesigns = new int[] { 10100, 10307, 21252, 21255, 21504, 21510, 30501, 30502, 30503, 30504, 30505, 30506, 30507, 30508, 30509, 30510 };

		private bool m_randomback = true;

		public CardDeck(int decksize, int backdesign)
		{
			m_DeckSize = decksize > 8 ? 8 : decksize < 1 ? 1 : decksize;
			m_cards = new short[decksize * 52];
			for (int i = 0; i < m_cards.Length; i++)
				m_cards[i] = (short)  (i % 52);
			m_cards = LongShuffle(m_cards);

			if (backdesign != 0)
			{
				m_randomback = false;
				m_BackDesign = backdesign;
			}
			else
				m_BackDesign = Utility.RandomList(BackDesigns);
		}

		public int Remaining
		{
			get { return (m_DeckSize * 52) - m_DeckPointer; }
		}

		public int BackDesign
		{
			get { return m_BackDesign; }
		}

		public void ChangeBackDesign()
		{
			m_BackDesign = Utility.RandomList(BackDesigns);
		}

		public void ShuffleDeck()
		{
			m_cards = NormalShuffle(m_cards);
		}

		public void QuickShuffle()
		{
			m_cards = QuickShuffle(m_cards);
		}

		public short[] QuickShuffle(short[] cards)
		{
			return Shuffle(cards, 1);
		}

		private short[] NormalShuffle(short[] cards)
		{
			return Shuffle(cards, 4);
		}

		private short[] LongShuffle(short[] cards)
		{
			return Shuffle(cards, 8);
		}

		private short[] Shuffle(short[] cards, short shufflecount)
		{
			if (shufflecount == 1)
				cards = Cut(cards);
			for (int c = 0; c < shufflecount; c++)
			{
				for (int i = 0; i < cards.Length; i++)
				{
					short rnd1 = (short)i;
					short rnd2 = (short)Utility.Random(cards.Length);
					if (rnd2 == rnd1)
						continue;
					short temp = cards[rnd1];
					cards[rnd1] = cards[rnd2];
					cards[rnd2] = temp;
				}
				if (c % 2 == 0 && shufflecount > 1)
					cards = Cut(cards);
			}
			m_DeckPointer = 0;
			m_shufflecount++;
			if (m_shufflecount > 25 && m_randomback)
			{
				ChangeBackDesign();
				m_shufflecount = 0;
			}
			return cards;
		}

		private short[] Cut(short[] cards)
		{
			short[] copy = new short[cards.Length];
			int cutpoint = cards.Length / 2;
			int rnum = Utility.Random(2);
			cutpoint += (rnum == 0 ? Utility.Random(10) * (cards.Length % 52 + 1) * -1 : Utility.Random(10) * (cards.Length % 52 + 1));
			for (int i = 0; i < copy.Length; i++)
			{
				copy[i] = cards[cutpoint];
				cutpoint++;
				if (cutpoint == cards.Length)
					cutpoint = 0;
			}
			return copy;
		}

		public short GetOneCard()
		{
			if (m_DeckPointer >= m_cards.Length)
			{
				ShuffleDeck();
			}
			return m_cards[m_DeckPointer++];
		}
	}
}