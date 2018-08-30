using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

namespace Server.Items.Crops
{
    public class MandrakeSeed : BaseCrop
    {
        // return true to allow planting on Dirt Item (ItemID 0x32C9)
        // See CropHelper.cs for other overriddable types
        public override bool CanGrowGarden { get { return true; } }

        [Constructable]
        public MandrakeSeed()
            : this(1)
        {
        }

        [Constructable]
        public MandrakeSeed(int amount)
            : base(0xDCF)
        {
            this.Name = "mandrake seed";
            Stackable = true;
            Weight = .5;
            Hue = 1140;
            Movable = true;
            Amount = amount;
        }

        public override void OnSingleClick(Mobile from)
        {
            from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, 0x3B2, 3, "", String.Format("{0} small brown seed{1}", Amount > 1 ? Amount.ToString() : "a", Amount > 1 ? "s" : "")));
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (from.Mounted && !CropHelper.CanWorkMounted)
            {
                from.SendMessage("You cannot plant a seed while mounted.");
                return;
            }

            Point3D m_pnt = from.Location;
            Map m_map = from.Map;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042010); //You must have the object in your backpack to use it. 
                return;
            }

            else if (!CropHelper.CheckCanGrow(this, m_map, m_pnt.X, m_pnt.Y))
            {
                from.SendMessage("This plant will not be able to grow here.");
                return;
            }

            //check for BaseCrop on this tile
            ArrayList cropshere = CropHelper.CheckCrop(m_pnt, m_map, 0);
            if (cropshere.Count > 0)
            {
                from.SendMessage("There is already a plant growing here.");
                return;
            }

            //check for over planting prohibt if 4 maybe 3 neighboring crops
            /*ArrayList cropsnear = CropHelper.CheckCrop( m_pnt, m_map, 1 );
            if ( ( cropsnear.Count > 3 ) || (( cropsnear.Count == 3 ) && Utility.RandomBool() ) )
            {
                from.SendMessage( "There are too many crops nearby." ); 
                return;
            }*/

            if (this.BumpZ) ++m_pnt.Z;

            if (!from.Mounted)
                from.Animate(32, 5, 1, true, false, 0); // Bow

            from.SendMessage("You plant the seed.");
            this.Consume();
            Item item = new MandrakeSeedling(from);
            item.Location = m_pnt;
            item.Map = m_map;

        }

        public MandrakeSeed(Serial serial)
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
    }


    public class MandrakeSeedling : BaseCrop
    {
        private static Mobile m_sower;
        public Timer thisTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Sower { get { return m_sower; } set { m_sower = value; } }

        [Constructable]
        public MandrakeSeedling(Mobile sower)
            : base(0xC68)
        {
            Movable = false;
            Name = "a mandrake seedling";
            m_sower = sower;

            init(this);
        }

        public static void init(MandrakeSeedling plant)
        {
            plant.thisTimer = new CropHelper.GrowTimer(plant, typeof(MandrakeCrop), plant.Sower);
            plant.thisTimer.Start();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.Mounted && !CropHelper.CanWorkMounted)
            {
                from.SendMessage("The plant is too small to harvest while mounted.");
                return;
            }

            if ((Utility.RandomDouble() <= .25) && !(m_sower.AccessLevel > AccessLevel.Counselor))
            { //25% Chance
                from.SendMessage("You accidently uproot the seedling.");
                thisTimer.Stop();
                this.Delete();
            }
            else from.SendMessage("You can't seem to find anything to gather.");
        }

        public MandrakeSeedling(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_sower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_sower = reader.ReadMobile();

            init(this);
        }
    }

    public class MandrakeCrop : BaseCrop
    {
        private const int max = 4;
        private int fullGraphic;
        private int pickedGraphic;
        private DateTime lastpicked;

        private Mobile m_sower;
        private int m_yield;

        public Timer regrowTimer;

        private DateTime m_lastvisit;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastSowerVisit { get { return m_lastvisit; } }

        [CommandProperty(AccessLevel.GameMaster)] // debuging
        public bool Growing { get { return regrowTimer.Running; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Sower { get { return m_sower; } set { m_sower = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Yield { get { return m_yield; } set { m_yield = value; } }

        public int Capacity { get { return max; } }
        public int FullGraphic { get { return fullGraphic; } set { fullGraphic = value; } }
        public int PickGraphic { get { return pickedGraphic; } set { pickedGraphic = value; } }
        public DateTime LastPick { get { return lastpicked; } set { lastpicked = value; } }

        [Constructable]
        public MandrakeCrop(Mobile sower)
            : base(0xC69)
        {
            Movable = false;
            Name = "a mandrake plant";

            m_sower = sower;
            m_lastvisit = DateTime.Now;

            init(this, false);
        }

        public static void init(MandrakeCrop plant, bool full)
        {
            plant.PickGraphic = (0xC69);
            plant.FullGraphic = (0x18DF);

            plant.LastPick = DateTime.Now;
            plant.regrowTimer = new CropTimer(plant);

            if (full)
            {
                plant.Yield = plant.Capacity;
                ((Item)plant).ItemID = plant.FullGraphic;
            }
            else
            {
                plant.Yield = 0;
                ((Item)plant).ItemID = plant.PickGraphic;
                plant.regrowTimer.Start();
            }
        }

        public void UpRoot(Mobile from)
        {
            from.SendMessage("The mandrake plant withers away.");
            if (regrowTimer.Running)
                regrowTimer.Stop();

            this.Delete();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_sower == null || m_sower.Deleted)
                m_sower = from;

            if (from.Mounted && !CropHelper.CanWorkMounted)
            {
                from.SendMessage("You cannot gather anything while mounted.");
                return;
            }

            if (DateTime.Now > lastpicked.AddSeconds(3)) // 3 seconds between picking
            {
                lastpicked = DateTime.Now;

                int mageValue = (int)from.Skills[SkillName.Magery].Value / 10;  //FUTURE add two skill checks...
                if (mageValue == 0)
                {
                    from.SendMessage("You have no idea how to pick this.");
                    return;
                }

                if (from.InRange(this.GetWorldLocation(), 1))
                {
                    if (m_yield < 1)
                    {
                        from.SendMessage("There is nothing here to harvest.");

                        if (PlayerCanDestroy && !(m_sower.AccessLevel > AccessLevel.Counselor))
                        {
                            UpRootGump g = new UpRootGump(from, this);
                            from.SendGump(g);
                        }
                    }
                    else //check skill and sower
                    {
                        from.Direction = from.GetDirectionTo(this);

                        from.Animate(from.Mounted ? 29 : 32, 5, 1, true, false, 0);

                        if (from == m_sower)
                        {
                            mageValue *= 2;
                            m_lastvisit = DateTime.Now;
                        }

                        if (mageValue > m_yield)
                            mageValue = m_yield + 1;

                        int pick = Utility.Random(mageValue);
                        if (pick == 0)
                        {
                            from.SendMessage("You do not manage to harvest anything.");
                            return;
                        }

                        m_yield -= pick;
                        from.SendMessage("You manage to gather {0} mandrake!", pick);

                        double chance = Utility.RandomDouble(); // chance to get a seed is 20%
                        if (0.20 > chance)
                        {
                            from.AddToBackpack(new MandrakeSeed(1));
                            from.SendMessage("You found a seed!");
                        }

                        //PublicOverheadMessage( MessageType.Regular, 0x3BD, false, string.Format( "{0}", m_yield )); 
                        ((Item)this).ItemID = pickedGraphic;

                        MandrakeRoot crop = new MandrakeRoot(pick);
                        from.AddToBackpack(crop);

                        chance = Utility.RandomDouble(); // chance to pull out plant is 10%
                        if ((0.10 > chance) && !(m_sower.AccessLevel > AccessLevel.Counselor))
                        { 
                            from.SendMessage("You accidently uproot all of the mandrake plant.");
                            regrowTimer.Stop();
                            this.Delete();
                            return;
                        }

                        if (SowerPickTime != TimeSpan.Zero && m_lastvisit + SowerPickTime < DateTime.Now && !(m_sower.AccessLevel > AccessLevel.Counselor))
                        {
                            this.UpRoot(from);
                            return;
                        }

                        if (!regrowTimer.Running)
                        {
                            regrowTimer.Start();
                        }
                    }
                }
                else
                {
                    from.SendMessage("You are too far away to gather anything.");
                }
            }
        }

        private class CropTimer : Timer
        {
            private MandrakeCrop i_plant;

            public CropTimer(MandrakeCrop plant)
                : base(TimeSpan.FromMinutes(45), TimeSpan.FromMinutes(30))
            {
                Priority = TimerPriority.OneSecond;
                i_plant = plant;
            }

            protected override void OnTick()
            {
                if ((i_plant != null) && (!i_plant.Deleted))
                {
                    int current = i_plant.Yield;

                    if (++current >= i_plant.Capacity)
                    {
                        current = i_plant.Capacity;
                        ((Item)i_plant).ItemID = i_plant.FullGraphic;
                        Stop();
                    }
                    else if (current <= 0)
                        current = 1;

                    i_plant.Yield = current;
                    //i_plant.PublicOverheadMessage( MessageType.Regular, 0x22, false, string.Format( "{0}", current )); 
                }
                else Stop();
            }
        }

        public MandrakeCrop(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            writer.Write(m_lastvisit);
            writer.Write(m_sower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 1:
                    {
                        m_lastvisit = reader.ReadDateTime();
                        goto case 0;
                    }
                case 0:
                    {
                        m_sower = reader.ReadMobile();
                        break;
                    }
            }

            if (version == 0)
                m_lastvisit = DateTime.Now;

            init(this, true);
        }
    }
}
