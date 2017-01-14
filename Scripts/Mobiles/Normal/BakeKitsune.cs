using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a bake kitsune corpse")]
    public class BakeKitsune : BaseCreature
    {
        [Constructable]
        public BakeKitsune()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a bake kitsune";
            this.Body = 246;

            this.SetStr(171, 220);
            this.SetDex(126, 145);
            this.SetInt(376, 425);

            this.SetHits(301, 350);

            this.SetDamage(15, 22);

            this.SetDamageType(ResistanceType.Physical, 70);
            this.SetDamageType(ResistanceType.Energy, 30);

            this.SetResistance(ResistanceType.Physical, 40, 60);
            this.SetResistance(ResistanceType.Fire, 70, 90);
            this.SetResistance(ResistanceType.Cold, 40, 60);
            this.SetResistance(ResistanceType.Poison, 40, 60);
            this.SetResistance(ResistanceType.Energy, 40, 60);

            this.SetSkill(SkillName.EvalInt, 80.1, 90.0);
            this.SetSkill(SkillName.Magery, 80.1, 90.0);
            this.SetSkill(SkillName.MagicResist, 80.1, 100.0);
            this.SetSkill(SkillName.Tactics, 70.1, 90.0);
            this.SetSkill(SkillName.Wrestling, 50.1, 55.0);

            this.Fame = 8000;
            this.Karma = -8000;

            this.Tamable = true;
            this.ControlSlots = 2;
            this.MinTameSkill = 80.7;

            if (Utility.RandomDouble() < .25)
                this.PackItem(Engines.Plants.Seed.RandomBonsaiSeed());
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Rich);
            this.AddLoot(LootPack.MedScrolls, 2);
        }

        public override int Meat
        {
            get
            {
                return 5;
            }
        }
        public override int Hides
        {
            get
            {
                return 30;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Regular;
            }
        }
        public override FoodType FavoriteFood
        {
            get
            {
                return FoodType.Fish;
            }
        }
        public override bool ShowFameTitle
        {
            get
            {
                return false;
            }
        }
        public override bool ClickTitle
        {
            get
            {
                return false;
            }
        }
        public override bool PropertyTitle
        {
            get
            {
                return false;
            }
        }

        public override void OnCombatantChange()
        {
            if (this.Combatant == null && !this.IsBodyMod && !this.Controlled && this.m_DisguiseTimer == null && Utility.RandomBool())
                this.m_DisguiseTimer = Timer.DelayCall(TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30)), new TimerCallback(Disguise));
        }

        public override bool OnBeforeDeath()
        {
            this.RemoveDisguise();

            return base.OnBeforeDeath();
        }

        #region Disguise
        private Timer m_DisguiseTimer;

        public void Disguise()
        {
            if (this.Combatant != null || this.IsBodyMod || this.Controlled)
                return;

            this.FixedEffect(0x376A, 8, 32);
            this.PlaySound(0x1FE);

            this.Female = Utility.RandomBool();

            if (this.Female)
            {
                this.BodyMod = 0x191;
                this.Name = NameList.RandomName("female");
            }
            else
            {
                this.BodyMod = 0x190;
                this.Name = NameList.RandomName("male");
            }

            this.Title = "the mystic llama herder";
            this.Hue = Race.Human.RandomSkinHue();
            this.HairItemID = Race.Human.RandomHair(this);
            this.HairHue = Race.Human.RandomHairHue();
            this.FacialHairItemID = Race.Human.RandomFacialHair(this);
            this.FacialHairHue = this.HairHue;

            switch ( Utility.Random(4) )
            {
                case 0:
                    this.AddItem(new Shoes(Utility.RandomNeutralHue()));
                    break;
                case 1:
                    this.AddItem(new Boots(Utility.RandomNeutralHue()));
                    break;
                case 2:
                    this.AddItem(new Sandals(Utility.RandomNeutralHue()));
                    break;
                case 3:
                    this.AddItem(new ThighBoots(Utility.RandomNeutralHue()));
                    break;
            }

            this.AddItem(new Robe(Utility.RandomNondyedHue()));

            this.m_DisguiseTimer = null;
            this.m_DisguiseTimer = Timer.DelayCall(TimeSpan.FromSeconds(75), new TimerCallback(RemoveDisguise));
        }

        public void RemoveDisguise()
        {
            if (!this.IsBodyMod)
                return;
			
            this.Name = "a bake kitsune";
            this.Title = null;
            this.BodyMod = 0;
            this.Hue = 0;
            this.HairItemID = 0;
            this.HairHue = 0;
            this.FacialHairItemID = 0;
            this.FacialHairHue = 0;

            this.DeleteItemOnLayer(Layer.OuterTorso);
            this.DeleteItemOnLayer(Layer.Shoes);

            this.m_DisguiseTimer = null;
        }

        public void DeleteItemOnLayer(Layer layer)
        {
            Item item = this.FindItemOnLayer(layer);

            if (item != null)
                item.Delete();
        }

        #endregion

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 > Utility.RandomDouble())
            {
                /* Blood Bath
                * Start cliloc 1070826
                * Sound: 0x52B
                * 2-3 blood spots
                * Damage: 2 hps per second for 5 seconds
                * End cliloc: 1070824
                */
                ExpireTimer timer = (ExpireTimer)m_Table[defender];

                if (timer != null)
                {
                    timer.DoExpire();
                    defender.SendLocalizedMessage(1070825); // The creature continues to rage!
                }
                else
                    defender.SendLocalizedMessage(1070826); // The creature goes into a rage, inflicting heavy damage!

                timer = new ExpireTimer(defender, this);
                timer.Start();
                m_Table[defender] = timer;
            }
        }

        private static readonly Hashtable m_Table = new Hashtable();
	
        private class ExpireTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_From;
            private int m_Count;

            public ExpireTimer(Mobile m, Mobile from)
                : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
            {
                this.m_Mobile = m;
                this.m_From = from;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            public void DoExpire()
            {
                this.Stop();
                m_Table.Remove(this.m_Mobile);
            }

            public void DrainLife()
            {
                if (this.m_Mobile.Alive)
                    this.m_Mobile.Damage(2, this.m_From);
                else
                    this.DoExpire();
            }

            protected override void OnTick()
            {
                this.DrainLife();

                if (++this.m_Count >= 5)
                {
                    this.DoExpire();
                    this.m_Mobile.SendLocalizedMessage(1070824); // The creature's rage subsides.
                }
            }
        }
		
        public override int GetAngerSound()
        {
            return 0x4DE;
        }

        public override int GetIdleSound()
        {
            return 0x4DD;
        }

        public override int GetAttackSound()
        {
            return 0x4DC;
        }

        public override int GetHurtSound()
        {
            return 0x4DF;
        }

        public override int GetDeathSound()
        {
            return 0x4DB;
        }

        public BakeKitsune(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && this.PhysicalResistance > 60)
            {
                this.SetResistance(ResistanceType.Physical, 40, 60);
                this.SetResistance(ResistanceType.Fire, 70, 90);
                this.SetResistance(ResistanceType.Cold, 40, 60);
                this.SetResistance(ResistanceType.Poison, 40, 60);
                this.SetResistance(ResistanceType.Energy, 40, 60);
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(RemoveDisguise));
        }
    }
}