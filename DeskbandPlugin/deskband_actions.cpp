#include "stdafx.h"
#include "deskband_actions.h"

namespace deskband_actions
{
	static double last_length = 0.0;
	static float last_volume = 0.0;
	static bool last_stop_after_current_state = false;
	static void* last_album_art_buffer = NULL;
	static t_size last_album_art_buffer_size = 0;
	static bool last_album_art_stub = false;

	void send_command(int cmd, PVOID data, size_t size)
	{
		HWND dw = FindWindow(NULL, TEXT(DESKBAND_MSGWINDOW_TITLE));
		if (dw != NULL)
		{
			COPYDATASTRUCT cds;
			cds.dwData = cmd;
			cds.cbData = size;
			cds.lpData = data;
			SendMessage(dw, WM_COPYDATA, (WPARAM)core_api::get_main_window(), (LPARAM)&cds);
		}
	}

	void show()
	{
		send_command(DESKBAND_CMD_Show, NULL, 0);
	}

	void hide()
	{
		send_command(DESKBAND_CMD_Hide, NULL, 0);
	}

	void send_track_length(double length)
	{
		last_length = length;
		send_command(DESKBAND_CMD_TrackLength, &length, sizeof(length));
	}

	void send_track_time(double time)
	{
		send_command(DESKBAND_CMD_TrackTime, &time, sizeof(time));
	}

	void send_track_text(int index, pfc::string8 text)
	{
		const char *textPtr = text.get_ptr();
		size_t  textLen = strlen(textPtr);
		size_t size = sizeof(int) + sizeof(size_t) + textLen + 1;

		char *data = (char *)malloc(size);
		memcpy(data, &index, sizeof(int));
		memcpy(data + sizeof(int), &textLen, sizeof(size_t));
		memcpy(data + sizeof(int) + sizeof(size_t), textPtr, textLen);

		send_command(DESKBAND_CMD_Text, data, size);

		free(data);
	}

	void send_file_path(int index, pfc::string8 path)
	{
		const char *textPtr = path.get_ptr();
		size_t  textLen = strlen(textPtr);
		size_t size = sizeof(int) + sizeof(size_t) + textLen + 1;

		char *data = (char *)malloc(size);
		memcpy(data, &index, sizeof(int));
		memcpy(data + sizeof(int), &textLen, sizeof(size_t));
		memcpy(data + sizeof(int) + sizeof(size_t), textPtr, textLen);

		send_command(DESKBAND_CMD_FilePath, data, size);

		free(data);
	}

	void send_pause_state(bool state)
	{
		int data = state ? 1 : 0;
		send_command(DESKBAND_CMD_PauseState, &data, sizeof(data));
	}

	void send_stop()
	{
		send_command(DESKBAND_CMD_Stop, NULL, 0);
	}

	void send_track_volume(float volume)
	{
		last_volume = volume;
		send_command(DESKBAND_CMD_VolumeLevel, &volume, sizeof(volume));
	}

	void send_stop_after_current(bool state)
	{
		last_stop_after_current_state = state;

		int data = state ? 1 : 0;
		send_command(DESKBAND_CMD_StopAfterCurrentState, &data, sizeof(data));
	}

	void send_album_art(const void *art, t_size len, bool stub)
	{
		if (last_album_art_buffer != NULL && (art == NULL || art != NULL && art != last_album_art_buffer))
		{
			free(last_album_art_buffer);
			last_album_art_buffer = NULL;
		}

		if (art != NULL && len > 0)
		{
			if (last_album_art_buffer == NULL)
			{
				last_album_art_buffer = malloc(len);
				memcpy(last_album_art_buffer, art, len);
				last_album_art_buffer_size = len;
				last_album_art_stub = stub;
			}

			size_t size = sizeof(len) + len + sizeof(stub);
			char *data = (char *)malloc(size);
			memcpy(data, &len, sizeof(len));
			memcpy(data + sizeof(len), art, len);
			memcpy(data + sizeof(len) + len, &stub, sizeof(stub));

			send_command(DESKBAND_CMD_AlbumArt, data, size);

			free(data);
		}
	}

	void resend_last_state()
	{
		send_track_length(last_length);
		send_track_volume(last_volume);
		send_stop_after_current(last_stop_after_current_state);
		send_album_art(last_album_art_buffer, last_album_art_buffer_size, last_album_art_stub);
	}

	void resend_last_nontrack_state()
	{
		send_track_volume(last_volume);
		send_stop_after_current(last_stop_after_current_state);
		send_album_art(last_album_art_buffer, last_album_art_buffer_size, last_album_art_stub);
	}

	void send_version()
	{
		const char *textPtr = DESKBAND_CONTROLS_VERSION;
		size_t  textLen = strlen(textPtr);
		size_t size = sizeof(size_t) + textLen + 1;

		char *data = (char *)malloc(size);
		memcpy(data, &textLen, sizeof(size_t));
		memcpy(data + sizeof(size_t), textPtr, textLen);

		send_command(DESKBAND_CMD_Version, data, size);

		free(data);
	}
};