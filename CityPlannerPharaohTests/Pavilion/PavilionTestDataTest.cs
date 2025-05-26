namespace CityPlannerPharaohTests.Pavilion;

[TestFixture]
public class PavilionTestDataTest
{
    [Test]
    public void ConfirmDataIsUnique()
    {
        Dictionary<string, string[]> dict = new();
        foreach (var item in PavilionTestData.Data)
        {
            string[] d = item[1..];
            string key = string.Concat(d);
            if (dict.TryGetValue(key, out var first))
            {
                Assert.Fail($"Duplicate data for names `{first[0]}` and `{item[0]}`");
            }

            dict.Add(key, item);
        }
    }
}
