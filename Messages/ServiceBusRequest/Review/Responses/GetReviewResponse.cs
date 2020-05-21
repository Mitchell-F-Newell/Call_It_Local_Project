using Messages.DataTypes.Database.CompanyDirectory;

using System;

namespace Messages.ServiceBusRequest.Review.Responses
{
    [Serializable]
    public class GetCompanyInfoResponse : ServiceBusResponse
    {
        public GetCompanyInfoResponse(bool result, string response, CompanyInstance companyInfo)
            : base(result, response)
        {
            this.companyInfo = companyInfo;
        }

        /// <summary>
        /// Contains information about the company the client requested
        /// </summary>
        public CompanyInstance companyInfo;
    }
}
