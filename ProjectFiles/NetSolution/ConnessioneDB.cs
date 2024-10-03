#region Using directives
using System;
using UAManagedCore;
using OpcUa = UAManagedCore.OpcUa;
using FTOptix.HMIProject;
using FTOptix.Retentivity;
using FTOptix.UI;
using FTOptix.NativeUI;
using FTOptix.CoreBase;
using FTOptix.Core;
using FTOptix.NetLogic;
using System.Data.OracleClient;
using FTOptix.SQLiteStore;
using FTOptix.Store;
#endregion

public class ConnessioneDB : BaseNetLogic
{
    public override void Start()
    {
        // Insert code to be executed when the user-defined logic is started
    }

    public override void Stop()
    {
        // Insert code to be executed when the user-defined logic is stopped
    }

    

    [ExportMethod]
    public void ConnectingToOracle()
    {
        string connectionString = GetConnectionString();
        using (OracleConnection connection = new OracleConnection())
        {           
            connection.ConnectionString = connectionString;
            Log.Info("ConnectionString: " + connection.ConnectionString);

            connection.Open();
            Log.Info("State:" + connection.State);
           

            OracleCommand command = connection.CreateCommand();
            string sql = "SELECT * FROM MAGAUTO.ARTICOLI_VIEW";
            command.CommandText = sql;

            OracleDataReader reader = command.ExecuteReader();

            // Save the names of the columns of the table to an array
            string[] columns = { "Column1", "Column2", "Column3" };

            // Create and populate a matrix with values to insert into the odbc table
            var rawValues = new object[1, 3];

            var myStore = Project.Current.Get<Store>("DataStores/EmbeddedDatabase1");

            // Get Table1 from myStore
            var table1 = myStore.Tables.Get<Table>("TabellaDatiOracle");

            //svuoto la tabella di appoggio prima di iniziare a riempirla
            object[,] resultSet;
            string[] header;

            // execute query on store of the current project
            myStore.Query("DELETE FROM TabellaDatiOracle", out header, out resultSet);

            int contatore = 0;

            while (reader.Read())
            {
                rawValues[0, 0] = (string)reader[0];
                rawValues[0, 1] = (string)reader[1];
                rawValues[0, 2] = (string)reader[2];

                table1.Insert(columns, rawValues);
                contatore++;
                //Log.Info(myField);
            }
            connection.Close();
            Log.Info("Numero record tabella:" + contatore);
            
        }
    }
    private static string GetConnectionString()
    {
        //String connString = "(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))(CONNECT_DATA =(SID = orcl)));User ID=system;Password=Asem2024;";
        //String connString = "(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.0.9)(PORT = 1521))(CONNECT_DATA =(SID = ORCL)));User ID=MAGAUTO;Password=MAGAUTO";
        String connString = "Data Source= 192.168.0.9:1521/ORCL; User Id=MAGAUTO; Password=MAGAUTO";

        return connString;
    }

}
