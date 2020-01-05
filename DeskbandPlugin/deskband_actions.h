#pragma once
namespace deskband_actions
{
	void show();
	void hide();
	void send_track_length(double length);
	void send_track_time(double time);
	void send_track_text(GUID id, pfc::string8 text);
	void set_playlist_format(char* fmt, t_size len);
	void handle_playlist_change(t_size p_playlist);
	t_size get_last_playlist_current_index();
	void send_pause_state(bool state);
	void send_stop();
	void send_track_volume(float volume, float step);
	void send_stop_after_current(bool state);
	void send_album_art(const void *art, t_size len, bool stub);
	void resend_last_state();
	void resend_last_nontrack_state();
	void send_version();

	void initialize_vis_stream();
	void send_visualization_data();
};
