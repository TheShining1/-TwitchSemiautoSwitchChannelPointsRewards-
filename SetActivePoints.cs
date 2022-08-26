using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

public class CPHInline
{
	public bool Execute()
	{
		List<string> requiredGroups = new List<string>{"Soundalerts"};
		if (args.ContainsKey("targetGroupName")) requiredGroups.Add(args["targetGroupName"].ToString());

		string filePath = @"data\settings.json";
		string jsonStr = File.ReadAllText(filePath);

		JObject settingsJson = JObject.Parse(jsonStr);
		JObject channelPointsJson = settingsJson.Value<JObject>("channelPoints");
		JArray rewardsArray = channelPointsJson.Value<JArray>("rewards");

		List<string> rewardList = new List<string>();

		foreach (JObject reward in rewardsArray)
		{
			string rewardID = reward["id"].ToString();
			string groupName = reward["group"].ToString();
			
			if (requiredGroups.Exists(el => el.ToLower() == groupName.ToLower()))
			{
				CPH.LogInfo(groupName);
				rewardList.Add(rewardID);
			}
		}

		Parallel.ForEach(rewardList, reward =>
		{
			CPH.EnableReward(reward);
		});

		return true;
	}
}
