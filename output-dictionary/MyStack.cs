using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;
using Pulumi.Aws.S3;

class MyStack : Stack
{
    public MyStack()
    {
        // Below we will construct a simple example using a for loop, that will result in a Dictionary<t,k> being outputted for use outside this stack. This Dictionary will contain the bucket name as the key and bucket ids of each bucket, as the value.
        var outputKeyValues = new List<Output<KeyValuePair<string, string>>>();
        
        // 3 is an arbitrary number which can be substituted for your use case.
        for (var i = 0; i < 3; i++)
        {
            var bucketName = $"some-bucket-{i}";
            var bucket = new Bucket(bucketName, new BucketArgs
            {
                // configure as necessary
                BucketName = $"{bucketName}-named-bucket"
            });

            // Make use of apply to retrieve the concrete values from the Bucket, once they are available.
            var data = Output.Tuple(bucket.BucketName, bucket.Id).Apply(t =>
            {
                var (name, id) = t;
                return new KeyValuePair<string, string>(name, id);
            });
            
            outputKeyValues.Add(data);
        }

        // The All() does not currently support Dictionary types as an input; only Enumerable types. Thus, we are using a List<KeyValuePairs<T,K>> to track our outputs
        BucketData = Output.All(outputKeyValues).Apply(x =>
        {
            return x.ToImmutableDictionary(kv => kv.Key, kv => kv.Value);
        });
    }

    [Output] 
    public Output<ImmutableDictionary<string, string>> BucketData { get; set; }
}
