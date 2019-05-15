using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Products.Database.Model
{
    public class Product
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Price { get; set; }
    }
}
