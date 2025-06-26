using System.Collections.Generic;

namespace TournamentScoring.Model
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public List<MatchResult> Matches { get; set; } = new();
        public bool IsDisqualified { get; set; }
        public int PenaltyPoints { get; set; }

        /// <summary>
        /// Constructeur complet
        /// </summary>
        /// <param name="name">Nom du joueur</param>
        /// <param name="matches">Liste des résultats de combat</param>
        /// <param name="isDisqualified">Statut de disqualification</param>
        /// <param name="penaltyPoints">Points de pénalité</param>
        public Player(string name, List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
        {
            Name = name;
            Matches = matches ?? new List<MatchResult>();
            IsDisqualified = isDisqualified;
            PenaltyPoints = penaltyPoints;
        }
    }
}