using System.Diagnostics;

namespace CityPlannerPharaoh;

public class SanelyScolledPanel : Panel
{
    public event Action<object, ZoomEventArgs>? Zoom;

    // This stops the Panel from reseting the scoll positions ALL THE TIME!
    protected override Point ScrollToControl(Control activeControl)
    {
        // Returning the current location prevents the panel from
        // scrolling to the active control when the panel loses and regains focus
        return this.DisplayRectangle.Location;
    }

    /// <summary>
    ///  Handles mouse wheel processing for our scrollbars.
    /// </summary>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
        if (ModifierKeys == Keys.Control)
        {
            //Debug.WriteLine($"~~~delta:{e.Delta}, clicks:{e.Clicks}");
            if (this.Zoom != null)
            {
                var args = new ZoomEventArgs { Delta = Math.Sign(e.Delta) };
                this.Zoom(this, args);
                ((HandledMouseEventArgs)e).Handled = true;
                return;
            }
        }
        // Favor the vertical scroll bar, since it's the most common use. However, if
        // there isn't a vertical scroll and the horizontal is on, then wheel it around.
        else if (VScroll && !(HScroll && ModifierKeys == Keys.Shift))
        {
            Rectangle client = ClientRectangle;
            int pos = -this.DisplayRectangle.Y;
            int maxPos = -(client.Height - this.DisplayRectangle.Height);

            pos = Math.Max(pos - e.Delta, 0);
            pos = Math.Min(pos, maxPos);

            AutoScrollPosition = new Point(-this.DisplayRectangle.X, pos);
            //SetDisplayRectLocation(this.DisplayRectangle.X, -pos);
            //SyncScrollbars(AutoScroll);
            if (e is HandledMouseEventArgs)
            {
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }
        else if (HScroll)
        {
            Rectangle client = ClientRectangle;
            int pos = -this.DisplayRectangle.X;
            int maxPos = -(client.Width - this.DisplayRectangle.Width);

            pos = Math.Max(pos - e.Delta, 0);
            pos = Math.Min(pos, maxPos);

            AutoScrollPosition = new Point(pos , -this.DisplayRectangle.Y);
            //SetDisplayRectLocation(-pos, this.DisplayRectangle.Y);
            //SyncScrollbars(AutoScroll);
            if (e is HandledMouseEventArgs)
            {
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        // The base implementation should be called before the implementation above,
        // but changing the order in Whidbey would be too much of a breaking change
        // for this particular class.
        //base.OnMouseWheel(e);
        //((MouseEventHandler)Events[s_mouseWheelEvent])?.Invoke(this, e);
    }
}
