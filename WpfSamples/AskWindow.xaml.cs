using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfSamples
{
    public class AskException : ApplicationException
    {
        public AskException(string msg) : base(msg) { }
    }

    /// <summary>
    /// Interaction logic for Permission.xaml
    /// </summary>
    public partial class AskWindow : Window
    {
        private string _choice;


        public static string Ask()
        {
            var p = new AskWindow();
            // when the dialog closes, return the text 'choice'
            string choice = "";
            bool closed = false;
            p.Closed += (o,e) => { 
                choice = p._choice;
                closed = true;
            };
            // dont show as a modal, allow rest of UI to keep on functioning!
            p.Show();
            // now wait until user makes his choice... while !closed thread.sleep.... eeeek!
            // ideally what you want is ... on event X... return Y!
            // i.e. Puppet Task pattern....


            // for now, we'll implement ...continue with pattern
            return choice;
        }

        // I think this will run the 'continue' with on the non UI thread...mmm!??
        // how do I demonstrate what problems this will cause?
        public static void Ask(Action<string> continueWith)
        {
            var p = new AskWindow();
            p.Closed += (o, e) =>
            {
                continueWith(p._choice);
            };

            try
            {
                p.Show();
            }
            catch (Exception)
            {
                // there's no safe way for me to notify the caller about this exception other 
                // than to let this 'bubble' up ???
                throw;
            }

            
        }

        public static Task<string> AskWithTask()
        {
            var completion = new TaskCompletionSource<string>();

            var p = new AskWindow();
            p.Closed += (o, e) =>
            {
                completion.SetResult(p._choice);
            };

            p.Show();
            return completion.Task;
        }


        public AskWindow()
        {
            InitializeComponent();
        }

        private void btOK_OnClick(object sender, RoutedEventArgs e)
        {
            _choice = "OK";
            Close();
        }


        private void btCancel_OnClick(object sender, RoutedEventArgs e)
        {
            _choice = "CANCEL";
            Close();
        }

        private void btThrow_OnClick(object sender, RoutedEventArgs e)
        {
            // we'll get to this in due course! see later chapters...

            //var ex = new AskException("Booowhaaam!");
            //throw ex;
            //Close();
        }
    }
}
