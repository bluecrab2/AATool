﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using AATool.Net;
using Newtonsoft.Json;

namespace AATool.Data.Progress
{
    [TypeConverter(typeof(Contribution))]
    [JsonObject]
    public class Contribution
    {
        [JsonProperty] public readonly Uuid PlayerId;
        [JsonProperty] public readonly Dictionary<string, DateTime> Advancements;
        [JsonProperty] public readonly HashSet<(string adv, string crit)> Criteria;
        [JsonProperty] public readonly Dictionary<string, int> ItemCounts;
        [JsonProperty] public readonly Dictionary<string, int> ItemsDropped;
        [JsonProperty] public readonly HashSet<string> BlocksPlaced;

        [JsonProperty] public bool HasGodApple;

        [JsonIgnore]
        public int CompletedCount => this.Advancements.Count;
        
        [JsonConstructor]
        public Contribution(Uuid playerId,
            Dictionary<string, DateTime> Advancements, 
            HashSet<(string, string)> Criteria,
            Dictionary<string, int> ItemCounts,
            Dictionary<string, int> ItemsDropped,
            HashSet<string> BlocksPlaced)
        {
            this.PlayerId      = playerId;
            this.Advancements  = Advancements;
            this.Criteria      = Criteria;
            this.ItemCounts    = ItemCounts;
            this.ItemsDropped    = ItemsDropped;
            this.BlocksPlaced  = BlocksPlaced;
        }

        public Contribution(Uuid playerId)
        {
            this.PlayerId     = playerId;
            this.Advancements = new ();
            this.Criteria     = new ();
            this.ItemCounts   = new ();
            this.ItemsDropped = new();
            this.BlocksPlaced = new ();
        }

        public static Contribution FromJsonString(string jsonString) =>
            JsonConvert.DeserializeObject<Contribution>(jsonString);

        public string ToJsonString() => 
            JsonConvert.SerializeObject(this);

        public bool IncludesAdvancement(string id) => 
            this.Advancements.ContainsKey(id);

        public bool IncludesBlock(string id) =>
            this.BlocksPlaced.Contains(id);

        public bool IncludesCriterion(string advancement, string criterion) => 
            this.Criteria.Contains((advancement, criterion));

        public int ItemCount(string item) => 
            this.ItemCounts.TryGetValue(item, out int val) ? val : 0;

        public void AddAdvancement(string advancement, DateTime whenFirstCompleted) =>
            this.Advancements.Add(advancement, whenFirstCompleted);

        public void AddCriterion(string advancement, string criterion) => 
            this.Criteria.Add((advancement, criterion));

        public void AddItemCount(string item, int count) =>
            this.ItemCounts[item] = count;

        public void AddDropCount(string item, int count) =>
            this.ItemsDropped[item] = count;

        public void AddBlock(string block) =>
            this.BlocksPlaced.Add(block);
    }
}
