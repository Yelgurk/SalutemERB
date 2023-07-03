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
using SalutemES.Engineer.Core;
using SalutemES.Engineer.Infrastructure;

namespace SalutemES.Engineer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public void CallSQL()
        {
            DataBaseApi.SetConnection("DESKTOP-J7PGA2A", "DB_SE_EngineerWS");
            Logger.SetLoggerPath(Environment.CurrentDirectory);

            FamilyViewModel FVM = new FamilyViewModel();

            /* case 1 */
            //FVM.FamilyNameFinder = "Пастеризаторы";

            //foreach (var x in FVM.FamilyModelCollection)
            //    Debug.WriteLine($"{x.Name} | {x.Count}");

            /* case 2 */
            FVM.FillCollection();

            foreach (var x in FVM.FamilyModelCollection)
                Debug.WriteLine($"{x.Name} | {x.Count}");

            /* case 3 */
            FVM.OnSelectedModelChanged = () => { Debug.WriteLine("Action called on model select"); };
            FVM.FamilyModelSelected = FVM.FamilyModelCollection[0];

            /* case 4 */
            FVM.OnSelectedModelChanged = null;
            FVM.FamilyModelSelected = FVM.FamilyModelCollection[1];
            
            /* case 5 */
            FVM.OnSelectedModelChanged = () => { Debug.WriteLine("Action called on model select against"); };
            FVM.FamilyModelSelected = FVM.FamilyModelCollection[1]; //not changed because similar to selected

            /* case 6 */
            FVM.FamilyModelSelected = FVM.FamilyModelCollection[2];

            Logger.WriteLine("I think executed successfully, idk, lets go test it");
        }
    }
}