using System;
using System.Collections.Generic;
using System.Linq;
using TournamentScoring.Contracts;
using TournamentScoring.Model;

namespace TournamentScoring.Services
{
    /// <summary>
    /// Service de classement pour le tournoi d'escrime fantastique
    /// </summary>
    public class TournamentRanking : ITournamentRanking
    {
        private readonly IScoreCalculator _scoreCalculator;

        /// <summary>
        /// Constructeur avec injection de dépendance
        /// </summary>
        /// <param name="scoreCalculator">Service de calcul des scores</param>
        public TournamentRanking(IScoreCalculator scoreCalculator)
        {
            _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
        }

        /// <summary>
        /// Classe les joueurs par score décroissant
        /// En cas d'égalité, le classement est alphabétique par nom
        /// </summary>
        /// <param name="players">Liste des joueurs à classer</param>
        /// <returns>Liste des joueurs classés par score décroissant</returns>
        public List<Player> GetRanking(List<Player> players)
        {
            if (players == null)
                throw new ArgumentNullException(nameof(players));

            var playersWithScores = players
                .Select(player => new
                {
                    Player = player,
                    Score = _scoreCalculator.CalculateScore(
                        player.Matches,
                        player.IsDisqualified,
                        player.PenaltyPoints)
                })
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Player.Name) 
                .Select(x => x.Player)
                .ToList();

            return playersWithScores;
        }

        /// <summary>
        /// Trouve le champion (joueur avec le meilleur score)
        /// En cas d'égalité, retourne le premier dans l'ordre alphabétique
        /// </summary>
        /// <param name="players">Liste des joueurs</param>
        /// <returns>Le joueur champion</returns>
        public Player GetChampion(List<Player> players)
        {
            if (players == null)
                throw new ArgumentNullException(nameof(players));

            if (!players.Any())
                throw new ArgumentException("La liste des joueurs ne peut pas être vide", nameof(players));

            var rankedPlayers = GetRanking(players);
            return rankedPlayers.First();
        }

        /// <summary>
        /// Obtient le score d'un joueur
        /// </summary>
        /// <param name="player">Le joueur dont on veut le score</param>
        /// <returns>Score calculé du joueur</returns>
        public int GetPlayerScore(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            return _scoreCalculator.CalculateScore(
                player.Matches,
                player.IsDisqualified,
                player.PenaltyPoints);
        }

        /// <summary>
        /// Obtient tous les joueurs ayant le score maximum
        /// </summary>
        /// <param name="players">Liste des joueurs</param>
        /// <returns>Liste des joueurs ex-aequo au premier rang</returns>
        public List<Player> GetTopScorers(List<Player> players)
        {
            if (players == null)
                throw new ArgumentNullException(nameof(players));

            if (!players.Any())
                return new List<Player>();

            var champion = GetChampion(players);
            var championScore = GetPlayerScore(champion);

            return players
                .Where(player => GetPlayerScore(player) == championScore)
                .OrderBy(player => player.Name)
                .ToList();
        }
    }
}