using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a lady melisande corpse")]
    public class LadyMelisande : BasePeerless
    {
        public static void Initialize()
        {
            EventSink.PlayerDeath += new PlayerDeathEventHandler(delegate(PlayerDeathEventArgs e)
            {
                HandleDeath(e.Mobile);
            });
        }

        [Constructable]
        public LadyMelisande()
            : base(AIType.AI_NecroMage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a lady melisande";
            this.Body = 0x102;
            this.BaseSoundID = 451;

            this.SetStr(400, 1000);
            this.SetDex(300, 400);
            this.SetInt(1500, 1700);

            this.SetHits(100000);	

            this.SetDamage(11, 18);
			
            this.SetDamageType(ResistanceType.Physical, 50);
            this.SetDamageType(ResistanceType.Energy, 50);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 55, 65);
            this.SetResistance(ResistanceType.Poison, 70, 75);
            this.SetResistance(ResistanceType.Energy, 70, 80);
			
            this.SetSkill(SkillName.Wrestling, 100, 105);
            this.SetSkill(SkillName.Tactics, 100, 105);
            this.SetSkill(SkillName.MagicResist, 120);
            this.SetSkill(SkillName.Magery, 120);
            this.SetSkill(SkillName.EvalInt, 120);
            this.SetSkill(SkillName.Meditation, 120);
            this.SetSkill(SkillName.Necromancy, 120);
            this.SetSkill(SkillName.SpiritSpeak, 120);

            this.PackResources(8);
            this.PackTalismans(5);

            Timer.DelayCall(TimeSpan.FromSeconds(1), new TimerCallback(SpawnSatyrs));

            this.Fame = 25000;
            this.Karma = -25000;

            this.VirtualArmor = 50;

            for (int i = 0; i < Utility.RandomMinMax(0, 1); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.SuperBoss, 8);
            this.AddLoot(LootPack.Parrot, 1);
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            c.DropItem(new DiseasedBark());
            c.DropItem(new EternallyCorruptTree());

            int drop = Utility.Random(4, 8);

            for (int i = 0; i < drop; i++)
                c.DropItem(new MelisandesFermentedWine());

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.2225)
            {
                switch ( Utility.Random(3) )
                {
                    case 0:
                        c.DropItem(new MelisandesHairDye());
                        break;
                    case 1:
                        c.DropItem(new MelisandesCorrodedHatchet());
                        break;
                    case 2:
                        c.DropItem(new AlbinoSquirrelImprisonedInCrystal());
                        break;
                }
            }

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new MyrmidonLegs());

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HumanFeyLeggings());

            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());
        }
		
        public override void OnThink()
        {
            base.OnThink();

            Mobile combatant = Combatant as Mobile;

            if (combatant != null)
            {
                if (this.CanTakeLife(combatant))
                    this.TakeLife(combatant);

                if (this.CanSmackTalk())
                    this.SmackTalk();

                if (this.CanPutridNausea())
                    this.PutridNausea();
            }
        }

        public override void SetLocation(Point3D newLocation, bool isTeleport)
        {
            if (newLocation.Z > -10)
                base.SetLocation(newLocation, isTeleport);
        }
		
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (willKill)
            {
                this.SpawnHelper(new Reaper(), 6490, 948, 19);
                this.SpawnHelper(new InsaneDryad(), 6497, 946, 17);
                this.SpawnHelper(new StoneHarpy(), 6511, 946, 28); 	

                this.Say(1075118); // Noooooo!  You shall never defeat me.  Even if I should fall, my tree will sustain me and I will rise again.
            }
				
            base.OnDamage(amount, from, willKill);				
        }
	
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override Poison PoisonImmune
        {
            get
            {
                return Poison.Lethal;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }

        public LadyMelisande(Serial serial)
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
		
        #region Smack Talk
        private DateTime m_NextSmackTalk;

        public bool CanSmackTalk()
        {
            if (this.m_NextSmackTalk > DateTime.UtcNow)
                return false;

            if (this.Combatant == null)
                return false;

            return this.Hits > 0.5 * this.HitsMax;
        }

        public void SmackTalk()
        {
            this.Say(Utility.RandomMinMax(1075102, 1075115)); // Muahahahaha!  I'll feast on your flesh.

            this.m_NextSmackTalk = DateTime.UtcNow + TimeSpan.FromSeconds(2 + Utility.RandomDouble() * 3);	
        }

        #endregion
		
        #region Putrid Nausea
        private static readonly Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();
        private DateTime m_NextPutridNausea;

        public bool CanPutridNausea()
        {
            if (this.m_NextPutridNausea > DateTime.UtcNow)
                return false;

            return this.Combatant != null;
        }

        public void PutridNausea()
        {
            List<Mobile> list = new List<Mobile>();

            foreach (Mobile m in this.GetMobilesInRange(4))
            {
                if (this.CanBeHarmful(m) && m.Player)
                    list.Add(m);
            }

            for (int i = 0; i < list.Count; i++)
            {
                Mobile m = list[i];

                if (m_Table.ContainsKey(m))
                {
                    Timer timer = m_Table[m];

                    if (timer != null)
                        timer.Stop();

                    m_Table[m] = Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), new TimerStateCallback<Mobile>(EndPutridNausea), m);
                }
                else
                    m_Table.Add(m, Timer.DelayCall<Mobile>(TimeSpan.FromSeconds(30), new TimerStateCallback<Mobile>(EndPutridNausea), m));

                m.Animate(32, 5, 1, true, false, 0); // bow animation
                m.SendLocalizedMessage(1072068); // Your enemy's putrid presence envelops you, overwhelming you with nausea.
            }

            this.m_NextPutridNausea = DateTime.UtcNow + TimeSpan.FromSeconds(40 + Utility.RandomDouble() * 30);
        }

        public void EndPutridNausea(Mobile m)
        {
            m_Table.Remove(m);
        }

        public static void HandleDeath(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                Timer timer = m_Table[m];

                if (timer != null)
                    timer.Stop();

                m_Table.Remove(m);
            }
        }

        public static bool UnderPutridNausea(Mobile m)
        { 
            return m_Table.ContainsKey(m);
        }

        #endregion
		
        #region Take Life
        private DateTime m_NextTakeLife;

        public bool CanTakeLife(Mobile from)
        {
            if (this.m_NextTakeLife > DateTime.UtcNow)
                return false;

            if (!this.CanBeHarmful(from))
                return false;

            if (this.Hits > 0.1 * this.HitsMax || this.Hits < 0.025 * this.HitsMax)
                return false;

            return true;
        }
				
        public void TakeLife(Mobile from)
        {
            this.Hits += from.Hits / (from.Player ? 2 : 6);
			
            this.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
            this.PlaySound(0x1F2);
			
            this.Say(1075117);  // Muahahaha!  Your life essence is MINE!
            this.Say(1075120); // An unholy aura surrounds Lady Melisande as her wounds begin to close.

            this.m_NextTakeLife = DateTime.UtcNow + TimeSpan.FromSeconds(15 + Utility.RandomDouble() * 45);
        }

        #endregion

        #region Helpers
        public override bool CanSpawnHelpers
        {
            get
            {
                return true;
            }
        }
        public override int MaxHelpersWaves
        {
            get
            {
                return 1;
            }
        }

        public override void SpawnHelpers()
        {
            int count = 4;

            if (this.Altar != null)
            {
                count = Math.Min(this.Altar.Fighters.Count, 4);

                for (int i = 0; i < count; i++)
                {
                    Mobile fighter = this.Altar.Fighters[i];

                    if (this.CanBeHarmful(fighter))
                    {
                        EnslavedSatyr satyr = new EnslavedSatyr();
                        satyr.Combatant = fighter;
                        this.SpawnHelper(satyr, GetSpawnPosition(fighter.Location, fighter.Map, 2));

                        fighter.SendLocalizedMessage(1075116); // A twisted satyr scrambles onto the branch beside you and attacks!
                    }
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                    this.SpawnHelper(new EnslavedSatyr(), 4);
            }
        }

        public void SpawnSatyrs()
        {
            this.SpawnHelper(new EnslavedSatyr(), 6485, 945, 19);
            this.SpawnHelper(new EnslavedSatyr(), 6486, 948, 22);
            this.SpawnHelper(new EnslavedSatyr(), 6487, 945, 17); 
            this.SpawnHelper(new EnslavedSatyr(), 6488, 947, 23); 
        }
        #endregion
    }
}