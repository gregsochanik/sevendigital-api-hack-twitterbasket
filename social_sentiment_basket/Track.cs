using System.Text;

namespace social_sentiment_basket {
    public class Track {
        public Track(int trackId, int releaseId, string artistName, string trackName) {
            TrackId = trackId;
            ReleaseId = releaseId;
            ArtistName = artistName;
            TrackName = trackName;
            Count = 1;
        }

        public int TrackId { get; private set; }
        public int ReleaseId { get; private set; }
        public string ArtistName { get; private set; }
        public string TrackName { get; private set; }
        public int Count { get; set; }

        public override string ToString() {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0}:{1}\t{2} - {3}\tCount: {4}", TrackId, ReleaseId, ArtistName, TrackName, Count);
            return stringBuilder.ToString();
        }
    }
}