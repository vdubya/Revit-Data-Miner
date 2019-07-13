using System.Windows;
using System.Windows.Input;

namespace Utilities
{
    public class ProgressBarViewModel : ViewModelBase
    {
        #region Constants

        private const int DEFAULT_VALUE = 0;

        private const bool DEFAULT_VALUE_CANCELED = false;

        private const bool DEFAULT_VALUE_COMPLETE = false;

        #endregion
        
        public ProgressBarViewModel()
        {}

        #region Properties

        /// <summary>
        /// Gets or sets progress of an processing job.
        /// </summary>
        public int Progress
        {
            get
            {
                return m_Progress;
            }
            set
            {
                m_Progress = value;
                NotifyPropertyChanged("Progress");
            }
        }

        /// <summary>
        /// Gets or sets progress of an processing job.
        /// </summary>
        public int SubProgress
        {
            get
            {
                return m_SubProgress;
            }
            set
            {
                m_SubProgress = value;
                NotifyPropertyChanged("SubProgress");
            }
        }

        /// <summary>
        /// The text of the main progress
        /// </summary>
        public string MainProgressText
        {
            get
            {
                return m_MainProgressText;
            }
            set
            {
                m_MainProgressText = value;
                NotifyPropertyChanged("MainProgressText");
            }
        }

        /// <summary>
        /// The text of the sub progress
        /// </summary>
        public string SubProgressText
        {
            get
            {
                return m_SubProgressText;
            }
            set
            {
                m_SubProgressText = value;
                NotifyPropertyChanged("SubProgressText");
            }
        }

        /// <summary>
        /// Gets or sets progress bar 
        /// </summary>
        public string Caption
        {
            get { return m_Caption; }
            set { m_Caption = value; }
        }

        public bool IsCanceled
        {
            get
            {
                return m_IsCanceled;
            }
        }

        public bool IsComplete
        {
            get
            {
                return m_IsComplete;
            }
            set
            {
                m_IsComplete = value;
            }
        }

        #endregion

        #region Public logic

        /// <summary>
        /// Clears progress bar value
        /// </summary>
        public void Clear()
        {
            m_Progress = DEFAULT_VALUE;
            m_SubProgress = DEFAULT_VALUE;
            m_IsCanceled = DEFAULT_VALUE_CANCELED;
            m_IsComplete = DEFAULT_VALUE_COMPLETE;
        }

        #endregion

        #region Private logic

        private void CancelExecution()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to cancel the process?", "Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (messageBoxResult == MessageBoxResult.Yes)
                m_IsCanceled = true;
        }

        private bool canCancelExecution()
        {
            return !m_IsCanceled;
        }

        public bool RequestCancellation()
        {
            CancelExecution();

            return m_IsCanceled;
        }

        #endregion

        #region Private fields

        private int m_Progress = DEFAULT_VALUE;

        private int m_SubProgress = DEFAULT_VALUE;

        private string m_MainProgressText = string.Empty;

        private string m_SubProgressText = string.Empty;

        private string m_Caption = string.Empty;

        private bool m_IsCanceled = false;

        private bool m_IsComplete = false;
        
        #endregion
    }
}