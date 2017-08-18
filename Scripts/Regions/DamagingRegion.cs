using System;
using System.Collections;
using System.Xml;
using Server.Network;
using Server.Mobiles;

namespace Server.Regions
{
    public class DamagingRegion : MondainRegion
    { 
        private Hashtable m_Table;
        public DamagingRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public virtual int EnterMessage
        {
            get
            {
                return 0;
            }
        }
        public virtual int EnterSound
        {
            get
            {
                return 0;
            }
        }
        public virtual TimeSpan DamageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }
        public Hashtable Table
        {
            get
            {
                return m_Table;
            }
        }
        public override void OnEnter(Mobile m)
        {
            base.OnEnter(m);
		
            if (m.Player && m.Alive && m.IsPlayer())
            {
                if (EnterSound > 0)
                    m.PlaySound(EnterSound);
				
                if (EnterMessage > 0)	
                    m.SendLocalizedMessage(EnterMessage); 
				
                StartTimer(m);
            }
        }

        public override void OnLocationChanged(Mobile m, Point3D oldLocation)
        {
            base.OnLocationChanged(m, oldLocation);
			
            StopTimer(m);
			
            if (m.Player && m.Alive && m.IsPlayer())
                StartTimer(m);
        }

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);
			
            StopTimer(m);
        }

        public void StartTimer(Mobile m)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
				
            m_Table[m] = Timer.DelayCall(TimeSpan.Zero, DamageInterval, new TimerStateCallback(Damage), m);
        }

        public void StopTimer(Mobile m)
        {
            if (m_Table == null)
                m_Table = new Hashtable();
				
            if (m_Table[m] != null)
            {
                Timer timer = (Timer)m_Table[m];
				
                timer.Stop();
            }
        }

        public void Damage(object state)
        {
            if (state is Mobile)
                Damage((Mobile)state);			
        }

        public virtual void Damage(Mobile m)
        {
            if (m.Player && !m.Alive)
                StopTimer(m);
				
            m.RevealingAction();
        }
    }

    public class CrystalField : DamagingRegion
    {
        public CrystalField(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override int EnterMessage
        {
            get
            {
                return 1072396;
            }
        }// An electric wind chills your blood, making it difficult to traverse the cave unharmed.
        public override int EnterSound
        {
            get
            {
                return 0x22F;
            }
        }
        public override void Damage(Mobile m)
        {
            base.Damage(m);
			
            if (m.NetState != null)
                AOS.Damage(m, Utility.Random(2, 6), 0, 0, 100, 0, 0);
            else
                m.LogoutLocation = new Point3D(6502, 87, 0);
        }
    }

    public class IcyRiver : DamagingRegion
    { 
        public IcyRiver(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override void Damage(Mobile m)
        {
            base.Damage(m);

            if (m.NetState != null)
            {
                int dmg = Utility.Random(2, 3);

                if (m is PlayerMobile)
                {

                    PlayerMobile pm = m as PlayerMobile;

                    dmg = (int)Server.Items.BalmOfProtection.HandleDamage(pm, dmg);

                }

                AOS.Damage(m, dmg, 0, 0, 100, 0, 0);
            }
        }
    }

    public class PoisonedSemetery : DamagingRegion
    { 
        public PoisonedSemetery(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override TimeSpan DamageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(5);
            }
        }
        public override void Damage(Mobile m)
        {
            base.Damage(m);
			
            if (m.NetState != null)
            {
                m.FixedParticles(0x36B0, 1, 14, 0x26BB, 0x3F, 0x7, EffectLayer.Waist);
                m.PlaySound(0x229);
                AOS.Damage(m, Utility.Random(2, 3), 0, 0, 0, 100, 0);
            }
        }
    }

    public class PoisonedTree : DamagingRegion
    { 
        public PoisonedTree(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override TimeSpan DamageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(1);
            }
        }
        public override void Damage(Mobile m)
        {
            base.Damage(m);
			
            if (m.NetState != null)
            {
                m.FixedEffect(0x374A, 1, 17);
                m.PlaySound(0x1E1);
                m.LocalOverheadMessage(MessageType.Regular, 0x21, 1074165); // You feel dizzy from a lack of clear air
				
                int mod = (int)(m.Str * 0.1);
				
                if (mod > 10)
                    mod = 10;
					
                m.AddStatMod(new StatMod(StatType.Str, "Poisoned Tree Str", mod * -1, TimeSpan.FromSeconds(1)));
				
                mod = (int)(m.Int * 0.1);
				
                if (mod > 10)
                    mod = 10;
					
                m.AddStatMod(new StatMod(StatType.Int, "Poisoned Tree Int", mod * -1, TimeSpan.FromSeconds(1)));
            }
        }
    }

    public class AcidRiver : DamagingRegion
    { 
        public AcidRiver(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override TimeSpan DamageInterval
        {
            get
            {
                return TimeSpan.FromSeconds(3);
            }
        }
        public override void Damage(Mobile m)
        {
            base.Damage(m);
			
            if (m.Alive)
            {
                if (m.Location.X > 6484 && m.Location.Y > 500)
                    m.Kill();
                else
                {
                    m.FixedParticles(0x36B0, 1, 14, 0x26BB, 0x3F, 0x7, EffectLayer.Waist);
                    m.PlaySound(0x229);
					
                    int damage = 0;
					
                    damage += (int)Math.Pow(m.Location.X - 6200, 0.5);				
                    damage += (int)Math.Pow(m.Location.Y - 330, 0.5);	
					
                    if (damage > 20)
                        m.SendLocalizedMessage(1074567); // The acid river is much stronger here. You realize that allowing the acid to touch your flesh will surely kill you.
                    else if (damage > 10)
                        m.SendLocalizedMessage(1074566); // The acid river has gotten deeper. The concentration of acid is significantly stronger.
                    else
                        m.SendLocalizedMessage(1074565); // The acid river burns your skin.
					
                    AOS.Damage(m, damage, 0, 0, 0, 100, 0);
                }
            }
        }
    }
}