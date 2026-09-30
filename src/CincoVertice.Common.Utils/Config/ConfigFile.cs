namespace CincoVertice.Common.Utils.Config;

public class ConfigFile
{
    private string _filename = string.Empty;

    private ConfigLexer _lexer;

    public ConfigFile(string filename)
    {
        _lexer = new ConfigLexer();

        Filename = filename;
    }

    public ConfigItems Items
    {
        get
        {
            return _lexer.Items;
        }
    }

    /// <summary>
    /// Gets or sets Filename and sets lexer text.
    /// </summary>
    public string Filename
    {
        get
        {
            return _filename;
        }

        set
        {
            try
            {
                _filename = value;

                string text = File.ReadAllText(value);

                _lexer.Text = text;
            }
            catch (Exception)
            {
                if (_lexer.Text == null)
                {
                    _lexer.Text = string.Empty;
                }

                // MC.Message.Error("Filename(): " + value, ex.Message);
            }
        }
    }

    /// <summary>
    /// Save to Filename.
    /// </summary>
    public void Save()
    {
        try
        {
            File.WriteAllText(Filename, _lexer.TokensToText());
        }
        catch (Exception)
        {
            // MC.Message.Error("ConfigFile.Save()", ex.Message);
        }
    }

    /// <summary>
    /// Save to Filename.
    /// </summary>
    /// <returns>True if successful, otherwise, false.</returns>
    public bool SaveAs(string newFilename)
    {
        try
        {
            File.WriteAllText(newFilename, _lexer.TokensToText());

            return true;
        }
        catch (Exception)
        {
            // MC.Message.Error("ConfigFile.SaveAs()", ex.Message);
        }

        return false;
    }

    public override string ToString()
    {
        return _lexer.TokensToText();
    }
}
