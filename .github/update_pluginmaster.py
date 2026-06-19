import json, os, sys, time

repo_path = sys.argv[1]
version = sys.argv[2]
repo_full_name = sys.argv[3]

json_path = os.path.join(repo_path, 'pluginmaster.json')

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

entry = data[0] if data else {}

download_url = f"https://github.com/{repo_full_name}/releases/download/v{version}/JustBackup.zip"

entry['Name'] = 'JustBackup'
entry['Author'] = 'NightmareXIV, QianChangUwU'
entry['Punchline'] = '自动备份游戏和插件配置。'
entry['Description'] = '在游戏/插件启动时自动备份你的游戏和插件配置，支持自动清理旧备份、7-zip压缩、角色配置识别等。'
entry['InternalName'] = 'JustBackup'
entry['AssemblyVersion'] = version
entry['TestingAssemblyVersion'] = version
entry['DalamudApiLevel'] = 15
entry['TestingDalamudApiLevel'] = 15
entry['DownloadLinkInstall'] = download_url
entry['DownloadLinkUpdate'] = download_url
entry['DownloadLinkTesting'] = download_url
entry['RepoUrl'] = f'https://github.com/{repo_full_name}'
entry['Tags'] = ['backup', 'utility', 'cn']
entry['ApplicableVersion'] = 'any'
entry['LoadPriority'] = 0
entry['AcceptsFeedback'] = True
entry['LastUpdate'] = int(time.time())

if not data:
    data = [entry]
else:
    data[0] = entry

with open(json_path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=4, ensure_ascii=False)
