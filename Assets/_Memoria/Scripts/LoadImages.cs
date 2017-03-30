using System;
using System.Collections.Generic;
using Gamelogic;
using Memoria.Core;
using UnityEngine;

namespace Memoria
{
	public enum Test
	{
		Test1,
		Test2,
		Test3,
		Test4
	}

	public class LoadImages : LoadImageBehaviour
	{
		[Header("Custom Information")]
		public string groupImageCsvPath;
		public Test test;
		
		[SerializeField]
		private FileType _fileType = FileType.Jpg;

		private List<string> _groupOneList;
		private List<string> _otherGroupList;

		private List<string> _relevantTestList;
		private List<string> _irrelevantTestList;

		private List<string> _organizedItemList;

		public void Awake()
		{
			groupImageCsvPath = GLPlayerPrefs.GetString("Config", "GroupPath");
			test = (Test)GLPlayerPrefs.GetInt("Config", "Test");
		}

		public override void Initialize(DIOManager fatherDioManager, LoadImagesController loadImagesController)
		{
			base.Initialize(fatherDioManager, loadImagesController);

			GenerateImageGroupDictionary();

			GenerateTestGroups();

			GenerateOrganizedItemList();
		}

		public override string FormattedIndex()
		{
			return _organizedItemList[loadImagesController.ImagesLoaded];
		}

		public override bool ValidationOfIndex(string index)
		{
			return true;
		}

		public override Tuple<string, TextureFormat> TextureFormatGetter()
		{
			switch (_fileType)
			{
				case FileType.Jpg:
					return new Tuple<string, TextureFormat>(@".jpg", TextureFormat.DXT1);
				case FileType.Png:
					return new Tuple<string, TextureFormat>(@".png", TextureFormat.DXT5);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void GenerateImageGroupDictionary()
		{
			_groupOneList = new List<string>();
			_otherGroupList = new List<string>();

			string line;
			var file = new System.IO.StreamReader(groupImageCsvPath);
			while ((line = file.ReadLine()) != null)
			{
				var lineSeparated = line.Split(',');

				var g1 = lineSeparated[1];
				var g2 = lineSeparated[2];
				var g3 = lineSeparated[3];
				var g4 = lineSeparated[4];

				if (g1.Equals("x") && !g2.Equals("x") && !g3.Equals("x") && !g4.Equals("x"))
					_groupOneList.Add(lineSeparated[0]);
				else if (!g1.Equals("x") && g2.Equals("x") && !g3.Equals("x") && !g4.Equals("x"))
					_otherGroupList.Add(lineSeparated[0]);
				else if (!g1.Equals("x") && !g2.Equals("x") && g3.Equals("x") && !g4.Equals("x"))
					_otherGroupList.Add(lineSeparated[0]);
				else if (!g1.Equals("x") && !g2.Equals("x") && !g3.Equals("x") && g4.Equals("x"))
					_otherGroupList.Add(lineSeparated[0]);
				else if (!g1.Equals("x") && !g2.Equals("x") && !g3.Equals("x") && !g4.Equals("x"))
					_otherGroupList.Add(lineSeparated[0]);
			}

			print("Group One Images: " + _groupOneList.Count);
			print("Irrelevant Images: " + _otherGroupList.Count);

			file.Close();
		}

		private void GenerateTestGroups()
		{
			_relevantTestList = new List<string>();
			_irrelevantTestList = new List<string>();

			var initialRelevantValue = 0;
			var initialIrrelevantValue = 0;

			switch (test)
			{
				case Test.Test1:
					initialRelevantValue = 0;
					initialIrrelevantValue = 0;
					break;
				case Test.Test2:
					initialRelevantValue = 30;
					initialIrrelevantValue = 130;
					break;
				case Test.Test3:
					initialRelevantValue = 60;
					initialIrrelevantValue = 260;
					break;
				case Test.Test4:
					initialRelevantValue = 90;
					initialIrrelevantValue = 390;
					break;
			}

			var lastRelevantValue = initialRelevantValue + 30;
			for (int i = initialRelevantValue; i < lastRelevantValue; i++)
			{
				_relevantTestList.Add(_groupOneList[i]);
			}

			var lastIrrelevantValue = initialIrrelevantValue + 130;
			for (int i = initialIrrelevantValue; i < lastIrrelevantValue; i++)
			{
				_irrelevantTestList.Add(_otherGroupList[i]);
			}
		}

		private void GenerateOrganizedItemList()
		{
			_organizedItemList = new List<string>();
			var relevantIndex = 0;
			var relevantIndexLimit = 7;
			var irrelevantIndex = 0;
			var irrelevantIndexLimit = 32;

			for (var i = 0; i < 4; i++)
			{
				var circleItems = new List<string>();

				for (var j = relevantIndex; j < relevantIndexLimit; j++)
				{
					circleItems.Add(_relevantTestList[j]);
				}
				relevantIndex += 7;
				relevantIndexLimit += 7;

				for (var j = irrelevantIndex; j < irrelevantIndexLimit; j++)
				{
					circleItems.Add(_irrelevantTestList[j]);
				}
				irrelevantIndex += 32;
				irrelevantIndexLimit += 32;

				circleItems.Shuffle();

				_organizedItemList.AddRange(circleItems);
			}

			_organizedItemList.Add(_relevantTestList[relevantIndex]);
			_organizedItemList.Add(_irrelevantTestList[irrelevantIndex]);
			_organizedItemList.Add(_relevantTestList[relevantIndex + 1]);
			_organizedItemList.Add(_irrelevantTestList[irrelevantIndex + 1]);
		}

		public override void EndAction()
		{
			dioManager.csvCreator.AddLines(test.ToString(), string.Empty);
		}
	}
}