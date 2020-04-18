using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.Astronomy
{
    public class PrimerOnBritannianAstronomy : Item
    {
        public override int LabelNumber => 1158515;  // Looking to the Heavens: A Primer on Britannian Astronomy

        [Constructable]
        public PrimerOnBritannianAstronomy()
            : base(0xFF0)
        {
            Hue = 298;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile && m.InRange(GetWorldLocation(), 3))
            {
                Gump gump = new Gump(100, 100);
                gump.AddImage(0, 0, 0x761C);
                gump.AddImage(95, 40, 0x69E);
                gump.AddHtmlLocalized(115, 200, 350, 400, 1158516, "#1158516", 0x1, false, true);
                /*The study of Britannian astronomy dates back to the appearances of strangers from offwordly realms. When it was learned that 
                 * Sosaria exists within a much larger universe, curiosity tilted our heads upwards towards the sky. Before long skilled tinkers 
                 * granted the ability to see objects at great distance became a reality - enter the telescope. While the most impressive example 
                 * of these contraptions is the one located in Moonglow at 43o 52'N, 122o 4'W, where I have chosen to spend my time deep in
                 * research, recent developments in miniaturization have brought the size of these instruments within the grasp of the casual 
                 * observer.<br><br>Using a telescope may appear quite simple, but one would be naive to think there is no more beyond haphazardly 
                 * pointing towards the sky to make observations!<br><br>The best viewing hours are during the night between the hours of 5pm 
                 * and 4am. Any standard clock is an essential tool in mapping the night sky!<br><br>While there are a variety of objects in the
                 * night sky to observe if one points their telescope at the object as it traverses space, the real search is for constellations
                 * - a unique collection of several stars.<br><br>Constellations appear at various periods during the night. Constellations can 
                 * be seen during the early evening (5pm-8pm), late at night (9pm-11pm), midnight (12am), or the middle of the night (1am-4am). 
                 * After 4am daylight will drown out any attempt at making meaningful observations.<br><br>Constellations can be observed in 
                 * these time periods based on their location in the night sky. Britannian astronomers have adopted a coordinate system that
                 * uses right ascension (RA) and declination (DEC) to pinpoint a constellation's location during night time periods.<br><br>RA 
                 * can be measured from 0-24 and DEC can be measured from 0-90. Current Telescopes are capable of resolving increments of 1 hour
                 * of RA and 0.2 degrees of DEC.<br><br>When a time period has been selected and the RA and DEC have been set one need only 
                 * initiate a switch conveniently located in the viewfinder, to check that section of the sky! Should you be lucky enough to 
                 * see a constellation a standard mapmaking pen can be used to create a star chart. As the potential discoverer of a new 
                 * constellation you have the opportunity to name and submit your discovery to the Britannian Astronomical Society, of whom 
                 * this author is primary registrar, for documentation.<br><br>Happy skywatching!*/

                m.SendGump(gump);
            }
        }

        public PrimerOnBritannianAstronomy(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}