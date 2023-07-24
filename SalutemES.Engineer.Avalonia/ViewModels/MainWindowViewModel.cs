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
using System.Xml.Linq;
using Avalonia.Layout;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SalutemES.Engineer.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<MenuItemModel> MenuCollection { get; private set; }

    [ObservableProperty]
    private bool _popupTriggered = false;

    [ObservableProperty]
    private object? _popupPanelContent;

    public MainWindowViewModel() => MenuCollection = new ObservableCollection<MenuItemModel>() {
        new MenuItemModel() { Name = "Запуск", Icon = App.GetIcon("save_regular"), Control = new(), IsSelectable = false },
        new MenuItemModel() { Name = "Создание заявки", Icon = App.GetIcon("save_regular"), Control = App.Host!.Services.GetRequiredService<OrderBuilderControl>(), IsSelectable = true, IsFloat = true },
        new MenuItemModel() { Name = "Детали", Icon = App.GetIcon("document_edit_regular"), Control = App.Host!.Services.GetRequiredService<ComponentsEditorControl>(), IsSelectable = true },
        new MenuItemModel() { Name = "Изделия", Icon = App.GetIcon("table_edit_regular"), Control = App.Host!.Services.GetRequiredService<ProductsEditorControl>(), IsSelectable = true },
        new MenuItemModel() { Name = "Настройки", Icon = App.GetIcon("launcher_settings_regular"), Control = App.Host!.Services.GetRequiredService<SettingsControl>(), IsSelectable = false }
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

    public void DisplayPopupControl(UserControl Control)
    {
        PopupPanelContent = Control;
        PopupTriggered = true;
    }

    public void ClosePopupControl() => PopupTriggered = false;
}