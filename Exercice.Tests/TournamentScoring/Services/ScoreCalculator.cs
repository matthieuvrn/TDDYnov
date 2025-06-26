using System;
using System.Collections.Generic;
using TournamentScoring.Contracts;
using TournamentScoring.Model;

namespace TournamentScoring.Services
{
    /// <summary>
    /// Service de calcul des scores pour le tournoi d'escrime fantastique
    /// </summary>
    public class ScoreCalculator : IScoreCalculator
    {
        /// <summary>
        /// Calcule le score final d'un joueur selon les règles du tournoi
        /// </summary>
        /// <param name="matches">Liste des résultats de combat dans l'ordre chronologique</param>
        /// <param name="isDisqualified">True si le joueur est disqualifié</param>
        /// <param name="penaltyPoints">Points de pénalité (nombre positif)</param>
        /// <returns>Score final (jamais négatif)</returns>
        public int CalculateScore(List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
        {
            if (matches == null)
                throw new ArgumentNullException(nameof(matches), "La liste des matches ne peut pas être null");

            if (penaltyPoints < 0)
                throw new ArgumentException("Les points de pénalité ne peuvent pas être négatifs", nameof(penaltyPoints));

            // disqualifié, score = 0
            if (isDisqualified)
                return 0;

            int baseScore = CalculateBaseScore(matches);

            int bonusScore = CalculateConsecutiveWinsBonus(matches);

            int totalScore = baseScore + bonusScore;

            // application d pénalités (score final jamais négatif)
            int finalScore = Math.Max(0, totalScore - penaltyPoints);

            return finalScore;
        }

        private int CalculateBaseScore(List<MatchResult> matches)
        {
            int score = 0;

            foreach (var match in matches)
            {
                switch (match.Outcome)
                {
                    case MatchResult.Result.Win:
                        score += 3;
                        break;
                    case MatchResult.Result.Draw:
                        score += 1;
                        break;
                    case MatchResult.Result.Loss:
                        score += 0;
                        break;
                }
            }

            return score;
        }

        private int CalculateConsecutiveWinsBonus(List<MatchResult> matches)
        {
            int bonusCount = 0;
            int consecutiveWins = 0;

            foreach (var match in matches)
            {
                if (match.Outcome == MatchResult.Result.Win)
                {
                    consecutiveWins++;

                    if (consecutiveWins == 3)
                    {
                        bonusCount++;
                    }
                }
                else
                {
                    consecutiveWins = 0;
                }
            }

            return bonusCount * 5; // chaque bonus = 5 points
        }
    }
}