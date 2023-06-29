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
            List<FamilyModel> FamilyModelCollection = new List<FamilyModel>();
            List<ProductModel> ProductModelByFamilyCollection = new List<ProductModel>();
            List<ProductModel> ProductModelByComponentCollection = new List<ProductModel>();
            List<ComponentModel> ComponentModelCollection = new List<ComponentModel>();
            List<ComponentFileModel> ComponentFileModelCollection = new List<ComponentFileModel>();
            List<ExportExcelModel> ExportExcelModelCollection = new List<ExportExcelModel>();
            

            foreach (string[] cortage in
                DataBaseApi.SetConnection("DESKTOP-J7PGA2A", "DB_SE_EngineerWS")
                .Api.PrepareCommand(DataBaseRequests.GetFamilies)
                .Api.ExecuteCommand<List<string[]>>()
                .Api.DataBaseResponse<List<string[]>>()!)
                FamilyModelCollection.Add(new FamilyModel(cortage));

            foreach (string[] cortage in
                DataBaseApi.ConnectionAvailable()
                .Api.PrepareCommand(DataBaseRequests.GetProductsListByFamily, "Пастеризаторы")
                .Api.ExecuteCommand<List<string[]>>()
                .Api.DataBaseResponse<List<string[]>>()!)
                ProductModelByFamilyCollection.Add(new ProductModel(cortage));

            foreach (string[] cortage in
                DataBaseApi.ConnectionAvailable()
                .Api.PrepareCommand(DataBaseRequests.GetProductsListByComponent, "Крышка панели с пультом")
                .Api.ExecuteCommand<List<string[]>>()
                .Api.DataBaseResponse<List<string[]>>()!)
                ProductModelByComponentCollection.Add(new ProductModel(cortage));

            foreach (string[] cortage in
                DataBaseApi.ConnectionAvailable()
                .Api.PrepareCommand(DataBaseRequests.GetComponentsListByProduct, "ПС 250")
                .Api.ExecuteCommand<List<string[]>>()
                .Api.DataBaseResponse<List<string[]>>()!)
                ComponentModelCollection.Add(new ComponentModel(cortage));

            foreach (string[] cortage in
                DataBaseApi.ConnectionAvailable()
                .Api.PrepareCommand(DataBaseRequests.GetFilesListByComponent, "Корпус ПС 100")
                .Api.ExecuteCommand<List<string[]>>()
                .Api.DataBaseResponse<List<string[]>>()!)
                ComponentFileModelCollection.Add(new ComponentFileModel(cortage));

            foreach (FamilyModel x in FamilyModelCollection)
                Debug.WriteLine(x);
            Debug.WriteLine("\n");

            foreach (ProductModel x in ProductModelByFamilyCollection)
                Debug.WriteLine(x);
            Debug.WriteLine("\n");

            foreach (ProductModel x in ProductModelByComponentCollection)
                Debug.WriteLine(x);
            Debug.WriteLine("\n");

            foreach (ComponentModel x in ComponentModelCollection)
                Debug.WriteLine(x);
            Debug.WriteLine("\n");

            foreach (ComponentFileModel x in ComponentFileModelCollection)
                Debug.WriteLine(x);
            Debug.WriteLine("\n");
        }
    }
}