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
            List<string[]>? Response;

            // example #1
            DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS");

            Response =
                DataBaseApi.ConnectionAvailable()
                ?.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                ?.ExecuteCommand<List<string[]>>()
                ?.DataBaseResponse<List<string[]>>();

            if (Response is not null)
                foreach (var arr in Response)
                {
                    foreach (string cell in arr)
                        Debug.Write($" {cell} |");
                    Debug.WriteLine("");
                }



            // example #2
            Response =
                DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
                ?.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                ?.ExecuteCommand<List<string[]>>()
                ?.DataBaseResponse<List<string[]>>();

            if (Response is not null)
                foreach (var arr in Response)
                {
                    foreach (string cell in arr)
                        Debug.Write($" {cell} |");
                    Debug.WriteLine("");
                }



            // example #3
            DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
                ?.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы");

            /* somewhere in code, maybe in another one class */
            DataBaseApi.CommandPrepeared()
                ?.ExecuteCommand<List<string[]>>()
                ?.DataBaseResponse<List<string[]>>();

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