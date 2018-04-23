using Server.Items;
using Server.Misc;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class BountyGuard : WarriorGuard
    {
        [Constructable]
        public BountyGuard()
        {
        }

        private static readonly List<int> RejectedHeadSayings = new List<int>()
        {
            500661, // I shall place this on my mantle!
            500662, // This tasteth like chicken.
            500663, // This tasteth just like the juicy peach I just ate.
            500664, // Ahh!  That was the one piece I was missing!
            500665, // Somehow, it reminds me of mother.
            500666, // It's a sign!  I can see Elvis in this!
            500667, // Thanks, I was missing mine.
            500668, // I'll put this in the lost-and-found box.
            500669 // My family will eat well tonight!
        };

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            var bh = dropped as BountiedHead;
            if (bh != null && bh.Player != null && !bh.Player.Deleted &&
                bh.Created + TimeSpan.FromDays(1.0) > DateTime.UtcNow)
            {
                SayTo(from, 500670); // Ah, a head!  Let me check to see if there is a bounty on this.
                Timer.DelayCall(TimeSpan.FromSeconds(5.0), CheckBountyOnHead, new object[] {bh, from});
                return true;
            }
            else if (dropped is Head)
            {
                Say(RejectedHeadSayings[Utility.Random(RejectedHeadSayings.Count)]);
                dropped.Delete();
                return true;
            }

            SayTo(from, 500660); // If this were the head of a murderer, I would check for a bounty.
            return false;
        }

        private void CheckBountyOnHead(object[] states)
        {
            var head = (BountiedHead) states[0];
            var bountyHunter = (Mobile) states[1];
            var bi = BountyInformation.GetBountyInformation(head.Player);

            if (bi == null)
            {
                Say("The reward on this scoundrel's head has already been claimed!");
                head.Delete();
                return;
            }

            var totalBounty = bi.Bounty;
            var headBounty = head.Bounty;
            var difference = totalBounty - headBounty;

            if (difference < 0)
            {
                Say("The reward on this scoundrel's head has already been claimed!");
                head.Delete();
                return;
            }

            bi.SubtractBounty(headBounty);
            AwardBounty(bountyHunter, head.PlayerName, headBounty);
            head.Delete();
        }

        private void AwardBounty(Mobile bountyHunter, string killer, int total)
        {
            var bountySize = 0;

            if (total > 15000)
                bountySize = 2;
            else if (total > 100)
                bountySize = 1;

            switch (bountySize)
            {
                case 2:
                    bountyHunter.PlaySound(0x2E6);
                    Say("Thou hast brought the infamous " + killer + " to justice!  Thy reward of " + total + "gp hath been deposited in thy bank account.");
                    break;
                case 1:
                    bountyHunter.PlaySound(0x2E5);
                    Say("Tis a minor criminal, thank thee. Thy reward of " + total + "gp hath been deposited in thy bank account.");
                    break;
                default:
                    bountyHunter.PlaySound(0x2E4);
                    Say("Thou hast wasted thy time for " + total + "gp.");
                    break;
            }

            Banker.Deposit(bountyHunter, total);
        }

        public BountyGuard(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            var version = reader.ReadInt();
        }
    }
}