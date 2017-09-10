using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x9981, 0x9982)]
    public class ScrollOfAlacrityBook : BaseSpecialScrollBook
    {
        public override Type ScrollType { get { return typeof(ScrollofAlacrity); } }
        public override int LabelNumber { get { return 1154321; } } // Scrolls of Alacrity Book
        public override int BadDropMessage { get { return 1154323; } } // This book only holds Scrolls of Alacrity.
        public override int DropMessage { get { return 1154326; } }    // You add the scroll to your Scrolls of Alacrity book.
        public override int RemoveMessage { get { return 1154322; } }  // You remove a Scroll of Alacrity and put it in your pack.
        public override int GumpTitle { get { return 1154324; } }  // Alacrity Scrolls

        [Constructable]
        public ScrollOfAlacrityBook()
            : base(0x9981)
        {
            Hue = 1195;
        }

        public ScrollOfAlacrityBook(Serial serial)
            : base(serial)
        {
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

        public override Dictionary<SkillCat, List<SkillName>> SkillInfo { get { return _SkillInfo; } }
        public override Dictionary<int, double> ValueInfo { get { return _ValueInfo; } }

        public static Dictionary<SkillCat, List<SkillName>> _SkillInfo;
        public static Dictionary<int, double> _ValueInfo;

        public static void Initialize()
        {
            _SkillInfo = new Dictionary<SkillCat, List<SkillName>>();

            _SkillInfo[SkillCat.Miscellaneous] = new List<SkillName>() { SkillName.ArmsLore, SkillName.Begging, SkillName.Camping, SkillName.Cartography, SkillName.Forensics, SkillName.ItemID, SkillName.TasteID};
            _SkillInfo[SkillCat.Combat] = new List<SkillName>() { SkillName.Anatomy, SkillName.Archery, SkillName.Fencing, SkillName.Focus, SkillName.Healing, SkillName.Macing, SkillName.Parry, SkillName.Swords, SkillName.Tactics, SkillName.Throwing, SkillName.Wrestling };
            _SkillInfo[SkillCat.TradeSkills] = new List<SkillName>() { SkillName.Alchemy, SkillName.Blacksmith, SkillName.Fletching, SkillName.Carpentry, SkillName.Cooking, SkillName.Inscribe, SkillName.Lumberjacking, SkillName.Mining, SkillName.Tailoring, SkillName.Tinkering };
            _SkillInfo[SkillCat.Magic] = new List<SkillName>() { SkillName.Bushido, SkillName.Chivalry, SkillName.EvalInt, SkillName.Imbuing, SkillName.Magery, SkillName.Meditation, SkillName.Mysticism, SkillName.Necromancy, SkillName.Ninjitsu, SkillName.MagicResist, SkillName.Spellweaving, SkillName.SpiritSpeak };
            _SkillInfo[SkillCat.Wilderness] = new List<SkillName>() { SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Fishing, SkillName.Herding, SkillName.Tracking, SkillName.Veterinary };
            _SkillInfo[SkillCat.Thievery] = new List<SkillName>() { SkillName.DetectHidden, SkillName.Hiding, SkillName.Lockpicking, SkillName.Poisoning, SkillName.RemoveTrap, SkillName.Snooping, SkillName.Stealing, SkillName.Stealth };
            _SkillInfo[SkillCat.Bard] = new List<SkillName>() { SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation };

            _ValueInfo = new Dictionary<int, double>();

            _ValueInfo[1154324] = 0.0;
        }
    }
}