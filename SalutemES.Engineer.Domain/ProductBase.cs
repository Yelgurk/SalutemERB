using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public class ProductBase : ReflectionExtension
{
    public ProductBase() { }
    public ProductBase(string[] DataBaseResponse) => FillModel(DataBaseResponse, this);

    public string? Name { get; set; }
    public string? FamilyName { get; set; }
    public string? Count { get; set; }
}