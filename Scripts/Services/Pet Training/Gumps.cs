using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.SkillHandlers;
using System.Collections.Generic;
using System.Linq;
using Server.Misc;

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
            var profile = PetTrainingHelper.GetAbilityProfile(Creature);

            AddPage(0);
            AddBackground(0, 24, 310, 325, 0x24A4);
            AddHtml(47, 32, 210, 18, ColorAndCenter("#000080", Creature.Name), false, false);

            AddButton(140, 0, 0x82D, 0x82D, 0, GumpButtonType.Reply, 0);

            AddImage(40, 62, 0x82B);
            AddImage(40, 258, 0x82B);

            if (Creature.Controlled && Creature.ControlMaster == User)
            {
                AddImage(28, 272, 0x826);

                var trainProfile = PetTrainingHelper.GetTrainingProfile(Creature, true);
                var def = PetTrainingHelper.GetTrainingDefinition(Creature);

                if (trainProfile.HasBegunTraining && def != null && def.Class != Class.Untrainable)
                {
                    AddImage(53, 290, 0x805);

                    AddHtmlLocalized(47, 270, 160, 18, 1157491, 0xC8, false, false); // Pet Training Progress:

                    double progress = trainProfile.TrainingProgressPercentile * 100;

                    if (progress >= 1)
                    {
                        AddBackground(53, 290, (int)(109.0 * (progress / 100)), 11, 0x806);
                    }

                    AddHtml(162, 285, 50, 18, FormatDouble(progress, false, true), false, false);
                    AddButton(215, 288, 0x15E1, 0x15E5, 1, GumpButtonType.Reply, 0);
                    AddTooltip(1157568); // View real-time training progress. 

                    if (trainProfile.CanApplyOptions)
                    {
                        AddButton(53, 305, 0x837, 0x838, 2, GumpButtonType.Reply, 0);
                        AddHtmlLocalized(73, 300, 160, 18, 1157492, false, false); // Pet Training Options
                    }

                    AddButton(250, 280, 0x9AA, 0x9A9, 3, GumpButtonType.Reply, 0);
                    AddTooltip(1158013); // Cancel Training Process. All remaining points will be removed.
                }
                else if (Creature.ControlSlots < Creature.ControlSlotsMax)
                {
                    AddHtmlLocalized(47, 270, 160, 18, 1157487, 0xC8, false, false); // Begin Animal Training
                    AddButton(53, 288, 0x837, 0x838, 4, GumpButtonType.Reply, 0);
                }
            }

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
            AddHtml(180, 92, 75, 18, FormatStat(RegenRates.HitPointRegen(Creature)), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1079411, _Label, false, false); // Stamina Regeneration
            AddHtml(180, 110, 75, 18, FormatStat(RegenRates.StamRegen(Creature)), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1079410, _Label, false, false); // Mana Regeneration
            AddHtml(180, 128, 75, 18, FormatStat(RegenRates.ManaRegen(Creature)), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 3);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 1);

            AddPage(3);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 1061645, 0xC8, false, false); // Resistances

            AddHtmlLocalized(53, 92, 160, 18, 1061646, _Label, false, false); // Physical
            AddHtml(180, 92, 75, 18, FormatElement(Creature.PhysicalResistance, null), false, false);

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
            AddHtml(180, 92, 75, 18, FormatElement(Creature.PhysicalDamage, null), false, false);

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
            AddHtml(180, 92, 100, 18, FormatSkill(Creature, SkillName.Wrestling), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044087, _Label, false, false); // Tactics
            AddHtml(180, 110, 100, 18, FormatSkill(Creature, SkillName.Tactics), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044086, _Label, false, false); // Magic Resistance
            AddHtml(180, 128, 100, 18, FormatSkill(Creature, SkillName.MagicResist), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1044061, _Label, false, false); // Anatomy
            AddHtml(180, 146, 100, 18, FormatSkill(Creature, SkillName.Anatomy), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1002082, _Label, false, false); // Healing
            AddHtml(180, 164, 100, 18, FormatSkill(Creature, SkillName.Healing), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 1002122, _Label, false, false); // Poisoning
            AddHtml(180, 182, 100, 18, FormatSkill(Creature, SkillName.Poisoning), false, false);

            AddHtmlLocalized(53, 200, 160, 18, 1044074, _Label, false, false); // Detect Hidden
            AddHtml(180, 200, 100, 18, FormatSkill(Creature, SkillName.DetectHidden), false, false);

            AddHtmlLocalized(53, 218, 160, 18, 1002088, _Label, false, false); // Hiding
            AddHtml(180, 218, 100, 18, FormatSkill(Creature, SkillName.Hiding), false, false);

            AddHtmlLocalized(53, 236, 160, 18, 1002118, _Label, false, false); // Parrying
            AddHtml(180, 236, 100, 18, FormatSkill(Creature, SkillName.Parry), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 6);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 4);

            AddPage(6);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001032, 0xC8, false, false); // Lore & Knowledge

            AddHtmlLocalized(53, 92, 160, 18, 1044085, _Label, false, false); // Magery
            AddHtml(180, 92, 100, 18, FormatSkill(Creature, SkillName.Magery), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044076, _Label, false, false); // Eval Int
            AddHtml(180, 110, 100, 18, FormatSkill(Creature, SkillName.EvalInt), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044106, _Label, false, false); // Meditation
            AddHtml(180, 128, 100, 18, FormatSkill(Creature, SkillName.Meditation), false, false);

            AddHtmlLocalized(53, 146, 160, 18, 1044109, _Label, false, false); // Necromancy
            AddHtml(180, 146, 100, 18, FormatSkill(Creature, SkillName.Necromancy), false, false);

            AddHtmlLocalized(53, 164, 160, 18, 1002140, _Label, false, false); // Spirit Speak
            AddHtml(180, 164, 100, 18, FormatSkill(Creature, SkillName.SpiritSpeak), false, false);

            AddHtmlLocalized(53, 182, 160, 18, 1044115, _Label, false, false); // Mysticism
            AddHtml(180, 182, 100, 18, FormatSkill(Creature, SkillName.Mysticism), false, false);

            AddHtmlLocalized(53, 200, 160, 18, 1044110, _Label, false, false); // Focus
            AddHtml(180, 200, 100, 18, FormatSkill(Creature, SkillName.Focus), false, false);

            AddHtmlLocalized(53, 218, 160, 18, 1044114, _Label, false, false); // Spellweaving
            AddHtml(180, 218, 100, 18, FormatSkill(Creature, SkillName.Spellweaving), false, false);

            AddHtmlLocalized(53, 236, 160, 18, 1044075, _Label, false, false); // Discordance
            AddHtml(180, 236, 100, 18, FormatSkill(Creature, SkillName.Discordance), false, false);

            AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 7);
            AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 5);

            AddPage(7);

            AddImage(28, 76, 0x826);

            AddHtmlLocalized(47, 74, 160, 18, 3001032, 0xC8, false, false); // Lore & Knowledge

            AddHtmlLocalized(53, 92, 160, 18, 1044112, _Label, false, false); // Bushido
            AddHtml(180, 92, 100, 18, FormatSkill(Creature, SkillName.Bushido), false, false);

            AddHtmlLocalized(53, 110, 160, 18, 1044113, _Label, false, false); // Ninjitsu
            AddHtml(180, 110, 100, 18, FormatSkill(Creature, SkillName.Ninjitsu), false, false);

            AddHtmlLocalized(53, 128, 160, 18, 1044111, _Label, false, false); // Chivalry
            AddHtml(180, 128, 100, 18, FormatSkill(Creature, SkillName.Chivalry), false, false);

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

            AddHtmlLocalized(47, 182, 160, 18, 1049569, 0xC8, false, false); // Pack Instincts

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

            if (Creature.Tamable)
            {
                AddImage(28, 218, 0x826);

                AddHtmlLocalized(47, 216, 160, 18, 1115783, 0xC8, false, false); // Pet Slots
                AddHtml(53, 236, 80, 18, FormatPetSlots(Creature.ControlSlots, Creature.ControlSlotsMax), false, false);

                AddHtmlLocalized(158, 236, 115, 18, 1157600, Creature.CurrentTameSkill.ToString("0.0"), _Label, false, false);
                AddTooltip(1157586);
            }

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

                    if (loc[0].Number > 0)
                    {
                        AddHtmlLocalized(53, y, 180, 18, loc[0].Number, _Label, false, false);
                    }
                    else if (loc[1].String != null)
                    {
                        AddHtml(53, y, 180, 18, loc[0].String, false, false);
                    }

                    if (loc[1].Number > 0)
                    {
                        AddTooltip(loc[1]);
                    }

                    y += 18;
                }

                y = 92;

                if (profile.Advancements != null)
                {
                    AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 9);
                    AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 7);

                    AddPage(9);

                    AddImage(28, 76, 0x826);

                    AddHtmlLocalized(47, 74, 160, 18, 1157505, 0xC8, false, false); // Pet Advancements

                    for (int i = profile.Advancements.Count - 1; i >= 0; i--)
                    {
                        var loc = PetTrainingHelper.GetLocalization(profile.Advancements[i]);
                        bool skill = profile.Advancements[i] is SkillName; // ? "#228B22" : "#FF4500";

                        if (loc[0].Number > 0)
                        {
                            AddHtmlLocalized(53, y, 180, 18, loc[0], C32216(skill ? 0x008000 : 0xFF4500), false, false);

                            if (skill)
                            {
                                AddHtml(180, y, 75, 18, String.Format("<div align=right>{0:F1}</div>", Creature.Skills[(SkillName)profile.Advancements[i]].Cap), false, false);
                            }
                        }
                        else if (loc[0].String != null)
                        {
                            AddHtml(53, y, 180, 18, Color(skill ? "#008000" : "#FF4500", loc[0]), false, false);
                        }

                        AddTooltip(PetTrainingHelper.GetCategoryLocalization(profile.Advancements[i]));

                        y += 18;
                    }

                    AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);
                    AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 8);
                }
                else
                {
                    AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);
                    AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 7);

                }
            }
            else
            {
                AddButton(240, 328, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);
                AddButton(217, 328, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 7);
            }
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (Creature == null || Creature.Map == null || Creature.Map == Map.Internal || !Creature.Alive || Creature.Deleted || Creature.IsDeadBondedPet)
                id = 0;

            switch (id)
            {
                case 0:
                    User.CloseGump(typeof(PetTrainingOptionsGump));
                    User.CloseGump(typeof(PetTrainingPlanningGump));
                    User.CloseGump(typeof(PetTrainingInfoGump));
                    User.CloseGump(typeof(PetTrainingConfirmGump));
                    User.CloseGump(typeof(PetTrainingConfirmationGump));
                    break;
                case 1: // training tracker
                    User.CloseGump(typeof(PetTrainingProgressGump));

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            BaseGump.SendGump(new PetTrainingProgressGump(User, Creature));
                        });
                    break;
                case 2: // pet training options
                    if (User.HasGump(typeof(PetTrainingConfirmationGump)) ||
                    User.HasGump(typeof(PetTrainingOptionsGump)) ||
                    User.HasGump(typeof(PetTrainingPlanningGump)) ||
                    User.HasGump(typeof(PetTrainingConfirmGump)))
                    {
                        Refresh();
                        break;
                    }

                    var trainProfile = PetTrainingHelper.GetTrainingProfile(Creature, true);

                    if (trainProfile.HasBegunTraining)
                    {
                        if (!trainProfile.HasRecievedControlSlotWarning)
                        {
                            trainProfile.HasRecievedControlSlotWarning = true;

                            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                                {
                                    BaseGump.SendGump(new PetTrainingConfirmGump(User, 1157571, 1157572, () =>
                                    {
                                        Refresh();
                                        User.CloseGump(typeof(PetTrainingOptionsGump));
                                        BaseGump.SendGump(new PetTrainingOptionsGump(User, Creature));
                                    }));
                                });
                        }
                        else
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                                {
                                    Refresh();
                                    User.CloseGump(typeof(PetTrainingOptionsGump));
                                    BaseGump.SendGump(new PetTrainingOptionsGump(User, Creature));
                                });
                        }
                    }
                    break;
                case 3: // cancel
                    BaseGump.SendGump(new PetTrainingConfirmGump(User, 1153093, 1158019, () =>
                        {
                            var trainProfile1 = PetTrainingHelper.GetTrainingProfile(Creature, true);

                            if (trainProfile1 != null)
                            {
                                trainProfile1.EndTraining();
                            }
                        }));
                    break;
                case 4: // begin training
                    var trainProfile2 = PetTrainingHelper.GetTrainingProfile(Creature, true);
                    trainProfile2.BeginTraining();
                    Refresh();

                    Server.Engines.Quests.UsingAnimalLoreQuest.CheckComplete(User);
                    break;
            }
        }

        public int Pages(AbilityProfile profile)
        {
            if (profile == null || profile.Advancements == null || profile.Advancements.Count == 0)
                return 8;

            return 9;
        }

        private static string FormatSkill(BaseCreature c, SkillName name)
        {
            if (c.Skills[name].Base < 10.0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0:F1}/{1}</div>", c.Skills[name].Value, c.Skills[name].Cap);
        }

        private static string FormatAttributes(int cur, int max)
        {
            return AnimalLoreGump.FormatAttributes(cur, max);
        }

        private static string FormatStat(int val)
        {
            return AnimalLoreGump.FormatStat(val);
        }

        public static string FormatDouble(double val, bool dontshowzero = true, bool percentage = false)
        {
            if (dontshowzero)
            {
                return AnimalLoreGump.FormatDouble(val);
            }

            if (percentage)
            {
                return String.Format("<div align=right>{0:F1}%</div>", val);
            }

            return String.Format("<div align=right>{0:F1}</div>", val);
        }

        public static string FormatElement(int val, string color)
        {
            if (color == null)
            {
                if (val <= 0)
                    return String.Format("<div align=right>---</div>");

                return String.Format("<div align=right>{0}%</div>", val);
            }

            if (val <= 0)
                return String.Format("<BASEFONT COLOR={0}><div align=right>---</div>", color);

            return String.Format("<BASEFONT COLOR={1}><div align=right>{0}%</div>", val, color);
        }

        public static string FormatDamage(int min, int max)
        {
            return AnimalLoreGump.FormatDamage(min, max);
        }

        public string FormatPetSlots(int min, int max)
        {
            return String.Format("<BASEFONT COLOR=#57412F>{0} => {1}", min.ToString(), max.ToString());
        }
    }

    public class PetTrainingProgressGump : BaseGump
    {
        public override int GetTypeID()
        {
            return 0xF3EE3;
        }

        public BaseCreature Creature { get; private set; }

        public PetTrainingProgressGump(PlayerMobile pm, BaseCreature bc)
            : base(pm, 50, 200)
        {
            Creature = bc;
        }

        public override void AddGumpLayout()
        {
            List<BaseCreature> pets = new List<BaseCreature>(User.AllFollowers.OfType<BaseCreature>().Where(p => 
                p.TrainingProfile != null &&
                p.TrainingProfile.HasBegunTraining &&
                p.Map == User.Map));

            if (pets == null || pets.Count == 0)
            {
                AddBackground(0, 24, 254, 170, 0x24A4);
                return;
            }

            if (pets.Contains(Creature) && pets[0] != Creature)
            {
                pets.Remove(Creature);
                pets.Insert(0, Creature);
            }

            int length = 254 + ((pets.Count - 1) * 60);

            AddBackground(0, 24, 254, length, 0x24A4);

            AddHtmlLocalized(30, 32, 200, 18, 1114513, "#1157491", 0, false, false); // Pet Training Progress:

            AddButton(120, 0, 0x82D, 0x82D, 0, GumpButtonType.Reply, 0);

            for (int i = 0; i < pets.Count; i++)
            {
                var pet = pets[i];

                AddHtml(53, 60 + (40 * i), 210, 18, Color("#000080", pet.Name), false, false);

                var trainProfile = PetTrainingHelper.GetTrainingProfile(pet);
                double progress = 0.0;

                AddImage(53, 80 + (40 * i), 0x805);

                if (trainProfile != null)
                {
                    progress = trainProfile.TrainingProgressPercentile * 100;

                    if (progress >= 1)
                    {
                        AddBackground(53, 80 + (40 * i), (int)(109.0 * (progress / 100)), 11, 0x806);
                    }
                }

                AddHtml(162, 78 + (40 * i), 50, 18, NewAnimalLoreGump.FormatDouble(progress, false, true), false, false);
            }
        }
    }

    public class PetTrainingConfirmGump : BaseGump
    {
        private int _Title;
        private int _Body;

        private Action ConfirmCallback { get; set; }
        private Action CancelCallback { get; set; }

        public PetTrainingConfirmGump(PlayerMobile pm, int title, int body, Action confirmCallback, Action cancelCallback = null)
            : base(pm, 250, 50)
        {
            pm.CloseGump(GetType());

            _Title = title;
            _Body = body;
            ConfirmCallback = confirmCallback;
            CancelCallback = cancelCallback;
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 454, 240, 0x24A4);

            AddHtmlLocalized(0, 12, 454, 16, CenterLoc, String.Format("#{0}", _Title.ToString()), 0xF424E5, false, false);
            AddHtmlLocalized(55, 65, 344, 80, _Body, C32216(0x8B0000), false, false);

            AddECHandleInput();

            AddButton(70, 150, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
            AddHtml(70, 153, 126, 16, Center("Yes"), false, false);

            AddECHandleInput();
            AddECHandleInput();

            AddButton(235, 150, 0x9CC8, 0x9CC7, 2, GumpButtonType.Reply, 0);
            AddHtml(235, 153, 126, 16, Center("Cancel"), false, false);
        }

        public override void OnResponse(RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (ConfirmCallback != null)
                {
                    ConfirmCallback();
                }

                OnConfirm();
            }
            else if (info.ButtonID == 2)
            {
                if (CancelCallback != null)
                {
                    CancelCallback();
                }

                OnCancel();
            }
        }

        public virtual void OnConfirm()
        {
        }

        public virtual void OnCancel()
        {
        }
    }

    public class PetTrainingOptionsGump : BaseGump
    {
        public BaseCreature Creature { get; private set; }
        public int Category { get; private set; }

        public AbilityProfile AbilityProfile { get; private set; }
        public TrainingProfile TrainingProfile { get; private set; }
        public TrainingDefinition Definition { get; private set; }

        public PetTrainingOptionsGump(PlayerMobile pm, BaseCreature bc)
            : base(pm, 40, 200)
        {
            Creature = bc;
            Category = 0;

            Definition = PetTrainingHelper.GetTrainingDefinition(Creature);
            AbilityProfile = PetTrainingHelper.GetAbilityProfile(Creature, true);
            TrainingProfile = PetTrainingHelper.GetTrainingProfile(Creature, true);
        }

        public override void AddGumpLayout()
        {
            AddBackground(0, 0, 574, 560, 0x24A4);

            AddHtmlLocalized(0, 11, 574, 18, 1157485, false, false); // <CENTER>ANIMAL TRAINING MENU</CENTER>

            AddImageTiled(35, 40, 220, 440, 2624);
            AddImageTiled(37, 42, 216, 436, 3004);

            AddImageTiled(265, 40, 270, 440, 2624);
            AddImageTiled(267, 42, 266, 436, 3004);

            AddHtmlLocalized(35, 63, 220, 20, 1044010, 0, false, false); // <CENTER>CATEGORIES</CENTER>
            AddHtmlLocalized(265, 63, 270, 20, 1044011, 0, false, false); // <CENTER>SELECTIONS</CENTER>

            AddECHandleInput();

            AddButton(35, 490, 0x9CC8, 0x9CC7, 0, GumpButtonType.Reply, 0);
            AddHtml(35, 493, 126, 20, Center("CANCEL"), false, false);

            AddECHandleInput();
            AddECHandleInput();

            AddButton(220, 490, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
            AddHtml(220, 493, 126, 20, Center("PLAN"), false, false);

            AddECHandleInput();
            AddECHandleInput();

            AddButton(410, 490, 0x9CC8, 0x9CC7, 2, GumpButtonType.Reply, 0);
            AddHtml(410, 493, 126, 20, Center("INFO"), false, false);

            int y = 90;
            var def = PetTrainingHelper.GetTrainingDefinition(Creature);

            AddButton(40, y, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddHtmlLocalized(75, y + 2, 150, 16, 3010049, false, false); // Stats
            y += 24;

            AddButton(40, y, 4005, 4007, 4, GumpButtonType.Reply, 0);
            AddHtmlLocalized(75, y + 2, 180, 16, 1114254, false, false); // Resists
            y += 24;

            if (HasAvailable(PetTrainingHelper.MagicSkills))
            {
                AddButton(40, y, 4005, 4007, 5, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157495, false, false); // Increase Magic Skill Caps
                y += 24;
            }

            if (HasAvailable(PetTrainingHelper.CombatSkills))
            {
                AddButton(40, y, 4005, 4007, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157496, false, false); // Increase Combat Skill Caps
                y += 24;
            }

            if (HasAvailable(Definition.MagicalAbilities))
            {
                AddButton(40, y, 4005, 4007, 7, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157481, false, false); // Magical Abilities
                y += 24;
            }

            if (HasAvailable(Definition.SpecialAbilities))
            {
                AddButton(40, y, 4005, 4007, 8, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157480, false, false); // Special Abilities
                y += 24;
            }

            if (HasAvailable(Definition.WeaponAbilities))
            {
                AddButton(40, y, 4005, 4007, 9, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157479, false, false); // Special Moves
                y += 24;
            }

            if (HasAvailable(Definition.AreaEffects))
            {
                AddButton(40, y, 4005, 4007, 10, GumpButtonType.Reply, 0);
                AddHtmlLocalized(75, y + 2, 180, 16, 1157482, false, false); // Area Effect Abilities
            }

            switch (Category)
            {
                case 0: BuildStatsPage(); break;
                case 1: BuildResistsPage(); break;
                case 2: BuildMagicSkillCapsPage(); break;
                case 3: BuildCombatSkillCapsPage(); break;
                case 4: BuildMagicalAbilitiesPage(); break;
                case 5: BuildSpecialAbilitiesPage(); break;
                case 6: BuildSpecialMovesPage(); break;
                case 7: BuildAreaEffectsPage(); break;
            }
        }

        private void BuildStatsPage()
        {
            int y = 90;

            foreach (int i in Enum.GetValues(typeof(PetStat)))
            {
                var tp = PetTrainingHelper.GetTrainingPoint((PetStat)i);

                AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }
        private void BuildResistsPage()
        {
            int y = 90;

            foreach (int i in Enum.GetValues(typeof(ResistanceType)))
            {
                var tp = PetTrainingHelper.GetTrainingPoint((ResistanceType)i);

                AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }

        private void BuildMagicSkillCapsPage()
        {
            int y = 90;

            foreach (var skill in PetTrainingHelper.MagicSkills)
            {
                if ((!PetTrainingHelper.CommonSkill(Creature, skill) && Creature.Skills[skill].Base <= 0) || Creature.Skills[skill].Cap >= 120)
                    continue;

                var tp = PetTrainingHelper.GetTrainingPoint(skill);

                AddButton(275, y, 4005, 4007, 100 + (int)skill, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }

        private void BuildCombatSkillCapsPage()
        {
            int y = 90;

            foreach (var skill in PetTrainingHelper.CombatSkills)
            {
                if ((!PetTrainingHelper.CommonSkill(Creature, skill) && Creature.Skills[skill].Base <= 0) || Creature.Skills[skill].Cap >= 120)
                    continue;

                var tp = PetTrainingHelper.GetTrainingPoint(skill);

                AddButton(275, y, 4005, 4007, 100 + (int)skill, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }

        private void BuildMagicalAbilitiesPage()
        {
            int y = 90;

            for (int i = 0; i < PetTrainingHelper.MagicalAbilities.Length; i++)
            {
                MagicalAbility abil = PetTrainingHelper.MagicalAbilities[i];
                var master = Creature.ControlMaster;

                if ((abil == MagicalAbility.Chivalry && master != null && master.Karma < 0) ||
                    (abil == MagicalAbility.Necromancy && master != null && master.Karma > 0) ||
                    (abil == MagicalAbility.Necromage && master != null && master.Karma > 0))
                {
                    continue;
                }

                if ((Definition.MagicalAbilities & abil) == 0 ||  AbilityProfile.HasAbility(abil) ||
                    !AbilityProfile.CanChooseMagicalAbility(abil) || /*(abil <= MagicalAbility.WrestlingMastery && AbilityProfile.AbilityCount() >= 3) ||*/
                    ((abil & MagicalAbility.Tokuno) != 0 && !AbilityProfile.TokunoTame))
                {
                    continue;
                }

                var tp = PetTrainingHelper.GetTrainingPoint(abil);

                AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }

        private void BuildSpecialAbilitiesPage()
        {
            int y = 90;

            for (int i = 0; i < SpecialAbility.Abilities.Length; i++)
            {
                var abil = SpecialAbility.Abilities[i];

                if (AbilityProfile.HasAbility(abil) || (AbilityProfile.HasSpecialMagicalAbility() && !AbilityProfile.IsRuleBreaker(abil)))
                    continue;

                if (Definition.HasSpecialAbility(abil) || (AbilityProfile.HasAbility(MagicalAbility.Poisoning) && abil is VenomousBite))
                {
                    var tp = PetTrainingHelper.GetTrainingPoint(abil);

                    AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                    Record(tp, 320, y + 2);

                    y += 22;
                }
            }
        }

        private void BuildSpecialMovesPage()
        {
            int y = 90;

            for (int i = 0; i < Definition.WeaponAbilities.Length; i++)
            {
                var abil = Definition.WeaponAbilities[i];

                if (AbilityProfile.HasAbility(abil))
                    continue;

                var tp = PetTrainingHelper.GetTrainingPoint(abil);

                AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                Record(tp, 320, y + 2);

                y += 22;
            }
        }

        private void BuildAreaEffectsPage()
        {
            int y = 90;

            for (int i = 0; i < AreaEffect.Effects.Length; i++)
            {
                var abil = AreaEffect.Effects[i];

                if (AbilityProfile.HasAbility(abil))
                    continue;

                if (Definition.HasAreaEffect(abil) || (AbilityProfile.HasAbility(MagicalAbility.Poisoning) && abil is PoisonBreath))
                {
                    var tp = PetTrainingHelper.GetTrainingPoint(abil);

                    AddButton(275, y, 4005, 4007, 100 + i, GumpButtonType.Reply, 0);
                    Record(tp, 320, y + 2);

                    y += 22;
                }
            }
        }

        private void Record(TrainingPoint tp, int x, int y)
        {
            if (tp == null)
            {
                AddLabel(x, y, 0, "Null Training Point");
                return;
            }

            if (tp.Name.Number > 0)
            {
                AddHtmlLocalized(x, y, 215, 20, tp.Name.Number, 0, false, false);
            }
            else if (tp.Name.String != null)
            {
                AddLabel(x, y, 0, tp.Name.String);
            }
        }

        private bool HasAvailable(object o)
        {
            if (o == null)
                return false;

            if (o is MagicalAbility && Definition.MagicalAbilities != 0)
            {
                foreach (var i in Enum.GetValues(typeof(MagicalAbility)))
                {
                    var ability = (MagicalAbility)i;

                    if ((Definition.MagicalAbilities & ability) != 0 && !AbilityProfile.HasAbility(ability))
                    {
                        return true;
                    }
                }

                return false;
            }
 
            if (o is SpecialAbility[])
            {
                if (!AbilityProfile.CanChooseSpecialAbility((SpecialAbility[])o))
                {
                    return false;
                }

                if (AbilityProfile.HasAbility(MagicalAbility.Poisoning) && !AbilityProfile.HasAbility(SpecialAbility.VenomousBite))
                {
                    return true;
                }

                foreach (var ability in (SpecialAbility[])o)
                {
                    if (!AbilityProfile.HasAbility(ability))
                    {
                        return true;
                    }
                }
            }
            else if (o is WeaponAbility[])
            {
                if (!AbilityProfile.CanChooseWeaponAbility())
                {
                    return false;
                }

                foreach (var ability in (WeaponAbility[])o)
                {
                    if (!AbilityProfile.HasAbility(ability))
                    {
                        return true;
                    }
                }
            }
            else if (o is AreaEffect[])
            {
                if (!AbilityProfile.CanChooseAreaEffect())
                {
                    return false;
                }

                if (AbilityProfile.HasAbility(MagicalAbility.Poisoning) && !AbilityProfile.HasAbility(AreaEffect.PoisonBreath))
                {
                    return true;
                }

                foreach (var ability in (AreaEffect[])o)
                {
                    if (!AbilityProfile.HasAbility(ability))
                    {
                        return true;
                    }
                }
            }
            else if (o is SkillName[])
            {
                foreach (var name in (SkillName[])o)
                {
                    var skill = Creature.Skills[name];

                    if (skill.Base > 0 && skill.Cap < 120)
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 0)
            {
                return;
            }

            if (id == 1)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        if (User.HasGump(typeof(NewAnimalLoreGump)))
                        {
                            Refresh();
                            User.CloseGump(typeof(PetTrainingPlanningGump));
                            BaseGump.SendGump(new PetTrainingPlanningGump(User, Creature));
                        }
                    });
                return;
            }

            if (id == 2)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        if (User.HasGump(typeof(NewAnimalLoreGump)))
                        {
                            Refresh();
                            User.CloseGump(typeof(PetTrainingInfoGump));
                            BaseGump.SendGump(new PetTrainingInfoGump(User));
                        }
                    });
                return;
            }

            if (id <= 11)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        if (User.HasGump(typeof(NewAnimalLoreGump)))
                        {
                            Category = id - 3;
                            Refresh();
                        }
                    });
                return;
            }

            TrainingPoint tp = null;

            switch (Category)
            {
                case 0: // stats
                    PetStat stat = (PetStat)id - 100;
                    tp = PetTrainingHelper.GetTrainingPoint(stat);
                    break;
                case 1: // resists
                    ResistanceType r = (ResistanceType)id - 100;
                    tp = PetTrainingHelper.GetTrainingPoint(r);
                    break;
                case 2: // mag skill
                case 3: // combat skill
                    SkillName sk = (SkillName)id - 100;
                    tp = PetTrainingHelper.GetTrainingPoint(sk);
                    break;
                case 4: // mag abil
                    MagicalAbility mabil = PetTrainingHelper.MagicalAbilities[id - 100];
                    tp = PetTrainingHelper.GetTrainingPoint(mabil);
                    break;
                case 5: // spec abil
                    SpecialAbility sabil = SpecialAbility.Abilities[id - 100];
                    tp = PetTrainingHelper.GetTrainingPoint(sabil);
                    break;
                case 6: // spec moves
                    WeaponAbility wabil = Definition.WeaponAbilities[id - 100];
                    tp = PetTrainingHelper.GetTrainingPoint(wabil);
                    break;
                case 7: // area effects
                    AreaEffect aabil = AreaEffect.Effects[id - 100];
                    tp = PetTrainingHelper.GetTrainingPoint(aabil);
                    break;

            }

            if (tp != null)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        if (User.HasGump(typeof(NewAnimalLoreGump)))
                        {
                            User.CloseGump(typeof(PetTrainingConfirmationGump));
                            BaseGump.SendGump(new PetTrainingConfirmationGump(User, Creature, tp));
                        }
                    });
            }
        }
    }

    public class PetTrainingConfirmationGump : BaseGump
    {
        public BaseCreature Creature { get; private set; }
        public TrainingPoint TrainingPoint { get; private set; }

        public int Value { get; private set; }
        public int StartValue { get; private set; }

        public PetTrainingConfirmationGump(PlayerMobile pm, BaseCreature bc, TrainingPoint tp)
            : base(pm, 50, 200)
        {
            Creature = bc;
            TrainingPoint = tp;

            Value = tp.Start;
        }

        public override void AddGumpLayout()
        {
            var profile = PetTrainingHelper.GetTrainingProfile(Creature, true);

            AddBackground(0, 0, 574, 470, 0x24A4);
            AddHtmlLocalized(0, 12, 574, 16, 1157486, false, false); // <CENTER>TRAINING CONFIRMATION</CENTER>

            AddImageTiled(35, 40, 245, 140, 2624);
            AddImageTiled(37, 42, 241, 136, 3004);

            AddImageTiled(290, 40, 245, 140, 2624);
            AddImageTiled(292, 42, 241, 136, 3004);

            AddImageTiled(35, 190, 245, 140, 2624);
            AddImageTiled(37, 192, 241, 136, 3004);

            AddImageTiled(290, 190, 245, 140, 2624);
            AddImageTiled(292, 192, 241, 136, 3004);

            AddImageTiled(35, 340, 500, 60, 2624);
            AddImageTiled(37, 342, 496, 56, 3004);

            AddButton(40, 410, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
            AddHtml(40, 413, 126, 20, Center("Back"), false, false);

            AddButton(415, 410, 0x9CC8, 0x9CC7, profile.TrainingMode == TrainingMode.Regular ? 8 : 9, GumpButtonType.Reply, 0);
            AddHtml(415, 413, 126, 20, Center(profile.TrainingMode == TrainingMode.Regular ? "Train Pet" : "Add To Plan"), false, false);

            AddHtmlLocalized(35, 55, 245, 20, CenterLoc, "#1114269", 0, false, false); // PROPERTY INFORMATION
            AddHtmlLocalized(50, 85, 60, 16, 1114270, false, false); // Property:
            AddHtmlLocalized(50, 105, 60, 16, 1114272, false, false); // Weight:

            if (TrainingPoint.Name.Number > 0)
                AddHtmlLocalized(120, 85, 200, 16, TrainingPoint.Name.Number, false, false);
            else if (TrainingPoint.Name.String != null)
                AddLabel(120, 85, 0, TrainingPoint.Name.String);

            AddLabel(120, 105, 0, TrainingPoint.Weight.ToString("0.0"));

            if (TrainingPoint.Description.Number > 0)
                AddHtmlLocalized(305, 55, 215, 115, TrainingPoint.Description.Number, true, true);
            else if (TrainingPoint.Description.String != null)
                AddHtml(305, 55, 215, 115, TrainingPoint.Description.String, true, true);

            AddHtmlLocalized(290, 205, 245, 20, CenterLoc, "#1113650", 0, false, false); // RESULTS

            int start = -1;
            int max = TrainingPoint.GetMax(Creature);
            PetTrainingHelper.GetStartValue(TrainingPoint, Creature, ref start);
            StartValue = start;
            
            if (StartValue > Value)
            {
                Value = StartValue;
            }

            if (Value > max)
                Value = max;

            int cost = PetTrainingHelper.GetTotalCost(TrainingPoint, Creature, Value, StartValue);
            double weight = TrainingPoint.Weight;

            if (CanAdjust() && cost > profile.TrainingPoints)
            {
                Value = StartValue + (int)(profile.TrainingPoints / weight);
            }

            if (StartValue > 0)
            {
                double valueWeight = ((double)Value * weight);
                double maxWeight = max * weight;
                double nonAdjustedWeight = (Value - StartValue) * weight;

                double originalValueWeight = valueWeight;

                if (TrainingPoint.TrainPoint is PetStat && (PetStat)TrainingPoint.TrainPoint <= PetStat.Mana)
                {
                    var stat = (PetStat)TrainingPoint.TrainPoint;

                    double maxTotalCap = PetTrainingHelper.GetTrainingCapTotal(stat);
                    double currentTotal = stat <= PetStat.Int ? PetTrainingHelper.GetTotalStatWeight(Creature) : PetTrainingHelper.GetTotalAttributeWeight(Creature);

                    if (currentTotal >= maxTotalCap)
                    {
                        valueWeight = 0;
                    }
                    else if (currentTotal + nonAdjustedWeight > maxTotalCap)
                    {
                        valueWeight -= (currentTotal + nonAdjustedWeight) - maxTotalCap;
                    }
                }
                else if (TrainingPoint.TrainPoint is ResistanceType)
                {
                    var resist = (ResistanceType)TrainingPoint.TrainPoint;
                    double maxTotalCap = PetTrainingHelper.GetTrainingCapTotal(resist);
                    double currentTotal = PetTrainingHelper.GetTotalResistWeight(Creature);

                    if (currentTotal >= maxTotalCap)
                    {
                        valueWeight = 0;
                    }
                    else if (currentTotal + nonAdjustedWeight > maxTotalCap)
                    {
                        valueWeight -= (currentTotal + nonAdjustedWeight) - maxTotalCap;
                    }
                }

                if (valueWeight > maxWeight)
                {
                    valueWeight = maxWeight;
                }

                if (originalValueWeight != valueWeight)
                {
                    Value = Math.Max(StartValue, (int)(valueWeight / weight));
                }
            }

            cost = PetTrainingHelper.GetTotalCost(TrainingPoint, Creature, Value, StartValue);
            int avail = profile.TrainingPoints - cost;

            AddHtmlLocalized(35, 205, 245, 20, CenterLoc, "#1157493", 0, false, false); // REQUIREMENTS

            if (TrainingPoint.Requirements != null && TrainingPoint.Requirements.Length > 0)
            {
                for (int i = 0; i < TrainingPoint.Requirements.Length; i++)
                {
                    var req = TrainingPoint.Requirements[i];

                    if (req == null)
                        continue;

                    if (req.Name.Number > 0)
                        AddHtmlLocalized(45, 225 + (i * 20), 190, 18, req.Name.Number, false, false);
                    else if (req.Name.String != null)
                        AddHtml(45, 225 + (i * 20), 190, 18, req.Name.String, false, false);

                    AddTooltip(1157523);

                    int reqCost = req.Cost;

                    if (req.Requirement is SkillName && Creature.Skills[(SkillName)req.Requirement].Value > 0)
                        reqCost = 0;

                    AddLabel(245, 225 + (i * 20), 0x26, reqCost.ToString());
                    AddTooltip(1157523);
                }

            }
            else if (TrainingPoint.TrainPoint is SkillName)
            {
                int cliloc;

                switch (Value)
                {
                    default:
                    case 50: cliloc = 1049639; break;
                    case 100: cliloc = 1049640; break;
                    case 150: cliloc = 1049641; break;
                    case 200: cliloc = 1049642; break;
                }

                AddHtmlLocalized(45, 225, 225, 60, cliloc, String.Format("#{0}", TrainingPoint.Name.Number), 0, false, false);
            }

            AddHtmlLocalized(305, 225, 145, 18, 1157490, false, false); // Avail. Training Points:
            AddLabel(455, 225, avail <= 0 ? 0x26 : 0, avail.ToString());

            AddHtmlLocalized(305, 245, 145, 18, 1113646, false, false); // Total Property Weight:
            AddLabel(455, 245, 0, String.Format("{0}/{1}", ((int)(Value * weight)).ToString(), (max * weight).ToString()));

            if (TrainingPoint.Name.Number > 0)
                AddHtmlLocalized(305, 265, 145, 18, TrainingPoint.Name.Number, false, false);
            else if (TrainingPoint.Name.String != null)
                AddLabel(305, 265, 0, TrainingPoint.Name.String);

            if (TrainingPoint.TrainPoint is SkillName)
            {
                AddLabel(455, 265, 0, (100.0 + ((double)Value / 10)).ToString("0.0"));
            }
            else
            {
                AddLabel(455, 265, 0, StartValue > Value ? StartValue.ToString() : Value.ToString());
            }

            AddHtmlLocalized(230, 352, 150, 18, 1113586, false, false); // Property Weight:
            AddHtml(267, 373, 40, 18, Center(((int)(Value * weight)).ToString()), false, false);

            if (CanAdjust())
            {
                AddButton(205, 375, 0x1464, 0x1464, 2, GumpButtonType.Reply, 0);
                AddButton(213, 375, 0x1466, 0x1466, 2, GumpButtonType.Reply, 0);

                AddButton(225, 375, 0x1464, 0x1464, 3, GumpButtonType.Reply, 0);
                AddButton(233, 375, 0x1466, 0x1466, 3, GumpButtonType.Reply, 0);

                AddButton(245, 375, 0x1464, 0x1464, 4, GumpButtonType.Reply, 0);
                AddButton(253, 375, 0x1466, 0x1466, 4, GumpButtonType.Reply, 0);

                AddButton(305, 375, 0x1464, 0x1464, 5, GumpButtonType.Reply, 0);
                AddButton(313, 375, 0x1466, 0x1466, 5, GumpButtonType.Reply, 0);

                AddButton(325, 375, 0x1464, 0x1464, 6, GumpButtonType.Reply, 0);
                AddButton(333, 375, 0x1466, 0x1466, 6, GumpButtonType.Reply, 0);

                AddButton(345, 375, 0x1464, 0x1464, 7, GumpButtonType.Reply, 0);
                AddButton(353, 375, 0x1466, 0x1466, 7, GumpButtonType.Reply, 0);
            }
            else
            {
                AddImage(205, 375, 0x1464);
                AddImage(213, 375, 0x1466);

                AddImage(225, 375, 0x1464);
                AddImage(233, 375, 0x1466);

                AddImage(245, 375, 0x1464);
                AddImage(253, 375, 0x1466);

                AddImage(305, 375, 0x1464);
                AddImage(313, 375, 0x1466);

                AddImage(325, 375, 0x1464);
                AddImage(333, 375, 0x1466);

                AddImage(345, 375, 0x1464);
                AddImage(353, 375, 0x1466);
            }

            AddLabel(207, 373, 0, "<");
            AddLabel(211, 373, 0, "<");
            AddLabel(215, 373, 0, "<");

            AddLabel(228, 373, 0, "<");
            AddLabel(232, 373, 0, "<");

            AddLabel(251, 373, 0, "<");

            AddLabel(312, 373, 0, ">");

            AddLabel(329, 373, 0, ">");
            AddLabel(333, 373, 0, ">");

            AddLabel(348, 373, 0, ">");
            AddLabel(352, 373, 0, ">");
            AddLabel(356, 373, 0, ">");
        }

        private bool CanAdjust()
        {
            return TrainingPoint.GetMax(Creature) != TrainingPoint.Start;
        }

        public override void OnResponse(RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case 0:
                    break;
                case 1:
                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            if (User.HasGump(typeof(NewAnimalLoreGump)))
                            {
                                BaseGump.SendGump(new PetTrainingOptionsGump(User, Creature));
                            }
                        });
                    break;
                case 2: // <<<
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        Value = TrainingPoint.Start;
                    }

                    Refresh();
                    break;
                case 3: // <<
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        if (TrainingPoint.TrainPoint is SkillName)
                        {
                            Value = Math.Max(TrainingPoint.Start, Value -= 50);
                        }
                        else
                        {
                            Value = Value - (TrainingPoint.GetMax(Creature) / 10);
                        }
                    }
                    Refresh();
                    break;
                case 4: // <
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        if (TrainingPoint.TrainPoint is SkillName)
                        {
                            Value = Math.Max(TrainingPoint.Start, Value -= 50);
                        }
                        else
                        {
                            if (TrainingPoint.Weight < 1.0)
                            {
                                Value -= (int)(TrainingPoint.Weight * 100);
                            }
                            else
                            {
                                Value--;
                            }
                        }
                    }
                    Refresh();
                    break;
                case 5: // >
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        if (TrainingPoint.TrainPoint is SkillName)
                        {
                            Value = Math.Min(TrainingPoint.GetMax(Creature), Value += 50);
                        }
                        else
                        {
                            if (TrainingPoint.Weight < 1.0)
                            {
                                Value += (int)(TrainingPoint.Weight * 100);
                            }
                            else
                            {
                                Value++;
                            }
                        }
                    }
                    Refresh();
                    break;
                case 6: // >>
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        if (TrainingPoint.TrainPoint is SkillName)
                        {
                            Value = Math.Min(TrainingPoint.GetMax(Creature), Value += 50);
                        }
                        else
                        {
                            //int cost = PetTrainingHelper.GetTotalCost(TrainingPoint, Creature, Value, StartValue);
                            Value += (TrainingPoint.GetMax(Creature) / 10);
                        }
                    }
                    Refresh();
                    break;
                case 7: // >>>
                    if (TrainingPoint.Start != TrainingPoint.GetMax(Creature))
                    {
                        Value = TrainingPoint.GetMax(Creature);
                    }

                    Refresh();
                    break;
                case 8: // train pet
                    var profile = PetTrainingHelper.GetTrainingProfile(Creature, true);
                    int cost = PetTrainingHelper.GetTotalCost(TrainingPoint, Creature, Value, StartValue);

                    Item scroll = null;

                    if (!Creature.Controlled || User != Creature.ControlMaster)
                    {
                        User.SendLocalizedMessage(1114368); // This is not your pet!
                    }
                    else if (!Creature.InRange(User.Location, 12))
                    {
                        User.SendLocalizedMessage(1153204); // The pet is too far away from you!
                    }
                    else if (Server.Spells.SpellHelper.CheckCombat(User) || Server.Spells.SpellHelper.CheckCombat(Creature) ||
                        Creature.Aggressed.Count > 0 || Creature.Combatant != null)
                    {
                        User.SendLocalizedMessage(1156876); // Since you have been in combat recently you may not use this feature.
                    }
                    else if (!profile.HasIncreasedControlSlot && User.Followers >= User.FollowersMax)
                    {
                        User.SendLocalizedMessage(1157498); // You do not have the available pet slots to train this pet. Please free up pet slots and try again.
                    }
                    else if (cost > profile.TrainingPoints)
                    {
                        User.SendLocalizedMessage(1157500); // Your pet lacks the required points to complete this training.
                    }
                    else if (TrainingPoint.TrainPoint is SkillName && !CheckPowerScroll((SkillName)TrainingPoint.TrainPoint, Value, ref scroll))
                    {
                        User.SendLocalizedMessage(1157499); // You are unable to train your pet. The required powerscroll was not found in your main backpack. 
                    }
                    else if (StartValue >= Value)
                    {
                        User.SendLocalizedMessage(1157501); // Your pet looks to have already completed that training. 
                    }
                    else
                    {
                        BaseGump.SendGump(new PetTrainingConfirmGump(User, 1157502, TrainingPoint.TrainPoint is MagicalAbility ? 1157566 : 1157503, () =>
                            {
                                if (PetTrainingHelper.ApplyTrainingPoint(Creature, TrainingPoint, Value))
                                {
                                    User.SendLocalizedMessage(1157497); // You successfully train the pet!

                                    if (TrainingPoint.TrainPoint is SkillName)
                                    {
                                        Effects.SendLocationParticles(EffectItem.Create(Creature.Location, Creature.Map, EffectItem.DefaultDuration), 0, 0, 0, 0, 0, 5060, 0);
                                        Effects.PlaySound(Creature.Location, Creature.Map, 0x243);

                                        Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(Creature.X - 6, Creature.Y - 6, Creature.Z + 15), Creature.Map), Creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                                        Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(Creature.X - 4, Creature.Y - 6, Creature.Z + 15), Creature.Map), Creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);
                                        Effects.SendMovingParticles(new Entity(Server.Serial.Zero, new Point3D(Creature.X - 6, Creature.Y - 4, Creature.Z + 15), Creature.Map), Creature, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                                        Effects.SendTargetParticles(Creature, 0x375A, 35, 90, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                                        if (scroll != null)
                                            scroll.Delete();
                                    }
                                    else
                                    {
                                        Creature.PlaySound(0x1EB);
                                        Creature.FixedEffect(0x375A, 1, 11);
                                    }

                                    profile.OnTrain(User, cost);

                                    ResendGumps(profile.HasBegunTraining);

                                    Server.Engines.Quests.TeachingSomethingNewQuest.CheckComplete(User);
                                }
                            },
                            () =>
                            {
                                ResendGumps(profile.HasBegunTraining);
                            }));
                    }
                    break;
                case 9:
                    int v = Value;

                    if (Value > StartValue)
                    {
                        if (StartValue > 0)
                            v -= StartValue;

                        PetTrainingHelper.GetPlanningProfile(Creature, true).AddToPlan(TrainingPoint.TrainPoint, Value, PetTrainingHelper.GetTotalCost(TrainingPoint, Creature, Value, StartValue));

                        User.SendLocalizedMessage(1157592); // Your selection has been added to your training plan.
                    }

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        if (User.HasGump(typeof(NewAnimalLoreGump)))
                        {
                            Refresh();

                            BaseGump gump = User.FindGump<PetTrainingPlanningGump>();

                            if (gump != null)
                            {
                                gump.Refresh();
                            }
                            else
                            {
                                BaseGump.SendGump(new PetTrainingPlanningGump(User, Creature));
                            }
                        }
                    });
                    break;
            }
        }

        private void ResendGumps(bool sendOptions)
        {
            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
            {
                BaseGump gump = User.FindGump<NewAnimalLoreGump>();

                if (gump != null)
                {
                    gump.Refresh();
                }
                else
                {
                    BaseGump.SendGump(new NewAnimalLoreGump(User, Creature));
                }

                if (sendOptions)
                {
                    gump = User.FindGump<PetTrainingOptionsGump>();

                    if (gump != null)
                    {
                        gump.Refresh();
                    }
                    else
                    {
                        BaseGump.SendGump(new PetTrainingOptionsGump(User, Creature));
                    }
                }
            });
        }

        private bool CheckPowerScroll(SkillName name, int value, ref Item scroll)
        {
            if (User.Backpack == null)
                return false;

            scroll = User.Backpack.Items.OfType<PowerScroll>().FirstOrDefault(ps => ps.Skill == name && ps.Value == 100 + (value / 10));
            
            return scroll != null;
        }
    }

    public class PetTrainingPlanningGump : BaseGump
    {
        public BaseCreature Creature { get; private set; }

        public PetTrainingPlanningGump(PlayerMobile pm, BaseCreature bc)
            : base(pm, 50, 200)
        {
            Creature = bc;
        }

        public override void AddGumpLayout()
        {
            var profile = PetTrainingHelper.GetTrainingProfile(Creature, true);
            var plan = profile.PlanningProfile;

            AddBackground(0, 0, 654, 524, 0x24A4);

            AddButton(275, 35, 0x9CC8, 0x9CC7, 1, GumpButtonType.Reply, 0);
            AddHtml(275, 38, 126, 20, Center(profile.TrainingMode == TrainingMode.Planning ? "DISABLE" : "ENABLE"), false, false);

            AddHtmlLocalized(0, 11, 654, 18, CenterLoc, "#1157591", 0xF424E5, false, false); // Pet Training Planning 
            AddHtmlLocalized(60, 60, 216, 18, 1044010, 0, false, false); // <CENTER>CATEGORIES</CENTER>
            AddHtmlLocalized(275, 60, 216, 18, 1044011, 0, false, false); // <CENTER>SELECTIONS</CENTER>
            AddHtmlLocalized(510, 60, 150, 18, 1113586, 0, false, false); // Property Weight:

            AddHtmlLocalized(260, 80, 150, 16, 1157490, C32216(0x8B0000), false, false); // Avail. Training Points:
            AddLabel(510, 80, 0, profile.TrainingPoints.ToString());

            int y = 95;
            int total = 0;

            for (int i = 0; i < plan.Entries.Count; i++)
            {
                var entry = plan.Entries[i];

                AddButton(25, y, 4017, 4019, i + 100, GumpButtonType.Reply, 0);
                AddHtmlLocalized(60, y, 200, 18, PetTrainingHelper.GetCategoryLocalization(entry.TrainPoint), false, false);
                
                var loc = PetTrainingHelper.GetLocalization(entry.TrainPoint);

                if (loc[0].Number > 0)
                    AddHtmlLocalized(260, y, 200, 18, loc[0], false, false);
                else if (loc[0].String != null)
                    AddHtml(260, y, 200, 18, loc[0].String, false, false);

                var value = entry.TrainPoint is SkillName ? entry.Value + 1000 : entry.Value;

                AddLabel(460, y, entry.Value == 0 ? 0x27 : 0, value.ToString());
                AddLabel(510, y, entry.Cost == 0 ? 0x27 : 0, String.Format("-{0}", entry.Cost));

                total += entry.Cost;
                y += 22;
            }

            int remaining = profile.TrainingPoints - total;

            AddHtmlLocalized(260, y, 200, 18, 1157599, C32216(0x8B0000), false, false); // Remaining Training Points:
            AddLabel(510, y + 5, remaining < profile.TrainingPoints ? 0x27 : 0, remaining.ToString());
        }

        public override void OnResponse(RelayInfo info)
        {
            int id = info.ButtonID;

            if (id == 0)
                return;

            var profile = PetTrainingHelper.GetTrainingProfile(Creature, true);
            var plan = profile.PlanningProfile;

            if (id == 1)
            {
                if (profile.TrainingMode == TrainingMode.Planning)
                {
                    profile.TrainingMode = TrainingMode.Regular;
                    User.SendLocalizedMessage(1157598); // You have disabled pet training planning.
                }
                else
                {
                    profile.TrainingMode = TrainingMode.Planning;
                    User.SendLocalizedMessage(1157597); // You have enabled pet training planning. All pet training selections will be added to the planning gump which will display up to 20 training options. For more details view the information gump.
                }

                BaseGump gump = User.FindGump<PetTrainingConfirmationGump>();

                if (gump != null)
                {
                    gump.Refresh();
                }

                Refresh();
            }
            else
            {
                id = id - 100;

                if (id >= 0 && id < plan.Entries.Count)
                {
                    plan.Entries.RemoveAt(id);
                    User.SendLocalizedMessage(1157593); // Your selection has been removed to your training plan.
                }

                Refresh();
            }
        }
    }

    public class PetTrainingInfoGump : BaseGump
    {
        public PetTrainingInfoGump(PlayerMobile pm)
            : base(pm, 50, 200)
        {
        }

        public override void AddGumpLayout()
        {
            AddPage(0);
            AddBackground(0, 0, 654, 500, 0x24A4);
            AddHtmlLocalized(0, 45, 654, 16, CenterLoc, "#1157527", 0xF424E5, false, false); // Discovering Animal Training

            AddPage(1);

            AddHtmlLocalized(30, 75, 590, 380, 1157558, true, true);

            AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 4);
            AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 2);

            AddPage(2);

            AddHtmlLocalized(30, 75, 590, 380, 1157563, true, true);

            AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 1);
            AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 3);

            AddPage(3);

            AddHtmlLocalized(30, 75, 590, 380, 1157552, true, true);

            AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 2);
            AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 4);

            AddPage(4);

            AddHtmlLocalized(30, 75, 590, 380, 1157553, true, true);

            AddButton(560, 45, 0x15E3, 0x15E7, 0, GumpButtonType.Page, 3);
            AddButton(585, 45, 0x15E1, 0x15E5, 0, GumpButtonType.Page, 1);
        }
    }
}
