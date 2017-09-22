@echo off
cd Gsof.Google.Analytics
call nuget pack -properties Configuration=Release -Verbosity detailed -Exclude dmidecode.exe -OutputDirectory ..\build

cd ..