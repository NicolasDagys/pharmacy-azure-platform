FROM mcr.microsoft.com/mssql/server:2022-latest

USER root

COPY Pharmacy_Schema.sql /scripts/Pharmacy_Schema.sql
COPY Pharmacy_SeedData.sql /scripts/Pharmacy_SeedData.sql
COPY docker/init.sh /scripts/init.sh

RUN chmod +x /scripts/init.sh

CMD /bin/bash /scripts/init.sh & /opt/mssql/bin/sqlservr