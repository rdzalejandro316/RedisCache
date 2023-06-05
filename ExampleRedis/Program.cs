using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ExampleRedis
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //string connectionString = "ExampleRedisCache.redis.cache.windows.net:6380,password=bh0ED7LToW1k8KomZIzuUhd2KcE17sH2HAzCaDubtZc=,ssl=True,abortConnect=False";

                var conn = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsetings.json")
                .Build();

                string connectionString = conn["connectionString"].ToString();

                var redisConnection = ConnectionMultiplexer.Connect(connectionString);
                IDatabase db = redisConnection.GetDatabase();

                //create key-value
                // bool wasSet = db.StringSet("favorite:flavor", "i-love-rocky-road");
                // Console.Write("CREADO:" + wasSet);

                //get key value
                // string value = db.StringGet("favorite:flavor");
                // Console.WriteLine(value);

                //execute ping
                // var result = db.Execute("ping");
                // Console.WriteLine(result.ToString()); // displays: "PONG"


                // get client connect
                // var result = db.Execute("client", "list");
                // Console.WriteLine($"Type = {result.Type}\r\nResult = {result}");

                // save complex values
                var stat = new GameStat("Soccer", new DateTime(2019, 7, 16), "Local Game", 
                new[] { "Team 1", "Team 2" },
                new[] { ("Team 1", 2), ("Team 2", 1) });

                string serializedValue = Newtonsoft.Json.JsonConvert.SerializeObject(stat);
                bool added = db.StringSet("event:1950-world-cup", serializedValue);


                string value = db.StringGet("event:1950-world-cup");
                Console.WriteLine(value);

                //close connection
                redisConnection.Dispose();
                redisConnection = null; 

            }
            catch (Exception w)
            {
                Console.WriteLine(w);
            }
        }

        public class GameStat
        {
            public string Id { get; set; }
            public string Sport { get; set; }
            public DateTimeOffset DatePlayed { get; set; }
            public string Game { get; set; }
            public IReadOnlyList<string> Teams { get; set; }
            public IReadOnlyList<(string team, int score)> Results { get; set; }

            public GameStat(string sport, DateTimeOffset datePlayed, string game, string[] teams, IEnumerable<(string team, int score)> results)
            {
                Id = Guid.NewGuid().ToString();
                Sport = sport;
                DatePlayed = datePlayed;
                Game = game;
                Teams = teams.ToList();
                Results = results.ToList();
            }

            public override string ToString()
            {
                return $"{Sport} {Game} played on {DatePlayed.Date.ToShortDateString()} - " +
                       $"{String.Join(',', Teams)}\r\n\t" +
                       $"{String.Join('\t', Results.Select(r => $"{r.team} - {r.score}\r\n"))}";
            }
        }
    }
}
