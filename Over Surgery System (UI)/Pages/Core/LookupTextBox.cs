using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OverSurgerySystem.UI.Pages.Core
{
    public class LookupTextBox : TextBox
    {
        public event EventHandler StoppedTyping = delegate { };
        public int TimeToIdle   { set; get; }
        private Timer WaitTimer { set; get; }

        public LookupTextBox() : base()
        {
            TimeToIdle  = 500;
            WaitTimer   = new Timer( e => Dispatcher.Invoke( () => StoppedTyping( this , EventArgs.Empty ) ) );
        }

        protected override void OnTextChanged( TextChangedEventArgs e )
        {
            base.OnTextChanged( e );
            WaitTimer.Change( TimeToIdle , Timeout.Infinite );
        }
    }
}
