using System;
using System.ComponentModel;

namespace Utilities
{
    public partial class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void OnNotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }
    }

    public partial class ViewModelBase : IDisposable
    {
        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                {
                    //clear managed resources.
                }

                //clear unmanaged resources.
                mIsDisposed = true;
            }
        }

        ~ViewModelBase()
        {
            this.Dispose(false);
        }

        private bool mIsDisposed = false;
    }
}
