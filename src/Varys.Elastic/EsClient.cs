using Elasticsearch.Net;
using Elasticsearch.Net.ConnectionPool;
using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Varys.Elastic.Entities;


namespace Varys.Elastic
{
    public class EsClient
    {
        internal static bool IsTest { get; set; }

        internal static string DefaultIndex
        {
            get { return ConfigurationManager.AppSettings["esindex"]; }
        }

        internal static string Nodes
        {
            get { return ConfigurationManager.AppSettings["esurl"]; }
        }

        internal static int Replicas
        {
            get { return Int32.Parse(ConfigurationManager.AppSettings["esreplicas"]); }
        }

        internal static int Shards
        {
            get { return Int32.Parse(ConfigurationManager.AppSettings["esshards"]); }
        }
        private static ElasticClient _client = null;
        internal static ElasticClient Client
        {
            get
            {
                List<Uri> nodes = new List<Uri>();
                var nodesString = Nodes;
                var nodesStringSplit = nodesString.Split(',');
                foreach (var node in nodesStringSplit)
                {
                    var esConn = new Uri(node);
                    nodes.Add(esConn);
                }
                var connectionPool = new SniffingConnectionPool(nodes);

                var settings = new ConnectionSettings(connectionPool, DefaultIndex)
                    .ExposeRawResponse()
                    .EnableMetrics()
                    .SniffLifeSpan(TimeSpan.FromMinutes(1))
                    //.SetTimeout(1000)
                    .SetPingTimeout(1000)
                    .MaximumRetries(5);


                _client = new ElasticClient(settings);
                if (!_client.IndexExists(x => x.Index(DefaultIndex)).Exists)
                    CreateIndex();
                return _client;
            }
            private set { _client = value; }
        }

        public static void ReIndex()
        {
            Client.DeleteIndex(c => c.Index(DefaultIndex));
            CreateIndex();
        }
        public static void CreateIndex()
        {
            var settings = new IndexSettings
            {
                NumberOfReplicas = Replicas,
                NumberOfShards = Shards
            };

            var analysis = new AnalysisSettings();
            analysis.Analyzers.Add("autocomplete", new CustomAnalyzer
            {
                Tokenizer = new WhitespaceTokenizer().Type,
                Filter = new[] { new LowercaseTokenFilter().Type, "engram" }
            });

            analysis.TokenFilters.Add("engram", new EdgeNGramTokenFilter
            {
                MinGram = 1,
                MaxGram = 20
            });

            settings.Analysis = analysis;

            _client.CreateIndex(c =>
                 c.Index(DefaultIndex)
                 .InitializeUsing(settings)
                 .AddMapping<EsAddress>(m => m.MapFromAttributes())
                 );
        }
    }
}
