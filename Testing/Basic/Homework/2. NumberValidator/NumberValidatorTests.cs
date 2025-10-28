
using System.Runtime.CompilerServices;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.NumberValidator;

[TestFixture]
public class NumberValidatorTests
{
    #region Constructor tests

    private static IEnumerable<TestCaseData> Constructor_MustThrow_OnPrecisionExceed()
    {
        yield return new TestCaseData(0, 0, false).SetName("Некорректная длина");
        yield return new TestCaseData(-1, -1, false).SetName("Некорректная длина");
        yield return new TestCaseData(-2, 2, true).SetName("Некорректная длина");
        yield return new TestCaseData(0, -3, false).SetName("Некорректная длина");
        yield return new TestCaseData(-5, -5, false).SetName("Некорректная длина");
    }

    private static IEnumerable<TestCaseData> Constructor_MustThrow_OnScaleExceed()
    {
        yield return new TestCaseData(1, 2, true).SetName("Некорректная точность");
        yield return new TestCaseData(1, 1, false).SetName("Некорректная точность");
        yield return new TestCaseData(1, -1, false).SetName("Некорректная точность");
    }

    private static IEnumerable<TestCaseData> Constructor_NotThrow_OnValidArguments()
    {
        yield return new TestCaseData(1, 0, true).SetName("Корректные аргументы");
        yield return new TestCaseData(int.MaxValue, int.MaxValue-1, true).SetName("Корректные аргументы");
    }

    [Test]
    [
        TestCaseSource(nameof(Constructor_MustThrow_OnPrecisionExceed)),
        TestCaseSource(nameof(Constructor_MustThrow_OnScaleExceed))
    ]
    public void Constructor_Throws_OnInvalidArgument(int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);
        action.Should().Throw();
    }

    [Test, TestCaseSource(nameof(Constructor_NotThrow_OnValidArguments))]
    public void Constructor_NotThrows_OnValidArgument(int precision, int scale, bool onlyPositive)
    {
        Action action = () => new NumberValidator(precision, scale, onlyPositive);
        action.Should().NotThrow();
    }

    #endregion

    #region IsValidNumber tests

    private static IEnumerable<TestCaseData> IsValidNumbers_ValidNumbersCases()
    {
        yield return new TestCaseData(17, 2, true, new string[] { "0.0", "0", "012322121.23" }).SetName("Правильные числа");

        yield return new TestCaseData(4, 2, false, new string[] { "-1,23", "12,32" }).SetName("Правильные числа");

        yield return new TestCaseData(4, 2, true, new string[] { "+1,23", "32" }).SetName("Правильные числа");

        yield return new TestCaseData(20, 19, false, new string[] { "1,1234567890123456789", "1123456789012345678.9" }).SetName("Правильные числа");
    }

    private static IEnumerable<TestCaseData> IsValidNumbers_TooBigNumbersCases()
    {
        yield return new TestCaseData(3, 2, false, new string[] { "00.00", "-0.00", "+0.00", "01,24" }).SetName("Слишком большие числа");

        yield return new TestCaseData(19, 18, false, new string[] { "1.1234567890123456789", "12345678912345678900.01" }).SetName("Слишком большие числа");

    }
    private static IEnumerable<TestCaseData> IsValidNumbers_WithWrongSignCases()
    {
        yield return new TestCaseData(4, 2, true, new string[] { "-0.00", "-1.23" }).SetName("Числа с неправильным знаком");;
        
        yield return new TestCaseData(4, 3, true, new string[] { ".000", "+.124" }).SetName("Числа без целой части");;
    }
    
    private static IEnumerable<TestCaseData> IsValidNumbers_WithLiteralsCases()
    {
        yield return new TestCaseData(4, 2, true, new string[] { "O.00", "_0.00"}).SetName("Некорректные символы");

        yield return new TestCaseData(17, 2, false, new string[] {"0.sd00", "OIOIIOIO"}).SetName("Некорректные символы");;

        yield return new TestCaseData(19, 18, false, new string[] {"1,12    0123456789", "1678912345678900 . 01", "#!@$:|><1,2?%^&*()"}).SetName("Некорректные символы");;
        
    }

    [Test, TestCaseSource(nameof(IsValidNumbers_ValidNumbersCases))]
    public void IsValidNumber_IsTrue_OnCorrectNumber(int precision, int scale, bool onlyPositive, string[] numbers)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        foreach (var number in numbers)
        {
            validator.IsValidNumber(number).Should().BeTrue();
        }
    }

    [Test]
    [
        TestCaseSource(nameof(IsValidNumbers_TooBigNumbersCases)),
        TestCaseSource(nameof(IsValidNumbers_WithLiteralsCases)),
        TestCaseSource(nameof(IsValidNumbers_WithWrongSignCases))
    ]
    public void IsValidNumber_IsFalse_OnIncorrectNumber(int precision, int scale, bool onlyPositive, string[] numbers)
    {
        var validator = new NumberValidator(precision, scale, onlyPositive);

        foreach (var number in numbers)
        {
            validator.IsValidNumber(number).Should().BeFalse();
        }
    }

    #endregion
}