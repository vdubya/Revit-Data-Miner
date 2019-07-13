using System;
using System.Threading;
using System.Windows.Threading;

namespace Utilities
{
    /// <summary>
    /// Provides work with progress bar
    /// </summary>
    internal class ProgressBarHandler
    {
        #region Constants

        public const int DEFAULT_EXECUTED_COMMANDS_COUNT = 1;

        private const int DEFAULT_FIRST_COMMAND = 0;

        private const int DEFAULT_PERCENT = 100;

        #endregion

        #region Constructor

        private ProgressBarHandler()
        { }

        #endregion

        #region Properties

        /// <summary>
        /// The instance of ProgressBarHandler class
        /// </summary>
        public static ProgressBarHandler Instance
        {
            get
            {
                return mInstance;
            }
        }

        /// <summary>
        /// Gets or sets count of the executed commands
        /// </summary>
        public int ExecutedCommandsCount
        {
            get
            {
                return mExecutedCommandsCount;
            }
            set
            {
                mExecutedCommandsCount = value;
            }
        }

        public bool IsCanceled
        {
            get
            {
                return mViewModel.IsCanceled;
            }
        }

        public bool IsComplete
        {
            get
            {
                return mViewModel.IsComplete;
            }
            set
            {
                mViewModel.IsComplete = value;
            }
        }

        #endregion

        #region Public logic

        /// <summary>
        /// Shows the Progress dialog.
        /// </summary>
        public void Show(string title)
        {
            mViewModel.Clear();
            mViewModel.Caption = title;
            Thread progressBarThread = new Thread(Action);
            progressBarThread.SetApartmentState(ApartmentState.STA);
            progressBarThread.Start(mViewModel);
            Thread.Sleep(600);
        }

        /// <summary>
        /// Hides the Progress dialog.
        /// </summary>
        public void Close()
        {
            Thread.Sleep(400);

            if (mProgressDialog != null && mProgressDialog.IsVisible)
            {
                mProgressDialog.Dispatcher.Invoke(DispatcherPriority.Normal,
                    new Action(() =>
                    {
                        mProgressDialog.Close();
                    }));
            }
        }

        /// <summary>
        /// Clears dialog
        /// </summary>
        public void Clear()
        {
            mCurrentIteration = 0;
            mCurrentSubIteration = 0;
            mCurrentExecutedCommand = DEFAULT_FIRST_COMMAND;
            mExecutedCommandsCount = DEFAULT_EXECUTED_COMMANDS_COUNT;
            mMaxCount = 0;
            mMaxSubStepsCount = 0;
        }

        public bool Cancel()
        {
            return mViewModel.RequestCancellation();
        }

        /// <summary>
        /// Increases count of percents shown in ProgressBar
        /// </summary>
        /// <param name="count">Count of iterations at the current command</param>
        public void NextCommand(int count, string commandName)
        {
            mCurrentExecutedCommand++;
            mMaxCount += count;
            mCommandName = commandName;
            int perc = DEFAULT_PERCENT / mExecutedCommandsCount * mCurrentExecutedCommand;
            mCountOfStepsInOneProgress = mMaxCount / perc;
            mViewModel.MainProgressText = commandName;

            if (mCountOfStepsInOneProgress == 0)
                mCountOfStepsInOneProgress = 1;
        }

        /// <summary>
        /// Progresses main process on one step
        /// </summary>
        /// <param name="subStepsCount">The count of substeps</param>
        public void ProgressMainProcess(int subStepsCount, string text)
        {
            mCurrentIteration++;
            mMaxSubStepsCount = subStepsCount;
            mCurrentSubIteration = 0;

            if (mExecutedCommandsCount != 0)
                mViewModel.Progress = CountPercentage(mCurrentIteration, mMaxCount, DEFAULT_PERCENT / mExecutedCommandsCount * mCurrentExecutedCommand);

            mViewModel.MainProgressText = string.Format("{0}: {1}", mCommandName, text);
            mViewModel.SubProgressText = string.Empty;
            mViewModel.SubProgress = mCurrentSubIteration;
            Thread.Sleep(40);

            if (mCurrentIteration > mMaxCount)
            {
                Close();
                Clear();
            }
        }

        /// <summary>
        /// Progresses sub process
        /// </summary>
        public void ProgressSubProcess(string text)
        {
            ProgressSubProcess(text, 1);
        }

        /// <summary>
        /// Progresses sub process
        /// </summary>
        public void ProgressSubProcess(string text, int count)
        {
            if (mCurrentSubIteration > mMaxSubStepsCount)
                return;

            mCurrentSubIteration += count;
            int ost = 1;

            if (mCountOfStepsInOneProgress != 1)
                Math.DivRem(mCurrentIteration, mCountOfStepsInOneProgress, out ost);

            if (ost == 0)
                ost = mCountOfStepsInOneProgress;

            mViewModel.SubProgress = (int)((ost - 1) * (DEFAULT_PERCENT / (double)mCountOfStepsInOneProgress) + DEFAULT_PERCENT / (double)mCountOfStepsInOneProgress / (double)mMaxSubStepsCount * mCurrentSubIteration);
            mViewModel.SubProgressText = text;

            Thread.Sleep(40);
        }

        #endregion

        #region Private logic

        private void Action(object viewModel)
        {
            mProgressDialog = new ProgressDialogView();
            mProgressDialog.DataContext = (ProgressBarViewModel)viewModel;
            mProgressDialog.ShowDialog();
        }

        private int CountPercentage(int x, int max, int perc)
        {
            if (max < 1)
                return 0;

            return (int)(x / (double)max * perc);
        }

        #endregion

        #region Private fields

        private int mMaxCount = default(int);

        private int mMaxSubStepsCount = default(int);

        private int mCurrentIteration = default(int);

        private int mCurrentSubIteration = default(int);

        private int mCurrentExecutedCommand = DEFAULT_FIRST_COMMAND;

        private int mExecutedCommandsCount = DEFAULT_EXECUTED_COMMANDS_COUNT;

        private int mCountOfStepsInOneProgress = 1;

        private ProgressBarViewModel mViewModel = new ProgressBarViewModel();

        private ProgressDialogView mProgressDialog = null;

        private string mCommandName = string.Empty;

        private static readonly ProgressBarHandler mInstance = new ProgressBarHandler();

        #endregion
    }
}