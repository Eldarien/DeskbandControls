#include "stdafx.h"
#include "msgwindow.h"
#include "deskband_actions.h"
#include "setup_helper.h"

DECLARE_COMPONENT_VERSION(DESKBAND_CONTROLS_TITLE, DESKBAND_CONTROLS_VERSION, DESKBAND_CONTROLS_ABOUT_TEXT)

namespace deskband_controls_plugin
{
	class initquit_handler : public initquit {
		virtual void on_init()
		{
			deskband_actions::initialize_vis_stream();

			msgwindow::create();

			if (!setup_helper::is_deskband_installed()) {
				setup_helper::launch_installer();
			}

			deskband_actions::send_version();
			deskband_actions::show();
		}

		virtual void on_quit()
		{
			msgwindow::destroy();

			deskband_actions::hide();
		}
	};
	static initquit_factory_t<initquit_handler> foo_initquit;

	class playlist_handler : public playlist_callback_static
	{
	public:
		virtual unsigned get_flags()
		{
			return flag_on_items_added
				| flag_on_items_reordered
				| flag_on_items_removed
				| flag_on_items_modified;
		}

		virtual void on_items_added(t_size p_playlist, t_size p_start, const pfc::list_base_const_t<metadb_handle_ptr> & p_data, const bit_array & p_selection)
		{
			deskband_actions::handle_playlist_change(p_playlist);
		}
		virtual void on_items_reordered(t_size p_playlist, const t_size * p_order, t_size p_count)
		{
			deskband_actions::handle_playlist_change(p_playlist);
		}
		virtual void on_items_removing(t_size p_playlist, const bit_array & p_mask, t_size p_old_count, t_size p_new_count) {}
		virtual void on_items_removed(t_size p_playlist, const bit_array & p_mask, t_size p_old_count, t_size p_new_count)
		{
			deskband_actions::handle_playlist_change(p_playlist);
		}
		virtual void on_items_selection_change(t_size p_playlist, const bit_array & p_affected, const bit_array & p_state) {}
		virtual void on_item_focus_change(t_size p_playlist, t_size p_from, t_size p_to) {}

		virtual void on_items_modified(t_size p_playlist, const bit_array & p_mask)
		{
			deskband_actions::handle_playlist_change(p_playlist);
		}

		virtual void on_items_modified_fromplayback(t_size p_playlist, const bit_array & p_mask, play_control::t_display_level p_level) {}
		virtual void on_items_replaced(t_size p_playlist, const bit_array & p_mask, const pfc::list_base_const_t<t_on_items_replaced_entry> & p_data) {}
		virtual void on_item_ensure_visible(t_size p_playlist, t_size p_idx) {}
		virtual void on_playlist_activate(t_size p_old, t_size p_new) {}
		virtual void on_playlist_created(t_size p_index, const char * p_name, t_size p_name_len) {}
		virtual void on_playlists_reorder(const t_size * p_order, t_size p_count) {}
		virtual void on_playlists_removing(const bit_array & p_mask, t_size p_old_count, t_size p_new_count) {}
		virtual void on_playlists_removed(const bit_array & p_mask, t_size p_old_count, t_size p_new_count) {}
		virtual void on_playlist_renamed(t_size p_index, const char * p_new_name, t_size p_new_name_len) {}
		virtual void on_default_format_changed() {}
		virtual void on_playback_order_changed(t_size p_new_index) {}
		virtual void on_playlist_locked(t_size p_playlist, bool p_locked) {}
	};
	static service_factory_single_t<playlist_handler> foo_playlist_handler;

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

		float _last_volume_step = 1.0f;

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
			static_api_ptr_t<playback_control_v2> control;
			deskband_actions::send_track_time(p_time);
			deskband_actions::send_stop_after_current(control->get_stop_after_current());

			float step = control->get_volume_step();
			if (step != _last_volume_step)
			{
				_last_volume_step = step;
				deskband_actions::send_track_volume(control->get_volume(), step);
			}
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
			static_api_ptr_t<playback_control_v2> control;
			deskband_actions::send_track_volume(p_new_val, control->get_volume_step());
		}

		void on_playback_new_track(metadb_handle_ptr p_track)
		{
			static_api_ptr_t<playback_control_v2> control;
			deskband_actions::send_track_length(p_track->get_length()); // main flag that new track is starting
			deskband_actions::send_track_volume(control->get_volume(), control->get_volume_step());
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

			//foo_playlist_handler.get_static_instance().handle_playlist_change(0);
			deskband_actions::handle_playlist_change(0);
		}
	};
	static play_callback_static_factory_t<playback_handler> foo_playback;
}