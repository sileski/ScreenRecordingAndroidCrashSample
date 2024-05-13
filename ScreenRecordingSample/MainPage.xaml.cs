﻿using Plugin.Maui.ScreenRecording;

namespace ScreenRecordingSample;

public partial class MainPage : ContentPage
{
	readonly IScreenRecording screenRecording;

	public MainPage(IScreenRecording screenRecording)
	{
		InitializeComponent();

#if IOS
		CheckIOSSimulator();
#endif

		this.screenRecording = screenRecording;

		btnStart.IsEnabled = true;
		btnStop.IsEnabled = false;
	}

	void CheckIOSSimulator()
	{
		if(DeviceInfo.DeviceType == DeviceType.Virtual)
		{
			DisplayAlert("iOS Simulator", "Screen recording is not supported on the iOS Device Simulator", "OK");
		}
	}

	async void StartRecordingClicked(object sender, EventArgs e)
	{
		if (screenRecording.IsRecording)
			return;

		if (!screenRecording.IsSupported)
		{
			await DisplayAlert("Not Supported", "Screen recording is not supported", "OK");
			return;
		}

		btnStart.IsEnabled = false;
		btnStop.IsEnabled = true;
		if (setCustomNotification.IsToggled)
		{
			screenRecording.StartRecording(new()
			{
				EnableMicrophone = recordMicrophone.IsToggled,
				SaveToGallery = saveToGallery.IsToggled,
				NotificationContentTitle = ContentTitle.Text,
				NotificationContentText = ContentText.Text
			});
		}
		else
		{
			screenRecording.StartRecording(new()
			{
				EnableMicrophone = recordMicrophone.IsToggled,
				SaveToGallery = saveToGallery.IsToggled
			});
		}
	}

	async void StopRecordingClicked(object sender, EventArgs e)
	{
		ScreenRecordingFile screenResult = await screenRecording.StopRecording();

		if (screenResult != null)
		{
			FileInfo f = new(screenResult.FullPath);
			await Shell.Current.DisplayAlert("File Created", $"Path: {screenResult.FullPath} Size: {f.Length.ToString("N0")} bytes", "OK");

			mediaElement.Source = screenResult.FullPath;
		}
		else
		{
			await Shell.Current.DisplayAlert("No Screen Recording", "NADA", "OK");
		}

		btnStart.IsEnabled = true;
		btnStop.IsEnabled = false;
	}

	void OnToggled(object sender, ToggledEventArgs e)
	{
		ContentTitle.IsVisible = e.Value;
		ContentText.IsVisible = e.Value;
	}
}
