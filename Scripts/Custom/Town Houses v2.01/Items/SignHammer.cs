using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;

namespace Knives.TownHouses
{
	public enum HammerJob{ Flip, Swap }

    public class SignHammer : Item
    {
        private static Hashtable s_Table = new Hashtable();
        private static ArrayList s_List = new ArrayList();

        public static void Initialize()
        {
            // Signs
            s_Table[0xB95] = 0xB96;
            s_Table[0xB96] = 0xB95;
            s_Table[0xBA3] = 0xBA4;
            s_Table[0xBA4] = 0xBA3;
            s_Table[0xBA5] = 0xBA6;
            s_Table[0xBA6] = 0xBA5;
            s_Table[0xBA7] = 0xBA8;
            s_Table[0xBA8] = 0xBA7;
            s_Table[0xBA9] = 0xBAA;
            s_Table[0xBAA] = 0xBA9;
            s_Table[0xBAB] = 0xBAC;
            s_Table[0xBAC] = 0xBAB;
            s_Table[0xBAD] = 0xBAE;
            s_Table[0xBAE] = 0xBAD;
            s_Table[0xBAF] = 0xBB0;
            s_Table[0xBB0] = 0xBAF;
            s_Table[0xBB1] = 0xBB2;
            s_Table[0xBB2] = 0xBB1;
            s_Table[0xBB3] = 0xBB4;
            s_Table[0xBB4] = 0xBB3;
            s_Table[0xBB5] = 0xBB6;
            s_Table[0xBB6] = 0xBB5;
            s_Table[0xBB7] = 0xBB8;
            s_Table[0xBB8] = 0xBB7;
            s_Table[0xBB9] = 0xBBA;
            s_Table[0xBBA] = 0xBB9;
            s_Table[0xBBB] = 0xBBC;
            s_Table[0xBBC] = 0xBBB;
            s_Table[0xBBD] = 0xBBE;
            s_Table[0xBBE] = 0xBBD;
            s_Table[0xBBF] = 0xBC0;
            s_Table[0xBC0] = 0xBBF;
            s_Table[0xBC1] = 0xBC2;
            s_Table[0xBC2] = 0xBC1;
            s_Table[0xBC3] = 0xBC4;
            s_Table[0xBC4] = 0xBC3;
            s_Table[0xBC5] = 0xBC6;
            s_Table[0xBC6] = 0xBC5;
            s_Table[0xBC7] = 0xBC8;
            s_Table[0xBC8] = 0xBC7;
            s_Table[0xBC9] = 0xBCA;
            s_Table[0xBCA] = 0xBC9;
            s_Table[0xBCB] = 0xBCC;
            s_Table[0xBCC] = 0xBCB;
            s_Table[0xBCD] = 0xBCE;
            s_Table[0xBCE] = 0xBCD;
            s_Table[0xBCF] = 0xBD0;
            s_Table[0xBD0] = 0xBCF;
            s_Table[0xBD1] = 0xBD2;
            s_Table[0xBD2] = 0xBD1;
            s_Table[0xBD3] = 0xBD4;
            s_Table[0xBD4] = 0xBD3;
            s_Table[0xBD5] = 0xBD6;
            s_Table[0xBD6] = 0xBD5;
            s_Table[0xBD7] = 0xBD8;
            s_Table[0xBD8] = 0xBD7;
            s_Table[0xBD9] = 0xBDA;
            s_Table[0xBDA] = 0xBD9;
            s_Table[0xBDB] = 0xBDC;
            s_Table[0xBDC] = 0xBDB;
            s_Table[0xBDD] = 0xBDE;
            s_Table[0xBDE] = 0xBDD;
            s_Table[0xBDF] = 0xBE0;
            s_Table[0xBE0] = 0xBDF;
            s_Table[0xBE1] = 0xBE2;
            s_Table[0xBE2] = 0xBE1;
            s_Table[0xBE3] = 0xBE4;
            s_Table[0xBE4] = 0xBE3;
            s_Table[0xBE5] = 0xBE6;
            s_Table[0xBE6] = 0xBE5;
            s_Table[0xBE7] = 0xBE8;
            s_Table[0xBE8] = 0xBE7;
            s_Table[0xBE9] = 0xBEA;
            s_Table[0xBEA] = 0xBE9;
            s_Table[0xBEB] = 0xBEC;
            s_Table[0xBEC] = 0xBEB;
            s_Table[0xBED] = 0xBEE;
            s_Table[0xBEE] = 0xBED;
            s_Table[0xBEF] = 0xBF0;
            s_Table[0xBF0] = 0xBEF;
            s_Table[0xBF1] = 0xBF2;
            s_Table[0xBF2] = 0xBF1;
            s_Table[0xBF3] = 0xBF4;
            s_Table[0xBF4] = 0xBF3;
            s_Table[0xBF5] = 0xBF6;
            s_Table[0xBF6] = 0xBF5;
            s_Table[0xBF7] = 0xBF8;
            s_Table[0xBF8] = 0xBF7;
            s_Table[0xBF9] = 0xBFA;
            s_Table[0xBFA] = 0xBF9;
            s_Table[0xBFB] = 0xBFC;
            s_Table[0xBFC] = 0xBFB;
            s_Table[0xBFD] = 0xBFE;
            s_Table[0xBFE] = 0xBFD;
            s_Table[0xBFF] = 0xC00;
            s_Table[0xC00] = 0xBFF;
            s_Table[0xC01] = 0xC02;
            s_Table[0xC02] = 0xC01;
            s_Table[0xC03] = 0xC04;
            s_Table[0xC04] = 0xC03;
            s_Table[0xC05] = 0xC06;
            s_Table[0xC06] = 0xC05;
            s_Table[0xC07] = 0xC08;
            s_Table[0xC08] = 0xC07;
            s_Table[0xC09] = 0xC0A;
            s_Table[0xC0A] = 0xC09;
            s_Table[0xC0B] = 0xC0C;
            s_Table[0xC0C] = 0xC0B;
            s_Table[0xC0D] = 0xC0E;
            s_Table[0xC0E] = 0xC0D;

            // Hangers
            s_Table[0xB97] = 0xB98;
            s_Table[0xB98] = 0xB97;
            s_Table[0xB99] = 0xB9A;
            s_Table[0xB9A] = 0xB99;
            s_Table[0xB9B] = 0xB9C;
            s_Table[0xB9C] = 0xB9B;
            s_Table[0xB9D] = 0xB9E;
            s_Table[0xB9E] = 0xB9D;
            s_Table[0xB9F] = 0xBA0;
            s_Table[0xBA0] = 0xB9F;
            s_Table[0xBA1] = 0xBA2;
            s_Table[0xBA2] = 0xBA1;

            // Hangers for swapping
            s_List.Add(0xB97);
            s_List.Add(0xB98);
            s_List.Add(0xB99);
            s_List.Add(0xB9A);
            s_List.Add(0xB9B);
            s_List.Add(0xB9C);
            s_List.Add(0xB9D);
            s_List.Add(0xB9E);
            s_List.Add(0xB9F);
            s_List.Add(0xBA0);
            s_List.Add(0xBA1);
            s_List.Add(0xBA2);
        }

        private HammerJob c_Job;

        public HammerJob Job { get { return c_Job; } set { c_Job = value; } }

        [Constructable]
        public SignHammer()
            : base(0x13E3)
        {
            Name = "Sign Hammer";
        }

        public int GetFlipFor(int id)
        {
            return (s_Table[id] == null ? id : (int)s_Table[id]);
        }

        public int GetNextSign(int id)
        {
            if (!s_List.Contains(id))
                return id;

            int idx = s_List.IndexOf(id);

            if (idx + 2 < s_List.Count)
                return (int)s_List[idx + 2];

            if (idx % 2 == 0)
                return (int)s_List[0];

            return (int)s_List[1];
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (RootParent != m)
            {
                m.SendMessage("That item must be in your backpack to use.");
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(m);

            if (m.AccessLevel == AccessLevel.Player && (house == null || house.Owner != m))
            {
                m.SendMessage("You have to be inside your house to use this.");
                return;
            }

            m.BeginTarget(3, false, TargetFlags.None, new TargetCallback(OnTarget));
        }

        protected void OnTarget(Mobile m, object obj)
        {
            Item item = obj as Item;

            if (item == null)
            {
                m.SendMessage("You cannot change that with this.");
                return;
            }

            if (item == this)
            {
                new SignHammerGump(m, this);
                return;
            }

            if (c_Job == HammerJob.Flip)
            {
                int id = GetFlipFor(item.ItemID);

                if (id == item.ItemID)
                    m.SendMessage("You cannot change that with this.");
                else
                    item.ItemID = id;
            }
            else
            {
                int id = GetNextSign(item.ItemID);

                if (id == item.ItemID)
                    m.SendMessage("You cannot change that with this.");
                else
                    item.ItemID = id;
            }
        }

        public SignHammer(Serial serial)
            : base(serial)
        {
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
    }

	public class SignHammerGump : GumpPlusLight
	{
		private SignHammer c_Hammer;

		public SignHammerGump( Mobile m, SignHammer hammer ) : base( m, 100, 100 )
		{
			c_Hammer = hammer;

			NewGump();
		}

		protected override void BuildGump()
		{
            AddBackground(0, 0, 200, 200, 2600);

            AddButton(50, 45, 2152, 2154, "Swap", new GumpCallback(Swap));
			AddHtml( 90, 50, 70, "Swap Hanger");

			AddButton( 50, 95, 2152, 2154, "Flip", new GumpCallback( Flip ) );
			AddHtml( 90, 100, 70, "Flip Sign or Hanger");
        }

		private void Swap()
		{
			c_Hammer.Job = HammerJob.Swap;
		}

		private void Flip()
		{
			c_Hammer.Job = HammerJob.Flip;
		}
	}
}