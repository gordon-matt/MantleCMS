﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Data.Entity.EntityFramework
{
    public interface IEntityFrameworkHelper
    {
        void EnsureTables<TContext>(TContext context)
            where TContext : DbContext;
    }

    public class SqlEntityFrameworkHelper : IEntityFrameworkHelper
    {
        public void EnsureTables<TContext>(TContext context)
            where TContext : DbContext
        {
            string script = context.Database.GenerateCreateScript();
            if (!string.IsNullOrEmpty(script))
            {
                try
                {
                    var connection = context.Database.GetDbConnection();

                    bool isConnectionClosed = connection.State == ConnectionState.Closed;

                    if (isConnectionClosed)
                    {
                        connection.Open();
                    }

                    var existingTableNames = new List<string>();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SELECT table_name from INFORMATION_SCHEMA.TABLES WHERE table_type = 'base table'";

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                existingTableNames.Add(reader.GetString(0).ToLowerInvariant());
                            }
                        }
                    }

                    var split = script.Split(new[] { "CREATE TABLE " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string sql in split)
                    {
                        var tableName = sql.Substring(0, sql.IndexOf("(", StringComparison.OrdinalIgnoreCase));
                        tableName = tableName.Split('.').Last();
                        tableName = tableName.Trim().TrimStart('[').TrimEnd(']').ToLowerInvariant();

                        if (existingTableNames.Contains(tableName))
                        {
                            continue;
                        }

                        try
                        {
                            using (var createCommand = connection.CreateCommand())
                            {
                                createCommand.CommandText = "CREATE TABLE " + sql.Substring(0, sql.LastIndexOf(";"));
                                createCommand.ExecuteNonQuery();
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore
                        }
                    }

                    if (isConnectionClosed)
                    {
                        connection.Close();
                    }
                }
                catch (Exception)
                {
                    // Ignore
                }
            }
        }
    }
}