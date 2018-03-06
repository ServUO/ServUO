using System;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("a juka corpse")] // Why is this 'juka' and warriors 'jukan' ? :-(
    public class JukaMage : BaseCreature
    {
        private DateTime m_NextAbilityTime;
        [Constructable]
        public JukaMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a juka mage";
            this.Body = 765;

            this.SetStr(201, 300);
            this.SetDex(71, 90);
            this.SetInt(451, 500);

            this.SetHits(121, 180);

            this.SetDamage(4, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 20, 30);
            this.SetResistance(ResistanceType.Fire, 35, 45);
            this.SetResistance(ResistanceType.Cold, 30, 40);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 35, 45);

            this.SetSkill(SkillName.Anatomy, 80.1, 90.0);
            this.SetSkill(SkillName.EvalInt, 80.2, 100.0);
            this.SetSkill(SkillName.Magery, 99.1, 100.0);
            this.SetSkill(SkillName.Meditation, 80.2, 100.0);
            this.SetSkill(SkillName.MagicResist, 140.1, 150.0);
            this.SetSkill(SkillName.Tactics, 80.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 80.1, 90.0);

            this.Fame = 15000;
            this.Karma = -15000;

            this.VirtualArmor = 16;

            Container bag = new Bag();

            int count = Utility.RandomMinMax(10, 20);

            for (int i = 0; i < count; ++i)
            {
                Item item = Loot.RandomReagent();

                if (item == null)
                    continue;

                if (!bag.TryDropItem(this, item, false))
                    item.Delete();
            }

            this.PackItem(bag);

            this.PackItem(new ArcaneGem());

            if (Core.ML && Utility.RandomDouble() < .33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(4));

            this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
        }

        public JukaMage(Serial serial)
            : base(serial)
        {
        }

        public override bool AlwaysMurderer
        {
            get
            {
                return true;
            }
        }
        public override bool CanRummageCorpses
        {
            get
            {
                return true;
            }
        }
        public override int Meat
        {
            get
            {
                return 1;
            }
        }
        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.Average, 2);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override int GetIdleSound()
        {
            return 0x1AC;
        }

        public override int GetAngerSound()
        {
            return 0x1CD;
        }

        public override int GetHurtSound()
        {
            return 0x1D0;
        }

        public override int GetDeathSound()
        {
            return 0x28D;
        }

        public override void OnThink()
        {
            if (DateTime.UtcNow >= this.m_NextAbilityTime)
            {
                JukaLord toBuff = null;
                IPooledEnumerable eable = GetMobilesInRange(8);

                foreach (Mobile m in eable)
                {
                    if (m is JukaLord && this.IsFriend(m) && m.Combatant != null && this.CanBeBeneficial(m) && m.CanBeginAction(typeof(JukaMage)) && this.InLOS(m))
                    {
                        toBuff = (JukaLord)m;
                        break;
                    }
                }
                eable.Free();

                if (toBuff != null)
                {
                    if (this.CanBeBeneficial(toBuff) && toBuff.BeginAction(typeof(JukaMage)))
                    {
                        this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(30, 60));

                        toBuff.Say(true, "Give me the power to destroy my enemies!");
                        this.Say(true, "Fight well my lord!");

                        this.DoBeneficial(toBuff);

                        object[] state = new object[] { toBuff, toBuff.HitsMaxSeed, toBuff.RawStr, toBuff.RawDex };

                        SpellHelper.Turn(this, toBuff);

                        int toScale = toBuff.HitsMaxSeed;

                        if (toScale > 0)
                        {
                            toBuff.HitsMaxSeed += AOS.Scale(toScale, 75);
                            toBuff.Hits += AOS.Scale(toScale, 75);
                        }

                        toScale = toBuff.RawStr;

                        if (toScale > 0)
                            toBuff.RawStr += AOS.Scale(toScale, 50);

                        toScale = toBuff.RawDex;

                        if (toScale > 0)
                        {
                            toBuff.RawDex += AOS.Scale(toScale, 50);
                            toBuff.Stam += AOS.Scale(toScale, 50);
                        }

                        toBuff.Hits = toBuff.Hits;
                        toBuff.Stam = toBuff.Stam;

                        toBuff.FixedParticles(0x375A, 10, 15, 5017, EffectLayer.Waist);
                        toBuff.PlaySound(0x1EE);

                        Timer.DelayCall(TimeSpan.FromSeconds(20.0), new TimerStateCallback(Unbuff), state);
                    }
                }
                else
                {
                    this.m_NextAbilityTime = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(2, 5));
                }
            }

            base.OnThink();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private void Unbuff(object state)
        {
            object[] states = (object[])state;

            JukaLord toDebuff = (JukaLord)states[0];

            toDebuff.EndAction(typeof(JukaMage));

            if (toDebuff.Deleted)
                return;

            toDebuff.HitsMaxSeed = (int)states[1];
            toDebuff.RawStr = (int)states[2];
            toDebuff.RawDex = (int)states[3];

            toDebuff.Hits = toDebuff.Hits;
            toDebuff.Stam = toDebuff.Stam;
        }
    }
}