
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

