using CincoVertice.Common.Utils.Config;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config;

public class ConfigFileTest
{
    private readonly ITestOutputHelper output;

    public ConfigFileTest(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void ConfigFile_GetsValidInput_ParsesCorrectly()
    {
        // Create test configuration file.
        string content = @"keyA =    1   
keyB =     2.5       
   keyC =     This is a string value        
keyD =    [""     This is a string value        ""]   
keyE =    [""     This is a multi
line string value        ""]   
keyHexA = 0x1
keyHexB =   0x1   
keyBinA = 0b1
keyBinB =    0b1  
keyBinC = 0b2
   keyNumberA = 1
keyNumberB =    1   
keyNumberC = 001
keyFloatA = 1.5
keyFloatB =    1.5   
keyFloatC = 01.5
   keyFloatD = .5
keyFloatE = 0.5
keyFloatF = 1.";

        string file = Environment.CurrentDirectory + '\\' + "TestIOConfigFile.conf";
        File.WriteAllText(file, content);

        ConfigFile config = new(file);

        // Print parsed token for debugging purposes.
        output.WriteLine(config.ToString());

        // Count keys
        Assert.Equal(19, config.Items.KeysCount);

        int index = 0;

        Assert.Equal(ConfigTokenType.Integer, config.Items.GetValueType(index));
        Assert.Equal("keyA", config.Items.GetKeyName(index));
        Assert.Equal("1", config.Items.GetValue(index));
        Assert.Equal("1", config.Items.GetValue("keyA"));

        // It should ignore left and right spaces
        index = 1;
        Assert.Equal("keyB", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("2.5", config.Items.GetValue(index));
        Assert.Equal("2.5", config.Items.GetValue("keyB"));

        // It should not trim right spaces before EOL, but left spaces are trimmed.
        index = 2;
        Assert.Equal("keyC", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.StringUnquoted, config.Items.GetValueType(index));
        Assert.Equal("This is a string value        ", config.Items.GetValue(index));
        Assert.Equal("This is a string value        ", config.Items.GetValue("keyC"));

        // It should not trim left and right spaces between [""], but spaces outside [""] are trimmed.
        index = 3;
        Assert.Equal("keyD", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.StringQuoted, config.Items.GetValueType(index));
        Assert.Equal("     This is a string value        ", config.Items.GetValue(index));
        Assert.Equal("     This is a string value        ", config.Items.GetValue("keyD"));

        index = 4;
        Assert.Equal("keyE", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.StringQuoted, config.Items.GetValueType(index));
        Assert.Equal(@"     This is a multi
line string value        ", config.Items.GetValue(index));
        Assert.Equal(@"     This is a multi
line string value        ", config.Items.GetValue("keyE"));

        // Hex values
        index = 5;
        Assert.Equal("keyHexA", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Hex, config.Items.GetValueType(index));
        Assert.Equal("0x1", config.Items.GetValue(index));
        Assert.Equal("0x1", config.Items.GetValue("keyHexA"));

        // Hex values. Trim spaces
        index = 6;
        Assert.Equal("keyHexB", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Hex, config.Items.GetValueType(index));
        Assert.Equal("0x1", config.Items.GetValue(index));
        Assert.Equal("0x1", config.Items.GetValue("keyHexB"));

        // Bin values
        index = 7;
        Assert.Equal("keyBinA", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Binary, config.Items.GetValueType(index));
        Assert.Equal("0b1", config.Items.GetValue(index));
        Assert.Equal("0b1", config.Items.GetValue("keyBinA"));

        // Trim spaces
        index = 8;
        Assert.Equal("keyBinB", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Binary, config.Items.GetValueType(index));
        Assert.Equal("0b1", config.Items.GetValue(index));
        Assert.Equal("0b1", config.Items.GetValue("keyBinB"));

        // 0b2 is not a binary, should be an unquoted string.
        index = 9;
        Assert.Equal("keyBinC", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.StringUnquoted, config.Items.GetValueType(index));
        Assert.Equal("0b2", config.Items.GetValue(index));
        Assert.Equal("0b2", config.Items.GetValue("keyBinC"));

        index = 10;
        Assert.Equal("keyNumberA", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Integer, config.Items.GetValueType(index));
        Assert.Equal("1", config.Items.GetValue(index));
        Assert.Equal("1", config.Items.GetValue("keyNumberA"));

        // Trim spaces
        index = 11;
        Assert.Equal("keyNumberB", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Integer, config.Items.GetValueType(index));
        Assert.Equal("1", config.Items.GetValue(index));
        Assert.Equal("1", config.Items.GetValue("keyNumberB"));

        index = 12;
        Assert.Equal("keyNumberC", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Integer, config.Items.GetValueType(index));
        Assert.Equal("001", config.Items.GetValue(index));
        Assert.Equal("001", config.Items.GetValue("keyNumberC"));

        index = 13;
        Assert.Equal("keyFloatA", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("1.5", config.Items.GetValue(index));
        Assert.Equal("1.5", config.Items.GetValue("keyFloatA"));

        // Trim spaces
        index = 14;
        Assert.Equal("keyFloatB", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("1.5", config.Items.GetValue(index));
        Assert.Equal("1.5", config.Items.GetValue("keyFloatB"));

        index = 15;
        Assert.Equal("keyFloatC", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("01.5", config.Items.GetValue(index));
        Assert.Equal("01.5", config.Items.GetValue("keyFloatC"));

        index = 16;
        Assert.Equal("keyFloatD", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal(".5", config.Items.GetValue(index));
        Assert.Equal(".5", config.Items.GetValue("keyFloatD"));

        index = 17;
        Assert.Equal("keyFloatE", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("0.5", config.Items.GetValue(index));
        Assert.Equal("0.5", config.Items.GetValue("keyFloatE"));

        // 1. is considered string_unquoted.
        index = 18;
        Assert.Equal("keyFloatF", config.Items.GetKeyName(index));
        Assert.Equal(ConfigTokenType.Decimal, config.Items.GetValueType(index));
        Assert.Equal("1.", config.Items.GetValue(index));
        Assert.Equal("1.", config.Items.GetValue("keyFloatF"));

        config.Save();

        string savedtext = File.ReadAllText(file);

        string expectedText = @"keyA = 1
keyB = 2.5
keyC = This is a string value        
keyD = [""     This is a string value        ""]
keyE = [""     This is a multi
line string value        ""]
keyHexA = 0x1
keyHexB = 0x1
keyBinA = 0b1
keyBinB = 0b1
keyBinC = 0b2
keyNumberA = 1
keyNumberB = 1
keyNumberC = 001
keyFloatA = 1.5
keyFloatB = 1.5
keyFloatC = 01.5
keyFloatD = .5
keyFloatE = 0.5
keyFloatF = 1.";

        Assert.Equal(expectedText, savedtext);
    }

    [Fact]
    public void ConfigFile_SetsNewValueForGivenKey_KeyStoresNewValue()
    {
        // Arrange
        // Create test configuration file.
        string content = @"keyA =    1   
keyB =     2.5       
   keyC =     This is a string value        ";

        string file = Environment.CurrentDirectory + '\\' + "TestIOConfigFile.conf";
        File.WriteAllText(file, content);

        // Act & assert
        ConfigFile config = new(file);
        Assert.Equal("1", config.Items.GetValue("keyA"));

        // Act & assert
        config.Items.Set("keyA", "2");
        Assert.Equal("2", config.Items.GetValue("keyA"));

        // Clean up
        File.Delete(file);
    }
}
