using System.Windows;
using System.Windows.Interop;

namespace Utilities
{
    /// <summary>
    /// Interaction logic for ProgressBar.xaml
    /// </summary>
    public partial class ProgressDialogView : Window
    {
        public ProgressDialogView()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ProgressBarHandler.Instance.IsComplete)
            {
                var shouldCancel = ProgressBarHandler.Instance.Cancel();

                if (!shouldCancel)
                {
                    e.Cancel = true;
                }
            }
        }
        
		void Button_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
    }
}
