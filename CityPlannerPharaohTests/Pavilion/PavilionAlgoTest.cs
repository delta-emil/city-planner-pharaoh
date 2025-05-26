namespace CityPlannerPharaohTests.Pavilion;

[TestFixture]
public class PavilionAlgoTest
{
    [TestCaseSource(sourceType: typeof(PavilionTestData), sourceName: nameof(PavilionTestData.Data))]
    public void CheckValidCase(string[] item)
    {
        string caseName = item[0];
        string[] expected = item[1..];
        string[] input = new string[expected.Length];
        for (int i = 0; i < expected.Length; i++)
        {
            input[i] = expected[i].Replace('D', ' ').Replace('M', ' ').Replace('J', ' ').Replace('g', ' ');
        }

        string[] output = PavilionAlgo.AddPavilion(input) ?? input;
        Assert.That(string.Join(Environment.NewLine, output), Is.EqualTo(string.Join(Environment.NewLine, expected)), caseName);
    }

    [Test]
    public void CheckTheImpossibleCrossroadCase()
    {
        string[] input =
        [
            "   .",
            "....",
            "   .",
            "   .",
        ];

        string[]? output = PavilionAlgo.AddPavilion(input);
        if (output != null)
        {
            Assert.Fail(string.Join(Environment.NewLine, output));
        }
    }

    [Test]
    public void CheckNotProperCrossroadCase()
    {
        string[] input =
        [
            "    ",
            "  ..",
            "  . ",
            "....",
        ];

        string[]? output = PavilionAlgo.AddPavilion(input);
        if (output != null)
        {
            Assert.Fail(string.Join(Environment.NewLine, output));
        }
    }

    [Test]
    public void CheckOtherBuildingInTheWayCase()
    {
        string[] input =
        [
            "  . ",
            "X . ",
            "  . ",
            "....",
        ];

        string[]? output = PavilionAlgo.AddPavilion(input);
        if (output != null)
        {
            Assert.Fail(string.Join(Environment.NewLine, output));
        }
    }
}
