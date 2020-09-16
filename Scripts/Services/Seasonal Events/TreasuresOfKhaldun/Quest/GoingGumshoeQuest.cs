using Server.Engines.Quests;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Khaldun
{
    public class GoingGumshoeQuest : BaseQuest
    {
        /* Going Gumshoe */
        public override object Title => 1158588;

        /*You've heard rumblings of Pagan Cultists causing petty crimes in the more upscale sections of Britannia's cities,
         * but to your understanding these are sympathizers taking advantage of fear to make some easy coin. Still, if the
         * Crown is rolling out a new division of the RBG to investigate...something...you bet there's an opportunity for 
         * you! You should find Inspector Jasper and inquire further.*/
        public override object Description => 1158589;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Find Inspector Jasper, and inquire about the Town Cryer article at the new Detective Branch in East Britain */
        public override object Uncomplete => 1158590;

        public override object Complete => null;

        public override int AcceptSound => 0x2E8;
        public override int CompleteMessage => 1158616;  // // You've found Chief Inspector Jasper! Speak to him to learn more!

        public GoingGumshoeQuest()
        {
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch

            AddObjective(new InternalObjective());
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override bool CanOffer()
        {
            return !QuestHelper.HasQuest<GoingGumshoeQuest2>(Owner) &&
                !QuestHelper.HasQuest<GoingGumshoeQuest3>(Owner) &&
                !QuestHelper.HasQuest<GoingGumshoeQuest4>(Owner);
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => Quest.Uncomplete;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }
    }

    public class GoingGumshoeQuest2 : BaseQuest
    {
        /* Going Gumshoe */
        public override object Title => 1158588;

        /*You can drop the coffee pods with the investigators outside...*looks up*...oh, you aren't here for the delivery? I warned 
         * them talking with reporters was a terrible idea - we've been getting walk-ins like you since the article broke, lads 
         * and ladies thinking they can become renowned detectives - Ha! Think you've got the intellect? The cunning and wisdom
         * to sniff a case where it leads you? Well I guess we'll see about that. I've got bigger cases to deal with, you can 
         * take this one. Something about vandalism and a funeral at the Britain Cemetery. Report back to me if you find anything
         * worthwhile - and I do mean worthwhile! Don't come back to me with half baked theories and bogus evidence. I need facts! 
         * Oh - and read this, you'll need it if you even hope to break a single case.*/
        public override object Description => 1158592;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /*What are you still doing here? Didn't I send you to the cemetery? */
        public override object Uncomplete => 1158594;

        public override object Complete => null;

        public override int AcceptSound => 0x2E8;
        public override int CompleteMessage => 1158595;  // TOIDO: This

        // Note to whoever reads this: This is stupid but I find no easier way to do this...
        public bool VisitedHeastone1 { get; set; }
        public bool VisitedHeastone2 { get; set; }
        public bool VisitedHeastone3 { get; set; }
        public bool VisitedHeastone4 { get; set; }

        public bool ClueBook1_1 { get; set; }
        public bool ClueBook1_2 { get; set; }
        public bool ClueDust1 { get; set; }

        public bool ClueBook2_1 { get; set; }
        public bool ClueBook2_2 { get; set; }
        public bool ClueDust2 { get; set; }

        public bool ClueBook3_1 { get; set; }
        public bool ClueBook3_2 { get; set; }
        public bool ClueDust3 { get; set; }

        public bool ClueBook4_1 { get; set; }
        public bool ClueBook4_2 { get; set; }
        public bool ClueDust4 { get; set; }

        public bool IsComplete => ClueBook1_1 && ClueBook1_2 && ClueDust1 &&
                       ClueBook2_1 && ClueBook2_2 && ClueDust2 &&
                       ClueBook3_1 && ClueBook3_2 && ClueDust3 &&
                       ClueBook4_1 && ClueBook4_2 && ClueDust4;

        public GoingGumshoeQuest2()
        {
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch

            AddObjective(new InternalObjective());
        }

        public override void OnAccept()
        {
            base.OnAccept();

            Item book = new DetectiveBook();

            if (Owner.Backpack == null || !Owner.Backpack.TryDropItem(Owner, book, false))
            {
                Owner.BankBox.DropItem(book);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(VisitedHeastone1);
            writer.Write(VisitedHeastone2);
            writer.Write(VisitedHeastone3);
            writer.Write(VisitedHeastone4);

            writer.Write(ClueBook1_1);
            writer.Write(ClueBook1_2);
            writer.Write(ClueDust1);

            writer.Write(ClueBook2_1);
            writer.Write(ClueBook2_2);
            writer.Write(ClueDust2);

            writer.Write(ClueBook3_1);
            writer.Write(ClueBook3_2);
            writer.Write(ClueDust3);

            writer.Write(ClueBook4_1);
            writer.Write(ClueBook4_2);
            writer.Write(ClueDust4);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            VisitedHeastone1 = reader.ReadBool();
            VisitedHeastone2 = reader.ReadBool();
            VisitedHeastone3 = reader.ReadBool();
            VisitedHeastone4 = reader.ReadBool();

            ClueBook1_1 = reader.ReadBool();
            ClueBook1_2 = reader.ReadBool();
            ClueDust1 = reader.ReadBool();

            ClueBook2_1 = reader.ReadBool();
            ClueBook2_2 = reader.ReadBool();
            ClueDust2 = reader.ReadBool();

            ClueBook3_1 = reader.ReadBool();
            ClueBook3_2 = reader.ReadBool();
            ClueDust3 = reader.ReadBool();

            ClueBook4_1 = reader.ReadBool();
            ClueBook4_2 = reader.ReadBool();
            ClueDust4 = reader.ReadBool();
        }

        private class InternalObjective : BaseObjective
        {
            /* You have been assigned as a probationary investigator with the Detective Branch of the RBG. Pursue leads and follow the clues where they lead you... */
            public override object ObjectiveDescription => 1158593;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }
    }

    public class GoingGumshoeQuest3 : BaseQuest
    {
        /* Going Gumshoe */
        public override object Title => 1158588;

        /**You approach Inspector Jasper and show him the evidence you have discovered as well as your report
         * on the state of your investigation* Would you look at that, I suspect you aren't as useless as you 
         * look. With a little luck you may even crack this case wide open! It looks like these notes are encrypted 
         * - we need to find out what they say. There's a cryptologist at the Lycaeum who I've worked with in the
         * past. Get his help and we should be a step closer to figuring out what all this means.*/
        public override object Description => 1158595;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /*What are you still doing here? Didn't I send you to the Lycaeum?*/
        public override object Uncomplete => 1158597;

        public override object Complete => null;

        public override int AcceptSound => 0x2E8;
        public override int CompleteMessage => 1158595;  // TOIDO: This

        public Item BookCase { get; set; }
        public bool FoundCipherBook { get; set; }
        public bool BegunDecrypting { get; set; }

        public bool IsComplete => BegunDecrypting;

        public GoingGumshoeQuest3()
        {
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch

            AddObjective(new InternalObjective());

            if (!World.Loading)
            {
                BookCase = GetBookcase();
            }
        }

        private LibraryBookcase GetBookcase()
        {
            IPooledEnumerable eable = Map.Trammel.GetItemsInBounds(Bounds[Utility.Random(Bounds.Length)]);
            List<Item> bookcases = new List<Item>();

            foreach (Item item in eable)
            {
                if (item is LibraryBookcase)
                {
                    bookcases.Add(item);
                }
            }

            if (bookcases.Count > 0)
            {
                Item bookcase = bookcases[Utility.Random(bookcases.Count)];

                ColUtility.Free(bookcases);

                return (LibraryBookcase)bookcase;
            }

            Console.WriteLine("Error: GoingGumshoeQuest3 has no bookcases. setup LibraryBookcase per EA via [CreateWorld command.");
            return null;
        }

        public static bool CheckBookcase(Mobile from, Item item)
        {
            if (from is PlayerMobile)
            {
                GoingGumshoeQuest3 quest = QuestHelper.GetQuest<GoingGumshoeQuest3>((PlayerMobile)from);

                if (quest != null && !quest.FoundCipherBook)
                {
                    if (quest.BookCase == null)
                    {
                        quest.BookCase = quest.GetBookcase();
                    }

                    if (item == quest.BookCase)
                    {
                        quest.FoundCipherBook = true;

                        from.PrivateOverheadMessage(Network.MessageType.Regular, 0x47E, 1158713, from.NetState);
                        // *You find the cipher text hidden among the books! Return to the Cryptologist to tell him where it is!*

                        Region region = Region.Find(from.Location, from.Map);

                        if (region is QuestRegion)
                        {
                            ((QuestRegion)region).ClearFromMessageTable(from);
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public static Rectangle2D[] Bounds =
        {
            new Rectangle2D(4285, 961, 10, 18),
            new Rectangle2D(4325, 961, 10, 18),
            new Rectangle2D(4285, 989, 10, 18),
            new Rectangle2D(4325, 989, 10, 18)
        };

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(BookCase);
            writer.Write(FoundCipherBook);
            writer.Write(BegunDecrypting);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            BookCase = reader.ReadItem();
            FoundCipherBook = reader.ReadBool();
            BegunDecrypting = reader.ReadBool();

            if (BookCase == null)
            {
                Timer.DelayCall(() => GetBookcase());
            }
        }

        private class InternalObjective : BaseObjective
        {
            /* Go to the Lycaeum in Moonglow and enlist the assistance of the Cryptologist to help decode the evidence. Return to Inspector Jasper 
             * when you have more to report. */
            public override object ObjectiveDescription => 1158596;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }

        private class QuestRegion : Region
        {
            public static void Initialize()
            {
                new QuestRegion();
            }

            public QuestRegion()
                : base("Going Gumshoe Quest Region",
                        Map.Trammel,
                        DefaultPriority,
                        Bounds)
            {
                Register();
            }

            private Dictionary<Mobile, DateTime> _MessageTable;

            public override void OnLocationChanged(Mobile m, Point3D oldLocation)
            {
                base.OnLocationChanged(m, oldLocation);

                if (m is PlayerMobile)
                {
                    GoingGumshoeQuest3 quest = QuestHelper.GetQuest<GoingGumshoeQuest3>((PlayerMobile)m);

                    if (quest != null && !quest.FoundCipherBook && 0.2 > Utility.RandomDouble())
                    {
                        Rectangle2D rec = Bounds.FirstOrDefault(b => b.Contains(m.Location));

                        if (rec.Contains(quest.BookCase) && CanGiveMessage(m))
                        {
                            //m.PrivateOverheadMessage(Server.Network.MessageType.Regular, 0x47E, 1158714, m.NetState); 
                            m.SendLocalizedMessage(1158714);
                            // *Your keen senses detect the books in this library have been recently disturbed...*

                            _MessageTable[m] = DateTime.UtcNow + TimeSpan.FromSeconds(15);
                        }
                    }
                }
            }

            private bool CanGiveMessage(Mobile m)
            {
                if (_MessageTable == null)
                {
                    _MessageTable = new Dictionary<Mobile, DateTime>();

                    return true;
                }

                if (_MessageTable.ContainsKey(m))
                {
                    return _MessageTable[m] < DateTime.UtcNow;
                }

                return true;
            }

            public void ClearFromMessageTable(Mobile m)
            {
                if (_MessageTable != null && _MessageTable.ContainsKey(m))
                {
                    _MessageTable.Remove(m);

                    if (_MessageTable.Count == 0)
                    {
                        _MessageTable = null;
                    }
                }
            }
        }
    }

    public class GoingGumshoeQuest4 : BaseQuest
    {
        /* Going Gumshoe */
        public override object Title => 1158588;

        /*I must say, you've done fine work. The Cryptologist's courier arrived shortly before you did, and thanks to
         * him we know what this is all about. There's one final piece of the puzzle, and hopefully for us whoever
         * is at the bottom of this won't have known to look for a dead man. There is a Sage called Humbolt that 
         * once lived in Papua. Tragically his life was taken from us too soon, but his spirit still wanders around
         * his old home. If anyone can put all the pieces together, it's him. Go to Papua and find him.*/
        public override object Description => 1158634;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /*Go to Papau and search for the spirit of Sage Humbolt*/
        public override object Uncomplete => 1158635;

        public override object Complete => null;

        public override int AcceptSound => 0x2E8;
        public override int CompleteMessage => 1158595;  // TOIDO: This

        public bool IsComplete { get; set; }

        public GoingGumshoeQuest4()
        {
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch

            AddObjective(new InternalObjective());
        }

        public override void GiveRewards()
        {
            Item reward = new DetectiveCredentials();

            if (Owner.Backpack == null || !Owner.Backpack.TryDropItem(Owner, reward, false))
            {
                Owner.BankBox.DropItem(reward);
            }

            reward = new GumshoeTitleDeed();

            if (Owner.Backpack == null || !Owner.Backpack.TryDropItem(Owner, reward, false))
            {
                Owner.BankBox.DropItem(reward);
            }

            base.GiveRewards();

            Owner.DoneQuests.Add(new QuestRestartInfo(typeof(GoingGumshoeQuest), TimeSpan.Zero));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(IsComplete);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            IsComplete = reader.ReadBool();
        }

        private class InternalObjective : BaseObjective
        {
            /* Go to Papau and search for the spirit of Sage Humbolt */
            public override object ObjectiveDescription => 1158635;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }
    }
}
