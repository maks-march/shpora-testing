using FluentAssertions;
using NUnit.Framework;
using TDD.Task;

namespace TDD;

[TestFixture]
public class GameTests
{
    [Test]
    [Explicit]
    public void HaveZeroScore_BeforeAnyRolls()
    {
        new Game()
            .GetScore()
            .Should().Be(0);
    }

    [Test]
    public void HaveCorrectScore_AfterOneRoll()
    {
        var game = new Game();
        game.Roll(5);
        game.GetScore().Should().Be(5);
    }

    //[Test]
    //public void DidPlayerGet_Spare()
    //{
    //    var game = new Game();
    //    game.Roll(5);
    //    game.Roll(5);
    //    game.IsSpare.Should().BeTrue();
    //}

    //[Test]
    //public void DidPlayerGet_Strike()
    //{
    //    var game = new Game();
    //    game.Roll(10);
    //    game.IsStrike.Should().BeTrue();
    //}

    [Test]
    public void BeatMoreThanTenKegels_ShouldCrashGame()
    {
        var game = new Game();
        var action = () => game.Roll(11);
        action.Should().Throw<Exception>();
    }

    [Test]
    public void BeatLessThanZeroKegels_ShouldCrashGame()
    {
        var game = new Game();
        var action = () => game.Roll(-11);
        action.Should().Throw<Exception>();
    }

    [Test]
    public void ThrowAfterSpare()
    {
        var game = new Game();
        game.Roll(5);
        game.Roll(5);
        game.Roll(4);
        game.GetScore().Should().Be(18);
    }

    [Test]
    public void ThrowAfterStrike()
    {
        var game = new Game();
        game.Roll(10);
        game.Roll(5);
        game.GetScore().Should().Be(5);
    }

    [Test]
    public void FrameAfterStrike()
    {
        var game = new Game();
        game.Roll(10);
        game.Roll(5);
        game.Roll(1);
        game.GetScore().Should().Be(22);
    }

    [Test]
    public void StrikesTwoFramesInARow_ThenTwoRolls()
    {
        var game = new Game();
        game.Roll(10);
        game.Roll(10);
        game.Roll(1);
        game.Roll(1);
        game.GetScore().Should().Be(35);
    }

    [Test]
    public void SpareAndTwoThrows()
    {
        var game = new Game();
        game.Roll(5);
        game.Roll(5);
        game.Roll(4);
        game.Roll(4);
        game.GetScore().Should().Be(22);
    }

}
