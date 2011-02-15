using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

            //var list = new List<string> {
            //                                "listening to \"R Kelly - Love Letter\" http://tumblr.com/x8412ul766",
            //                                "I am listening to \"The Skeletons - Positive Force\" on Jazz Masters Channel via 1CLUB.FM Radio Station http://1club.fm",
            //                                "♥ If You Leave Her by J.J. Cale #lastfm: http://bit.ly/fdo9DW",
            //                                "♥ Karma Police by Radiohead #lastfm: http://bit.ly/fdo9DW",
            //                                "♥ Epic by Faith No More #lastfm: http://bit.ly/fdo9DW"
            //                            }
            //    ;

namespace social_sentiment_basket {
    public class SocialBasketSentiment {

        private readonly List<Track> _tracks = new List<Track>();
        private int _sequenceId;

        public void Start() {
            var basketController = new BasketController();

            while (true) {
                Console.Clear();
                Console.WriteLine("Scanning twitter...");

                int currentTweetCount = _tracks.Sum(t => t.Count);
               
                var tweets = GrabTweets();

                Console.WriteLine("Found {0} tweets", tweets.Count());

                var tracks = SearchEchoNestApi(tweets);
                AggregateTracks(tracks);

                var newTracks = _tracks.Sum(t => t.Count) - currentTweetCount;
                if (newTracks > 0) {
                    var sorted = _tracks.OrderByDescending(t => t.Count).ToList();
                    Console.WriteLine("{0}", DateTime.Now);
                    Console.WriteLine("New Tracks: {0}", newTracks);
                    sorted.ForEach(Console.WriteLine);
                } else {
                    Console.WriteLine("No new matches...");
                }

                System.Threading.Thread.Sleep(30000);
            }
            //var basketId = basketController.CreateBasket();
            //Basket basket = null;

            //tracks.ForEach(t => basket = basketController.AddToBasket(basketId, t));
            //Console.WriteLine("Adding tweeted tracks to basket");
            //Console.WriteLine(basket.ToString());

            //Basket updatedBasket = basketController.DeleteFromBasket(basketId, basket.Items.Last());
            //Console.WriteLine("Deleteing last tweeted track form basket");

            //Console.WriteLine(updatedBasket.ToString());
        }

        private void AggregateTracks(IEnumerable<Track> tracks) {
            foreach (var track in tracks) {
                var existingTrack = _tracks.Find(t => t.TrackId == track.TrackId);
                if (existingTrack != null) { existingTrack.Count++; } 
                else _tracks.Add(track);
            }
        }

        private IEnumerable<string> GrabTweets() {
			
			string searchUrl = string.Format(Config.DataSiftApiStream, Config.DataSiftApiStreamIdentifier, Config.DataSiftApiUsername, Config.DataSiftApiKey, _sequenceId); 

            var searchRequest = WebRequest.Create(searchUrl);
            var tweetStreamResult = new XmlDocument();

            using (var webResponse = searchRequest.GetResponse()) {
                var searchResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                tweetStreamResult.LoadXml(searchResponse);
            }

            var sequenceID = tweetStreamResult.SelectSingleNode("//sequence_id");
            if (sequenceID != null)
                _sequenceId = int.Parse(sequenceID.InnerText);

            var xmlNodeList = tweetStreamResult.SelectNodes("//content");
            var list = new List<string>();
            foreach (XmlNode node in xmlNodeList) {
                list.Add(node.InnerText);
            }

            return list;
        }

        private List<Track> SearchApi(IEnumerable<string> tweets) {
            var results = new List<Track>();
            foreach(var tweet in tweets) {
                var track = ParseTweet(tweet);

                if(string.IsNullOrEmpty(track)) {
                    Console.WriteLine("No track for {0}", tweet);
                    continue;
                }
				
				string searchUrl = string.Format(Config.SevenDigitalApiTrackSearch, track, Config.SevenDigitalConsumerKey);
                var searchRequest = WebRequest.Create(searchUrl);
                using (var webResponse = searchRequest.GetResponse()) {
                    var searchResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    var searchResults = new XmlDocument();
                    searchResults.LoadXml(searchResponse);
                    var topResult = searchResults.SelectSingleNode("/response/searchResults/searchResult/track");

                    if(topResult == null) {
                        Console.WriteLine("Could not find anything for {0}", track);
                        continue;
                    }

                    Track sentiment = ParseTrack(topResult);
                    results.Add(sentiment);
                }
            }
            return results;
        }

		private static Track GetTrackFromApi(string trackId) {

			string searchUrl = string.Format(Config.SevenDigitalApiTrackDetails, trackId, Config.SevenDigitalConsumerKey);

            var searchRequest = WebRequest.Create(searchUrl);
            using (var webResponse = searchRequest.GetResponse()) {
                var searchResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                var searchResults = new XmlDocument();
                searchResults.LoadXml(searchResponse);
                if (searchResults.SelectSingleNode("response").Attributes["status"].InnerText != "ok")
                    return null;
                var result = searchResults.SelectSingleNode("/response/track");
                
                return ParseTrack(result);
            }
        }

        private IEnumerable<Track> SearchEchoNestApi(IEnumerable<string> tweets) {
            var results = new List<Track>();
            foreach (var tweet in tweets) {
                var track = ParseTweet(tweet);

                if (string.IsNullOrEmpty(track)) {
                    continue;
                }

            	string searchUrl = string.Format(Config.EchoNestApiSongSearch, Config.EchoNestApiKey, track);
                var searchRequest = WebRequest.Create(searchUrl);
                using (var webResponse = searchRequest.GetResponse()) {
                    var searchResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    var searchResults = new XmlDocument();

                    try {
                        searchResults.LoadXml(searchResponse);
                    } catch (Exception e) {
                        continue;
                    }
                    var topResult = searchResults.SelectSingleNode("//track/foreign_id");
                    if (topResult == null) continue;

                    string trackId = topResult.InnerText.Replace("7digital:track:", "");

                    Track sentiment = GetTrackFromApi(trackId);

                    if (sentiment == null) {
                        Console.WriteLine("Could not find anything for {0}", track);
                        continue;
                    }

                    results.Add(sentiment);
                }
            }
            return results;
        }

        private static Track ParseTrack(XmlNode topResult) {
            var trackId = topResult.Attributes["id"].InnerText;
            var trackName = topResult.SelectSingleNode("title").InnerText;
            var artistName = topResult.SelectSingleNode("artist/name").InnerText;
            var releaseId = topResult.SelectSingleNode("release").Attributes["id"].InnerText;

            return new Track(Convert.ToInt32(trackId), Convert.ToInt32(releaseId), artistName, trackName);
        }

        private Track ParseEchoNestTrack(XmlNode topResult) {
            var trackId = topResult.Attributes["id"].InnerText;
            var trackName = topResult.SelectSingleNode("title").InnerText;
            var artistName = topResult.SelectSingleNode("artist/name").InnerText;
            var releaseId = topResult.SelectSingleNode("release").Attributes["id"].InnerText;

            return new Track(Convert.ToInt32(trackId), Convert.ToInt32(releaseId), artistName, trackName);
        }

        private static string ParseTweet(string tweet) {
            tweet = tweet.Replace("listening to ", "");
            const string pattern = "\"[\\D]+?\"";
            const string lastFmPattern = "♥[\\D]+?#";
            string trackAndArtist;

            if(Regex.IsMatch(tweet, pattern))
                trackAndArtist = Regex.Match(tweet, pattern).Value;
            else 
                trackAndArtist = Regex.Match(tweet, lastFmPattern).Value;

            trackAndArtist = trackAndArtist.Replace("\"", "");
            trackAndArtist = trackAndArtist.Replace("#", "");
            trackAndArtist = trackAndArtist.Replace("♥", "");
			
            return trackAndArtist.Trim();
        }
    }
}