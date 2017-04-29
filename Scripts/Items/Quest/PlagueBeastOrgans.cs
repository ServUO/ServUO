using System;
using System.Collections.Generic;
using Server.Network;

namespace Server.Items
{
    public class PlagueBeastOrgan : PlagueBeastInnard
    {
        private List<PlagueBeastComponent> m_Components;
        private int m_BrainHue;
        private bool m_Opened;
        private Timer m_Timer;
        public PlagueBeastOrgan()
            : this(1, 0)
        {
            this.Visible = false;
        }

        public PlagueBeastOrgan(int itemID, int hue)
            : base(itemID, hue)
        {
            this.m_Components = new List<PlagueBeastComponent>();
            this.m_Opened = false;

            this.Movable = false;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Initialize));
        }

        public PlagueBeastOrgan(Serial serial)
            : base(serial)
        {
        }

        public virtual bool IsCuttable
        {
            get
            {
                return false;
            }
        }
        public List<PlagueBeastComponent> Components
        {
            get
            {
                return this.m_Components;
            }
        }
        public int BrainHue
        {
            get
            {
                return this.m_BrainHue;
            }
            set
            {
                this.m_BrainHue = value;
            }
        }
        public bool Opened
        {
            get
            {
                return this.m_Opened;
            }
            set
            {
                this.m_Opened = value;
            }
        }
        public virtual void Initialize()
        {
        }

        public void AddComponent(PlagueBeastComponent c, int x, int y)
        {
            Container pack = this.Parent as Container;

            if (pack != null)
                pack.DropItem(c);

            c.Organ = this;
            c.Location = new Point3D(this.X + x, this.Y + y, this.Z);
            c.Map = this.Map;

            this.m_Components.Add(c);
        }

        public override bool Scissor(Mobile from, Scissors scissors)
        {
            if (this.IsCuttable && this.IsAccessibleTo(from))
            {
                if (!this.m_Opened && this.m_Timer == null)
                {
                    this.m_Timer = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(3), new TimerStateCallback<Mobile>(FinishOpening), from);
                    scissors.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071897); // You carefully cut into the organ.
                    return true;
                }
                else
                    scissors.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071898); // You have already cut this organ open.
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public virtual bool OnLifted(Mobile from, PlagueBeastComponent c)
        {
            return c.IsGland || c.IsBrain;
        }

        public virtual bool OnDropped(Mobile from, Item item, PlagueBeastComponent to)
        {
            return false;
        }

        public virtual void FinishOpening(Mobile from)
        {
            this.m_Opened = true;

            if (this.Owner != null)
                this.Owner.PlaySound(0x50);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteItemList<PlagueBeastComponent>(this.m_Components);
            writer.Write((int)this.m_BrainHue);
            writer.Write((bool)this.m_Opened);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Components = reader.ReadStrongItemList<PlagueBeastComponent>();
            this.m_BrainHue = reader.ReadInt();
            this.m_Opened = reader.ReadBool();
        }
    }

    public class PlagueBeastMaidenOrgan : PlagueBeastOrgan
    {
        public PlagueBeastMaidenOrgan()
            : base(0x124D, 0x0)
        {
        }

        public PlagueBeastMaidenOrgan(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.Opened)
                this.FinishOpening(from);
        }

        public override void FinishOpening(Mobile from)
        {
            this.ItemID = 0x1249;

            if (this.Owner != null)
                this.Owner.PlaySound(0x187);
			
            this.AddComponent(new PlagueBeastComponent(0x1D0D, 0x0), 22, 3);
            this.AddComponent(new PlagueBeastComponent(0x1D12, 0x0), 15, 18);
            this.AddComponent(new PlagueBeastComponent(0x1DA3, 0x21), 26, 46);

            if (this.BrainHue > 0)
                this.AddComponent(new PlagueBeastComponent(0x1CF0, this.BrainHue, true), 22, 29);

            this.Opened = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PlagueBeastRockOrgan : PlagueBeastOrgan
    {
        public PlagueBeastRockOrgan()
            : base(0x177A, 0x60)
        {
        }

        public PlagueBeastRockOrgan(Serial serial)
            : base(serial)
        {
        }

        public override bool IsCuttable
        {
            get
            {
                return true;
            }
        }

        public override bool Carve(Mobile from, Item with)
        {
            if (this.IsAccessibleTo(from))
                with.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071896); // This is too crude an implement for such a procedure. 

            return false;
        }

        public override bool OnLifted(Mobile from, PlagueBeastComponent c)
        {
            base.OnLifted(from, c);

            if (c.IsBrain)
            {
                this.AddComponent(new PlagueBeastBlood(), -7, 24);
                return true;
            }

            return false;
        }

        public override void FinishOpening(Mobile from)
        {
            base.FinishOpening(from);

            this.AddComponent(new PlagueBeastComponent(0x1775, 0x60), 3, 5);
            this.AddComponent(new PlagueBeastComponent(0x1777, 0x1), 10, 14);

            if (this.BrainHue > 0)
                this.AddComponent(new PlagueBeastComponent(0x1CF0, this.BrainHue, true), 1, 24); // 22, 29 
            else
                this.AddComponent(new PlagueBeastBlood(), -7, 24);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class PlagueBeastRubbleOrgan : PlagueBeastOrgan
    {
        private static readonly int[] m_Hues = new int[]
        {
            0xD, 0x17, 0x2B, 0x42, 0x54, 0x5D
        };
        private int m_Veins;
        public PlagueBeastRubbleOrgan()
            : base()
        {
            this.m_Veins = 3;
        }

        public PlagueBeastRubbleOrgan(Serial serial)
            : base(serial)
        {
        }

        public override void Initialize()
        {
            this.Hue = Utility.RandomList(m_Hues);

            this.AddComponent(new PlagueBeastComponent(0x3BB, this.Hue), 0, 0);
            this.AddComponent(new PlagueBeastComponent(0x3BA, this.Hue), 4, 6);
            this.AddComponent(new PlagueBeastComponent(0x3BA, this.Hue), -6, 17);

            int v = Utility.Random(4);

            this.AddComponent(new PlagueBeastVein(0x1B1B, v == 0 ? this.Hue : RandomHue(this.Hue)), -23, -3);
            this.AddComponent(new PlagueBeastVein(0x1B1C, v == 1 ? this.Hue : RandomHue(this.Hue)), 19, 4);
            this.AddComponent(new PlagueBeastVein(0x1B1B, v == 2 ? this.Hue : RandomHue(this.Hue)), 21, 27);
            this.AddComponent(new PlagueBeastVein(0x1B1B, v == 3 ? this.Hue : RandomHue(this.Hue)), 10, 40);
        }

        public override bool OnLifted(Mobile from, PlagueBeastComponent c)
        {
            if (c.IsBrain)
            {
                this.AddComponent(new PlagueBeastBlood(), -13, 25);
                return true;
            }

            return false;
        }

        public override void FinishOpening(Mobile from)
        {
            this.AddComponent(new PlagueBeastComponent(0x1777, 0x1), 5, 14);

            if (this.BrainHue > 0)
                this.AddComponent(new PlagueBeastComponent(0x1CF0, this.BrainHue, true), -5, 22);
            else
                this.AddComponent(new PlagueBeastBlood(), -13, 25);

            this.Opened = true;
        }

        public virtual void OnVeinCut(Mobile from, PlagueBeastVein vein)
        {
            if (vein.Hue != this.Hue)
            {
                if (!this.Opened && this.m_Veins > 0 && --this.m_Veins == 0)
                    this.FinishOpening(from);
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1071901); // * As you cut the vein, a cloud of poison is expelled from the plague beast's organ, and the plague beast dissolves into a puddle of goo *
                from.ApplyPoison(from, Poison.Greater);
                from.PlaySound(0x22F);

                if (this.Owner != null)
                {
                    this.Owner.Unfreeze();
                    this.Owner.Kill();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_Veins);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Veins = reader.ReadInt();
        }

        private static int RandomHue(int exculde)
        {
            for (int i = 0; i < 20; i++)
            {
                int hue = Utility.RandomList(m_Hues);

                if (hue != exculde)
                    return hue;
            }

            return 0xD;
        }
    }

    public class PlagueBeastBackupOrgan : PlagueBeastOrgan
    {
        private Timer m_Timer;
        private Item m_Gland;
        public PlagueBeastBackupOrgan()
            : base(0x1362, 0x6)
        {
        }

        public PlagueBeastBackupOrgan(Serial serial)
            : base(serial)
        {
        }

        public override bool IsCuttable
        {
            get
            {
                return true;
            }
        }
        public override void Initialize()
        {
            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x42), 16, 39);
            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x42), 39, 49);
            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x42), 39, 48);
            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x42), 44, 42);
            this.AddComponent(new PlagueBeastComponent(0x1CF2, 0x42), 20, 34);
            this.AddComponent(new PlagueBeastComponent(0x135F, 0x42), 47, 58);
            this.AddComponent(new PlagueBeastComponent(0x1360, 0x42), 70, 68);
        }

        public override bool Carve(Mobile from, Item with)
        {
            if (this.IsAccessibleTo(from))
                with.PublicOverheadMessage(MessageType.Regular, 0x3B2, 1071896); // This is too crude an implement for such a procedure. 

            return false;
        }

        public override bool OnLifted(Mobile from, PlagueBeastComponent c)
        {
            if (c.IsBrain)
            {
                this.AddComponent(new PlagueBeastBlood(), 47, 72);
                return true;
            }
            else if (c.IsGland)
            {
                this.m_Gland = null;
                return true;
            }

            return c.IsGland;
        }

        public override bool OnDropped(Mobile from, Item item, PlagueBeastComponent to)
        {
            if (to.Hue == 0x1 && this.m_Gland == null && item is PlagueBeastGland)
            {
                this.m_Gland = item;
                this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(3), new TimerCallback(FinishHealing));
                from.SendAsciiMessage(0x3B2, "* You place the healthy gland inside the organ sac *");
                item.Movable = false;

                if (this.Owner != null)
                    this.Owner.PlaySound(0x20);

                return true;
            }

            return false;
        }

        public override void FinishOpening(Mobile from)
        {
            base.FinishOpening(from);

            this.AddComponent(new PlagueBeastComponent(0x1363, 0xF), -3, 3);
            this.AddComponent(new PlagueBeastComponent(0x1365, 0x1), -3, 10);

            this.m_Gland = new PlagueBeastComponent(0x1CEF, 0x3F, true);
            this.AddComponent((PlagueBeastComponent)this.m_Gland, -4, 16);
        }

        public void FinishHealing()
        {
            for (int i = 0; i < 7 && i < this.Components.Count; i++)
                this.Components[i].Hue = 0x6;

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(2), new TimerCallback(OpenOrgan));
        }

        public void OpenOrgan()
        {
            this.AddComponent(new PlagueBeastComponent(0x1367, 0xF), 55, 61);
            this.AddComponent(new PlagueBeastComponent(0x1366, 0x1), 57, 66);

            if (this.BrainHue > 0)
                this.AddComponent(new PlagueBeastComponent(0x1CF0, this.BrainHue, true), 55, 69);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((Item)this.m_Gland);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Gland = reader.ReadItem();
        }
    }

    public class PlagueBeastMainOrgan : PlagueBeastOrgan
    {
        private int m_Brains;
        public PlagueBeastMainOrgan()
            : base()
        {
            this.m_Brains = 0;
        }

        public PlagueBeastMainOrgan(Serial serial)
            : base(serial)
        {
        }

        public bool Complete
        {
            get
            {
                return this.m_Brains >= 4;
            }
        }
        public override void Initialize()
        {
            // receptacles
            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x42), -36, -2);
            this.AddComponent(new PlagueBeastComponent(0x1FB3, 0x42), -42, 0);
            this.AddComponent(new PlagueBeastComponent(0x9DF, 0x42), -53, -7);

            this.AddComponent(new PlagueBeastComponent(0x1B1C, 0x54), 29, 9);
            this.AddComponent(new PlagueBeastComponent(0x1D06, 0x54), 18, -2);
            this.AddComponent(new PlagueBeastComponent(0x9DF, 0x54), 36, -1);

            this.AddComponent(new PlagueBeastComponent(0x1D10, 0x2B), -36, 47);
            this.AddComponent(new PlagueBeastComponent(0x1B1C, 0x2B), -24, 62);
            this.AddComponent(new PlagueBeastComponent(0x9DF, 0x2B), -41, 74);

            this.AddComponent(new PlagueBeastComponent(0x1B1B, 0x60), 39, 56);
            this.AddComponent(new PlagueBeastComponent(0x1FB4, 0x60), 34, 52);
            this.AddComponent(new PlagueBeastComponent(0x9DF, 0x60), 45, 71);

            // main part
            this.AddComponent(new PlagueBeastComponent(0x1351, 0x15), 23, 0);
            this.AddComponent(new PlagueBeastComponent(0x134F, 0x15), -22, 0);
            this.AddComponent(new PlagueBeastComponent(0x1350, 0x15), 0, 0);
        }

        public override bool OnLifted(Mobile from, PlagueBeastComponent c)
        {
            if (c.IsBrain)
                this.m_Brains--;

            return true;
        }

        public override bool OnDropped(Mobile from, Item item, PlagueBeastComponent to)
        {
            if (!this.Opened && to.IsReceptacle && item.Hue == to.Hue)
            {
                to.Organ = this;
                this.m_Brains++;
                from.LocalOverheadMessage(MessageType.Regular, 0x34, 1071913); // You place the organ in the fleshy receptacle near the core.

                if (this.Owner != null)
                {
                    this.Owner.PlaySound(0x1BA);

                    if (this.Owner.IsBleeding)
                    {
                        from.LocalOverheadMessage(MessageType.Regular, 0x34, 1071922); // The plague beast is still bleeding from open wounds.  You must seal any bleeding wounds before the core will open!
                        return true;
                    }
                }
				
                if (this.m_Brains == 4)
                    this.FinishOpening(from);

                return true;
            }

            return false;
        }

        public override void FinishOpening(Mobile from)
        {
            this.AddComponent(new PlagueBeastComponent(0x1363, 0x1), 0, 22);
            this.AddComponent(new PlagueBeastComponent(0x1D04, 0xD), 0, 22);

            if (this.Owner != null && this.Owner.Backpack != null)
            {
                PlagueBeastMutationCore core = new PlagueBeastMutationCore();
                this.Owner.Backpack.AddItem(core);
                core.Movable = false;
                core.Cut = false;
                core.X = this.X;
                core.Y = this.Y + 34;

                this.Owner.PlaySound(0x21);
                this.Owner.PlaySound(0x166);
            }

            this.Opened = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write((int)this.m_Brains);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Brains = reader.ReadInt();
        }
    }
}