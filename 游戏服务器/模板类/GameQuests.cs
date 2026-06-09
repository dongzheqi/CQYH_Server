using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using CsvHelper;

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
			DataTable dataTable;
			dataTable = new DataTable();
			using (StreamReader reader = 配置读取.打开(Settings.游戏数据目录 + "\\System\\任务成就\\紧急任务.csv"))
			{
				using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
				using CsvDataReader reader2 = new CsvDataReader(csv);
				dataTable.Load(reader2);
			}
			foreach (DataRow item in dataTable.Rows.Cast<DataRow>())
			{
				int startNPCMap;
				startNPCMap = Convert.ToInt32(item["StartNPCMap"].ToString());
				int urgentTaskX;
				urgentTaskX = Convert.ToInt32(item["UrgentTaskX"].ToString());
				int urgentTaskY;
				urgentTaskY = Convert.ToInt32(item["UrgentTaskY"].ToString());
				int key;
				key = Convert.ToInt32(item["QuestID"].ToString());
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
