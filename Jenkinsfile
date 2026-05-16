pipeline {
    agent any

    triggers {
        githubPush()
    }

    environment {
        PATH = "/opt/homebrew/bin:/usr/local/bin:${PATH}"
        BACKEND_IMAGE  = "mikedeni/mybrick-backend"
        FRONTEND_IMAGE = "mikedeni/mybrick-frontend"
        IMAGE_TAG      = "${BUILD_NUMBER}"
        DOCKER_BUILDKIT = "1"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Test & Build (parallel)') {
            parallel {
                stage('Test') {
                    steps {
                        sh 'dotnet test app/backend.tests/ConstructionSaaS.Tests.csproj -v minimal'
                    }
                }
                stage('Docker Build Backend') {
                    steps {
                        sh "docker build -t ${BACKEND_IMAGE}:${IMAGE_TAG} -t ${BACKEND_IMAGE}:latest ./app/backend"
                    }
                }
                stage('Docker Build Frontend') {
                    steps {
                        sh "docker build -t ${FRONTEND_IMAGE}:${IMAGE_TAG} -t ${FRONTEND_IMAGE}:latest ./app/frontend"
                    }
                }
            }
        }

        stage('Push to Hub') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'dockerhub-credentials',
                    usernameVariable: 'DOCKER_USER',
                    passwordVariable: 'DOCKER_PASS'
                )]) {
                    sh '''
                        rm -f ~/.docker/config.json
                        echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
                        docker push ${BACKEND_IMAGE}:${IMAGE_TAG} &
                        docker push ${BACKEND_IMAGE}:latest &
                        docker push ${FRONTEND_IMAGE}:${IMAGE_TAG} &
                        docker push ${FRONTEND_IMAGE}:latest &
                        wait
                    '''
                }
            }
        }

        stage('Provision Infrastructure') {
            steps {
                sh '''
                    cd terraform
                    terraform init -input=false
                    terraform apply -auto-approve -input=false
                '''
            }
        }

        stage('Configure Environment') {
            steps {
                sh 'ansible-playbook -i ansible/inventory ansible/playbook.yml'
            }
        }

        stage('Deploy') {
            steps {
                sh 'kubectl apply -f k8s/'
                sh "kubectl set image deployment/mybrick-backend mybrick-backend=${BACKEND_IMAGE}:${IMAGE_TAG} -n production"
                sh "kubectl set image deployment/mybrick-frontend mybrick-frontend=${FRONTEND_IMAGE}:${IMAGE_TAG} -n production"
            }
        }
    }

    post {
        always {
            sh 'docker logout || true'
        }
    }
}
