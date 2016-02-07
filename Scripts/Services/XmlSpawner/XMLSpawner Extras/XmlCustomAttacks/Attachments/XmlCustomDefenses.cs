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
** XmlCustomDefenses
** 11/26/04
** ArteGordon
**
** This attachment will allow you to create a system for adding special defenses to shields including combo defenses that require
** a series of specific special defensive moves to be executed in a timed sequence.
** This is the defensive counterpart to XmlCustomAttacks.
**
*/
namespace Server.Engines.XmlSpawner2
{
    public class XmlCustomDefenses : XmlAttachment
    {

        // ------------------------------------------------------------------------------
        // BEGINNING of user-defined special defenses and combos information
        // ------------------------------------------------------------------------------

        //
        // define the Combo and special defense enums
        //
        // you must first add entries here if you wish to add new defenses
        //

        // DEFENSES
        public enum ComboDefenses
        {
            ColdWind
        }
        
        public enum SpecialDefenses
        {
            MindDrain,
            StamDrain,
            ParalyzingFear,
            GiftOfHealth,
            SpikeShield,
            PuffOfSmoke
        }

        public static new void Initialize()
        {
            //
            // define the special defenses and their use requirements
            //
            // ideally, you have a definition for every SpecialDefenses enum.  Although it isnt absolutely necessary,
            // if it isnt defined here, it will not be available for use

            AddSpecialDefense("Shield of Spikes", "Returns damage to an attacker",
                SpecialDefenses.SpikeShield, 0x2086, IconTypes.ItemID,TimeSpan.FromSeconds(10.5), // example of using an itemid for the gump icon
                0, 20, 0, 0,
                30,30,0,
                null,
                null,
                new Type [] {typeof(PigIron)},
                new int [] { 3 }
            );
            
            AddSpecialDefense("Mind Drain", "Drains mana from the target",
                SpecialDefenses.MindDrain, 0x5007, IconTypes.GumpID, TimeSpan.FromSeconds(10.5),  // explicitly specifying the gump icon as a gumpid
                10, 5, 5, 0,
                0,0,40,
                new SkillName [] { SkillName.Magery },
                new int [] { 50 },
                null,
                null
            );
            
            AddSpecialDefense("Stam Drain", "Drains stamina from the target",
                SpecialDefenses.StamDrain, 0x500e, TimeSpan.FromSeconds(10.5),                // if the icon type is not specified, gump icons use gumpids
                30, 5, 0, 0,
                40, 40, 0,
                null,
                null,
                new Type [] {typeof(Ginseng), typeof(Garlic)},
                new int [] { 1, 2 }
            );

            AddSpecialDefense("Paralyzing Fear", "Paralyzes the target and causes it to flee",
                SpecialDefenses.ParalyzingFear, 0x500d, TimeSpan.FromSeconds(10.5),
                10, 10, 5, 10,
                0, 0, 40,
                new SkillName [] { SkillName.Necromancy },
                new int [] { 30 },
                new Type [] {typeof(Head)},
                new int [] { 1 }
            );

            AddSpecialDefense("Gift of Health", "Absorbs damage as health",
                SpecialDefenses.GiftOfHealth, 0x500c, TimeSpan.FromSeconds(10.5),
                40, 30, 0, 0,
                0, 0, 30,
                null,
                null,
                new Type [] {typeof(Ginseng), typeof(MandrakeRoot)},
                new int [] { 4, 4 }
            );

            AddSpecialDefense("Puff of Smoke", "Makes the defender invisible",
                SpecialDefenses.PuffOfSmoke, 0x520b, TimeSpan.FromSeconds(10.5),
                20, 40, 0, 0,
                0, 40, 0,
                new SkillName [] { SkillName.Stealth, SkillName.Hiding },
                new int [] { 50, 50 },
                new Type [] {typeof(SpidersSilk)},
                new int [] { 2 }
            );
            
            //
            // define combos and the sequence of special defenses needed to activate them
            //
            AddComboDefense( "Cold Wind", ComboDefenses.ColdWind,
                new SpecialDefenses []
                    {
                    SpecialDefenses.SpikeShield,
                    SpecialDefenses.SpikeShield,
                    SpecialDefenses.StamDrain
                    }
            );


            
            // after deser, restore combo and specials lists to all existing CustomDefenses attachments based on these definitions
            foreach(XmlAttachment x in XmlAttach.AllAttachments.Values)
            {
                if(x is XmlCustomDefenses)
                {
                    ((XmlCustomDefenses)x).FillComboList();
                    ((XmlCustomDefenses)x).FillSpecialsList();
                }
            }
        }

        //
        // carry out the special defenses
        //
        // If you add a new defense, you must add the code here to define what it actually does when it hits
        // can optionally return a value that will be used to reduce damage
        //
        public int DoSpecialDefense(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, SpecialDefense special)
		{
		
		    if(attacker == null || defender == null || weapon == null || special == null) return 0;

            defender.SendMessage("you defend with {0}!", special.Name);

            // apply the special defense
            switch(special.DefenseID)
            {

                case SpecialDefenses.MindDrain:
                {
                    attacker.Mana -= damageGiven;
                    defender.FixedEffect( 0x375A, 10, 15 );
                    // absorb all of the damage you would have taken
                    return damageGiven;
                }
                case SpecialDefenses.StamDrain:
                {
                    attacker.Stam -= damageGiven;
                    defender.FixedEffect( 0x374A, 10, 15 );
                    // absorb all of the damage you would have taken
                    return damageGiven;
                }
                case SpecialDefenses.SpikeShield:
                {
                    // return the damage to attacker
                    attacker.Damage( damageGiven, defender );
                    defender.SendMessage("{0} damage reflected!", damageGiven);
                    // absorb all of the damage you would have taken
                    return damageGiven;
                }
                case SpecialDefenses.PuffOfSmoke:
                {
                    defender.Hidden = true;
                    break;
                }
                case SpecialDefenses.GiftOfHealth:
                {
                    defender.FixedEffect( 0x376A, 9, 32 );
                    defender.PlaySound( 0x202 );
                    defender.Hits += damageGiven;
                    defender.SendMessage("healed {0}!", damageGiven);
                    // absorb all of the damage you would have taken
                    return damageGiven;
                }
                case SpecialDefenses.ParalyzingFear:
                {
                    // lose target focus
                    attacker.Combatant = null;
                    // flee
                    if(attacker is BaseCreature)
                    {
                    	((BaseCreature)attacker).BeginFlee(TimeSpan.FromSeconds(6));
                    }
                    // and become paralyzed
                    attacker.Freeze( TimeSpan.FromSeconds(damageGiven/10) );
			        attacker.FixedEffect( 0x376A, 9, 32 );
			        attacker.PlaySound( 0x204 );
                    break;
                }
                default:
                    defender.SendMessage("no effect");
                    break;
            }
            return 0;
        }

		//
        // carry out the combo defenses
        //
        // If you add a new combo, you must add the code here to define what it actually does when it is activated
        //
        public void DoComboDefense(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, ComboDefense combo)
		{
		    if(attacker == null || defender == null || weapon == null || combo == null) return;

		    defender.SendMessage("You unleash the combo defense {0}!",combo.Name);

		    // apply the combo defense
            switch(combo.DefenseID)
            {
                case ComboDefenses.ColdWind:
                {
                    // 5 sec paralyze
                    attacker.FixedEffect( 0x376A, 9, 32 );
			        attacker.PlaySound( 0x204 );
                    attacker.Freeze( TimeSpan.FromSeconds(5) );
                    // 7x stam drain
                    attacker.Stam -= weapon.MaxDamage*7;
                    break;
                }
            }
		}
		
		[Attachable]
        public XmlCustomDefenses(string name, int nspecials)
        {

            FillComboList();

            //
            // you can put any named specials configurations that you want in here
            //
            if(String.Compare("brogan", name, true) == 0)
            {
                AddSpecial(SpecialDefenses.SpikeShield);
                AddSpecial(SpecialDefenses.MindDrain);
                AddSpecial(SpecialDefenses.StamDrain);
                AddSpecial(SpecialDefenses.ParalyzingFear);
            } else
            if(String.Compare("random", name, true) == 0)
            {
                // assign the requested number of random special defenses
                SetRandomSpecials(nspecials);
            }
        }


        // this constructor is intended to be called from within scripts that wish to define custom defense configurations
        // by passing it a list of SpecialDefenses
        public XmlCustomDefenses(SpecialDefenses [] defenselist)
        {
            if(defenselist != null)
            {
                foreach(SpecialDefenses sid in defenselist)
                {
                    AddSpecial(sid);
                }
            }
        }
        
        public XmlCustomDefenses(SpecialDefenses defense)
        {
            AddSpecial(defense);
        }

        [Attachable]
        public XmlCustomDefenses(string name) : this(name, 1)
        {
        }
        
        [Attachable]
        public XmlCustomDefenses() : this("random", 1)
        {
        }

		// ------------------------------------------------------------------------------
        // END of user-defined special defenses and combos information
        // ------------------------------------------------------------------------------

        private static Hashtable AllSpecials = new Hashtable();
        private static Hashtable AllCombos = new Hashtable();
        
        public enum IconTypes
        {
            GumpID,
            ItemID
        }

        public class SpecialDefense
        {
            public string      Name;           // attack name
            public string      Description;    // attack description
            public SpecialDefenses        DefenseID;  // defense id
            public TimeSpan    ChainTime;    // time available until next defense in the chain must be performed
            public int Icon;                 // button icon for this defense
            public IconTypes IconType;          // what type of art to use for button icon
            public int ManaReq;             // mana usage for this defense
            public int StamReq;             // stamina usage for this defense
            public int HitsReq;             // hits usage for this defense
            public int KarmaReq;            // karma usage for this defense
            public int StrReq;             // str requirements for this defense
            public int DexReq;             // dex requirements for this defense
            public int IntReq;             // int requirements for this defense
            public Type [] Reagents;       // reagent list used for this defense
            public int [] Quantity;        // reagent quantity list
            public SkillName [] Skills;    // list of skill requirements for this defense
            public int [] MinSkillLevel;   // minimum skill levels

            public SpecialDefense( string name, string description, SpecialDefenses id, int icon, IconTypes itype, TimeSpan duration,
            int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
            SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
            {
                Name = name;
                Description = description;
                DefenseID = id;
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

        public class ComboDefense
        {
            public string Name;
            public ComboDefenses DefenseID;
            public SpecialDefenses [] DefenseSequence;

            public ComboDefense( string name, ComboDefenses id, SpecialDefenses [] sequence)
            {
                Name = name;
                DefenseID = id;
                DefenseSequence = sequence;
            }
        }

        public class ActiveCombo
        {
            public ComboDefense Combo;
            public int PositionInSequence;

            public ActiveCombo(ComboDefense c)
            {
                Combo = c;
                PositionInSequence  = 0;
            }
        }

        private ComboTimer m_ComboTimer;
        public SpecialDefense m_SelectedDefense;

        // these are the lists of special moves and combo status for each instance
        private ArrayList Specials = new ArrayList();
        private ArrayList Combos = new ArrayList();

        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlCustomDefenses(ASerial serial) : base(serial)
        {
        }

        public override void Serialize( GenericWriter writer )
		{
            base.Serialize(writer);

            writer.Write( (int) 0 );

            // version 0
            // save the specials for this instance
            writer.Write( Specials.Count);

            foreach(SpecialDefense s in Specials)
            {
                writer.Write(s.DefenseID.ToString());
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
                    SpecialDefenses stype = (SpecialDefenses)Enum.Parse(typeof(SpecialDefenses), sname);
                    tmpSpecialsList.Add(stype);
                    } catch{}
                }
                break;
            }
		}


		private static void AddSpecialDefense( string name, string description, SpecialDefenses id, int icon, TimeSpan duration,
        int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
        SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
        {
            AddSpecialDefense( name, description, id, icon, IconTypes.GumpID, duration,
            mana, stam, hits, karma, minstr, mindex, minint,
            skills, minlevel, reagents, quantity);
        }
		
		private static void AddSpecialDefense( string name, string description, SpecialDefenses id, int icon, IconTypes itype, TimeSpan duration,
        int mana, int stam, int hits, int karma, int minstr, int mindex, int minint,
        SkillName [] skills, int [] minlevel, Type [] reagents, int [] quantity)
        {
            AllSpecials.Add(id, new SpecialDefense( name, description, id, icon, itype,
            duration, mana, stam, hits, karma,
            minstr, mindex, minint, skills, minlevel, reagents, quantity) );
        }

        private static SpecialDefense GetSpecialDefense( SpecialDefenses id )
        {
            return((SpecialDefense)AllSpecials[id]);
        }

        private void AddSpecial( SpecialDefenses id )
        {
            SpecialDefense s = GetSpecialDefense(id);

            if(s != null)
                Specials.Add(s);
        }

        private static void AddComboDefense( string name, ComboDefenses id, SpecialDefenses [] sequence)
        {
            AllCombos.Add(id, new ComboDefense( name, id,sequence) );
        }

        private static ComboDefense GetComboDefense( ComboDefenses id )
        {
            return((ComboDefense)AllCombos[id]);
        }
        
        public static void AddDefense(object target, SpecialDefenses defense)
        {
            // is there an existing custom attacks attachment to add to?
            XmlCustomDefenses a = (XmlCustomDefenses)XmlAttach.FindAttachment(target, typeof(XmlCustomDefenses));

            if(a == null)
            {
                // add a new custom attacks attachment
                XmlAttach.AttachTo(target, new XmlCustomDefenses(defense));
            } else
            {
                // add the new attack to existing attack list
                a.AddSpecial(defense);
            }
        }

        private void FillComboList()
        {
            Combos = new ArrayList();

            foreach(ComboDefense c in AllCombos.Values)
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

            foreach(SpecialDefenses sid in tmpSpecialsList)
            {
                SpecialDefense s = GetSpecialDefense(sid);
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

                    SpecialDefense s = GetSpecialDefense((SpecialDefenses)rand);
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

		private void CheckCombos(Mobile attacker, Mobile defender, BaseWeapon weapon, int damageGiven, SpecialDefense s)
		{
            if(s == null) return;

            foreach(ActiveCombo c in Combos)
            {
                if(c != null && c.Combo != null && c.Combo.DefenseSequence != null && c.PositionInSequence < c.Combo.DefenseSequence.Length)
                {
                    if(c.Combo.DefenseSequence[c.PositionInSequence] == s.DefenseID)
                    {
                        if(++c.PositionInSequence >= c.Combo.DefenseSequence.Length)
                        {
                            // combo is complete so execute it
                            DoComboDefense(attacker, defender, weapon, damageGiven, c.Combo);

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

		public static bool CheckRequirements(Mobile from, SpecialDefense s)
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
                // clear the selected defense
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

        public override int OnArmorHit(Mobile attacker, Mobile defender, Item armor, BaseWeapon weapon, int damageGiven)
        {
            if(attacker == null || defender == null || weapon == null || armor == null || m_SelectedDefense == null) return 0;

            if(!CheckRequirements(defender,  m_SelectedDefense)) return 0;

            // take the requirements
            if(defender.Backpack != null && m_SelectedDefense.Reagents != null && m_SelectedDefense.Quantity != null)
            {
                defender.Backpack.ConsumeTotal( m_SelectedDefense.Reagents, m_SelectedDefense.Quantity, true);
            }

            defender.Mana -= m_SelectedDefense.ManaReq;
            defender.Stam -= m_SelectedDefense.StamReq;
            defender.Hits -= m_SelectedDefense.HitsReq;
            defender.Karma -= m_SelectedDefense.KarmaReq;

            // apply the attack
            int damage = DoSpecialDefense( attacker, defender, weapon, damageGiven, m_SelectedDefense);

            if(m_SelectedDefense.KarmaReq > 0)
            {
                defender.SendMessage("and lose a little karma.");
            }

            // after applying a special defense activate the specials timer for combo chaining
            DoComboTimer(defender, m_SelectedDefense.ChainTime);

            // check all combos to see which have this defense as the next in sequence, and which might be complete
            CheckCombos( attacker, defender, weapon, damageGiven, m_SelectedDefense);

            // clear the selected defense
            m_SelectedDefense = null;

            // redisplay the gump
            defender.SendGump(new CustomDefenseGump(defender, this));

            return damage;
        }

		public override void OnEquip( Mobile from )
		{
            // open the specials gump
            if(from == null || !from.Player) return;

            from.SendGump(new CustomDefenseGump(from, this));
		}
		
		public override void OnRemoved( object parent )
		{
            // open the specials gump
            if(parent != null && parent is Mobile && ((Mobile)parent).Player)
            {
                ((Mobile)parent).CloseGump(typeof( CustomDefenseGump ));
            }
		}

		public override string OnIdentify(Mobile from)
		{

            string msg = "Special Defenses:";

            foreach(SpecialDefense s in Specials)
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

            // only allow attachment to shields (for now)
            if(!(AttachedTo is BaseShield))
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
			private XmlCustomDefenses m_attachment;
			private Mobile m_from;

			public ComboTimer( Mobile from, XmlCustomDefenses a, TimeSpan delay ) : base( delay )
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
                    m_from.SendGump(new CustomDefenseGump(m_from, m_attachment));
                }
			}
		}
		
		private class CustomDefenseInfoGump : Gump
		{
            private XmlCustomDefenses m_attachment;
            private SpecialDefense m_special;

    		public CustomDefenseInfoGump( Mobile from, XmlCustomDefenses a, SpecialDefense s) : base( 0,0)
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

		private class CustomDefenseGump : Gump
		{
            private XmlCustomDefenses m_attachment;
            private const int vertspacing = 47;

    		public CustomDefenseGump( Mobile from, XmlCustomDefenses a) : base( 0,0)
            {
                if(a == null)
                {
                    return;
                }
                if(from != null)
                    from.CloseGump(typeof( CustomDefenseGump));

                m_attachment = a;

                int specialcount = a.Specials.Count;

                // prepare the page
                AddPage( 0 );

                AddBackground( 0, 0, 70, 75 + specialcount*vertspacing, 5054 );
                AddLabel( 13, 2, 55, "Defense" );
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
                    SpecialDefense s = (SpecialDefense)m_attachment.Specials[i];

                    // flag the defense as being selected
                    // this puts a white background behind the selected defense.  Doesnt look as nice, but works in both the
                    // 2D and 3D client.  I prefer to leave this commented out for best appearance in the 2D client but
                    // feel free to uncomment it for best client compatibility.
                    /*
                    if(m_attachment != null && m_attachment.m_SelectedDefense != null && m_attachment.m_SelectedDefense == s)
                    {
                        AddImageTiled( 2, y-2, 66, vertspacing+2, 0xBBC );
                    }
                    */

                    // add the defense button

                    if(s.IconType == IconTypes.ItemID)
                    {
                        AddButton( 5, y, 0x5207, 0x5207, (int)s.DefenseID + 1000, GumpButtonType.Reply, 0 );
                        AddImageTiled( 5, y, 44, 44, 0x283E );
                        AddItem(5, y, s.Icon);
                    } else
                    {
                        AddButton( 5, y, s.Icon, s.Icon, (int)s.DefenseID + 1000, GumpButtonType.Reply, 0 );
                    }
                    
                    // flag the defense as being selected
                    // colors the defense icon red.  Looks better that the white background highlighting, but only supported by the 2D client.
                    if(m_attachment != null && m_attachment.m_SelectedDefense != null && m_attachment.m_SelectedDefense == s)
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
                    AddButton( 52, y+13, 0x4b9, 0x4b9, 2000+(int)s.DefenseID, GumpButtonType.Reply, 0 );

                    y += vertspacing;
                }

    		}
    		public override void OnResponse( NetState state, RelayInfo info )
            {

                if(m_attachment == null || state == null || state.Mobile == null || info == null) return;

                // go through all of the possible specials and find the matching button
                for(int i = 0;i<m_attachment.Specials.Count;i++)
                {
                    SpecialDefense s = (SpecialDefense)m_attachment.Specials[i];

                    if(s != null && info.ButtonID == (int)s.DefenseID + 1000)
                    {
                        // if clicked again, then deselect
                        if(s == m_attachment.m_SelectedDefense)
                        {
                            m_attachment.m_SelectedDefense = null;
                        } else
                        {
                            // see whether they have the required resources for this defense
                            if(CheckRequirements(state.Mobile,  s))
                            {
                                // if so, then let them select it
                                m_attachment.m_SelectedDefense = s;
                            } else
                            {
                                // otherwise clear it
                                m_attachment.m_SelectedDefense = null;
                            }
                        }

                        state.Mobile.SendGump(new CustomDefenseGump(state.Mobile, m_attachment));
                        break;
                    }else
                    if(s != null && info.ButtonID == (int)s.DefenseID + 2000)
                    {
                        state.Mobile.CloseGump(typeof(CustomDefenseInfoGump));
                        state.Mobile.SendGump(new CustomDefenseGump(state.Mobile, m_attachment));
                        state.Mobile.SendGump(new CustomDefenseInfoGump(state.Mobile, m_attachment, s));
                        break;
                    }
                }
            }
		}
    }
}
