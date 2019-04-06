using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    [Flipable(0x9A95, 0x9AA7)]
    public class PowerScrollBook : BaseSpecialScrollBook
    {
        public override Type ScrollType { get { return typeof(PowerScroll); } }
        public override int LabelNumber { get { return 1155684; } } // Power Scroll Book
        public override int BadDropMessage { get { return 1155691; } } // This book only holds Power Scrolls.
        public override int DropMessage { get { return 1155692; } }    // You add the scroll to your Power Scroll book.
        public override int RemoveMessage { get { return 1155690; } }  // You remove a Power Scroll and put it in your pack.
        public override int GumpTitle { get { return 1155689; } }  // Power Scrolls

        [Constructable]
        public PowerScrollBook() 
            : base(0x9A95)
        {
            Hue = 1153;
        }

        public PowerScrollBook(Serial serial)
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

            _SkillInfo[SkillCat.Miscellaneous] = new List<SkillName>();
            _SkillInfo[SkillCat.Combat] = new List<SkillName>() { SkillName.Anatomy, SkillName.Archery, SkillName.Fencing, SkillName.Focus, SkillName.Healing, SkillName.Macing, SkillName.Parry, SkillName.Swords, SkillName.Tactics, SkillName.Throwing, SkillName.Wrestling };
            _SkillInfo[SkillCat.TradeSkills] = new List<SkillName>() { SkillName.Blacksmith, SkillName.Tailoring };
            _SkillInfo[SkillCat.Magic] = new List<SkillName>() { SkillName.Bushido, SkillName.Chivalry, SkillName.EvalInt, SkillName.Imbuing, SkillName.Magery, SkillName.Meditation, SkillName.Mysticism, SkillName.Necromancy, SkillName.Ninjitsu, SkillName.MagicResist, SkillName.Spellweaving, SkillName.SpiritSpeak };
            _SkillInfo[SkillCat.Wilderness] = new List<SkillName>() { SkillName.AnimalLore, SkillName.AnimalTaming, SkillName.Fishing, SkillName.Veterinary };
            _SkillInfo[SkillCat.Thievery] = new List<SkillName>() { SkillName.Stealing, SkillName.Stealth };
            _SkillInfo[SkillCat.Bard] = new List<SkillName>() { SkillName.Discordance, SkillName.Musicianship, SkillName.Peacemaking, SkillName.Provocation };

            _ValueInfo = new Dictionary<int, double>();

            _ValueInfo[1155685] = 105;
            _ValueInfo[1155686] = 110;
            _ValueInfo[1155687] = 115;
            _ValueInfo[1155688] = 120;
        }
    }
}