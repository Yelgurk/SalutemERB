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
using System.Collections.ObjectModel;
using SalutemES.Engineer.Avalonia.Models;
using Microsoft.Extensions.DependencyInjection;
using Avalonia.Controls;
using Microsoft.Extensions.Hosting;
using SalutemES.Engineer.Avalonia.Views;
using System.Drawing;
using System.Xml.Linq;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<MenuItemModel> MenuCollection { get; private set; }

    public MainWindowViewModel() => MenuCollection = new ObservableCollection<MenuItemModel>() {
        new MenuItemModel() { Name = "Создание заявки", Icon = App.GetIcon("save_regular"), Control = App.Host!.Services.GetRequiredService<OrderBuilderControl>() },
        new MenuItemModel() { Name = "Детали", Icon = App.GetIcon("document_edit_regular"), Control = App.Host!.Services.GetRequiredService<ComponentsEditorControl>() },
        new MenuItemModel() { Name = "Изделия", Icon = App.GetIcon("table_edit_regular"), Control = App.Host!.Services.GetRequiredService<ProductsEditorControl>() },
        new MenuItemModel() { Name = "Настройки", Icon = App.GetIcon("launcher_settings_regular"), Control = App.Host!.Services.GetRequiredService<SettingsControl>() }
    };

    private MenuItemModel? _menuItemSelected;

    public MenuItemModel MenuItemSelected
    {
        get => _menuItemSelected ?? MenuCollection[0];
        set
        {
            if (value is not null)
                App.SetWindowContent((_menuItemSelected = value).Control);
        }
    }
}