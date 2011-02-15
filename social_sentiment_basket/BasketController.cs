using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml;

namespace social_sentiment_basket {
    public class BasketController {
        public Basket AddToBasket(Guid basketId, Track track) {
            var addToBasketUrl = string.Format("http://api.7digital.com/1.2/basket/additem?basketid={0}&releaseid={1}&trackid={2}&country=GB&oauth_consumer_key=test-api",
                                               basketId, track.ReleaseId, track.TrackId);
            var addToBasketRequest = WebRequest.Create(addToBasketUrl);
            using (var webResponse = addToBasketRequest.GetResponse()) {
                var addToBasketResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                var addToBasket = new XmlDocument();
                addToBasket.LoadXml(addToBasketResponse);
                var status = addToBasket.SelectSingleNode("/response").Attributes["status"].InnerText;
                if (status != "ok") {
                    throw new InvalidOperationException(string.Format("Could not add track: {0}", track));
                }
                return ParseBasket(addToBasket);
            }
        }

        public Basket DeleteFromBasket(Guid basketId, BasketItem item) {
            var addToBasketUrl = string.Format("http://api.7digital.com/1.2/basket/removeitem?basketid={0}&itemId={1}&country=GB&oauth_consumer_key=test-api",
                                               basketId, item.ItemId);
            var addToBasketRequest = WebRequest.Create(addToBasketUrl);
            using (var webResponse = addToBasketRequest.GetResponse()) {
                var addToBasketResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                var addToBasket = new XmlDocument();
                addToBasket.LoadXml(addToBasketResponse);
                var status = addToBasket.SelectSingleNode("/response").Attributes["status"].InnerText;
                if (status != "ok") {
                    throw new InvalidOperationException(string.Format("Could not delete track: {0}", item));
                }
                return ParseBasket(addToBasket);
            }
        }

        public Basket ParseBasket(XmlDocument basket) {

            var id = basket.SelectSingleNode("/response/basket").Attributes["id"].InnerText;
            var formattedPrice = basket.SelectSingleNode("/response/basket/price/formattedPrice").InnerText;
            XmlNodeList nodes = basket.SelectNodes("//basketItem");
            var items = new List<BasketItem>();
            foreach (XmlNode xmlNode in nodes) {
                string itemId = xmlNode.Attributes["id"].InnerText;
                string trackName = xmlNode.SelectSingleNode("itemName").InnerText;
                string artistName = xmlNode.SelectSingleNode("artistName").InnerText;
                string trackId = xmlNode.SelectSingleNode("trackId").InnerText;
                string releaseId = xmlNode.SelectSingleNode("releaseId").InnerText;
                var item = new BasketItem(int.Parse(itemId), int.Parse(trackId), int.Parse(releaseId), artistName, trackName);
                items.Add(item);
            }

            return new Basket(new Guid(id), formattedPrice, items);
        }

        public Guid CreateBasket() {
            var createBasketUrl = "http://api.7digital.com/1.2/basket/create?country=GB&oauth_consumer_key=test-api";
            var createBasketRequest = WebRequest.Create(createBasketUrl);
            using (var webResponse = createBasketRequest.GetResponse()) {
                var createBasketResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                var newBasket = new XmlDocument();
                newBasket.LoadXml(createBasketResponse);
                var basketId = newBasket.SelectSingleNode("/response/basket").Attributes["id"].InnerText;
                Console.WriteLine("Basket Id: {0}", basketId);
                return new Guid(basketId);
            }
        }

    }
}