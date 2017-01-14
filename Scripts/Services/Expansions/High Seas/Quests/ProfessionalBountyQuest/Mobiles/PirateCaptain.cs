using Server;
using System;
using Server.Items;
using Server.Multis;
using System.Collections.Generic;
using Server.Engines.Quests;

namespace Server.Mobiles
{
    public class PirateCaptain : BaseShipCaptain
    {
        public static List<string> m_RedNames;

        public static void Initialize()
        {
            m_RedNames = new List<string>();

            foreach (Mobile mob in World.Mobiles.Values)
            {
                if (mob != null && mob is PlayerMobile && mob.Kills >= 5)
                    m_RedNames.Add(mob.Name);
            }
        }

        private DateTime m_NextTalk;
        private int m_PirateName;
        private int m_Adjective;
        private int m_Noun;

        public int PirateName { get { return m_PirateName; } }
        public int Adjective { get { return m_Adjective; } }
        public int Noun { get { return m_Noun; } }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool Commandable { get { return false; } }

        #region Bounty Quest
        private ProfessionalBountyQuest m_Quest;
        private bool m_IsCaught;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsCaught { get { return m_IsCaught; } set { m_IsCaught = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public ProfessionalBountyQuest Quest { get { return m_Quest; } set { m_Quest = value; } }
        #endregion

        [Constructable]
        public PirateCaptain()
            : this(null)
        {
        }

        public PirateCaptain(BaseGalleon galleon)
            : base(galleon, AIType.AI_Melee, FightMode.Weakest, 25, 1, .2, .4)
        {
            PickRandomName();

            if(m_PirateName > 0 && (m_PirateName == 1116679 || m_PirateName == 1116680 || m_PirateName == 1116683))
                Female = true;

            SpeechHue = Utility.RandomDyedHue();
            Title = "the dread pirate";
            Hue = Race.RandomSkinHue();

            Body = Female ? 0x191 : 0x190;

            AddItem(new ShortPants(Utility.RandomNeutralHue()));

            SetStr(500, 750);
            SetDex(125, 175);
            SetInt(61, 75);

            SetHits(4500, 5000);

            SetDamage(23, 35);

            SetSkill(SkillName.Fencing, 115.0, 120.0);
            SetSkill(SkillName.Macing, 115.0, 120.0);
            SetSkill(SkillName.MagicResist, 115.0, 120.0);
            SetSkill(SkillName.Swords, 115.0, 120.0);
            SetSkill(SkillName.Tactics, 115.0, 120.0);
            SetSkill(SkillName.Wrestling, 115.0, 120.0);
            SetSkill(SkillName.Anatomy, 115.0, 120.0);

            Item hat;
            Item fancyShirt = new FancyShirt();
            Item shirt = new Shirt(GetRandomShirtHue());
            shirt.Layer = Layer.OuterTorso;

            if (Utility.RandomBool())
                hat = new Bandana();
            else
                hat = new TricorneHat();

            hat.Hue = Utility.RandomNeutralHue();

            AddItem(new Boots());
            AddItem(shirt);
            AddItem(fancyShirt);
            AddItem(hat);
            AddItem(new Cloak(Utility.RandomNeutralHue()));

            switch (Utility.Random(7))
            {
                case 0: AddItem(new Longsword()); break;
                case 1: AddItem(new Cutlass()); break;
                case 2: AddItem(new Broadsword()); break;
                case 5: AddItem(new Dagger()); break;
            }

            Utility.AssignRandomHair(this);

            Fame = 22000;
            Karma = -22000;
        }

        public static int GetRandomShirtHue()
        {
            return Utility.RandomMinMax(2498, 2644);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 2);
        }

        public void PickRandomName()
        {
            m_Adjective = Utility.RandomMinMax(1116631, 1116650);
            m_Noun = Utility.RandomMinMax(1116651, 1116670);

            if (m_RedNames.Count == 0 || 0.90 > Utility.RandomDouble())
                m_PirateName = Utility.RandomMinMax(1116671, 1116686);
            else
            {
                m_PirateName = -1;
                Name = GetRandomRedName();
            }
        }

        public string GetRandomRedName()
        {
            return m_RedNames[Utility.Random(m_RedNames.Count)];
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            string args;
            
            if(m_PirateName > 0)
                args = String.Format("#{0}\t#{1}\t#{2}", m_Adjective, m_Noun, m_PirateName);
            else
                args = String.Format("#{0}\t#{1}\t{2}", m_Adjective, m_Noun, Name);

            list.Add(1116690, args);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (!IsCaught || m_NextTalk > DateTime.UtcNow)
                return;

            IPooledEnumerable eable = this.GetMobilesInRange(7);
            foreach(Mobile mob in eable)
            {
                if (mob is PlayerMobile) 
                {
                    OnTalk();
                    break;
                }
            }
            eable.Free();
        }

        public void OnTalk()
        {
            Say(Utility.RandomMinMax(1149701, 1149720));
            m_NextTalk = DateTime.UtcNow + TimeSpan.FromMinutes(1);
        }

        #region Quest Stuff
        public bool TryBound(Mobile from, BaseQuest quest)
        {
            if (from == null || Galleon == null || !Galleon.Contains(this) || quest == null)
                return false;

            if (m_IsCaught)
            {
                from.SendMessage("That pirate is already bound to a ship!");
                return false;
            }

            Combatant = null;
            Warmode = false;
            m_IsCaught = true;
            return true;
        }

        public void OnBound(ProfessionalBountyQuest quest)
        {
            if (quest == null || quest.Pole == null)
                return;

            BindingPole pole = quest.Pole;

            int x = pole.X;
            int y = pole.Y;

            while (x == pole.X && y == pole.Y)
            {
                x = Utility.RandomMinMax(pole.X - 1, pole.X + 1);
                y = Utility.RandomMinMax(pole.Y - 1, pole.Y + 1);
            }

            Frozen = true;

            Item toDisarm = FindItemOnLayer(Layer.OneHanded);
            if (toDisarm == null || !toDisarm.Movable)
                toDisarm = FindItemOnLayer(Layer.TwoHanded);

            if (toDisarm != null)
            {
                if (Backpack != null)
                    Backpack.DropItem(toDisarm);
                else
                    toDisarm.Delete();
            }

            m_Quest = quest;

            if (quest != null && quest.Galleon != null)
                quest.Galleon.CapturedCaptain = this;

            Timer.DelayCall(TimeSpan.FromSeconds(2.5), new TimerStateCallback(MoveCaptainToShip), new object[] { x, y, pole });
        }

        private void MoveCaptainToShip(object obj)
        {
            object[] objs = (object[])obj;
            int x = (int)objs[0];
            int y = (int)objs[1];
            Item pole = objs[2] as Item;

            if (pole != null)
                MoveToWorld(new Point3D(x, y, pole.Z), pole.Map);

            Blessed = true;
            Title = "[Captured Captain]";
        }

        public override bool OnBeforeDeath()
        {
            List<PlayerMobile> hasQuest = new List<PlayerMobile>();
            List<DamageStore> rights = GetLootingRights();
            for (int i = 0; i < rights.Count; i++)
            {
                if (!rights[i].m_HasRight)
                    continue;

                Mobile mob = rights[i].m_Mobile;

                //if they have the quest and looting rights, give them a certificate
                if (mob is PlayerMobile && mob.NetState != null && QuestHelper.GetQuest((PlayerMobile)mob, typeof(ProfessionalBountyQuest)) != null)
                    hasQuest.Add((PlayerMobile)mob);
            }

            if (hasQuest.Count > 0)
            {
                PlayerMobile questee = hasQuest[Utility.Random(hasQuest.Count)];
                BaseQuest q = QuestHelper.GetQuest(questee, typeof(ProfessionalBountyQuest));

                if (q != null && q is ProfessionalBountyQuest)
                {
                    ((ProfessionalBountyQuest)q).OnPirateDeath(this);
                    questee.AddToBackpack(new DeathCertificate(this));
                }
            }

            if (m_IsCaught && m_Quest != null)
            {
                for (int i = 0; i < m_Quest.Objectives.Count; i++)
                {
                    if (m_Quest.Objectives[i] is BountyQuestObjective && ((BountyQuestObjective)m_Quest.Objectives[i]).CapturedCaptain == this)
                    {
                        ((BountyQuestObjective)m_Quest.Objectives[i]).CapturedCaptain = null;
                        ((BountyQuestObjective)m_Quest.Objectives[i]).Captured = false;
                        m_Quest = null;
                    }
                }
            }

            return base.OnBeforeDeath();
        }
        #endregion

        public PirateCaptain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_IsCaught);
            writer.Write(m_Adjective);
            writer.Write(m_Noun);
            writer.Write(m_PirateName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_IsCaught = reader.ReadBool();
            m_Adjective = reader.ReadInt();
            m_Noun = reader.ReadInt();
            m_PirateName = reader.ReadInt();

            if (IsCaught)
                Frozen = true;

            m_NextTalk = DateTime.UtcNow;
        }
    }
}