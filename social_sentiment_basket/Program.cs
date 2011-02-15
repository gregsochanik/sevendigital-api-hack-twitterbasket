using System;

namespace social_sentiment_basket {
    class Program {
        static void Main(string[] args) {
            try {
                new SocialBasketSentiment().Start();
            } catch (Exception e) {
                Console.WriteLine(e);
            }
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
