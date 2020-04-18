using Server.Gumps;
using Server.Mobiles;
using Server.SkillHandlers;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public interface ISliderKit : IEntity
    {
        int[] Order { get; }
        int Style { get; }

        void Complete(Mobile m);
    }

    public class SliderTrapTrainingKit : Item, ISliderKit, IRemoveTrapTrainingKit
    {
        public override int LabelNumber => 1159016;  // Slider Trap Training Kit

        private int _Style;

        public int[] Order { get; set; } = new int[9];

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Test
        {
            get { return false; }
            set
            {
                if (value)
                {
                    TestOrder();
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Style
        {
            get { return _Style; }
            set
            {
                if (_Style != value)
                {
                    _Style = value;

                    if (_Style < 0)
                        _Style = 0;

                    if (_Style > 7)
                        _Style = 7;

                    Reset();
                }
            }
        }

        [Constructable]
        public SliderTrapTrainingKit()
            : base(41875)
        {
            _Style = -1;
            Style = Utility.Random(8);
        }

        public void Reset()
        {
            for (int i = 0; i < Order.Length; i++)
            {
                Order[i] = 0;
            }

            int randomIndex = Utility.Random(Order.Length);
            int startID = 0x9CEE + (_Style * 9); // start at 1+ because the upper left picture is alwasy omitted
            List<int> list = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                list.Add(startID + i);
            }

            for (int i = 0; i < Order.Length; i++)
            {
                if (i != randomIndex)
                {
                    int add = list[Utility.Random(list.Count)];
                    Order[i] = add;
                    list.Remove(add);
                }
            }

            int invCount = 0;
            for (int i = 0; i < Order.Length - 1; i++)
            {
                for (int j = i + 1; j < Order.Length; j++)
                {
                    if (Order[j] != 0 && Order[i] != 0 && Order[i] > Order[j])
                        invCount++;
                }
            }

            if (invCount % 2 == 1)
            {
                if (randomIndex > 2)
                {
                    int temp = Order[0];
                    Order[0] = Order[1];
                    Order[1] = temp;
                }
                else
                {
                    int temp = Order[Order.Length - 1];
                    Order[Order.Length - 1] = Order[Order.Length - 2];
                    Order[Order.Length - 2] = temp;
                }
            }
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                m.SendLocalizedMessage(1159008); // That appears to be trapped, using the remove trap skill would yield better results...
            }
            else
            {
                m.SendMessage("That is not your chest!"); // TODO: Could not find cliloc, but this is the message given
            }
        }

        public void OnRemoveTrap(Mobile from)
        {
            if (from is PlayerMobile)
            {
                BaseGump.SendGump(new SliderTrapGump((PlayerMobile)from, this));
            }
        }

        public void Complete(Mobile from)
        {
            from.SendLocalizedMessage(1159009); // You successfully disarm the trap!

            from.CheckTargetSkill(SkillName.RemoveTrap, this, 0, 100);

            Reset();
        }

        private void TestOrder()
        {
            int startID = 0x9CEE + (_Style * 9); // start at 1+ because the upper left picture is alwasy omitted
            List<int> list = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                list.Add(startID + i);
            }

            for (int i = 0; i < Order.Length; i++)
            {
                if (i != 1)
                {
                    Order[i] = list[0];
                    list.RemoveAt(0);
                }
                else
                {
                    Order[i] = 0;
                }
            }
        }

        public SliderTrapTrainingKit(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(_Style);

            for (int i = 0; i < Order.Length; i++)
            {
                writer.Write(Order[i]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            _Style = reader.ReadInt();

            for (int i = 0; i < Order.Length; i++)
            {
                Order[i] = reader.ReadInt();
            }
        }
    }

    public class SliderTrapGump : BaseGump
    {
        public ISliderKit Kit { get; set; }
        public int[] Order => Kit.Order;

        public SliderTrapGump(PlayerMobile pm, ISliderKit kit)
            : base(pm, 100, 100)
        {
            pm.CloseGump(GetType());
            Kit = kit;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 270, 445, 0x6DB);
            AddImage(15, 20, 0x9CED + (Kit.Style * 9));
            AddAlphaRegion(15, 20, 80, 133);

            for (int i = 0; i < Order.Length; i++)
            {
                int order = Order[i];

                if (order == 0)
                    continue;

                int x = i % 3 == 0 ? 15 : i % 3 == 1 ? 95 : 175;
                int y = i <= 2 ? 20 : i <= 5 ? 153 : 286;

                AddButton(x, y, order, order, i + 1, GumpButtonType.Reply, 0);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            if (!Kit.Deleted && info.ButtonID >= 1 && info.ButtonID <= 9)
            {
                int pick = info.ButtonID - 1;
                int empty = Array.IndexOf(Order, 0);

                if (ValidMove(pick, empty))
                {
                    User.SendSound(0x42);

                    int id = Order[pick];
                    Order[pick] = 0;
                    Order[empty] = id;

                    if (CheckSolution(User))
                    {
                        Kit.Complete(User);
                    }
                    else
                    {
                        Refresh();
                    }
                }
                else
                {
                    User.SendSound(0x051);
                    Refresh();
                }
            }
        }

        private bool ValidMove(int pick, int empty)
        {
            switch (pick)
            {
                case 0: return empty == 1 || empty == 3;
                case 1: return empty == 0 || empty == 2 || empty == 4;
                case 2: return empty == 1 || empty == 5;
                case 3: return empty == 0 || empty == 4 || empty == 6;
                case 4: return empty == 1 || empty == 3 || empty == 5 || empty == 7;
                case 5: return empty == 2 || empty == 4 || empty == 8;
                case 6: return empty == 3 || empty == 7;
                case 7: return empty == 4 || empty == 6 || empty == 8;
                case 8: return empty == 5 || empty == 7;
            }

            return false;
        }

        private bool CheckSolution(Mobile m)
        {
            int start = 0x9CEE + (Kit.Style * 9);

            return Order[0] == 0 &&
                Order[1] == start &&
                Order[2] == start + 1 &&
                Order[3] == start + 2 &&
                Order[4] == start + 3 &&
                Order[5] == start + 4 &&
                Order[6] == start + 5 &&
                Order[7] == start + 6 &&
                Order[8] == start + 7;
        }
    }
}
