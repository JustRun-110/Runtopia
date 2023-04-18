.\sonar-scanner-msbuild-4.7.1.2311-net46\SonarScanner.MSBuild.exe begin /k:"UnityProject" /d:sonar.host.url="http://j8b110.p.ssafy.io:9000" /d:sonar.login="sqp_d1841f5e3eb90eee1ad03addc544d22560a12112"

"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" Runtopia.sln -t:Rebuild

.\sonar-scanner-msbuild-4.7.1.2311-net46\SonarScanner.MSBuild.exe end /d:sonar.login="sqp_d1841f5e3eb90eee1ad03addc544d22560a12112"