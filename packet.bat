@echo off
cd Gsof.Google.Analytics
call nuget pack Gsof.Google.Analytics.nuspec -properties Configuration=Release;version="0.1.1";id="Gsof.Google.Analytics" -Verbosity detailed -Exclude dmidecode.exe -OutputDirectory ..\build

cd ..