using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    #region test data
    private static Person currentTsar = TsarRegistry.GetCurrentTsar();
    private static Person tsarCopy = new Person("Ivan IV The Terrible", 54, 170, 70, new Person("Vasili III of Russia", 28, 170, 60, null));
    private static Person person0Parents = new Person("1", 1, 1, 1, null);
    private static Person person0ParentsCopy = new Person("1", 1, 1, 1, null);
    private static Person person1Parents = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, null));
    private static Person person1ParentsСopy = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, null));
    private static Person person2Parents = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, new Person("3", 3, 3, 3, null)));
    private static Person person2ParentsCopy = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, new Person("3", 3, 3, 3, null)));
    #endregion
    private static IEnumerable<TestCaseData> TsarCheck_Cases()
    {
        yield return new TestCaseData(null, null).SetName("Пустые аргументы");
        yield return new TestCaseData(currentTsar, tsarCopy).SetName("Проверка царя");
        yield return new TestCaseData(person0Parents, person0ParentsCopy).SetName("Нет родителя");
        yield return new TestCaseData(person1Parents, person1ParentsСopy).SetName("Есть родитель");
        yield return new TestCaseData(person2Parents, person2ParentsCopy).SetName("Родитель у родителя");
    }

    private static IEnumerable<TestCaseData> TsarCheck_FailCases()
    {
        var tsarCopy = new Person("Ivan IV The", 54, 170, 70, new Person("Vasili III of Russia", 28, 170, 60, null));

        yield return new TestCaseData(currentTsar, tsarCopy).SetName("Проверка неправильного царя");
        yield return new TestCaseData(person0Parents, person1ParentsСopy).SetName("Разный родитель");
        yield return new TestCaseData(person1Parents, person2ParentsCopy).SetName("Разный родитель у родителя");
        yield return new TestCaseData(currentTsar, person0Parents).SetName("Полностью разные");
    }

    [Test, TestCaseSource(nameof(TsarCheck_Cases))]
    [Description("Проверка текущего царя")]
    [Category("ToRefactor")]
    public void CheckCurrentTsar(Person? actualTsar, Person? expectedTsar)
    {
        // Перепишите код на использование Fluent Assertions.
        AreEqualFuent(actualTsar, expectedTsar);
    }

    [Test, TestCaseSource(nameof(TsarCheck_FailCases))]
    public void FailCheckCurrentTsar(Person? actualTsar, Person? expectedTsar)
    {
        Action act = () => AreEqualFuent(actualTsar, expectedTsar);
        act.Should().Throw<AssertionException>();
    }

    private void AreEqualFuent(Person? actual, Person? expected)
    {
        actual.Should().BeEquivalentTo(expected, options =>
            options
                .Excluding(t => t.Id)
                .Excluding(t => t.Parent!) // Игнорировать Parent
        );
        if (actual != null && expected != null)
        {
            if (actual.Parent != null && expected.Parent != null)
            {
                AreEqualFuent(actual.Parent, expected.Parent);
            }
            if (actual.Parent == null || expected.Parent == null)
            {
                actual.Parent.Should().BeNull();
                expected.Parent.Should().BeNull(); // Если один из родителей null то оба должны быть null
            }
        }        
    }

    // При добавлении новых полей в Person придется расширять условие в return еще больше;
    // Тест должен пройти по всем Parent прежде чем дать результат, 
    // в моем тесте при различии полей, он сразу прервется выкинув исключение;
    // Я добавил TestCaseSource для добавления новых кейсов, в данном тесте данные собираются в нем;
    // Использование FluentAssertions упрощает прочтение кода в моем тесте;

    [Test]
    [Description("Альтернативное решение. Какие у него недостатки?")]
    public void CheckCurrentTsar_WithCustomEquality()
    {
        var actualTsar = TsarRegistry.GetCurrentTsar();
        var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
            new Person("Vasili III of Russia", 28, 170, 60, null));

        // Какие недостатки у такого подхода? 
        ClassicAssert.True(AreEqual(actualTsar, expectedTsar));
    }

    private bool AreEqual(Person? actual, Person? expected)
    {
        if (actual == expected) return true;
        if (actual == null || expected == null) return false;
        // Большой return 
        return
            actual.Name == expected.Name
            && actual.Age == expected.Age
            && actual.Height == expected.Height
            && actual.Weight == expected.Weight
            && AreEqual(actual.Parent, expected.Parent);
    }
}
