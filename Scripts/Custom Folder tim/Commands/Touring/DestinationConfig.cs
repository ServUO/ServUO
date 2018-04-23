using System;
using Server;

namespace Server.Touring
{
	public static class DestinationConfig
	{
		public static void Init()
		{
			Tour.AddDestination(
				Map.Ilshenar,
				new Point3D(666, 524, 0),
				@"Utgard Docks",
				@"Hier bei den Docks kommen alle neuen Spieler an! Hier dreht sich alles um das Meer,Schiffe und Fische. Zudem ist hier jeden Tag von 6 Uhr morgens bis 14 Uhr mittags ein kleiner Markt.",
				TimeSpan.FromSeconds(15.0)
			);

			Tour.AddDestination(
				Map.Ilshenar,
				new Point3D(752, 557, 0),
				@"Utgard",
				@"Das ist sie! Die letzte Bastion der Menschheit. Eine Festung, eine Stadt, ein Zufluchtsort! Hier findet man alle Personen, seien es H�ndler, B�rger, Milit�rs und Andere.",
				TimeSpan.FromSeconds(10.0)
			);

			Tour.AddDestination(
				Map.Ilshenar,
				new Point3D(892, 872, 0),
				@"Mine",
				@"Das ist die einzige Mine auf Utgard. Hier kann man Erz abbauen, es verarbeiten und in der Stadt weiterverkaufen. Die ganzen Waffen und Werkzeuge und andere Gegenst�nde wurden aus Erzen aus dieser Mine erschaffen!",
				TimeSpan.FromSeconds(15.0)
                  );

			Tour.AddDestination(
				Map.Ilshenar,
				new Point3D(929, 547, 0),
				@"Wald",
				@"Das hier ist ein Wald ganz in der N�he der Stadt Utgard. In W�ldern kann man wilde Tiere wie B�ren oder W�lfe treffen!",
				TimeSpan.FromSeconds(15.0)
                  );
		}
	}
}