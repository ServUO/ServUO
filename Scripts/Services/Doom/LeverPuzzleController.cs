using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

/*
this is From me to you, Under no terms, Conditions...   K?  to apply you
just simply Unpatch/delete, Stick these in, Same location.. Restart
*/
namespace Server.Engines.Doom
{
    public class LeverPuzzleController : Item
    {
        public static string[] Msgs = 
        {
            "You are pinned down by the weight of the boulder!!!", // 0
            "A speeding rock hits you in the head!", // 1
            "OUCH!"							// 2
        };
        /* font&hue for above msgs. index matches */
        public static int[][] MsgParams = 
        {
            new int[] { 0x66d, 3 },
            new int[] { 0x66d, 3 },
            new int[] { 0x34, 3 }
        };
        /* World data for items */
        public static int[][] TA = 
        {
            new int[] { 316, 64, 5 },
            /* 3D Coords for levers */ new int[] { 323, 58, 5 },
            new int[] { 332, 63, 5 },
            new int[] { 323, 71, 5 },
            new int[] { 324, 64 },
            /* 2D Coords for standing regions */ new int[] { 316, 65 },
            new int[] { 324, 58 },
            new int[] { 332, 64 },
            new int[] { 323, 72 },
            new int[] { 468, 92, -1 }, new int[] { 0x181D, 0x482 },
            /* 3D coord, itemid+hue for L.R. teles */ new int[] { 469, 92, -1 }, new int[] { 0x1821, 0x3fd },
            new int[] { 470, 92, -1 }, new int[] { 0x1825, 0x66d },
            new int[] { 319, 70, 18 }, new int[] { 0x12d8 },
            /* 3D coord, itemid for statues */ new int[] { 329, 60, 18 }, new int[] { 0x12d9 },
            new int[] { 469, 96, 6 }
        /* 3D Coords for Fake Box */ };
        /* CLILOC data for statue "correct souls" messages */
        public static int[] Statue_Msg = { 1050009, 1050007, 1050008, 1050008 };
        /* Exit & Enter locations for the lamp room */
        public static Point3D lr_Exit = new Point3D(353, 172, -1);
        public static Point3D lr_Enter = new Point3D(467, 96, -1);
        /* "Center" location in puzzle */
        public static Point3D lp_Center = new Point3D(324, 64, -1);
        /* Lamp Room Area */
        public static Rectangle2D lr_Rect = new Rectangle2D(465, 92, 10, 10);
        /* Lamp Room area Poison message data */
        public static int[][] PA = 
        {
            new int[] { 0, 0, 0xA6 },
            new int[] { 1050001, 0x485, 0xAA },
            new int[] { 1050003, 0x485, 0xAC },
            new int[] { 1050056, 0x485, 0xA8 },
            new int[] { 1050057, 0x485, 0xA4 },
            new int[] { 1062091, 0x23F3, 0xAC }
        };
        public static Poison[] PA2 = 
        {
            Poison.Lesser,
            Poison.Regular,
            Poison.Greater,
            Poison.Deadly,
            Poison.Lethal,
            Poison.Lethal
        };
        /* SOUNDS */
        private static readonly int[] fs = { 0x144, 0x154 };
        private static readonly int[] ms = { 0x144, 0x14B };
        private static readonly int[] fs2 = { 0x13F, 0x154 };
        private static readonly int[] ms2 = { 0x13F, 0x14B };
        private static readonly int[] cs1 = { 0x244 };
        private static readonly int[] exp = { 0x307 };
        private static bool installed;
        private bool m_Enabled;
        private UInt16 m_MyKey;
        private UInt16 m_TheirKey;
        private List<Item> m_Levers;
        private List<Item> m_Teles;
        private List<Item> m_Statues;
        private List<LeverPuzzleRegion> m_Tiles;
        private Mobile m_Successful;
        private LampRoomBox m_Box;
        private Region m_LampRoom;
        private Timer m_Timer;
        private Timer l_Timer;
        public LeverPuzzleController()
            : base(0x1822)
        {
            this.Movable = false;
            this.Hue = 0x4c;
            installed = true;
            int i = 0;

            this.m_Levers = new List<Item>();	/* codes are 0x1 shifted left x # of bits, easily handled here */
            for (; i < 4; i++)
                this.m_Levers.Add(AddLeverPuzzlePart(TA[i], new LeverPuzzleLever((ushort)(1 << i), this)));

            this.m_Tiles = new List<LeverPuzzleRegion>();
            for (; i < 9; i++)
                this.m_Tiles.Add(new LeverPuzzleRegion(this, TA[i]));

            this.m_Teles = new List<Item>();
            for (; i < 15; i++)
                this.m_Teles.Add(AddLeverPuzzlePart(TA[i], new LampRoomTeleporter(TA[++i])));

            this.m_Statues = new List<Item>();
            for (; i < 19; i++)
                this.m_Statues.Add(AddLeverPuzzlePart(TA[i], new LeverPuzzleStatue(TA[++i], this)));

            if (!installed)
                this.Delete();
            else
                this.Enabled = true;

            this.m_Box = (LampRoomBox)AddLeverPuzzlePart(TA[i], new LampRoomBox(this));
            this.m_LampRoom = new LampRoomRegion(this);
            this.GenKey();
        }

        public LeverPuzzleController(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public UInt16 MyKey
        {
            get
            {
                return this.m_MyKey;
            }
            set
            {
                this.m_MyKey = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public UInt16 TheirKey
        {
            get
            {
                return this.m_TheirKey;
            }
            set
            {
                this.m_TheirKey = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Enabled
        {
            get
            {
                return this.m_Enabled;
            }
            set
            {
                this.m_Enabled = value;
            }
        }
        public Mobile Successful
        {
            get
            {
                return this.m_Successful;
            }
        }
        public bool CircleComplete
        {
            get	/* OSI: all 5 must be occupied */
            {
                for (int i = 0; i < 5; i++)
                {
                    if (this.GetOccupant(i) == null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        public static void Initialize()
        {
			CommandSystem.Register("GenLeverPuzzle", AccessLevel.Administrator, new CommandEventHandler(GenLampPuzzle_OnCommand));
			CommandSystem.Register("LampPuzzleDelete", AccessLevel.Administrator, new CommandEventHandler(LampPuzzleDelete_OnCommand));
		}

		[Usage("LampPuzzleDelete")]
		[Description("Deletes lamp room and lever puzzle in doom.")]
		public static void LampPuzzleDelete_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("LeverPuzzleController");
			e.Mobile.SendMessage("Lamp room puzzle successfully deleted.");
		}


		[Usage("GenLeverPuzzle")]
        [Description("Generates lamp room and lever puzzle in doom.")]
        public static void GenLampPuzzle_OnCommand(CommandEventArgs e)
        {
            foreach (Item item in Map.Malas.GetItemsInRange(lp_Center, 0))
            {
                if (item is LeverPuzzleController)
                {
                    e.Mobile.SendMessage("Lamp room puzzle already exists: please delete the existing controller first ...");
                    return;
                }
            }
            e.Mobile.SendMessage("Generating Lamp Room puzzle...");
			LeverPuzzleController controller = new LeverPuzzleController();
			WeakEntityCollection.Add("LeverPuzzleController", controller);
			controller.MoveToWorld(lp_Center, Map.Malas);

            if (!installed)
                e.Mobile.SendMessage("There was a problem generating the puzzle.");
            else
                e.Mobile.SendMessage("Lamp room puzzle successfully generated.");
        }

        public static Item AddLeverPuzzlePart(int[] Loc, Item newitem)
        {
            if (newitem == null || newitem.Deleted)
            {
                installed = false;
            }
            else
            {
                newitem.MoveToWorld(new Point3D(Loc[0], Loc[1], Loc[2]), Map.Malas);
            }
            return newitem;
        }

        public static void NukeItemList(List<Item> list)
        {
            if (list != null && list.Count != 0)
            {
                foreach (Item item in list)
                {
                    if (item != null && !item.Deleted)
                    {
                        item.Delete();
                    }
                }
            }
        }

        public static void MoveMobileOut(Mobile m)
        {
            if (m != null)
            {
                if (m is PlayerMobile && !m.Alive)
                {
                    if (m.Corpse != null && !m.Corpse.Deleted)
                    {
                        m.Corpse.MoveToWorld(lr_Exit, Map.Malas);
                    }
                }
                BaseCreature.TeleportPets(m, lr_Exit, Map.Malas);
                m.Location = lr_Exit;
                m.ProcessDelta();
            }
        }

        public static bool AniSafe(Mobile m)
        {
            return (m != null && !TransformationSpellHelper.UnderTransformation(m) && m.BodyMod == 0 && m.Alive);
        }

        public static IEntity ZAdjustedIEFromMobile(Mobile m, int ZDelta)
        {
            return new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + ZDelta), m.Map);
        }

        public static void DoDamage(Mobile m, int min, int max, bool poison)
        {
            if (m != null && !m.Deleted && m.Alive)
            {
                int damage = Utility.Random(min, max);
                AOS.Damage(m, damage, (poison) ? 0 : 100, 0, 0, (poison) ? 100 : 0, 0);
            }
        }

        public static Point3D RandomPointIn(Point3D point, int range)
        {
            return RandomPointIn(point.X - range, point.Y - range, range * 2, range * 2, point.Z);
        }

        public static Point3D RandomPointIn(Rectangle2D rect, int z)
        {
            return RandomPointIn(rect.X, rect.Y, rect.Height, rect.Width, z);
        }

        public static Point3D RandomPointIn(int x, int y, int x2, int y2, int z)
        {
            return new Point3D(Utility.Random(x, x2), Utility.Random(y, y2), z);
        }

        public static void PlaySounds(Point3D location, int[] sounds)
        {
            foreach (int soundid in sounds)
                Effects.PlaySound(location, Map.Malas, soundid);
        }

        public static void PlayEffect(IEntity from, IEntity to, int itemid, int speed, bool explodes)
        {
            Effects.SendMovingParticles(from, to, itemid, speed, 0, true, explodes, 2, 0, 0);
        }

        public static void SendLocationEffect(IPoint3D p, int itemID, int speed, int duration, int hue)
        {
            Effects.SendPacket(p, Map.Malas, new LocationEffect(p, itemID, speed, duration, hue, 0));
        }

        public static void PlayerSendASCII(Mobile player, int index)
        {
            player.Send(new AsciiMessage(Serial.MinusOne, 0xFFFF, MessageType.Label, MsgParams[index][0], MsgParams[index][1], null, Msgs[index]));
        }

        /* I cant find any better way to send "speech" using fonts other than default */
        public static void POHMessage(Mobile from, int index) 
        {
            Packet p = new AsciiMessage(from.Serial, from.Body, MessageType.Regular, MsgParams[index][0], MsgParams[index][1], from.Name, Msgs[index]);
            p.Acquire();
            foreach (NetState state in from.Map.GetClientsInRange(from.Location))
                state.Send(p);

            Packet.Release(p);
        }

        public override void OnDelete()
        {
            this.KillTimers();
            base.OnDelete();
        }

        public override void OnAfterDelete()
        {
            NukeItemList(this.m_Teles);
            NukeItemList(this.m_Statues);
            NukeItemList(this.m_Levers);

            if (this.m_LampRoom != null)
            {
                this.m_LampRoom.Unregister();
            }
            if (this.m_Tiles != null)
            {
                foreach (Region region in this.m_Tiles)
                {
                    region.Unregister();
                }
            }
            if (this.m_Box != null && !this.m_Box.Deleted)
            {
                this.m_Box.Delete();
            }
        }

        public virtual PlayerMobile GetOccupant(int index)
        {
            LeverPuzzleRegion region = (LeverPuzzleRegion)this.m_Tiles[index];

            if (region != null)
            {
                if (region.Occupant != null && region.Occupant.Alive)
                {
                    return (PlayerMobile)region.Occupant;
                }
            }
            return null;
        }

        public virtual LeverPuzzleStatue GetStatue(int index)
        {
            LeverPuzzleStatue statue = (LeverPuzzleStatue)this.m_Statues[index];

            if (statue != null && !statue.Deleted)
            {
                return statue;
            }
            return null;
        }

        public virtual LeverPuzzleLever GetLever(int index)
        {
            LeverPuzzleLever lever = (LeverPuzzleLever)this.m_Levers[index];

            if (lever != null && !lever.Deleted)
            {
                return lever;
            }
            return null;
        }

        public virtual void PuzzleStatus(int message, string fstring)
        {
            for (int i = 0; i < 2; i++)
            {
                Item s;
                if ((s = this.GetStatue(i)) != null)
                {
                    s.PublicOverheadMessage(MessageType.Regular, 0x3B2, message, fstring);
                }
            }
        }

        public virtual void ResetPuzzle()
        {
            this.PuzzleStatus(1062053, null);
            this.ResetLevers();
        }

        public virtual void ResetLevers()
        {
            for (int i = 0; i < 4; i++)
            {
                Item l;
                if ((l = this.GetLever(i)) != null)
                {
                    l.ItemID = 0x108E;
                    Effects.PlaySound(l.Location, this.Map, 0x3E8);
                }
            }
            this.TheirKey ^= this.TheirKey;
        }

        public virtual void KillTimers()
        {
            if (this.l_Timer != null && this.l_Timer.Running)
            {
                this.l_Timer.Stop();
            }
            if (this.m_Timer != null && this.m_Timer.Running)
            {
                this.m_Timer.Stop();
            }
        }

        public virtual void RemoveSuccessful()
        {
            this.m_Successful = null;
        }

        public virtual void LeverPulled(UInt16 code)
        {
            int Correct = 0;
            Mobile m_Player;

            this.KillTimers();

            /* if one bit in each of the four nibbles is set, this is false */

            if ((this.TheirKey = (ushort)(code | (this.TheirKey <<= 4))) < 0x0FFF)
            {
                this.l_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), new TimerCallback(ResetPuzzle));
                return;
            }

            if (!this.CircleComplete)
            {
                this.PuzzleStatus(1050004, null); // The circle is the key...
            }
            else
            {
                if (this.TheirKey == this.MyKey)
                {
                    this.GenKey();
                    if ((this.m_Successful = (m_Player = this.GetOccupant(0))) != null)
                    {
                        SendLocationEffect(lp_Center, 0x1153, 0, 60, 1); 
                        PlaySounds(lp_Center, cs1);

                        Effects.SendBoltEffect(m_Player, true);
                        m_Player.MoveToWorld(lr_Enter, Map.Malas);

                        this.m_Timer = new LampRoomTimer(this);
                        this.m_Timer.Start();
                        this.m_Enabled = false;
                    }
                }
                else
                {
                    for (int i = 0; i < 16; i++)  /* Count matching SET bits, ie correct codes */
                    {
                        if ((((this.MyKey >> i) & 1) == 1) && (((this.TheirKey >> i) & 1) == 1))
                        {
                            Correct++;
                        }
                    }

                    this.PuzzleStatus(Statue_Msg[Correct], (Correct > 0) ? Correct.ToString() : null);

                    for (int i = 0; i < 5; i++)
                    {
                        if ((m_Player = this.GetOccupant(i)) != null)
                        {
                            Timer smash = new RockTimer(m_Player, this);
                            smash.Start();
                        }
                    }
                }
            }
            this.ResetLevers();
        }

        public virtual void GenKey() /* Shuffle & build key */
        {
            UInt16 tmp;
            int n, i;
            ushort[] CA = { 1, 2, 4, 8 };
            for (i = 0; i < 4; i++)
            {
                n = (((n = Utility.Random(0, 3)) == i) ? n & ~i : n); /* if(i==n) { return pointless; } */
                tmp = CA[i];
                CA[i] = CA[n];
                CA[n] = tmp;
            }
            for (i = 0; i < 4; this.MyKey = (ushort)(CA[(i++)] | (this.MyKey <<= 4)))
            {
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.WriteItemList(this.m_Levers, true);
            writer.WriteItemList(this.m_Statues, true);
            writer.WriteItemList(this.m_Teles, true);
            writer.Write(this.m_Box);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Levers = reader.ReadStrongItemList();
            this.m_Statues = reader.ReadStrongItemList();
            this.m_Teles = reader.ReadStrongItemList();

            this.m_Box = reader.ReadItem() as LampRoomBox;

            this.m_Tiles = new List<LeverPuzzleRegion>();
            for (int i = 4; i < 9; i++)
                this.m_Tiles.Add(new LeverPuzzleRegion(this, TA[i]));

            this.m_LampRoom = new LampRoomRegion(this);
            this.m_Enabled = true;
            this.m_TheirKey = 0;
            this.m_MyKey = 0;
            this.GenKey();
        }

        private static bool IsValidDamagable(Mobile m)
        {
            if (m != null && !m.Deleted)
            {
                if (m.Player && m.Alive)
                {
                    return true;
                }

                if (m is BaseCreature)
                {
                    BaseCreature bc = (BaseCreature)m;
                    if ((bc.Controlled || bc.Summoned) && !bc.IsDeadBondedPet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public class RockTimer : Timer
        {
            private readonly Mobile m_Player;
            private readonly LeverPuzzleController m_Controller;
            private int Count;
            public RockTimer(Mobile player, LeverPuzzleController Controller)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(.25))
            {
                this.Count = 0;
                this.m_Player = player;
                this.m_Controller = Controller;
            }

            protected override void OnTick()
            {
                if (this.m_Player == null || !(this.m_Player.Map == Map.Malas))
                {
                    this.Stop();
                }
                else
                {
                    this.Count++;
                    if (this.Count == 1) /* TODO consolidate */
                    { 
                        this.m_Player.Paralyze(TimeSpan.FromSeconds(2));
                        Effects.SendTargetEffect(this.m_Player, 0x11B7, 20, 10);
                        PlayerSendASCII(this.m_Player, 0);  // You are pinned down ...

                        PlaySounds(this.m_Player.Location, (!this.m_Player.Female) ? fs : ms);
                        PlayEffect(ZAdjustedIEFromMobile(this.m_Player, 50), this.m_Player, 0x11B7, 20, false);
                    }
                    else if (this.Count == 2)
                    {
                        DoDamage(this.m_Player, 80, 90, false);
                        Effects.SendTargetEffect(this.m_Player, 0x36BD, 20, 10);
                        PlaySounds(this.m_Player.Location, exp);
                        PlayerSendASCII(this.m_Player, 1); // A speeding rock  ...

                        if (AniSafe(this.m_Player))
                        {
                            this.m_Player.Animate(21, 10, 1, true, true, 0);
                        }
                    }
                    else if (this.Count == 3)
                    {
                        this.Stop();

                        Effects.SendTargetEffect(this.m_Player, 0x36B0, 20, 10);
                        PlayerSendASCII(this.m_Player, 1); // A speeding rock  ...
                        PlaySounds(this.m_Player.Location, (!this.m_Player.Female) ? fs2 : ms2);

                        int j = Utility.Random(6, 10);
                        for (int i = 0; i < j; i++)
                        {
                            IEntity m_IEntity = new Entity(Serial.Zero, RandomPointIn(this.m_Player.Location, 10), this.m_Player.Map);

                            List<Mobile> mobiles = new List<Mobile>();
                            IPooledEnumerable eable = m_IEntity.Map.GetMobilesInRange(m_IEntity.Location, 2);

                            foreach (Mobile m in eable)
                            {
                                mobiles.Add(m);
                            }
                            eable.Free();
                            for (int k = 0; k < mobiles.Count; k++)
                            {
                                if (IsValidDamagable(mobiles[k]) && mobiles[k] != this.m_Player)
                                {
                                    PlayEffect(this.m_Player, mobiles[k], this.Rock(), 8, true);
                                    DoDamage(mobiles[k], 25, 30, false);

                                    if (mobiles[k].Player)
                                    {
                                        POHMessage(mobiles[k], 2); // OUCH!
                                    }
                                }
                            }
                            PlayEffect(this.m_Player, m_IEntity, this.Rock(), 8, false);
                        }
                    }
                }
            }

            private int Rock()
            {
                return 0x1363 + Utility.Random(0, 11);
            }
        }

        public class LampRoomKickTimer : Timer
        {
            private readonly Mobile m;
            public LampRoomKickTimer(Mobile player)
                : base(TimeSpan.FromSeconds(.25))
            {
                this.m = player;
            }

            protected override void OnTick()
            {
                MoveMobileOut(this.m);
            }
        }

        public class LampRoomTimer : Timer
        {
            public LeverPuzzleController m_Controller;
            public int ticks;
            public int level;
            public LampRoomTimer(LeverPuzzleController controller)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.level = 0;
                this.ticks = 0;
                this.m_Controller = controller;
            }

            protected override void OnTick()
            {
                this.ticks++;
                List<Mobile> mobiles = this.m_Controller.m_LampRoom.GetMobiles();

                if (this.ticks >= 71 || this.m_Controller.m_LampRoom.GetPlayerCount() == 0)
                {
                    foreach (Mobile mobile in mobiles)
                    {
                        if (mobile != null && !mobile.Deleted && !mobile.IsDeadBondedPet)
                        {
                            mobile.Kill();
                        }
                    }
                    this.m_Controller.Enabled = true;
                    this.Stop();
                }
                else
                {
                    if (this.ticks % 12 == 0)
                    {
                        this.level++;
                    }
                    foreach (Mobile mobile in mobiles)
                    {
                        if (IsValidDamagable(mobile))
                        {
                            if (this.ticks % 2 == 0 && this.level == 5)
                            {
                                if (mobile.Player)
                                {
                                    mobile.Say(1062092);
                                    if (AniSafe(mobile))
                                    {
                                        mobile.Animate(32, 5, 1, true, false, 0);
                                    }
                                }
                                DoDamage(mobile, 15, 20, true);
                            }
                            if (Utility.Random((int)(this.level & ~0xfffffffc), 3) == 3)
                            {
                                mobile.ApplyPoison(mobile, PA2[this.level]);
                            }
                            if (this.ticks % 12 == 0 && this.level > 0 && mobile.Player)
                            {
                                mobile.SendLocalizedMessage(PA[this.level][0], null, PA[this.level][1]);
                            }
                        }
                    }
                    for (int i = 0; i <= this.level; i++)
                    {
                        SendLocationEffect(RandomPointIn(lr_Rect, -1), 0x36B0, Utility.Random(150, 200), 0, PA[this.level][2]);
                    }
                }
            }
        }
    }
}