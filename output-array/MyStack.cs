using System.Collections.Generic;
using System.Collections.Immutable;
using Pulumi;
using Pulumi.Aws.S3;

class MyStack : Stack
{
    public MyStack()
    {
        // Below we will construct a simple example using a for loop, that will result in an Array being outputted for use outside this stack. This array will contain the bucket ids of each bucket.

        // As we will not know the S3 bucket ids until 'pulumi up' has completed, we will reference Output<string> for now.
        var outputList = new List<Output<string>>();
        
        // 3 is an arbitrary number which can be substituted for your use case.
        for (var i = 0; i < 3; i++)
        {
            var name = $"some-bucket-{i}";
            var bucket = new Bucket(name, new BucketArgs
            {
                // configure as necessary
                BucketName = $"{name}-named-bucket"
            });
            
            outputList.Add(bucket.Id);
        }

        // The use of the "All" function is key. Pulumi will use All() to wait until all Output<string> elements in the array have concrete values.
        // The result will be an ImmutableArray with the physical bucket ids
        BucketNames = Output.All(outputList);
    }

    [Output]
    public Output<ImmutableArray<string>> BucketNames { get; set; }
}
