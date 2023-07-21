using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Infrastructure.DataBase;

public record DataBaseRequest
{
    private readonly string _command;

    public DataBaseRequest(string Command) => _command = Command;

    public string Command(params string[] Args) => Args.Any() ? string.Format(this.ToString(), Args) : Command();
    public string Command() => _command;

    public override string ToString() => Command();
}

public static class DBRequests
{
    public static DataBaseRequest GetFamilies => new("Select * From [dbo].VFamilyList;");
    public static DataBaseRequest GetFamiliesByName => new("Select * From [dbo].VFamilyList Where VFamilyList.name = '{0}';");
    public static DataBaseRequest GetProductsListFullInfo => new("Select * From [dbo].VComponentListFullInfo;");
    public static DataBaseRequest GetProductsListByComponent => new("Exec [dbo].GetProductsListByComponent '{0}';");
    public static DataBaseRequest GetProductsListByFamily => new("Exec [dbo].GetProductsListByFamily '{0}';");
    public static DataBaseRequest GetProductByName => new("Exec [dbo].GetProductByName '{0}';");
    public static DataBaseRequest GetComponentsList => new("Select * From [dbo].VComponentUsageList;");
    public static DataBaseRequest GetComponentsListByProduct => new("Exec [dbo].GetComponentsListByProduct '{0}';");
    public static DataBaseRequest GetComponentsDetailsByNameCode => new("Exec [dbo].GetComponentsDetails '{0}', '{1}';");
    public static DataBaseRequest GetFilesListByComponent => new("Exec [dbo].GetFilesListByComponent '{0}';");
    public static DataBaseRequest GetProductComponentsFullInfo => new("Exec [dbo].[GetProductComponentsFullInfo] '{0}';");

    public static DataBaseRequest AddComponentFile => new("Exec [dbo].AddComponentFile '{0}', '{1}';");
    public static DataBaseRequest AddComponent => new("Exec [dbo].AddComponent '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}';");
    public static DataBaseRequest AddProduct => new("Exec [dbo].AddProduct '{0}', '{1}';");
    public static DataBaseRequest AddFamily => new("Exec [dbo].AddFamily '{0}';");
    public static DataBaseRequest AddProductComponent => new("Exec [dbo].AddProductComponent '{0}', '{1}', '{2}';");

    public static DataBaseRequest DeleteComponentFile => new("Exec [dbo].DeleteComponentFile '{0}', '{1}';");
    public static DataBaseRequest DeleteComponent => new("Exec [dbo].DeleteComponent '{0}', '{1}';");
    public static DataBaseRequest DeleteProduct => new("Exec [dbo].DeleteProduct '{0}';");
    public static DataBaseRequest DeleteFamily => new("Exec [dbo].DeleteFamily '{0}';");
    public static DataBaseRequest DeleteProductComponent => new("Exec [dbo].DeleteProductComponent '{0}', '{1}';");

    public static DataBaseRequest EditComponent => new("Exec [dbo].EditComponent '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}';");
    public static DataBaseRequest EditFamily => new("Exec [dbo].EditFamily '{0}', '{1}';");
    public static DataBaseRequest EditProductName => new("Exec [dbo].EditProductName '{0}', '{1}';");
    public static DataBaseRequest EditProductFamily => new("Exec [dbo].EditProductFamily '{0}', '{1}';");

    public static DataBaseRequest CheckComponentExists => new("Exec [dbo].CheckComponentExists '{0}', '{1}';");
    public static DataBaseRequest CheckFamilyExists => new("Exec [dbo].CheckFamilyExists '{0}';");
    public static DataBaseRequest CheckProductExists => new("Exec [dbo].CheckProductExists '{0}';");
}

public record DataBaseTableTypeArgName(string Name);
public static class DBTableTypeNames
{
    public static DataBaseTableTypeArgName ExportRequestTableType => new("@ProductsList");
}

public record DataBaseTableArgProcedure(string Name);

public static class DBProceduresWithTableArg
{
    public static DataBaseTableArgProcedure GetFullExportTable => new("GetExportTable");
}