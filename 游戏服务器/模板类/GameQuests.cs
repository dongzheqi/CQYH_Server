using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace 游戏服务器.模板类
{
	public class GameQuests
	{
		public static IDictionary<int, GameQuests> 数据表;

		public static GameQuests[] AvailableQuests;

		public int Id;

		public int Chapter;

		public int Stage;

		public string Name;

		public int Level;

		public QuestType Type;

		public QuestResetType Reset;

		public QuestRelationLimit RelationLimit;

		public int StartNPCMap;

		public int StartNPCID;

		public int FinishNPCID;

		public int AutoStartNextID;

		public int MaxCompleteCount;

		public int ResetTime;

		public bool CanAbandon;

		public bool CanShare;

		public bool CanPublish;

		public bool CanTeleport;

		public int TeleportCostId;

		public int TeleportCostValue;

		public int UrgentTaskX;

		public int UrgentTaskY;

		public int CheckVersion;

		public List<GameQuestReward> Rewards = new List<GameQuestReward>();

		public List<GameQuestReward> SelectableRewards = new List<GameQuestReward>();

		public List<GameQuestMission> Missions = new List<GameQuestMission>();

		public List<GameQuestConstraint> Constraints = new List<GameQuestConstraint>();

		public static void 载入数据()
		{
			GameQuests.数据表 = new Dictionary<int, GameQuests>();
			List<GameQuests> list;
			list = new List<GameQuests>();
			string text;
			text = Settings.游戏数据目录 + "\\System\\Quests\\";
			if (Directory.Exists(text))
			{
				GameQuests[] array;
				array = 序列化类.反序列化<GameQuests>(text);
				foreach (GameQuests gameQuests in array)
				{
					for (int j = 0; j < gameQuests.Missions.Count; j++)
					{
						gameQuests.Missions[j].QuestId = gameQuests.Id;
						gameQuests.Missions[j].MissionIndex = j;
					}
					list.Add(gameQuests);
					GameQuests.数据表.Add(gameQuests.Id, gameQuests);
				}
			}
			GameQuests.AvailableQuests = list.OrderBy((GameQuests x) => x.Id).ToArray();
			string[] array2;
			array2 = File.ReadAllLines(Settings.游戏数据目录 + "\\System\\任务成就\\紧急任务.txt");
			new StringBuilder();
			string[] array3;
			array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				string[] array4;
				array4 = array3[i].Split(",");
				int startNPCMap;
				startNPCMap = Convert.ToInt32(array4[0]);
				int urgentTaskX;
				urgentTaskX = Convert.ToInt32(array4[1]);
				int urgentTaskY;
				urgentTaskY = Convert.ToInt32(array4[2]);
				int key;
				key = Convert.ToInt32(array4[3]);
				if (GameQuests.数据表.TryGetValue(key, out var value))
				{
					value.StartNPCMap = startNPCMap;
					value.UrgentTaskX = urgentTaskX;
					value.UrgentTaskY = urgentTaskY;
					value.StartNPCID = 0;
				}
			}
		}
	}
}
