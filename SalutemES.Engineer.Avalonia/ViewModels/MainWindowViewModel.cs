using SalutemES;
using SalutemES.Engineer.Domain;
using SalutemES.Engineer.Infrastructure.DataBase;
using static SalutemES.Engineer.Infrastructure.DataBase.DataBaseApi;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Diagnostics;
using System;
using System.Linq;
using SalutemES.Engineer.Core.ViewModels;

namespace SalutemES.Engineer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public void CallSQL()
        {
            DataBaseApi.SetConnection("DESKTOP-J7PGA2A", "DB_SE_EngineerWS");

            FamilyViewModel FamilyColl = new FamilyViewModel();
            FamilyColl.FillCollection();

            foreach (var x in FamilyColl.Families)
                Debug.WriteLine($"{x.Name} | {x.Count}");

            FamilyColl.FamilyName = "Пастеризаторы";

            foreach (var x in FamilyColl.Families)
                Debug.WriteLine($"{x.Name} | {x.Count}");
        }
    }
}