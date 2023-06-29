using Avalonia;
using SalutemES.Engineer.Domain;
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
            List<string[]> Response = new List<string[]>()
            {
                new string[] { "Обечайка", "01.00.002", "4", "aisi", "1.5", "", "19.5", "80", "плёнка", "собств" },
                new string[] { "Крышка", "01.00.005", "6", "aisi", "1.5", "1", "13.1", "70", "", "собств" },
                new string[] { "Шайба", "01.00.009", "8", "aisi", "3.0", "", "9.4", "64", "гравировка", "собств" }
            };

            List<ExportExcelModel> ExportList = new List<ExportExcelModel>();
            foreach (string[] cortage in Response)
                ExportList.Add(new ExportExcelModel(cortage));

            foreach (ExportExcelModel export in ExportList)
                Debug.WriteLine(export);
        }
    }
}