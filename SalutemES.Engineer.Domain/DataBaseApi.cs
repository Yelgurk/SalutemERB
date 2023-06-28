using OneOf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain;

public sealed partial class DataBaseApi
{
    private static SqlConnection? Connection { get; set; } = null;
    private static SqlCommand? Command { get; set; } = null;
    private static SqlDataReader? Reader { get; set; } = null;

    public static List<string[]>? ResponseArray { get; private set; } = null;
    public static string? ResponseString { get; private set; } = null;

    private static string ServerName = string.Empty;
    private static string DataBaseName = string.Empty;

    private static void SQLOpenConnection()
    {
        if (Connection is not null)
            SQLCloseConnection();

        Connection = new SqlConnection($@"
                Data Source = {ServerName};
                Initial Catalog = {DataBaseName};
                Integrated Security = True;");
        Connection.Open();
    }

    private static void SQLCloseConnection()
    {
        Connection?.Close();
        Connection?.Dispose();
        Connection = null;
    }

    public static DataBaseApiOr<SQLUnavailable> ConnectionAvailable() => SetConnection(ServerName, DataBaseName);

    public static DataBaseApiOr<SQLUnavailable> SetConnection(string ServerName, string DataBaseName)
    {
        DataBaseApi.ServerName = ServerName;
        DataBaseApi.DataBaseName = DataBaseName;

        bool Success = true;

        try { SQLOpenConnection(); }
        catch { Success = false; }
        finally { SQLCloseConnection(); }

        return Success ? new DataBaseApi() : new SQLUnavailable();
    }

    public static DataBaseApiOr<SQLCommandNotReady> CommandPrepeared() => Command is null ? new SQLCommandNotReady() : new DataBaseApi();

    public DataBaseApiOr<SQLCommandNotReady> PrepareCommand(DataBaseRequest Request, params string[] Args)
    {
        Command?.Dispose();

        SQLOpenConnection();

        try { Command = new SqlCommand(Request.Command(Args), Connection!); }
        catch { Command = null; }

        return Command is null ? new SQLCommandNotReady() : new DataBaseApi();
    }

    private static DataBaseApiOr<SQLExecError> ResponseIntoString()
    {
        try
        {
            Reader = Command!.ExecuteReader();

            ResponseString = "";

            while (Reader.Read())
                for (int i = 0; i < Reader.FieldCount; i++)
                    ResponseString += Convert.ToString(Reader.GetValue(i));
        }
        catch { ResponseString = null; }
        finally { SQLCloseConnection(); }

        return ResponseString is null ? new SQLExecError() : new DataBaseApi();
    }

    private static DataBaseApiOr<SQLExecError> ResponseIntoArray()
    {
        try
        {
            Reader = Command!.ExecuteReader();

            ResponseArray = new List<string[]>();

            if (Reader.HasRows)
                while (Reader.Read())
                {
                    string[] Cortage = new string[Reader.FieldCount];
                    for (int i = 0; i < Reader.FieldCount; i++)
                        Cortage[i] = Convert.ToString(Reader.GetValue(i))!;
                    ResponseArray.Add(Cortage);
                }
        }
        catch { ResponseArray = null; }
        finally { SQLCloseConnection(); }

        return ResponseArray is null ? new SQLExecError() : new DataBaseApi();
    }

    public DataBaseApiOr<SQLExecError> ExecuteCommand<T>()
    {
        return typeof(T) switch
        {
            Type arr when arr == typeof(List<string[]>) => ResponseIntoArray(),
            Type str when str == typeof(string) => ResponseIntoString(),
            _ => new SQLExecError()
        };
    }

    public T? DataBaseResponse<T>()
    {
        return typeof(T) switch
        {
            Type arr when arr == typeof(List<string[]>) => (T)Convert.ChangeType(ResponseArray, typeof(T))!,
            Type str when str == typeof(string) => (T)Convert.ChangeType(ResponseString, typeof(T))!,
            _ => default(T)
        };
    }
}