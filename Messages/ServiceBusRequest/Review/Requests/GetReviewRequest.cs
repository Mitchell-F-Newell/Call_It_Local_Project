using Messages.DataTypes.Database.CompanyDirectory;

using System;

namespace Messages.ServiceBusRequest.Review.Requests
{
    [Serializable]
    public class GetReviewRequest : ReviewServiceRequest
    {
        public GetReviewRequest(string companyName)
            : base(ReviewRequest.GetReview)
        {
            this.companyName = companyName;
        }

        /// <summary>
        /// Contains information needed to locate additional information about the company the client is requesting
        /// </summary>
        public string companyName;
    }
}
