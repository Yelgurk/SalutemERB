using SalutemES.Engineer.Domain;
using System.Collections.Generic;
using System.Diagnostics;

namespace SalutemES.Engineer.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public void CallSQL()
        {
            DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS");

            DataBaseApi.ConnectionAvailable()
                ?.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                ?.CommandPrepeared()
                ?.ExecuteCommand(DataBaseResponseType.ListArray);

            List<string[]>? Response = DataBaseApi.ResponseArray;

            if (Response is not null)
                foreach (var arr in Response)
                {
                    foreach (string cell in arr)
                        Debug.Write($" {cell} |");
                    Debug.WriteLine("");
                }
        }
    }
}