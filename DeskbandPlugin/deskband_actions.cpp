#include "stdafx.h"
#include "deskband_actions.h"

namespace deskband_actions
{
	static double last_length = 0.0;
	static float last_volume = 0.0;
	static float last_volume_step = 0.0;
	static bool last_stop_after_current_state = false;
	static void* last_album_art_buffer = NULL;
	static t_size last_album_art_buffer_size = 0;
	static bool last_album_art_stub = false;

	//static pfc::string_list_impl* last_playlist = NULL;
	//static size_t last_playlist_current_index = 0;
	static pfc::string8* playlist_format = NULL;
	static t_size last_playlist_current_index = 0;

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

	void send_track_text(GUID id, pfc::string8 text)
	{
		const char *textPtr = text.get_ptr();
		size_t textLen = strlen(textPtr);
		size_t size = sizeof(GUID) + sizeof(size_t) + textLen + 1;

		char *data = (char *)malloc(size);
		memcpy(data, &id, sizeof(GUID));
		memcpy(data + sizeof(GUID), &textLen, sizeof(size_t));
		memcpy(data + sizeof(GUID) + sizeof(size_t), textPtr, textLen);

		send_command(DESKBAND_CMD_Text, data, size);

		free(data);
	}

	void set_playlist_format(char* fmt, t_size len)
	{
		if (playlist_format != NULL)
		{
			delete playlist_format;
			playlist_format = NULL;
		}
		playlist_format = new pfc::string8(fmt, len);
	}

	void handle_playlist_change(t_size p_playlist)
	{
		static_api_ptr_t<playlist_manager> pm;
		t_size active_playlist = pm->get_active_playlist();
		if (p_playlist != 0 && p_playlist != active_playlist)
			return;

		if (playlist_format == NULL)
			return;

		service_ptr_t<titleformat_object> format;
		static_api_ptr_t<titleformat_compiler>()->compile(format, *playlist_format);

		bit_array_true t;
		metadb_handle_list meta_list;
		pm->playlist_get_items(active_playlist, meta_list, t);

		pfc::string_list_impl formatted_list;
		t_size meta_count = meta_list.get_count();
		for (t_size index = 0; index < meta_count; index++)
		{
			metadb_handle_ptr item = meta_list.get_item(index);
			pfc::string8 text;
			item->format_title(NULL, text, format, NULL);
			formatted_list.add_item(text);
		}

		static_api_ptr_t<playback_control> control;
		metadb_handle_ptr current_item;
		control->get_now_playing(current_item);
		t_size current_index = current_item != 0 ? meta_list.find_item(current_item) : 0;
		last_playlist_current_index = current_index;

		t_size count = formatted_list.get_count();
		t_size total_len = 0;
		for (t_size index = 0; index < count; index++)
		{
			auto item = formatted_list.get_item(index);
			total_len += strlen(item);
		}

		t_size size = sizeof(t_size) * 2 + sizeof(t_size) * count + total_len + 1; // current_index, count, [] of len:text
		char *data = (char *)malloc(size);
		char *p = data;

		memcpy(p, &current_index, sizeof(t_size)); // current_index
		p += sizeof(t_size);

		memcpy(p, &count, sizeof(t_size)); // count
		p += sizeof(t_size);

		for (t_size index = 0; index < count; index++)
		{
			auto item = formatted_list.get_item(index);
			t_size len = strlen(item);

			memcpy(p, &len, sizeof(t_size)); // len
			p += sizeof(t_size);

			memcpy(p, item, len); // string bytes
			p += len;
		}

		send_command(DESKBAND_CMD_Playlist, data, size);

		free(data);
	}

	t_size get_last_playlist_current_index()
	{
		return last_playlist_current_index;
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

	void send_track_volume(float volume, float step)
	{
		last_volume = volume;
		last_volume_step = step;
		float data[] = { volume, step };
		send_command(DESKBAND_CMD_VolumeLevel, &data, sizeof(data));
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
		send_track_volume(last_volume, last_volume_step);
		send_stop_after_current(last_stop_after_current_state);
		send_album_art(last_album_art_buffer, last_album_art_buffer_size, last_album_art_stub);

		//if (last_playlist != NULL) send_playlist(*last_playlist, last_playlist_current_index);
		handle_playlist_change(0);
	}

	void resend_last_nontrack_state()
	{
		send_track_volume(last_volume, last_volume_step);
		send_stop_after_current(last_stop_after_current_state);
		send_album_art(last_album_art_buffer, last_album_art_buffer_size, last_album_art_stub);

		handle_playlist_change(0);
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

	static service_ptr_t<visualisation_stream_v3> vis_stream;

	void initialize_vis_stream()
	{
		//static_api_ptr_t<visualisation_manager> vis_mgr;
		visualisation_manager::get()->create_stream(vis_stream, visualisation_manager::KStreamFlagNewFFT);
	}

	void send_visualization_data()
	{
		double time;
		if (vis_stream->get_absolute_time(time))
		{
			audio_chunk_impl chunk;

			// read samples before and after the current track time
			double length = (double)12 * 1e-3;
			//if (time >= length)
			//{
				//time -= length / 2;
				//length *= 2;
			//}
			vis_stream->get_chunk_absolute(chunk, time, length);


			//vis_stream->get_chunk_absolute(chunk, time, 100 * 0.001); // 100ms

			t_uint32 channel_count = chunk.get_channel_count();
			t_uint32 sample_count = chunk.get_sample_count();
			const audio_sample* samples = chunk.get_data();

			//console::formatter() << "Vis. data requested:  channels: " << channel_count << ", samples:" << sample_count_total;

			//TODO: send samples to deskband

			// size =channel_count + sample_count + samples
			size_t size = sizeof(t_uint32) + sizeof(t_uint32) + sizeof(audio_sample) * sample_count;
			char* data = (char*)malloc(size);
			memcpy(data, &channel_count, sizeof(t_uint32));
			memcpy(data + sizeof(t_uint32), &sample_count, sizeof(t_uint32));
			memcpy(data + sizeof(t_uint32) + sizeof(t_uint32), samples, sizeof(audio_sample) * sample_count);

			send_command(DESKBAND_CMD_VisualizationData, data, size);

			free(data);
		}
	}
};