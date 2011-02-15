using System.Text;

namespace social_sentiment_basket {
    public class BasketItem : Track {
        public int ItemId { get; private set; }

        public BasketItem(int itemId, int trackId, int releaseId, string artistName, string trackName) 
            : base(trackId, releaseId, artistName, trackName) {
            ItemId = itemId;
        }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} | {1}:{2}\t{3} - {4}", ItemId, TrackId, ReleaseId, ArtistName, TrackName);
            return stringBuilder.ToString();
        }
    }
}