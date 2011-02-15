using System.Configuration;

namespace social_sentiment_basket {
	public static class Config {

		public static string SevenDigitalApiTrackSearch { get; set; }
		public static string SevenDigitalApiTrackDetails { get; set; }
		public static string SevenDigitalConsumerKey { get; set; }
		public static string EchoNestApiSongSearch { get; set; }
		public static string EchoNestApiKey { get; set; }
		public static string DataSiftApiStream { get; set; }
		public static string DataSiftApiStreamIdentifier { get; set; }
		public static string DataSiftApiUsername { get; set; }
		public static string DataSiftApiKey { get; set; }

		static Config() {
			SevenDigitalApiTrackSearch = ConfigurationManager.AppSettings["SevenDigitalApiTrackSearch"];
			SevenDigitalApiTrackDetails = ConfigurationManager.AppSettings["SevenDigitalApiTrackDetails"];
			SevenDigitalConsumerKey = ConfigurationManager.AppSettings["SevenDigitalConsumerKey"];
			EchoNestApiSongSearch = ConfigurationManager.AppSettings["EchoNestApiSongSearch"];
			EchoNestApiKey = ConfigurationManager.AppSettings["EchoNestApiKey"];
			DataSiftApiStream = ConfigurationManager.AppSettings["DataSiftApiStream"];
			DataSiftApiStreamIdentifier = ConfigurationManager.AppSettings["DataSiftApiStreamIdentifier"];
			DataSiftApiUsername = ConfigurationManager.AppSettings["DataSiftApiUsername"];
			DataSiftApiKey = ConfigurationManager.AppSettings["DataSiftApiKey"];
		}
	}
}
