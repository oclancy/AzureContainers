
include config/env

ENV=dev
TFVARS=-var-file="config/${ENV}.tfvars.json"
OUTFILE=-out "out.plan"
BACKEND=-backend-config="config/${ENV}.backend"

source fmt:
	terraform fmt

init: 
	terraform ${TFVARS} ${BACKEND}

validate:
	terraform validate

plan:
	terraform plan ${TFVARS} ${OUTFILE} 

destroy:
	terraform destroy