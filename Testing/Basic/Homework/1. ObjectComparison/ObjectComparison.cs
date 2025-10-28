using System.Runtime.CompilerServices;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HomeExercise.Tasks.ObjectComparison;
public class ObjectComparison
{
    #region test data
    private static Person currentTsar = TsarRegistry.GetCurrentTsar();
    private static Person tsarCopy = new Person("Ivan IV The Terrible", 54, 170, 70, new Person("Vasili III of Russia", 28, 170, 60, null));
    private static Person tsarCopyDifferent = new Person("Ivan IV The Kind", 54, 170, 70, new Person("Vasili III of Russia", 28, 170, 60, null));

    private static Person person0Parents = new Person("1", 1, 1, 1, null);
    private static Person person0ParentsCopy = Person.Copy(person0Parents);
    private static Person person0ParentsLimits = new Person(string.Empty, int.MaxValue, int.MinValue, 0, null);
    private static Person person0ParentsLimitsCopy = Person.Copy(person0ParentsLimits);
    private static Person person1Parents = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, null));
    private static Person person1ParentsСopy = Person.Copy(person1Parents);
    private static Person person1ParentsDifferent = new Person("1", 1, 1, 1, new Person("3", 3, 3, 3, null));
    private static Person person2Parents = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, new Person("3", 3, 3, 3, null)));
    private static Person person2ParentsCopy = Person.Copy(person2Parents);
    private static Person person2ParentsDifferent = new Person("1", 1, 1, 1, new Person("2", 2, 2, 2, new Person("5", 5, 5, 5, null)));
    #endregion
    
    private static IEnumerable<TestCaseData> FieldCompare_SuccessCases()
    {
        yield return new TestCaseData(null, null).SetName("Пустые аргументы");
        yield return new TestCaseData(currentTsar, tsarCopy).SetName("Проверка царя");
        yield return new TestCaseData(person0Parents, person0ParentsCopy).SetName("Нет родителя");
        yield return new TestCaseData(person1Parents, person1ParentsСopy).SetName("Есть родитель");
        yield return new TestCaseData(person0Parents, person1ParentsСopy).SetName("Разные родители");
        yield return new TestCaseData(person2Parents, person2ParentsDifferent).SetName("Разные прародители");
        yield return new TestCaseData(person0ParentsLimits, person0ParentsLimitsCopy).SetName("Проверка с граничными значениями полей");
    }

    [Test, TestCaseSource(nameof(FieldCompare_SuccessCases))]
    [Description("Проверка Person по полям - поля равны")]
    public void AreEqual_NotThrows_OnEqualFields(Person? actual, Person? expected)
    {
        actual.Should().BeEquivalentTo(expected, options =>
            options
                .Excluding(t => t.Id)
                .Excluding(t => t.Parent)
        );
    }    
    
    private static IEnumerable<TestCaseData> FieldCompare_FailCases()
    {
        yield return new TestCaseData(currentTsar, null).SetName("Сравнение с null");
        yield return new TestCaseData(currentTsar, tsarCopyDifferent).SetName("Проверка неправильного царя");
        yield return new TestCaseData(currentTsar, person0Parents).SetName("Полностью разные");
        yield return new TestCaseData(person0ParentsLimits, person0Parents).SetName("Просто разные");
    }

    [Test, TestCaseSource(nameof(FieldCompare_FailCases))]
    [Description("Проверка Person по полям - поля разные")]
    public void AreEqual_Throws_OnDifferentFields(Person? actual, Person? expected)
    {
        actual.Should().NotBeEquivalentTo(expected, options =>
            options
                .Excluding(t => t.Id)
                .Excluding(t => t.Parent)
        );
    }

    private static IEnumerable<TestCaseData> FullCompare_SuccessCases()
    {
        yield return new TestCaseData(null, null).SetName("Пустые аргументы при полном сравнении");
        yield return new TestCaseData(currentTsar, tsarCopy).SetName("Проверка царя при полном сравнении");
        yield return new TestCaseData(person0Parents, person0ParentsCopy).SetName("Нет родителя при полном сравнении");
        yield return new TestCaseData(person1Parents, person1ParentsСopy).SetName("Есть родитель при полном сравнении");
        yield return new TestCaseData(person2Parents, person2ParentsCopy).SetName("Родитель у родителя при полном сравнении");
    }


    [Test, TestCaseSource(nameof(FullCompare_SuccessCases))]
    [Description("Проверка Person по Parent - Parent равны")]
    public void AreEqual_NotThrows_OnEqualPersons(Person? actual, Person? expected)
    {
        actual.Should().BeEquivalentTo(expected, options =>
            options
                .Excluding(t => t.Id)
                .Using<Person>(ctx =>
                    {
                        AreEqual_NotThrows_OnEqualFields(ctx.Subject, ctx.Expectation);
                        if (ctx.Subject != null && ctx.Expectation != null)
                        {
                            if (ctx.Subject.Parent != null && ctx.Expectation.Parent != null)
                            {
                                AreEqual_NotThrows_OnEqualPersons(ctx.Subject.Parent, ctx.Expectation.Parent);
                            }
                            else
                            {
                                ctx.Subject.Parent.Should().BeEquivalentTo(ctx.Expectation.Parent, options =>
                                    options
                                        .Excluding(t => t.Id)
                                        .Excluding(t => t.Parent)
                                );
                            }
                        }                        
                    }
                )
                .WhenTypeIs<Person>()
        ); 
    }

    private static IEnumerable<TestCaseData> FullCompare_FailCases()
    {
        yield return new TestCaseData(currentTsar, null).SetName("Сравнение с null при полном сравнении");
        yield return new TestCaseData(currentTsar, tsarCopyDifferent).SetName("Проверка неправильного царя при полном сравнении");
        yield return new TestCaseData(currentTsar, person1Parents).SetName("Полностью разные при полном сравнении");
        yield return new TestCaseData(person1Parents, person0Parents).SetName("Родитель null при полном сравнении");
        yield return new TestCaseData(person1Parents, person1ParentsDifferent).SetName("Разные родители при полном сравнении");
        yield return new TestCaseData(person2Parents, person2ParentsDifferent).SetName("Разные прародители при полном сравнении");
    }

    [Test, TestCaseSource(nameof(FullCompare_FailCases))]
    [Description("Проверка Person по Parent - Parent различны")]
    public void AreEqual_Throw_OnDifferentPersons(Person? actual, Person? expected)
    {
        actual.Should().NotBeEquivalentTo(expected, options =>
            options
                .Excluding(t => t.Id)
                .Using<Person>(ctx =>
                    {
                        AreEqual_NotThrows_OnEqualFields(ctx.Subject, ctx.Expectation);
                        if (ctx.Subject != null && ctx.Expectation != null)
                        {
                            if (ctx.Subject.Parent != null && ctx.Expectation.Parent != null)
                            {
                                AreEqual_NotThrows_OnEqualPersons(ctx.Subject.Parent, ctx.Expectation.Parent);
                            }
                            else
                            {
                                (ctx.Subject.Parent == null && ctx.Expectation.Parent == null).Should().BeTrue();
                            }
                        }
                    }
                )
                .WhenTypeIs<Person>()
        );
    }
    
    //Как будто бы если оставлять обобщенный метод кода становится меньше

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
