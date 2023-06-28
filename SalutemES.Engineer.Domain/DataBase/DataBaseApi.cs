using OneOf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SalutemES.Engineer.Domain.DataBase;

public sealed partial class DataBaseApi
{
    private static SqlConnection? Connection { get; set; }
    private static SqlCommand? Command { get; set; }
    private static SqlDataReader? Reader { get; set; }

    public static List<string[]>? ResponseArray { get; private set; }
    public static string? ResponseString { get; private set; }

    private static string ServerName { get; set; } = string.Empty;
    private static string DataBaseName { get; set; } = string.Empty;

    private static SqlException? SQLConnectionException { get; set; }

    private static void SQLOpenConnection()
    {
        try
        {
            if (Connection is not null)
                SQLCloseConnection();

            Connection = new SqlConnection($@"
                Data Source = {ServerName};
                Initial Catalog = {DataBaseName};
                Integrated Security = True;");
            Connection.Open();
        }
        catch (SqlException e) { SQLConnectionException = e; }
    }

    private static void SQLCloseConnection()
    {
        try
        {
            Connection?.Close();
            Connection?.Dispose();
            Connection = null;
        }
        catch (SqlException e) { SQLConnectionException = e; }
    }

    public static DataBaseApiOr<SQLUnavailable> ConnectionAvailable() => SetConnection(ServerName, DataBaseName);

    public static DataBaseApiOr<SQLUnavailable> SetConnection(string ServerName, string DataBaseName)
    {
        DataBaseApi.ServerName = ServerName;
        DataBaseApi.DataBaseName = DataBaseName;

        SQLConnectionException = null;
        SQLOpenConnection();
        SQLCloseConnection();

        return SQLConnectionException is null ? new DataBaseApi() : new SQLUnavailable();
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