# ExtensionSample
[![Build status](https://ci.appveyor.com/api/projects/status/1ffrdoc3o9m5fti2?svg=true)](https://ci.appveyor.com/project/bouchaet/extensionsample) [![Coverage Status](https://coveralls.io/repos/github/bouchaet/ExtensionSample/badge.svg?branch=master)](https://coveralls.io/github/bouchaet/ExtensionSample?branch=master) 
[![MIT licensed](https://img.shields.io/badge/license-MIT-blue.svg)](https://raw.githubusercontent.com/bouchaet/extensionsample/master/LICENSE)  
Dependency Rule playground

## Issues
If having `GetReferenceNearestTargetFrameworkTask` error with omnisharp on Windows,
run the following command (powershell). [see reference](https://developercommunity.visualstudio.com/content/problem/137779/the-getreferencenearesttargetframeworktask-task-wa.html)
```
Start-Process "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vs_installer.exe" -ArgumentList 'modify --installPath "C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools" --quiet --add Microsoft.VisualStudio.Component.NuGet.BuildTools --add Microsoft.Net.Component.4.5.TargetingPack --norestart --force' -Wait -PassThru
```
