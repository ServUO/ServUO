using System;
using Server;

namespace Server.Items
{
	public class CryptClosedCoffinEW : BaseAddon
	{

		[ Constructable ]
		public CryptClosedCoffinEW()
		{
			AddComponent( new AddonComponent( 7236 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7237 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7238 ), 1, 0, 0 );
		}

		public CryptClosedCoffinEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedCoffinAnkhEW : BaseAddon
	{

		[ Constructable ]
		public CryptClosedCoffinAnkhEW()
		{
			AddComponent( new AddonComponent( 7235 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7234 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7233 ), -1, 0, 0 );
		}

		public CryptClosedCoffinAnkhEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinEW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinEW()
		{
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7230 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7231 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7232 ), -1, 0, 0 );
		}

		public CryptOpenCoffinEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinEW : BaseAddon
	{

		[ Constructable ]
		public CryptNoLidCoffinEW()
		{
			AddComponent( new AddonComponent( 7241 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7240 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7239 ), -1, 0, 0 );
		}

		public CryptNoLidCoffinEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinAnkhEW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinAnkhEW()
		{
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7229 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7231 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7232 ), -1, 0, 0 );
		}

		public CryptOpenCoffinAnkhEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinSkeletonEW : BaseAddon
	{

		[ Constructable ]
		public CryptNoLidCoffinSkeletonEW()
		{
			AddComponent( new AddonComponent( 7509 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7511 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7510 ), 0, 0, 0 );
		}

		public CryptNoLidCoffinSkeletonEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinAnkhSkeletonEW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinAnkhSkeletonEW()
		{
			AddComponent( new AddonComponent( 7506 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7508 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7507 ), 0, 0, 0 );
		}

		public CryptOpenCoffinAnkhSkeletonEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinSkeletonEW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinSkeletonEW()
		{
			AddComponent( new AddonComponent( 7508 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7507 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7512 ), 1, 0, 0 );
		}

		public CryptOpenCoffinSkeletonEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedCoffinNS : BaseAddon
	{

		[ Constructable ]
		public CryptClosedCoffinNS()
		{
			AddComponent( new AddonComponent( 7250 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7251 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7252 ), 0, 1, 0 );
		}

		public CryptClosedCoffinNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedCoffinAnkhNS : BaseAddon
	{

		[ Constructable ]
		public CryptClosedCoffinAnkhNS()
		{
			AddComponent( new AddonComponent( 7249 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7248 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7247 ), 0, -1, 0 );
		}

		public CryptClosedCoffinAnkhNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinNS : BaseAddon
	{

		[ Constructable ]
		public CryptNoLidCoffinNS()
		{
			AddComponent( new AddonComponent( 7253 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7254 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7255 ), 0, -1, 0 );
		}

		public CryptNoLidCoffinNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinSkeletonNW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinSkeletonNW()
		{
			AddComponent( new AddonComponent( 7244 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7539 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7245 ), 0, 0, 0 );
		}

		public CryptOpenCoffinSkeletonNW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinAnkhSkeletonNW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinAnkhSkeletonNW()
		{
			AddComponent( new AddonComponent( 7245 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7243 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7539 ), 0, -1, 0 );
		}

		public CryptOpenCoffinAnkhSkeletonNW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinNW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinNW()
		{
			AddComponent( new AddonComponent( 7255 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7254 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7244 ), 0, 1, 0 );
		}

		public CryptOpenCoffinNW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinAnkhNW : BaseAddon
	{

		[ Constructable ]
		public CryptOpenCoffinAnkhNW()
		{
			AddComponent( new AddonComponent( 7254 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7243 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7255 ), 0, -1, 0 );
		}

		public CryptOpenCoffinAnkhNW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinSkeletonNW : BaseAddon
	{

		[ Constructable ]
		public CryptNoLidCoffinSkeletonNW()
		{
			AddComponent( new AddonComponent( 7540 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7542 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7541 ), 0, 0, 0 );
		}

		public CryptNoLidCoffinSkeletonNW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}



	public class CryptClosedCasketEW : BaseAddon
	{
		[ Constructable ]
		public CryptClosedCasketEW()
		{
			AddComponent( new AddonComponent( 7202 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7203 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7204 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7205 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7206 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7207 ), 1, 0, 0 );
		}

		public CryptClosedCasketEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketEW : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketEW()
		{
			AddComponent( new AddonComponent( 7208 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7209 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7210 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7211 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 7212 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7213 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7214 ), 1, 0, 0 );
		}

		public CryptOpenCasketEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketSkeletonEW : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketSkeletonEW()
		{
			AddComponent( new AddonComponent( 7500 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7501 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7502 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7211 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 7503 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7213 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7505 ), 1, 0, 0 );
		}

		public CryptOpenCasketSkeletonEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedCasketNS : BaseAddon
	{
		[ Constructable ]
		public CryptClosedCasketNS()
		{
			AddComponent( new AddonComponent( 7216 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7215 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7217 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7218 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7219 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7220 ), 0, 1, 0 );
		}

		public CryptClosedCasketNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketNS : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketNS()
		{
			AddComponent( new AddonComponent( 7221 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7222 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7223 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7224 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 7225 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7226 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7227 ), 0, 1, 0 );
		}

		public CryptOpenCasketNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketSkeletonNS : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketSkeletonNS()
		{
			AddComponent( new AddonComponent( 7532 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7533 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7534 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7224 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 7535 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7536 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7537 ), 0, 1, 0 );
		}

		public CryptOpenCasketSkeletonNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedSarcophagusEWF : BaseAddon
	{
		[ Constructable ]
		public CryptClosedSarcophagusEWF()
		{
			AddComponent( new AddonComponent( 7264 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7265 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7266 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7267 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7268 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7269 ), 1, 0, 0 );
		}

		public CryptClosedSarcophagusEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusEWF : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusEWF()
		{
			AddComponent( new AddonComponent( 7273 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7274 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7275 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7276 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7277 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7278 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusSkeletonEWF : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusSkeletonEWF()
		{
			AddComponent( new AddonComponent( 7513 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7514 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7515 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7516 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7517 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7278 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusSkeletonEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusEWF : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusEWF()
		{
			AddComponent( new AddonComponent( 7279 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7280 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7281 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7282 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7283 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7284 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7285 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7286 ), 2, 0, 0 );
		}

		public CryptOpenSarcophagusEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusSkeletonEWF : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusSkeletonEWF()
		{
			AddComponent( new AddonComponent( 7279 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7518 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7519 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7520 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7521 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7522 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7285 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7286 ), 2, 0, 0 );
		}

		public CryptOpenSarcophagusSkeletonEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedSarcophagusEWM : BaseAddon
	{
		[ Constructable ]
		public CryptClosedSarcophagusEWM()
		{
			AddComponent( new AddonComponent( 7287 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7288 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7289 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7290 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7291 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7292 ), 1, 0, 0 );
		}

		public CryptClosedSarcophagusEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusEWM : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusEWM()
		{
			AddComponent( new AddonComponent( 7296 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7297 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7298 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7299 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7300 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7301 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusSkeletonEWM : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusSkeletonEWM()
		{
			AddComponent( new AddonComponent( 7523 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7524 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7525 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7526 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7527 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7301 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusSkeletonEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusEWM : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusEWM()
		{
			AddComponent( new AddonComponent( 7302 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7303 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7304 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7305 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7306 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 7307 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 7308 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7309 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7310 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7311 ), 2, 0, 0 );
		}

		public CryptOpenSarcophagusEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusSkeletonEWM : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusSkeletonEWM()
		{
			AddComponent( new AddonComponent( 7529 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7531 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7309 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7310 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7311 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7307 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 7530 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 7302 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7303 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7528 ), 0, 1, 0 );
		}

		public CryptOpenSarcophagusSkeletonEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedSarcophagusNSF : BaseAddon
	{
		[ Constructable ]
		public CryptClosedSarcophagusNSF()
		{
			AddComponent( new AddonComponent( 7312 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7313 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7314 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7315 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7316 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7317 ), 0, 1, 0 );
		}

		public CryptClosedSarcophagusNSF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusNSF : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusNSF()
		{
			AddComponent( new AddonComponent( 7326 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7325 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7324 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7323 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7322 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7321 ), 1, 1, 0 );
		}

		public CryptOpenSarcophagusNSF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusSkeletonNSF : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusSkeletonNSF()
		{
			AddComponent( new AddonComponent( 7543 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7544 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7545 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7546 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7547 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7326 ), 0, 1, 0 );
		}

		public CryptNoLidSarcophagusSkeletonNSF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptClosedSarcophagusNSM : BaseAddon
	{
		[ Constructable ]
		public CryptClosedSarcophagusNSM()
		{
			AddComponent( new AddonComponent( 7335 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7336 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7337 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7338 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7339 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7340 ), 0, 1, 0 );
		}

		public CryptClosedSarcophagusNSM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}



	public class CryptOpenSarcophagusNSM : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusNSM()
		{
			AddComponent( new AddonComponent( 7350 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 7351 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7352 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7353 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7354 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 7355 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 7356 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7357 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7358 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7359 ), 0, 2, 0 );
		}

		public CryptOpenSarcophagusNSM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCasketBodyEW : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidCasketBodyEW()
		{
			AddComponent( new AddonComponent( 7207 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7205 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7204 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7202 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7203 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7206 ), 0, 0, 0 );
		}

		public CryptNoLidCasketBodyEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketBodyEW : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketBodyEW()
		{
			AddComponent( new AddonComponent( 7445 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7214 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7443 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7444 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7211 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7446 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7447 ), 1, 0, 0 );
		}

		public CryptOpenCasketBodyEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCasketBodyNS : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCasketBodyNS()
		{
			AddComponent( new AddonComponent( 7474 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7224 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7475 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7476 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7227 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7472 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 7473 ), 1, 1, 0 );
		}

		public CryptOpenCasketBodyNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinBodyEW : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidCoffinBodyEW()
		{
			AddComponent( new AddonComponent( 7451 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7450 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7452 ), -1, 0, 0 );
		}

		public CryptNoLidCoffinBodyEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidCoffinBodyNS : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidCoffinBodyNS()
		{
			AddComponent( new AddonComponent( 7479 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7481 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7480 ), 0, 0, 0 );
		}

		public CryptNoLidCoffinBodyNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinBodyAnkhEW : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCoffinBodyAnkhEW()
		{
			AddComponent( new AddonComponent( 7229 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7449 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7448 ), 0, 0, 0 );
		}

		public CryptOpenCoffinBodyAnkhEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinBodyAnkhNS : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCoffinBodyAnkhNS()
		{
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7477 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7243 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7478 ), 0, -1, 0 );
		}

		public CryptOpenCoffinBodyAnkhNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinBodyEW : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCoffinBodyEW()
		{
			AddComponent( new AddonComponent( 7230 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7228 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7449 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7448 ), 0, 0, 0 );
		}

		public CryptOpenCoffinBodyEW( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenCoffinBodyNS : BaseAddon
	{
		[ Constructable ]
		public CryptOpenCoffinBodyNS()
		{
			AddComponent( new AddonComponent( 7242 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7477 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7244 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7478 ), 0, -1, 0 );
		}

		public CryptOpenCoffinBodyNS( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusBodyEWM : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusBodyEWM()
		{
			AddComponent( new AddonComponent( 7471 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7470 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7306 ), -2, 1, 0 );
			AddComponent( new AddonComponent( 7307 ), -2, 0, 0 );
			AddComponent( new AddonComponent( 7302 ), 2, 1, 0 );
			AddComponent( new AddonComponent( 7311 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7310 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7309 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7469 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7303 ), 1, 1, 0 );
		}

		public CryptOpenSarcophagusBodyEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusBodyEWM : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusBodyEWM()
		{
			AddComponent( new AddonComponent( 7465 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7467 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7466 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7468 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7301 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7464 ), 1, 1, 0 );
		}

		public CryptNoLidSarcophagusBodyEWM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusBodyEWF : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusBodyEWF()
		{
			AddComponent( new AddonComponent( 7462 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7461 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7459 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7460 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7463 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7286 ), 2, 0, 0 );
			AddComponent( new AddonComponent( 7285 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7279 ), 2, 1, 0 );
		}

		public CryptOpenSarcophagusBodyEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusBodyEWF : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusBodyEWF()
		{
			AddComponent( new AddonComponent( 7453 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7454 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7455 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 7456 ), -1, 0, 0 );
			AddComponent( new AddonComponent( 7457 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7458 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusBodyEWF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusBodyNSF : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusBodyNSF()
		{
			AddComponent( new AddonComponent( 7485 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7486 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7482 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7326 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7484 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7483 ), 1, 0, 0 );
		}

		public CryptNoLidSarcophagusBodyNSF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusBodyNSF : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusBodyNSF()
		{
			AddComponent( new AddonComponent( 7491 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7489 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7333 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7487 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7488 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7327 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 7334 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7490 ), 0, -1, 0 );
		}

		public CryptOpenSarcophagusBodyNSF( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptNoLidSarcophagusBodyNSM : BaseAddon
	{
		[ Constructable ]
		public CryptNoLidSarcophagusBodyNSM()
		{
			AddComponent( new AddonComponent( 7349 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7496 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 7495 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7494 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7493 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7492 ), 1, 1, 0 );
		}

		public CryptNoLidSarcophagusBodyNSM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class CryptOpenSarcophagusBodyNSM : BaseAddon
	{
		[ Constructable ]
		public CryptOpenSarcophagusBodyNSM()
		{
			AddComponent( new AddonComponent( 7358 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 7359 ), 0, 2, 0 );
			AddComponent( new AddonComponent( 7350 ), 1, 2, 0 );
			AddComponent( new AddonComponent( 7351 ), 1, 1, 0 );
			AddComponent( new AddonComponent( 7497 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 7498 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 7354 ), 1, -2, 0 );
			AddComponent( new AddonComponent( 7355 ), 0, -2, 0 );
			AddComponent( new AddonComponent( 7499 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 7357 ), 0, 0, 0 );
		}

		public CryptOpenSarcophagusBodyNSM( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}