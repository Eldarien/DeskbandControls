#include "stdafx.h"
#include "msgwindow.h"
#include "deskband_actions.h"

namespace msgwindow
{
	static HWND hwnd;
	static wchar_t class_name[] = TEXT(FOOBAR_PLUGIN_MSGWINDOW_CLASS);
	static wchar_t window_title[] = TEXT(FOOBAR_PLUGIN_MSGWINDOW_TITLE);
	static service_ptr_t<visualisation_stream_v3> vis_stream;

	LRESULT CALLBACK WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam);

	void create()
	{
		static_api_ptr_t<visualisation_manager> vis_mgr;
		vis_mgr->create_stream(vis_stream, visualisation_manager::KStreamFlagNewFFT);

		HMODULE hInst = GetModuleHandle(NULL);

		WNDCLASSW wc = { 0 };
		wc.style = CS_HREDRAW | CS_VREDRAW;
		wc.hCursor = LoadCursor(NULL, IDC_ARROW);
		wc.hInstance = hInst;
		wc.lpfnWndProc = WndProc;
		wc.lpszClassName = class_name;
		wc.hbrBackground = NULL;
		RegisterClassW(&wc);

		hwnd = CreateWindowExW(0, class_name, window_title,
			WS_OVERLAPPEDWINDOW, 0, 0, 0, 0, HWND_MESSAGE, NULL, hInst, NULL);
	}

	void destroy()
	{
		DestroyWindow(hwnd);
	}

	struct copydata_in_main_callback : main_thread_callback
	{
		copydata_in_main_callback(COPYDATASTRUCT cds) : cds(cds) {}
		COPYDATASTRUCT cds;

		void callback_run() override
		{
			static_api_ptr_t<playback_control> control;
			static_api_ptr_t<metadb> db;

			switch (cds.dwData)
			{
			case FOOBAR_PLUGIN_CMD_PlayPause:
				control->play_or_pause();
				break;
			case FOOBAR_PLUGIN_CMD_Stop:
				control->stop();
				break;
			case FOOBAR_PLUGIN_CMD_Previous:
				control->start(playback_control::track_command_prev, false);
				break;
			case FOOBAR_PLUGIN_CMD_Next:
				control->start(playback_control::track_command_next, false);
				break;
			case FOOBAR_PLUGIN_CMD_ToggleSAC:
				control->toggle_stop_after_current();
				deskband_actions::send_stop_after_current(control->get_stop_after_current());
				break;
			case FOOBAR_PLUGIN_CMD_Random:
				control->start(playback_control::track_command_rand, false);
				break;
			case FOOBAR_PLUGIN_CMD_FormatString:
			{
				GUID id = *(GUID*)cds.lpData;
				char *fmt = ((char *)(cds.lpData) + sizeof(GUID));
				pfc::string8 fmt8 = pfc::string8(fmt, cds.cbData - sizeof(GUID));

				service_ptr_t<titleformat_object> format;
				static_api_ptr_t<titleformat_compiler>()->compile(format, fmt8);
				pfc::string8 value;
				control->playback_format_title(NULL, value, format, NULL, control->display_level_all);
				format.release();

				deskband_actions::send_track_text(id, value);
			}
			break;
			/*case FOOBAR_PLUGIN_CMD_FilePath:
			{
				int index = *(int*)cds.lpData;

				metadb_handle_ptr h;
				control->get_now_playing(h);
				pfc::string8 path(h->get_location().get_path());
				deskband_actions::send_file_path(index, path);
			}
			break;*/
			case FOOBAR_PLUGIN_CMD_Seek:
			{
				int position = *(int*)cds.lpData;
				control->playback_seek((double)position);
			}
			break;
			case FOOBAR_PLUGIN_CMD_Volume:
			{
				float volume = *(float*)cds.lpData;
				control->set_volume(volume);
			}
			break;
			case FOOBAR_PLUGIN_CMD_ResendLastState:
				deskband_actions::resend_last_state();
				break;
			case FOOBAR_PLUGIN_CMD_ResendLastNonTrackState:
				deskband_actions::resend_last_nontrack_state();
				break;
			case FOOBAR_PLUGIN_CMD_Activate:
				ShowWindow(core_api::get_main_window(), SW_RESTORE);
				break;
			case FOOBAR_PLUGIN_CMD_GetVersion:
				deskband_actions::send_version();
				break;
			case FOOBAR_PLUGIN_CMD_SetPlaylistFormat:
			{
				char *fmt = (char *)(cds.lpData);
				deskband_actions::set_playlist_format(fmt, cds.cbData);
				deskband_actions::handle_playlist_change(0);
			}
			break;
			case FOOBAR_PLUGIN_CMD_StartPlaylistIndex:
			{
				int index = *(int*)cds.lpData;
				//TODO: figure this out

				static_api_ptr_t<playlist_manager> pm;
				//t_size active_playlist = pm->get_active_playlist();
				//bit_array_true t;
				//metadb_handle_list meta_list;
				//pm->playlist_get_items(active_playlist, meta_list, t);
				//auto meta_item = meta_list.get_item(index);
				//auto meta_handle = meta_item.get_ptr();
				//pm->queue_add_item(meta_item);
				//pm->activeplaylist_set_selection_single(index, true);
				//control->stop();
				//control->start();
				pm->activeplaylist_execute_default_action(index);

			}
			break;
			case FOOBAR_PLUGIN_CMD_RequestVisualizationData:
			{
				double time;
				if (vis_stream->get_absolute_time(time))
				{
					audio_chunk_impl chunk;
					vis_stream->get_chunk_absolute(chunk, time, 100 * 0.001); // 100ms

					t_uint32 channel_count = chunk.get_channel_count();
					t_uint32 sample_count_total = chunk.get_sample_count();
					const audio_sample *samples = chunk.get_data();

					//console::formatter() << "Vis. data requested:  channels: " << channel_count << ", samples:" << sample_count_total;

					//TODO: send samples to deskband
				}
			}
			break;
			}

			// free lpData memory
			delete cds.lpData;
		}
	};

	LRESULT on_copydata(PCOPYDATASTRUCT p_cds)
	{
		// prepare CDS for callback
		COPYDATASTRUCT cds = { 0 };
		cds.cbData = p_cds->cbData;
		cds.dwData = p_cds->dwData;
		cds.lpData = new char[cds.cbData];
		CopyMemory(cds.lpData, p_cds->lpData, cds.cbData);

		// request callback in main thread, it should free data memory in lpData
		static_api_ptr_t<main_thread_callback_manager>()->add_callback(new service_impl_t<copydata_in_main_callback>(cds));

		return TRUE;
	}

	LRESULT CALLBACK WndProc(HWND hwnd, UINT uMsg, WPARAM wParam, LPARAM lParam)
	{
		LRESULT lResult = 0;

		switch (uMsg)
		{
		case WM_COPYDATA:
			lResult = on_copydata((PCOPYDATASTRUCT)lParam);
			break;
		default:
			lResult = DefWindowProc(hwnd, uMsg, wParam, lParam);
		}

		return lResult;
	}
}