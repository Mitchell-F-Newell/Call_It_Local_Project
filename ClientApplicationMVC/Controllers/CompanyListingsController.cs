﻿using ClientApplicationMVC.Models;

using Messages.DataTypes.Database.CompanyDirectory;
using Messages.ServiceBusRequest.CompanyDirectory.Responses;
using Messages.ServiceBusRequest.CompanyDirectory.Requests;
using Messages.ServiceBusRequest.AccuWeather.Responses;
using Messages.ServiceBusRequest.AccuWeather.Requests;

using System;
using System.Web.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Web.Routing;

namespace ClientApplicationMVC.Controllers
{
    /// <summary>
    /// This class contains the functions responsible for handling requests routed to *Hostname*/CompanyListings/*
    /// </summary>
    public class CompanyListingsController : Controller
    {
        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult Index()
        {
            if (Globals.isLoggedIn())
            {
                ViewBag.Companylist = null;
                return View("Index");
            }
            return RedirectToAction("Index", "Authentication");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/Search
        /// </summary>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult Search(string textCompanyName)
        {
            System.Diagnostics.Debug.WriteLine(textCompanyName + "\n\n\n\n\n");
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            CompanySearchRequest request = new CompanySearchRequest(textCompanyName);

            CompanySearchResponse response = connection.searchCompanyByName(request);
            if (response.result == false)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.Companylist = response.list;

            return View("Index");
        }

        /// <summary>
        /// This function is called when the client navigates to *hostname*/CompanyListings/DisplayCompany/*info*
        /// </summary>
        /// <param name="id">The name of the company whos info is to be displayed</param>
        /// <returns>A view to be sent to the client</returns>
        public ActionResult DisplayCompany(string id, string responseStatus = null)
        {
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            if ("".Equals(id))
            {
                return View("Index");
            }

            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                return RedirectToAction("Index", "Authentication");
            }

            ViewBag.CompanyName = id;
            ViewBag.ResponseStatus = responseStatus;

            GetCompanyInfoRequest infoRequest = new GetCompanyInfoRequest(new CompanyInstance(id));
            GetCompanyInfoResponse infoResponse = connection.getCompanyInfo(infoRequest);
            ViewBag.CompanyInfo = infoResponse.companyInfo;

            GetWeatherInfoRequest AccuWeatherRequest = new GetWeatherInfoRequest(infoResponse.companyInfo.locations[0]);
            GetWeatherInfoResponse AccuWeatherResponse = connection.getAccuWeatherInfo(AccuWeatherRequest);
            ViewBag.AccuWeatherInfo = AccuWeatherResponse.data;

            return View("DisplayCompany");
        }

        public ActionResult SubmitReview(string review, string companyName, string stars)
        {
            //Check if user is logged in
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            //Check that all info was provided
            if (review != "" && companyName != "" && stars != "")
            {
                //Get username and timestamp then generate a review object
                string username = Globals.getUser();
                long timestamp = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();
                Review reviewObject = new Review()
                {
                    companyName = companyName,
                    username = username,
                    review = review,
                    stars = Int32.Parse(stars),
                    timestamp = (int)timestamp
                };
                Reviews reviewsObject = new Reviews()
                {
                    review = reviewObject
                };
                //Create client and submit post
                var companyReviewClient = new HttpClient();
                var jsonObject = JsonConvert.SerializeObject(reviewsObject);
                var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                //This url is the one where the rest call it to
                var res = companyReviewClient.PostAsync("http://35.226.157.66/Home/SaveCompanyReview", stringContent).Result.Content.ReadAsStringAsync().Result;
                Response resObject = JsonConvert.DeserializeObject<Response>(res);
                //If response bad, redirect indicating bad request, otherwise, good
                if (resObject.response == "failure")
                {
                    return RedirectToAction("DisplayCompany", new { id = companyName, responseStatus = "Error occured when submitting review" });
                }
                else
                    return RedirectToAction("DisplayCompany", new { id = companyName, responseStatus = "Review successfully saved" });
            }
            else
                return View("Index", "Home");
        }


        public ActionResult DisplayReviews(string id)
        {
            //Check if user is logged in
            if (Globals.isLoggedIn() == false)
            {
                return RedirectToAction("Index", "Authentication");
            }
            //Check the the company name was not null
            if ("".Equals(id))
            {
                return View("Index");
            }
            ViewBag.CompanyName = id;
            ServiceBusConnection connection = ConnectionManager.getConnectionObject(Globals.getUser());
            if (connection == null)
            {
                //If can't connect to db, return false to view
                ViewBag.CompanyExists = false;
                return View("DisplayReviews");
            }

            GetCompanyInfoRequest infoRequest = new GetCompanyInfoRequest(new CompanyInstance(id));
            GetCompanyInfoResponse infoResponse = connection.getCompanyInfo(infoRequest);
            if (!infoResponse.result)
            {
                //If company doesn't exist in db, return false to view
                ViewBag.CompanyExists = false;
                return View("DisplayReviews");
            }
            //Otherwise company eixsts
            ViewBag.CompanyExists = true;

            var companyReviewClient = new HttpClient();
            var jsonObject = JsonConvert.SerializeObject(new CompanyName() { companyName = id });
            var stringContent = new StringContent(jsonObject, Encoding.UTF8, "application/json");
            //This url is the one where the rest call it to
            var res = companyReviewClient.PostAsync("http://35.226.157.66/Home/GetCompanyReview", stringContent).Result.Content.ReadAsStringAsync().Result;
            GetResponse resObject = JsonConvert.DeserializeObject<GetResponse>(res);

            int? totalStars = 0;
            int totalReviews = 0;
            string avgStars = "0.00";
            //Check if any reviews were returned
            if (resObject.reviews != null)
            {
                //Loop reviews calculating avg stars
                foreach (var review in resObject.reviews)
                {
                    totalStars += review.stars;
                    totalReviews++;
                }
                avgStars = ((float)totalStars / totalReviews).ToString("0.00");
            }
            //Pass all to display
            ViewBag.AvgStars = avgStars;
            ViewBag.ReviewList = resObject.reviews;
            return View("DisplayReviews");
        }

    }
}