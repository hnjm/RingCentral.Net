using System.Threading.Tasks;

namespace RingCentral.Paths.Restapi.Account.Extension.ForwardingNumber
{
    public partial class Index
    {
        public RestClient rc;
        public string forwardingNumberId;
        public Restapi.Account.Extension.Index parent;

        public Index(Restapi.Account.Extension.Index parent, string forwardingNumberId = null)
        {
            this.parent = parent;
            this.rc = parent.rc;
            this.forwardingNumberId = forwardingNumberId;
        }

        public string Path(bool withParameter = true)
        {
            if (withParameter && forwardingNumberId != null)
            {
                return $"{parent.Path()}/forwarding-number/{forwardingNumberId}";
            }

            return $"{parent.Path()}/forwarding-number";
        }

        /// <summary>
        /// Operation: Get Forwarding Number List
        /// Http Get /restapi/v1.0/account/{accountId}/extension/{extensionId}/forwarding-number
        /// </summary>
        public async Task<RingCentral.GetExtensionForwardingNumberListResponse> List(
            ListForwardingNumbersParameters queryParams = null)
        {
            return await rc.Get<RingCentral.GetExtensionForwardingNumberListResponse>(this.Path(false), queryParams);
        }

        /// <summary>
        /// Operation: Create Forwarding Number
        /// Http Post /restapi/v1.0/account/{accountId}/extension/{extensionId}/forwarding-number
        /// </summary>
        public async Task<RingCentral.ForwardingNumberInfo> Post(
            RingCentral.CreateForwardingNumberRequest createForwardingNumberRequest)
        {
            return await rc.Post<RingCentral.ForwardingNumberInfo>(this.Path(false), createForwardingNumberRequest);
        }

        /// <summary>
        /// Operation: Get Forwarding Number
        /// Http Get /restapi/v1.0/account/{accountId}/extension/{extensionId}/forwarding-number/{forwardingNumberId}
        /// </summary>
        public async Task<RingCentral.ForwardingNumberResource> Get()
        {
            if (this.forwardingNumberId == null)
            {
                throw new System.ArgumentNullException("forwardingNumberId");
            }

            return await rc.Get<RingCentral.ForwardingNumberResource>(this.Path());
        }

        /// <summary>
        /// Operation: Update Forwarding Number
        /// Http Put /restapi/v1.0/account/{accountId}/extension/{extensionId}/forwarding-number/{forwardingNumberId}
        /// </summary>
        public async Task<RingCentral.ForwardingNumberInfo> Put(
            RingCentral.UpdateForwardingNumberRequest updateForwardingNumberRequest)
        {
            if (this.forwardingNumberId == null)
            {
                throw new System.ArgumentNullException("forwardingNumberId");
            }

            return await rc.Put<RingCentral.ForwardingNumberInfo>(this.Path(), updateForwardingNumberRequest);
        }

        /// <summary>
        /// Operation: Delete Forwarding Number
        /// Http Delete /restapi/v1.0/account/{accountId}/extension/{extensionId}/forwarding-number/{forwardingNumberId}
        /// </summary>
        public async Task<string> Delete()
        {
            if (this.forwardingNumberId == null)
            {
                throw new System.ArgumentNullException("forwardingNumberId");
            }

            return await rc.Delete<string>(this.Path());
        }
    }
}

namespace RingCentral.Paths.Restapi.Account.Extension
{
    public partial class Index
    {
        public Restapi.Account.Extension.ForwardingNumber.Index ForwardingNumber(string forwardingNumberId = null)
        {
            return new Restapi.Account.Extension.ForwardingNumber.Index(this, forwardingNumberId);
        }
    }
}