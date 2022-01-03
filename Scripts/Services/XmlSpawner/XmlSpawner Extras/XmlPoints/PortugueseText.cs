//Traduzido por ShadowMaster__

using System;
using Server;
using System.Text;

namespace Server.Engines.XmlSpawner2
{

    public class PortuguesePointsText
    {

        private static void Add(int index, string text)
        {
            XmlPoints.AddText(XmlPoints.LanguageType.PORTUGUESE, index, text);
        }

    	public static void Initialize()
    	{
            // add in all of the text phrases that will be used
            // indices that begin with 100 refer to messages displayed in the game window, so no real length limit
            // indices that begin with 200 refer to gump labels and so should be translated to text of similar length for proper appearance
            
            // basechallengegame.cs
            Add(100000, "A taxa de entrada de {0} moedas de ouro foram retornadas para você.");
            Add(100001, "A taxa de entrade de {0} foi depositada em seu banco.");
            Add(100002, "Que comessem os jogos!");
            Add(100003, "{0} cancelado");

            // challengeregion.cs
            Add(100104, "Atos de violência não são permitos antes ou depois do duelo.");
            Add(100105, "Você não autorização para abrir aquilo aqui.");
            Add(100106, "Você entrou na arena de Jogos '{0}'");
            Add(100107, "Você deixou a arena de jogos '{0}'");
            
            // xmlpoints.cs
            Add(100207, "Eles possuem muita experiência para serem desafiados.");
            Add(100208, "Faltam {0} para que o desafio atual seja cancelado.");
            Add(100209, "Cancelando desafio atual.  Espere {0} minutos");
            Add(100210, "{0} esta cancelando o desafio atual. {1} minutos restantes.");
            Add(100211, "{0} já está sendo desafiado.");
            Add(100212, "Você não pode desafiar você mesmo.");
            Add(100213, "PVP não suportado.");
            Add(100214, "Desafio com {0} foi cancelado");
            Add(100215, "Você recebeu {0} pontos por matar {1}");
            Add(100216, "{0} derrotou {1} em combate.");
            Add(100217, "Você perdeu {0} ponto(s) por ter sido derrotado por {1}");
            // xmlpoints identify
            Add(100218, "Pontos Atuais = {0}");
            Add(100219, "Rank = {0}");
            Add(100220, "Sem ranking");
            Add(100221, "Créditos disponíveis = {0}");
            Add(100222, "Nº de Kills = {0}\nNº de Mortes = {1}\nÚltimas Kills");
            Add(100223, "{0} morte em {1}");
            // xmlpoints gump
            Add(200224, "Points Standing for {0}");
            Add(200225, "Atualmente desafiando {0}");
            Add(200226, "Ver Kills" );
            Add(200227, "Anúncio de kills");
            Add(200228, "Top Players");
            Add(200229, "Duelo PVP");
            Add(200230, "Survivor");
            Add(200231, "Deathmatch");
            Add(200232, "KotH");
            Add(200233, "DeathBall");
            Add(200234, "Team Survivor");
            Add(200235, "Team DMatch");
            Add(200236, "Team DBall");
            Add(200237, "Team KotH");
            Add(200238, "CTF");
            // topplayers gump
            Add(200239, "Top Player Rankings");
            Add(200240, "Filtrar por Guildas");
            Add(200241, "Filtrar por Nomes");
            Add(200242, "Nome");
            Add(200243, "Guilda");
            Add(200244, "Pontos");
            Add(200245, "Kills");
            Add(200246, "Mortes");
            Add(200247, "Rank");
            Add(200248, "Mudar");
            Add(200249, "Tempo no Ranking" );
            Add(200250, "{0} dias " );
            Add(200251, "{0} horas " );
            Add(200252, "{0} minutos" );
            Add(200253, "acabou de mudar" );
            // challenge gump
            Add(200254, "Você está desafiando" );
            Add(200255, "{0}. Continuar?" );
            Add(200256, "Você não vai ganhar pontos!" );
            Add(100257, "Você propôs um desafio {0}." );
            Add(100258, "Você decidiu por desafiar {0}." );
            Add(200259, "Você esta sendo desafiado por" );
            Add(200260, "{0}. Aceitar?" );
            Add(100261, "{0} já está sendo desafiado." );
            Add(100262, "Você já está sendo desafiado." );
            Add(100263, "{0} aceitou seu desafio!" );
            Add(100264, "Você aceitou o desafio de {0}!" );
            Add(100265, "Sua proposta de desafiar {0} foi recusada." );
            Add(100266, "Você recusou o desafio de {0}." );
			Add(100267, "Você não pode emitir um desafio aqui.");

            // gauntlet scripts
            Add(100300, "Prêmio por {0}" );
            Add(100301, "Você recebeu um cheque por {0}" );
            Add(100302, "Survivor!" );
            Add(100303, "Impossivél realizar o desafio: {0}: Um outro desafio já está sendo realizado nesta área.");
            Add(100304, "Impossivél realizar o desafio: {0}: Deve ser realiado numa área de Desafios.");
            Add(100305, "Impossivél realizar o desafio: {0}.");
            Add(100306, "Propramando um desafio de: {0}.");
            Add(100307, "{0} desafio sendo preparado em '{1}' by {2}");
            Add(100308, "{0} foi desqualificado");
            Add(100309, "Você está fora dos limitas da arena!  Você tem {0} segundos para retornar.");
            Add(100310, "Você tem {0} para se mostrar");
            Add(100311, "Você venceu {0}");
            Add(100312, "O vencedor é {0}");
            Add(100313, "EMPATE");
            Add(100314, "{0} foi morto");
            Add(100315, "Desafiado por {0}");
            Add(100400, "Deathmatch");
            Add(100401, "{0} foi penalizado.");
            Add(100410, "King of the Hill");
            Add(100411, "Deathball");
            Add(100412, "{0} has dropped the ball!");
            Add(100413, "Team Survivor");
            Add(100414, "Time {0} é o vencedor!");
            Add(100415, "Team Deathmatch");
            Add(100416, "Team Deathball");
            Add(100417, "Team KotH");
            Add(100418, "CTF");
            Add(100419, "A bandeira do time {0} retornou para a base.");
            Add(100420, "Time {0} tem a bandeira do time {1}.");
            Add(100421, "Time {0} marcou");
            
            // game gump scripts
            Add(200500, "Survivor");
            Add(200501, "Organizado por: {0}");
            Add(200502, "Taxa de entrada: {0}");
            Add(200503, "Tamanho da Arena: {0}");
            Add(200504, "Total Purse: {0}");
            Add(200505, "Loc: {0} {1}");
            Add(200506, "Participantes: {0}");
            Add(200507, "Ativos: {0}");
            Add(200508, "Página: {0}/{1}");
            Add(200509, "Esperando");
            Add(200510, "Aceito");
            Add(200511, "Fundos Insuficientes");
            Add(200520, "W.O");
            Add(200521, "Escondido");
            Add(200522, "Fora das fronteiras");
            Add(200523, "Offline");
            Add(200524, "Vencedor");
            Add(200525, "Ativar");
            Add(200526, "Morto");
            Add(200527, "Desqualificado");
            Add(200528, "Adicionar");
            Add(200529, "Começar");
            Add(200530, "Ajuste de taxa de entrada: ");
            Add(200531, "Ajuste do tamanho da arena: ");
            Add(200532, "Atualizar");
            Add(200533, "Desafio já em andamento!");
            Add(200534, "{0} é o vencedor!");
            Add(100535, "O desafio já está cheio!");
            Add(100536, "{0} não se qualifica. No points support.");
            Add(100537, "{0} já está em um desafio.");
            Add(100538, "{0} não pode pagar a taxa de entrada.");
            Add(100539, "{0} ainda não aceitou.");
            Add(100540, "Número de participantes insuficiente.");
            Add(100541, "Não se pode retirar a taxa de entrada ({0}) de sua conta no banco.");
            Add(100542, "A taxa de entrada no valor de {0} foi retirada de seu banco.");
            Add(100543, "Você desistiu do {0}");
            Add(100544, "{0} desiste.");
            Add(100545, "Você foi expulsso do {0}");
            Add(100546, "{0} does not qualify. No points support.");
            Add(100547, "{0} já está em um desafio.");
            Add(100548, "{0} já foi adicionado ao jogo.");
            Add(100549, "Você adicionou {0} ao desafio.");
            Add(100550, "Você foi convidado a participar do desafio: {0}.");
            Add(200560, "Desafio de Deathmatch");
            Add(200561, "Pontos necessários: {0}");
            Add(200562, "Pontos necessários: zero");
            Add(200563, "Tempo maxímo: {0}");
            Add(200564, "Tempo maxímo: Ilimitado");
            Add(200565, "Tempo restante: {0}");
            Add(200566, "Placar: ");
            Add(200567, "Tempo em minutos: ");
            Add(100568, "A partida seria eterna.");
            Add(200570, "Desafio de Deathball");
            Add(200572, "Taxa de entrada: ");
            Add(200573, "Tamanho da arena: ");
            Add(200580, "Desafio do King of the Hill");
            Add(200590, "Desafio de Team Deathmatch");
            Add(200591, "Times");
            Add(200592, "Ajustar Times");
            Add(200593, "Time {0} é o vencedor!");
            Add(100594, "{0} não foi colocado em nenhum time.");
            Add(200595, "Status do Team Deathmatch");
            Add(200596, "Membros");
            Add(200597, "Ativar");
            Add(200598, "Placar");
            Add(200600, "Desafio de Team KotH");
            Add(200601, "Status do Team KotH");
            Add(200610, "Team Survivor");
            Add(200611, "Status do Team Survivor");
            Add(200620, "Desafio de Capture the Flag");
            Add(100621, "Time {0} base não definida.");
            Add(200622, "Base");
            Add(200623, "CTF Team Status");
            Add(200630, "Desafio de Team Deathball");
            Add(200631, "Status do Team Deathball");
            
            Add(200640, "Faction");
            Add(200649, "Localização: {0}");
            Add(200650, "{0} está ocupada.");

            Add(200660, "Cancelar");
            Add(200661, "Duelar aqui");
            Add(100670, "{0} está em combate.");

    	}
    }
}
