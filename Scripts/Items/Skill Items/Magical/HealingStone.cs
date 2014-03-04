using System;

namespace Server.Items
{
    public class HealingStone : Item
    {
        private Mobile m_Caster;
		private int m_Amount;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Caster
        {
            get { return this.m_Caster; }
            set { this.m_Caster = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
		public int HealPoints
		{
			get
			{
				return this.m_Amount;
			}
			set
			{
				this.m_Amount = value;
			}
		}
		
        [Constructable]
        public HealingStone(Mobile caster, int amount)
            : base(0x4078)
        {
            this.m_Caster = caster;
            this.m_Amount = amount;
        }

        public HealingStone(Serial serial)
            : base(serial)
        {
        }

       public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);

			list.Add("Healing Points: {0}", m_Amount);
		}
		
		public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendLocalizedMessage(502138); // That is too far away for you to use
                return;
            }
            else if (from != this.m_Caster)
            {
                // from.SendLocalizedMessage( ); // 
                return;
            }

            BaseWeapon weapon = from.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;

            if (weapon == null)
                weapon = from.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

            if (weapon != null)
            {
                from.SendLocalizedMessage(1080116); // You must have a free hand to use a Healing Stone.
            }
			else if (from.Hits >= from.HitsMax)
						from.SendMessage("You are at full health,");
					
            else if (from.BeginAction(typeof(BaseHealPotion)))
            {
                int healamount = Utility.RandomMinMax(BasePotion.Scale(from, 25), BasePotion.Scale(from, 35));
					
				if (healamount > m_Amount)
					healamount = m_Amount;
					
				from.Heal (healamount);
					
				m_Amount -= healamount;
					
				if (m_Amount <= 0)
					this.Consume();
					
				this.InvalidateProperties();	
					
				Timer.DelayCall(TimeSpan.FromSeconds(8.0), new TimerStateCallback(ReleaseHealLock), from);
				
				
            }
            else
                from.SendLocalizedMessage(1095172); // You must wait a few seconds before using another Healing Stone.
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            this.Delete();
            return false;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(this.m_Caster);
			writer.Write(this.m_Amount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    this.m_Caster = reader.ReadMobile();
                    this.m_Amount = reader.ReadInt();
                    goto case 0;
                case 0:
                    if (this.m_Amount < 1)
                        this.Consume();
                    break;
            }
			
        }

        private static void ReleaseHealLock(object state)
        {
            ((Mobile)state).EndAction(typeof(BaseHealPotion));
        }
    }
}