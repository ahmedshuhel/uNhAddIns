@echo off
ECHO Ejecutando Chinook
call sqlcmd -E -S .\sqlexpress -i ChinookSchema.sql -b -m 1

ECHO Ejecutando Chinook
call sqlcmd -E -S .\sqlexpress -i ChinookData.sql -b -m 1
PAUSE