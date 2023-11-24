namespace PayWithFawry.Models
{
    public class FawrypayRequest
    {
        public string merchantCode { get; set; }
        public string merchantRefNum { get; set; }
        public string customerProfileId { get; set; }
        public string customerName { get; set; }
        public string customerMobile { get; set; }
        public string customerEmail { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string language { get; set; }
        public List<ChargeItems> chargeItems { get; set; }
        public string signature { get; set; }
        public string paymentMethod { get; set; }
        public string description { get; set; }

    }
}
