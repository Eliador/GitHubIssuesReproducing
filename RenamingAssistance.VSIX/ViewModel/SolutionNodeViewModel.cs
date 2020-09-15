using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using RenamingAssistance.Core.CodeAnalysis;
using RenamingAssistance.VSIX.ViewModel.Common;

namespace RenamingAssistance.VSIX.ViewModel
{
    public class SolutionNodeViewModel : ViewModelBase
    {
        private readonly BitmapSource _expandedIconSource;
        private readonly BitmapSource _closedBitmapSource;
        private readonly SolutionNodeViewModel _parentNode;

        private bool _isExpanded;

        public SolutionNodeViewModel(
            string name,
            Bitmap expandedIcon,
            Bitmap closedIcon,
            SolutionNodeViewModel parentNode,
            DocumentChanges documentChanges = null)
        {
            Name = name;
            DocumentChanges = documentChanges;
            _parentNode = parentNode;
            
            _expandedIconSource = GetBitmapSource(expandedIcon);
            _closedBitmapSource = GetBitmapSource(closedIcon);           
        }

        public ObservableCollection<SolutionNodeViewModel> Folders { get; } = new ObservableCollection<SolutionNodeViewModel>();

        public string Name { get; }

        public DocumentChanges DocumentChanges { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                _isExpanded = value;
                RaisePropertyChanged(nameof(IsExpanded));
                RaisePropertyChanged(nameof(Icon));
            }
        }

        public BitmapSource Icon
        {
            get
            {
                if (DocumentChanges != null) { return _closedBitmapSource; }

                return IsExpanded ? _expandedIconSource : _closedBitmapSource;
            }
        }

        public ICollection<DocumentChanges> GetAllDocumentsToChange()
        {
            if (DocumentChanges != null)
            {
                return new [] { DocumentChanges };
            }

            return Folders.SelectMany(f => f.GetAllDocumentsToChange()).ToList();
        }

        private BitmapSource GetBitmapSource(Bitmap bitmap)
        {
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
