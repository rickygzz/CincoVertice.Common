using System.Text.RegularExpressions;
using Ganss.Excel;

namespace CincoVertice.Common.Utils.Phone;

public class PhoneNumberScore
{

    public List<PhoneModel> PhonesList = [];

    public List<PhoneScoreRuleModel> phoneScoreRuleModels = [];

    public PhoneNumberScore(string rulesFile)
    {
        LoadRules(rulesFile);
    }

    public void LoadRules(string file)
    {
        phoneScoreRuleModels = new ExcelMapper(file).Fetch<PhoneScoreRuleModel>().ToList();
    }

    public void LoadPhones(string file)
    {
        PhonesList = [.. new ExcelMapper(file)
            .Fetch<PhoneExcelModel>()
            .Select(p => new PhoneModel
            {
                Phone = p.Phone,
                Batch = p.Batch,
                Score = p.Score,
            })];
    }

    public void Score(PhoneModel model)
    {
        foreach (var rule in phoneScoreRuleModels)
        {
            var rx = Regex.Matches(model.Phone, rule.Rule);

            if (rx.Any())
            {
                model.Score += rx.Count * rule.Points;
                model.Rules.Add(rule);
            }
        }
    }

    public void Save(string file)
    {
        var excel = new ExcelMapper();

        excel.AddMapping<PhoneModel>("Rules", p => p.Rules).AsJson();

        excel.Save(file, PhonesList, "Phones");
    }
}
