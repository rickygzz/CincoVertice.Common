using System.Text.RegularExpressions;

namespace CincoVertice.Common.Utils.Phone;

public class PhoneNumberScore
{

    public List<PhoneModel> PhonesList = [];

    public List<PhoneScoreRuleModel> phoneScoreRuleModels = PhoneScoreRules.Create();


    public void Score(PhoneModel model)
    {
        foreach (var rule in phoneScoreRuleModels)
        {
            var rx = Regex.Matches(model.Phone, rule.Rule);

            if (rx.Count != 0)
            {
                model.Score += rx.Count * rule.Points;
                model.Rules.Add(rule);
            }
        }
    }
}
