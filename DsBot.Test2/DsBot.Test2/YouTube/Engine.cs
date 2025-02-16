using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System;

namespace DsBot.Test2.YouTube
{
    public class Engine
    {
        private string _apiKey = "AIzaSyDNm_lkD9d-GKpaPJNlcaYuJ9mwSoSOdeg";
        private string _channelID = "UCFdqrWOD23vJhVSK0_N8Cig";


        public youtubeVideo GetLatestVideo()
        {
            string VideoID;
            string VideoTitle;
            string VideoURL;
            string VideoThumbnail;
            DateTime? PublishedAt;

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = _apiKey,
                ApplicationName = "MyDiscordBot" 
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.ChannelId = _channelID;
            searchListRequest.MaxResults = 1;
            searchListRequest.Order = SearchResource.ListRequest.OrderEnum.Date;

            var searchListResponce = searchListRequest.Execute();

            foreach (var searchResult in searchListResponce.Items)
            {
                 if (searchResult.Id.Kind == "youtube#video")
                 {
                    VideoID = searchResult.Id.VideoId;
                    VideoTitle = searchResult.Snippet.Title;
                    VideoURL = $"https://www.youtube.com/watch?v={VideoID}";
                    PublishedAt = searchResult.Snippet.PublishedAt;
                    VideoThumbnail = searchResult.Snippet.Thumbnails.Default__.Url;

                    return new youtubeVideo()
                    {
                        VideoID = VideoID,
                        VideoTitle = VideoTitle,
                        VideoURL = VideoURL,
                        PublishedAt = PublishedAt,
                        VideoThumbnail = VideoThumbnail
                    };
                 }
            }

            return null;
        }
    }
    
}
