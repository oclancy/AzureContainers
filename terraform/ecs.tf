resource "aws_ecs_cluster" "main" {
  name = "${var.product}-cluster-${var.environment}"
}


resource "aws_ecs_task_definition" "main" {
  family = var.product
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn
  container_definitions = jsonencode([
     {
      name        = "console1-container-${var.environment}"
      image       = "firmus5oftware/consoleapp1:latest"
      essential   = true
    },
    {
      name        = "cosnole2-container-${var.environment}"
      image       = "firmus5oftware/consoleapp2:latest"
      essential   = true
    },
     {
      name        = "redis-container-${var.environment}"
      image       = "redis:latest"
      essential   = true
      portMappings = [{
        protocol      = "tcp"
        containerPort = 6379
        hostPort      = 6379
      }]
    }

  ])
}

resource "aws_iam_role" "ecs_task_role" {
  name = "${var.product}-ecsTaskRole"

  assume_role_policy = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
   {
     "Action": "sts:AssumeRole",
     "Principal": {
       "Service": "ecs-tasks.amazonaws.com"
     },
     "Effect": "Allow",
     "Sid": ""
   }
 ]
}
EOF
}

resource "aws_iam_role" "ecs_task_execution_role" {
  name = "${var.product}-ecsTaskExecutionRole"

  assume_role_policy = <<EOF
{
 "Version": "2012-10-17",
 "Statement": [
   {
     "Action": "sts:AssumeRole",
     "Principal": {
       "Service": "ecs-tasks.amazonaws.com"
     },
     "Effect": "Allow",
     "Sid": ""
   }
 ]
}
EOF
}

resource "aws_iam_role_policy_attachment" "ecs-task-execution-role-policy-attachment" {
  role       = aws_iam_role.ecs_task_execution_role.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

resource "aws_ecs_service" "main" {
  name                               = "${var.product}-service-${var.environment}"
  cluster                            = aws_ecs_cluster.main.id
  task_definition                    = aws_ecs_task_definition.main.arn
  desired_count                      = 1
  # deployment_minimum_healthy_percent = 20
  # deployment_maximum_percent         = 100
  launch_type                        = "FARGATE"
  scheduling_strategy                = "REPLICA"

  network_configuration {
    subnets          = aws_subnet.private.*.id
    assign_public_ip = false
  }

  # load_balancer {
  #   target_group_arn = var.aws_alb_target_group_arn
  #   container_name   = "${var.product}-container-${var.environment}"
  #   container_port   = var.container_port
  # }

  lifecycle {
    ignore_changes = [task_definition, desired_count]
  }
}