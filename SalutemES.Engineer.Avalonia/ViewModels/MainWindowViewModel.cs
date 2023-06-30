using SalutemES.Engineer.Domain;
using SalutemES.Engineer.Infrastructure.DataBase;
using static SalutemES.Engineer.Infrastructure.DataBase.DataBaseApi;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System;
using System.Linq;

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