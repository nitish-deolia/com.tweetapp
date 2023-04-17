using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.Core.WireProtocol.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace com.tweetapp.Models.Entities
{
    public class Tweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string TweetMessage { get; set; }
        public string Author_Id { get; set; }
        public DateTime Created_At { get; set; }
        public List<string> Tags { get; set; }
        public int Likes { get; set; }
        public List<Reply> ReplyList { get; set; }
        public List<TweetUpdate> UpdateHistory { get; set; }
        public DateTime UpdateUntil { get; set; }
        public bool IsActive { get; set; }
    }

    public class Reply
    {
        public ObjectId Id { get; set; }
        public string ReplyMessage { get; set; }
        public string Author_Id { get; set;}
        public int Likes { get; set; }
        public DateTime Created_At { get; set;}
        public List<string> Tags { get; set; }
    }

    public class TweetUpdate
    {
        public ObjectId Id { get; set; }
        public string Message { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
