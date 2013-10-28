using System;
using System.Collections;

namespace Server.Engines.ConPVP
{
    public class RulesetLayout
    {
        private static RulesetLayout m_Root;

        public static RulesetLayout Root
        {
            get
            {
                if (m_Root == null)
                {
                    ArrayList entries = new ArrayList();

                    entries.Add(new RulesetLayout("Spells", new RulesetLayout[]
                    {
                        new RulesetLayout("1st Circle", "Spells", new string[]
                        {
                            "Reactive Armor", "Clumsy",
                            "Create Food", "Feeblemind",
                            "Heal", "Magic Arrow",
                            "Night Sight", "Weaken"
                        }),
                        new RulesetLayout("2nd Circle", "Spells", new string[]
                        {
                            "Agility", "Cunning",
                            "Cure", "Harm",
                            "Magic Trap", "Untrap",
                            "Protection", "Strength"
                        }),
                        new RulesetLayout("3rd Circle", "Spells", new string[]
                        {
                            "Bless", "Fireball",
                            "Magic Lock", "Poison",
                            "Telekinesis", "Teleport",
                            "Unlock Spell", "Wall of Stone"
                        }),
                        new RulesetLayout("4th Circle", "Spells", new string[]
                        {
                            "Arch Cure", "Arch Protection",
                            "Curse", "Fire Field",
                            "Greater Heal", "Lightning",
                            "Mana Drain", "Recall"
                        }),
                        new RulesetLayout("5th Circle", "Spells", new string[]
                        {
                            "Blade Spirits", "Dispel Field",
                            "Incognito", "Magic Reflection",
                            "Mind Blast", "Paralyze",
                            "Poison Field", "Summon Creature"
                        }),
                        new RulesetLayout("6th Circle", "Spells", new string[]
                        {
                            "Dispel", "Energy Bolt",
                            "Explosion", "Invisibility",
                            "Mark", "Mass Curse",
                            "Paralyze Field", "Reveal"
                        }),
                        new RulesetLayout("7th Circle", "Spells", new string[]
                        {
                            "Chain Lightning", "Energy Field",
                            "Flame Strike", "Gate Travel",
                            "Mana Vampire", "Mass Dispel",
                            "Meteor Swarm", "Polymorph"
                        }),
                        new RulesetLayout("8th Circle", "Spells", new string[]
                        {
                            "Earthquake", "Energy Vortex",
                            "Resurrection", "Air Elemental",
                            "Summon Daemon", "Earth Elemental",
                            "Fire Elemental", "Water Elemental"
                        })
                    }));

                    if (Core.AOS)
                    {
                        entries.Add(new RulesetLayout("Chivalry", new string[]
                        {
                            "Cleanse by Fire",
                            "Close Wounds",
                            "Consecrate Weapon",
                            "Dispel Evil",
                            "Divine Fury",
                            "Enemy of One",
                            "Holy Light",
                            "Noble Sacrifice",
                            "Remove Curse",
                            "Sacred Journey"
                        }));

                        entries.Add(new RulesetLayout("Necromancy", new string[]
                        {
                            "Animate Dead",
                            "Blood Oath",
                            "Corpse Skin",
                            "Curse Weapon",
                            "Evil Omen",
                            "Horrific Beast",
                            "Lich Form",
                            "Mind Rot",
                            "Pain Spike",
                            "Poison Strike",
                            "Strangle",
                            "Summon Familiar",
                            "Vampiric Embrace",
                            "Vengeful Spirit",
                            "Wither",
                            "Wraith Form"
                        }));

                        if (Core.SE)
                        {
                            entries.Add(new RulesetLayout("Bushido", new string[]
                            {
                                "Confidence",
                                "Counter Attack",
                                "Evasion",
                                "Honorable Execution",
                                "Lightning Strike",
                                "Momentum Strike"
                            }));

                            entries.Add(new RulesetLayout("Ninjitsu", new string[]
                            {
                                "Animal Form",
                                "Backstab",
                                "Death Strike",
                                "Focus Attack",
                                "Ki Attack",
                                "Mirror Image",
                                "Shadow Jump",
                                "Suprise Attack"
                            }));

                            if (Core.ML)
                            {
                                entries.Add(new RulesetLayout("Spellweaving", new string[]
                                {
                                    "Arcane Circle",
                                    "Arcane Empowerment",
                                    "Attune Weapon",
                                    "Dryad Allure",
                                    "Essence of Wind",
                                    "Ethereal Voyage",
                                    "Gift of Life",
                                    "Gift of Renewal",
                                    "Immolating Weapon",
                                    "Nature's Fury",
                                    "Reaper Form",
                                    "Summon Fey",
                                    "Summon Fiend",
                                    "Thunderstorm",
                                    "Wildfire",
                                    "Word of Death"
                                }));
                            }
                        }
                    }

                    if (Core.AOS)
                    {
                        if (Core.SE)
                        {
                            entries.Add(new RulesetLayout("Combat Abilities", new string[]
                            {
                                "Stun",
                                "Disarm",
                                "Armor Ignore",
                                "Bleed Attack",
                                "Concussion Blow",
                                "Crushing Blow",
                                "Disarm",
                                "Dismount",
                                "Double Strike",
                                "Infectious Strike",
                                "Mortal Strike",
                                "Moving Shot",
                                "Paralyzing Blow",
                                "Shadow Strike",
                                "Whirlwind Attack",
                                "Riding Swipe",
                                "Frenzied Whirlwind",
                                "Block",
                                "Defense Mastery",
                                "Nerve Strike",
                                "Talon Strike",
                                "Feint",
                                "Dual Wield",
                                "Double Shot",
                                "Armor Pierce"
                            }));
                            //TODO: ADD ML ABILITIES
                        }
                        else
                        {
                            entries.Add(new RulesetLayout("Combat Abilities", new string[]
                            {
                                "Stun",
                                "Disarm",
                                "Armor Ignore",
                                "Bleed Attack",
                                "Concussion Blow",
                                "Crushing Blow",
                                "Disarm",
                                "Dismount",
                                "Double Strike",
                                "Infectious Strike",
                                "Mortal Strike",
                                "Moving Shot",
                                "Paralyzing Blow",
                                "Shadow Strike",
                                "Whirlwind Attack"
                            }));
                        }
                    }
                    else
                    {
                        entries.Add(new RulesetLayout("Combat Abilities", new string[]
                        {
                            "Stun",
                            "Disarm",
                            "Concussion Blow",
                            "Crushing Blow",
                            "Paralyzing Blow"
                        }));
                    }

                    entries.Add(new RulesetLayout("Skills", new string[]
                    {
                        "Anatomy",
                        "Detect Hidden",
                        "Evaluating Intelligence",
                        "Hiding",
                        "Poisoning",
                        "Snooping",
                        "Stealing",
                        "Spirit Speak",
                        "Stealth"
                    }));

                    if (Core.AOS)
                    {
                        entries.Add(new RulesetLayout("Weapons", new string[]
                        {
                            "Magical",
                            "Melee",
                            "Ranged",
                            "Poisoned",
                            "Wrestling"
                        }));

                        entries.Add(new RulesetLayout("Armor", new string[]
                        {
                            "Magical",
                            "Shields"
                        }));
                    }
                    else
                    {
                        entries.Add(new RulesetLayout("Weapons", new string[]
                        {
                            "Magical",
                            "Melee",
                            "Ranged",
                            "Poisoned",
                            "Wrestling",
                            "Runics"
                        }));

                        entries.Add(new RulesetLayout("Armor", new string[]
                        {
                            "Magical",
                            "Shields",
                            "Colored"
                        }));
                    }

                    if (Core.SE)
                    {
                        entries.Add(new RulesetLayout("Items", new RulesetLayout[]
                        {
                            new RulesetLayout("Potions", new string[]
                            {
                                "Agility",
                                "Cure",
                                "Explosion",
                                "Heal",
                                "Nightsight",
                                "Poison",
                                "Refresh",
                                "Strength"
                            })
                        },
                            new string[]
                            {
                                "Bandages",
                                "Wands",
                                "Trapped Containers",
                                "Bolas",
                                "Mounts",
                                "Orange Petals",
                                "Shurikens",
                                "Fukiya Darts",
                                "Fire Horns"
                            }));
                    }
                    else
                    {
                        entries.Add(new RulesetLayout("Items", new RulesetLayout[]
                        {
                            new RulesetLayout("Potions", new string[]
                            {
                                "Agility",
                                "Cure",
                                "Explosion",
                                "Heal",
                                "Nightsight",
                                "Poison",
                                "Refresh",
                                "Strength"
                            })
                        },
                            new string[]
                            {
                                "Bandages",
                                "Wands",
                                "Trapped Containers",
                                "Bolas",
                                "Mounts",
                                "Orange Petals",
							    "Fire Horns"
                            }));
                    }

                    m_Root = new RulesetLayout("Rules", (RulesetLayout[])entries.ToArray(typeof(RulesetLayout)));
                    m_Root.ComputeOffsets();

                    // Set up default rulesets

                    if (!Core.AOS)
                    {
                        #region Mage 5x
                        Ruleset m5x = new Ruleset(m_Root);

                        m5x.Title = "Mage 5x";

                        m5x.SetOptionRange("Spells", true);

                        m5x.SetOption("Spells", "Wall of Stone", false);
                        m5x.SetOption("Spells", "Fire Field", false);
                        m5x.SetOption("Spells", "Poison Field", false);
                        m5x.SetOption("Spells", "Energy Field", false);
                        m5x.SetOption("Spells", "Reactive Armor", false);
                        m5x.SetOption("Spells", "Protection", false);
                        m5x.SetOption("Spells", "Teleport", false);
                        m5x.SetOption("Spells", "Wall of Stone", false);
                        m5x.SetOption("Spells", "Arch Protection", false);
                        m5x.SetOption("Spells", "Recall", false);
                        m5x.SetOption("Spells", "Blade Spirits", false);
                        m5x.SetOption("Spells", "Incognito", false);
                        m5x.SetOption("Spells", "Magic Reflection", false);
                        m5x.SetOption("Spells", "Paralyze", false);
                        m5x.SetOption("Spells", "Summon Creature", false);
                        m5x.SetOption("Spells", "Invisibility", false);
                        m5x.SetOption("Spells", "Mark", false);
                        m5x.SetOption("Spells", "Paralyze Field", false);
                        m5x.SetOption("Spells", "Energy Field", false);
                        m5x.SetOption("Spells", "Gate Travel", false);
                        m5x.SetOption("Spells", "Polymorph", false);
                        m5x.SetOption("Spells", "Energy Vortex", false);
                        m5x.SetOption("Spells", "Air Elemental", false);
                        m5x.SetOption("Spells", "Summon Daemon", false);
                        m5x.SetOption("Spells", "Earth Elemental", false);
                        m5x.SetOption("Spells", "Fire Elemental", false);
                        m5x.SetOption("Spells", "Water Elemental", false);
                        m5x.SetOption("Spells", "Earthquake", false);
                        m5x.SetOption("Spells", "Meteor Swarm", false);
                        m5x.SetOption("Spells", "Chain Lightning", false);
                        m5x.SetOption("Spells", "Resurrection", false);

                        m5x.SetOption("Weapons", "Wrestling", true);

                        m5x.SetOption("Skills", "Anatomy", true);
                        m5x.SetOption("Skills", "Detect Hidden", true);
                        m5x.SetOption("Skills", "Evaluating Intelligence", true);

                        m5x.SetOption("Items", "Trapped Containers", true);
                        #endregion

                        #region Mage 7x
                        Ruleset m7x = new Ruleset(m_Root);

                        m7x.Title = "Mage 7x";

                        m7x.SetOptionRange("Spells", true);

                        m7x.SetOption("Spells", "Wall of Stone", false);
                        m7x.SetOption("Spells", "Fire Field", false);
                        m7x.SetOption("Spells", "Poison Field", false);
                        m7x.SetOption("Spells", "Energy Field", false);
                        m7x.SetOption("Spells", "Reactive Armor", false);
                        m7x.SetOption("Spells", "Protection", false);
                        m7x.SetOption("Spells", "Teleport", false);
                        m7x.SetOption("Spells", "Wall of Stone", false);
                        m7x.SetOption("Spells", "Arch Protection", false);
                        m7x.SetOption("Spells", "Recall", false);
                        m7x.SetOption("Spells", "Blade Spirits", false);
                        m7x.SetOption("Spells", "Incognito", false);
                        m7x.SetOption("Spells", "Magic Reflection", false);
                        m7x.SetOption("Spells", "Paralyze", false);
                        m7x.SetOption("Spells", "Summon Creature", false);
                        m7x.SetOption("Spells", "Invisibility", false);
                        m7x.SetOption("Spells", "Mark", false);
                        m7x.SetOption("Spells", "Paralyze Field", false);
                        m7x.SetOption("Spells", "Energy Field", false);
                        m7x.SetOption("Spells", "Gate Travel", false);
                        m7x.SetOption("Spells", "Polymorph", false);
                        m7x.SetOption("Spells", "Energy Vortex", false);
                        m7x.SetOption("Spells", "Air Elemental", false);
                        m7x.SetOption("Spells", "Summon Daemon", false);
                        m7x.SetOption("Spells", "Earth Elemental", false);
                        m7x.SetOption("Spells", "Fire Elemental", false);
                        m7x.SetOption("Spells", "Water Elemental", false);
                        m7x.SetOption("Spells", "Earthquake", false);
                        m7x.SetOption("Spells", "Meteor Swarm", false);
                        m7x.SetOption("Spells", "Chain Lightning", false);
                        m7x.SetOption("Spells", "Resurrection", false);

                        m7x.SetOption("Combat Abilities", "Stun", true);

                        m7x.SetOption("Skills", "Anatomy", true);
                        m7x.SetOption("Skills", "Detect Hidden", true);
                        m7x.SetOption("Skills", "Poisoning", true);
                        m7x.SetOption("Skills", "Evaluating Intelligence", true);

                        m7x.SetOption("Weapons", "Wrestling", true);

                        m7x.SetOption("Potions", "Refresh", true);
                        m7x.SetOption("Items", "Trapped Containers", true);
                        m7x.SetOption("Items", "Bandages", true);
                        #endregion

                        #region Standard 7x
                        Ruleset s7x = new Ruleset(m_Root);

                        s7x.Title = "Standard 7x";

                        s7x.SetOptionRange("Spells", true);

                        s7x.SetOption("Spells", "Wall of Stone", false);
                        s7x.SetOption("Spells", "Fire Field", false);
                        s7x.SetOption("Spells", "Poison Field", false);
                        s7x.SetOption("Spells", "Energy Field", false);
                        s7x.SetOption("Spells", "Teleport", false);
                        s7x.SetOption("Spells", "Wall of Stone", false);
                        s7x.SetOption("Spells", "Arch Protection", false);
                        s7x.SetOption("Spells", "Recall", false);
                        s7x.SetOption("Spells", "Blade Spirits", false);
                        s7x.SetOption("Spells", "Incognito", false);
                        s7x.SetOption("Spells", "Magic Reflection", false);
                        s7x.SetOption("Spells", "Paralyze", false);
                        s7x.SetOption("Spells", "Summon Creature", false);
                        s7x.SetOption("Spells", "Invisibility", false);
                        s7x.SetOption("Spells", "Mark", false);
                        s7x.SetOption("Spells", "Paralyze Field", false);
                        s7x.SetOption("Spells", "Energy Field", false);
                        s7x.SetOption("Spells", "Gate Travel", false);
                        s7x.SetOption("Spells", "Polymorph", false);
                        s7x.SetOption("Spells", "Energy Vortex", false);
                        s7x.SetOption("Spells", "Air Elemental", false);
                        s7x.SetOption("Spells", "Summon Daemon", false);
                        s7x.SetOption("Spells", "Earth Elemental", false);
                        s7x.SetOption("Spells", "Fire Elemental", false);
                        s7x.SetOption("Spells", "Water Elemental", false);
                        s7x.SetOption("Spells", "Earthquake", false);
                        s7x.SetOption("Spells", "Meteor Swarm", false);
                        s7x.SetOption("Spells", "Chain Lightning", false);
                        s7x.SetOption("Spells", "Resurrection", false);

                        s7x.SetOptionRange("Combat Abilities", true);

                        s7x.SetOption("Skills", "Anatomy", true);
                        s7x.SetOption("Skills", "Detect Hidden", true);
                        s7x.SetOption("Skills", "Poisoning", true);
                        s7x.SetOption("Skills", "Evaluating Intelligence", true);

                        s7x.SetOptionRange("Weapons", true);
                        s7x.SetOption("Weapons", "Runics", false);
                        s7x.SetOptionRange("Armor", true);

                        s7x.SetOption("Potions", "Refresh", true);
                        s7x.SetOption("Items", "Bandages", true);
                        s7x.SetOption("Items", "Trapped Containers", true);
                        #endregion

                        m_Root.Defaults = new Ruleset[] { m5x, m7x, s7x };
                    }
                    else
                    {
                        #region Standard All Skills
                        Ruleset all = new Ruleset(m_Root);

                        all.Title = "Standard All Skills";

                        all.SetOptionRange("Spells", true);

                        all.SetOption("Spells", "Wall of Stone", false);
                        all.SetOption("Spells", "Fire Field", false);
                        all.SetOption("Spells", "Poison Field", false);
                        all.SetOption("Spells", "Energy Field", false);
                        all.SetOption("Spells", "Teleport", false);
                        all.SetOption("Spells", "Wall of Stone", false);
                        all.SetOption("Spells", "Arch Protection", false);
                        all.SetOption("Spells", "Recall", false);
                        all.SetOption("Spells", "Blade Spirits", false);
                        all.SetOption("Spells", "Incognito", false);
                        all.SetOption("Spells", "Magic Reflection", false);
                        all.SetOption("Spells", "Paralyze", false);
                        all.SetOption("Spells", "Summon Creature", false);
                        all.SetOption("Spells", "Invisibility", false);
                        all.SetOption("Spells", "Mark", false);
                        all.SetOption("Spells", "Paralyze Field", false);
                        all.SetOption("Spells", "Energy Field", false);
                        all.SetOption("Spells", "Gate Travel", false);
                        all.SetOption("Spells", "Polymorph", false);
                        all.SetOption("Spells", "Energy Vortex", false);
                        all.SetOption("Spells", "Air Elemental", false);
                        all.SetOption("Spells", "Summon Daemon", false);
                        all.SetOption("Spells", "Earth Elemental", false);
                        all.SetOption("Spells", "Fire Elemental", false);
                        all.SetOption("Spells", "Water Elemental", false);
                        all.SetOption("Spells", "Earthquake", false);
                        all.SetOption("Spells", "Meteor Swarm", false);
                        all.SetOption("Spells", "Chain Lightning", false);
                        all.SetOption("Spells", "Resurrection", false);

                        all.SetOptionRange("Necromancy", true);
                        all.SetOption("Necromancy", "Summon Familiar", false);
                        all.SetOption("Necromancy", "Vengeful Spirit", false);
                        all.SetOption("Necromancy", "Animate Dead", false);
                        all.SetOption("Necromancy", "Wither", false);
                        all.SetOption("Necromancy", "Poison Strike", false);

                        all.SetOptionRange("Chivalry", true);
                        all.SetOption("Chivalry", "Sacred Journey", false);
                        all.SetOption("Chivalry", "Enemy of One", false);
                        all.SetOption("Chivalry", "Noble Sacrifice", false);

                        all.SetOptionRange("Combat Abilities", true);
                        all.SetOption("Combat Abilities", "Paralyzing Blow", false);
                        all.SetOption("Combat Abilities", "Shadow Strike", false);

                        all.SetOption("Skills", "Anatomy", true);
                        all.SetOption("Skills", "Detect Hidden", true);
                        all.SetOption("Skills", "Poisoning", true);
                        all.SetOption("Skills", "Spirit Speak", true);
                        all.SetOption("Skills", "Evaluating Intelligence", true);

                        all.SetOptionRange("Weapons", true);
                        all.SetOption("Weapons", "Poisoned", false);

                        all.SetOptionRange("Armor", true);

                        all.SetOptionRange("Ninjitsu", true);
                        all.SetOption("Ninjitsu", "Animal Form", false);
                        all.SetOption("Ninjitsu", "Mirror Image", false);
                        all.SetOption("Ninjitsu", "Backstab", false);
                        all.SetOption("Ninjitsu", "Suprise Attack", false);
                        all.SetOption("Ninjitsu", "Shadow Jump", false);

                        all.SetOptionRange("Bushido", true);

                        all.SetOptionRange("Spellweaving", true);
                        all.SetOption("Spellweaving", "Gift of Life", false);
                        all.SetOption("Spellweaving", "Summon Fey", false);
                        all.SetOption("Spellweaving", "Summon Fiend", false);
                        all.SetOption("Spellweaving", "Nature's Fury", false);

                        all.SetOption("Potions", "Refresh", true);
                        all.SetOption("Items", "Bandages", true);
                        all.SetOption("Items", "Trapped Containers", true);

                        m_Root.Defaults = new Ruleset[] { all };
                        #endregion
                    }

                    // Set up flavors

                    Ruleset pots = new Ruleset(m_Root);

                    pots.Title = "Potions";

                    pots.SetOptionRange("Potions", true);
                    pots.SetOption("Potions", "Explosion", false);

                    Ruleset para = new Ruleset(m_Root);

                    para.Title = "Paralyze";
                    para.SetOption("Spells", "Paralyze", true);
                    para.SetOption("Spells", "Paralyze Field", true);
                    para.SetOption("Combat Abilities", "Paralyzing Blow", true);

                    Ruleset fields = new Ruleset(m_Root);

                    fields.Title = "Fields";
                    fields.SetOption("Spells", "Wall of Stone", true);
                    fields.SetOption("Spells", "Fire Field", true);
                    fields.SetOption("Spells", "Poison Field", true);
                    fields.SetOption("Spells", "Energy Field", true);
                    fields.SetOption("Spells", "Wildfire", true);

                    Ruleset area = new Ruleset(m_Root);

                    area.Title = "Area Effect";
                    area.SetOption("Spells", "Earthquake", true);
                    area.SetOption("Spells", "Meteor Swarm", true);
                    area.SetOption("Spells", "Chain Lightning", true);
                    area.SetOption("Necromancy", "Wither", true);
                    area.SetOption("Necromancy", "Poison Strike", true);

                    Ruleset summons = new Ruleset(m_Root);

                    summons.Title = "Summons";
                    summons.SetOption("Spells", "Blade Spirits", true);
                    summons.SetOption("Spells", "Energy Vortex", true);
                    summons.SetOption("Spells", "Air Elemental", true);
                    summons.SetOption("Spells", "Summon Daemon", true);
                    summons.SetOption("Spells", "Earth Elemental", true);
                    summons.SetOption("Spells", "Fire Elemental", true);
                    summons.SetOption("Spells", "Water Elemental", true);
                    summons.SetOption("Necromancy", "Summon Familiar", true);
                    summons.SetOption("Necromancy", "Vengeful Spirit", true);
                    summons.SetOption("Necromancy", "Animate Dead", true);
                    summons.SetOption("Ninjitsu", "Mirror Image", true);
                    summons.SetOption("Spellweaving", "Summon Fey", true);
                    summons.SetOption("Spellweaving", "Summon Fiend", true);
                    summons.SetOption("Spellweaving", "Nature's Fury", true);

                    m_Root.Flavors = new Ruleset[] { pots, para, fields, area, summons };
                }

                return m_Root;
            }
        }

        private readonly string m_Title;

        private readonly string m_Description;

        private readonly string[] m_Options;

        private int m_Offset, m_TotalLength;

        private Ruleset[] m_Defaults;
        private Ruleset[] m_Flavors;

        private RulesetLayout m_Parent;
        private readonly RulesetLayout[] m_Children;

        public string Title
        {
            get
            {
                return this.m_Title;
            }
        }
        public string Description
        {
            get
            {
                return this.m_Description;
            }
        }
        public string[] Options
        {
            get
            {
                return this.m_Options;
            }
        }

        public int Offset
        {
            get
            {
                return this.m_Offset;
            }
        }
        public int TotalLength
        {
            get
            {
                return this.m_TotalLength;
            }
        }

        public RulesetLayout Parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public RulesetLayout[] Children
        {
            get
            {
                return this.m_Children;
            }
        }

        public Ruleset[] Defaults
        {
            get
            {
                return this.m_Defaults;
            }
            set
            {
                this.m_Defaults = value;
            }
        }
        public Ruleset[] Flavors
        {
            get
            {
                return this.m_Flavors;
            }
            set
            {
                this.m_Flavors = value;
            }
        }

        public RulesetLayout FindByTitle(string title)
        {
            if (this.m_Title == title)
                return this;

            for (int i = 0; i < this.m_Children.Length; ++i)
            {
                RulesetLayout layout = this.m_Children[i].FindByTitle(title);

                if (layout != null)
                    return layout;
            }

            return null;
        }

        public string FindByIndex(int index)
        {
            if (index >= this.m_Offset && index < (this.m_Offset + this.m_Options.Length))
                return this.m_Description + ": " + this.m_Options[index - this.m_Offset];

            for (int i = 0; i < this.m_Children.Length; ++i)
            {
                string opt = this.m_Children[i].FindByIndex(index);

                if (opt != null)
                    return opt;
            }

            return null;
        }

        public RulesetLayout FindByOption(string title, string option, ref int index)
        {
            if (title == null || this.m_Title == title)
            {
                index = this.GetOptionIndex(option);

                if (index >= 0)
                    return this;

                title = null;
            }

            for (int i = 0; i < this.m_Children.Length; ++i)
            {
                RulesetLayout layout = this.m_Children[i].FindByOption(title, option, ref index);

                if (layout != null)
                    return layout;
            }

            return null;
        }

        public int GetOptionIndex(string option)
        {
            return Array.IndexOf(this.m_Options, option);
        }

        public void ComputeOffsets()
        {
            int offset = 0;

            this.RecurseComputeOffsets(ref offset);
        }

        private int RecurseComputeOffsets(ref int offset)
        {
            this.m_Offset = offset;

            offset += this.m_Options.Length;
            this.m_TotalLength += this.m_Options.Length;

            for (int i = 0; i < this.m_Children.Length; ++i)
                this.m_TotalLength += this.m_Children[i].RecurseComputeOffsets(ref offset);

            return this.m_TotalLength;
        }

        public RulesetLayout(string title, string[] options)
            : this(title, title, new RulesetLayout[0], options)
        {
        }

        public RulesetLayout(string title, string description, string[] options)
            : this(title, description, new RulesetLayout[0], options)
        {
        }

        public RulesetLayout(string title, RulesetLayout[] children)
            : this(title, title, children, new string[0])
        {
        }

        public RulesetLayout(string title, string description, RulesetLayout[] children)
            : this(title, description, children, new string[0])
        {
        }

        public RulesetLayout(string title, RulesetLayout[] children, string[] options)
            : this(title, title, children, options)
        {
        }

        public RulesetLayout(string title, string description, RulesetLayout[] children, string[] options)
        {
            this.m_Title = title;
            this.m_Description = description;
            this.m_Children = children;
            this.m_Options = options;

            for (int i = 0; i < children.Length; ++i)
                children[i].m_Parent = this;
        }
    }
}