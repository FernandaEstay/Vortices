using System;
using UnityEngine;
using Gamelogic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ConfigurationManager : MonoBehaviour
{
	public string sceneName;

	[Header("Hardware")]
	public Toggle useLeapMotionToggle;
	public Toggle usePitchGrabToggle;
	public Toggle useHapticGloveToggle;
	public Toggle useJoystickToggle;

	[Header("Data Output")]
	public InputField dataOutputText;

	[Header("Unity Open Glove")]
	public InputField leftComText;
	public InputField rightComText;

	[Header("User ID")]
	public InputField userIdText;

	[Header("Load Images")]
	public InputField imagesText;
	public InputField folderImageAssetText;
	public InputField folderSmallText;
	public InputField fileNameText; 
	public InputField groupPathText;
	public Dropdown testDropdown;

	private const string Scope = "Config";

	public void Awake()
	{
		useLeapMotionToggle.isOn = GLPlayerPrefs.GetBool(Scope, "UseLeapMotion");
		usePitchGrabToggle.isOn = GLPlayerPrefs.GetBool(Scope, "UsePitchGrab");
		useHapticGloveToggle.isOn = GLPlayerPrefs.GetBool(Scope, "UseHapticGlove");
		useJoystickToggle.isOn = GLPlayerPrefs.GetBool(Scope, "UseJoystic");

		dataOutputText.text = GLPlayerPrefs.GetString(Scope, "DataOutput");

		leftComText.text = GLPlayerPrefs.GetString(Scope, "LeftCom");
		rightComText.text = GLPlayerPrefs.GetString(Scope, "RightCom");

		userIdText.text = GLPlayerPrefs.GetInt(Scope, "PersonId").ToString();

		imagesText.text = GLPlayerPrefs.GetString(Scope, "Images");
		folderImageAssetText.text = GLPlayerPrefs.GetString(Scope, "FolderImageAssetText");
		folderSmallText.text = GLPlayerPrefs.GetString(Scope, "FolderSmallText");
		fileNameText.text = GLPlayerPrefs.GetString(Scope, "FileName");
		groupPathText.text = GLPlayerPrefs.GetString(Scope, "GroupPath");

		testDropdown.value = GLPlayerPrefs.GetInt(Scope, "Test");
	}

	public void StartSimulation()
	{
		GLPlayerPrefs.SetBool(Scope, "UseLeapMotion", useLeapMotionToggle.isOn);
		GLPlayerPrefs.SetBool(Scope, "UsePitchGrab", usePitchGrabToggle.isOn);
		GLPlayerPrefs.SetBool(Scope, "UseHapticGlove", useHapticGloveToggle.isOn);
		GLPlayerPrefs.SetBool(Scope, "UseJoystic", useJoystickToggle.isOn);

		GLPlayerPrefs.SetString(Scope, "DataOutput", dataOutputText.text);

		GLPlayerPrefs.SetString(Scope, "LeftCom", leftComText.text);
		GLPlayerPrefs.SetString(Scope, "RightCom", rightComText.text);

		GLPlayerPrefs.SetInt(Scope, "UserID", Convert.ToInt32(userIdText.text));

		GLPlayerPrefs.SetString(Scope, "Images", imagesText.text);
		GLPlayerPrefs.SetString(Scope, "FolderImageAssetText", folderImageAssetText.text);
		GLPlayerPrefs.SetString(Scope, "FolderSmallText", folderSmallText.text);
		GLPlayerPrefs.SetString(Scope, "FileName", fileNameText.text);
		GLPlayerPrefs.SetString(Scope, "GroupPath", groupPathText.text);

		GLPlayerPrefs.SetInt(Scope, "Test", testDropdown.value);

		SceneManager.LoadScene(sceneName);
	}
}