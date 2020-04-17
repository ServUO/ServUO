using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x577E, 0x577F)]
    public class ScrollOfTranscendenceBook : BaseSpecialScrollBook
    {
        public override Type ScrollType => typeof(ScrollOfTranscendence);
        public override int LabelNumber => 1151679;  // Scrolls of Transcendence Book
        public override int BadDropMessage => 1151677;  // This book only holds Scrolls of Transcendence.
        public override int DropMessage => 1151674;     // You add the scroll to your Scrolls of Transcendence book.
        public override int RemoveMessage => 1151658;   // You remove a Scroll of Transcendence and put it in your pack. 
        public override int GumpTitle => 1151675;   // Scrolls of Transcendence

        [Constructable]
        public ScrollOfTranscendenceBook()
            : base(0x577E)
        {
            Hue = 0x490;
        }

        public ScrollOfTranscendenceBook(Serial serial)
            : base(serial)
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

        public override Dictionary<SkillCat, List<SkillName>> SkillInfo => _SkillInfo;
        public override Dictionary<int, double> ValueInfo => _ValueInfo;

        public static Dictionary<SkillCat, List<SkillName>> _SkillInfo;
        public static Dictionary<int, double> _ValueInfo;

        public static void Initialize()
        {
            _SkillInfo = new Dictionary<SkillCat, List<SkillName>>();

            _SkillInfo[SkillCat.Miscellaneous] = new List<SkillName>() { SkillName.ArmsLore, SkillName.Begging, SkillName.Camping, SkillName.Cartography, SkillName.Forensics, SkillName.ItemID, SkillName.TasteID };
            _SkillInfo[SkillCat.Combat] = new List<SkillName>() { SkillName.Anatomy, SkillName.Archery, SkillName.Fencing, SkillName.Focus, SkillName.Healing, SkillName.Macing, SkillName.Parry, SkillName.Swords, SkillName.Tactics, SkillName.Throwing, SkillName.Wrestling };
            _SkillInfo[SkillCat.TradeSkills] = new List<SkillName>() { SkillName.Alchemy, SkillName.Blacksmith, SkillName.Fletching, SkillName.Carpentry, SkillName.Cooking, SkillName.Inscribe, SkillName.Lumberjacking, SkillName.Mining, SkillName.Tailoring, SkillName.Tinkering };
            _SkillInfo[SkillCat.Magic] = new List<SkillName>() { SkillName.Bushido, SkillName.Chivalry, SkillName.EvalInt, SkillName.Imbuing, SkillName.Magery, SkillName.Meditation, SkillName.Mysticism, SkillName.Necromancy, SkillName.Ninjitsu, SkillName.MagicResist, SkillName.Spellweaving, SkillName.SpiritSpeak };
            _SkillInfo[SkillCat.Wilderness] = new List<SkillName>() { SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Fishing, SkillName.Herding, SkillName.Tracking, SkillName.Veterinary };
            _SkillInfo[SkillCat.Thievery] = new List<SkillName>() { SkillName.DetectHidden, SkillName.Hiding, SkillName.Lockpicking, SkillName.Poisoning, SkillName.RemoveTrap, SkillName.Snooping, SkillName.Stealing, SkillName.Stealth };
            _SkillInfo[SkillCat.Bard] = new List<SkillName>() { SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation };

            _ValueInfo = new Dictionary<int, double>();

            _ValueInfo[1151659] = 0.1;
            _ValueInfo[1151660] = 0.2;
            _ValueInfo[1151661] = 0.3;
            _ValueInfo[1151662] = 0.4;
            _ValueInfo[1151663] = 0.5;
            _ValueInfo[1151664] = 0.6;
            _ValueInfo[1151665] = 0.7;
            _ValueInfo[1151666] = 0.8;
            _ValueInfo[1151667] = 0.9;
            _ValueInfo[1151668] = 1.0;
            _ValueInfo[1151669] = 3.0;
            _ValueInfo[1151670] = 5.0;
        }
    }
}