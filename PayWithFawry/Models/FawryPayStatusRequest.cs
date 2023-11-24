namespace PayWithFawry.Models
{
    public class FawryPayStatusRequest
    {
        public string merchantCode { get; set; }
        public string merchantRefNumber { get; set; }
        public string signature { get; set; }
    }
}
