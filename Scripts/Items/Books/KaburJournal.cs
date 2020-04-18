namespace Server.Items
{
    public class KaburJournal : RedBook
    {
        public static readonly BookContent Content = new BookContent(
            "Journal", "Kabur",
            new BookPageInfo(
                "The campaign to slaughter",
                "the Meer goes well.",
                "Although they seem to",
                "oppose the forces of",
                "ours at every turn, we",
                "still defeat them in",
                "combat.  Spies of the",
                "Meer have been found and"),
            new BookPageInfo(
                "slain outside of the",
                "fortress of ours.  The",
                "fools underestimate us.",
                "We have the power of",
                "Lord Exodus behind us.",
                "Soon they will learn to",
                "serve the Juka and I",
                "shall carry the head of"),
            new BookPageInfo(
                "the wench, Dasha, on a",
                "spike for all the warriors",
                "of ours to share triumph",
                "under.",
                "",
                "One of the warriors of",
                "the Juka died today.",
                "During the training"),
            new BookPageInfo(
                "exercises of ours he",
                "spoke out in favor of",
                "the warriors of the",
                "Meer, saying that they",
                "were indeed powerful and",
                "would provide a challenge",
                "to the Juka.  A Juka in",
                "fear is no Juka.  I gave"),
            new BookPageInfo(
                "him the death of a",
                "coward, outside of battle.",
                "",
                "More spies of the Meer",
                "have been found around",
                "the fortress of ours.",
                "Many have been seen and",
                "escaped the wrath of the"),
            new BookPageInfo(
                "warriors of ours.  Those",
                "who have been captured",
                "and tortured have",
                "revealed nothing to us,",
                "even when subjected to",
                "the spells of the females.",
                " I know the Meer must",
                "have plans against us if"),
            new BookPageInfo(
                "they send so many spies.",
                " I may send the troops",
                "of the Juka to invade",
                "the camps of theirs as a",
                "warning.",
                "",
                "I have met Dasha in",
                "battle this day.  The"),
            new BookPageInfo(
                "efforts of hers to draw",
                "me into a Black Duel",
                "were foolish.   Had we",
                "not been interrupted in",
                "the cave I would have",
                "ended the life of hers",
                "but I will have to wait",
                "for another battle.  Lord"),
            new BookPageInfo(
                "Exodus has ordered more",
                "patrols around the",
                "fortress of ours.  If",
                "Dasha is any indication,",
                "the Meer will strike soon.",
                "",
                "More Meer stand outside",
                "of the fortress of ours"),
            new BookPageInfo(
                "than I have ever seen at",
                "once.  They must seek",
                "vengeance for the",
                "destruction of their",
                "forest.  Many Juka stand",
                "ready at the base of the",
                "mountain to face the",
                "forces of theirs but"),
            new BookPageInfo(
                "today may be the final",
                "battle.  Exodus has",
                "summoned me, I must",
                "prepare.",
                "",
                "Dusk has passed and the",
                "Juka now live in a new",
                "age, a later time.  I have"),
            new BookPageInfo(
                "just returned from",
                "exploring the new world",
                "that surrounds the",
                "fortress of the Juka.",
                "During the attack of the",
                "Meer the madman",
                "Adranath tried to destroy",
                "the fortress of ours"),
            new BookPageInfo(
                "with great magic.  At",
                "once he was still and",
                "light surrounded the",
                "fortress.  Everything",
                "faded from view.  When I",
                "regained the senses of",
                "mine I saw no sign of",
                "the Meer but Dasha."),
            new BookPageInfo(
                "She has not been found",
                "since this new being,",
                "Blackthorn, blasted her",
                "from the top of the",
                "fortress.",
                "The forest was gone, now",
                "replaced by grasslands.",
                "In the far distance I"),
            new BookPageInfo(
                "could see humans that",
                "had covered the bodies of",
                "theirs in marks.  Even",
                "Gargoyles populate this",
                "place.  Exodus has",
                "explained to me that the",
                "Juka and the fortress of",
                "ours have been pulled"),
            new BookPageInfo(
                "forward in time.  The",
                "world we knew is now",
                "thousands of years in the",
                "past.  Lord Exodus say",
                "he has has saved the",
                "Juka from extinction.  I",
                "do not want to believe",
                "him.  I asked this"),
            new BookPageInfo(
                "stranger about the Meer,",
                "but he tells me a new",
                "enemy remains to be",
                "destroyed.  It seems the",
                "enemies of ours have",
                "passed away to dust like",
                "the forest."),
            new BookPageInfo(
                "I have spoken with other",
                "Juka and I suspect I have",
                "been told the truth.  All",
                "the Juka had powerful",
                "dreams.  In the dreams",
                "of ours the Meer invaded",
                "the fortress of ours and",
                "a great battle took place."),
            new BookPageInfo(
                " All the Juka and all the",
                "Meer perished and the",
                "fortress was destroyed",
                "from Adranath's spells.  I",
                "would not like to believe",
                "that the Meer could ever",
                "destroy us, but now it",
                "seems we have seen a"),
            new BookPageInfo(
                "vision of the fate of",
                "ours now lost in time.  I",
                "must now wonder if the",
                "Meer did not die in the",
                "battle with the Juka, how",
                "did they die?  And more",
                "importantly, where is",
                "Dasha?"));
        [Constructable]
        public KaburJournal()
            : base(false)
        {
        }

        public KaburJournal(Serial serial)
            : base(serial)
        {
        }

        public override BookContent DefaultContent => Content;
        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add("Khabur's Journal");
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
}
