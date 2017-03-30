using System;
using Memoria.Core;
using UnityEngine;

namespace Memoria
{
	public class LoadDemoImages : LoadImageBehaviour
	{
		[Header("Custom Information")]

		[SerializeField]
		private FileType _fileType = FileType.Jpg;

		public override string FormattedIndex()
		{
			return (loadImagesController.ImagesLoaded + 1).ToString(string.Empty);
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

		public override void EndAction()
		{
			dioManager.csvCreator.AddLines("Demo", string.Empty);
		}
	}
}