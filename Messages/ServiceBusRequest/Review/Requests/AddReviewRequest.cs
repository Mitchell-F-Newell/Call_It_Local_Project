using NServiceBus;

using System;

namespace Messages.ServiceBusRequest.Review.Requests
{
    [Serializable]
    public class AddReviewRequest : ReviewServiceRequest
    {
        public AddReviewRequest(string companyName, string username, string review, int stars, int timestamp)
            : base(ReviewRequest.AddReview)
        {
            this.companyName = companyName;
            this.username = username;
            this.review = review;
            this.stars = stars;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Information used to search the database for companies
        /// </summary>
        public string companyName;
        public string username;
        public string review;
        public int stars;
        public int timestamp;
    }
}
