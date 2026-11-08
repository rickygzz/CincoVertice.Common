using CincoVertice.Common.Utils.Phone;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Phone;

public class PhoneNumberScoreTest
{
    [Fact]
    public void Score()
    {
        var a = new PhoneNumberScore
        {
            PhonesList =
            [
                new PhoneModel { Phone = "8125974788", Batch = "A" },
                new PhoneModel { Phone = "8121202021", Batch = "A" }
            ]
        };

        Assert.Equal("8125974788", a.PhonesList[0].Phone);

        foreach (PhoneModel model in a.PhonesList)
        {
            a.Score(model);
        }

        Assert.Equal(1, a.PhonesList[0].Score);
        Assert.Equal(12, a.PhonesList[1].Score);

        // a.Save(@"Result.xlsx");
    }

    [Theory]
    [InlineData("10", 1)]
    [InlineData("1010", 2)]
    [InlineData("1985", 5)]
    [InlineData("19851985", 10)]
    [InlineData("0123", 5)]
    [InlineData("777", 11)]
    public void Scores(string phone, int expectedScore)
    {
        var a = new PhoneNumberScore();

        var model = new PhoneModel()
        {
            Phone = phone,
            Batch = "A"
        };

        a.Score(model);

        Assert.Equal(expectedScore, model.Score);
    }

    [Fact]
    public void Rules_HasEveryRowOfTheSpreadsheet()
    {
        List<PhoneScoreRuleModel> rules = PhoneScoreRules.Create();

        Assert.Equal(126, rules.Count);
        Assert.Equal(126, rules.Select(r => r.Rule).Distinct().Count());
    }

    [Fact]
    public void Rules_EachScorerGetsItsOwnCopy()
    {
        Assert.NotSame(PhoneScoreRules.Create()[0], PhoneScoreRules.Create()[0]);
    }
}
