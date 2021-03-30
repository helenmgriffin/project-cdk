using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using System.Collections.Generic;
using System.IO;

namespace ProjectCdk.handler
{
    public class PutHandler
    {
        public bool Put(object o)
        {
            // Create a client
            AmazonDynamoDBClient client = new AmazonDynamoDBClient();

            // Define item attributes
            Dictionary<string, AttributeValue> attributes = new Dictionary<string, AttributeValue>();
            // Author is hash-key
            attributes["Author"] = new AttributeValue { S = "Mark Twain" };
            // Title is range-key
            attributes["Title"] = new AttributeValue { S = "The Adventures of Tom Sawyer" };
            // Other attributes
            attributes["Year"] = new AttributeValue { N = "1876" };
            attributes["Setting"] = new AttributeValue { S = "Missouri" };
            attributes["Pages"] = new AttributeValue { N = "275" };
            attributes["Genres"] = new AttributeValue
            {
                SS = new List<string> { "Satire", "Folk", "Children's Novel" }
            };

            // Create PutItem request
            PutItemRequest request = new PutItemRequest
            {
                TableName = "SampleTable",
                Item = attributes
            };

            // Issue PutItem request
            client.PutItemAsync(request);
            return true;
        }
    }
}
