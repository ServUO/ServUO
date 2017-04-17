using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Commands;
using Server.Network;
using Server.Items;
using Server.Commands;

namespace Server
{
    public class LifeStoneCore
    {
        private static string m_Version = "2.0";
        private static string m_SavePath = Path.Combine(Core.BaseDirectory, @"Saves\LifeStoneSystem");
        private static List<LifeStoneInfo> m_LifeStoneList = new List<LifeStoneInfo>();
        private static LifeStoneSettings m_Settings;

        [CommandProperty(AccessLevel.Administrator)] // Read Only
        public static string Version { get { return m_Version; } }

        [CommandProperty(AccessLevel.Administrator)] // Read Only
        public static string SaveDirectory { get { return m_SavePath; } }

        [CommandProperty(AccessLevel.Administrator)]
        public static LifeStoneSettings Settings { get { return m_Settings; } set { m_Settings = value; } }

        public static List<LifeStoneInfo> LifeStoneList { get { return m_LifeStoneList; } set { m_LifeStoneList = value; } }
        
        public static void Initialize()
        {
            Server.EventSink.WorldSave += new WorldSaveEventHandler(LifeStoneCore_WorldSave);

            // Purdy colors...
            Utility.PushColor(ConsoleColor.DarkGreen);
            Console.WriteLine("----------------------------------------");
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("Loading Life Stone version: {0}", m_Version);
            
            DateTime start = DateTime.Now;

            if (File.Exists(Path.Combine(m_SavePath, "LifeStoneSystem.bin")))
            {
                Load();
            }
            else
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("No save file found, creating default settings.");
                Utility.PopColor();
                LoadDefaults();
            }

            Console.WriteLine("Finished in {0:F1} seconds.", (DateTime.Now - start).TotalSeconds);
            Utility.PopColor();
            Console.WriteLine("----------------------------------------");

            CommandSystem.Register("LifeStoneConfig", AccessLevel.Administrator, new CommandEventHandler(OnCommand_LifeStone));
            CommandSystem.Register("LSConfig", AccessLevel.Administrator, new CommandEventHandler(OnCommand_LifeStone));

            Utility.PopColor();
        }

        [Usage("LSConfig")]
        [Description("Opens the control center for the Life Stone system.")]
        private static void OnCommand_LifeStone(CommandEventArgs e)
        {
            PlayerMobile from = e.Mobile as PlayerMobile;

            if (from != null && !from.Deleted)
            {
                from.SendGump(new PropertiesGump(e.Mobile, m_Settings));
            }
        }

        private static void Load()
        {
            try
            {
                string SavePath = Path.Combine(m_SavePath, "LifeStoneSystem.bin");

                using (FileStream fs = new FileStream(SavePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    BinaryReader br = new BinaryReader(fs);
                    BinaryFileReader reader = new BinaryFileReader(br);

                    int version = reader.ReadInt();

                    switch (version)
                    {
                        case 0:
                            {
                                m_LifeStoneList = new List<LifeStoneInfo>();
                                int length = reader.ReadInt();

                                for (int i = 0; i < length; i++)
                                {
                                    m_LifeStoneList.Add(new LifeStoneInfo(reader));
                                }
                                m_Settings = new LifeStoneSettings(reader);
                                break;
                            }
                    }

                    reader.Close();
                }
            }
            catch (Exception err)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("An error occured while loading the Life Stone System.");
                Console.WriteLine("Message: {0}", err.Message);
                Console.WriteLine("Stack Trace: {0}", err.StackTrace);
                Utility.PopColor();
            }
        }

        private static void LoadDefaults()
        {
            LifeStoneSettings lifeStoneSettings = new LifeStoneSettings(false, false, false, 7964, 1167);
            Settings = lifeStoneSettings;
        }

        public static void LifeStoneCore_WorldSave(WorldSaveEventArgs e)
        {
            Utility.PushColor(ConsoleColor.DarkGreen);
            Console.WriteLine("");
            Console.WriteLine("----------------------------------------");
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("Saving Life Stone system...");
            DateTime start = DateTime.Now;

            string SavePath = Path.Combine(m_SavePath, "LifeStoneSystem.bin");

            if (!Directory.Exists(m_SavePath))
                Directory.CreateDirectory(m_SavePath);

            GenericWriter writer = new BinaryFileWriter(SavePath, true);

            try
            {
                // Version
                writer.Write(0);

                // Verison 0
                writer.Write(m_LifeStoneList.Count);

                for (int i = 0; i < m_LifeStoneList.Count; i++)
                {
                    m_LifeStoneList[i].Serialize(writer);
                }
                m_Settings.Serialize(writer);
            }
            catch(Exception err)
            {
                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine("An error occured while saving the Life Stone system.");
                Console.WriteLine("Message: {0}", err.Message);
                Console.WriteLine("Stack Trace: {0}", err.StackTrace);
                Utility.PopColor();
            }

            writer.Close();

            Console.WriteLine("Finished saving in {0:F1} seconds.", (DateTime.Now - start).TotalSeconds);
            Utility.PopColor();
            Console.WriteLine("----------------------------------------");

            Utility.PopColor();
        }
    }
}