using CommunityToolkit.Mvvm.ComponentModel;
using SalutemES.Engineer.Infrastructure.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Core.ViewModels;

public class FamilyViewModel : ObservableObject
{
    public Action? OnModelChanged { get; set; } = null;

    public ObservableCollection<FamilyModel> Families { get; } = new ObservableCollection<FamilyModel>();

    private string _familyName = "";

    public string FamilyName
    {
        get => _familyName;
        set
        {
            if (SetProperty(ref _familyName, value))
            {
                if (_familyName == "")
                    FillCollection();
                else
                    FillCollection(DBRequests.GetFamiliesByName);

                OnModelChanged?.Invoke();
            }
        }
    }

    public void FillCollection() => FillCollection(DBRequests.GetFamilies);

    public void FillCollection(DataBaseRequest Request)
    {
        Families.Clear();
        DataBaseApi.ConnectionAvailable()
            .Handler(conn => conn.IsSuccess, error => Debug.WriteLine(error.Exception.message))
            ?.Api.PrepareCommand(Request, FamilyName)
            .Handler(comm => comm.IsSuccess, error => Debug.WriteLine(error.Exception.message))
            ?.Api.ExecuteCommand<List<string[]>>()
            .Handler(exec => exec.IsSuccess, error => Debug.WriteLine(error.Exception.message))
            ?.Api.DataBaseResponse<List<string[]>>()
            ?.ForEach(cortage => Families.Add(new FamilyModel(cortage)));
    }
}
