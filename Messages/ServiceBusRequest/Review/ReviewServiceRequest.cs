using System;

namespace Messages.ServiceBusRequest.Review
{
    [Serializable]
    public class ReviewServiceRequest : ServiceBusRequest
    {
        public ReviewServiceRequest(ReviewRequest requestType)
            : base(Service.CompanyReview)
        {
            this.requestType = requestType;
        }

        /// <summary>
        /// Indicates the type of request the client is seeking from the Company Directory Service
        /// </summary>
        public ReviewRequest requestType;
    }

    public enum ReviewRequest { AddReview, GetReview };
}
