using CincoVertice.Common.Utils.Lexer;

namespace CincoVertice.Common.Utils.Config.Models;

public class ConfigItemModel
{
    public GenericToken Key = new();
    public GenericToken Value = new();
}
