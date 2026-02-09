using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace ConsoleExperimentsApp.Experiments.MLNet
{
    public static class MLNetExperiments
    {
        public static async Task Run()
        {
            Console.WriteLine("=== ML.NET Experiments ===");
            Console.WriteLine("Description: Demonstrating machine learning with ML.NET including classification and regression models.\n");

            await Task.Run(() =>
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Description: Demonstrates binary classification using ML.NET to predict sentiment");
                Console.WriteLine("(positive/negative) from text reviews using logistic regression.");
                Console.ResetColor();
                SentimentAnalysisExample();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Description: Shows regression analysis using ML.NET to predict house prices");
                Console.WriteLine("based on features like size and number of bedrooms.");
                Console.ResetColor();
                PricePredictionExample();
            });

            Console.WriteLine("\nML.NET experiments completed.");
        }

        private static void SentimentAnalysisExample()
        {
            Console.WriteLine("1. Sentiment Analysis (Binary Classification)");
            Console.WriteLine("----------------------------------------------");

            var mlContext = new MLContext(seed: 0);

            var sentimentData = new List<SentimentData>
            {
                new SentimentData { Text = "This is a great product!", Sentiment = true },
                new SentimentData { Text = "Absolutely love it!", Sentiment = true },
                new SentimentData { Text = "Amazing quality and fast delivery", Sentiment = true },
                new SentimentData { Text = "Best purchase I've made", Sentiment = true },
                new SentimentData { Text = "Terrible product, waste of money", Sentiment = false },
                new SentimentData { Text = "Very disappointed with quality", Sentiment = false },
                new SentimentData { Text = "Do not recommend at all", Sentiment = false },
                new SentimentData { Text = "Worst experience ever", Sentiment = false }
            };

            var trainingData = mlContext.Data.LoadFromEnumerable(sentimentData);

            var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(SentimentData.Text))
                .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(SentimentData.Sentiment),
                    featureColumnName: "Features"));

            var model = pipeline.Fit(trainingData);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);

            var testSamples = new[]
            {
                "This is fantastic!",
                "I hate this product",
                "Really good value for money"
            };

            foreach (var sample in testSamples)
            {
                var prediction = predictionEngine.Predict(new SentimentData { Text = sample });
                Console.WriteLine($"Text: '{sample}'");
                Console.WriteLine($"  → Predicted: {(prediction.Prediction ? "Positive" : "Negative")} " +
                                $"(Confidence: {prediction.Probability:P2})");
            }
        }

        private static void PricePredictionExample()
        {
            Console.WriteLine("2. House Price Prediction (Regression)");
            Console.WriteLine("---------------------------------------");

            var mlContext = new MLContext(seed: 0);

            var housingData = new List<HousingData>
            {
                new HousingData { Size = 1000f, Bedrooms = 2, Price = 200000f },
                new HousingData { Size = 1500f, Bedrooms = 3, Price = 280000f },
                new HousingData { Size = 2000f, Bedrooms = 4, Price = 350000f },
                new HousingData { Size = 2500f, Bedrooms = 4, Price = 420000f },
                new HousingData { Size = 3000f, Bedrooms = 5, Price = 500000f },
                new HousingData { Size = 1200f, Bedrooms = 2, Price = 230000f },
                new HousingData { Size = 1800f, Bedrooms = 3, Price = 320000f },
                new HousingData { Size = 2200f, Bedrooms = 4, Price = 380000f }
            };

            var trainingData = mlContext.Data.LoadFromEnumerable(housingData);

            var pipeline = mlContext.Transforms.Concatenate("Features", nameof(HousingData.Size), nameof(HousingData.Bedrooms))
                .Append(mlContext.Regression.Trainers.Sdca(
                    labelColumnName: nameof(HousingData.Price),
                    featureColumnName: "Features"));

            var model = pipeline.Fit(trainingData);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<HousingData, PricePrediction>(model);

            var testHouses = new[]
            {
                new HousingData { Size = 1600f, Bedrooms = 3 },
                new HousingData { Size = 2800f, Bedrooms = 5 },
                new HousingData { Size = 1100f, Bedrooms = 2 }
            };

            foreach (var house in testHouses)
            {
                var prediction = predictionEngine.Predict(house);
                Console.WriteLine($"House: {house.Size} sq ft, {house.Bedrooms} bedrooms");
                Console.WriteLine($"  → Predicted Price: ${prediction.Price:N2}");
            }
        }

        public class SentimentData
        {
            public string Text { get; set; }
            public bool Sentiment { get; set; }
        }

        public class SentimentPrediction
        {
            [ColumnName("PredictedLabel")]
            public bool Prediction { get; set; }
            public float Probability { get; set; }
        }

        public class HousingData
        {
            public float Size { get; set; }
            public float Bedrooms { get; set; }
            public float Price { get; set; }
        }

        public class PricePrediction
        {
            [ColumnName("Score")]
            public float Price { get; set; }
        }
    }
}
