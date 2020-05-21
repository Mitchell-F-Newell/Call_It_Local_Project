using Messages;
using Messages.Database;
using Messages.DataTypes;
using Messages.DataTypes.Database.Chat;
using Messages.NServiceBus.Events;
using Messages.ServiceBusRequest.Chat.Responses;
using Messages.ServiceBusRequest.Chat.Requests;
using Messages.NServiceBus.Commands;
using Messages.ServiceBusRequest;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Database
{
    /// <summary>
    /// This portion of the class contains methods and functions
    /// </summary>
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private ChatServiceDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static ChatServiceDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new ChatServiceDatabase();
            }
            return instance;
        }

        public GetChatContactsResponse getContacts(string userName)
        {
            string responseMessage = "";
            bool result = false;

            GetChatContacts userResponseData = new GetChatContacts()
            {
                usersname = userName,
                contactNames = new List<string>()
            };

            if(openConnection() == true)
            {
                List<string> userContacts = new List<String>();

                string query = @"SELECT DISTINCT receiver FROM chatdata WHERE sender = '" + userName + @"'";
                MySqlCommand doQuery = new MySqlCommand(query, connection);
                MySqlDataReader incomingData = doQuery.ExecuteReader();

                while (incomingData.Read())
                {
                    userContacts.Add(incomingData.GetString("receiver"));
                }
                incomingData.Close();

                query = @"SELECT DISTINCT sender FROM chatdata WHERE receiver = '" + userName + @"'";
                doQuery = new MySqlCommand(query, connection);
                incomingData = doQuery.ExecuteReader();

                String userContact;
                while (incomingData.Read())
                {
                    userContact = incomingData.GetString("sender");
                    if (!userContacts.Contains(userContact))
                    {
                        userContacts.Add(userContact);
                    }
                }
                incomingData.Close();

                if(userContacts.Count() != 0)
                {
                    result = true;
                    userResponseData.contactNames = userContacts;
                }
                else
                {
                    responseMessage = "No user contacts were found.";
                }

                incomingData.Close();
                closeConnection();
            }
            else
            {
                responseMessage = "Error connecting to the database.";
                Debug.consoleMsg("Error connecting to the database.");
            }

            return new GetChatContactsResponse (result, responseMessage, userResponseData);
        }

        public GetChatHistoryResponse getChatHistory(ChatHistory chatHist)
        {
            string responseMessage = "";
            GetChatHistory userChatHistory = new GetChatHistory();
            bool result = false;

            if(openConnection() == true)
            {
                String query = @"SELECT * FROM chatdata WHERE (receiver = '" + chatHist.user1 + @"' AND 
                sender = '" + chatHist.user2 + @"') OR " + @"(receiver = '" + chatHist.user2 + @"' AND 
                sender = '" + chatHist.user1 + @"')";

                MySqlCommand doQuery = new MySqlCommand(query, connection);
                MySqlDataReader incomingData = doQuery.ExecuteReader();

                List<ChatMessage> userMessages = new List<ChatMessage>();

                    while (incomingData.Read())
                    {
                        result = true;
                        ChatMessage msg = new ChatMessage();
                        msg.receiver = incomingData.GetString("receiver");
                        msg.sender = incomingData.GetString("sender");
                        msg.unix_timestamp = incomingData.GetInt32("timestamp");
                        msg.messageContents = incomingData.GetString("message");
                        userMessages.Add(msg);
                    }
                    chatHist.messages = userMessages;
                    userChatHistory.history = chatHist;
               
                if (result == false) {
                    responseMessage = "No chat history was found between users.";
                }
                incomingData.Close();
                closeConnection();
            }
            else
            {
                responseMessage = "Error connecting to the database.";
                Debug.consoleMsg("Error connecting to the database.");
            }
            return new GetChatHistoryResponse(result, responseMessage, userChatHistory);
        }

        public ServiceBusResponse  sendMessage(ChatMessage newMessage)
        {
            bool result = false;
            string responseMessage = "";
            
            if(openConnection() == true)
            {
                string query = @"INSERT INTO chatdata(sender, receiver, timestamp, message)"
                + @"VALUES('" + newMessage.sender + @"' , '" + newMessage.receiver + @"','" +
                newMessage.unix_timestamp + @"','" + newMessage.messageContents + @"')";

                MySqlCommand doQuery = new MySqlCommand(query, connection);
                doQuery.ExecuteNonQuery();
                result = true;

                closeConnection();

            }
            else
            {
                responseMessage = "Error connecting to the database.";
                Debug.consoleMsg("Error connecting to the database.");
            }

            return new ServiceBusResponse(result, responseMessage);
        }
    }

    /// <summary>
    /// This portion of the class contains the properties and variables 
    /// </summary>
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "Chatdb";
        public override string databaseName { get; } = dbname;

        /// <summary>
        /// The singleton isntance of the database
        /// </summary>
        protected static ChatServiceDatabase instance = null;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
            (
                dbname,
                "chatdata",
                new Column[]
                {
                    new Column
                    (
                        "id", "INT",
                        new string[]
                        {
                         "NOT NULL AUTO_INCREMENT",
                         "UNIQUE"
                        }, true
                    ),
                    new Column
                    (
                        "sender", "VARCHAR(50)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                    new Column
                    (
                        "receiver", "VARCHAR(30)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                    new Column
                    (
                        "timestamp", "INT(32)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                    new Column
                    (
                        "message", "VARCHAR(280)",
                        new string[]
                        {
                            "NOT NULL",
                        }, false
                    ),
                }
            )
        };
    }
}
