﻿using System.Collections.Generic;
using System.Linq;
using CluedIn.Crawling.HubSpot.Core;
using CluedIn.Crawling.HubSpot.Infrastructure;
using CluedIn.Crawling.HubSpot.Infrastructure.Exceptions;

namespace CluedIn.Crawling.HubSpot.Iterators
{
    public class TemplatesIterator : HubSpotIteratorBase
    {
        public TemplatesIterator(IHubSpotClient client, HubSpotCrawlJobData jobData) : base(client, jobData)
        {
        }

        public override IEnumerable<object> Iterate(int? limit = null)
        {
            var offset = 0;
            var retries = 0;
            limit = limit ?? 100;

            var result = new List<object>();
            try
            {
                while (true)
                {
                    try
                    {

                        var response = Client.GetTemplatesAsync(limit.Value, offset).Result;

                        if (response?.objects == null || !response.objects.Any())
                            break;

                        result.AddRange(response.objects);

                        if (response.objects.Count < limit || response.offset == null)
                            break;

                        offset += limit.Value;
                        retries = 0;
                    }
                    catch (ThrottlingException e)
                    {
                        if (!ShouldRetryThrottledCall(e, retries))
                        {
                            break;
                        }

                        retries++;
                    }
                }
            }
            catch
            {
                return Enumerable.Empty<object>();
            }

            return result;
        }
    }
}