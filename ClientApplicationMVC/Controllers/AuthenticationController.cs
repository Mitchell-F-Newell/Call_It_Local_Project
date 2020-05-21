using ClientApplicationMVC.Models;

using Messages.NServiceBus.Commands;
using Messages.DataTypes;
using Messages.ServiceBusRequest;
using Messages.ServiceBusRequest.Authentication.Requests;

using System.Web.Mvc;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/Authentication/*
    /// </summary>
    public class AuthenticationController : Controller
    {
        /// <summary>
        /// The default method for this controller
        /// </summary
        /// <returns>The login page</returns>

        public ActionResult Index()
        {

            ViewBag.Message = "Please enter your username and password.";
            return View("Index");
        }

        public ActionResult LoggedInIndex()
        {

            return View("LoggedInIndex");
        }

        public ActionResult CreateAccount()
        {
            return View("CreateAccount");
        }
        /**
        public ActionResult LoggedInIndex()
        {
            ViewBag.LoggedInMessage = "Hello " + usernameGlob + ", you have successfully logged in";
            return View("LoggedInIndex");
        }*/
        //This class is incomplete and should be completed by the students in milestone 2
        //Hint: You will need to make use of the ServiceBusConnection class. See EchoController.cs for an example.

        [HttpPost]
        public ActionResult CreateAccounts()
        {
            return View("CreateAccount");
        }

        [HttpPost]
        public ActionResult logIn(string username, string password)
        {
            LogInRequest request = new LogInRequest(username, password);
            ServiceBusResponse response = ConnectionManager.sendLogIn(request);

            if (response.response.Equals(""))
            {
                if (response.result)
                {
                    ViewBag.LoggedInMessage = "Hello " + username + ", you have successfully logged in";
                    return View("LoggedInIndex");
                    return RedirectToAction("Index", "Home", new
                    {
                        msg = " log in sucessfull"
                    });
                }
                else
                {
                    ViewBag.LogInResult = "Incorrect username or password!";
                    return View("Index");
                }
            }
            else
            {
                ViewBag.LogInResult = "Invalid log in request: " + response.response;
                return View("Index");

            }
            //ViewBag.LogInResult = response.result;

        }

        [HttpPost]
        public ActionResult createAccountEntry(string username, string password, string address, string phonenumber, string email, string accountType)
        {
            var account = AccountType.notspecified;
            switch (accountType)
            {
                case "user":
                    account = AccountType.user;
                    break;
                case "business":
                    account = AccountType.business;
                    break;
            }
            phonenumber = phonenumber.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
            CreateAccountRequest request = new CreateAccountRequest(new CreateAccount()
            {
                username = username,
                password = password,
                address = address,
                email = email,
                phonenumber = phonenumber,
                type = account
            });
            ServiceBusResponse response = ConnectionManager.sendNewAccountInfo(request);
            if (response.result)
                ViewBag.CreateAccountResponse = "Account successfully created. You are now logged in";
            return View("CreateAccount");
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            //Models.isLoggedin();
            return View("Index");
        }
    }
}