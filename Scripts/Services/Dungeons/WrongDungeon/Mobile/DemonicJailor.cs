using Server.Items;
using Server.Network;
using System;
using System.Collections;

namespace Server.Mobiles
{
    public class DemonicJailor : BaseCreature
    {
        [Constructable]
        public DemonicJailor()
            : base(AIType.AI_Melee, FightMode.Closest, 15, 1, 0.1, 0.3)
        {
            Name = NameList.RandomName("male");
            SpeechHue = Utility.RandomDyedHue();
            Title = "the Demonic Jailor";
            Hue = 34531;
            Body = 0x190;

            SetStr(386, 400);
            SetDex(151, 165);
            SetInt(161, 175);

            SetDamage(8, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 125.0);
            SetSkill(SkillName.Fencing, 46.0, 77.5);
            SetSkill(SkillName.Macing, 35.0, 57.5);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.DetectHidden, 100);
            SetSkill(SkillName.MagicResist, 83.5, 92.5);
            SetSkill(SkillName.Swords, 125.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Lumberjacking, 125.0);

            Fame = 5000;
            Karma = -5000;

            SetWearable(new ShortPants(Utility.RandomRedHue()));
            AddItem(new Sandals(Utility.RandomRedHue()));
            AddItem(new Shirt(Utility.RandomRedHue()));
            AddItem(new SkinningKnife());

            Utility.AssignRandomHair(this);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            Point3D loc = new Point3D(5703, 639, 0);
            Map map = Map;

            Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x3728, 10, 10, 0, 0, 2023, 0);
            Effects.PlaySound(loc, map, 0x1FE);

            TeleportPets(defender, loc, map);

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

            if (UseSkill(SkillName.DetectHidden))
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

        public override bool AlwaysMurderer => true;

        public override bool BardImmune => false;

        public override bool Unprovokable => true;

        public override bool AreaPeaceImmune => true;

        public override Poison PoisonImmune => Poison.Lethal;

        public override void GenerateLoot()
        {
            AddLoot(LootPack.FilthyRich);
            AddLoot(LootPack.Meager);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}