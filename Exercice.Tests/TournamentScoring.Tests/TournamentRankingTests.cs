using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using TournamentScoring.Contracts;
using TournamentScoring.Services;
using TournamentScoring.Model;

namespace TournamentScoring.Tests
{
    public class TournamentRankingTests
    {
        private readonly ITournamentRanking _ranking;
        private readonly IScoreCalculator _calculator;

        public TournamentRankingTests()
        {
            _calculator = new ScoreCalculator();
            _ranking = new TournamentRanking(_calculator);
        }

        #region Tests de classement

        [Fact]
        public void Should_Rank_Players_By_Score_Descending()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 6 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Charlie", new List<MatchResult>
                {
                    new(MatchResult.Result.Draw),
                    new(MatchResult.Result.Draw)
                }) // 2 points
            };

            // Act
            var ranking = _ranking.GetRanking(players);

            // Assert
            ranking.Should().HaveCount(3);
            ranking[0].Name.Should().Be("Bob");
            ranking[1].Name.Should().Be("Alice");
            ranking[2].Name.Should().Be("Charlie");
        }

        [Fact]
        public void Should_Handle_Equal_Scores_With_Alphabetical_Order()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Zoe", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 6 points
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 6 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Draw)
                }) // 4 points
            };

            // Act
            var ranking = _ranking.GetRanking(players);

            // Assert
            ranking.Should().HaveCount(3);
            ranking[0].Name.Should().Be("Alice");
            ranking[1].Name.Should().Be("Zoe");
            ranking[2].Name.Should().Be("Bob");
        }

        [Fact]
        public void Should_Include_Disqualified_Players_At_Bottom()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 6 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }, isDisqualified: true), // 0 points (disqualifié)
                new Player("Charlie", new List<MatchResult>
                {
                    new(MatchResult.Result.Draw)
                }) // 1 point
            };

            // Act
            var ranking = _ranking.GetRanking(players);

            // Assert
            ranking.Should().HaveCount(3);
            ranking[0].Name.Should().Be("Alice");
            ranking[1].Name.Should().Be("Charlie");
            ranking[2].Name.Should().Be("Bob");
        }

        [Fact]
        public void Should_Handle_Empty_Player_List()
        {
            // Arrange
            var players = new List<Player>();

            // Act
            var ranking = _ranking.GetRanking(players);

            // Assert
            ranking.Should().BeEmpty();
        }

        [Fact]
        public void Should_Throw_Exception_When_Players_List_Is_Null()
        {
            // Act & Assert
            Action act = () => _ranking.GetRanking(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("players");
        }

        #endregion

        #region Tests de champion

        [Fact]
        public void Should_Return_Player_With_Highest_Score_As_Champion()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Draw)
                }), // 4 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Charlie", new List<MatchResult>
                {
                    new(MatchResult.Result.Win)
                }) // 3 points
            };

            // Act
            var champion = _ranking.GetChampion(players);

            // Assert
            champion.Name.Should().Be("Bob");
        }

        [Fact]
        public void Should_Return_First_Alphabetically_When_Multiple_Champions()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Zoe", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win)
                }) // 3 points
            };

            // Act
            var champion = _ranking.GetChampion(players);

            // Assert
            champion.Name.Should().Be("Alice");
        }

        [Fact]
        public void Should_Throw_Exception_When_No_Players_For_Champion()
        {
            // Arrange
            var players = new List<Player>();

            // Act & Assert
            Action act = () => _ranking.GetChampion(players);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("players")
                .WithMessage("*ne peut pas être vide*");
        }

        [Fact]
        public void Should_Throw_Exception_When_Players_List_Is_Null_For_Champion()
        {
            // Act & Assert
            Action act = () => _ranking.GetChampion(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("players");
        }

        #endregion

        #region Tests des méthodes en plus

        [Fact]
        public void Should_Calculate_Player_Score_Correctly()
        {
            // Arrange
            var player = new Player("Test", new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            }, penaltyPoints: 2);

            // Act
            var score = _ranking.GetPlayerScore(player);

            // Assert
            score.Should().Be(12);
        }

        [Fact]
        public void Should_Get_All_Top_Scorers()
        {
            // Arrange
            var players = new List<Player>
            {
                new Player("Alice", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Bob", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }), // 14 points
                new Player("Charlie", new List<MatchResult>
                {
                    new(MatchResult.Result.Win)
                }), // 3 points
                new Player("Zoe", new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                }) // 14 points
            };

            // Act
            var topScorers = _ranking.GetTopScorers(players);

            // Assert
            topScorers.Should().HaveCount(3);
            topScorers.Should().Contain(p => p.Name == "Alice");
            topScorers.Should().Contain(p => p.Name == "Bob");
            topScorers.Should().Contain(p => p.Name == "Zoe");
            topScorers.Should().NotContain(p => p.Name == "Charlie");

            // ordre aklpahbetique
            topScorers[0].Name.Should().Be("Alice");
            topScorers[1].Name.Should().Be("Bob");
            topScorers[2].Name.Should().Be("Zoe");
        }

        [Fact]
        public void Should_Return_Empty_List_For_Top_Scorers_When_No_Players()
        {
            // Arrange
            var players = new List<Player>();

            // Act
            var topScorers = _ranking.GetTopScorers(players);

            // Assert
            topScorers.Should().BeEmpty();
        }

        [Fact]
        public void Should_Throw_Exception_When_Player_Is_Null_For_Top_Scorer()
        {
            // Act & Assert
            Action act = () => _ranking.GetTopScorers(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Should_Throw_Exception_When_Player_Is_Null_For_Score()
        {
            // Act & Assert
            Action act = () => _ranking.GetPlayerScore(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("player");
        }

        [Fact]
        public void Should_Throw_Exception_When_Calculator_Is_Null()
        {
            // Act & Assert
            Action act = () => new TournamentRanking(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("scoreCalculator");
        }

        #endregion
    }
}