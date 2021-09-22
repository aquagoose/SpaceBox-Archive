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
                        "Whoops! SpaceBox has crashed. Please see the error log below, and send the error to me on my Discord, or on my email ollie@ollierobinson.co.uk"
                }),
                new StackLayoutItem(new TextArea() { ReadOnly = true, Text = exception.ToString(), Size = new Size(500, 470)})
            });
        }
    }
}