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

    public static DataBaseApi? ConnectionAvailable() => SetConnection(ServerName, DataBaseName);

    public static DataBaseApi? SetConnection(string ServerName, string DataBaseName)
    {
        DataBaseApi.ServerName = ServerName;
        DataBaseApi.DataBaseName = DataBaseName;

        bool Success = true;

        try { SQLOpenConnection(); }
        catch { Success = false; }
        finally { SQLCloseConnection(); }

        return Success ? new DataBaseApi() : null;
    }

    public DataBaseApi PrepareCommand(DataBaseRequest Request, params string[] Args)
    {
        Command?.Dispose();

        SQLOpenConnection();

        try { Command = new SqlCommand(Request.Command(Args), Connection!); }
        catch { Command = null; }

        return new DataBaseApi();
    }

    public DataBaseApi? CommandPrepeared() => Command is null ? null : new DataBaseApi();

    private static DataBaseApi? ResponseIntoString()
    {
        try
        {
            Reader = Command!.ExecuteReader();

            ResponseString = "";

            while (Reader.Read())
                for (int i = 0; i < Reader.FieldCount; i++)
                    ResponseString += Convert.ToString(Reader.GetValue(i));
        }
        catch
        { ResponseString = null; }
        finally
        { SQLCloseConnection(); }

        return ResponseString is null ? null : new DataBaseApi();
    }

    private static DataBaseApi? ResponseIntoArray()
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
        catch
        { ResponseArray = null; }
        finally
        { SQLCloseConnection(); }

        return ResponseArray is null ? null : new DataBaseApi();
    }

    public DataBaseApi? ExecuteCommand(DataBaseResponseType Type)
    {
        Debug.WriteLine(Command!.CommandText);

        return Type switch
        {
            DataBaseResponseType.String => ResponseIntoString(),
            DataBaseResponseType.ListArray => ResponseIntoArray(),
            _ => null
        };
    }
}