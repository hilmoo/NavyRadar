@echo off
SETLOCAL

SET "PG_USER=postgres"
SET "PG_DBNAME=postgres"
SET "PG_HOST=localhost"
SET "PGPASSWORD=postgres"
SET "PG_PORT=5432"
SET "PSQL_EXE=psql"

echo Starting database migrations for %PG_DBNAME%...
echo ---------------------------------

FOR %%f IN (*.sql) DO (
    echo Applying migration: %%f

    "%PSQL_EXE%" -U %PG_USER% -d %PG_DBNAME% -h %PG_HOST% -p %PG_PORT% -v ON_ERROR_STOP=1 -f "%%f"

    IF ERRORLEVEL 1 (
        echo.
        echo ****************************
        echo * ERROR applying %%f. Aborting.
        echo ****************************
        GOTO :eof
    )
)

echo ---------------------------------
echo All migrations applied successfully.

SET "PGPASSWORD="
ENDLOCAL
echo.
pause
