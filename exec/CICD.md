# Project : Runtopia

Runtopia 프로젝트의 배포 방식은 Jenkins Multibranch Pipeline을 활용한 CI/CD 자동화 환경으로 구성되어 있습니다.

Webhook이 설정되어 있어 Gitlab의 Push Or Merge가 일어날때마다 모든 브랜치에서 분기에 따른 Pipeline에 따라 Build를 진행하여 Gradle로 백엔드를 빌드하고 SonarQube Analysis를 자동으로 전송하여 정적 분석을 진행합니다. 또한 설치된 Unity Editor를 통해 Build를 진행하고 Dockerfile을 통해 Image를 생성한 후 다른 이미지들과 연계되어 Docker compose를 통해 배포하는 방식을 사용하고 있습니다.

## Nginx Port forwarding

| Port | Content            |
| ---- | ------------------ |
| 80   | HTTP 443 Redirect  |
| 443  | HTTPS Frontend     |
| 3306 | Mysql 5.7          |
| 5432 | Postgres           |
| 6379 | Redis for openvidu |
| 8000 | Backend            |
| 9000 | Sonarqube          |
| 9090 | Jenkins            |

## Step 1: Docker And Docker Compose Install

https://docs.docker.com/engine/install/ubuntu/

## Step 2: SSL Install

### SSL 발급

```sh
sudo apt-get install letsencrypt
sudo letsencrypt certonly --standalone -d 도메인
# 발급 경로
cd /etc/letsencrypt/live/도메인/

```

/etc/letsencrypt/options-ssl-nginx.conf 파일 생성

```
cd /etc/letsencrypt
vi options-ssl-nginx.conf
```

```sh
# This file contains important security parameters. If you modify this file
# manually, Certbot will be unable to automatically provide future security
# updates. Instead, Certbot will print and log an error message with a path to
# the up-to-date file that you will need to refer to when manually updating
# this file. Contents are based on https://ssl-config.mozilla.org

ssl_session_cache shared:le_nginx_SSL:10m;
ssl_session_timeout 1440m;
ssl_session_tickets off;

ssl_protocols TLSv1.2 TLSv1.3;
ssl_prefer_server_ciphers off;

ssl_ciphers "ECDHE-ECDSA-AES128-GCM-SHA256:ECDHE-RSA-AES128-GCM-SHA256:ECDHE-ECDSA-AES256-GCM-SHA384:ECDHE-RSA-AES256-GCM-SHA384:ECDHE-ECDSA-CHACHA20-POLY1305:ECDHE-RSA-CHACHA20-POLY1305:DHE-RSA-AES128-GCM-SHA256:DHE-RSA-AES256-GCM-SHA384";
```

## Step 3: Jenkins Install

### JDK 설치

sudo apt install openjdk-11-jdk

### jenkins repository key 추가, 및 패키지 저장소 추가다운로드

```sh
sudo wget -q -O - https://pkg.jenkins.io/debian-stable/jenkins.io.key | sudo apt-key add -
sudo sh -c 'echo deb https://pkg.jenkins.io/debian-stable binary/ > \
    /etc/apt/sources.list.d/jenkins.list'

```

### apt-get 업데이트 및 젠킨스 설치

```sh
sudo apt-get update
sudo apt-get install jenkins
```

### 젠킨스 실행( + 명령어)

```sh
//시작
sudo service jenkins start

//종료
sudo service jenkins stop

//재시작
sudo service jenkins restart

//젠킨스 상태보기
sudo service jenkins status
```

### 젠킨스 암호 확인

```sh
sudo cat /var/lib/jenkins/secrets/initialAdminPassword
```

### 젠킨스 포트 변경

```sh
sudo vim /etc/default/jenkins
```

HTTP_PORT 변경

Ctrl-O 저장

Ctrl-X 나가기

```sh
sudo chmod 777 /usr/lib/systemd/system/jenkins.service
sudo vim /usr/lib/systemd/system/jenkins.service
sudo chmod 444 /usr/lib/systemd/system/jenkins.service
```

```sh
sudo systemctl daemon-reload
sudo service jenkins restart
```

### 포트 확인

```sh
sudo lsof -i -P -n | grep jenkins
```

## Step 4: Unity 설치

Unity Editor를 설치하기 위해 Unity Hub 설치

https://docs.unity3d.com/hub/manual/InstallHub.html#install-hub-linux

GUI가 필요하므로 VNC를 먼저 설치

### VNC 설치를 먼저 진행

1. Install the necessary packages:

```sh
sudo apt-get update
sudo apt-get install -y xfce4 xfce4-goodies tightvncserver

```

2. Set up a VNC password:

```
vncpasswd

```

3. Create a VNC configuration file:

```
vim ~/.vnc/xstartup

```

Add the following lines to the file:

```sh
#!/bin/sh
xrdb $HOME/.Xresources
startxfce4 &

```

4. Make the configuration file executable:

```
chmod +x ~/.vnc/xstartup

```

5. Start the VNC server:

```
vncserver :1

```

6. VNC 클라이언트 활용해서 your-ec2-public-dns:5901
   로 들어가기

```
# public ip 출력
curl http://169.254.169.254/latest/meta-data/public-ipv4
```

### UnityHub 로그인

- 로그인이 필요하므로 브라우저 설치하기

1. Google Chrome 다운로드 페이지에서 최신 버전의 .deb 파일을 다운로드합니다:

```sh
wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb
```

2. 다운로드한 .deb 파일을 사용하여 Google Chrome을 설치합니다:

```sh
sudo apt-get install -y ./google-chrome-stable_current_amd64.deb
```

### Unity Trouble Shooting

Unity Hub의 URI Handler가 제대로 구성되지 않았기 때문에 xdg-open을 했을 때 오류가 발생할 수 있다.

- open xdg-open 에러가 뜰 경우

방법 1

- args에 넣어서 강제로 열기

1. 애플리케이션 메뉴에서 프로그래밍 > Unity Hub를 엽니다.
2. "Sign In"을 클릭합니다.
3. 빈 창이 열린 상태에서 소스를 표시하려면 Ctrl+U를 누릅니다.
4. 소스에서 "unityhub:xxx"를 포함하는 문자열을 복사합니다.
5. Unity Hub를 닫습니다.
6. 이전에 복사한 문자열로 터미널에서 Unity Hub를 시작합니다.

예를 들면 다음과 같습니다:

```sh
./UnityHub.AppImage --args 'unityhub:xxx'
```

이렇게 하면 Unity Hub가 로그인한채로 실행됩니다.

방법 2

- Unity Hub에 대한 사용자 지정 데스크톱 항목을 생성하고 URI 처리기를 설정하기

1. Create a new desktop entry file for Unity Hub:

```
sudo vim /usr/share/applications/unityhub.desktop
```

2. Add the following content to the file:

```
[Desktop Entry]
Type=Application
Name=Unity Hub
Comment=Unity Hub
#replace path
Exec=/path/to/UnityHub.AppImage %u
Icon=unityhub
Terminal=false
MimeType=x-scheme-handler/unityhub;
```

Exec=/path/to/UnityHub.AppImage %u
부분에서 path 위치 설정해주기

3. Update the MIME database:

```
sudo update-desktop-database
```

4. Set Unity Hub as the default handler for unityhub:// URIs:

```
xdg-mime default unityhub.desktop x-scheme-handler/unityhub
```

이후 UnityEditor 설치 후 CLI를 사용하기 위해 라이센스 발급 Unity_v2017.x.ulf

## Step 5: Jenkins Multibranch PipeLine 설정

### Multibranch Webhook 설정

Jenkins - Multibranch Scan Webhook Trigger 플러그인 설치

![image](https://user-images.githubusercontent.com/76441040/229709835-680171f9-1b55-4d13-87c5-734a27711355.png)

Multibranch 설정에서 webhook token 설정

![image](https://user-images.githubusercontent.com/76441040/229710382-88f33003-d24e-4d54-a8e3-3a6e2cda59ac.png)

Jenkins server URL 입력, 마지막에 token={토큰값}

Username과 Password는 Jenkins에 로그인할 수 있는 값 입력

Enable SSL verification 사용시 Jenkins에서 CSRF 설정

### JenkinsFile 작성

```sh

pipeline {
 agent any

 environment {
   GIT_URL = "https://lab.ssafy.com/s08-metaverse-game-sub2/S08P22B110.git"
   UNITY_PROJECT_PATH = "${WORKSPACE}/Runtopia"
   UNITY_PATH = "/home/ubuntu/Unity/Hub/Editor/2021.3.19f1/Editor"
   UNITY_LICENSE = "/home/ubuntu/Downloads/Unity_v2017.x.ulf"
 }

 tools {
   gradle "gradle-api"
 }

 stages {
   stage('Start'){
    steps{
      script{
        Author_ID = sh(script: "git show -s --pretty=%an", returnStdout: true).trim()
        Author_Name = sh(script: "git show -s --pretty=%ae", returnStdout: true).trim()
        mattermostSend(
          color: "#2A42EE",
          icon: "https://jenkins.io/images/logos/jenkins/jenkins.png",
          message: "Build Start : ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>)",
          channel: "s08p11b1@b110_bot",
          endpoint: "https://meeting.ssafy.com/hooks/rgqqwmnz7fyf5nzy3zebru8pgr")
      }

    }
   }
   stage('Pull') {
     steps {
       script {
         git url: "${GIT_URL}", branch: env.GIT_BRANCH, credentialsId: 'GitLabToken', poll: true, changelog: true
       }
     }
   }

   stage('SpringBoot Build') {
     steps {
       script {
         dir('backend') {
          sh 'chmod +x ./gradlew'
          sh './gradlew clean build'

         }
       }
     }
   }
   stage('Backend SonarQube Analysis') {
    when {
      expression {
        env.GIT_BRANCH == 'develop' ||
        (env.GIT_BRANCH =~ /^release-\d+$/).matches() ||
        env.GIT_BRANCH == 'main'
      }
    }
    steps{
      withSonarQubeEnv(installationName : 'SonarQube-Server') {
        dir('backend') {
          sh 'chmod +x ./gradlew'
          sh "./gradlew sonar"
          }
        }
      }
    }
   stage('Build Windows') {
      when {
        expression {
          env.GIT_BRANCH == 'develop' ||
          (env.GIT_BRANCH =~ /^release-\d+$/).matches() ||
          env.GIT_BRANCH == 'main'
        }
      }
      steps {
        sh "sudo ${UNITY_PATH}/Unity -batchmode -nographics -silent-crashes -logFile - -quit -manualLicenseFile ${UNITY_LICENSE}"
        sh "sudo ${UNITY_PATH}/Unity -batchmode -nographics -silent-crashes -logFile - -projectPath ${UNITY_PROJECT_PATH} -executeMethod Builder.BuildWindow -quit"
      }
    }
    // stage('Compress Builds') {
    //   when {
    //     expression {
    //       env.GIT_BRANCH == 'develop' ||
    //       (env.GIT_BRANCH =~ /^release-\d+$/).matches() ||
    //       env.GIT_BRANCH == 'main'
    //     }
    //   }
    //   steps {
    //     sh "cd ${UNITY_PROJECT_PATH}/Build/windows && sudo zip -r Runtopia.zip ."
    //     sh "mkdir -p ${WORKSPACE}/files && sudo mv ${UNITY_PROJECT_PATH}/Build/windows/Runtopia.zip ${WORKSPACE}/files"

    //   }
    // }
 }
  post {
    success {
      script{
        Author_ID = sh(script: "git show -s --pretty=%an", returnStdout: true).trim()
        Author_Name = sh(script: "git show -s --pretty=%ae", returnStdout: true).trim()
        mattermostSend(
          color: "#00f514",
          icon: "https://jenkins.io/images/logos/jenkins/jenkins.png",
          message: "Build SUCCESS : ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>)",
          channel: "s08p11b1@b110_bot",
          endpoint: "https://meeting.ssafy.com/hooks/rgqqwmnz7fyf5nzy3zebru8pgr")
      }
    }
    failure  {
      script{
        Author_ID = sh(script: "git show -s --pretty=%an", returnStdout: true).trim()
        Author_Name = sh(script: "git show -s --pretty=%ae", returnStdout: true).trim()
        mattermostSend(
          color: "#e00707",
          icon: "https://jenkins.io/images/logos/jenkins/jenkins.png",
          message: "Build FAILED : ${env.JOB_NAME} #${env.BUILD_NUMBER} (<${env.BUILD_URL}|Link to build>)",
          channel: "s08p11b1@b110_bot",
          endpoint: "https://meeting.ssafy.com/hooks/rgqqwmnz7fyf5nzy3zebru8pgr")
      }
    }
  }
}


```

파이프라인 설정

> env : 환경변수를 통해 unityEditor 위치, Git Url 등등 설정

> tools : 사용할 tool 설정 (gradle)

> Start : build 시작 시 mattermost로 알림 보내기

> Pull : 미리 설정한 Cretential를 통해 Git pull

> SpringBoot Build : Backend Gradle Build

> Backend SonarQube Analysis : Backend SonarQube 정적 분석을 실시하고 미리 설정한 웹훅을 통해 SonarServer로 보내기

> Build Windows : 미리 작성한 Builder.cs 스크립트 파일을 통해 Unity Cli build 빌드한 파일은 Build/Windows에 저장

> Compress Builds : 빌드한 파일을 압축하여 루트 프로젝트의 files로 이동 container된 백엔드와 공유 폴더를 지정하여 파일을 다운로드 할 수 있게 설정

> Multibranch PipeLine 완료

![젠킨스 멀티브랜치](https://user-images.githubusercontent.com/76441040/229729970-223f2c45-dd6c-4356-ac8e-a8ec55ce8105.png)

### UnityBuilder Script 작성

${UnityProject}/Assets/Editor/Builder.cs

```c#
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;

public class Builder
{
    // Change this to the path where you want to save the WebGL build
    private static string outputPath = "Build/windows";

    [MenuItem("Build/Build Window")]
    public static void BuildWindow()
    {
        // Build 디렉토리가 없으면 생성
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }


        string[] scenes = { "Assets/Scenes/Login.unity",
                            "Assets/Scenes/Lobby.unity",
                            "Assets/Scenes/Square.unity",
                            "Assets/Scenes/Game_Multi.unity",
                            "Assets/Scenes/Game_End.unity" };
        BuildPipeline.BuildPlayer(scenes, outputPath + "/Game.exe", BuildTarget.StandaloneWindows64, BuildOptions.CleanBuildCache);
        Debug.Log("Windows build completed and saved in " + outputPath);
    }
}
```

> Local에서 Test

```

${UNITY_PATH}/Unity -batchmode -nographics -silent-crashes -logFile - -projectPath ${UNITY_PROJECT_PATH} -executeMethod Builder.BuildWindow -quit
```

## Step 6 : Docker Compose 설정

```docker
version: "3"

services:
  database-mysql:
    container_name: database-mysql
    image: mysql/mysql-server:5.7

    environment:
      MYSQL_ROOT_PASSWORD: "root"
      MYSQL_ROOT_HOST: "%"
      MYSQL_DATABASE: "runtopia_web_db"
      TZ: Asia/Seoul

    volumes:
      - ./db/mysql-init.d:/docker-entrypoint-initdb.d

    ports:
      - "13306:3306"

    command:
      - --character-set-server=utf8mb4
      - --collation-server=utf8mb4_unicode_ci
    networks:
      - runtopia_network
  redis:
    container_name: redis
    image: redis:latest
    command: redis-server --port 6379
    ports:
      - "6379:6379"
    networks:
      - runtopia_network
    labels:
      - "name=redis"
      - "mode=standalone"

  application:
    build:
      context: ./backend
      dockerfile: Dockerfile
    restart: always
    container_name: runtopia_app
    ports:
      - 8000:8000
    environment:
      SPRING_DATASOURCE_URL: jdbc:mysql://database-mysql:3306/runtopia_web_db?useUnicode=true&characterEncoding=utf8&serverTimezone=Asia/Seoul&zeroDateTimeBehavior=convertToNull&rewriteBatchedStatements=true
      SPRING_DATASOURCE_USERNAME: root
      SPRING_DATASOURCE_PASSWORD: root
      TZ: Asia/Seoul
    volumes:
      - ./files:/root/files
    depends_on:
      - database-mysql
    networks:
      - runtopia_network

  web:
    container_name: nginx
    image: nginx
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx/conf.d:/etc/nginx/conf.d
      - /etc/letsencrypt:/etc/letsencrypt
      - ./frontend:/usr/share/nginx/html
    depends_on:
      - application
    networks:
      - runtopia_network
  sonarqube:
    image: sonarqube:lts
    container_name: sonarqube
    ports:
      - "9000:9000"
    ulimits:
      nofile:
        soft: 262144
        hard: 262144
    networks:
      - runtopia_network
    environment:
      - sonar.jdbc.url=jdbc:postgresql://sonar-db:5432/sonar
    volumes:
      - sonarqube_conf:/opt/sonarqube/conf
      - sonarqube_data:/opt/sonarqube/data
      - sonarqube_extensions:/opt/sonarqube/extensions
      - sonarqube_logs:/opt/sonarqube/logs

  sonar-db:
    image: postgres
    container_name: postgres_sonar
    ports:
      - "5432:5432"
    networks:
      - runtopia_network
    environment:
      - POSTGRES_USER=sonar
      - POSTGRES_PASSWORD=sonar
    volumes:
      - postgresql:/var/lib/postgresql
      - postgresql_data:/var/lib/postgresql/data
networks:
  runtopia_network:
volumes:
  sonarqube_conf:
  sonarqube_data:
  sonarqube_extensions:
  sonarqube_logs:
  postgresql:
  postgresql_data:

```

### Nginx 설정

```nginx

server{
    listen 80;
    listen [::]:80;
    server_name j8b110.p.ssafy.io;
    server_tokens off;
    charset utf-8;
    location / {
        return 301 https://$host;
    }
}
server {
    listen 443 ssl;
    server_name j8b110.p.ssafy.io;
    ssl_certificate /etc/letsencrypt/live/j8b110.p.ssafy.io/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/j8b110.p.ssafy.io/privkey.pem;
    include /etc/letsencrypt/options-ssl-nginx.conf;

    charset utf-8;

    location / {
        root /usr/share/nginx/html;
        index index.html;
        try_files $uri $uri/ /index.html;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
            proxy_set_header Origin "";
            proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header Host $http_host;
    }
    location /api/ {
        proxy_pass http://j8b110.p.ssafy.io:8000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
            proxy_set_header Origin "";
            proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header Host $http_host;
    }


}
```

## Step 7 : Sonarqube

### backend Sonarqube 설정

Jenkins - SonarQube Scanner for Jenkins, SonarServer 플러그인 설치

![image](https://user-images.githubusercontent.com/76441040/229723140-c74229c6-4627-43cb-a91f-207aa1ccdf40.png)

SonarServer URL 설정

![image](https://user-images.githubusercontent.com/76441040/229727878-6583f852-1b0a-4095-a813-2f63e0439fa5.png)

SonarQube 컨테이너에서 WebHook 토큰 설정

### Unity Sonarqube 설정

> local에서 진행

sonar-scanner-msbuild 설치

```powershell
.\sonar-scanner-msbuild-4.7.1.2311-net46\SonarScanner.MSBuild.exe begin /k:"UnityProject" /d:sonar.host.url="http://j8b110.p.ssafy.io:9000" /d:sonar.login="{소나큐브 로그인 토큰 값}"

"{MSBUILD 경로}\MSBuild.exe" Runtopia.sln -t:Rebuild

.\sonar-scanner-msbuild-4.7.1.2311-net46\SonarScanner.MSBuild.exe end /d:sonar.login="{소나큐브 로그인 토큰 값}"
```

> SonarQube 정적분석 완료

![image](https://user-images.githubusercontent.com/76441040/229727600-1ae8570d-8bfe-4522-a281-463e817dc294.png)
