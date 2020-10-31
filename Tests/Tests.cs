using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class Tests
    {
        [TestClass]
        public class AreConnectionLevelCommandTimeoutsRespected
        {
            [TestMethod]
            public void ForSynchronousMethods()
            {
                NpgsqlException thrownException = null;

                try
                {
                    var connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;CommandTimeout=1;";

                    using var conn = new NpgsqlConnection(connStr);
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("select pg_sleep(3)::TEXT;", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            Console.WriteLine(reader.GetString(0));
                }
                catch (NpgsqlException ex)
                {
                    thrownException = ex;
                }

                const string v4_MESSAGE = "Connection timed out";
                const string v5_MESSAGE = "Timeout during reading attempt";
                Assert.IsNotNull(thrownException, $"Expected a {nameof(NpgsqlException)} to be thrown");
                Assert.IsTrue(
                    thrownException.InnerException.Message.Contains(v4_MESSAGE) ||
                    thrownException.InnerException.Message.Contains(v5_MESSAGE),
                    $"actual message: {thrownException.InnerException.Message}");
            }

            [TestMethod]
            public async Task ForAsynchronousMethods()
            {
                Exception thrownException = null;

                try
                {
                    var connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres;CommandTimeout=1;";

                    await using var conn = new NpgsqlConnection(connStr);
                    await conn.OpenAsync();

                    await using (var cmd = new NpgsqlCommand("select pg_sleep(3)::TEXT;", conn))
                    await using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            Console.WriteLine(reader.GetString(0));
                }
                catch (Exception ex)
                {
                    thrownException = ex;
                }

                Assert.IsNotNull(thrownException, $"Expected an excpetion to be thrown");
            }
        }

        [TestClass]
        public class AreStatementTimeoutsRespected
        {
            [TestMethod]
            public void ForSynchronousMethods()
            {
                PostgresException thrownException = null;

                try
                {
                    var connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres";

                    using var conn = new NpgsqlConnection(connStr);
                    conn.Open();

                    using (var cmd = new NpgsqlCommand("set statement_timeout to 1000;", conn))
                        cmd.ExecuteNonQuery();

                    using (var cmd = new NpgsqlCommand("select pg_sleep(3)::TEXT;", conn))
                    using (var reader = cmd.ExecuteReader())
                        while (reader.Read())
                            Console.WriteLine(reader.GetString(0));
                }
                catch (PostgresException ex)
                {
                    thrownException = ex;
                }

                Assert.IsNotNull(thrownException, $"Expected a {nameof(PostgresException)} to be thrown");
                Assert.AreEqual("57014", thrownException?.SqlState);
                Assert.AreEqual("canceling statement due to statement timeout", thrownException.MessageText);
            }

            [TestMethod]
            public async Task ForAsynchronousMethods()
            {
                PostgresException thrownException = null;

                try
                {
                    var connStr = "Host=localhost;Port=5432;Username=postgres;Password=postgres";

                    await using var conn = new NpgsqlConnection(connStr);
                    await conn.OpenAsync();

                    await using (var cmd = new NpgsqlCommand("set statement_timeout to 1000;", conn))
                        await cmd.ExecuteNonQueryAsync();

                    await using (var cmd = new NpgsqlCommand("select pg_sleep(3)::TEXT;", conn))
                    await using (var reader = await cmd.ExecuteReaderAsync())
                        while (await reader.ReadAsync())
                            Console.WriteLine(reader.GetString(0));
                }
                catch (PostgresException ex)
                {
                    thrownException = ex;
                }

                Assert.IsNotNull(thrownException, $"Expected a {nameof(PostgresException)} to be thrown");
                Assert.AreEqual("57014", thrownException?.SqlState);
                Assert.AreEqual("canceling statement due to statement timeout", thrownException.MessageText);
            }
        }
    }
}
