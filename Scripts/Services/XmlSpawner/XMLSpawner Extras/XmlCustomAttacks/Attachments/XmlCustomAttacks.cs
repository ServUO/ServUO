using System;
using System.Data;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Spells;
using System.Collections;
using Server.Gumps;
using System.Text;

/*
** XmlCustomAttacks
** 11/16/04
** ArteGordon
**
** This attachment will allow you to create a system for adding special attacks to weapons including combo attacks that require
** a series of specific special attacks to be executed in a timed sequence
**
** Version 1.01
** updated 11/26/04
** - restricted attachment to weapons only.
** - added a few more example custom attacks
*/
namespace Server.Engines.XmlSpawner2
{
    public class XmlCustomAttacks : XmlAttachment
    {

        // ------------------------------------------------------------------------------
        // BEGINNING of user-defined special attacks and combos information
        // ------------------------------------------------------------------------------

        //
        // define the Combo and special attack enums
        //
        // you must first add entries here if you wish to add new attacks
        //
        
        // ATTACKS
        public enum ComboAttacks
        {
            ThunderStrike,
            LightningRain,
            SqueezingFist
        }

        public enum SpecialAttacks
        {
            TripleSlash,
            MindDrain,
            StamDrain,
            ParalyzingFear,
            GiftOfHealth,
            VortexStrike,
            PuffOfSmoke
        }

        public static new void Initialize()
        {
            //
            // define the special attacks and their use requirements
            //
            // ideally, you have a definition for every SpecialAttacks enum.  Although it isnt absolutely necessary,
            // if it isnt defined here, it will not be available for use

            AddSpecialAttack("Triple Slash", "Deals triple the max damage of the weapon",        // attack name, and description
                SpecialAttacks.TripleSlash, 0x520C, TimeSpan.FromSeconds(3.5),                   // attack id, id of gump icon, and chaining time
                30, 20, 5, 0,                                                                    // mana, stam, hits, karma usage
                70,30,0,                                                                        // str, dex, int requirements
                new SkillName [] { SkillName.ArmsLore },                                        //  skill requirement list
                new int [] { 50 },                                                               // minimum skill levels
                new Type [] {typeof(MandrakeRoot)},                                              // reagent list
                new int [] { 1 }                                                                 // reagent quantities
            );

            AddSpecialAttack("Mind Drain", "Drains mana from the target",
                SpecialAttacks.MindDrain, 0x5007, IconTypes.GumpID, TimeSpan.FromSeconds(3.5),  // explicitly specifying the gump icon as a gumpid
                10, 5, 5, 0,
                0,0,40,
                new SkillName [] { SkillName.Magery },
                new int [] { 50 },
                null,
                null
            );
            AddSpecialAttack("Vortex Strike", "Calls an energy vortex to aid you in battle",
                SpecialAttacks.VortexStrike, 0x20b9, IconTypes.ItemID,TimeSpan.FromSeconds(3.5), // example of using an itemid for the gump icon
                40, 20, 0, 0,
                0,0,30,
                new SkillName [] { SkillName.Magery },
                new int [] { 60 },
                new Type [] {typeof(Diamond)},
                new int [] { 3 }
            );
            

            AddSpecialAttack("Stam Drain", "Drains stamina from the target",
                SpecialAttacks.StamDrain, 0x500e, TimeSpan.FromSeconds(3.5),                // if the icon type is not specified, gump icons use gumpids
                30, 5, 0, 0,
                40, 40, 0,
                null,
                null,
                new Type [] {typeof(Ginseng), typeof(Garlic)},
                new int [] { 1, 2 }
            );

            AddSpecialAttack("Paralyzing Fear", "Paralyzes the target and causes it to flee",
                SpecialAttacks.ParalyzingFear, 0x500d, TimeSpan.FromSeconds(3.5),
                10, 10, 5, 10,
                0, 0, 40,
                new SkillName [] { SkillName.Necromancy },
                new int [] { 30 },
                new Type [] {typeof(Head)},
                new int [] { 1 }
            );

            AddSpecialAttack("Gift of Health", "Restores attacker to full health",
                SpecialAttacks.GiftOfHealth, 0x500c, TimeSpan.FromSeconds(3.5),
                40, 20, 0, 0,
                0, 0, 30,
                new SkillName [] { SkillName.Healing },
                new int [] { 60 },
                new Type [] {typeof(Ginseng), typeof(MandrakeRoot), typeof(Gold)},
                new int [] { 2, 2, 50 }
            );

            AddSpecialAttack("Puff of Smoke", "Makes the attacker invisible",
                SpecialAttacks.PuffOfSmoke, 0x520b, TimeSpan.FromSeconds(3.5),
                20, 40, 0, 0,
                0, 40, 0,
                new SkillName [] { SkillName.Stealth, SkillName.Hiding },
                new int [] { 50, 50 },
                new Type [] {typeof(SpidersSilk)},
                new int [] { 2 }
            );


            //
            // define combos and the sequence of special attacks needed to activate them
            //
            AddComboAttack( "Thunder Strike", ComboAttacks.ThunderStrike,                       // combo name, and id
                new SpecialAttacks []                                                           // list of special attacks needed to complete the combo
                    {
                    SpecialAttacks.TripleSlash,
                    SpecialAttacks.MindDrain,
                    SpecialAttacks.ParalyzingFear,
                    SpecialAttacks.TripleSlash,
                    SpecialAttacks.StamDrain
                    }
            );

            AddComboAttack( "Lightning Rain", ComboAttacks.LightningRain,
                new SpecialAttacks []
                    {
                    SpecialAttacks.TripleSlash,
                    SpecialAttacks.MindDrain,
                    SpecialAttacks.MindDrain,
                    SpecialAttacks.StamDrain
                    }
            );

            AddComboAttack( "Squeezing Fist", ComboAttacks.SqueezingFist,
                new SpecialAttacks []
                    {
                    SpecialAttacks.MindDrain,
                    SpecialAttacks.StamDrain
                    }
            );

            // after deser, restore combo and specials lists to all existing CustomAttacks attachments based on these definitions
            foreach(XmlAttachment x in XmlAttach.AllAttachments.Values)
            {
                if(x is XmlCustomAttacks)
                {
                    ((XmlCustomAttacks)x).FillComboList();
                    ((XmlCustomAttacks)x).FillSpecialsList();
                }
            }
        }

        //
        // carry out the special attacks
        //
        // If you add a new attack, you must add the code here to define what it actually does when it hits
        //
        public void DoSpecialAttack(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, SpecialAttack special)
		{

		    if(attacker == null || defender == null || weapon == null || special == null) return;

            attacker.SendMessage("you strike with {0}!", special.Name);

            // apply the special attack
            switch(special.AttackID)
            {
                case SpecialAttacks.TripleSlash:
                {
                    defender.Damage( weapon.MaxDamage*3, attacker );
                    break;
                }
                case SpecialAttacks.MindDrain:
                {
                    defender.Mana -= weapon.MaxDamage;
					attacker.Mana += weapon.MaxDamage;
                    break;
                }
                case SpecialAttacks.VortexStrike:
                {
                    attacker.PlaySound( 0x217 );
                    BaseCreature m = new EnergyVortex();
                    m.Summoned = true;
                    m.SummonMaster = attacker;
                    m.Combatant = defender;
                    m.MoveToWorld(defender.Location, defender.Map);
                    break;
                }
                case SpecialAttacks.StamDrain:
                {
                    defender.Stam -= weapon.MaxDamage;
					attacker.Stam += weapon.MaxDamage;
                    break;
                }
                case SpecialAttacks.PuffOfSmoke:
                {
                    attacker.Hidden = true;
                    break;
                }
                case SpecialAttacks.GiftOfHealth:
                {
                    attacker.FixedEffect( 0x376A, 9, 32 );
                    attacker.PlaySound( 0x202 );
                    attacker.Hits = attacker.HitsMax;
                    break;
                }
                case SpecialAttacks.ParalyzingFear:
                {
                    // lose target focus
                    defender.Combatant = null;
                    // flee
                    if(defender is BaseCreature)
                    {
                    	((BaseCreature)defender).BeginFlee(TimeSpan.FromSeconds(6));
                    }
                    // and become paralyzed
                    defender.Freeze( TimeSpan.FromSeconds(3) );
			        defender.FixedEffect( 0x376A, 9, 32 );
			        defender.PlaySound( 0x204 );
                    break;
                }
                default:
                    attacker.SendMessage("no effect");
                    break;
            }
        }

        //
        // carry out the combo attacks
        //
        // If you add a new combo, you must add the code here to define what it actually does when it is activated
        //
        public void DoComboAttack(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, ComboAttack combo)
		{
		    if(attacker == null || defender == null || weapon == null || combo == null) return;

		    attacker.SendMessage("You unleash the combo attack {0}!",combo.Name);

		    // apply the combo attack
            switch(combo.AttackID)
            {
                case ComboAttacks.ThunderStrike:
                {
                    defender.FixedEffect( 0x376A, 9, 32 );
			        defender.PlaySound( 0x28 );
			        // 5x damage
                    defender.Damage( weapon.MaxDamage*5, attacker );
                    // mana and stam drain
                    defender.Mana -= weapon.MaxDamage*3;
                    defender.Stam -= weapon.MaxDamage*3;
                    // full self heal
                    attacker.FixedEffect( 0x376A, 9, 32 );
                    attacker.Hits = attacker.HitsMax;
                    break;
                }
                case ComboAttacks.LightningRain:
                {
                    defender.Damage( weapon.MaxDamage*3, attacker );
                    defender.Mana -= weapon.MaxDamage*7;
                    defender.Stam -= weapon.MaxDamage*4;
                    break;
                }
                case ComboAttacks.SqueezingFist:
                {
                    // 5 sec paralyze
                    defender.FixedEffect( 0x376A, 9, 32 );
			        defender.PlaySound( 0x204 );
                    defender.Freeze( TimeSpan.FromSeconds(5) );
                    // 7x stam drain
                    defender.Stam -= weapon.MaxDamage*7;
                    break;
                }
            }
		}


		[Attachable]
        public XmlCustomAttacks(string name, int nspecials)
        {

            FillComboList();

            //
            // you can put any named specials configurations that you want in here
            //
            if(String.Compare("tartan", name, true) == 0)
            {
                AddSpecial(SpecialAttacks.TripleSlash);
                AddSpecial(SpecialAttacks.MindDrain);
                AddSpecial(SpecialAttacks.StamDrain);
                AddSpecial(SpecialAttacks.ParalyzingFear);
            } else
            if(String.Compare("balzog", name, true) == 0)
            {
                AddSpecial(SpecialAttacks.StamDrain);
                AddSpecial(SpecialAttacks.ParalyzingFear);
            } else
            if(String.Compare("klamath", name, true) == 0)
            {
                AddSpecial(SpecialAttacks.TripleSlash);
                AddSpecial(SpecialAttacks.MindDrain);
            } else
            if(String.Compare("random", name, true) == 0)
            {
                // assign the requested number of random special attacks
                SetRandomSpecials(nspecials);
            }
        }


        // this constructor is intended to be called from within scripts that wish to define custom attack configurations
        // by passing it a list of SpecialAttacks
        public XmlCustomAttacks(SpecialAttacks [] attacklist)
        {
            if(attacklist != null)
            {
                foreach(SpecialAttacks sid in attacklist)
                {
                    AddSpecial(sid);
                }
            }
        }
        
        public XmlCustomAttacks(SpecialAttacks attack)
        {
            AddSpecial(attack);
        }
        
        [Attachable]
        public XmlCustomAttacks(string name) : this(name, 1)
        {
        }
        
        [Attachable]
        public XmlCustomAttacks() : this("random", 1)
        {
        }

		// ------------------------------------------------------------------------------
        // END of user-defined special attacks and combos information
        // ------------------------------------------------------------------------------

        private static Hashtable AllSpecials = new Hashtable();
        private static Hashtable AllCombos = new Hashtable();
        
        public enum IconTypes
        {
            GumpID,
            ItemID
        }

        public class SpecialAttack
        {
            public string      Name;           // attack name
            public string      Description;    // attack description
            public SpecialAttacks         AttackID;  // attack id
            public TimeSpan    ChainTime;    // time available until next attack in the chain must be performed
            public int Icon;                 // button icon for this attack
            public IconTypes IconType;          // what type of art to use for button icon
            public int ManaReq;             // mana usage for this attack
            public int StamReq;             // stamina usage for this attack
            public int HitsReq;             // hits usage for this attack
            public int KarmaReq;            // karma usage for this attack
            public int StrReq;             // str requirements for this attack
            public int DexReq;             // dex requirements for this attack
            public int IntReq;             // int requirements for this attack
            public Type [] Reagents;       // reagent list used for this attack
            public int [] Quantity;        // reagent quantity list
            public SkillName [] Skills;    // list of skill requirements for this attack
            public int [] MinSkillLevel;   // minimum skill levels

            public SpecialAttack( string name, string description, SpecialAttacks id, int icon, IconTypes itype, TimeSpan duration,
            int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
            SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
            {
                Name = name;
                Description = description;
                AttackID = id;
                ChainTime = duration;
                Icon = icon;
                IconType = itype;
                ManaReq = mana;
                StamReq = stam;
                HitsReq = hits;
                KarmaReq = karma;
                StrReq = minstr;
                DexReq = mindex;
                IntReq = minint;
                Reagents = reagents;
                Quantity = quantity;
                Skills = skills;
                MinSkillLevel = minlevel;
            }
        }

        public class ComboAttack
        {
            public string Name;
            public ComboAttacks AttackID;
            public SpecialAttacks [] AttackSequence;

            public ComboAttack( string name, ComboAttacks id, SpecialAttacks [] sequence)
            {
                Name = name;
                AttackID = id;
                AttackSequence = sequence;
            }
        }

        public class ActiveCombo
        {
            public ComboAttack Combo;
            public int PositionInSequence;

            public ActiveCombo(ComboAttack c)
            {
                Combo = c;
                PositionInSequence  = 0;
            }
        }

        private ComboTimer m_ComboTimer;
        public SpecialAttack m_SelectedAttack;

        // these are the lists of special moves and combo status for each instance
        private ArrayList Specials = new ArrayList();
        private ArrayList Combos = new ArrayList();

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlCustomAttacks(ASerial serial) : base(serial)
        {
        }

        public override void Serialize( GenericWriter writer )
		{
            base.Serialize(writer);

            writer.Write( (int) 0 );

            // version 0
            // save the specials for this instance
            writer.Write( Specials.Count);

            foreach(SpecialAttack s in Specials)
            {
                writer.Write(s.AttackID.ToString());
            }
        }

        private ArrayList tmpSpecialsList = new ArrayList();

        public override void Deserialize(GenericReader reader)
		{
		    base.Deserialize(reader);

            int version = reader.ReadInt();
            switch(version)
            {

            case 0:
                // version 0
                int count = reader.ReadInt();

                for(int i = 0; i<count;i++)
                {
                    string sname = reader.ReadString();

                    try{
                    SpecialAttacks stype = (SpecialAttacks)Enum.Parse(typeof(SpecialAttacks), sname);
                    tmpSpecialsList.Add(stype);
                    } catch{}
                }
                break;
            }
		}
		
		private static void AddSpecialAttack( string name, string description, SpecialAttacks id, int icon, TimeSpan duration,
        int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
        SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
        {
            AddSpecialAttack( name, description, id, icon, IconTypes.GumpID, duration,
            mana, stam, hits, karma, minstr, mindex, minint,
            skills, minlevel, reagents, quantity);
        }
		
		private static void AddSpecialAttack( string name, string description, SpecialAttacks id, int icon, IconTypes itype, TimeSpan duration,
        int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
        SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
        {
            AllSpecials.Add(id, new SpecialAttack( name, description, id, icon, itype,
            duration, mana, stam, hits, karma,
            minstr, mindex, minint, skills, minlevel, reagents, quantity) );
        }

        private static SpecialAttack GetSpecialAttack( SpecialAttacks id )
        {
            return((SpecialAttack)AllSpecials[id]);
        }
        
        public void AddSpecial( SpecialAttacks id )
        {
            SpecialAttack s = GetSpecialAttack(id);

            if(s != null)
                Specials.Add(s);
        }


        private static void AddComboAttack( string name, ComboAttacks id, SpecialAttacks [] sequence)
        {
            AllCombos.Add(id, new ComboAttack( name, id,sequence) );
        }

        private static ComboAttack GetComboAttack( ComboAttacks id )
        {
            return((ComboAttack)AllCombos[id]);
        }
        
        public static void AddAttack(object target, SpecialAttacks attack)
        {
            // is there an existing custom attacks attachment to add to?
            XmlCustomAttacks a = (XmlCustomAttacks)XmlAttach.FindAttachment(target, typeof(XmlCustomAttacks));

            if(a == null)
            {
                // add a new custom attacks attachment
                XmlAttach.AttachTo(target, new XmlCustomAttacks(attack));
            } else
            {
                // add the new attack to existing attack list
                a.AddSpecial(attack);
            }
        }

        private void FillComboList()
        {
            Combos = new ArrayList();

            foreach(ComboAttack c in AllCombos.Values)
            {
                if(c != null)
                {
                    Combos.Add(new ActiveCombo(c));
                }
            }
        }

        private void FillSpecialsList()
        {
            Specials = new ArrayList();

            foreach(SpecialAttacks sid in tmpSpecialsList)
            {
                SpecialAttack s = GetSpecialAttack(sid);
                if(s != null)
                {
                    Specials.Add(s);
                }
            }
        }

        private void SetRandomSpecials(int count)
        {
            int nspecials = AllSpecials.Count;

            if(nspecials > 0)
            {
                int ntries = 0;
                int total = 0;
                ArrayList tmplist = new ArrayList();
                while(total < count)
                {
                    int rand = Utility.Random(nspecials);
                    int nrtries = 0;
                    while(tmplist.Contains(rand))
                    {
                        rand = Utility.Random(nspecials);
                        // add some sanity checking
                        if(nrtries++ > 100) break;
                    }

                    tmplist.Add(rand);

                    SpecialAttack s = GetSpecialAttack((SpecialAttacks)rand);
                    if(s != null)
                    {
                        Specials.Add(s);
                        total++;
                    }

                    // add some sanity checking
                    if(tmplist.Count >= nspecials) break;
                    if(ntries++ > 100) break;
                }
            }
        }

		private void ResetCombos()
		{
            foreach(ActiveCombo c in Combos)
            {
                c.PositionInSequence = 0;
            }
		}
		
		private bool HasActiveCombos
		{
            get{
                if(Combos == null) return false;

                foreach(ActiveCombo c in Combos)
                {
                    if(c.PositionInSequence > 0) return true;
                }
                
                return false;
            }
		}

		private void CheckCombos(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, SpecialAttack s)
		{
            if(s == null) return;

            foreach(ActiveCombo c in Combos)
            {
                if(c != null && c.Combo != null && c.Combo.AttackSequence != null && c.PositionInSequence < c.Combo.AttackSequence.Length)
                {
                    if(c.Combo.AttackSequence[c.PositionInSequence] == s.AttackID)
                    {
                        if(++c.PositionInSequence >= c.Combo.AttackSequence.Length)
                        {
                            // combo is complete so execute it
                            DoComboAttack(attacker, defender, weapon, damageGiven, c.Combo);

                            // and reset it
                            c.PositionInSequence = 0;
                        }
                    } else
                    {
                        // out of sequence so reset the combo
                        c.PositionInSequence = 0;
                    }
                }
            }
		}

		public static bool CheckRequirements(Mobile from, SpecialAttack s)
        {
            if(from == null || s == null) return false;
            
            // test for str, dex, int requirements
            if(from.Str < s.StrReq)
            {
                from.SendMessage("Need {0} Str to perform {1}", s.StrReq, s.Name);
                return false;
            }
            if(from.Dex < s.DexReq)
            {
                from.SendMessage("Need {0} Dex to perform {1}", s.DexReq, s.Name);
                return false;
            }
            if(from.Int < s.IntReq)
            {
                from.SendMessage("Need {0} Int to perform {1}", s.IntReq, s.Name);
                return false;
            }
            
            // test for skill requirements
            if(s.Skills != null && s.MinSkillLevel != null)
            {
                if(from.Skills == null) return false;

                for(int i = 0; i < s.Skills.Length; i++)
                {
                    // and check level
                    if(i < s.MinSkillLevel.Length)
                    {
                        Skill skill =  from.Skills[s.Skills[i]];
                        if(skill != null && s.MinSkillLevel[i] > skill.Base)
                        {
                            from.SendMessage("Need {0} {1} to perform {2}", s.MinSkillLevel[i], s.Skills[i].ToString(), s.Name);
                            return false;
                        }
                    } else
                    {
                        from.SendMessage(33,"Error in skill level specification for {0}", s.Name);
                        return false;
                    }
                }
            }


            // test for mana, stam, and hits requirements
            if(from.Mana < s.ManaReq)
            {
                from.SendMessage("Need {0} Mana to perform {1}", s.ManaReq, s.Name);
                return false;
            }
            if(from.Stam < s.StamReq)
            {
                from.SendMessage("Need {0} Stamina to perform {1}", s.StamReq, s.Name);
                return false;
            }
            if(from.Hits < s.HitsReq)
            {
                // clear the selected attack
                from.SendMessage("Need {0} Hits to perform {1}", s.HitsReq, s.Name);
                return false;
            }


            // check for any reagents that are specified
            if(s.Reagents != null && s.Quantity != null)
            {
                if(from.Backpack == null) return false;

                for(int i = 0; i < s.Reagents.Length; i++)
                {
                    // go through each reagent
                    Item ret = from.Backpack.FindItemByType(s.Reagents[i], true);

                    // and check quantity
                    if(i < s.Quantity.Length)
                    {
                        if(ret == null || s.Quantity[i] > ret.Amount)
                        {
                            from.SendMessage("Need {1} {0} to perform {2}", s.Reagents[i].Name, s.Quantity[i], s.Name);
                            return false;
                        }
                    } else
                    {
                        from.SendMessage(33,"Error in quantity specification for {0}", s.Name);
                        return false;
                    }
                }
            }
            return true;
        }

		public override void OnWeaponHit(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven)
        {
            if(attacker == null || defender == null || weapon == null || m_SelectedAttack == null) return;

            if(!CheckRequirements(attacker,  m_SelectedAttack)) return;

            // take the requirements
            if(attacker.Backpack != null && m_SelectedAttack.Reagents != null && m_SelectedAttack.Quantity != null)
            {
                attacker.Backpack.ConsumeTotal( m_SelectedAttack.Reagents, m_SelectedAttack.Quantity, true);
            }

            attacker.Mana -= m_SelectedAttack.ManaReq;
            attacker.Stam -= m_SelectedAttack.StamReq;
            attacker.Hits -= m_SelectedAttack.HitsReq;
            attacker.Karma -= m_SelectedAttack.KarmaReq;

            // apply the attack
            DoSpecialAttack( attacker, defender, weapon, damageGiven, m_SelectedAttack);

            if(m_SelectedAttack.KarmaReq > 0)
            {
                attacker.SendMessage("and lose a little karma.");
            }

            // after applying a special attack activate the specials timer for combo chaining
            DoComboTimer(attacker, m_SelectedAttack.ChainTime);

            // check all combos to see which have this attack as the next in sequence, and which might be complete
            CheckCombos(attacker, defender, weapon, damageGiven, m_SelectedAttack);

            // clear the selected attack
            m_SelectedAttack = null;

            // redisplay the gump
            attacker.SendGump(new CustomAttacksGump(attacker, this));
        }

		public override void OnEquip( Mobile from )
		{
            // open the specials gump
            if(from == null || !from.Player) return;

            from.SendGump(new CustomAttacksGump(from, this));

		}
		
		public override void OnRemoved( object parent )
		{
            // open the specials gump
            if(parent != null && parent is Mobile && ((Mobile)parent).Player)
            {
                ((Mobile)parent).CloseGump(typeof( CustomAttacksGump ));
            }
		}

		public override string OnIdentify(Mobile from)
		{

            string msg = "Special Attacks:";

            foreach(SpecialAttack s in Specials)
            {
                msg += String.Format("\n{0}", s.Name);
            }

            if(Expiration > TimeSpan.Zero)
            {
                msg = String.Format("{0}\nexpires in {0} mins ", msg, Expiration.TotalMinutes);
            }

            return msg;
		}

		public override void OnAttach()
		{
            base.OnAttach();

            // only allow attachment to weapons and shields
            if(!(AttachedTo is BaseWeapon))
            {
                Delete();
            }
        }
		
		public void DoComboTimer(Mobile from, TimeSpan delay)
        {
            if ( m_ComboTimer != null )
                m_ComboTimer.Stop();
    
            m_ComboTimer = new ComboTimer(from, this, delay);

            m_ComboTimer.Start();
        }

        private class ComboTimer : Timer
		{
			private XmlCustomAttacks m_attachment;
			private Mobile m_from;

			public ComboTimer( Mobile from, XmlCustomAttacks a, TimeSpan delay ) : base( delay )
			{
				Priority = TimerPriority.OneSecond;
				m_attachment = a;
				m_from = from;
			}

			protected override void OnTick()
			{
                if(m_attachment == null || m_attachment.Deleted) return;

                // the combo has expired
                m_attachment.ResetCombos();

                // refresh the gump
                if(m_from != null)
                {
                    m_from.SendGump(new CustomAttacksGump(m_from, m_attachment));
                }
			}
		}
		
		private class CustomAttacksInfoGump : Gump
		{
            private XmlCustomAttacks m_attachment;
            private SpecialAttack m_special;

    		public CustomAttacksInfoGump( Mobile from, XmlCustomAttacks a, SpecialAttack s) : base( 0,0)
            {

                m_attachment = a;
                m_special = s;

                // prepare the page
                AddPage( 0 );

                AddBackground( 0, 0, 400, 300, 5054 );
                AddAlphaRegion( 0, 0, 400, 300 );
                AddLabel( 20, 2, 55, String.Format("{0}",s.Name) );

                StringBuilder text = new StringBuilder();

                text.AppendFormat("\n{0}", s.Description );

                
                text.AppendFormat("\n\nMinimum Stats/Skills:" );
                if(s.StrReq > 0)
                {
                    text.AppendFormat("\n     {0} Str", s.StrReq );
                }
                if(s.DexReq > 0)
                {
                    text.AppendFormat("\n     {0} Dex", s.DexReq );
                }
                if(s.IntReq > 0)
                {
                    text.AppendFormat("\n     {0} Int", s.IntReq );
                }

                if(s.Skills != null)
                for(int i = 0;i < s.Skills.Length;i++)
                {
                    if(i < s.MinSkillLevel.Length)
                    {
                        text.AppendFormat("\n     {1} {0}", s.Skills[i].ToString(), s.MinSkillLevel[i]  );
                    } else
                    {
                        text.AppendFormat("\n     {1} {0}", s.Skills[i].ToString(), "???"  );
                    }
                }

                text.AppendFormat("\n\nConsumes:" );

                // generate the text requirements
                if(s.ManaReq > 0)
                {
                    text.AppendFormat("\n     {0} Mana", s.ManaReq );
                }
                if(s.StamReq > 0)
                {
                    text.AppendFormat("\n     {0} Stamina", s.StamReq );
                }
                if(s.HitsReq > 0)
                {
                    text.AppendFormat("\n     {0} Hits", s.HitsReq );
                }
                if(s.KarmaReq > 0)
                {
                    text.AppendFormat("\n     {0} Karma", s.KarmaReq );
                }

                if(s.Reagents != null)
                for(int i = 0;i < s.Reagents.Length;i++)
                {
                    if(i < s.Quantity.Length)
                    {
                        text.AppendFormat("\n     {1} {0}", s.Reagents[i].Name, s.Quantity[i]  );
                    } else
                    {
                        text.AppendFormat("\n     {1} {0}", s.Reagents[i].Name, "???"  );
                    }
                }

                AddHtml( 20,20, 360, 260, text.ToString(), true , true );
            }
        }

		private class CustomAttacksGump : Gump
		{
            private XmlCustomAttacks m_attachment;
            private const int vertspacing = 47;

    		public CustomAttacksGump( Mobile from, XmlCustomAttacks a) : base( 0,0)
            {
                if(a == null)
                {
                    return;
                }
                if(from != null)
                    from.CloseGump(typeof( CustomAttacksGump));

                m_attachment = a;

                int specialcount = a.Specials.Count;

                // prepare the page
                AddPage( 0 );

                AddBackground( 0, 0, 70, 75 + specialcount*vertspacing, 5054 );
                AddLabel( 15, 2, 55, "Attack" );
                // if combos are still active then give it the green light
                if(m_attachment != null && m_attachment.HasActiveCombos)
                {
                    // green button
                    //AddImage( 20, 10, 0x2a4e );
                    AddImage( 15, 25, 0x0a53 );
                } else
                {
                    // red button
                    //AddImage( 20, 10, 0x2a62 );
                    AddImage( 15, 25, 0x0a52 );
                }
                // go through the list of enabled moves and add buttons for them
                int y = 70;
                for(int i = 0;i<specialcount;i++)
                {
                    SpecialAttack s = (SpecialAttack)m_attachment.Specials[i];

                    // flag the attack as being selected
                    // this puts a white background behind the selected attack.  Doesnt look as nice, but works in both the
                    // 2D and 3D client.  I prefer to leave this commented out for best appearance in the 2D client but
                    // feel free to uncomment it for best client compatibility.
                    /*
                    if(m_attachment != null && m_attachment.m_SelectedAttack != null && m_attachment.m_SelectedAttack == s)
                    {
                        AddImageTiled( 2, y-2, 66, vertspacing+2, 0xBBC );
                    }
                    */

                    // add the attack button

                    if(s.IconType == IconTypes.ItemID)
                    {
                        AddButton( 5, y, 0x5207, 0x5207, (int)s.AttackID + 1000, GumpButtonType.Reply, 0 );
                        AddImageTiled( 5, y, 44, 44, 0x283E );
                        AddItem(5, y, s.Icon);
                    } else
                    {
                        AddButton( 5, y, s.Icon, s.Icon, (int)s.AttackID + 1000, GumpButtonType.Reply, 0 );
                    }
                    
                    // flag the attack as being selected
                    // colors the attack icon red.  Looks better that the white background highlighting, but only supported by the 2D client.
                    if(m_attachment != null && m_attachment.m_SelectedAttack != null && m_attachment.m_SelectedAttack == s)
                    {
                        if(s.IconType == IconTypes.ItemID)
                        {
                            AddItem( 5, y, s.Icon, 33 );
                        } else
                        {
                            AddImage( 5, y, s.Icon, 33 );
                        }
                    }


                    // add the info button
                    AddButton( 52, y+13, 0x4b9, 0x4b9, 2000+(int)s.AttackID, GumpButtonType.Reply, 0 );

                    y += vertspacing;
                }

    		}
    		public override void OnResponse( NetState state, RelayInfo info )
            {

                if(m_attachment == null || state == null || state.Mobile == null || info == null) return;

                // go through all of the possible specials and find the matching button
                for(int i = 0;i<m_attachment.Specials.Count;i++)
                {
                    SpecialAttack s = (SpecialAttack)m_attachment.Specials[i];

                    if(s != null && info.ButtonID == (int)s.AttackID + 1000)
                    {
                        // if clicked again, then deselect
                        if(s == m_attachment.m_SelectedAttack)
                        {
                            m_attachment.m_SelectedAttack = null;
                        } else
                        {
                            // see whether they have the required resources for this attack
                            if(CheckRequirements(state.Mobile,  s))
                            {
                                // if so, then let them select it
                                m_attachment.m_SelectedAttack = s;
                            } else
                            {
                                // otherwise clear it
                                m_attachment.m_SelectedAttack = null;
                            }
                        }

                        state.Mobile.SendGump(new CustomAttacksGump(state.Mobile, m_attachment));
                        break;
                    }else
                    if(s != null && info.ButtonID == (int)s.AttackID + 2000)
                    {
                        state.Mobile.CloseGump(typeof(CustomAttacksInfoGump));
                        state.Mobile.SendGump(new CustomAttacksGump(state.Mobile, m_attachment));
                        state.Mobile.SendGump(new CustomAttacksInfoGump(state.Mobile, m_attachment, s));
                        break;
                    }
                }
            }
		}
    }
}
