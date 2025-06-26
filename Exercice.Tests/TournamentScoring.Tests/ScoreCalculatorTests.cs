using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using TournamentScoring.Contracts;
using TournamentScoring.Services;
using TournamentScoring.Model;

namespace TournamentScoring.Tests
{
    public class ScoreCalculatorTests
    {
        private readonly IScoreCalculator _calculator;

        public ScoreCalculatorTests()
        {
            _calculator = new ScoreCalculator();
        }

        #region Tests de base (fonctionnement normal)

        [Fact]
        public void Should_Calculate_Basic_Score_Without_Bonus()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),  // 3 points
                new(MatchResult.Result.Draw), // 1 point
                new(MatchResult.Result.Loss)  // 0 point
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(4);
        }

        [Fact]
        public void Should_Calculate_Score_With_Only_Wins()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(6);
        }

        [Fact]
        public void Should_Calculate_Score_With_Only_Draws()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Draw),
                new(MatchResult.Result.Draw),
                new(MatchResult.Result.Draw)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(3);
        }

        [Fact]
        public void Should_Calculate_Score_With_Only_Losses()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Loss),
                new(MatchResult.Result.Loss)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);
        }

        #endregion

        #region Tests du bonus de série

        [Fact]
        public void Should_Add_Bonus_For_Three_Consecutive_Wins()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(14);
        }

        [Fact]
        public void Should_Add_Bonus_For_Four_Consecutive_Wins()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(17);
        }

        [Fact]
        public void Should_Not_Give_Bonus_When_Series_Is_Interrupted()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Loss),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(9);
        }

        [Fact]
        public void Should_Give_Multiple_Bonuses_For_Multiple_Series()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win), // Serie 1
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win), // Bonus 1
                new(MatchResult.Result.Loss), // Interruption
                new(MatchResult.Result.Win), // Serie 2
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)  // Bonus 2
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(31);
        }

        [Fact]
        public void Should_Not_Give_Bonus_For_Win_Draw_Win_Pattern()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Draw),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(10);
        }

        #endregion

        #region Tests de disqualification

        [Fact]
        public void Should_Return_Zero_When_Player_Is_Disqualified()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win)
            };

            // Act
            var score = _calculator.CalculateScore(matches, isDisqualified: true);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        public void Should_Return_Zero_When_Disqualified_With_No_Matches()
        {
            // Arrange
            var matches = new List<MatchResult>();

            // Act
            var score = _calculator.CalculateScore(matches, isDisqualified: true);

            // Assert
            score.Should().Be(0);
        }

        #endregion

        #region Tests des pénalité

        [Fact]
        public void Should_Subtract_Penalty_Points_From_Score()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Draw)
            };

            // Act
            var score = _calculator.CalculateScore(matches, penaltyPoints: 3);

            // Assert
            score.Should().Be(12);
        }

        [Fact]
        public void Should_Not_Allow_Negative_Final_Score_With_High_Penalties()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Draw)
            };

            // Act
            var score = _calculator.CalculateScore(matches, penaltyPoints: 10);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        public void Should_Return_Zero_When_Penalties_Equal_Score()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Win),
                new(MatchResult.Result.Draw)
            };

            // Act
            var score = _calculator.CalculateScore(matches, penaltyPoints: 7);

            // Assert
            score.Should().Be(0);
        }

        #endregion

        #region Tests des cas limites

        [Fact]
        public void Should_Return_Zero_For_Empty_Match_List()
        {
            // Arrange
            var matches = new List<MatchResult>();

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(0);
        }

        [Fact]
        public void Should_Throw_Exception_When_Matches_Is_Null()
        {
            // Act & Assert
            Action act = () => _calculator.CalculateScore(null);

            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("matches")
                .WithMessage("*ne peut pas être null*");
        }

        [Fact]
        public void Should_Throw_Exception_When_Penalty_Points_Are_Negative()
        {
            // Arrange
            var matches = new List<MatchResult>
            {
                new(MatchResult.Result.Win)
            };

            // Act & Assert
            Action act = () => _calculator.CalculateScore(matches, penaltyPoints: -5);

            act.Should().Throw<ArgumentException>()
                .WithParameterName("penaltyPoints")
                .WithMessage("*ne peuvent pas être négatifs*");
        }

        [Fact]
        public void Should_Handle_Very_Long_Tournament_With_Complex_Pattern()
        {
            // Arrange evec 100 matches 
            var matches = new List<MatchResult>();

            // 3 wins, 1 loss, repeté 25 fois
            for (int i = 0; i < 25; i++)
            {
                matches.Add(new(MatchResult.Result.Win));
                matches.Add(new(MatchResult.Result.Win));
                matches.Add(new(MatchResult.Result.Win)); 
                matches.Add(new(MatchResult.Result.Loss));
            }

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(350);
        }

        #endregion

        #region Tests avec données paramétré

        [Theory]
        [InlineData(3, 0, 0, 14)] // 3 wins = 9 + 5 bonus = 14
        [InlineData(2, 1, 0, 7)]  // 2 wins, 1 draw = 7
        [InlineData(0, 0, 3, 0)]  // 3 loss = 0 points
        [InlineData(1, 2, 1, 5)]  // 1 win, 2 draws, 1 loss = 5 points
        public void Should_Calculate_Score_With_Different_Results(int wins, int draws, int losses, int expectedScore)
        {
            // Arrange
            var matches = new List<MatchResult>();

            for (int i = 0; i < wins; i++)
                matches.Add(new(MatchResult.Result.Win));
            for (int i = 0; i < draws; i++)
                matches.Add(new(MatchResult.Result.Draw));
            for (int i = 0; i < losses; i++)
                matches.Add(new(MatchResult.Result.Loss));

            // Act
            var score = _calculator.CalculateScore(matches);

            // Assert
            score.Should().Be(expectedScore);
        }

        public static IEnumerable<object[]> ComplexScenarios()
        {
            yield return new object[]
            {
                new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Draw)
                },
                false, // pas disqualifie
                0,     // pas de panalité
                15     // attendu : 3*3 + 1 + 5 bonus = 15
            };

            yield return new object[]
            {
                new List<MatchResult>
                {
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Loss),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win),
                    new(MatchResult.Result.Win)
                },
                false, // pas disqualifie
                5,     // pas de panalité
                26     // attendu : 7*3 + 2*5 - 5 = 21 + 10 - 5 = 26
            };
        }

        [Theory]
        [MemberData(nameof(ComplexScenarios))]
        public void Should_Handle_Complex_Scenarios(List<MatchResult> matches, bool isDisqualified, int penaltyPoints, int expectedScore)
        {
            // Act
            var score = _calculator.CalculateScore(matches, isDisqualified, penaltyPoints);

            // Assert
            score.Should().Be(expectedScore);
        }

        #endregion
    }
}