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
    public static DataBaseRequest GetFamiliesByName => new("Select * From [dbo].VFamilyList Where family = '{0}';");
    public static DataBaseRequest GetProductsListByComponent => new("Exec [dbo].GetProductsListByComponent '{0}';");
    public static DataBaseRequest GetProductsListByFamily => new("Exec [dbo].GetProductsListByFamily '{0}';");
    public static DataBaseRequest GetComponentsList => new("Select * From [dbo].VComponentUsageList;");
    public static DataBaseRequest GetComponentsListByProduct => new("Exec [dbo].GetComponentsListByProduct '{0}';");
    public static DataBaseRequest GetFilesListByComponent => new("Exec [dbo].GetFilesListByComponent '{0}';");
    public static DataBaseRequest RenameFamily => new("Exec [dbo].RenameFamily '{0}', '{1}';");
    public static DataBaseRequest RenameProduct => new("Exec [dbo].RenameProduct '{0}', '{1}';");
    public static DataBaseRequest RenameComponent => new("Exec [dbo].RenameComponent '{0}', '{1}';");
    public static DataBaseRequest ChangeFilePath => new("Exec [dbo].ChangeFilePath '{0}', '{1}';");
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