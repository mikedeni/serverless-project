pipeline {
    agent {
        label 'mac'
    }

    environment {
        PATH = "/opt/homebrew/bin:/usr/local/bin:${PATH}"
        BACKEND_IMAGE  = "mikedeni/mybrick-backend"
        FRONTEND_IMAGE = "mikedeni/mybrick-frontend"
        IMAGE_TAG      = "${BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test app/backend.tests/ConstructionSaaS.Tests.csproj -v n'
            }
        }

        stage('Docker Build') {
            steps {
                sh "docker build -t ${BACKEND_IMAGE}:${IMAGE_TAG} ./app/backend"
                sh "docker build -t ${FRONTEND_IMAGE}:${IMAGE_TAG} ./app/frontend"
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
                        echo $DOCKER_PASS | docker login -u $DOCKER_USER --password-stdin
                        docker push ${BACKEND_IMAGE}:${IMAGE_TAG}
                        docker push ${FRONTEND_IMAGE}:${IMAGE_TAG}
                    '''
                }
            }
        }

        stage('Deploy') {
            steps {
                sh 'cd terraform && terraform init && terraform apply -auto-approve'
                sh 'cd ansible && ansible-playbook -i inventory playbook.yml'
                sh 'kubectl apply -f k8s/'
            }
        }
    }

    post {
        always {
            sh 'docker logout'
        }
    }
}
