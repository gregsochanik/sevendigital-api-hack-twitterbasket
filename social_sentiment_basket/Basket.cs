using System;
using System.Collections.Generic;
using System.Text;

namespace social_sentiment_basket {
    public class Basket {
        public Basket(Guid id, string formattedPrice, IEnumerable<BasketItem> tracks) {
            Id = id;
            FormattedPrice = formattedPrice;
            Items = tracks;
        }

        public Guid Id { get; set; }
        public IEnumerable<BasketItem> Items { get; set; }
        public string FormattedPrice { get; set; }

        public override string ToString() {
            var sb = new StringBuilder();
            foreach (var item in Items) {
                sb.AppendLine(item.ToString());
            }

            return string.Format("Basket Id: {0} Price: {1} Tracks: {2}{3}", Id, FormattedPrice, Environment.NewLine, sb);
        }
    }
}