
using System.Runtime.CompilerServices;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    #region Constructor tests

    private static IEnumerable<TestCaseData> Constructor_MustThrow()
    {
        yield return new TestCaseData(-1, 2, true);
        yield return new TestCaseData(-1, 2, false);
        yield return new TestCaseData(1, -2, true);
        yield return new TestCaseData(-1, -2, false);
        yield return new TestCaseData(1, 2, true);
        yield return new TestCaseData(1, 1, false);
    }

    private static IEnumerable<TestCaseData> Constructor_NotThrow()
    {
        yield return new TestCaseData(1, 0, true);
        yield return new TestCaseData(int.MaxValue, int.MaxValue-1, true);
    }

    [Test, TestCaseSource(nameof(Constructor_MustThrow))]
    public void Constructor_Throw_OnInvalidArgument(int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);
        action.Should().Throw();
    }

    [Test, TestCaseSource(nameof(Constructor_NotThrow))]
    public void Constructor_NotThrow_OnValidArgument(int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);
        action.Should().NotThrow();
    }
    
    #endregion

    #region IsValidNumber tests

    private static IEnumerable<TestCaseData> CorrectNumbers_Cases()
    {
        yield return new TestCaseData(17, 2, true, new string[] { "0.0", "0" });

        yield return new TestCaseData(4, 2, false, new string[] { "-1.23", "12.32" });
        
        yield return new TestCaseData(4, 2, true, new string[] {"+1.23", "32"});

        yield return new TestCaseData(20, 19, false, new string[] {"1.1234567890123456789", "1123456789012345678.9"});
    }
    private static IEnumerable<TestCaseData> IncorrectNumbers_Cases()
    {
        yield return new TestCaseData(3, 2, true, new string[] { "00.00", "-0.00", "a.sd", "+0.00", "+1.23" });
        
        yield return new TestCaseData(4, 2, true, new string[] {"-0.00", "-1.23"});

        yield return new TestCaseData(17, 2, false, new string[] {"0.sd00", "O12322121.23"});

        yield return new TestCaseData(19, 18, false, new string[] {"1.1234567890123456789", "12345678912345678900.01"});
        
    }

    [Test, TestCaseSource(nameof(CorrectNumbers_Cases))]
    public void IsValidNumber_CorrectNumber(int precision, int scale, bool onlyPositive, string[] numbers)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        foreach (var number in numbers)
        {
            validator.IsValidNumber(number).Should().BeTrue();
        }
    }

    [Test, TestCaseSource(nameof(IncorrectNumbers_Cases))]
    public void IsValidNumber_IncorrectNumber(int precision, int scale, bool onlyPositive, string[] numbers)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        foreach (var number in numbers)
        {
            validator.IsValidNumber(number).Should().BeFalse();
        }
    }

    #endregion
}