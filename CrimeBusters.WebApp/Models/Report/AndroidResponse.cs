using System;

namespace CrimeBusters.WebApp.Models.Report
{
    /// <summary>
    /// Data class for the Android Response.
    /// </summary>
    public class AndroidResponse
    {
        public String multicast_id { get; set; }
        public int success { get; set; }
        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public AndroidResponseResult[] results { get; set; }
    }

    /// <summary>
    /// Data class for the ResponseKvp for Android
    /// </summary>
    public class AndroidResponseResult
    {
        public String message_id { get; set; }
        public String registration_id { get; set; }
        public String error { get; set; }
    }
}