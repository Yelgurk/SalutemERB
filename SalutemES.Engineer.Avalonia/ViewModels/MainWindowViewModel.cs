using Avalonia;
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
            Response =
                DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
                .Handler(api => api.IsSuccess, null)
                ?.DataBase.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                .Handler(api => api.IsSuccess)
                ?.DataBase.ExecuteCommand<List<string[]>>()
                .Handler(api => api.IsSuccess)
                ?.DataBase.DataBaseResponse<List<string[]>>();

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
                .Handler(api => api.IsSuccess, error => Debug.WriteLine(error.Exception.message))
                ?.DataBase.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                .Handler(api => api.IsSuccess, error => Debug.WriteLine(error.Exception.message))
                ?.DataBase.ExecuteCommand<List<string[]>>()
                .Handler(api => api.IsSuccess, (error) => { Debug.WriteLine(error.Exception.message); })
                ?.DataBase.DataBaseResponse<List<string[]>>();

            if (Response is not null)
                foreach (var arr in Response)
                {
                    foreach (string cell in arr)
                        Debug.Write($" {cell} |");
                    Debug.WriteLine("");
                }

            // example #3
            Response =
                DataBaseApi.SetConnection(@"DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
                .DataBase.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                .DataBase.ExecuteCommand<List<string[]>>()
                .DataBase.DataBaseResponse<List<string[]>>();

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