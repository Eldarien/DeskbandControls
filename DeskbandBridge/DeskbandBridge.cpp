#include "stdafx.h"
#include "DeskbandBridge.h"

using namespace System;

namespace DeskbandBridge {
	public ref class FB2KConstants {
	public:
		literal const String ^ DeskbandControlsVersion = DESKBAND_CONTROLS_VERSION;
		literal const String ^ DeskbandControlsTitle = DESKBAND_CONTROLS_TITLE;

		literal const String ^ FoobarPluginMsgWindowClass = FOOBAR_PLUGIN_MSGWINDOW_CLASS;
		literal const String ^ FoobarPluginMsgWindowTitle = FOOBAR_PLUGIN_MSGWINDOW_TITLE;

		literal const String ^ DeskbandGuid = DESKBAND_GUID;
		literal const String ^ DeskbandMsgWindowTitle = DESKBAND_MSGWINDOW_TITLE;
	};

	public ref class FB2KCommands {
	public:
		literal int ResendLastState = FOOBAR_PLUGIN_CMD_ResendLastState;
		literal int ResendLastNonTrackState = FOOBAR_PLUGIN_CMD_ResendLastNonTrackState;

		literal int PlayPause = FOOBAR_PLUGIN_CMD_PlayPause;
		literal int Stop = FOOBAR_PLUGIN_CMD_Stop;
		literal int Previous = FOOBAR_PLUGIN_CMD_Previous;
		literal int Next = FOOBAR_PLUGIN_CMD_Next;
		literal int ToggleSAC = FOOBAR_PLUGIN_CMD_ToggleSAC;
		literal int Random = FOOBAR_PLUGIN_CMD_Random;

		literal int FormatString = FOOBAR_PLUGIN_CMD_FormatString;
		literal int Seek = FOOBAR_PLUGIN_CMD_Seek;
		literal int Volume = FOOBAR_PLUGIN_CMD_Volume;
		literal int Activate = FOOBAR_PLUGIN_CMD_Activate;
		literal int GetVersion = FOOBAR_PLUGIN_CMD_GetVersion;
		literal int SetPlaylistFormat = FOOBAR_PLUGIN_CMD_SetPlaylistFormat;
		literal int StartPlaylistIndex = FOOBAR_PLUGIN_CMD_StartPlaylistIndex;
		literal int RequestVisualizationData = FOOBAR_PLUGIN_CMD_RequestVisualizationData;
	};

	public ref class DeskbandCommands {
	public:
		literal int Show = DESKBAND_CMD_Show;
		literal int Hide = DESKBAND_CMD_Hide;
		literal int Text = DESKBAND_CMD_Text;
		literal int TrackLength = DESKBAND_CMD_TrackLength;
		literal int TrackTime = DESKBAND_CMD_TrackTime;
		literal int PauseState = DESKBAND_CMD_PauseState;
		literal int Stop = DESKBAND_CMD_Stop;
		literal int VolumeLevel = DESKBAND_CMD_VolumeLevel;
		literal int StopAfterCurrentState = DESKBAND_CMD_StopAfterCurrentState;
		literal int AlbumArt = DESKBAND_CMD_AlbumArt;
		literal int Version = DESKBAND_CMD_Version;
		literal int Playlist = DESKBAND_CMD_Playlist;
		literal int VisualizationData = DESKBAND_CMD_VisualizationData;
	};
}