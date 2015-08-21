/*
 * Created by ArteGordon.
 * Translated To Spanish By: Darkness_PR(Paul)
 * Date: 2/5/2005
 * Time: 8:24 AM
 * 
 * Feel Free To Mod This How Ever You See Fit.
 */
using System;
using Server;
using System.Text;

namespace Server.Engines.XmlSpawner2
{

    public class SpanishPointsText
    {

        private static void Add(int index, string text)
        {
            XmlPoints.AddText(XmlPoints.LanguageType.SPANISH, index, text);
        }

    	public static void Initialize()
    	{
            // add in all of the text phrases that will be used
            // indices that begin with 100 refer to messages displayed in the game window, so no real length limit
            // indices that begin with 200 refer to gump labels and so should be translated to text of similar length for proper appearance
            
            // basechallengegame.cs
            Add(100000, "La cantidad de {0} oro a sido devolvida a ti.");
            Add(100001, "La Cantidad De {0} oro a sido devolvida a tu banco.");
            Add(100002, "Deja que los juegos empiezen!");
            Add(100003, "{0} cancelados");

            // challengeregion.cs
            Add(100104, "No sera permitido hacer daño antes o despues de un desafio.");
            Add(100105, "No es permitido abrir eso aqui.");
            Add(100106, "Has entrado la area de un juego '{0}'");
            Add(100107, "Has salido de la area del juego '{0}'");
            
            // xmlpoints.cs
            Add(100207, "Ellos no son expertos para ser desafiados");
            Add(100208, "{0} minuto(s) quedan para que el desafio sea cancelado");
            Add(100209, "Cancelando el desafio.  Por favor espera {0} minutos");
            Add(100210, "{0} esta cancelando el desafio. {1} minutos de espera");
            Add(100211, "{0} el challenge ya ha empezado.");
            Add(100212, "No te puedes desafiar tu mismo.");
            Add(100213, "Tu no tienes XmlPoints.");
            Add(100214, "El desafio con {0} ha sido cancelado.");
            Add(100215, "Has recivido {0} puntos por matar. {1}");
            Add(100216, "{0} a derrotado {1} en combate.");
            Add(100217, "Has perdido {0} punto (s) por haber sido matado por {1}");
            // xmlpoints identify
            Add(100218, "Tu Puntos Actuales = {0}");
            Add(100219, "Rango = {0}");
            Add(100220, "No hay rango");
            Add(100221, "Creditos Disponible = {0}");
            Add(100222, "Total De Matanzas = {0}\nTotal De Muertes = {1}\nLista De Matanzas");
            Add(100223, "{0} matado a {1}");
            // xmlpoints gump
            Add(200224, "Points Standing for {0}");
            Add(200225, "Actualmente en un desafio {0}");
            Add(200226, "Ver matanzas" );
            Add(200227, "Decir las matanzas");
            Add(200228, "Jugadores superiores");
            Add(200229, "Challenge");
            Add(200230, "LMS");
            Add(200231, "Deathmatch");
            Add(200232, "KotH");
            Add(200233, "DeathBall");
            Add(200234, "Team LMS");
            Add(200235, "Team DMatch");
            Add(200236, "Team DBall");
            Add(200237, "Team KotH");
            Add(200238, "CTF");
            // topplayers gump
            Add(200239, "Rangos de los Jugadores");
            Add(200240, "Filtro por Guild");
            Add(200241, "Filtro por Nombre");
            Add(200242, "Nombre");
            Add(200243, "Guild");
            Add(200244, "Puntos");
            Add(200245, "Matanzas");
            Add(200246, "Muertes");
            Add(200247, "Rango");
            Add(200248, "Cambiar");
            Add(200249, "Tiempo en el Rango" );
            Add(200250, "{0} dias " );
            Add(200251, "{0} horas " );
            Add(200252, "{0} minutos" );
            Add(200253, "actualmente cambiado" );
            // challenge gump
            Add(200254, "Estas desafiando" );
            Add(200255, "{0}. Continuar?" );
            Add(200256, "Tu no vas a ganar puntos!" );
            Add(100257, "Tu has hecho un challenge a {0}." );
            Add(100258, "Tu has decido contra el desafio {0}." );
            Add(200259, "Tu has sido desafiado por" );
            Add(200260, "{0}. Aceptar?" );
            Add(100261, "{0} ha sido ya desafiado." );
            Add(100262, "Tu estas en un desafio ya." );
            Add(100263, "{0} acepto tu desafio!" );
            Add(100264, "Tu has aceptado el desafio de {0}!" );
            Add(100265, "Tu desafio para {0} a sido cancelado." );
            Add(100266, "Has negado el desafio por {0}." );
			Add(100267, "Usted no puede publicar un desafío aquí.");

            // gauntlet scripts
            Add(100300, "Premio de {0}" );
            Add(100301, "Has recivido un cheque por {0}" );
            Add(100302, "Last Man Standing" );
            Add(100303, "Incapaz de instalar {0} Challenge: Otro juego ya esta en proceso en esta area.");
            Add(100304, "Incapaz de instalar el {0} Challenge: Tiene que estar en la area de Juego.");
            Add(100305, "Incapaz de instalar el {0} Challenge.");
            Add(100306, "Setting-up un {0} Challenge.");
            Add(100307, "{0} Challenge ha sido preparado '{1}' by {2}");
            Add(100308, "{0} has sido descalificado");
            Add(100309, "Estas fuera del area!  Tienes {0} segundo(s) para regrear.");
            Add(100310, "You have {0} seconds become unhidden");
            Add(100311, "Tu has ganado {0}");
            Add(100312, "El ganador es {0}");
            Add(100313, "El evento ha sido empate");
            Add(100314, "{0} ha sido matado");
            Add(100315, "Challenge por {0}");
            Add(100400, "Deathmatch");
            Add(100401, "{0} fue penalizado.");
            Add(100410, "King of the Hill");
            Add(100411, "Deathball");
            Add(100412, "{0} dejo caer la bola!");
            Add(100413, "Team LMS");
            Add(100414, "Grupo {0} es el ganador!");
            Add(100415, "Grupo Deathmatch");
            Add(100416, "Grupo Deathball");
            Add(100417, "Grupo KotH");
            Add(100418, "CTF");
            Add(100419, "Grupo {0} su bandera ha regresado a su base");
            Add(100420, "Gurpo {0} tiene la bandera del grupo {1} ");
            Add(100421, "Grupo {0} a anotado");
            
            // game gump scripts
            Add(200500, "Last Man Standing Challenge");
            Add(200501, "Organizado por: {0}");
            Add(200502, "Costo Para Entras: {0}");
            Add(200503, "Size De la Arena: {0}");
            Add(200504, "Total de Purse: {0}");
            Add(200505, "Lugar: {0} {1}");
            Add(200506, "Jugadores: {0}");
            Add(200507, "Activo: {0}");
            Add(200508, "Pagina: {0}/{1}");
            Add(200509, "Esperando");
            Add(200510, "Accepto");
            Add(200511, "Fondos escasos");
            Add(200520, "Perdio");
            Add(200521, "Escondido");
            Add(200522, "Fuera del Arena");
            Add(200523, "Offline");
            Add(200524, "Ganador");
            Add(200525, "Activo");
            Add(200526, "Muerto");
            Add(200527, "Descalificado");
            Add(200528, "Añadir");
            Add(200529, "Empezar");
            Add(200530, "Setiar Costo de Entrada: ");
            Add(200531, "Setiar el Size del Arena: ");
            Add(200532, "Restaurar");
            Add(200533, "El juego esta en proceso!");
            Add(200534, "{0} es el ganador!");
            Add(100535, "El Challenge esta lleno!");
            Add(100536, "{0} no cualifica. No hay puntos.");
            Add(100537, "{0} esta ya en un challenge.");
            Add(100538, "{0} no puede pagar el costo de entrada.");
            Add(100539, "{0} no ha aceptado todavia.");
            Add(100540, "No hay suficiente jugadores.");
            Add(100541, "No pudo sacar la cantidad de {0} Oro del banco.");
            Add(100542, "La cantidad de {0} oro ha sido sacado de tu banco.");
            Add(100543, "Has salido del {0}");
            Add(100544, "{0} se ha salido.");
            Add(100545, "Tu has sido sacado por {0}");
            Add(100546, "{0} no cualifica. No hay puntos.");
            Add(100547, "{0} ya esta en un Challenge.");
            Add(100548, "{0} ya ha sido añadido al juego.");
            Add(100549, "Tu añadistes a {0} para el challenge.");
            Add(100550, "Tu has sido invitado para participar en {0}.");
            Add(200560, "Deathmatch Challenge");
            Add(200561, "Puntajes del Blanco: {0}");
            Add(200562, "Puntos del Blanco: None");
            Add(200563, "Tiempo del Juego: {0}");
            Add(200564, "Tiempo del Juego: Unlimited");
            Add(200565, "Tiempo que queda {0}");
            Add(200566, "Puntos: ");
            Add(200567, "Minutos: ");
            Add(100568, "Ninguna condicion final para terminar el juego."); //Dunno how to translate//
            Add(200570, "Deathball Challenge");
            Add(200572, "Costo De Entrada: ");
            Add(200573, "Size Del Arena: ");
            Add(200580, "King of the Hill Challenge");
            Add(200590, "Team Deathmatch Challenge");
            Add(200591, "Grupos");
            Add(200592, "Setiar Grupos");
            Add(200593, "Grupo {0} es el ganador!");
            Add(100594, "{0} no has asignado un equipo.");
            Add(200595, "Deathmatch Team Status");
            Add(200596, "Miembros");
            Add(200597, "Activo");
            Add(200598, "Puntos");
            Add(200600, "Team KotH Challenge");
            Add(200601, "KotH Team Status");
            Add(200610, "Team LMS Challenge");
            Add(200611, "LMS Team Status");
            Add(200620, "Capture the Flag Challenge");
            Add(100621, "Grupo {0} no ha definido la base.");
            Add(200622, "Base");
            Add(200623, "CTF Team Status");
            Add(200630, "Team Deathball Challenge");
            Add(200631, "Deathball Team Status");
            
            Add(200640, "Faction");
            Add(200649, "Location: {0}");
            Add(200650, "{0} is occupied");

            Add(200660, "Cancel");
            Add(200661, "Duel here");
            Add(100670, "{0} is in combat.");

    	}
    }
}

