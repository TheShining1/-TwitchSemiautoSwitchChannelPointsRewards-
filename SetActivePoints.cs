using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class CPHInline
{
	string LogPrefix = "SetActivePoints::";

	public bool Execute()
	{
		List<string> requiredGroups = new List<string>();
		if (args.ContainsKey("genericGroupName") && args["genericGroupName"].ToString() != "")
		{
			CPH.LogDebug($"{LogPrefix}Adding generic groups: {args["genericGroupName"].ToString()}");
			requiredGroups.AddRange(args["genericGroupName"].ToString().Split(','));
		}

		if (args.ContainsKey("targetGroupName") && args.ContainsKey("targetGroupName").ToString() != "")
		{
			CPH.LogDebug($"{LogPrefix}Adding target group: {args["targetGroupName"].ToString()}");
			requiredGroups.Add(args["targetGroupName"].ToString());
		}

		if (requiredGroups.Count == 0)
		{
			CPH.LogDebug($"{LogPrefix}No groups to enable");
			return false;
		}

		CPH.LogDebug($"{LogPrefix}Getting rewards");
		string filePath = @"data\twitch_rewards.json";
		string jsonStr = File.ReadAllText(filePath);

		JObject channelPointsJson = JObject.Parse(jsonStr);
		JArray rewardsArray = channelPointsJson.Value<JArray>("rewards");

		List<string> rewardList = new List<string>();

		foreach (JObject reward in rewardsArray)
		{
			string rewardID = reward["id"].ToString();
			string groupName = reward["group"].ToString();
			
			if (requiredGroups.Exists(el => el.ToLower() == groupName.ToLower()))
			{
				CPH.LogDebug($"{LogPrefix}Adding reward: {reward["name"].ToString()}");
				rewardList.Add(rewardID);
			}
		}

		Parallel.ForEach(rewardList, reward =>
		{
			CPH.LogDebug($"{LogPrefix}Enabling reward: {reward}");
			CPH.EnableReward(reward);
		});

		return true;
	}
}
