using System.Collections.Generic;
using Pulumi;
using Pulumi.Aws.S3;

class MyStack : Stack
{
    public MyStack()
    {
        // Create an AWS resource (S3 Bucket)
        var buckets = new List<Output<BucketData>>();
        for (var i = 0; i < 3; i++)
        {
            var name = $"my-bucket-{i}";
            var bucket = new Bucket(name,
                new BucketArgs
                {
                    LifecycleRules = new InputList<Pulumi.Aws.S3.Inputs.BucketLifecycleRuleArgs>
                    {
                        new Pulumi.Aws.S3.Inputs.BucketLifecycleRuleArgs
                        {
                            Enabled = true,
                            Expiration = new Pulumi.Aws.S3.Inputs.BucketLifecycleRuleExpirationArgs
                            {
                                Days = 10
                            },
                            Id = "log",
                            Prefix = "log/"
                        },
                        new Pulumi.Aws.S3.Inputs.BucketLifecycleRuleArgs
                        {
                            Enabled = true,
                            Expiration = new Pulumi.Aws.S3.Inputs.BucketLifecycleRuleExpirationArgs
                            {
                                Date = "2021-07-01"
                            },
                            Id = "tmp",
                            Prefix = "tmp/"
                        }
                    }
                });

            var data = Output.Tuple(bucket.Id, bucket.BucketName, bucket.LifecycleRules).Apply(t =>
            {
                var (id, name, rules) = t;

                var bucketData = new BucketData
                {
                    Id = id,
                    Name = name,
                    LifecycleData = new List<LifecycleData>()
                };

                foreach (var rule in rules)
                {
                    bucketData.LifecycleData.Add(new LifecycleData
                    {
                        Id = rule.Id,
                        Prefix = rule.Prefix,
                        Enabled = rule.Enabled
                    });
                }

                return bucketData;
            });
            
            buckets.Add(data);
        }

        BucketDataRaw = Output.All(buckets).Apply(x => Newtonsoft.Json.JsonConvert.SerializeObject(x));
    }

    [Output]
    public Output<string> BucketDataRaw { get; set; }
}

public class BucketData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<LifecycleData> LifecycleData { get; set; }
}

public class LifecycleData
{
    public string Id { get; set; }
    public string Prefix { get; set; }
    public bool Enabled { get; set; }
}
