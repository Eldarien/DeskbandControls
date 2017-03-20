#include "stdafx.h"
#include "setup_helper.h"

namespace setup_helper
{
	HANDLE shell_execute_without_redirection(const wchar_t* cmd, const wchar_t* params, const wchar_t* verb)
	{
		// We need to run native process, not emulated 32bit one, so disable redirection for the next ShellExecuteEx call
		PVOID oldRedirectionValue = NULL;
		if (Wow64DisableWow64FsRedirection(&oldRedirectionValue))
		{
			SHELLEXECUTEINFO shExInfo = { 0 };
			shExInfo.cbSize = sizeof(shExInfo);
			shExInfo.fMask = SEE_MASK_NOCLOSEPROCESS;
			shExInfo.hwnd = 0;
			shExInfo.lpVerb = verb;
			shExInfo.lpFile = cmd;
			shExInfo.lpParameters = params;
			shExInfo.lpDirectory = NULL;
			shExInfo.nShow = SW_SHOW;
			shExInfo.hInstApp = NULL;

			BOOL shellExecuteResult = ShellExecuteEx(&shExInfo);
			Wow64RevertWow64FsRedirection(oldRedirectionValue); // Immediately re-enable redirection.
			return shellExecuteResult ? shExInfo.hProcess : NULL;
		}
		return NULL;
	}

	bool is_process_running(const wchar_t* processName)
	{
		bool exists = false;
		PROCESSENTRY32 entry;
		entry.dwSize = sizeof(PROCESSENTRY32);

		HANDLE snapshot = CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, NULL);

		if (Process32First(snapshot, &entry))
			while (Process32Next(snapshot, &entry))
				if (!_wcsicmp(entry.szExeFile, processName))
				{
					exists = true;
					break;
				}

		CloseHandle(snapshot);
		return exists;
	}

	// public functions

	bool is_deskband_installed()
	{
		HKEY hKey;
		LONG lRes = RegOpenKeyEx(HKEY_CLASSES_ROOT, L"CLSID\\{" DESKBAND_GUID "}\\InprocServer32", 0, KEY_QUERY_VALUE | KEY_WOW64_64KEY, &hKey);
		if (lRes != ERROR_SUCCESS)
		{
			RegCloseKey(hKey);
			return false;
		}

		WCHAR szBuffer[512];
		DWORD dwBufferSize = sizeof(szBuffer);
		lRes = RegQueryValueEx(hKey, L"Assembly", NULL, NULL, (LPBYTE)szBuffer, &dwBufferSize);
		if (lRes != ERROR_SUCCESS)
		{
			return false;
		}
		RegCloseKey(hKey);

		const wchar_t* versionString = L", Version=" DESKBAND_CONTROLS_VERSION ".0,"; // ", Version=a.b.c.0,"
		PTSTR searchRes = StrStr(szBuffer, versionString);
		return searchRes == NULL ? false : true;
	}

	void launch_installer()
	{
		const char* myPath = core_api::get_my_full_path();
		t_size myPathLen = pfc::strlen_utf8(myPath);
		wchar_t pathBuffer[MAX_PATH];
		pfc::stringcvt::convert_utf8_to_wide(pathBuffer, MAX_PATH, myPath, myPathLen);
		PathRemoveFileSpec(pathBuffer);

		wchar_t paramsBuf[MAX_PATH * 2] = { 0 };
		StrCat(paramsBuf, L"/c cd \"");
		StrCat(paramsBuf, pathBuffer);
		StrCat(paramsBuf, L"\" & install.cmd");

		wchar_t cmdBuf[MAX_PATH] = { 0 };
		ExpandEnvironmentStrings(L"%WINDIR%\\System32\\cmd.exe", cmdBuf, MAX_PATH);

		HANDLE cmdProcess = shell_execute_without_redirection(cmdBuf, paramsBuf, L"runas");
		if (cmdProcess)
		{
			WaitForSingleObject(cmdProcess, INFINITE);
			CloseHandle(cmdProcess);
		}
		
		// check if explorer.exe is running and launch if not
		ExpandEnvironmentStrings(L"%WINDIR%\\explorer.exe", cmdBuf, MAX_PATH);
		if (!is_process_running(cmdBuf))
		{
			shell_execute_without_redirection(cmdBuf, NULL, NULL);
		}
	}
}