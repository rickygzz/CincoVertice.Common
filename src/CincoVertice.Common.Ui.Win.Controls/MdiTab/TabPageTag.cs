namespace CincoVertice.Common.Ui.Win.Controls.MdiTab;

/// <summary>
/// Used for the TabPage tag, to hold more than one object.
/// </summary>
public class TabPageTag
{
    /// <summary>
    /// Gets or sets form linked to the TabPage.
    /// </summary>
    public Form OwnerForm { get; set; } = null!;

    /// <summary>
    /// Gets or sets menu linked to the TabPage.
    /// </summary>
    public ContextMenuStrip Menu { get; set; } = null!;
}
