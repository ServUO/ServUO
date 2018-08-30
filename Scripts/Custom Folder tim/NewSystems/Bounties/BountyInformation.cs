using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Server.Items;

namespace Server.Misc
{
    public class BountyInformation
    {
        public static void Configure()
        {
            EventSink.WorldLoad += Load;
            EventSink.WorldSave += Save;
            CommandSystem.Register("BountyProps", AccessLevel.Counselor, BeginBountyProps);
        }

        private static void BeginBountyProps(CommandEventArgs e)
        {
            e.Mobile.BeginTarget(-1, false, TargetFlags.None, ShowBountyProps);
        }

        private static void ShowBountyProps(Mobile from, object targeted)
        {
            var pm = targeted as PlayerMobile;
            if (pm == null)
            {
                from.SendMessage("Only players can have bounties.");
                return;
            }

            var bounty = AllInfo.FirstOrDefault(x => x.BountyPlayer == pm);
            if (bounty == null)
            {
                from.SendMessage("That player has no bounty.");
                return;
            }

            from.SendGump(new PropertiesGump(from, bounty));
        }

        private static void Save(WorldSaveEventArgs e)
        {
            if (!Directory.Exists("Saves/Bounties"))
                Directory.CreateDirectory("Saves/Bounties");

            var fullPath = Path.Combine("Saves/Bounties", "BountyInformation.xml");

            using (var op = new StreamWriter(fullPath))
            {
                var xml = new XmlTextWriter(op)
                {
                    Formatting = Formatting.Indented,
                    IndentChar = '\t',
                    Indentation = 1
                };

                xml.WriteStartDocument(true);
                xml.WriteStartElement("bounties");
                xml.WriteAttributeString("count", AllInfo.Count.ToString());

                var toSave = GetValidBounties();
                foreach (var bi in toSave)
                {
                    bi.SaveBounty(xml);
                }

                xml.WriteEndElement();

                xml.Close();
            }
        }

        private static void Load()
        {
            if (!Directory.Exists("Saves/Bounties"))
                return;

            var fullPath = Path.Combine("Saves/Bounties", "BountyInformation.xml");

            if (!File.Exists(fullPath))
                return;

            var saveFile = new XmlDocument();
            saveFile.Load(fullPath);

            var root = saveFile["bounties"];
            var noFail = true;
            var tried = 0;

            foreach (XmlElement bountyInfo in root.GetElementsByTagName("bounty"))
            {
                LoadBounty(bountyInfo, ref noFail);
                tried++;
            }

            if (noFail)
            {
                Console.WriteLine("\n" + AllInfo.Count + " bounties loaded successfully.");
            }
            else
            {
                Console.WriteLine("An error occurred while loading bounties." + AllInfo.Count + " out of " + tried +
                                  " loaded successfully.");
            }
        }

        public static List<BountyInformation> AllInfo = new List<BountyInformation>();

        private PlayerMobile _bountyPlayer;

        [CommandProperty(AccessLevel.Counselor)]
        public PlayerMobile BountyPlayer { get { return _bountyPlayer; } }

        private int _bounty;

        [CommandProperty(AccessLevel.Counselor)]
        public int Bounty
        {
            get { return _bounty; }
            set
            {
                var changed = _bounty != value;
                _bounty = value;

                if (changed)
                    UpdateBountyBoards();
            }
        }

        private void UpdateBountyBoards()
        {
            BountyBoard.UpdateBounty(BountyPlayer);
        }

        [CommandProperty(AccessLevel.Counselor)]
        public DateTime LastBounty { get; set; }

        public bool Expired
        {
            get
            {
                return BountyPlayer.Deleted || BountyPlayer.Kills < 5 || Bounty <= 0 ||
                       LastBounty + TimeSpan.FromDays(14.0) < DateTime.UtcNow;
            }
        }

        public static BountyInformation AddBounty(PlayerMobile pm, int bounty, bool updateBoards)
        {
            var bi = AllInfo.FirstOrDefault(info => info.BountyPlayer == pm);

            if (bi == null)
            {
                bi = new BountyInformation { _bountyPlayer = pm};
                AllInfo.Add(bi);
            }

            bi.AddBounty(bounty, updateBoards);
            return bi;
        }

        public static List<BountyInformation> GetValidBounties()
        {
            AllInfo = AllInfo.Where(x => !x.Expired).ToList();
            return AllInfo;
        }

        private void AddBounty(int bounty, bool updateBoards)
        {
            if (updateBoards)
                Bounty += bounty;
            else
                _bounty += bounty;

            LastBounty = DateTime.UtcNow;
        }

        public void SubtractBounty(int headBounty)
        {
            Bounty -= headBounty;
        }

        public static BountyInformation GetBountyInformation(Mobile bountyPlayer)
        {
            return AllInfo.FirstOrDefault(info => info.BountyPlayer == bountyPlayer);
        }

        public static int GetBounty(Mobile bountyPlayer)
        {
            var bi = GetBountyInformation(bountyPlayer);
            return bi == null ? 0 : bi.Bounty;
        }

        private static void LoadBounty(XmlElement node, ref bool noFail)
        {
            try
            {
                var serial = Utility.GetXMLInt32(Utility.GetText(node["serial"], "-1"), -1);
                var mob = World.FindMobile(serial) as PlayerMobile;
                var bounty = Utility.GetXMLInt32(Utility.GetText(node["bountyAmount"], "-1"), -1);
                var lastBounty = Utility.GetXMLDateTime(Utility.GetText(node["lastBountyTime"], null), DateTime.UtcNow);

                if (mob == null || bounty == -1 || lastBounty + TimeSpan.FromDays(14.0) < DateTime.UtcNow)
                    return;

                var bi = AddBounty(mob, bounty, false);
                bi.LastBounty = lastBounty;
            }
            catch
            {
                noFail = false;
            }
        }

        private void SaveBounty(XmlTextWriter xml)
        {
            xml.WriteStartElement("bounty");

            xml.WriteStartElement("serial");
            xml.WriteString(BountyPlayer.Serial.Value.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("bountyAmount");
            xml.WriteString(Bounty.ToString());
            xml.WriteEndElement();

            xml.WriteStartElement("lastBountyTime");
            xml.WriteString(XmlConvert.ToString(LastBounty, XmlDateTimeSerializationMode.Utc));
            xml.WriteEndElement();

            xml.WriteEndElement();
        }
    }
}