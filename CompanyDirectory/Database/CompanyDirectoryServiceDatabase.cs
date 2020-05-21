using Messages;
using Messages.Database;
using Messages.DataTypes;
using Messages.DataTypes.Database.CompanyDirectory;
using Messages.NServiceBus.Events;
using Messages.ServiceBusRequest.CompanyDirectory.Responses;
using Messages.ServiceBusRequest.Echo.Requests;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyDirectoryService.Database
{
    /// <summary>
    /// This portion of the class contains methods and functions
    /// </summary>
    public partial class CompanyDirectoryServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private CompanyDirectoryServiceDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static CompanyDirectoryServiceDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new CompanyDirectoryServiceDatabase();
            }
            return instance;
        }

        /// <summary>
        /// Saves the foreward echo to the database
        /// </summary>
        /// <param name="account">Information about the echo</param>
        public void saveBusiness(AccountCreated account)
        {
            if (openConnection() == true)
            {
                string query = @"INSERT INTO businesses(username, address, phonenumber, email)" +
                   @"VALUES('" + account.username + @"', '" + account.address + @"', '" + account.phonenumber +
                   @"', '" + account.email + @"');";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();

                closeConnection();
            }
            else
            {
                Debug.consoleMsg("Unable to connect to database");
            }
        }

        public CompanySearchResponse searchCompanyInfo(string companyName)
        {
            string responseMessage = "";
            CompanyList businessList = new CompanyList();
            bool result = false;

            if (openConnection() == true)
            {
                string query = @"SELECT b.username FROM businesses as b WHERE b.username LIKE '%" + companyName + @"%'";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                List<string> companies = new List<string>();
                if (reader.Read())
                {
                    result = true;
                    companies.Add(reader.GetString("username"));
                    while (reader.Read())
                    {
                        companies.Add(reader.GetString("username"));
                    }
                    businessList.companyNames = companies.ToArray();
                }
                else
                {
                    responseMessage = "No businesses with the name '" + companyName + "' found";
                }
                reader.Close();
                closeConnection();
            }
            else
            {
                responseMessage = "Could not establish connection to the database";
                Debug.consoleMsg("Could not establish connection to the database");
            }
            return new CompanySearchResponse(result, responseMessage, businessList);
        }

        public GetCompanyInfoResponse getCompanyInfo(string companyName)
        {

            string message = "";
            CompanyInstance company = new CompanyInstance(companyName);
            bool result = false;

            if (openConnection() == true)
            {
                string query = @"SELECT * FROM businesses as b WHERE b.username='" + companyName + @"'";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    result = true;
                    company.companyName = reader.GetString("username");
                    company.email = reader.GetString("email");
                    company.locations = new string[] { reader.GetString("address") };
                    company.phoneNumber = reader.GetString("phonenumber");
                }
                else
                {
                    message = "No businesses with the name '" + companyName + "' found";
                }
                closeConnection();
            }
            else
            {
                message = "Could not establish connection to the database";
                Debug.consoleMsg("Could not establish connection to the database");
            }
            return new GetCompanyInfoResponse(result, message, company);
        }
    }

    /// <summary>
    /// This portion of the class contains the properties and variables 
    /// </summary>
    public partial class CompanyDirectoryServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "CompanyDirectorydb";
        public override string databaseName { get; } = dbname;

        /// <summary>
        /// The singleton isntance of the database
        /// </summary>
        protected static CompanyDirectoryServiceDatabase instance = null;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
            (
                dbname,
                "businesses",
                new Column[]
                {
                    new Column
                    (
                        "username", "VARCHAR(30)",
                        new string[]
                        {
                            "NOT NULL",
                            "UNIQUE"
                        }, true
                    ),
                    new Column
                    (
                        "address", "VARCHAR(100)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                    new Column
                    (
                        "phonenumber", "VARCHAR(20)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                    new Column
                    (
                        "email", "VARCHAR(40)",
                        new string[]
                        {
                            "NOT NULL",
                            "UNIQUE"
                        }, false
                    ),
                }
            ),
        };
    }
}
