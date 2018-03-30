using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Targeting;

namespace Server.SkillHandlers
{
    public class AnimalLore
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.AnimalLore].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            if (PetTrainingHelper.Enabled && m.HasGump(typeof(NewAnimalLoreGump)))
            {
                m.SendLocalizedMessage(500118); // You must wait a few moments to use another skill.
            }
            else
            {
                m.Target = new InternalTarget();
                m.SendLocalizedMessage(500328); // What animal should I look at?
            }

            return TimeSpan.FromSeconds(1.0);
        }

        private class InternalTarget : Target
        {
			private static void SendGump(Mobile from, BaseCreature c)
			{
                from.CheckTargetSkill(SkillName.AnimalLore, c, 0.0, 120.0);

                if (PetTrainingHelper.Enabled && from is PlayerMobile)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(1), () =>
                        {
                            BaseGump.SendGump(new NewAnimalLoreGump((PlayerMobile)from, c));
                        });
                }
                else
                {
                    from.CloseGump(typeof(AnimalLoreGump));
                    from.SendGump(new AnimalLoreGump(c));
                }
			}

			private static void Check(Mobile from, BaseCreature c, double min)
			{
				if (from.CheckTargetSkill(SkillName.AnimalLore, c, min, 120.0))
					SendGump(from, c);
				else
					from.SendLocalizedMessage(500334); // You can't think of anything you know offhand.
			}

            public InternalTarget()
                : base(8, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!from.Alive)
                {
                    from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.
                }
                else if (targeted is BaseCreature)
                {
                    BaseCreature c = (BaseCreature)targeted;

                    if (!c.IsDeadPet)
                    {
                        if (c.Body.IsAnimal || c.Body.IsMonster || c.Body.IsSea)
                        {
							double skill = from.Skills[SkillName.AnimalLore].Value;
							if(skill < 100.0)
                            {
								if (c.Controlled)
									SendGump(from, c);
								else
									from.SendLocalizedMessage(1049674); // At your skill level, you can only lore tamed creatures.
                            }
                            else if (skill < 110.0)
                            {
								if (c.Controlled)
									SendGump(from, c);
								else if (c.Tamable)
									Check(from, c, 80.0);
								else
									from.SendLocalizedMessage(1049675); // At your skill level, you can only lore tamed or tameable creatures.
                            }
                            else
                            {
								if (c.Controlled)
									SendGump(from, c);
								else if (c.Tamable)
									Check(from, c, 80.0);
								else
									Check(from, c, 100.0);
                            }
                        }
                        else
                        {
                            from.SendLocalizedMessage(500329); // That's not an animal!
                        }
                    }
                    else
                    {
                        from.SendLocalizedMessage(500331); // The spirits of the dead are not the province of animal lore.
                    }
                }
                else
                {
                    from.SendLocalizedMessage(500329); // That's not an animal!
                }
            }
        }
    }

    public class AnimalLoreGump : Gump
    {
        public static string FormatSkill(BaseCreature c, SkillName name)
        {
            Skill skill = c.Skills[name];

            if (skill.Base < 10.0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0:F1}</div>", skill.Value);
        }

        public static string FormatAttributes(int cur, int max)
        {
            if (max == 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0}/{1}</div>", cur, max);
        }

        public static string FormatStat(int val)
        {
            if (val == 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0}</div>", val);
        }

        public static string FormatDouble(double val)
        {
            if (val == 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0:F1}</div>", val);
        }

        public static string FormatElement(int val)
        {
            if (val <= 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0}%</div>", val);
        }

        #region Mondain's Legacy
        public static string FormatDamage(int min, int max)
        {
            if (min <= 0 || max <= 0)
                return "<div align=right>---</div>";

            return String.Format("<div align=right>{0}-{1}</div>", min, max);
        }

        #endregion

        private const int LabelColor = 0x24E5;

        public AnimalLoreGump(BaseCreature c)
            : base(250, 50)
        {
            AddPage(0);

            AddImage(100, 100, 2080);
            AddImage(118, 137, 2081);
            AddImage(118, 207, 2081);
            AddImage(118, 277, 2081);
            AddImage(118, 347, 2083);

            AddHtml(147, 108, 210, 18, String.Format("<center><i>{0}</i></center>", c.Name), false, false);

            AddButton(240, 77, 2093, 2093, 2, GumpButtonType.Reply, 0);

            AddImage(140, 138, 2091);
            AddImage(140, 335, 2091);

            int pages = (Core.AOS ? 5 : 3);
            int page = 0;

            #region Attributes
            AddPage(++page);

            AddImage(128, 152, 2086);
            AddHtmlLocalized(147, 150, 160, 18, 1049593, 200, false, false); // Attributes

            AddHtmlLocalized(153, 168, 160, 18, 1049578, LabelColor, false, false); // Hits
            AddHtml(280, 168, 75, 18, FormatAttributes(c.Hits, c.HitsMax), false, false);

            AddHtmlLocalized(153, 186, 160, 18, 1049579, LabelColor, false, false); // Stamina
            AddHtml(280, 186, 75, 18, FormatAttributes(c.Stam, c.StamMax), false, false);

            AddHtmlLocalized(153, 204, 160, 18, 1049580, LabelColor, false, false); // Mana
            AddHtml(280, 204, 75, 18, FormatAttributes(c.Mana, c.ManaMax), false, false);

            AddHtmlLocalized(153, 222, 160, 18, 1028335, LabelColor, false, false); // Strength
            AddHtml(320, 222, 35, 18, FormatStat(c.Str), false, false);

            AddHtmlLocalized(153, 240, 160, 18, 3000113, LabelColor, false, false); // Dexterity
            AddHtml(320, 240, 35, 18, FormatStat(c.Dex), false, false);

            AddHtmlLocalized(153, 258, 160, 18, 3000112, LabelColor, false, false); // Intelligence
            AddHtml(320, 258, 35, 18, FormatStat(c.Int), false, false);

            if (Core.AOS)
            {
                int y = 276;

                if (Core.SE)
                {
                    double bd = Items.BaseInstrument.GetBaseDifficulty(c);
                    if (c.Uncalmable)
                        bd = 0;

                    AddHtmlLocalized(153, 276, 160, 18, 1070793, LabelColor, false, false); // Barding Difficulty
                    AddHtml(320, y, 35, 18, FormatDouble(bd), false, false);

                    y += 18;
                }

                AddImage(128, y + 2, 2086);
                AddHtmlLocalized(147, y, 160, 18, 1049594, 200, false, false); // Loyalty Rating
                y += 18;

                AddHtmlLocalized(153, y, 160, 18, (!c.Controlled || c.Loyalty == 0) ? 1061643 : 1049595 + (c.Loyalty / 10), LabelColor, false, false);
            }
            else
            {
                AddImage(128, 278, 2086);
                AddHtmlLocalized(147, 276, 160, 18, 3001016, 200, false, false); // Miscellaneous

                AddHtmlLocalized(153, 294, 160, 18, 1049581, LabelColor, false, false); // Armor Rating
                AddHtml(320, 294, 35, 18, FormatStat(c.VirtualArmor), false, false);
            }

            AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
            AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, pages);
            #endregion

            #region Resistances
            if (Core.AOS)
            {
                AddPage(++page);

                AddImage(128, 152, 2086);
                AddHtmlLocalized(147, 150, 160, 18, 1061645, 200, false, false); // Resistances

                AddHtmlLocalized(153, 168, 160, 18, 1061646, LabelColor, false, false); // Physical
                AddHtml(320, 168, 35, 18, FormatElement(c.PhysicalResistance), false, false);

                AddHtmlLocalized(153, 186, 160, 18, 1061647, LabelColor, false, false); // Fire
                AddHtml(320, 186, 35, 18, FormatElement(c.FireResistance), false, false);

                AddHtmlLocalized(153, 204, 160, 18, 1061648, LabelColor, false, false); // Cold
                AddHtml(320, 204, 35, 18, FormatElement(c.ColdResistance), false, false);

                AddHtmlLocalized(153, 222, 160, 18, 1061649, LabelColor, false, false); // Poison
                AddHtml(320, 222, 35, 18, FormatElement(c.PoisonResistance), false, false);

                AddHtmlLocalized(153, 240, 160, 18, 1061650, LabelColor, false, false); // Energy
                AddHtml(320, 240, 35, 18, FormatElement(c.EnergyResistance), false, false);

                AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
                AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
            }
            #endregion

            #region Damage
            if (Core.AOS)
            {
                AddPage(++page);

                AddImage(128, 152, 2086);
                AddHtmlLocalized(147, 150, 160, 18, 1017319, 200, false, false); // Damage

                AddHtmlLocalized(153, 168, 160, 18, 1061646, LabelColor, false, false); // Physical
                AddHtml(320, 168, 35, 18, FormatElement(c.PhysicalDamage), false, false);

                AddHtmlLocalized(153, 186, 160, 18, 1061647, LabelColor, false, false); // Fire
                AddHtml(320, 186, 35, 18, FormatElement(c.FireDamage), false, false);

                AddHtmlLocalized(153, 204, 160, 18, 1061648, LabelColor, false, false); // Cold
                AddHtml(320, 204, 35, 18, FormatElement(c.ColdDamage), false, false);

                AddHtmlLocalized(153, 222, 160, 18, 1061649, LabelColor, false, false); // Poison
                AddHtml(320, 222, 35, 18, FormatElement(c.PoisonDamage), false, false);

                AddHtmlLocalized(153, 240, 160, 18, 1061650, LabelColor, false, false); // Energy
                AddHtml(320, 240, 35, 18, FormatElement(c.EnergyDamage), false, false);

                #region Mondain's Legacy
                if (Core.ML)
                {
                    AddHtmlLocalized(153, 258, 160, 18, 1076750, LabelColor, false, false); // Base Damage
                    AddHtml(300, 258, 55, 18, FormatDamage(c.DamageMin, c.DamageMax), false, false);
                }
                #endregion

                AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
                AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
            }
            #endregion

            #region Skills
            AddPage(++page);

            AddImage(128, 152, 2086);
            AddHtmlLocalized(147, 150, 160, 18, 3001030, 200, false, false); // Combat Ratings

            AddHtmlLocalized(153, 168, 160, 18, 1044103, LabelColor, false, false); // Wrestling
            AddHtml(320, 168, 35, 18, FormatSkill(c, SkillName.Wrestling), false, false);

            AddHtmlLocalized(153, 186, 160, 18, 1044087, LabelColor, false, false); // Tactics
            AddHtml(320, 186, 35, 18, FormatSkill(c, SkillName.Tactics), false, false);

            AddHtmlLocalized(153, 204, 160, 18, 1044086, LabelColor, false, false); // Magic Resistance
            AddHtml(320, 204, 35, 18, FormatSkill(c, SkillName.MagicResist), false, false);

            AddHtmlLocalized(153, 222, 160, 18, 1044061, LabelColor, false, false); // Anatomy
            AddHtml(320, 222, 35, 18, FormatSkill(c, SkillName.Anatomy), false, false);

            #region Mondain's Legacy
            if (c is CuSidhe)
            {
                AddHtmlLocalized(153, 240, 160, 18, 1044077, LabelColor, false, false); // Healing
                AddHtml(320, 240, 35, 18, FormatSkill(c, SkillName.Healing), false, false);
            }
            else
            {
                AddHtmlLocalized(153, 240, 160, 18, 1044090, LabelColor, false, false); // Poisoning
                AddHtml(320, 240, 35, 18, FormatSkill(c, SkillName.Poisoning), false, false);
            }
            #endregion

            AddImage(128, 260, 2086);
            AddHtmlLocalized(147, 258, 160, 18, 3001032, 200, false, false); // Lore & Knowledge

            AddHtmlLocalized(153, 276, 160, 18, 1044085, LabelColor, false, false); // Magery
            AddHtml(320, 276, 35, 18, FormatSkill(c, SkillName.Magery), false, false);

            AddHtmlLocalized(153, 294, 160, 18, 1044076, LabelColor, false, false); // Evaluating Intelligence
            AddHtml(320, 294, 35, 18, FormatSkill(c, SkillName.EvalInt), false, false);

            AddHtmlLocalized(153, 312, 160, 18, 1044106, LabelColor, false, false); // Meditation
            AddHtml(320, 312, 35, 18, FormatSkill(c, SkillName.Meditation), false, false);

            AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, page + 1);
            AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
            #endregion

            #region Misc
            AddPage(++page);

            AddImage(128, 152, 2086);
            AddHtmlLocalized(147, 150, 160, 18, 1049563, 200, false, false); // Preferred Foods

            int foodPref = 3000340;

            if ((c.FavoriteFood & FoodType.FruitsAndVegies) != 0)
                foodPref = 1049565; // Fruits and Vegetables
            else if ((c.FavoriteFood & FoodType.GrainsAndHay) != 0)
                foodPref = 1049566; // Grains and Hay
            else if ((c.FavoriteFood & FoodType.Fish) != 0)
                foodPref = 1049568; // Fish
            else if ((c.FavoriteFood & FoodType.Meat) != 0)
                foodPref = 1049564; // Meat
            else if ((c.FavoriteFood & FoodType.Eggs) != 0)
                foodPref = 1044477; // Eggs

            AddHtmlLocalized(153, 168, 160, 18, foodPref, LabelColor, false, false);

            AddImage(128, 188, 2086);
            AddHtmlLocalized(147, 186, 160, 18, 1049569, 200, false, false); // Pack Instincts

            int packInstinct = 3000340;

            if ((c.PackInstinct & PackInstinct.Canine) != 0)
                packInstinct = 1049570; // Canine
            else if ((c.PackInstinct & PackInstinct.Ostard) != 0)
                packInstinct = 1049571; // Ostard
            else if ((c.PackInstinct & PackInstinct.Feline) != 0)
                packInstinct = 1049572; // Feline
            else if ((c.PackInstinct & PackInstinct.Arachnid) != 0)
                packInstinct = 1049573; // Arachnid
            else if ((c.PackInstinct & PackInstinct.Daemon) != 0)
                packInstinct = 1049574; // Daemon
            else if ((c.PackInstinct & PackInstinct.Bear) != 0)
                packInstinct = 1049575; // Bear
            else if ((c.PackInstinct & PackInstinct.Equine) != 0)
                packInstinct = 1049576; // Equine
            else if ((c.PackInstinct & PackInstinct.Bull) != 0)
                packInstinct = 1049577; // Bull

            AddHtmlLocalized(153, 204, 160, 18, packInstinct, LabelColor, false, false);

            if (!Core.AOS)
            {
                AddImage(128, 224, 2086);
                AddHtmlLocalized(147, 222, 160, 18, 1049594, 200, false, false); // Loyalty Rating

                AddHtmlLocalized(153, 240, 160, 18, (!c.Controlled || c.Loyalty == 0) ? 1061643 : 1049595 + (c.Loyalty / 10), LabelColor, false, false);
            }

            AddButton(340, 358, 5601, 5605, 0, GumpButtonType.Page, 1);
            AddButton(317, 358, 5603, 5607, 0, GumpButtonType.Page, page - 1);
            #endregion
        }
    }
}