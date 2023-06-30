using Newtonsoft.Json;
using SalutemES.Engineer.Domain;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Reflection;
using SalutemES.Engineer.Application.SourceGenerator;

namespace SalutemES.Engineer.Application.ObservableModel;

[CopyPropertiesAttribute(typeof(ComponentBase))]
public partial class ComponentModel : ObservableObject
{
    private ComponentBase _base;

    public ComponentModel() : this(new ComponentBase()) { }
    public ComponentModel(string[] SetterValue) : this(new ComponentBase(SetterValue)) { }
    public ComponentModel(ComponentBase Base) => _base = Base;

    public string Name { get => _base.Name ?? ""; set { _base.Name = value; OnPropertyChanged(nameof(Name)); } }
    public string Code { get => _base.Code ?? ""; set { _base.Code = value; OnPropertyChanged(nameof(Code)); } }

}
