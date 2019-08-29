using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace WallControl
{
    public static class AccessFunction
    {
        private static OleDbConnection dbConnection;

        public static void AccessOpen()
        {
            string connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=MatrixCode.mdb;";
            AccessFunction.dbConnection = new OleDbConnection(connectionString);
            try
            {
                AccessFunction.dbConnection.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void AccessCloseAll()
        {
            AccessFunction.dbConnection.Close();
        }

        public static DataTable GetTables()
        {
            string selectCommandText = "SELECT * FROM protocol";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, AccessFunction.dbConnection);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public static DataTable GetClientProtocol(string clientname)
        {
            string selectCommandText = "SELECT * FROM protocol WHERE client_type='" + clientname + "'";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, AccessFunction.dbConnection);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public static void DeleteClientProtocol(string clientname)
        {
            string selectCommandText = "SELECT * FROM protocol WHERE client_type='" + clientname + "'";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, AccessFunction.dbConnection);
            OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder(oleDbDataAdapter);
            DataTable dataTable = new DataTable();
            oleDbDataAdapter.Fill(dataTable);
            dataTable.Rows[0].Delete();
            oleDbDataAdapter.DeleteCommand = oleDbCommandBuilder.GetDeleteCommand();
            oleDbDataAdapter.Update(dataTable);
        }

        public static void InsertClientProtocol(string[] str)
        {
            string selectCommandText = "SELECT * FROM protocol";
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommandText, AccessFunction.dbConnection);
            OleDbCommandBuilder oleDbCommandBuilder = new OleDbCommandBuilder(oleDbDataAdapter);
            DataSet dataSet = new DataSet();
            oleDbDataAdapter.Fill(dataSet);
            OleDbCommandBuilder oleDbCommandBuilder2 = new OleDbCommandBuilder(oleDbDataAdapter);
            oleDbDataAdapter.UpdateCommand = oleDbCommandBuilder2.GetUpdateCommand();
            int count = dataSet.Tables[0].Columns.Count;
            DataRow dataRow = dataSet.Tables[0].NewRow();
            for (int i = 0; i < count; i++)
            {
                dataRow[i] = str[i];
            }
            dataSet.Tables[0].Rows.Add(dataRow);
            oleDbDataAdapter.Update(dataSet);
        }
    }
}
