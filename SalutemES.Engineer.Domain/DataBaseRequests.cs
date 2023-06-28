using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public record DataBaseRequest
{
    private readonly string _command;

    public DataBaseRequest(string Command) => _command = Command;

    public string Command(params string[] Args) => Args.Any() ? string.Format(this.ToString(), Args) : Command();
    public string Command() => this.ToString();

    public override string ToString() => _command;
}

public static class DataBaseRequests
{
    public static DataBaseRequest GetFullExportTable => new("Exec [dbo].GetFullExportTable");
    public static DataBaseRequest GetProductsListByComponent => new("Exec [dbo].GetProductsListByComponent '{0}';");
    public static DataBaseRequest GetProductsListByFamily => new("Exec [dbo].GetProductsListByFamily '{0}';");
    public static DataBaseRequest GetComponentsListByProduct => new("Exec [dbo].GetComponentsListByProduct '{0}';");
    public static DataBaseRequest GetFilesListByComponent => new("Exec [dbo].GetFilesListByComponent '{0}';");
    public static DataBaseRequest RenameFamily => new("Exec [dbo].RenameFamily '{0}', '{1}';");
    public static DataBaseRequest RenameProduct => new("Exec [dbo].RenameProduct '{0}', '{1}';");
    public static DataBaseRequest RenameComponent => new("Exec [dbo].RenameComponent '{0}', '{1}';");
    public static DataBaseRequest ChangeFilePath => new("Exec [dbo].ChangeFilePath '{0}', '{1}';");
}
