#! /usr/bin/bash

set -euo pipefail
IFS=$'\n\t'

function switch
{
    output="/tmp/assume-role-output.json"

    #aws sts assume-role --role-arn "arn:aws:iam::522854165171:role/FirmusDeveloper" --role-session-name AWSCLI-Session > $output
    (aws sts assume-role --role-arn "arn:aws:iam::${ACCOUNT_ID}:role/${ASSUMED_ROLE}" --role-session-name AWSCLI-Session) > $output
    AccessKeyId=$(cat $output | jq -r '.Credentials.AccessKeyId')
    SecretAccessKey=$(cat $output | jq -r '.Credentials.SecretAccessKey')
    SessionToken=$(cat $output | jq -r '.Credentials.SessionToken')

    cat <<EOF
    export AWS_ACCESS_KEY_ID=$AccessKeyId
    export AWS_SECRET_ACCESS_KEY=$SecretAccessKey
    export AWS_SESSION_TOKEN=$SessionToken
EOF
}

eval switch