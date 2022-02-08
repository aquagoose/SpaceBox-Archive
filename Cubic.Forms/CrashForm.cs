using System;
using Eto.Drawing;
using Eto.Forms;

namespace Cubic.Forms
{
    public class CrashForm : Form
    {
        public CrashForm(Exception exception)
        {
            ClientSize = new Size(500, 500);
            Title = "SpaceBox";

            Content = new StackLayout(new[]
            {
                new StackLayoutItem(new Label()
                {
                    Text =
                        "Whoops! SpaceBox has crashed. Please see the error log below, and, if you wish, create an issue on the GitHub. However this issue is unlikely to be fixed, it is more for the interest of the developer."
                }),
                new StackLayoutItem(new TextArea() { ReadOnly = true, Text = exception.ToString(), Size = new Size(500, 470)})
            });
        }
    }
}