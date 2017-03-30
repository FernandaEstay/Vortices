using System;
using System.Collections;
using System.Linq;
using Gamelogic;
using Memoria.Core;
using UnityCallbacks;
using UnityEngine;

namespace Memoria
{
	public enum FileType
	{
		Jpg,
		Png
	}

	public class LoadImagesController : GLMonoBehaviour, IOnValidate
	{
		public int images = 1;

		private LoadImageBehaviour _loadImageBehaviour;
		private DIOManager _dioManager;

		public LoadImageBehaviour LoadImageBehaviour { get { return _loadImageBehaviour; } }

		public int ImagesLoaded { get { return _imagesLoaded; } }
		private int _imagesLoaded;

		public void Initialize(DIOManager dioManager)
		{
			_dioManager = dioManager;

			_loadImageBehaviour = GetComponent<LoadImageBehaviour>();
			_loadImageBehaviour.Initialize(_dioManager, this);
		}

		public IEnumerator LoadFolderImages()
		{
			var listOfDio = _dioManager.sphereControllers           //sphereController es un List<SphereController>
				.SelectMany(sc => sc.dioControllerList).ToList();   //obtiene una lista de las esferas
			var tuple = _loadImageBehaviour.TextureFormatGetter(); //tuple < "jpg o Png", "TextureFormat.DXT1">

            string fileSuffix = tuple.First;
			TextureFormat textureFormat = tuple.Second;

			string indexSuffix;
			string fullFilename;
			WWW www;
			Texture2D texTmp;
			PitchGrabObject pitchGrabObject;

			while (true)
			{
				if (_imagesLoaded == images)
					break;

				indexSuffix = _loadImageBehaviour.FormattedIndex();

				if (!_loadImageBehaviour.ValidationOfIndex(indexSuffix))
					continue;

				fullFilename = _loadImageBehaviour.pathPrefix +
					_loadImageBehaviour.pathImageAssets +
					_loadImageBehaviour.pathSmall +
					_loadImageBehaviour.filename + indexSuffix + fileSuffix;

				www = new WWW(fullFilename);
				texTmp = new Texture2D(1024, 1024, textureFormat, false);
				www.LoadImageIntoTexture(texTmp);

				pitchGrabObject = listOfDio[_imagesLoaded].pitchGrabObject;
				pitchGrabObject.InitializeMaterial(texTmp);
				pitchGrabObject.SetId(_loadImageBehaviour.filename
					+ indexSuffix + fileSuffix);

				_imagesLoaded++;

				yield return new WaitForEndOfFrame();
			}

			_loadImageBehaviour.EndAction();
		}

		public void OnValidate()
		{
			images = Mathf.Max(1, images);
		}
	}

	public abstract class LoadImageBehaviour : GLMonoBehaviour
	{
		public string pathPrefix = @"file://";
		public string pathImageAssets = @"D:\";
		public string pathSmall = @"";
		public string filename = @"photo";

		protected DIOManager dioManager;
		protected LoadImagesController loadImagesController;

		public abstract string FormattedIndex();
		public abstract bool ValidationOfIndex(string index);
		public abstract Tuple<string, TextureFormat> TextureFormatGetter();
		public abstract void EndAction();

		public virtual void Initialize(DIOManager fatherDioManager, LoadImagesController loadImagesController)
		{
			dioManager = fatherDioManager;
			this.loadImagesController = loadImagesController;
		}
	}
}