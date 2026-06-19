import json, os, sys, time

repo_path = sys.argv[1]
version = sys.argv[2]
repo_full_name = sys.argv[3]

json_path = os.path.join(repo_path, 'pluginmaster.json')

with open(json_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

download_url = f"https://github.com/{repo_full_name}/releases/download/v{version}/JustBackup.zip"

entry = {
    'Name': 'JustBackup',
    'Author': 'NightmareXIV, QianChangUwU',
    'Punchline': '自动备份游戏和插件配置。',
    'Description': '在游戏/插件启动时自动备份你的游戏和插件配置，支持自动清理旧备份、7-zip压缩、角色配置识别等。',
    'InternalName': 'JustBackup',
    'AssemblyVersion': version,
    'TestingAssemblyVersion': version,
    'DalamudApiLevel': 15,
    'TestingDalamudApiLevel': 15,
    'DownloadLinkInstall': download_url,
    'DownloadLinkUpdate': download_url,
    'DownloadLinkTesting': download_url,
    'RepoUrl': f'https://github.com/{repo_full_name}',
    'IconUrl': download_url,
    'Tags': ['backup', 'utility', 'cn'],
    'ApplicableVersion': 'any',
    'LoadPriority': 0,
    'AcceptsFeedback': True,
    'LastUpdate': int(time.time()),
}

idx = next((i for i, e in enumerate(data) if e.get('InternalName') == 'JustBackup'), -1)
if idx >= 0:
    data[idx] = entry
else:
    data.append(entry)

with open(json_path, 'w', encoding='utf-8') as f:
    json.dump(data, f, indent=4, ensure_ascii=False)
