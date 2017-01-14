using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a travesty's corpse")]
    public class Travesty : BasePeerless
    {
        [Constructable]
        public Travesty()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "Travesty";
            this.Body = 0x108;

            this.SetStr(909, 949);
            this.SetDex(901, 948);
            this.SetInt(903, 947);

            this.SetHits(35000);

            this.SetDamage(25, 30);
			
            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 52, 67);
            this.SetResistance(ResistanceType.Fire, 51, 68);
            this.SetResistance(ResistanceType.Cold, 51, 69);
            this.SetResistance(ResistanceType.Poison, 51, 70);
            this.SetResistance(ResistanceType.Energy, 50, 68);

            this.SetSkill(SkillName.Wrestling, 100.1, 119.7);
            this.SetSkill(SkillName.Tactics, 102.3, 118.5);
            this.SetSkill(SkillName.MagicResist, 101.2, 119.6);
            this.SetSkill(SkillName.Anatomy, 100.1, 117.5);

            this.Fame = 8000;
            this.Karma = -8000;

            this.VirtualArmor = 50;
            this.PackTalismans(5);
            this.PackResources(8);

            for (int i = 0; i < Utility.RandomMinMax(1, 6); i++)
            {
                this.PackItem(Loot.RandomScroll(0, Loot.ArcanistScrollTypes.Length, SpellbookType.Arcanist));
            }
        }

        public Travesty(Serial serial)
            : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
			
            c.DropItem(new EyeOfTheTravesty());
            c.DropItem(new OrdersFromMinax());

            switch ( Utility.Random(3) )
            {
                case 0:
                    c.DropItem(new TravestysSushiPreparations());
                    break;
                case 1:
                    c.DropItem(new TravestysFineTeakwoodTray());
                    break;
                case 2:
                    c.DropItem(new TravestysCollectionOfShells());
                    break;
            }

            if (Utility.RandomDouble() < 0.10)
                c.DropItem(new HumanFeyLeggings());

            if (Utility.RandomDouble() < 0.6)
                c.DropItem(new ParrotItem());

            if (Utility.RandomDouble() < 0.1)
                c.DropItem(new TragicRemainsOfTravesty());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new ImprisonedDog());

            if (Utility.RandomDouble() < 0.05)
                c.DropItem(new MarkOfTravesty());

            if (Utility.RandomDouble() < 0.025)
                c.DropItem(new CrimsonCincture());

            if (Utility.RandomDouble() < 0.025)
            {
                switch (Utility.Random(7))
                {
                    case 0:
                        c.DropItem(new AssassinLegs());
                        break;
                    case 1:
                        c.DropItem(new AssassinArms());
                        break;
                    case 2:
                        c.DropItem(new AssassinGloves());
                        break;
                    case 3:
                        c.DropItem(new MalekisHonor());
                        break;
                    case 4:
                        c.DropItem(new JusticeBreastplate());
                        break;
                    case 5:
                        c.DropItem(new CompassionArms());
                        break;
                    case 6:
                        c.DropItem(new ValorGauntlets());
                        break;
                }
            }
        }

        public override bool CanRummageCorpses
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
        public override bool CanAnimateDead
        {
            get
            {
                return true;
            }
        }
        public override BaseCreature Animates
        {
            get
            {
                return new LichLord();
            }
        }
        public override bool GivesMLMinorArtifact
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 5;
            }
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.AosSuperBoss, 8);
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

        private bool m_SpawnedHelpers;
        private Timer m_Timer;
        private string m_Name;
        private int m_Hue;

        public override void OnThink()
        {
            base.OnThink();

            if (this.Combatant == null)
                return;

            if (this.Combatant is Mobile && this.Name != this.Combatant.Name)
                this.Morph((Mobile)Combatant);
        }

        public virtual void Morph(Mobile combatant)
        {
            this.m_Name = this.Name;
            this.m_Hue = this.Hue;

            this.Body = combatant.Body;
            this.Hue = combatant.Hue;
            this.Name = combatant.Name;
            this.Female = combatant.Female;
            this.Title = combatant.Title;

            foreach (Item item in combatant.Items)
            {
                if (item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank)
                    if (this.FindItemOnLayer(item.Layer) == null)
                        this.AddItem(new ClonedItem(item));
            }

            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = Timer.DelayCall(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), new TimerCallback(EndMorph));
        }

        public void DeleteItems()
        {
            for (int i = this.Items.Count - 1; i >= 0; i --)
                if (this.Items[i] is ClonedItem)
                    this.Items[i].Delete();

            if (this.Backpack != null)
            {
                for (int i = this.Backpack.Items.Count - 1; i >= 0; i --)
                    if (this.Backpack.Items[i] is ClonedItem)
                        this.Backpack.Items[i].Delete();
            }
        }

        public virtual void EndMorph()
        {
            if (this.Combatant != null && this.Name == this.Combatant.Name)
                return;

            this.DeleteItems();

            if (this.m_Timer != null)
            {
                this.m_Timer.Stop();		
                this.m_Timer = null;	
            }

            if (this.Combatant is Mobile)
            {
                this.Morph((Mobile)Combatant);
                return;
            }

            this.Body = 264;
            this.Title = null;
            this.Name = this.m_Name;
            this.Hue = this.m_Hue;

            this.PlaySound(0x511);
            this.FixedParticles(0x376A, 1, 14, 5045, EffectLayer.Waist);
        }

        public override bool OnBeforeDeath()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            return base.OnBeforeDeath();
        }

        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            base.OnAfterDelete();
        }

        #region Spawn Helpers
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

        public override bool CanSpawnWave()
        {
            if (this.Hits > 1100)
                this.m_SpawnedHelpers = false;

            return !this.m_SpawnedHelpers && this.Hits < 1000;
        }

        public override void SpawnHelpers()
        {
            this.m_SpawnedHelpers = true;

            for (int i = 0; i < 10; i++)
            {
                switch ( Utility.Random(3) )
                {
                    case 0:
                        this.SpawnHelper(new DragonsFlameMage(), 25);
                        break;
                    case 1:
                        this.SpawnHelper(new SerpentsFangAssassin(), 25);
                        break;
                    case 2:
                        this.SpawnHelper(new TigersClawThief(), 25);
                        break;
                }
            }
        }

        #endregion

        private class ClonedItem : Item
        { 
            public ClonedItem(Item oItem)
                : base(oItem.ItemID)
            {
                this.Name = oItem.Name;
                this.Weight = oItem.Weight;
                this.Hue = oItem.Hue;
                this.Layer = oItem.Layer;
            }

            public override DeathMoveResult OnParentDeath(Mobile parent)
            {
                return DeathMoveResult.RemainEquiped;
            }

            public override DeathMoveResult OnInventoryDeath(Mobile parent)
            {
                this.Delete();
                return base.OnInventoryDeath(parent);
            }

            public ClonedItem(Serial serial)
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
        }
    }
}