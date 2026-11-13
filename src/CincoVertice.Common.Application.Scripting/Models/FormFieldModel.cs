namespace CincoVertice.Common.Application.Scripting.Models;

/// <summary>
///     One box of dialog.form(): a text box, or a fixed list to pick from.
/// </summary>
/// <param name="Prompt">The label above the box.</param>
/// <param name="DefaultText">
///     The starting text of a text box; for a list, the item to select, or one that isn't there to select the first.
/// </param>
/// <param name="Choices">The items of a list, at least one; null for a text box.</param>
public sealed record FormFieldModel(string Prompt, string DefaultText, IReadOnlyList<string>? Choices = null);
