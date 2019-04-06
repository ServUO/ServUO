using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Network;

namespace Server.Mobiles 
{ 
    public class DemonicJailor : BaseCreature 
    { 
        [Constructable] 
        public DemonicJailor()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.3)
        {
            this.Name = NameList.RandomName("male");
            this.SpeechHue = Utility.RandomDyedHue(); 
            this.Title = "the demonic jailor";
            this.Hue = 34531;
            this.Body = 0x190;             

            this.SetStr(386, 400);
            this.SetDex(151, 165);
            this.SetInt(161, 175);

            this.SetDamage(8, 10);

            this.SetDamageType(ResistanceType.Physical, 100);

            this.SetResistance(ResistanceType.Physical, 35, 45);
            this.SetResistance(ResistanceType.Fire, 25, 30);
            this.SetResistance(ResistanceType.Cold, 25, 30);
            this.SetResistance(ResistanceType.Poison, 10, 20);
            this.SetResistance(ResistanceType.Energy, 10, 20);

            this.SetSkill(SkillName.Anatomy, 125.0);
            this.SetSkill(SkillName.Fencing, 46.0, 77.5);
            this.SetSkill(SkillName.Macing, 35.0, 57.5);
            this.SetSkill(SkillName.Poisoning, 60.0, 82.5);
            this.SetSkill(SkillName.DetectHidden, 100);
            this.SetSkill(SkillName.MagicResist, 83.5, 92.5);
            this.SetSkill(SkillName.Swords, 125.0);
            this.SetSkill(SkillName.Tactics, 125.0);
            this.SetSkill(SkillName.Lumberjacking, 125.0);

            this.Fame = 5000;
            this.Karma = -5000;

            this.VirtualArmor = 40;

            this.SetWearable(new ShortPants(Utility.RandomRedHue()));
            this.AddItem(new Sandals(Utility.RandomRedHue())); 
            this.AddItem(new Shirt(Utility.RandomRedHue()));
            this.AddItem(new SkinningKnife());

            Utility.AssignRandomHair(this);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
        
            Point3D loc = new Point3D(5703, 639, 0);
            Map map = this.Map;

            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
            Effects.PlaySound(loc, map, 0x1FE);

            BaseCreature.TeleportPets(defender, loc, map);

            defender.MoveToWorld(loc, map);

            defender.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1152076); // You are captured by the jailor and returned to your cell.
        }

        public override void OnDamagedBySpell(Mobile caster)
        {
            ParalyzeAttack(caster);
        }

        private static Hashtable m_Table;

        public virtual void ParalyzeAttack(Mobile to)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            if (to.Alive && to.Player && m_Table[to] == null)
            {
                to.SendSpeedControl(SpeedControlType.WalkSpeed);
                to.FixedEffect(0x376A, 6, 1);
                to.SendLocalizedMessage(500111); // You are frozen and cannot move.
                to.PlaySound(0x204);

                m_Table[to] = Timer.DelayCall(TimeSpan.FromSeconds(15), new TimerStateCallback(EndParalyze_Callback), to);
            }
        }

        private void EndParalyze_Callback(object state)
        {
            if (state is Mobile)
                ParalyzeEnd((Mobile)state);
        }

        public virtual void ParalyzeEnd(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            m_Table[from] = null;

            from.SendSpeedControl(SpeedControlType.Disable);
        }

        public static bool UnderParalyzeAttack(Mobile from)
        {
            if (m_Table == null)
                m_Table = new Hashtable();

            return m_Table[from] != null;
        }

        public DemonicJailor(Serial serial)
            : base(serial)
        { 
        }

        private DateTime m_NextTerror;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {

            if (this.UseSkill(SkillName.DetectHidden))
                m.RevealingAction();
            base.OnMovement(m, oldLocation);

               if (m_NextTerror < DateTime.Now && m != null && InRange(m.Location, 3) && m.AccessLevel == AccessLevel.Player)
            {
                m.Frozen = true;
                m.SendLocalizedMessage(1080342, Title, 33); // Terror slices into your very being, destroying any chance of resisting ~1_name~ you might have had

                Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerStateCallback(Terrorize), m);
            }
        }

        private void Terrorize(object o)
        {
            if (o is Mobile)
            {
                Mobile m = (Mobile)o;

                m.Frozen = false;
                m.SendLocalizedMessage(1005603); // You can move again!

                m_NextTerror = DateTime.Now + TimeSpan.FromMinutes(1);
            }
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool BardImmune { get { return !Core.SE; } }
        public override bool Unprovokable { get { return Core.SE; } }
        public override bool AreaPeaceImmune { get { return Core.SE; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void GenerateLoot()
        {
            this.AddLoot(LootPack.FilthyRich);
            this.AddLoot(LootPack.Meager);
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