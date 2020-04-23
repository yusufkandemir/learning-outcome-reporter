FROM mcr.microsoft.com/mssql/server:2019-latest
EXPOSE 1433

WORKDIR /db

COPY setup.sql .
COPY entrypoint.sh .

ENTRYPOINT ./entrypoint.sh

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=10 \
    CMD /opt/mssql-tools/bin/sqlcmd -S localhost -U ${DB_USERNAME} -P ${DB_PASSWORD} -Q "SELECT 1" || exit 1
