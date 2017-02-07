using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Items;

//
// This implements the Primeval Lich Lever Puzzle for SA.  
// 
// To install, copy this file anywhere into the scripts directory tree.  Next edit
// ChampionSpawn.cs and add the following line of code at the end of the Start() and Stop()
// methods in the ChampionSpawn.cs file:
// 
//          PrimevalLichPuzzle.Update(this);
//
// Next compile and be sure that the Primeval Lich championSpawn is created. Then you can
// use the command [genlichpuzzle to create the puzzle controller.  After that the puzzle
// will become active/inactive automatically.
//
// The "key" property on the puzzle controller shows the solution levers,
// numbered (west to east) as 1 thru 8 for testing purposes.
//
namespace Server.Engines.CannedEvil
{
    public class PrimevalLichPuzzleLever : Item
    {
        private PrimevalLichPuzzle m_Controller;
        private byte m_Code;
        // Constructor
        public PrimevalLichPuzzleLever(byte code, PrimevalLichPuzzle controller)
            : base(0x108C)
        {
            this.m_Code = code;
            this.m_Controller = controller;
            this.Movable = false;
        }

        // serialization code
        public PrimevalLichPuzzleLever(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (null == m)
                return;

            if (null == this.m_Controller || this.m_Controller.Deleted || !this.m_Controller.Active)
            {
                this.Delete();
                return;
            }

            if (null != this.m_Controller.Successful)
                m.SendLocalizedMessage(1112374);  // The puzzle has already been completed.
            else
            {
                this.ItemID ^= 2;
                Effects.PlaySound(this.Location, this.Map, 0x3E8);
                this.m_Controller.LeverPulled(this.m_Code, m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write((byte)this.m_Code);
            writer.Write(this.m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Code = reader.ReadByte();
            this.m_Controller = reader.ReadItem() as PrimevalLichPuzzle;

            // remove if no controller exists or is deleted
            if (null == this.m_Controller || this.m_Controller.Deleted)
                this.Delete();
        }
    }

    public class PrimevalLichPuzzle : Item
    {
        // expected location of the Priveval Lich champion altar
        private static readonly Point3D altarLoc = new Point3D(7001, 1008, -15);
        // location to place the Primeval Lich puzzle controller
        private static readonly Point3D controlLoc = new Point3D(6999, 977, -15);
        // puzzle lever data
        private static readonly int[][] leverdata = 
        { // 3D coord, hue for levers
            new int[] { 6981, 977, -15, 1204 }, // red
            new int[] { 6984, 977, -15, 1150 }, // white
            new int[] { 6987, 977, -15, 1175 }, // black
            new int[] { 6990, 977, -15, 1264 }, // blue
            new int[] { 7009, 977, -15, 1275 }, // purple
            new int[] { 7012, 977, -15, 1272 }, // green
            new int[] { 7015, 977, -15, 1260 }, // gold
            new int[] { 7018, 977, -15, 1166 }, // pink 
        };
        // these are serialized
        private static PrimevalLichPuzzle m_Instance;
        private ChampionSpawn m_Altar;
        private long m_Key;
        private Mobile m_Successful;
        private List<PrimevalLichPuzzleLever> m_Levers;
        // these are not serialized
        private byte m_NextKey;
        private int m_Correct;
        private Timer l_Timer;
        // Constructor
        public PrimevalLichPuzzle(Mobile m)
            : base(0x1BC3)
        {
            if (null == m || null != m_Instance)
            {
                this.Delete();

                if (m_Instance.Deleted)
                    m_Instance = null;

                return;
            }

            this.Movable = false;
            this.Visible = false;
            m_Instance = this;
            this.MoveToWorld(controlLoc, Map.Felucca);

            this.m_Levers = new List<PrimevalLichPuzzleLever>();

            this.m_Altar = this.FindAltar();

            if (null == this.m_Altar)
            {
                m.SendMessage(33, "Primeval Lich champion spawn not found.");
                this.Delete();
            }
            else
            {
                this.UpdatePuzzleState(this.m_Altar);
            }
        }

        // serialization code
        public PrimevalLichPuzzle(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Successful
        {
            get
            {
                return this.m_Successful;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public long Key
        {
            get
            {
                return this.m_Key;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get
            {
                return(!this.Deleted && null != this.m_Altar && this.m_Altar.Active);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSpawn ChampionAltar
        {
            get
            {
                return this.m_Altar;
            }
            set
            {
                this.m_Altar = value;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "puzzle control";
            }
        }
        public static void Initialize()
        {
			CommandSystem.Register("GenLichPuzzle", AccessLevel.Administrator, new CommandEventHandler(GenLichPuzzle_OnCommand));
			CommandSystem.Register("DeleteLichPuzzle", AccessLevel.Administrator, new CommandEventHandler(DeleteLichPuzzle_OnCommand));
		}

		[Usage("DeleteLichPuzzle")]
        [Description("Deletes the Primeval Lich lever puzzle.")]
		public static void DeleteLichPuzzle_OnCommand(CommandEventArgs e)
		{
			WeakEntityCollection.Delete("primevallich");
		}

		[Usage("GenLichPuzzle")]
        [Description("Generates the Primeval Lich lever puzzle.")]
        public static void GenLichPuzzle_OnCommand(CommandEventArgs e)
        {
            GenLichPuzzle(e.Mobile);
        }

        public static void GenLichPuzzle(Mobile m)
        {
            if (null != m_Instance)
            {
                m.SendMessage("Primeval Lich lever puzzle already exists: please delete the existing one first ...");
                return;
            }

            m.SendMessage("Generating Primeval Lich lever puzzle...");
            PrimevalLichPuzzle control = new PrimevalLichPuzzle(m);
            if (null == control || control.Deleted)
            {
                m.SendMessage(33, "There was a problem generating the puzzle.");
            }
            else
            {
                m.SendMessage("The puzzle was successfully generated.");
                WeakEntityCollection.Add("primevallich", control);
            }
        }

        // static hook for the ChampionSpawn code to update the puzzle state
        public static void Update(ChampionSpawn altar)
        {
            if (null != m_Instance)
            {
                if (m_Instance.Deleted)
                    m_Instance = null;
                else if (m_Instance.ChampionAltar == altar)
                    m_Instance.UpdatePuzzleState(altar);
            }
        }

        public override void OnAfterDelete()
        {
            this.RemovePuzzleLevers();
            if (this == m_Instance)
                m_Instance = null;

            base.OnAfterDelete();
        }

        // process a lever pull
        public void LeverPulled(byte key, Mobile m)
        {
            if (!this.Active || null == m)
                return;

            // teleport if this is a dummy key
            if (0 == key)
            {
                Point3D loc = this.m_Altar.GetSpawnLocation();
                m.MoveToWorld(loc, this.Map);

                this.ResetLevers();
                return;
            }
            // if the lever is correct, increment the count of correct levers pulled
            else if (key == this.m_NextKey)
                this.m_Correct++;

            // stop and restart the lever reset timer
            if (null != this.l_Timer)
                this.l_Timer.Stop();
            this.l_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), new TimerCallback(ResetLevers));

            // if this is the last key, check for correct solution and give messages/rewards
            if (6 == this.m_NextKey++)
            {
                // all 6 were correct, so set successful and give a reward
                if (6 == this.m_Correct)
                {
                    this.m_Successful = m;
                    this.GiveReward(m);
                }

                // send a message based of the number of correct levers pulled
                m.SendLocalizedMessage(1112378 + this.m_Correct);
                this.ResetLevers();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write((PrimevalLichPuzzle)m_Instance);
            writer.Write((ChampionSpawn)this.m_Altar);
            writer.Write((long)this.m_Key);
            writer.Write((Mobile)this.m_Successful);
            writer.WriteItemList(this.m_Levers, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    m_Instance = reader.ReadItem() as PrimevalLichPuzzle;
                    this.m_Altar = reader.ReadItem() as ChampionSpawn;
                    this.m_Key = reader.ReadLong();
                    this.m_Successful = reader.ReadMobile();
                    this.m_Levers = reader.ReadStrongItemList<PrimevalLichPuzzleLever>();
                    break;
            }

            if (null == this.m_Levers)
                this.m_Levers = new List<PrimevalLichPuzzleLever>();
            //            if ( null != m_Instance && m_Instance.Deleted && this == m_Instance )
            //            {
            //                m_Instance = null;
            //                return;
            //            }
            //            // remove if no altar exists
            //            if ( null == m_Altar )
            //                Timer.DelayCall( TimeSpan.FromSeconds( 0.0 ), new TimerCallback( Delete ) );
            //            ResetLevers();
        }

        // search for Primeval Lich altar within 10 spaces of the expected location
        private ChampionSpawn FindAltar()
        {
            foreach (Item item in Map.Felucca.GetItemsInRange(altarLoc, 10))
            {
                if (item is ChampionSpawn)
                {
                    ChampionSpawn champ = (ChampionSpawn)item;
                    if (ChampionSpawnType.Infuse == champ.Type)
                        return champ;
                }
            }
            return null;
        }

        // internal code to update the puzzle state
        private void UpdatePuzzleState(ChampionSpawn altar)
        {
            if (!this.Deleted && null != altar && altar == this.m_Altar)
            {
                if (ChampionSpawnType.Infuse != this.m_Altar.Type || !this.Active)
                    this.RemovePuzzleLevers();
                else if (0 == this.m_Levers.Count)
                    this.CreatePuzzleLevers();
            }
        }

        private void CreatePuzzleLevers()
        {
            // remove any existing puzzle levers
            this.RemovePuzzleLevers();

            // generate a new key for the puzzle
            int len = leverdata.Length;

            if (8 == len)
            {
                // initialize new keymap
                byte[] keymap = new byte[len];
                int ndx;
                byte code = 1;
                int newkey = 0;
                while (code <= 6)
                {
                    ndx = Utility.Random(len);
                    if (0 == keymap[ndx])
                    {
                        keymap[ndx] = code++;
                        newkey = newkey * 10 + ndx + 1;
                    }
                }
                this.m_Key = newkey;

                // create the puzzle levers
                PrimevalLichPuzzleLever lever;
                int[] val;

                if (null == this.m_Levers)
                    this.m_Levers = new List<PrimevalLichPuzzleLever>();

                for (int i = 0; i < len; i++)
                {
                    lever = new PrimevalLichPuzzleLever(keymap[i], this);
                    if (null != lever)
                    {
                        val = leverdata[i];
                        lever.MoveToWorld(new Point3D(val[0], val[1], val[2]), Map.Felucca);
                        lever.Hue = val[3];
                        this.m_Levers.Add(lever);
                    }
                }
            }
        }

        // remove any existing puzzle levers
        private void RemovePuzzleLevers()
        {
            if (null != this.l_Timer)
            {
                this.l_Timer.Stop();
                this.l_Timer = null;
            }

            if (null != this.m_Levers)
            {
                foreach (Item item in this.m_Levers)
                {
                    if (item != null && !item.Deleted)
                        item.Delete();
                }
                this.m_Levers.Clear();
            }

            this.m_Successful = null;
            this.m_Key = 0;
        }

        // reset puzzle levers to default position
        private void ResetLevers()
        {
            if (null != this.l_Timer)
            {
                this.l_Timer.Stop();
                this.l_Timer = null;
            }

            if (null != this.m_Levers)
            {
                foreach (PrimevalLichPuzzleLever l in this.m_Levers)
                {
                    if (null != l && !l.Deleted)
                    {
                        l.ItemID = 0x108C;
                        Effects.PlaySound(l.Location, this.Map, 0x3E8);
                    }
                }
            }

            this.m_Correct = 0;
            this.m_NextKey = 1;
        }

        // distribute a reward to the puzzle solver
        private void GiveReward(Mobile m)
        {
            if (null == m)
                return;

            Item item = null;

            switch ( Utility.Random(1) )
            {
                case 0:
                    item = ScrollofTranscendence.CreateRandom(10, 10);
                    break;
                case 1:
                    // black bone container
                    break;
                case 2:
                    // red bone container
                    break;
                case 3:
                    // gruesome standard
                    break;
                case 4:
                    // bag of gems
                    break;
                case 5:
                    // random piece of spawn decoration
                    break;
            }

            // drop the item in backpack or bankbox
            if (null != item)
            {
                Container pack = m.Backpack;
                if (null == pack || !pack.TryDropItem(m, item, false))
                    m.BankBox.DropItem(item);
            }
        }

        // generate a new keymap
        private void GenKey()
        {
        }
    }
}