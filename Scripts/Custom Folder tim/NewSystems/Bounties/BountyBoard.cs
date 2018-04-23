using Server.Misc;
using Server.Network;
using System.Collections.Generic;
using System.Linq;
using Server.Network.Misc;
using Server.Mobiles;

namespace Server.Items
{
    public class BountyBoard : BaseBulletinBoard
    {
        private static readonly List<BountyBoard> AllBoards = new List<BountyBoard>();

        [Constructable]
        public BountyBoard() : base(0x1E5E)
        {
            BoardName = "bounty board";
            AllBoards.Add(this);
            GetInitialBounties();
        }

        private void GetInitialBounties()
        {
            var bountyPlayers = BountyInformation.GetValidBounties().Select(x=>x.BountyPlayer);

            foreach (var bounty in bountyPlayers)
            {
                AddBountyToBoard(bounty);
            }
        }

        public BountyBoard(Serial serial) : base(serial)
        {
            AllBoards.Add(this);
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from,
                string.Format("a bounty board with {0} posted bount{1}", Items.Count, Items.Count == 1 ? "y" : "ies"));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            list.Add("Bounty Board");
            list.Add("{0} posted bount{1}", Items.Count, Items.Count == 1 ? "y" : "ies");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (CheckRange(from))
            {
                Cleanup();
                
                var state = from.NetState;

                state.Send(new BBDisplayBoard(this));
                if (state.ContainerGridLines)
                    state.Send(new BountyPackets.BBContent6017(from, this));
                else
                    state.Send(new BountyPackets.BBContent(from, this));
            }
            else
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
        }

        public override void OnItemAdded(Item item)
        {
            if (!(item is BountyMessage))
            {
                var bm = item as BulletinMessage;
                if (bm != null && bm.Poster != null)
                    bm.Poster.SendMessage("This board is for bounty messages only.");
                item.Delete();
            }
        }

        public override void Cleanup()
        {
            // Remove any outdated bounties, and remove any posts made by players. Cannot deny player postings without editing BaseBulletinBoard.
            var validBountyPlayers = BountyInformation.GetValidBounties().Select(x=>x.BountyPlayer);
            Items.Where(x => !validBountyPlayers.Contains(((BountyMessage)x).BountyPlayer))
                .ToList()
                .ForEach( message => message.Delete() );
        }

        public override void Delete()
        {
            AllBoards.Remove(this);
            base.Delete();
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

        private static void GloballyCreateMessage(Mobile m)
        {
            foreach(var bb in AllBoards)
                bb.AddBountyToBoard(m);
        }

        private static void GloballyDeleteMessage(Mobile m)
        {
            foreach (var bb in AllBoards)
            {
                var curMsg = bb.Items.FirstOrDefault(x => x is BountyMessage && ((BountyMessage)x).BountyPlayer == m);
                if (curMsg != null)
                    curMsg.Delete();
            }
        }

        private void AddBountyToBoard(Mobile m)
        {
            AddItem(new BountyMessage(m));
        }

        public static void UpdateBounty(PlayerMobile pk)
        {
            // Unfortunately BulletinMessage does not allow updating of the subject. So we have to delete and remake the bounty message.
            GloballyDeleteMessage(pk);
            GloballyCreateMessage(pk);
        }
    }

    public class BountyMessage : BulletinMessage
    {
        public Mobile BountyPlayer;
        public int BountyAmount;
        
        public BountyMessage(Mobile m) : this(m, BountyInformation.GetBounty(m))
        {
        }

        public BountyMessage(Mobile bountied, int bounty) : base(bountied, null, bounty + " gold pieces", CreateDescription(bountied))
        {
            BountyPlayer = bountied;
            BountyAmount = bounty;
        }

        public static string[] CreateDescription(Mobile bountyPlayer)
        {
            string subtext1 = null;
            string subtext2 = null;

            switch (Utility.Random(18))
            {
                case 0: subtext1 = "hath murdered one too many!"; break;
                case 1: subtext1 = "shall not slay again!"; break;
                case 2: subtext1 = "hath slain too many!"; break;
                case 3: subtext1 = "cannot continue to kill!"; break;
                case 4: subtext1 = "must be stopped."; break;
                case 5: subtext1 = "is a bloodthirsty monster."; break;
                case 6: subtext1 = "is a killer of the worst sort."; break;
                case 7: subtext1 = "hath no conscience!"; break;
                case 8: subtext1 = "hath cowardly slain many."; break;
                case 9: subtext1 = "must die for all our sakes."; break;
                case 10: subtext1 = "sheds innocent blood!"; break;
                case 11: subtext1 = "must fall to preserve us."; break;
                case 12: subtext1 = "must be taken care of."; break;
                case 13: subtext1 = "is a thug and must die."; break;
                case 14: subtext1 = "cannot be redeemed."; break;
                case 15: subtext1 = "is a shameless butcher."; break;
                case 16: subtext1 = "is a callous monster."; break;
                case 17: subtext1 = "is a cruel, casual killer."; break;
            }

            switch (Utility.Random(7))
            {
                case 0: subtext2 = "A bounty is hereby offered"; break;
                case 1: subtext2 = "Lord British sets a price"; break;
                case 2: subtext2 = "Claim the reward! 'Tis"; break;
                case 3: subtext2 = "Lord Blackthorn set a price"; break;
                case 4: subtext2 = "The Paladins set a price"; break;
                case 5: subtext2 = "The Merchants set a price"; break;
                case 6: subtext2 = "Lord British's bounty"; break;
            }

            var text = string.Format("The foul scum known as {0} {1} For {2} is responsible for {3} murders. {4} of {5} gold pieces for {6} head!", bountyPlayer.RawName, subtext1, (bountyPlayer.Body.IsFemale ? "she" : "he"), bountyPlayer.Kills, subtext2, BountyInformation.GetBounty(bountyPlayer).ToString(), (bountyPlayer.Body.IsFemale ? "her" : "his"));

            var current = 0;
            var linesList = new List<string>();

            // break up the text into single line length pieces
            while (current < text.Length)
            {
                // make each line 25 chars long
                var length = text.Length - current;

                if (length > 25)
                {
                    length = 25;

                    while (text[current + length] != ' ')
                        length--;

                    length++;
                    linesList.Add(text.Substring(current, length));
                }
                else
                {
                    linesList.Add(string.Format("{0} ", text.Substring(current, length)));
                }

                current += length;
            }

            return linesList.ToArray();
        }

        public BountyMessage(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(BountyPlayer);
            writer.Write(BountyAmount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            BountyPlayer = reader.ReadMobile();
            BountyAmount = reader.ReadInt();

            if (BountyPlayer == null)
                Delete();
        }
    }
}
