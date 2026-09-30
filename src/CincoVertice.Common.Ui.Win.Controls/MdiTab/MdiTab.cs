using System.ComponentModel;

namespace CincoVertice.Common.Ui.Win.Controls.MdiTab;

public class MdiTab : TabControl
{
    /// <summary>
    /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or
    /// to prevent changes in that window from being redrawn.
    /// wParam   The redraw state.If this parameter is TRUE, the content can be redrawn after a change.
    ///          If this parameter is FALSE, the content cannot be redrawn after a change.
    /// </summary>
    private const int WMSETREDRAW = 11;

    private Point dragStartPosition = Point.Empty;

    private readonly Brush brushMain = new SolidBrush(Color.FromArgb(132, 150, 176));

    // private Brush brushTaxCertificates = new SolidBrush(Color.FromArgb(189, 215, 238));
    // private Brush brushOrganization = new SolidBrush(Color.FromArgb(254, 229, 153));
    // private Brush brushScreens = new SolidBrush(Color.FromArgb(197, 224, 179));
    // private Brush brushInvoice = new SolidBrush(Color.FromArgb(112, 173, 71));
    // private Brush brushProduct = new SolidBrush(Color.FromArgb(146, 208, 80));
    // private Brush brushDarkText = new SolidBrush(Color.FromArgb(34, 42, 53));
    private readonly Brush brushLightText = new SolidBrush(Color.FromArgb(255, 255, 255));

    /// <summary>
    /// Initializes a new instance of the <see cref="MDITab"/> class.
    /// </summary>
    public MdiTab()
    {
        // Set initial TabControl settings
        Appearance = TabAppearance.Buttons;
        Dock = DockStyle.Top;
        AllowDrop = true;
        DrawMode = TabDrawMode.OwnerDrawFixed;
        Padding = new Point(18, 3);
        ItemSize = new Size(0, 30);
        MaximumSize = new Size(99999, 32);
        Size = new Size(10, 31);
        Visible = true;

        MouseDown += new MouseEventHandler(CtlTabs_MouseDown!);
        MouseMove += new MouseEventHandler(CtlTabs_MouseMove!);
        DragOver += new DragEventHandler(CtlTabs_DragOver!);

        SelectedIndexChanged += new EventHandler(CtlTabs_SelectedIndexChanged!);
        DrawItem += new DrawItemEventHandler(CtlTabs_DrawItem!);
        MouseClick += new MouseEventHandler(CtlTabs_MouseClick!);

        // To add MdiChildActivate event handler, we need the parent handle set at ParentChanged
        ParentChanged += new EventHandler(CtlTabs_ParentChanged!);
    }

    /// <summary>
    /// Gets or sets size property.
    /// </summary>
    [DefaultValue(typeof(Size), "10, 31")]
    public new Size Size
    {
        get { return base.Size; }
        set { base.Size = new Size(value.Width, 31); }
    }

    /// <summary>
    /// Gets or sets MaximumSize property
    /// </summary>
    [DefaultValue(typeof(Size), "99999, 32")]
    public new Size MaximumSize
    {
        get { return base.Size; }
        set { base.Size = new Size(value.Width, 32); }
    }

    /// <summary>
    /// Sends the specified message to a window or windows. The SendMessage function calls the window procedure
    /// for the specified window and does not return until the window procedure has processed the message.
    /// </summary>
    /// <param name="hWnd">Window whose window procedure will receive the message.</param>
    /// <param name="wMsg">Message to be sent</param>
    /// <param name="wParam">wParam Additional message-specific information.</param>
    /// <param name="lParam">lParam Additional message-specific information.</param>
    /// <returns>
    ///     The return value specifies the result of the message processing; it depends on the message sent.
    /// </returns>
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int SendMessage(nint hWnd, int wMsg, bool wParam, int lParam);

    private void CtlTabs_ParentChanged(object sender, EventArgs e)
    {
        // As soon as the parent is set, wire the MdiChildActivate event handler
        ((Form)Parent!).MdiChildActivate += new EventHandler(ParentForm_MdiChildActivate!);
    }

    private void CtlTabs_MouseClick(object sender, MouseEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        if (SelectedIndex != -1)
        {
            Rectangle rectTab = GetTabRect(SelectedIndex);

            if (SelectedIndex != 0)
            {
                // Check if close icon was pressed
                Rectangle rectClose = new Rectangle(
                    rectTab.X + rectTab.Width - Padding.X - 5,
                    rectTab.Y,
                    Padding.X + 5,
                    rectTab.Height);

                if (rectClose.Contains(e.Location))
                {
                    ((TabPageTag)SelectedTab?.Tag!).OwnerForm.Close();
                }
            }

            if (rectTab.Contains(e.Location)
                && SelectedTab != null
                && ((TabPageTag)SelectedTab.Tag!).Menu != null) // && e.Button == MouseButtons.Right)
            {
                Rectangle rect = GetTabRect(TabPages.IndexOf(SelectedTab));
                Point pt = PointToScreen(new Point(rect.X, rect.Y + rect.Height));

                ((TabPageTag)SelectedTab.Tag).Menu.Show(pt);
            }
        }
    }

    private void CtlTabs_MouseDown(object sender, MouseEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        TabPage tp = HoverTab();
        if (tp != null)
        {
            Rectangle rectTab = GetTabRect(TabPages.IndexOf(tp));

            Rectangle rectClose = new Rectangle(
                rectTab.X + rectTab.Width - Padding.X,
                rectTab.Y,
                Padding.X,
                rectTab.Height);

            if (!rectClose.Contains(e.Location))
            {
                dragStartPosition = new Point(e.X, e.Y);
            }
        }
        else
        {
            dragStartPosition = Point.Empty;
        }
    }

    private void CtlTabs_MouseMove(object sender, MouseEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        if (e.Button != MouseButtons.Left || dragStartPosition == Point.Empty)
        {
            return;
        }

        // When moving the mouse while holding the left button
        // Get the hovered tab
        TabPage tp = HoverTab();

        if (tp != null
            && (Math.Abs(e.X - dragStartPosition.X) > 6 || Math.Abs(e.Y - dragStartPosition.Y) > 6))
        {
            // Start dragging the clicked TabPage
            DoDragDrop(tp, DragDropEffects.All);
            dragStartPosition = Point.Empty;
        }
    }

    private void CtlTabs_DragOver(object sender, DragEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        // Get the tab it is dragging over to
        TabPage hoverTab = HoverTab();

        if (hoverTab == null)
        {
            e.Effect = DragDropEffects.None;
        }
        else
        {
            if (e.Data!.GetDataPresent(typeof(TabPage)))
            {
                e.Effect = DragDropEffects.Move;
                TabPage dragTab = (TabPage)e.Data!.GetData(typeof(TabPage))!;

                if (hoverTab == dragTab)
                {
                    return;
                }

                Rectangle tabRect = GetTabRect(TabPages.IndexOf(hoverTab));
                tabRect.Inflate(-3, -3);
                if (tabRect.Contains(PointToClient(new Point(e.X, e.Y))))
                {
                    SwapTabPages(dragTab, hoverTab);
                    SelectedTab = dragTab;
                }
            }
        }
    }

    /// <summary>
    /// Check if cursor position is within a TabPage area
    /// TabIndex = 0 is omitted
    /// </summary>
    /// <returns>Hovered TabPage</returns>
    private TabPage HoverTab()
    {
        for (int index = 1; index <= TabCount - 1; index++)
        {
            if (GetTabRect(index).Contains(PointToClient(Cursor.Position)))
            {
                // Cursor position is within this tabpage
                return TabPages[index];
            }
        }

        return null!;
    }

    private void SwapTabPages(TabPage tp1, TabPage tp2)
    {
        int index1 = TabPages.IndexOf(tp1);
        int index2 = TabPages.IndexOf(tp2);

        TabPages[index1] = tp2;
        TabPages[index2] = tp1;
    }

    /// <summary>
    /// Occurs when a multiple-document interface (MDI) child form is activated or closed within an MDI application.
    /// </summary>
    /// <param name="sender">sender</param>
    /// <param name="e">e</param>
    private void ParentForm_MdiChildActivate(object sender, EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        Form parent = (Parent as Form)!;

        if (parent.ActiveMdiChild == null)
        {
            // There is no MDI Child. Hide the tab control
            Visible = false;
        }
        else
        {
            // Always maximize the child forms
            parent.ActiveMdiChild.WindowState = FormWindowState.Maximized;

            // If the child form is new and it has no tab page, create one
            if (parent.ActiveMdiChild.Tag == null)
            {
                Form tabForm = parent.ActiveMdiChild;

                TabPage tp = new TabPage(tabForm.Text);

                // Tab page points to the MDI Child
                tp.Tag = new TabPageTag() { OwnerForm = tabForm, Menu = tabForm.ContextMenuStrip! };
                tabForm.ContextMenuStrip = null;

                tp.Parent = this;

                // MDI Child tag property points to the tabpage, used on FormClosed
                tabForm.Tag = tp;

                // To dispose the tab in case the form is closed programmaticaly
                tabForm.FormClosed += new FormClosedEventHandler(ActiveMdiChild_FormClosed!);

                // To update the tab text in case the form text is updated after the tab is created.
                tabForm.TextChanged += new EventHandler(ActiveMdiChild_Text!);

                SelectedTab = tp;
            }
            else
            {
                SelectedTab = (TabPage)parent.ActiveMdiChild.Tag;
            }

            // Make tabs visible since it is not empty
            if (!Visible)
            {
                Visible = true;
            }
        }
    }

    private void ActiveMdiChild_Text(object sender, EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        Form form = (sender as Form)!;

        if (form!.Tag != null)
        {
            (form.Tag as TabPage)!.Text = form.Text;
        }
    }

    private void ActiveMdiChild_FormClosed(object sender, FormClosedEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        // Destroy the corresponding tab pages
        ((sender as Form)!.Tag as TabPage)?.Dispose();
    }

    private void CtlTabs_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        if (SelectedTab != null && SelectedTab.Tag != null)
        {
            // SETREDRAW = false
            SendMessage(Handle, WMSETREDRAW, false, 0);

            ((TabPageTag)SelectedTab.Tag).OwnerForm.Select();

            // SETREDRAW = true
            SendMessage(Handle, WMSETREDRAW, true, 0);

            // To decide whether using Invalidate(true) or this.Update() read
            // https://blogs.msdn.microsoft.com/subhagpo/2005/02/22/whats-the-difference-between-control-invalidate-control-update-and-control-refresh/
            Refresh();
        }
    }

    private void CtlTabs_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (DesignMode)
        {
            return;
        }

        Brush brushBackground;
        Brush brushText;

        TabPage page = TabPages[e.Index];

        Rectangle paddedBounds = e.Bounds;
        int yOffset = e.State == DrawItemState.Selected ? -1 : 1;
        yOffset += paddedBounds.Height / 2 - (int)(e.Font!.GetHeight() / 2);
        paddedBounds.Offset(1, yOffset);

        if (e.Index == 0)
        {
            paddedBounds.Offset(14, 0);

            brushBackground = brushMain;
            brushText = brushLightText;

            e.Graphics.FillRectangle(brushBackground, e.Bounds);
        }
        else
        {
            brushBackground = new SolidBrush(page.BackColor);
            brushText = new SolidBrush(page.ForeColor);

            e.Graphics.FillRectangle(brushBackground, e.Bounds);

            // Adjust tabcontrol.padding width to allocate space for the image
            // TextRenderer.DrawText(e.Graphics, page.Text, this.TabForms.Font, paddedBounds, page.ForeColor);
            //e.Graphics.DrawImage(Properties.Resources.Close16, e.Bounds.Right - 21, 7);
        }

        // if (e.Index == 0)
        // {
        //    brushBackground = this.brushMain;

        //    brushText = this.brushLightText;

        //    paddedBounds.Offset(14, 0);

        //    e.Graphics.FillRectangle(brushBackground, e.Bounds);
        // }
        // else
        // {
        //    Form f = ((TabPageTag)page.Tag).OwnerForm;

        //    if (f.GetType() == typeof(FrmOrganization))
        //    {
        //        brushBackground = this.brushOrganization;
        //    }
        //    else if (f.GetType() == typeof(FrmTaxCertificates))
        //    {
        //        brushBackground = this.brushTaxCertificates;
        //    }
        //    else if (f.GetType() == typeof(FrmScreens))
        //    {
        //        brushBackground = this.brushScreens;
        //    }
        //    else if (f.GetType() == typeof(FrmInvoice))
        //    {
        //        brushText = this.brushLightText;
        //        brushBackground = this.brushInvoice;
        //    }
        //    else if (f.GetType() == typeof(FrmInvoiceCatalog))
        //    {
        //        brushText = this.brushLightText;
        //        brushBackground = this.brushInvoice;
        //    }
        //    else if (f.GetType() == typeof(FrmProduct))
        //    {
        //        brushBackground = this.brushProduct;
        //    }

        //    e.Graphics.FillRectangle(brushBackground, e.Bounds);

        //    // Adjust tabcontrol.padding width to allocate space for the image
        //    // TextRenderer.DrawText(e.Graphics, page.Text, this.TabForms.Font, paddedBounds, page.ForeColor);
        //    e.Graphics.DrawImage(Properties.Resources.Close16, e.Bounds.Right - 21, 7);
        // }

        e.Graphics.DrawString(page.Text, e.Font, brushText, paddedBounds);
    }

    /// <summary>
    /// OpenMDI. Uses Generics.
    /// </summary>
    /// <typeparam name="T">Form </typeparam>
    /// <param name="multipleInstances">If form is open, then close</param>
    /// <returns>Returns the open or new form</returns>
    public Form OpenMDI<T>(bool multipleInstances)
        where T : Form, new()
    {
        Form parentForm = (Parent as Form)!;

        if (multipleInstances)
        {
            var openMdiChilden = (Parent as Form)?.MdiChildren.OfType<T>().FirstOrDefault();

            if (openMdiChilden != null)
            {
                if (openMdiChilden.WindowState == FormWindowState.Minimized)
                {
                    openMdiChilden.WindowState = FormWindowState.Maximized;
                }

                openMdiChilden.Activate();
                return openMdiChilden;
            }
        }

        // Create nform
        T newForm = new()
        {
            MdiParent = parentForm
        };
        newForm.Show();

        return newForm;
    }
}
