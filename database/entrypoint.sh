#!/bin/bash

setup_file=setup.sql

function setup_database {
    # Wait for MSSQL server to start
    status1=1
    status2=1

    interval=5
    timeout=60

    i=0

    while [[ $status1 -ne 0 ]] && [[ $status2 -ne 0 ]] && [[ $i -lt timeout ]]; do
        i=$i+$interval
        
        /opt/mssql-tools/bin/sqlcmd -t 1 -U sa -P $SA_PASSWORD -Q "SELECT 1" &> /dev/null
        status1=$?

        /opt/mssql-tools/bin/sqlcmd -t 1 -U $DB_USERNAME -P $DB_PASSWORD -Q "SELECT 1" &> /dev/null
        status2=$?
        
        sleep $interval
    done

    if [[ $status1 -ne 0 ]] && [[ $status2 -ne 0 ]]; then
        echo "Error: MSSQL SERVER took more than ${timeout} seconds to start up."
        exit 1
    fi

    echo "======= MSSQL SERVER STARTED ========"

    if [[ $status2 -ne 0 ]]; then
        # Run the setup script to create the DB and the schema in the DB
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -i $setup_file
    else
        echo "DB is already set, so skipping the setup process"
    fi

    echo "======= MSSQL CONFIG COMPLETE ======="
}

/opt/mssql/bin/sqlservr &
setup_database

wait
