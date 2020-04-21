using Server.Commands;
using Server.Items;
using System;
using System.Collections.Generic;

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
            m_Code = code;
            m_Controller = controller;
            Movable = false;
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

            if (null == m_Controller || m_Controller.Deleted || !m_Controller.Active)
            {
                Delete();
                return;
            }

            if (null != m_Controller.Successful)
                m.SendLocalizedMessage(1112374);  // The puzzle has already been completed.
            else
            {
                ItemID ^= 2;
                Effects.PlaySound(Location, Map, 0x3E8);
                m_Controller.LeverPulled(m_Code, m);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(m_Code);
            writer.Write(m_Controller);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Code = reader.ReadByte();
            m_Controller = reader.ReadItem() as PrimevalLichPuzzle;

            // remove if no controller exists or is deleted
            if (null == m_Controller || m_Controller.Deleted)
                Delete();
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
                Delete();
                //Probably not needed, OnAfterDelete sets it null anyway
                if (m_Instance != null && m_Instance.Deleted)
                    m_Instance = null;
                return;
            }

            Movable = false;
            Visible = false;
            m_Instance = this;
            MoveToWorld(controlLoc, Map.Felucca);

            m_Levers = new List<PrimevalLichPuzzleLever>();

            m_Altar = FindAltar();

            if (null == m_Altar)
            {
                m.SendMessage(33, "Primeval Lich champion spawn not found.");
                Delete();
            }
            else
            {
                UpdatePuzzleState(m_Altar);
            }
        }

        // serialization code
        public PrimevalLichPuzzle(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Successful => m_Successful;
        [CommandProperty(AccessLevel.GameMaster)]
        public long Key => m_Key;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active => (!Deleted && null != m_Altar && m_Altar.Active);
        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionSpawn ChampionAltar
        {
            get
            {
                return m_Altar;
            }
            set
            {
                m_Altar = value;
            }
        }
        public override string DefaultName => "puzzle control";
        public static void Initialize()
        {
            CommandSystem.Register("GenLichPuzzle", AccessLevel.Administrator, GenLichPuzzle_OnCommand);
            CommandSystem.Register("DeleteLichPuzzle", AccessLevel.Administrator, DeleteLichPuzzle_OnCommand);
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
                if (m != null)
                {
                    m.SendMessage("Primeval Lich lever puzzle already exists: please delete the existing one first ...");
                }

                Utility.WriteConsoleColor(ConsoleColor.Green, "Primeval Lich lever puzzle already exists: please delete the existing one first ...");

                return;
            }

            if (m != null)
            {
                m.SendMessage("Generating Primeval Lich lever puzzle...");
            }

            Utility.WriteConsoleColor(ConsoleColor.Green, "Generating Primeval Lich lever puzzle...");

            PrimevalLichPuzzle control = new PrimevalLichPuzzle(m);
            if (null == control || control.Deleted)
            {
                if (m != null)
                {
                    m.SendMessage(33, "There was a problem generating the puzzle.");
                }

                Utility.WriteConsoleColor(ConsoleColor.Green, "There was a problem generating the puzzle.");
            }
            else
            {
                if (m != null)
                {
                    m.SendMessage("The puzzle was successfully generated.");
                }

                Utility.WriteConsoleColor(ConsoleColor.Green, "The puzzle was successfully generated.");
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
            RemovePuzzleLevers();
            if (this == m_Instance)
                m_Instance = null;

            base.OnAfterDelete();
        }

        // process a lever pull
        public void LeverPulled(byte key, Mobile m)
        {
            if (!Active || null == m)
                return;

            // teleport if this is a dummy key
            if (0 == key)
            {
                Point3D loc = m_Altar.GetSpawnLocation();
                m.MoveToWorld(loc, Map);

                ResetLevers();
                return;
            }
            // if the lever is correct, increment the count of correct levers pulled
            else if (key == m_NextKey)
                m_Correct++;

            // stop and restart the lever reset timer
            if (null != l_Timer)
                l_Timer.Stop();
            l_Timer = Timer.DelayCall(TimeSpan.FromSeconds(30.0), ResetLevers);

            // if this is the last key, check for correct solution and give messages/rewards
            if (6 == m_NextKey++)
            {
                // all 6 were correct, so set successful and give a reward
                if (6 == m_Correct)
                {
                    m_Successful = m;
                    GiveReward(m);
                }

                // send a message based of the number of correct levers pulled
                m.SendLocalizedMessage(1112378 + m_Correct);
                ResetLevers();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version

            writer.Write(m_Instance);
            writer.Write(m_Altar);
            writer.Write(m_Key);
            writer.Write(m_Successful);
            writer.WriteItemList(m_Levers, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    m_Instance = reader.ReadItem() as PrimevalLichPuzzle;
                    m_Altar = reader.ReadItem() as ChampionSpawn;
                    m_Key = reader.ReadLong();
                    m_Successful = reader.ReadMobile();
                    m_Levers = reader.ReadStrongItemList<PrimevalLichPuzzleLever>();
                    break;
            }

            if (null == m_Levers)
                m_Levers = new List<PrimevalLichPuzzleLever>();
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
            if (!Deleted && null != altar && altar == m_Altar)
            {
                if (ChampionSpawnType.Infuse != m_Altar.Type || !Active)
                    RemovePuzzleLevers();
                else if (0 == m_Levers.Count)
                    CreatePuzzleLevers();
            }
        }

        private void CreatePuzzleLevers()
        {
            // remove any existing puzzle levers
            RemovePuzzleLevers();

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
                m_Key = newkey;

                // create the puzzle levers
                PrimevalLichPuzzleLever lever;
                int[] val;

                if (null == m_Levers)
                    m_Levers = new List<PrimevalLichPuzzleLever>();

                for (int i = 0; i < len; i++)
                {
                    lever = new PrimevalLichPuzzleLever(keymap[i], this);
                    if (null != lever)
                    {
                        val = leverdata[i];
                        lever.MoveToWorld(new Point3D(val[0], val[1], val[2]), Map.Felucca);
                        lever.Hue = val[3];
                        m_Levers.Add(lever);
                    }
                }
            }
        }

        // remove any existing puzzle levers
        private void RemovePuzzleLevers()
        {
            if (null != l_Timer)
            {
                l_Timer.Stop();
                l_Timer = null;
            }

            if (null != m_Levers)
            {
                foreach (Item item in m_Levers)
                {
                    if (item != null && !item.Deleted)
                        item.Delete();
                }
                m_Levers.Clear();
            }

            m_Successful = null;
            m_Key = 0;
        }

        // reset puzzle levers to default position
        private void ResetLevers()
        {
            if (null != l_Timer)
            {
                l_Timer.Stop();
                l_Timer = null;
            }

            if (null != m_Levers)
            {
                foreach (PrimevalLichPuzzleLever l in m_Levers)
                {
                    if (null != l && !l.Deleted)
                    {
                        l.ItemID = 0x108C;
                        Effects.PlaySound(l.Location, Map, 0x3E8);
                    }
                }
            }

            m_Correct = 0;
            m_NextKey = 1;
        }

        // distribute a reward to the puzzle solver
        private void GiveReward(Mobile m)
        {
            if (null == m)
                return;

            Item item = null;

            switch (Utility.Random(1))
            {
                case 0:
                    item = ScrollOfTranscendence.CreateRandom(10, 10);
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
