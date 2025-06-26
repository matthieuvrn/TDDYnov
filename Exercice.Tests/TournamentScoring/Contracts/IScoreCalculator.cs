using System.Collections.Generic;
using TournamentScoring.Model;

namespace TournamentScoring.Contracts
{
    /// <summary>
    /// Interface pour le calculateur de score de tournoi d'escrime
    /// </summary>
    public interface IScoreCalculator
    {
        /// <summary>
        /// Calcule le score final d'un joueur selon les règles du tournoi
        /// </summary>
        /// <param name="matches">Liste des résultats de combat dans l'ordre chronologique</param>
        /// <param name="isDisqualified">True si le joueur est disqualifié</param>
        /// <param name="penaltyPoints">Points de pénalité (nombre positif)</param>
        /// <returns>Score final (jamais négatif)</returns>
        int CalculateScore(List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0);
    }
}