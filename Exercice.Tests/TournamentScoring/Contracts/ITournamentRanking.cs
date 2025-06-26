using System.Collections.Generic;
using TournamentScoring.Model;

namespace TournamentScoring.Contracts
{
    /// <summary>
    /// Interface pour le système de classement de tournoi
    /// </summary>
    public interface ITournamentRanking
    {
        /// <summary>
        /// Classe les joueurs par score décroissant
        /// En cas d'égalité, le classement est alphabétique par nom
        /// </summary>
        /// <param name="players">Liste des joueurs à classer</param>
        /// <returns>Liste des joueurs classés par score décroissant</returns>
        List<Player> GetRanking(List<Player> players);

        /// <summary>
        /// Trouve le champion (joueur avec le meilleur score)
        /// En cas d'égalité, retourne le premier dans l'ordre alphabétique
        /// </summary>
        /// <param name="players">Liste des joueurs</param>
        /// <returns>Le joueur champion</returns>
        Player GetChampion(List<Player> players);

        /// <summary>
        /// Obtient le score d'un joueur
        /// </summary>
        /// <param name="player">Le joueur dont on veut le score</param>
        /// <returns>Score calculé du joueur</returns>
        int GetPlayerScore(Player player);

        /// <summary>
        /// Obtient tous les joueurs ayant le score maximum
        /// </summary>
        /// <param name="players">Liste des joueurs</param>
        /// <returns>Liste des joueurs ex-aequo au premier rang</returns>
        List<Player> GetTopScorers(List<Player> players);
    }
}