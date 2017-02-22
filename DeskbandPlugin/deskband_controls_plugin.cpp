#include "stdafx.h"
#include "msgwindow.h"
#include "deskband_actions.h"

DECLARE_COMPONENT_VERSION(DESKBAND_CONTROLS_TITLE, DESKBAND_CONTROLS_VERSION, DESKBAND_CONTROLS_ABOUT_TEXT);

namespace deskband_controls_plugin
{
	class initquit_handler : public initquit {
		virtual void on_init()
		{
			msgwindow::create();

			deskband_actions::show();
		}

		virtual void on_quit()
		{
			msgwindow::destroy();

			deskband_actions::hide();
		}
	};
	static initquit_factory_t<initquit_handler> foo_initquit;

	class playback_handler: public play_callback_static {
		unsigned get_flags()
		{
			return flag_on_playback_new_track |
				flag_on_playback_time |
				flag_on_playback_seek |
				flag_on_playback_pause |
				flag_on_playback_stop |
				flag_on_volume_change;
		}

		void on_playback_starting(play_control::t_track_command p_command,bool p_paused) {}
		void on_playback_stop(play_control::t_stop_reason p_reason)
		{
			deskband_actions::send_stop();
		}
		void on_playback_seek(double p_time)
		{
			deskband_actions::send_track_time(p_time);
		}
		void on_playback_time(double p_time)
		{
			static_api_ptr_t<playback_control> control;
			deskband_actions::send_track_time(p_time);
			deskband_actions::send_stop_after_current(control->get_stop_after_current());
		}
		void on_playback_pause(bool p_state)
		{
			deskband_actions::send_pause_state(p_state);
		}
		void on_playback_edited(metadb_handle_ptr p_track) {}
		void on_playback_dynamic_info(const file_info & p_info) {}
		void on_playback_dynamic_info_track(const file_info & p_info) {}
		void on_volume_change(float p_new_val)
		{
			deskband_actions::send_track_volume(p_new_val);
		}

		void on_playback_new_track(metadb_handle_ptr p_track)
		{
			static_api_ptr_t<playback_control> control;
			deskband_actions::send_track_length(p_track->get_length()); // main flag that new track is starting
			deskband_actions::send_track_volume(control->get_volume());
			deskband_actions::send_stop_after_current(control->get_stop_after_current());
			deskband_actions::send_pause_state(control->is_paused());

			// album art
			abort_callback_dummy abort;
			static_api_ptr_t<album_art_manager_v2> aamv2;
			album_art_data_ptr aad;
			bool stub = false;
			try
			{
				album_art_extractor_instance_v2::ptr aaeiv2 = aamv2->open(pfc::list_single_ref_t<metadb_handle_ptr>(p_track),
					pfc::list_single_ref_t<GUID>(album_art_ids::cover_front), abort);
				aad = aaeiv2->query(album_art_ids::cover_front, abort);
			}
			catch (...)
			{
				stub = true;
				try
				{
					album_art_extractor_instance_v2::ptr aaeiv2_stub = aamv2->open_stub(abort);
					aad = aaeiv2_stub->query(album_art_ids::cover_front, abort);
				} catch (...) {}
			}
			if (aad.is_valid() && aad->get_size())
			{
				deskband_actions::send_album_art(aad->get_ptr(), aad->get_size(), stub);
				aad.release();
			}
			else
			{
				deskband_actions::send_album_art(NULL, 0, stub);
			}
		}
	};
	static play_callback_static_factory_t<playback_handler> foo_playback;
}