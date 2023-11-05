using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using SalutemES.Engineer.Avalonia.ViewModels;
using System.Diagnostics;
using SalutemES.Engineer.Avalonia.Extra;

namespace SalutemES.Engineer.Avalonia.Views
{
    public partial class MainWindow : Window
    {
        private Key[] KeyLog = new Key[2] { Key.A, Key.A };
        private KeyCombination KeyComb = KeyCombination.NONE;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            this.KeyDown += (s, e) => {
                KeyLog[0] = KeyLog[1];
                KeyLog[1] = e.Key;

                if (KeyLog[0] == Key.LeftCtrl && KeyLog[1] == Key.F)
                    KeyComb = KeyCombination.CTRL_F;

                if (KeyComb > 0 && MainWindowContentPanel.Content is IHotKeyHandler)
                {
                    (MainWindowContentPanel.Content as IHotKeyHandler)!.HotkeyWorked(KeyComb);
                    KeyComb = KeyCombination.NONE;
                }
            };
        }

        public void SetContent<T>(T Control) => MainWindowContentPanel.Content = Control;

        public MainWindowViewModel ViewModel => (this.DataContext as MainWindowViewModel)!;
    }
}