using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace OverSurgerySystem.UI.Pages.Core
{
    public class EditorPage<T> : Page where T: new()
    {
        public static Action<T> OnConfirm   { set; get; }
        public static Action    OnCancel    { set; get; }
        public static Action    OnNavigate  { set; get; }
        public static Action    OnReturn    { set; get; }

        public static int View          = 0b0001;
        public static int Edit          = 0b0010;
        public static int Find          = 0b0100;
        public static int Restricted    = 0b1000;

        public static T CurrentItem { set; get; }
        public static int Mode      { set; get; }
        
        public static bool IsView
        {
            get
            {
                return( Mode & View ) == View;
            }
        }

        public static bool IsEdit
        {
            get
            {
                return ( Mode & Edit ) == Edit;
            }
        }

        public static bool IsFind
        {
            get
            {
                return ( Mode & Find ) == Find;
            }
        }

        public static bool IsRestricted
        {
            get
            {
                return ( Mode & Restricted ) == Restricted;
            }
        }

        public static void Setup( T item , int mode )
        {
            CurrentItem = item;
            Mode        = mode;
        }
    }
}
