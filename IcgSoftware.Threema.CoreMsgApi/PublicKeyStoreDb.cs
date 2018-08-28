using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace IcgSoftware.Threema.CoreMsgApi
{
	class PublicKeyStoreDb : PublicKeyStore
	{
		private readonly string connectionString;

		public PublicKeyStoreDb(string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentException("connectionString must be not null or empty.");
			}

			this.connectionString = connectionString;

			this.CreateDatabase();
		}

		/// <summary>
		/// Fetch public key in store for particular threema id
		/// </summary>
		/// <param name="threemaId">Threema id to fetch</param>
		/// <returns>Public key</returns>
		protected override byte[] FetchPublicKey(string threemaId)
		{
			using (DbConnection connection = GetConnection(this.connectionString))
			{
				connection.Open();

				string sql = "SELECT threema_id, key FROM public_key WHERE threema_id = @threemaId";

				var command = connection.CreateCommand();
				command.CommandType = CommandType.Text;
				command.CommandText = sql;

				var paramThreemaId = command.CreateParameter();
				paramThreemaId.ParameterName = "threemaId";
				paramThreemaId.Value = threemaId;
				command.Parameters.Add(paramThreemaId);

				byte[] publicKey = null;

				using (var reader = command.ExecuteReader(CommandBehavior.SingleRow))
				{
					if (reader.HasRows)
					{
						if (reader.Read())
						{
							publicKey = DataUtils.HexStringToByteArray(reader[1].ToString());
						}
					}
					reader.Close();
				}

				return publicKey;
			}
		}

		/// <summary>
		/// Save threema id and public key into store
		/// </summary>
		/// <param name="threemaId">Threema id</param>
		/// <param name="publicKey">public key</param>
		protected override void Save(string threemaId, byte[] publicKey)
		{
			using (DbConnection connection = GetConnection(this.connectionString))
			{
				connection.Open();

				string sql = "INSERT INTO public_key (threema_id, key) VALUES (@threemaId, @key)";

				var command = connection.CreateCommand();
				command.CommandType = CommandType.Text;
				command.CommandText = sql;

				var paramThreemaId = command.CreateParameter();
				paramThreemaId.ParameterName = "threemaId";
				paramThreemaId.Value = threemaId;

				var paramKey = command.CreateParameter();
				paramKey.ParameterName = "key";
				paramKey.Value = DataUtils.ByteArrayToHexString(publicKey);

				command.Parameters.Add(paramThreemaId);
				command.Parameters.Add(paramKey);
				command.ExecuteNonQuery();

				connection.Close();
			}
		}

		/// <summary>
		/// Create database and table for public key store
		/// </summary>
		private void CreateDatabase()
		{
			using (DbConnection connection = GetConnection(this.connectionString))
			{
				connection.Open();

				string sql = "CREATE TABLE IF NOT EXISTS public_key (id INTEGER PRIMARY KEY AUTOINCREMENT, threema_id VARCHAR(8) NOT NULL UNIQUE, key VARCHAR(64) NOT NULL)";

				var command = connection.CreateCommand();
				command.CommandText = sql;
				command.ExecuteNonQuery();

				connection.Close();
			}
		}

		/// <summary>
		/// Get connetion is configured in App.config
		/// </summary>
		/// <param name="connectionString">Connect String</param>
		/// <returns>Db Connection</returns>
		private DbConnection GetConnection(string connectionString)
		{
            //throw new CoreMigrationException("SQLite is not supported");
            return new SqliteConnection(connectionString);

            //string providerName = "System.Data.SQLite";
            //var dbProvider = Microsoft.Data.Sqlite.SqliteFactory.Instance;
/*
            string providerName = null;            

            DbConnectionStringBuilder connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

			if (connectionStringBuilder.ContainsKey("provider"))
			{
				providerName = connectionStringBuilder["provider"].ToString();
			}
			else
			{              
                ConnectionStringSettings connectionStringSetting = ConfigurationManager
                        .ConnectionStrings
                        .Cast<ConnectionStringSettings>()
                        .FirstOrDefault(x => x.ConnectionString == connectionString);
                if (connectionStringSetting != null)
                {
                    providerName = connectionStringSetting.ProviderName;
                }
            }

			if (providerName != null)
			{
				bool providerExists = DbProviderFactories
						.GetFactoryClasses()
						.Rows.Cast<DataRow>()
						.Any(r => r[2].Equals(providerName));
				if (providerExists)
				{
					DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);
					DbConnection dbConnection = factory.CreateConnection();

					dbConnection.ConnectionString = connectionString;
					return dbConnection;
				}
			}

			return null;
*/
		}
	}
}
