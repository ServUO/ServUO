using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Necro
{
    public class SummonedPaladin : BaseCreature
    {
        private PlayerMobile m_Necromancer;
        private bool m_ToDelete;
        public SummonedPaladin(PlayerMobile necromancer)
            : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            this.m_Necromancer = necromancer;

            this.InitStats(45, 30, 5);
            this.Title = "the Paladin";

            this.Hue = 0x83F3;

            this.Female = false;
            this.Body = 0x190;
            this.Name = NameList.RandomName("male");

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, false);

            this.FacialHairHue = this.HairHue;

            this.AddItem(new Boots(0x1));
            this.AddItem(new ChainChest());
            this.AddItem(new ChainLegs());
            this.AddItem(new RingmailArms());
            this.AddItem(new PlateHelm());
            this.AddItem(new PlateGloves());
            this.AddItem(new PlateGorget());

            this.AddItem(new Cloak(0xCF));

            this.AddItem(new ThinLongsword());

            this.SetSkill(SkillName.Swords, 50.0);
            this.SetSkill(SkillName.Tactics, 50.0);

            this.PackGold(500);
        }

        public SummonedPaladin(Serial serial)
            : base(serial)
        {
        }

        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool PlayerRangeSensitive
        {
            get
            {
                return false;
            }
        }
        public static void BeginSummon(PlayerMobile player)
        {
            new SummonTimer(player).Start();
        }

        public override bool IsHarmfulCriminal(IDamageable target)
        {
            if (target is Mobile && (Mobile)target == this.m_Necromancer)
                return false;

            return base.IsHarmfulCriminal(target);
        }

        public override void OnThink()
        {
            if (!this.m_ToDelete && !this.Frozen)
            {
                if (this.m_Necromancer == null || this.m_Necromancer.Deleted || this.m_Necromancer.Map == Map.Internal)
                {
                    this.Delete();
                    return;
                }

                if (this.Combatant != this.m_Necromancer)
                    this.Combatant = this.m_Necromancer;

                if (!this.m_Necromancer.Alive)
                {
                    QuestSystem qs = this.m_Necromancer.Quest;

                    if (qs is DarkTidesQuest && qs.FindObjective(typeof(FindMardothEndObjective)) == null)
                        qs.AddObjective(new FindMardothEndObjective(false));

                    this.Say(1060139, this.m_Necromancer.Name); // You have made my work easy for me, ~1_NAME~.  My task here is done.

                    this.m_ToDelete = true;

                    Timer.DelayCall(TimeSpan.FromSeconds(5.0), new TimerCallback(Delete));
                }
                else if (this.m_Necromancer.Map != this.Map || this.GetDistanceToSqrt(this.m_Necromancer) > this.RangePerception + 1)
                {
                    Effects.SendLocationParticles(EffectItem.Create(this.Location, this.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(this.m_Necromancer.Location, this.m_Necromancer.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);

                    this.Map = this.m_Necromancer.Map;
                    this.Location = this.m_Necromancer.Location;

                    this.PlaySound(0x1FE);

                    this.Say(1060140); // You cannot escape me, knave of evil!
                }
            }

            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            QuestSystem qs = this.m_Necromancer.Quest;

            if (qs is DarkTidesQuest && qs.FindObjective(typeof(FindMardothEndObjective)) == null)
                qs.AddObjective(new FindMardothEndObjective(true));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)this.m_Necromancer);
            writer.Write((bool)this.m_ToDelete);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Necromancer = reader.ReadMobile() as PlayerMobile;
            this.m_ToDelete = reader.ReadBool();

            if (this.m_ToDelete)
                this.Delete();
        }

        private class SummonTimer : Timer
        {
            private readonly PlayerMobile m_Player;
            private SummonedPaladin m_Paladin;
            private int m_Step;
            public SummonTimer(PlayerMobile player)
                : base(TimeSpan.FromSeconds(4.0))
            {
                this.Priority = TimerPriority.FiftyMS;

                this.m_Player = player;
            }

            protected override void OnTick()
            {
                if (this.m_Player.Deleted)
                {
                    if (this.m_Step > 0)
                        this.m_Paladin.Delete();

                    return;
                }

                if (this.m_Step > 0 && this.m_Paladin.Deleted)
                    return;

                if (this.m_Step == 0)
                {
                    SummonedPaladinMoongate moongate = new SummonedPaladinMoongate();
                    moongate.MoveToWorld(new Point3D(2091, 1348, -90), Map.Malas);

                    Effects.PlaySound(moongate.Location, moongate.Map, 0x20E);

                    this.m_Paladin = new SummonedPaladin(this.m_Player);
                    this.m_Paladin.Frozen = true;

                    this.m_Paladin.Location = moongate.Location;
                    this.m_Paladin.Map = moongate.Map;

                    this.Delay = TimeSpan.FromSeconds(2.0);
                    this.Start();
                }
                else if (this.m_Step == 1)
                {
                    this.m_Paladin.Direction = this.m_Paladin.GetDirectionTo(this.m_Player);
                    this.m_Paladin.Say(1060122); // STOP WICKED ONE!

                    this.Delay = TimeSpan.FromSeconds(3.0);
                    this.Start();
                }
                else
                {
                    this.m_Paladin.Frozen = false;

                    this.m_Paladin.Say(1060123); // I will slay you before I allow you to complete your evil rites!

                    this.m_Paladin.Combatant = this.m_Player;
                }

                this.m_Step++;
            }
        }
    }

    public class SummonedPaladinMoongate : Item
    {
        public SummonedPaladinMoongate()
            : base(0xF6C)
        {
            this.Movable = false;
            this.Hue = 0x482;
            this.Light = LightType.Circle300;

            Timer.DelayCall(TimeSpan.FromSeconds(10.0), new TimerCallback(Delete));
        }

        public SummonedPaladinMoongate(Serial serial)
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

            this.Delete();
        }
    }
}