using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Ringo.Common.Models;

namespace Ringo.Common.Services
{
    public class PredictArtist
    {
        public static async Task<Artist> GetPrediction()
        {
            var pipeline = new LearningPipeline();

            //pipeline.Add(new TextLoader<SentimentData>(dataPath, separator: ","));

            //pipeline.Add(new TextFeaturizer("Features", "SentimentText"));

            //pipeline.Add(new FastTreeBinaryClassifier());

            //pipeline.Add(new PredictedLabelColumnOriginalValueConverter(PredictedLabelColumn = "PredictedLabel"));

            //var model = pipeline.Train<SentimentData, SentimentPrediction>();

            Artist artist = new Artist();
            return artist;
        }

        public static async Task TrainModel()
        {
            var pipeline = new LearningPipeline();
            var data = new List<Artist> {
               new Artist { name = "radiohead" },
               new Artist { name = "radiohead" }
            };
            var collection = CollectionDataSource.Create(data);
            pipeline.Add(collection);


        }


    }
}
