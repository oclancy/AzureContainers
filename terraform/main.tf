resource "aws_s3_bucket" "db_backup" {
  bucket = "firmus-db-backup"

  tags = {
    Name        = "db-backup"
    Environment = "Dev"
  }
}

output "bucket" {
  value = aws_s3_bucket.db_backup.arn
}