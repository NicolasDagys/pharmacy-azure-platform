#!/bin/bash

echo "Waiting for SQL Server..."

sleep 30

/opt/mssql-tools18/bin/sqlcmd \
-S localhost \
-U sa \
-P "$SA_PASSWORD" \
-C \
-i /scripts/Pharmacy_Schema.sql

/opt/mssql-tools18/bin/sqlcmd \
-S localhost \
-U sa \
-P "$SA_PASSWORD" \
-C \
-i /scripts/Pharmacy_SeedData.sql

echo "Database initialized."