namespace Mantle.Data.Services;

public class SqlServerEntityFrameworkHelper : IMantleEntityFrameworkHelper
{
    public void EnsureTables<TContext>(TContext context) where TContext : DbContext
    {
        var connection = context.Database.GetDbConnection();
        bool isConnectionClosed = connection.State == ConnectionState.Closed;
        if (isConnectionClosed)
        {
            connection.Open();
        }

        var existingTableNames = new List<string>();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            existingTableNames.Add(reader.GetString(0).ToLowerInvariant());
        }

        var commands = context.Database.GenerateCreateScript()
            .Split("GO", StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim());

        var newTables = commands
            .Where(x => x.StartsWith("CREATE TABLE [", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.RightOf("[").LeftOf("]"))
            .Where(x => !existingTableNames.Contains(x.ToLowerInvariant()))
            .ToList();

        foreach (string newTable in newTables)
        {
            string quotedTableName = $"[{newTable}]";
            var tableCommands = commands.Where(x => x.Contains(quotedTableName, StringComparison.OrdinalIgnoreCase)).ToList();
            string createCommand = tableCommands.First(x => x.StartsWith($"CREATE TABLE {quotedTableName}", StringComparison.OrdinalIgnoreCase));

            try
            {
                context.Database.ExecuteSqlRaw(createCommand);
            }
            catch { } // TODO: Log it..

            tableCommands.Remove(createCommand);

            foreach (string cmd in tableCommands)
            {
                if (cmd.StartsWith($"CREATE TABLE ", StringComparison.OrdinalIgnoreCase))
                {
                    // There should only be one create table command per table
                    continue;
                }

                try
                {
                    context.Database.ExecuteSqlRaw(cmd);
                }
                catch { } // TODO: Log it..
            }
        }
    }
}