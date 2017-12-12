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
using OverSurgerySystem.UI.Persistent;
using OverSurgerySystem.UI.Pages.Staffs;

namespace OverSurgerySystem.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static readonly DependencyProperty ScaleValueProp    = DependencyProperty.Register( "ScaleValue" , typeof( double ) , typeof( MainWindow ) , new UIPropertyMetadata( 1.0 , new PropertyChangedCallback( OnScaleValueChanged ) , new CoerceValueCallback( OnCoerceScaleValue ) ) );
        public static readonly float[][] AspectRatioDefinition      = new float[][]
        {
            new float[] { 1920 , 1440 },
            new float[] { 1920 , 1080 }
        };

        private const int WM_EXITSIZEMOVE           = 0x232;
        public const int ASPECT_RATIO_4_3           = 0;
        public const int ASPECT_RATIO_16_9          = 1;
        public const double MIN_SCALE               = 0.5;
        public static float[] CURRENT_ASPECT_RATIO  = AspectRatioDefinition[ASPECT_RATIO_4_3];

        public static MainWindow instance;
        public static MenuBanner Instance { private set; get; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded              += OnLoad;
            MainPage.Navigating += App.PreventNavigation;
        }

        public void OnLoad( object sender , RoutedEventArgs e )
        {
            IntPtr handle       = new WindowInteropHelper( this ).Handle;
            HwndSource source   = HwndSource.FromHwnd( handle );

            source.AddHook( new HwndSourceHook( SizeChangeEnd ) );
            Banner.Navigate( MenuBanner.Instance );
            App.GoToLoginPage();
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

        public void CheckWindowState()
        {
            if( WindowState == WindowState.Normal )
            {
                WindowStyle                                     = WindowStyle.SingleBorderWindow;
                ResizeMode                                      = ResizeMode.CanResizeWithGrip;
                MenuBanner.Instance.ExitButton.Visibility       = Visibility.Collapsed;
                MenuBanner.Instance.MaximizeButton.Visibility   = Visibility.Collapsed;
                MenuBanner.Instance.MinimizeButton.Visibility   = Visibility.Collapsed;
            }
            else if( WindowState == WindowState.Maximized )
            {
                // The window seems to need to be collapsed and re-displayed to properly cover the taskbar in Windows 10.
                // Additionally, the resize mode must also be set to NoResize.
                Visibility                                      = Visibility.Collapsed;
                WindowStyle                                     = WindowStyle.None;
                ResizeMode                                      = ResizeMode.NoResize;
                MenuBanner.Instance.ExitButton.Visibility       = Visibility.Visible;
                MenuBanner.Instance.MaximizeButton.Visibility   = Visibility.Visible;
                MenuBanner.Instance.MinimizeButton.Visibility   = Visibility.Visible;
                Visibility                                      = Visibility.Visible;
            }
        }
        
        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );
            if( e.Property == Window.WindowStateProperty )
            {
                CheckWindowState();
            }
        }

        private static void OnScaleValueChanged( DependencyObject o , DependencyPropertyChangedEventArgs e ) { }
        private static object OnCoerceScaleValue( DependencyObject o , object value )
        {
            MainWindow window = o as MainWindow;
            if( window != null )
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
                return ( double )( GetValue( ScaleValueProp ) );
            }
            set
            {
                SetValue( ScaleValueProp , value );
            }
        }

        private void OnSizeChanged( object sender , EventArgs e )
        {
            double xScale   = ActualWidth / CURRENT_ASPECT_RATIO[0];
            double yScale   = ActualHeight / CURRENT_ASPECT_RATIO[1];
            double newScale = Math.Max( xScale , yScale );
            double scale    = Math.Max( MIN_SCALE , newScale );
            ScaleValue      = ( double )( OnCoerceScaleValue( MainWinObject , scale ) );
        }
    }
}
