using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;

namespace MyPractice
{
    class Program
    {
        static string connectionStr = "Server=.;Database=Minions;Trusted_Connection=True";

        static void Task2()
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr =
                    "SELECT * FROM Villains WHERE ID IN (SELECT VillainId FROM MinionsVillains GROUP BY VillainId HAVING COUNT(MinionId) >=3)";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]}");
                        }

                        Console.WriteLine();
                    }
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private static string GetVillainNameById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT Name FROM Villains WHERE Id = @id";                      
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);
                command.Parameters.AddWithValue("@id", id);
                var result = (string)command.ExecuteScalar();
                return result;
            }
        }


        private static bool IsVillainHasMinionsById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT COUNT(*) FROM MinionsVillains WHERE VillainId = @id";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);
                cmd.Parameters.AddWithValue("@id", id);
                var result = (int)cmd.ExecuteScalar();
                return result > 0;
            }
        }


        private static bool IsVillainExistById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT COUNT(*) FROM Villains  WHERE Id=@id";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);
                command.Parameters.AddWithValue("@id", id);
                var result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }

        static void Task3()
        {
            Console.Write("Enter VillainId: ");

            int id = int.Parse(Console.ReadLine());

            if (!IsVillainExistById(id))
            {
                Console.WriteLine($"В базе данных не существует злодея с идентификатором {id}");
                return;
            }

            string villainName = GetVillainNameById(id);
            Console.WriteLine($"Villain: {villainName}");

            if (!IsVillainHasMinionsById(id))
            {
                Console.WriteLine($"(no minions)");
                return;
            }
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr =
                    "SELECT Minions.Name as MinionName,Minions.Age as MinionAge FROM MinionsVillains" +
                    "JOIN Villains ON Villains.Id=VillainId " +
                    "JOIN Minions ON Minions.Id=MinionId " +
                    "WHERE VillainId=@id ORDER BY MinionName ASC";

                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                int number = 1;
                using (reader)
                {
                    using (reader)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{number++}. {reader["MinionName"]} {reader["MinionAge"]}");
                        }
                    }
                }
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static bool IsVillainExist(string name)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT COUNT(*) FROM Villains WHERE Name = @name";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);
                command.Parameters.AddWithValue("@name", name);
                var result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }

        private static bool IsTownExist(string name)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT COUNT(*) FROM Towns WHERE Name=@name";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);
                command.Parameters.AddWithValue("@name", name);
                var result = (int)command.ExecuteScalar();
                return result > 0;
            }
        }

        private static int InsertTown(string name)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "INSERT INTO Towns (Name, CountryCode) VALUES  (@name, 2);" +
                    " SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);

                cmd.Parameters.AddWithValue("@name", name);

                var result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        private static int InsertVillain(string name)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@name, 2);" +
                    "SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);

                cmd.Parameters.AddWithValue("@name", name);

                var result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        private static int InsertMinion(string name, int age, int townId)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "INSERT INTO Minions (Name, Age, TownId) VALUES (@name, @age, @townId);" +
                    "SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@age", age);
                cmd.Parameters.AddWithValue("@townId", townId);

                var result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }
        }

        private static void InsertVillainsMinion(int minionId, int villainId)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "INSERT INTO MinionsVillains  (MinionId, VillainId) VALUES(@minionId, @villainId);" +
                "SELECT SCOPE_IDENTITY()";
                SqlCommand cmd = new SqlCommand(selectionCmdStr, connection);

                cmd.Parameters.AddWithValue("@minionId", minionId);
                cmd.Parameters.AddWithValue("@villainId", villainId);
            }
        }


        static void Task4()
        {
            string[] minionData = Console.ReadLine().Split(' ');
            string[] villainData = Console.ReadLine().Split(' ');
            string minionName = minionData[1];
            int minionAge = int.Parse(minionData[2]);
            string minionTown = minionData[3];
            int minionTownId = -1;

            if (!IsTownExist(minionTown))
            {
                minionTownId = InsertTown(minionTown);
                Console.WriteLine($"Город {minionTown} был добавлен в БД, id={minionTownId}");
            }

            string villainName = villainData[1];
            int villainId = -1;

            if (!IsVillainExist(villainName))
            {
                villainId = InsertVillain(villainName);
                Console.WriteLine($"Злодей {villainName} был добавлен в БД, id={villainId}");
            }

            var minionId = InsertMinion(minionName, minionAge, minionTownId);
            InsertVillainsMinion(minionId, villainId);

            Console.WriteLine($"Успешно добавлен {minionName}, чтобы быть миньоном {villainName}");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static int DeleteMinionsVillainsByVillainId(int villainId)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);

                command.Parameters.AddWithValue("@villainId", villainId);
                return command.ExecuteNonQuery();
            }
        }

        private static int DeleteVillainById(int id)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "DELETE FROM Villains WHERE Id = @id";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);

                command.Parameters.AddWithValue("@id", id);
                return command.ExecuteNonQuery();
            }
        }

        static void Task5()
        {
            int villainId = int.Parse(Console.ReadLine());

            if (!IsVillainExistById(villainId))
            {
                Console.WriteLine("Такой злодей не найден.");
                return;
            }

            var villainName = GetVillainNameById(villainId);

            var minionsCount = DeleteMinionsVillainsByVillainId(villainId);

            DeleteVillainById(villainId);

            Console.WriteLine($"{villainName} был удалён. {minionsCount} миньонов было освобождено.");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static int IncrementMinionsAge(int[] ids)
        {
            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = $"UPDATE Minions SET Age=Age+1 WHERE Id IN ({string.Join(", ", ids)})";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);

                return command.ExecuteNonQuery();
            }
        }

        static void Task6()
        {
            var minionsIds = Console.ReadLine().Split(' ').Select(int.Parse);
            var updated = IncrementMinionsAge(minionsIds.ToArray());

            Console.WriteLine($"{updated} строк обновлено.");

            SqlConnection connection = new SqlConnection(connectionStr);
            connection.Open();
            using (connection)
            {
                string selectionCmdStr = "SELECT Minions.Name as MinionName, Minions.Age as MinionAge FROM Minions";
                SqlCommand command = new SqlCommand(selectionCmdStr, connection);
                SqlDataReader reader = command.ExecuteReader();
                int number = 1;
                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{number++}. {reader["MinionName"]} {reader["MinionAge"]}");
                    }
                }
            }
        }
        static void Main(string[] args)
        {
        }
    }
}

