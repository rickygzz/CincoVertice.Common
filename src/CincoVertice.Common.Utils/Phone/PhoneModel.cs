namespace CincoVertice.Common.Utils.Phone;

public class PhoneModel
{
    public string Phone { get; set; } = string.Empty;

    public string Batch { get; set; } = string.Empty;

    public int Score { get; set; }

    public List<PhoneScoreRuleModel> Rules { get; set; } = [];
}
