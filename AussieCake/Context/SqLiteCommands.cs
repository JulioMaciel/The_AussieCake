using AussieCake.Models;
using AussieCake.Util;
using System.Data;
using System.Data.SQLite;
using System.IO;

namespace AussieCake.Helper
{
	/// <summary>
	/// Class to simplify connections to a SQLite database
	/// </summary>
	public class SqLiteCommands
	{
		private static readonly string ConnectionString;

		/// <summary>
		/// Checks if the database exists and creates the connnection to the database
		/// </summary>
		/// <param name="DatabasePath">Database Path</param>
		static SqLiteCommands()
		{				
			// Checks if the database exists
			if (!File.Exists(CakePaths.Database))
			{
				SQLiteConnection.CreateFile(CakePaths.Database);
			}

			ConnectionString = "Data Source=" + CakePaths.Database + "; Version=3";
		}

		/// <summary>
		/// Creates a database
		/// </summary>
		/// <param name="Query">SQL statement</param>
		protected static void CreateDb(string Query)
		{
			SendQuery(Query);
		}

		/// <summary>
		/// Sends a SQL query that doesn't return any value
		/// It can also be used to create a table, but it's recomended to use CreateDb instead
		/// </summary>
		/// <param name="Query">SQL statement</param>
		protected static void SendQuery(string Query)
		{
			using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
			{
				using (SQLiteCommand SqlCmd = new SQLiteCommand())
				{
					SqlCmd.Connection = Connection;
					SqlCmd.CommandText = Query;
					Connection.Open();
					SqlCmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// Gets the first result from the query
		/// </summary>
		/// <param name="Query">SQL statement</param>
		/// <returns>Return the value inside a System.Object and must be converted. If no result, returns null</returns>
		protected static SQLiteDataReader GetFromQuery(string Query)
		{
			SQLiteDataReader result = null;
			using (SQLiteConnection Connection = new SQLiteConnection(ConnectionString))
			{
				using (SQLiteCommand SqlCmd = new SQLiteCommand())
				{
					SqlCmd.Connection = Connection;
					SqlCmd.CommandText = Query;
					Connection.Open();
					using (SQLiteDataReader Reader = SqlCmd.ExecuteReader())
					{
						if (Reader.NextResult())
						{
							result = Reader;
						}
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Gets table values inside a dataset
		/// </summary>
		/// <param name="query">SQL statement</param>
		/// <param name="tableName">Name of the table</param>
		/// <returns>All the returned values into a dataset</returns>
		protected static DataSet GetTable(ModelType type)
		{
			string query = "SELECT * FROM " + type.ToDescString();
			DataSet data = new DataSet();

			using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
			{
				using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection))
				{
					adapter.Fill(data);
				}
			}


			return data;
		}
	}
}