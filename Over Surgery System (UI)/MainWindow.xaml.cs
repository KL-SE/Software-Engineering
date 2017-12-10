using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OverSurgerySystem.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register( "ScaleValue" , typeof(double) , typeof(MainWindow) , new UIPropertyMetadata( 1.0 , new PropertyChangedCallback(OnScaleValueChanged) , new CoerceValueCallback(OnCoerceScaleValue) ) );

        private const int WM_EXITSIZEMOVE                       = 0x232;
        public const int ASPECT_RATIO_4_3                       = 0;
        public const int ASPECT_RATIO_16_9                      = 1;
        public const double MIN_SCALE                           = 0.5;
        public static readonly float[][] AspectRatioDefinition  = new float[][]
        {
            new float[] { 1920 , 1440 },
            new float[] { 1920 , 1080 }
        };

        public static float[] CURRENT_ASPECT_RATIO = AspectRatioDefinition[ASPECT_RATIO_16_9];

        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnLoad;
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            MainPage.Source = new Uri( "Pages/LoginPage.xaml" , UriKind.Relative );

            HwndSource source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
            source.AddHook( new HwndSourceHook( SizeChangeEnd ) );
        }

        private IntPtr SizeChangeEnd( IntPtr hwnd , int msg , IntPtr wParam , IntPtr lParam , ref bool handled )
        {
            if( msg == WM_EXITSIZEMOVE )
            {
                double calcWidth = ScaleValue * CURRENT_ASPECT_RATIO[0];
                if( calcWidth < Width )
                    Width = calcWidth;
            }

            return IntPtr.Zero;
        }

        private static void OnScaleValueChanged( DependencyObject o , DependencyPropertyChangedEventArgs e ) { }
        private static object OnCoerceScaleValue( DependencyObject o , object value )
        {
            MainWindow mainWindow = o as MainWindow;
            if( mainWindow != null )
            {
                double scale = ( double ) value; 
                if( double.IsNaN( scale ) )
                    return 1.0f;
                
                return Math.Max( 0.1 , scale );
            }
            else
            {
                return value;
            }
        }

        public double ScaleValue
        {
            get
            {
                return (double) GetValue( ScaleValueProperty );
            }
            set
            {
                SetValue( ScaleValueProperty , value );
            }
        }

        private WindowState CurrentWindowState { set; get; }

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged(e);
            if( e.Property == Window.WindowStateProperty )
                CurrentWindowState = ( WindowState )( e.NewValue );
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            double xScale   = ActualWidth / CURRENT_ASPECT_RATIO[0];
            double yScale   = ActualHeight / CURRENT_ASPECT_RATIO[1];
            double newScale = ( CurrentWindowState == WindowState.Maximized ) ? Math.Max( xScale , yScale ) : Math.Min( xScale , yScale );
            double scale    = Math.Max( MIN_SCALE , newScale );
            ScaleValue      = ( double ) OnCoerceScaleValue( MainWinObject , scale );
        }
    }
}
