using System.Windows;
using RenamingAssistance.VSIX.Common;

namespace RenamingAssistance.VSIX.Views
{
    public abstract class DialogWindowBase<TViewModel> : Window
    {
        public readonly TViewModel ViewModel;

        protected DialogWindowBase()
        {
            ViewModel = (TViewModel) CompositionUtil.ServiceProvider.GetService(typeof(TViewModel));
            DataContext = ViewModel;
        }
    }
}
