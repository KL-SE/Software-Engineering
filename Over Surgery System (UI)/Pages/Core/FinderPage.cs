using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OverSurgerySystem.UI.Pages.Core
{
    public class FinderPage<T> : Page where T: new()
    {
        public static T         LastPrototype;
        public static List<T>   SearchResult = new List<T>();
        public static Action<T> OnSelect    { set; get; }
        public static Action<T> OnFind      { set; get; }
        public static Action<T> OnFound     { set; get; }
        public static Action    OnCancel    { set; get; }

        public static void OnInitialFind( T t ) { OnFind?.Invoke( t );      }
        public static void OnInitialCancel()    { OnCancel?.Invoke();       }
        public static void RowSelected( T t )   { OnSelect?.Invoke( t );    }

        public static void Reset()
        {
            LastPrototype = default(T);
        }

        public void Populate( StackPanel parent )
        {
            parent.Children.RemoveRange( 0 , parent.Children.Count );
            if( SearchResult.Count > 0 )
            {
                foreach( T t in SearchResult )
                {
                    if( t == null )
                        continue;

                    parent.Children.Add( GetRow( t ) );
                }
            }
            else
            {
                Border border   = new Border()      { Style = Resources["RowBorder"] as Style, Width = 1682                     };
                TextBlock text  = new TextBlock()   { Style = Resources["ShortRowText"] as Style, Text = "No results found."    };
                border.Child    = text;
                
                parent.Children.Add( border );
            }
        }

        public StackPanel GetRow( T t )
        {
            StackPanel rowPanel     = new StackPanel()  { Style = Resources["RowEffect"] as Style, Orientation = Orientation.Horizontal };
            Border firstColumn      = new Border()      { Style = Resources["RowBorder"] as Style, Width = 250                          };
            Border secondColumn     = new Border()      { Style = Resources["RowBorder"] as Style, Width = 450                          };
            Border thirdColumn      = new Border()      { Style = Resources["RowBorder"] as Style, Width = 750                          };
            Border forthColumn      = new Border()      { Style = Resources["RowBorder"] as Style, Width = 250                          };
            TextBlock firstText     = new TextBlock()   { Style = Resources["ShortRowText"] as Style                                    };
            TextBlock secondText    = new TextBlock()   { Style = Resources["LongRowText" ] as Style                                    };
            TextBlock thirdText     = new TextBlock()   { Style = Resources["LongRowText" ] as Style                                    };
            TextBlock forthText     = new TextBlock()   { Style = Resources["ShortRowText"] as Style                                    };

            rowPanel.Children.Add( firstColumn  );
            rowPanel.Children.Add( secondColumn );
            rowPanel.Children.Add( thirdColumn  );
            rowPanel.Children.Add( forthColumn  );

            firstColumn.Child   = firstText;
            secondColumn.Child  = secondText;
            thirdColumn.Child   = thirdText;
            forthColumn.Child   = forthText;

            string[] texts      = GetData( t );
            firstText.Text      = texts[0];
            secondText.Text     = texts[1];
            thirdText.Text      = texts[2];
            forthText.Text      = texts[3];

            rowPanel.MouseLeftButtonUp += ( object o , MouseButtonEventArgs e ) => { SetEventHandler(); RowSelected( t ); };

            return rowPanel;
        }

        public virtual string[] GetData( T t )  { return null;  }
        public virtual void SetEventHandler()   {               }
    }
}
