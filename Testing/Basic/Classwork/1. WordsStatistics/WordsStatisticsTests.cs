using System.Collections.Immutable;
using Basic.Classwork.DoNotOpen;
using Basic.Task.WordsStatistics.WordsStatistics;
using FluentAssertions;
using NUnit.Framework;

namespace Basic.Task.WordsStatistics;

// Документация по FluentAssertions с примерами : https://github.com/fluentassertions/fluentassertions/wiki

[TestFixture]
public class WordsStatisticsTests
{

    private IWordsStatistics wordsStatistics;

    [SetUp]
    public void SetUp()
    {
        wordsStatistics = CreateStatistics();
    }

    public virtual IWordsStatistics CreateStatistics()
    {
        // меняется на разные реализации при запуске exe
        return new WordsStatisticsImpl();
    }

    private void AddWords(string[] words)
    {
        foreach (var word in words)
        {
            wordsStatistics.AddWord(word);
        }
    }


    [Test]
    public void GetStatistics_IsEmpty_AfterCreation()
    {
        wordsStatistics.GetStatistics().Should().BeEmpty();
    }

    [Test]
    public void GetStatistics_ContainsItem_AfterAddition()
    {
        wordsStatistics.AddWord("abc");
        wordsStatistics.GetStatistics().Should().Equal(new WordCount("abc", 1));
    }

    [Test]
    public void GetStatistics_WordsInOrder()
    {
        wordsStatistics.AddWord("1");
        AddWords(["2", "2"]);
        AddWords(["4", "4"]);
        AddWords(["3", "3", "3"]);

        wordsStatistics.GetStatistics().Should().Equal(
            new WordCount("3", 3),
            new WordCount("2", 2),
            new WordCount("4", 2),
            new WordCount("1", 1)
        );
    }

    [Test]
    public void AddWord_LotsOfDifferentNums()
    {
        var result = new WordsStatisticsImpl();
        for (int i = 0; i < 2048; i++)
        {
            wordsStatistics.AddWord(i.ToString());
            result.AddWord(i.ToString());
        }
        wordsStatistics.GetStatistics().Should().Equal(result.GetStatistics());
    }

    [Test]
    public void AddWord_CantAdd_Null()
    {
        Action act = () => wordsStatistics.AddWord(null);

        act.Should().Throw();
    }

    [Test]
    public void AddWord_CantAdd_Empty()
    {
        Action act = () => wordsStatistics.AddWord(string.Empty);

        act.Should().NotThrow();
    }
    
    [Test]
    public void AddWord_Empty_IsNotAdded()
    {
        try
        {
            wordsStatistics.AddWord(string.Empty);
            wordsStatistics.AddWord("     ");
            wordsStatistics.AddWord("");
        }
        catch (Exception ex)
        {
            ex.Should().BeNull();
        }
        finally
        {
            wordsStatistics.GetStatistics().Should().HaveCount(0);
        }
    }

    [Test]
    public void AddWord_IsCuttedCorrectly()
    {
        wordsStatistics.AddWord("12345678910"); // > 10
        wordsStatistics.AddWord("0123456789"); // = 10
        wordsStatistics.AddWord("012345678"); // 5 < < 10
        wordsStatistics.AddWord("012345"); // 5 < < 10
        wordsStatistics.AddWord("0123"); // < 5
        wordsStatistics.AddWord("0"); // < 5

        wordsStatistics.GetStatistics().Select(x => x.Word.Length).OrderBy(x => x).Should().Equal(1, 4, 6, 9, 10, 10);
    }

}