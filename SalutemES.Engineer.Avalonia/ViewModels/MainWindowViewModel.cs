using Avalonia;
using SalutemES.Engineer.Infrastructure;
using SalutemES.Engineer.Infrastructure.DataBase;
using System.Collections.Generic;
using System.Diagnostics;

namespace SalutemES.Engineer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public void CallSQL()
        {

        }
    }
}