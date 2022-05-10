
include config/.env

ENV=dev
TFVARS=-var-file="config/${ENV}.tfvars.json"
OUTFILE= "out.plan"
BACKEND=-backend-config="config/${ENV}.backend"

source fmt:
	terraform fmt

init: 
	terraform init -reconfigure ${TFVARS} ${BACKEND}

validate: 
	terraform validate

plan: init
	terraform plan ${TFVARS} -out ${OUTFILE} 

apply: 
	terraform apply ${OUTFILE} 

destroy:
	terraform destroy ${TFVARS}