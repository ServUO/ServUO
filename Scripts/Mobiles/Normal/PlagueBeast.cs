using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a plague beast corpse")]
    public class PlagueBeast : BaseCreature, IDevourer
    {
        private int m_DevourTotal;
        private int m_DevourGoal;
        private bool m_HasMetalChest = false;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TotalDevoured
        {
            get
            {
                return this.m_DevourTotal;
            }
            set
            {
                this.m_DevourTotal = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int DevourGoal
        {
            get
            {
                return (this.IsParagon ? this.m_DevourGoal + 25 : this.m_DevourGoal);
            }
            set
            {
                this.m_DevourGoal = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasMetalChest
        {
            get
            {
                return this.m_HasMetalChest;
            }
        }

        [Constructable]
        public PlagueBeast()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            this.Name = "a plague beast";
            this.Body = 775;

            this.SetStr(302, 500);
            this.SetDex(80);
            this.SetInt(16, 20);

            this.SetHits(318, 404);

            this.SetDamage(20, 24);

            this.SetDamageType(ResistanceType.Physical, 60);
            this.SetDamageType(ResistanceType.Poison, 40);

            this.SetResistance(ResistanceType.Physical, 45, 55);
            this.SetResistance(ResistanceType.Fire, 40, 50);
            this.SetResistance(ResistanceType.Cold, 25, 35);
            this.SetResistance(ResistanceType.Poison, 65, 75);
            this.SetResistance(ResistanceType.Energy, 25, 35);

            this.SetSkill(SkillName.MagicResist, 35.0);
            this.SetSkill(SkillName.Tactics, 100.0);
            this.SetSkill(SkillName.Wrestling, 100.0);

            this.Fame = 13000;
            this.Karma = -13000;

            this.VirtualArmor = 30;
            if (Utility.RandomDouble() < 0.80)
                this.PackItem(new PlagueBeastGland());

            if (Core.ML && Utility.RandomDouble() < 0.33)
                this.PackItem(Engines.Plants.Seed.RandomPeculiarSeed(4));

            this.m_DevourTotal = 0;
            this.m_DevourGoal = Utility.RandomMinMax(15, 25); // How many corpses must be devoured before a metal chest is awarded
        }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Gems, Utility.Random(1, 3));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.ApplyPoison(this, this.IsParagon ? Poison.Lethal : Poison.Deadly);
            defender.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
            defender.PlaySound(0x1CB);
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            if (this.Map != null && caster != this && 0.25 > Utility.RandomDouble())
            {
                BaseCreature spawn = new PlagueSpawn(this);

                spawn.Team = this.Team;
                spawn.MoveToWorld(this.Location, this.Map);
                spawn.Combatant = caster;

                this.Say(1053034); // * The plague beast creates another beast from its flesh! *
            }

            base.OnDamagedBySpell(caster);
        }

        public override bool AutoDispel
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

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            if (this.Map != null && attacker != this && 0.25 > Utility.RandomDouble())
            {
                BaseCreature spawn = new PlagueSpawn(this);

                spawn.Team = this.Team;
                spawn.MoveToWorld(this.Location, this.Map);
                spawn.Combatant = attacker;

                this.Say(1053034); // * The plague beast creates another beast from its flesh! *
            }

            base.OnGotMeleeAttack(attacker);
        }

        public PlagueBeast(Serial serial)
            : base(serial)
        {
        }

        public override int GetIdleSound()
        {
            return 0x1BF;
        }

        public override int GetAttackSound()
        {
            return 0x1C0;
        }

        public override int GetHurtSound()
        {
            return 0x1C1;
        }

        public override int GetDeathSound()
        {
            return 0x1C2;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(this.m_HasMetalChest);
            writer.Write(this.m_DevourTotal);
            writer.Write(this.m_DevourGoal);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        this.m_HasMetalChest = reader.ReadBool();
                        this.m_DevourTotal = reader.ReadInt();
                        this.m_DevourGoal = reader.ReadInt();
                        break;
                    }
            }
        }

        public override void OnThink()
        {
            base.OnThink();

            // Check to see if we need to devour any corpses
            IPooledEnumerable eable = this.GetItemsInRange(3); // Get all corpses in range

            foreach (Item item in eable)
            {
                if (item is Corpse) // For each Corpse
                {
                    Corpse corpse = item as Corpse;

                    // Ensure that the corpse was killed by us
                    if (corpse != null && corpse.Killer == this && corpse.Owner != null)
                    {
                        if (!corpse.DevourCorpse() && !corpse.Devoured)
                            this.PublicOverheadMessage(MessageType.Emote, 0x3B2, 1053032); // * The plague beast attempts to absorb the remains, but cannot! *
                    }
                }
            }
            eable.Free();
        }

        #region IDevourer Members

        public bool Devour(Corpse corpse)
        {
            if (corpse == null || corpse.Owner == null) // sorry we can't devour because the corpse's owner is null
                return false;

            if (corpse.Owner.Body.IsHuman)
                corpse.TurnToBones(); // Not bones yet, and we are a human body therefore we turn to bones.

            this.IncreaseHits((int)Math.Ceiling((double)corpse.Owner.HitsMax * 0.75));
            this.m_DevourTotal++;

            this.PublicOverheadMessage(MessageType.Emote, 0x3B2, 1053033); // * The plague beast absorbs the fleshy remains of the corpse *

            if (!this.m_HasMetalChest && this.m_DevourTotal >= this.DevourGoal)
            {
                this.PackItem(new MetalChest());
                this.m_HasMetalChest = true;
            }

            return true;
        }

        #endregion

        private void IncreaseHits(int hp)
        {
            int maxhits = 2000;

            if (this.IsParagon)
                maxhits = (int)(maxhits * Paragon.HitsBuff);

            if (hp < 1000 && !Core.AOS)
                hp = (hp * 100) / 60;

            if (this.HitsMaxSeed >= maxhits)
            {
                this.HitsMaxSeed = maxhits;

                int newHits = this.Hits + hp + Utility.RandomMinMax(10, 20); // increase the hp until it hits if it goes over it'll max at 2000

                this.Hits = Math.Min(maxhits, newHits);
                // Also provide heal for each devour on top of the hp increase
            }
            else
            {
                int min = (hp / 2) + 10;
                int max = hp + 20;
                int hpToIncrease = Utility.RandomMinMax(min, max);

                this.HitsMaxSeed += hpToIncrease;
                this.Hits += hpToIncrease;
                // Also provide heal for each devour
            }
        }
    }
}