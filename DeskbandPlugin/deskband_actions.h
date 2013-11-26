#pragma once
namespace deskband_actions
{
	void show();
	void hide();
	void send_track_length(double length);
	void send_track_time(double time);
	void send_track_text(int index, pfc::string8 text);
	void send_file_path(int index, pfc::string8 path);
	void send_pause_state(bool state);
	void send_stop();
	void send_track_volume(float volume);
	void send_stop_after_current(bool state);
	void send_album_art(const void *art, t_size len);
	void resend_last_state();
	void resend_last_nontrack_state();
	void send_version();
};
