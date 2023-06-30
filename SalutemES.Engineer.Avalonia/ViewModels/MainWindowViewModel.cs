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
            List<ExportTableRequestModel> Request = new List<ExportTableRequestModel>();
            Request.Add(new ExportTableRequestModel() { product = "ПС 100", count = 2 });
            Request.Add(new ExportTableRequestModel() { product = "ТМП 200", count = 4 });

            List<string[]>? Response = null;


            Response = DataBaseApi.SetConnection("DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
            .Handler(exec => exec.IsSuccess, error => Debug.WriteLine(error.Exception.message))
            !.Api.PrepareCommand(DBProceduresWithTableArg.GetFullExportTable, Request, DBTableTypeNames.ExportRequestTableType)
            !.Api.ExecuteCommand<List<string[]>>()
            ?.Handler(exec => exec.IsSuccess, error => Debug.WriteLine(error.Exception.message))
            ?.Api.DataBaseResponse<List<string[]>>();

            if (Response is not null)
            {
                foreach (var response in Response)
                {
                    foreach (var item in response)
                    {
                        Debug.Write($"{item}|");
                    }
                    Debug.WriteLine("");
                }
            }
        }
    }
}