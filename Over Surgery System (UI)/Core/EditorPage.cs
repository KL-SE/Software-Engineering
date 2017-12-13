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

        public static int View          = 1;
        public static int Edit          = 2;
        public static int Find          = 4;
        public static int Restricted    = 8;
        public static int BackOnly      = 16;
        public static int NoBack        = 32;

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

        public static bool IsBackOnly
        {
            get
            {
                return ( Mode & BackOnly ) == BackOnly;
            }
        }

        public static bool IsNoBack
        {
            get
            {
                return ( Mode & NoBack ) == NoBack;
            }
        }

        public static void Setup( T item , int mode )
        {
            CurrentItem = item;
            Mode        = mode;
        }

        public static void Reset()
        {
            Mode = 0;
        }
    }
}
