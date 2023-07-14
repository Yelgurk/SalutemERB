using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Avalonia.Models;

public class MenuItemModel
{
    public required string Name { get; set; }
    public required StreamGeometry Icon { get; set; }
    public required UserControl Control { get; set; }
    public required bool IsSelectable { get; set; }
}
