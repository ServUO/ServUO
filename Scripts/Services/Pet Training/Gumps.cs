using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.SkillHandlers;

namespace Server.Mobiles
{
    public class NewAnimalLoreGump : BaseGump
    {
        private int _Label = 0xF424E5;

        public BaseCreature Creature { get; private set; }

        public NewAnimalLoreGump(PlayerMobile pm, BaseCreature bc)
            : base(pm, 250, 50)
        {
            Creature = bc;
        }

        public override void AddGumpLayout()
        {
            var profile = PetTrainingHelper.GetProfile(Creature);

            AddPage(0);
            AddBackground(0, 24, 310, 325, 0x24A4);
            AddHtml(47, 32, 210, 18, ColorAndCenter("#000080", Creature.Name), false, false);

            AddButton(140, 0, 0x82D, 0x82D, 0, GumpButtonType.Reply, 0);

            AddImage(40, 62, 0x82B);
            AddImage(40, 258, 0x82B);
            AddImage(28, 272, 0x826);

            AddHtmlLocalized(47, 270, 160, 18, 1157491, 0xC8, false, false); // Pet Training Progress:
            AddImage(53, 290, 0x805);

            double progress = 100.0; // TODO: Get training progress from BaseCreature
            AddBackground(53, 290, (int)(109.0 * (progress / 100)), 11, 0x806);

            AddHtml(162, 285, 50, 18, FormatDouble(progress) + "%", false, false);
            AddButton(215, 288, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
            AddTooltip(1157568); // View real-time training progress. 

            AddButton(53, 305, 0x837, 0x838, 2, GumpButtonType.Reply, 0);
            AddHtmlLocalized(73, 300, 160, 18, 1157492, false, false); // Pet Training Options

            AddButton(250, 280, 0x9AA, 0x9A9, 3, GumpButtonType.Reply, 0);
            AddTooltip(1158013); // Cancel Training Process. All remaining points will be removed.

            AddPage(1);

            AddImage(28, 76, 0x826);
            AddHtmlLocalized(47, 74, 160, 18, 1049593, 0xC8, false, false); // Attributes

            AddHtmlLocalized(53, 92, 160, 18, 1049578, _Label, false, false); // Hits
            AddHtml(180, 92, 75, 18, FormatAttributes(Creature.Hits, Creature.HitsMax), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1049579, _Label, false, false); // Stamina
            AddHtml(180, 110, 75, 18, FormatAttributes(Creature.Stam, Creature.StamMax), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1049580, _Label, false, false); // Mana
            AddHtml(180, 128, 75, 18, FormatAttributes(Creature.Mana, Creature.ManaMax), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1028335, _Label, false, false); // Strength
            AddHtml(180, 146, 75, 18, FormatStat(Creature.RawStr), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 3000113, _Label, false, false); // Dexterity
            AddHtml(180, 164, 75, 18, FormatStat(Creature.RawDex), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 3000112, _Label, false, false); // Intelligence
            AddHtml(180, 182, 75, 18, FormatStat(Creature.RawInt), false, false);

            double bd = Items.BaseInstrument.GetBaseDifficulty(Creature);

            if (Creature.Uncalmable)
                bd = 0;

            AddHtmlLocalized(53, 200, 160, 18, 1070793, _Label, false, false); // Barding Difficulty
            AddHtml(180, 200, 75, 18, FormatDouble(bd), false, false);

            AddImage(28, 220, 0x826);

            AddHtmlLocalized(47, 220, 160, 18, 1049594, 0xC8, false, false); // Loyalty Rating
            AddHtmlLocalized(53, 236, 160, 18, (!Creature.Controlled || Creature.Loyalty == 0) ? 1061643 : 1049595 + (Creature.Loyalty / 10), _Label, false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 2);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, Pages(profile));

            AddPage(2);

            AddImage(28, 76, 0x826);
            AddHtmlLocalized(47, 74, 160, 18, 1049593, 0xC8, false, false); // Attributes

            AddHtmlLocalized(53, 92, 160, 18, 1075627, _Label, false, false); // Hit Point Regeneration
            AddHtml(180, 92, 75, 18, FormatStat(profile == null ? 0 : profile.RegenHits), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1079411, _Label, false, false); // Stamina Regeneration
            AddHtml(180, 110, 75, 18, FormatStat(profile == null ? 0 : profile.RegenStam), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1079410, _Label, false, false); // Mana Regeneration
            AddHtml(180, 128, 75, 18, FormatStat(profile == null ? 0 : profile.RegenMana), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 3);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 1);

            AddPage(3);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 1061645, 0xC8, false, false); // Resistances

            AddHtmlLocalized(53, 92, 160, 18, 1061646, _Label, false, false); // Physical
            AddHtml(180, 92, 75, 18, FormatElement(Creature.PhysicalResistance, "#000000"), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1061647, _Label, false, false); // Fire
            AddHtml(180, 110, 75, 18, FormatElement(Creature.FireResistance, "#FF0000"), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1061648, _Label, false, false); // Cold
            AddHtml(180, 128, 75, 18, FormatElement(Creature.ColdResistance, "#000080"), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1061649, _Label, false, false); // Poison
            AddHtml(180, 146, 75, 18, FormatElement(Creature.PoisonResistance, "#008000"), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1061650, _Label, false, false); // Energy
            AddHtml(180, 164, 75, 18, FormatElement(Creature.EnergyResistance, "#BF80FF"), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 4);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 2);

            AddPage(4);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 1017319, 0xC8, false, false); // Damage

            AddHtmlLocalized(53, 92, 160, 18, 1061646, _Label, false, false); // Physical
            AddHtml(180, 92, 75, 18, FormatElement(Creature.PhysicalDamage, "#000000"), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1061647, _Label, false, false); // Fire
            AddHtml(180, 110, 75, 18, FormatElement(Creature.FireDamage, "#FF0000"), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1061648, _Label, false, false); // Cold
            AddHtml(180, 128, 75, 18, FormatElement(Creature.ColdDamage, "#000080"), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1061649, _Label, false, false); // Poison
            AddHtml(180, 146, 75, 18, FormatElement(Creature.PoisonDamage, "#008000"), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1061650, _Label, false, false); // Energy
            AddHtml(180, 164, 75, 18, FormatElement(Creature.EnergyDamage, "#BF80FF"), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 1076750, _Label, false, false);  // Base Damage
            AddHtml(180, 182, 75, 18, FormatDamage(Creature.DamageMin, Creature.DamageMax), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 5);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 3);

            AddPage(5);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001030, 0xC8, false, false); // Combat Ratings

            AddHtmlLocalized(53, 92, 160, 18, 1044103, _Label, false, false); // Wrestling
            AddHtml(180, 92, 75, 18, FormatSkill(Creature, SkillName.Wrestling), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044087, _Label, false, false); // Tactics
            AddHtml(180, 110, 35, 18, FormatSkill(Creature, SkillName.Tactics), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044086, _Label, false, false); // Magic Resistance
            AddHtml(180, 128, 35, 18, FormatSkill(Creature, SkillName.MagicResist), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1044061, _Label, false, false); // Anatomy
            AddHtml(180, 146, 35, 18, FormatSkill(Creature, SkillName.Anatomy), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1002082, _Label, false, false); // Healing
            AddHtml(180, 164, 35, 18, FormatSkill(Creature, SkillName.Healing), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 1002122, _Label, false, false); // Poisoning
            AddHtml(180, 182, 35, 18, FormatSkill(Creature, SkillName.Poisoning), false, false);

            AddHtmlLocalized(53, 200, 160, 18, 1044110, _Label, false, false); // Detect Hidden
            AddHtml(180, 200, 35, 18, FormatSkill(Creature, SkillName.DetectHidden), false, false);

            AddHtmlLocalized(53, 218, 160, 18, 1002088, _Label, false, false); // Hiding
            AddHtml(180, 218, 35, 18, FormatSkill(Creature, SkillName.Hiding), false, false);

            AddHtmlLocalized(53, 236, 160, 18, 1002118, _Label, false, false); // Parrying
            AddHtml(180, 236, 35, 18, FormatSkill(Creature, SkillName.Parry), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 6);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 4);

            AddPage(6);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001032, 0xC8, false, false); // Lore & Knowledge

            AddHtmlLocalized(53, 92, 160, 18, 1044085, _Label, false, false); // Magery
            AddHtml(180, 92, 75, 18, FormatSkill(Creature, SkillName.Magery), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044076, _Label, false, false); // Eval Int
            AddHtml(180, 110, 35, 18, FormatSkill(Creature, SkillName.EvalInt), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044106, _Label, false, false); // Meditation
            AddHtml(180, 128, 35, 18, FormatSkill(Creature, SkillName.Meditation), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1044109, _Label, false, false); // Necromancy
            AddHtml(180, 146, 35, 18, FormatSkill(Creature, SkillName.Necromancy), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1002140, _Label, false, false); // Spirit Speak
            AddHtml(180, 164, 35, 18, FormatSkill(Creature, SkillName.SpiritSpeak), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 1044115, _Label, false, false); // Mysticism
            AddHtml(180, 182, 35, 18, FormatSkill(Creature, SkillName.Mysticism), false, false);

            AddHtmlLocalized(53, 200, 160, 18, 1044110, _Label, false, false); // Focus
            AddHtml(180, 200, 35, 18, FormatSkill(Creature, SkillName.Focus), false, false);

            AddHtmlLocalized(53, 218, 160, 18, 1044114, _Label, false, false); // Spellweaving
            AddHtml(180, 218, 35, 18, FormatSkill(Creature, SkillName.Spellweaving), false, false);

            AddHtmlLocalized(53, 236, 160, 18, 1044075, _Label, false, false); // Discordance
            AddHtml(180, 236, 35, 18, FormatSkill(Creature, SkillName.Discordance), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 7);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 5);

            AddPage(7);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001032, 0xC8, false, false); // Lore & Knowledge

            AddHtmlLocalized(53, 92, 160, 18, 1044112, _Label, false, false); // Bushido
            AddHtml(180, 92, 75, 18, FormatSkill(Creature, SkillName.Bushido), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044113, _Label, false, false); // Ninjitsu
            AddHtml(180, 110, 35, 18, FormatSkill(Creature, SkillName.Ninjitsu), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044111, _Label, false, false); // Chivalry
            AddHtml(180, 128, 35, 18, FormatSkill(Creature, SkillName.Chivalry), false, false);

            AddImage(28, 146, 0x826);

            AddHtmlLocalized(47, 144, 160, 18, 1049563, 0xC8, false, false); // Preferred Foods

            int foodPref = 3000340;

            if ((Creature.FavoriteFood & FoodType.FruitsAndVegies) != 0)
                foodPref = 1049565; // Fruits and Vegetables
            else if ((Creature.FavoriteFood & FoodType.GrainsAndHay) != 0)
                foodPref = 1049566; // Grains and Hay
            else if ((Creature.FavoriteFood & FoodType.Fish) != 0)
                foodPref = 1049568; // Fish
            else if ((Creature.FavoriteFood & FoodType.Meat) != 0)
                foodPref = 1049564; // Meat
            else if ((Creature.FavoriteFood & FoodType.Eggs) != 0)
                foodPref = 1044477; // Eggs

            AddHtmlLocalized(53, 164, 160, 18, foodPref, _Label, false, false);

            AddImage(28, 182, 0x826);

            AddHtmlLocalized(47, 200, 160, 18, 1049569, 0xC8, false, false); // Pack Instincts

            int packInstinct = 3000340;

            if ((Creature.PackInstinct & PackInstinct.Canine) != 0)
                packInstinct = 1049570; // Canine
            else if ((Creature.PackInstinct & PackInstinct.Ostard) != 0)
                packInstinct = 1049571; // Ostard
            else if ((Creature.PackInstinct & PackInstinct.Feline) != 0)
                packInstinct = 1049572; // Feline
            else if ((Creature.PackInstinct & PackInstinct.Arachnid) != 0)
                packInstinct = 1049573; // Arachnid
            else if ((Creature.PackInstinct & PackInstinct.Daemon) != 0)
                packInstinct = 1049574; // Daemon
            else if ((Creature.PackInstinct & PackInstinct.Bear) != 0)
                packInstinct = 1049575; // Bear
            else if ((Creature.PackInstinct & PackInstinct.Equine) != 0)
                packInstinct = 1049576; // Equine
            else if ((Creature.PackInstinct & PackInstinct.Bull) != 0)
                packInstinct = 1049577; // Bull

            AddHtmlLocalized(53, 200, 160, 18, packInstinct, _Label, false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 8);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 6);

            AddImage(28, 218, 0x826);

            AddHtmlLocalized(47, 216, 160, 18, 1115783, 0xC8, false, false); // Pet Slots
            AddHtml(53, 236, 80, 18, FormatPetSlots(Creature.MinPetSlots, Creature.MaxPetSlots), false, false);

            AddHtmlLocalized(158, 236, 115, 18, 1157600, Creature.MinTameSkill.ToString(), _Label, false, false);
            AddTooltip(1157586);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 8);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 6);

            AddPage(8);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001032, 0xC8, false, false); // Lore & Knowledge

            if (profile != null)
            {
                int y = 92;

                foreach (object o in profile.EnumerateAllAbilities())
                {
                    var loc = PetTrainingHelper.GetLocalization(o);

                    AddHtmlLocalized(53, y, 180, 18, loc[0], _Label, false, false);
                    AddTooltip(loc[1]);

                    y += 18;
                }

                if (profile.History != null)
                {
                    AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 9);
                    AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 7);

                    AddPage(9);

                    AddImage(28, 76, 0x826);

                    AddHtmlLocalized(47, 74, 160, 18, 1157505, 0xC8, false, false); // Pet Advancements

                    for (int i = 0; i > profile.History.Count; i++)
                    {
                        var loc = PetTrainingHelper.GetLocalization(profile.History[i]);

                        AddHtmlLocalized(53, y, 180, 18, loc[0], 0xFF00, false, false);
                        AddTooltip(PetTrainingHelper.GetCategoryLocalization(profile.History[i]));

                        y += 18;
                    }

                    AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 8);
                    AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 1);
                }
            }
        }

        public int Pages(AbilityProfile profile)
        {
            if (profile == null || profile.History == null || profile.History.Count == 0)
                return 8;

            return 9;
        }

        private static string FormatSkill(BaseCreature c, SkillName name)
        {
            return AnimalLoreGump.FormatSkills(c, name);
        }

        private static string FormatAttributes(int cur, int max)
        {
            return AnimalLoreGump.FormatAttributes(cur, max);
        }

        private static string FormatStat(int val)
        {
            return AnimalLoreGump.FormatStat(val);
        }

        private static string FormatDouble(double val)
        {
            return AnimalLoreGump.FormatDouble(val);
        }

        private static string FormatElement(int val, string color)
        {
            if (val <= 0)
                return String.Format("<BASEFONT COLOR={0}><div align=right>---</div>", color);

            return String.Format("<BASEFONT COLOR={1}><div align=right>{0}%</div>", val, color);
        }

        private static string FormatDamage(int min, int max)
        {
            return AnimalLoreGump.FormatDamage(min, max);
        }

        private string FormatPetSlots(int min, int max)
        {
            return String.Format("<BASEFONT COLOR=#57412F>{0} => {1}", min.ToString, max.ToString());
        }
    }

    public class BaseAnimalTrainingGump : BaseGump
    {
        public BaseAnimalTrainingGump(PlayerMobile pm, BaseCreature bc)
            : base(250, 50)
        {
        }

        public override void AddGumpLayout()
        {
        }
    }
}
