using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace TickDataImporter
{
    internal class DatabaseManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private void WriteFileNameToDB(string fileName)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            conn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = conn;
            sqlCmd.CommandText = string.Format(
                    @"INSERT INTO [TickData].[dbo].[tblImported]
                        ([FileName])
                    VALUES
                        ('{0}')", fileName);
            sqlCmd.ExecuteNonQuery();
        }

        private void WriteTickDataToDB(List<TickDate> lstTickdata)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            conn.Open();
            SqlCommand sqlCmd = new SqlCommand();
            var builder = new StringBuilder();
            sqlCmd.CommandTimeout = 1000;
            foreach (var item in lstTickdata)
            {
                builder.AppendLine(string.Format(
                        @"INSERT INTO [TickData].[dbo].[tblTickData]
                           ([InstrumentID]
                           ,[DateTime]
                           ,[Price]
                           ,[Volume]
                           ,[MarketFlag]
                           ,[SalesCondition]
                           ,[ExcludeFlag]
                           ,[UnfilteredPrice])
                     VALUES
                           ('{0}','{1}',{2},{3},'{4}',{5},'{6}',{7})", item.GetTickDataEntries()));
            }
            sqlCmd.CommandText = builder.ToString();
            sqlCmd.Connection = conn;
            sqlCmd.ExecuteNonQuery();
            conn.Close();
        }

        private void WriteBulkInsertToDB(string filenmae)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=mwa\sqlexpress;Initial Catalog=TickData;Integrated Security=True");
            try
            {
                conn.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.CommandText = string.Format("BULK INSERT [TickData].[dbo].[tblTickData] FROM '{0}' WITH (FORMATFILE = '{1}')", filenmae,
                    @"C:\Users\Willi\Documents\visual studio 2010\Projects\TickDataToDB\TickDataImporter\Otherstuff\bulkFormat.fmt");
                sqlCmd.Connection = conn;
                sqlCmd.ExecuteNonQuery();
                File.Delete(filenmae);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}